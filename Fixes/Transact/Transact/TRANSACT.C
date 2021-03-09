// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
// Version 1.0          Steve Wright / Paul Bowers    1994,1995,1996,1997
//
//             Initial Release.
//
// Version 2.0               Paul Bowers                    28 August 1998
//             Fix Sales Enquiry Bar Code to Boots Bar Code Conversion
//             In ISE request. Debug for pilot
//
// Version 2.1               Paul Bowers                1st September 1998
//             Include the RFSCF control file in all transactions, record
//             price check activity and quota management processing
//
// Version 2.2               Paul Bowers                24th September 1998
//             Move the file open & close for the PSBt and EALTerms and
//             WORKG files from INformation Sign on / Exit to Store Sales
//             request only. These files are only used by this txn, and
//             normally requested at store close, which is also attempting
//             to rename these files for backup. if the HHT is left in any
//             information screen (inc menu) these files are left open
//             which results in psb52 abending with access conflict.
//             in order to minimise this window of opportunity the files
//             are opened / closed as quick as possible. Timings show
//             this to have no noticable affect on the performance of this
//             transaction.
// Version 2.3               Steve Wright/Paul Bowers      30th October 1998
//    Phase II enhancements
//    Prevent data being sent when dummy (XXX) transaction is transmitted
//    by the sockets server.
//    Add time stamps to message rx/tx debug.
//    Add extra details to PLI record
//    Add extra details to PLE record
//    Remove processing of RFSCF activity flag
//    Add general prep_nak() function to prepare NAK replies (and convert
//    all existing NAK replies) - to simplify code
//    Add extra details to EQR
//    Add Gregorian/Julian date conversion routines
//    Assertions for handling two digit dates:
//        1) Any two digit dates from other files/systems are converted and
//        held internally as four digit dates. The conversion is done by
//        windowing the date such that YY>=85 = 19YY, YY<85 = 20YY.
//        2) When four digit dates have to be written to other files/systems
//        as two digit dates they will be written as YYYY modulo 100 (thus
//        converting YYyy to yy.)
// Version 3.0               Steve Wright                  30th July 2001
//             Phase III enhancements (Symbol Spectrum 24 - DS Spread)
//             Project Envoy - Symbol 1746 / 19xx (Palm OS)
//             Support new PAL/PAR transaction pair - lookup URL info.
// Version 3.2               Steve Wright              30th November 2001
//     Fix PLX transaction. Make list status ' ' if list is exited but
//     not completed.
//     Fix CLI record - parent_code missing
//
// Version 4.0               Steve Wright/Paul Bowers       28th August 2003
//     2003 Trial
//     Update to cope with new deal system
//
// Version 4.0               Paul Bowers                 26th September 2003
//    Various fixes for 2003 trial
//
// Version 4.02
//    RF Trial Phase 2       Paul Bowers                18th February 2004
//    Changes to introduce proximity printing
//    change to SNR and PRT transactions.
//    Introduce the PRTCTL file
//
// Version 4.03
//    Break fix for dissacociate dfile session numbers
//    Introduce the dynamic session number tracking table.
//                           Paul Bowers                6th April 2004
// Version 5.00
//     Introducte functionality for Pre-Scan
//                           Paul Bowers                5th May 2004
//
// Version 6.00              Paul Bowers                5th July 2004
// MyStoreNet Changes to ENQ / ISE and introduce SIE/SIR and blind sign on
// support.
//
// Version 7.00              Paul Bowers                31st August 2004
// Offsite Stock Room Support (OSSR)
// extend the length of the CLILF / PLLOL add new fields to PLLDB RFSCF
//
// Version 8.00              Paul Bowers                 20th October 2004
// Fix TSF update logic in PLC transaction to include date of last
// delivery and return a message to the PPC if the product has been
// delivered in the last INVOK.RP.DAYS days.
//
// Version 9.0               Stuart Highley              26th January 2005
// - Added OSSR WAN functionality.
// - Added Promotions functionality.
// - Added Credit Claiming functionality.
// - Removed incorrect handling of lrt_log return codes.
// - Update 'date of last gap' field of RFHIST if a shelf monitor gap
//   comes through.
// - Reduced stack usage of process() by removing local record structures.
//   This can no doubt be inproved further.
// - Used request structure pointers to improve readability.
// - UPWARDS TSF: Always allow the PLC to adjust a +ve item TSF if the
//   adjustment is also +ve.
//
// Version 10               Paul Bowers                27th November 2006
// - Planners to store changes in A7A
//
// Version 11               Paul Bowrts                23rd May 2007
// - Electronic Recalls A7C
//
// Version 12               Brian Greenfield           11th September 2007
// Unstall printer queue every 15 minutes - new function call to
// unstall_sel_stack().
// Added call to DealEnquiry and moved code to trans03.c because we
// blew 64K limit.
//
// Version 13               Brian Greenfield           2nd November 2007
// Minor correction for price check counts - now tests for no date in
// RFHIST record for new lines that have no previous count date.
//
// Version 14               Brian Greenfield           3rd January 2008
// Added WriteToFile call for WRF command.
//
// Version 15               Brian Greenfield           6th Febuary 2008
// Added Markdown flag passing to EQR.
//
// Version 16               Brian Greenfield           13th August 2008
// Added MSA Multi-Site Request and process_gap function call change.
// Moved LRT_PRT code to new function  trans04_LRT_PRT in trans04.
//
// Version 17               Brian Greenfield           1st Sept 2008
// Added ASN/Directs commands.
//
// Version 18               Brian Greenfield           10th Sept 2008
// Added ALR for MC70 support and asset logging in SOR request.
// Also launch PST47 instead of PSS47 if the device was an MC70. This
// can be detected from the DevType field in the pq.
//
// Version 19               Brian Greenfield           24th Fen 2009
// Changed to that SSC09.286 is launched instead of PSS79.286 for
// directs buffer file processing.
// Also corrected the launch of types CB, PUB, and DIR to correctly
// pass the filename as a logical.
//
// Version 20               Brian Greenfield           4th Mar 2009
// Changed the CHN command within check_command to call new function
// ChainApplication().
//
// Version 21               Brian Greenfield           5th Mar 2009
// Corrected legacy directs buffer logical filename.
// I had been advised wrongly!
//
// Version 22               Brian Greenfield           17th Dec 2009
// Changes for RF Stabilisation. new ANK internal command. This
// command simply converts into a NAK.
//
// Version 23               Charles Skadorwa           15th Apr 2010
// Changes to support POD Logging Enhancements.
//
// Version 24               Charles Skadorwa           12th Mar 2012
// Changes to support Stock File Accuracy  (commented: //CSk 12-03-2012 SFA).
// Added new CLA message to process() function.
// Added new CLD message to process() function.
//
// Version 25               Charles Skadorwa           11th Sep 2012
// Changes to support Stock File Accuracy  (commented: //CSk 11-09-2012 SFA).
// Override System compiler date century to stop it reporting 2012 as 1912.
//
// Version A               Visakha Satya               20th Apr 2015
// SC079 Dallas Positive Receiving
// - The changes to transact are limited to the provision of functions
//   to process the new Dallas Positive receiving process. New messages
//   types will be added in Transact for allowing Dallas UOD's to be
//   positively receipted in stores.
//   The new Dallas Message Types to be added are as follows -
//   DAC - Dallas Positive Receiving Check Message
//   DAL - Pending Dallas PO list request
//   DAD - Pending Dallas PO list Detail response
//   DAE - Pending Dallas PO list End response
//   DAR - Dallas UOD Receipt request
// - Removed the commented out lines in Process function from previous
//   version.
//
// Version B                Ranjith Gopalankutty       07t July  2016
//  Fix to the issue reported Post 16A due to automatic Date & Time 
//  enhancement, changed McLoader was sending request to get the date
//  to Transact exactly at 12.00am. After RS5 project PSB90 would 
//  disable the Transact. As a result NAK response was coming back 
//  to HHT. Change is that Transact will respond back with SNR for
//  dummy sign on request even if it is closed. Created a new function
//  call for the same DummmySignOn()
// -----------------------------------------------------------------------

#include "transact.h" //SDH 19-May-2006

/* include files */

#include "srfiles.h"            //Streamline SDH 17-Sep-2008
#include <string.h>
#include <math.h>
#include "osfunc.h"           /* needed for disp_msg() */
#include "trxutil.h"            // Streamline SDH 22-Sep-2008
#include "trxfile.h"            // Streamline SDH 22-Sep-2008
#include "rfsfile.h"
//#include "wrap.h"
#include "rfglobal.h"           // v4.01
#include "trxpog.h"
#include <time.h>               //11-09-2007 12 BMG
#include "trxbase.h"            //Streamline SDH 17-Sep-2008
//#include "prtctl.h"             //Streamline SDH 17-Sep-2008
//#include "prtlist.h"            //Streamline SDH 17-Sep-2008
#include "idf.h"                //Streamline SDH 17-Sep-2008
#include "isf.h"                //Streamline SDH 17-Sep-2008
#include "rfhist.h"             //Streamline SDH 17-Sep-2008
#include "irf.h"                //Streamline SDH 17-Sep-2008
#include "rfscf.h"              //Streamline SDH 17-Sep-2008
#include "invok.h"              //Streamline SDH 17-Sep-2008
#include "af.h"                 //Streamline SDH 17-Sep-2008
#include "affcf.h"              //SDH 20-May-2009 Model Day
#include "trxuod.h"             //Streamline SDH 17-Sep-2008
#include "trxdeal.h"            //Streamline SDH 17-Sep-2008
#include "trxrcall.h"           //Streamline SDH 17-Sep-2008
#include "sockserv.h"
#include "clfiles.h"            //Streamline SDH 17-Sep-2008
#include "trxGIA.h"             //BMG 01-09-2008 17 ASN/Directs
#include "podok.h"              //CSk 15-04-2010 POD Logging
#include "podlog.h"             //CSk 15-04-2010 POD Logging
#include "trxDAL.h"             //AVS 20-04-2015 Dallas
#include "transmsg.h"           //AVS 20-04-2015 Dallas
#include "trxbase2.h"           //BRG 07-07-2016 Date & time fix

#define WOGAPBF            "D:\\ADX_UDT1\\WOGAPBF.DAT"                              // 13-9-04 pab


//////////////////////////////////////////////////////////////////////////
///                                                                     //
///   Static (private) variables                                        //
///                                                                     //
//////////////////////////////////////////////////////////////////////////

static BOOLEAN status_wk_block = FALSE;                                 // Streamline SDH 23-Sep-2008
static BOOLEAN status_wg_block = FALSE;                                 // Streamline SDH 23-Sep-2008


static void unstall_sel_stack(void) {                                                          // BMG 1.1 11-09-2007

    WORD i;
    static time_t LastTime;
    time_t CurrTime;

    if ( LastTime == 0 ) {
       // Initialise time when we first come in here
       time(&LastTime);
    }

    time(&CurrTime);

    // Check every 15 minutes
    if ( difftime(CurrTime, LastTime) >= 900 ) {
       time(&LastTime);
       for (i = 0; i < MAX_CONC_UNITS; i++) {
           // If the entry is stalled then set it to Ready
           if ( pq[i].state == PST_STALLED ) {
               sprintf(msg, "PQ slot %d file %s stalled "               // Null fix SDH 29-Sep-2009
                       "- setting to ready", i, pq[i].fname);           // Null fix SDH 29-Sep-2009
               disp_msg(msg);                                           // Null fix SDH 29-Sep-2009
               pq[i].state = PST_READY;
           }
           // While we're going through all entries, if an in-progress file was last
           // modified more than an hour ago then set it to Adopted to get it processed
           if ( (pq[i].state == PST_ALLOC) && (difftime(CurrTime, pq[i].last_access_time) >= 3600) ) {
              process_workfile( pq[i].unit, SYS_LAB );
              process_workfile( pq[i].unit, SYS_GAP );
              process_workfile( pq[i].unit, SYS_CB );       //BMG 01-09-2008 17 ASN/Directs
              process_workfile( pq[i].unit, SYS_PUB );      //BMG 01-09-2008 17 ASN/Directs
              process_workfile( pq[i].unit, SYS_DIR );      //BMG 01-09-2008 17 ASN/Directs
           }
       }
       //Call a non-destructive process orphans
       process_orphans(FALSE);
    }
}


static void dump_pq_stack(void){                                       // BMG 1.1 11-09-2007 Moved from rfs.c.

    if (debug) {                                                // SDH 10-May-2006
        disp_msg("Dump of PQ table follows...");                // SDH 10-May-2006
        disp_msg("States: 0=PST_FREE 1=PST_ALLOC "              // SDH 10-May-2006
                 "2=PST_READY 3=PST_ADOPTED "                   // SDH 10-May-2006 // BMG 1.1 11-09-2007
                 "4=PST_RUNNING 5=PST_STALLED");                                   // BMG 1.1 11-09-2007
        disp_msg("Types:  0=SYS_LAB  1=SYS_GAP   2=SYS_ORPHAN " // SDH 10-May-2006
                 "3=SYS_CB  4=SYS_PUB 5=SYS_DIR");                                 //BMG 01-09-2008 17 ASN/Directs
        for (WORD i = 0; i < MAX_CONC_UNITS; i++) {             // SDH 10-May-2006
            sprintf(msg, "Index:%d Name: %s State: %d Type: %d" // SDH 10-May-2006 // BMG 1.1 11-09-2007
                         " Unit: %d Last Access Time: %s",                         // BMG 1.1 11-09-2007
                    i, pq[i].fname, pq[i].state,                // SDH 10-May-2006
                    pq[i].type, pq[i].unit,                     // SDH 10-May-2006 // BMG 1.1 11-09-2007
                    asctime(localtime(&pq[i].last_access_time)));                  // BMG 1.1 11-09-2007
            disp_msg(msg);                                      // SDH 10-May-2006
        }                                                       // SDH 10-May-2006
        disp_msg("End of PQ table dump.");                      // SDH 10-May-2006
    }                                                           // SDH 10-May-2006

}


static void CycleLogs(void) {                                              // BMG 1.1 11-09-2007 Moved from transact.c.

    B_TIME nowTime;                                                 //CSk 15-04-2010 POD Logging
    B_DATE nowDate;                                                 //CSk 15-04-2010 POD Logging
    char cMonth;                                                    //CSk 15-04-2010 POD Logging
    LONG cls_loop;                                                  // BMG 1.1 11-09-2007
    LONG lRc;                                                       //CSk 15-04-2010 POD Logging
    LONG fsz;                                                       //CSk 15-04-2010 POD Logging
    LONG lOutFile;                                                  //CSk 15-04-2010 POD Logging
    UWORD hh_unitx;                                                 // BMG 1.1 11-09-2007

    cMonth = 0x00;                                                  //CSk 15-04-2010 POD Logging

    // Cycle LRTLG
    s_close( 0, lrtlg.fnum );
    s_delete( 0, LRTLGBKP );
    s_rename( 0x2000,
              LRTLG,
              LRTLGBKP );

    // Cycle DBG
    s_close( 0, dbg.fnum );
    s_delete( 0, DBGBKP );
    s_rename( 0x2000,
              DBG,
              DBGBKP );

    // Cycle PODOK                                                     //CSk 15-04-2010 POD Logging
    // Obtain PODOK file date stamp. If the month value is different   //CSk 15-04-2010 POD Logging
    // to the Controller date's month value, then delete the PODBKP    //CSk 15-04-2010 POD Logging
    // file and rename the PODOK to the PODBKP.                        //CSk 15-04-2010 POD Logging
    //Get Controller date & time in string format                      //CSk 15-04-2010 POD Logging
    PodokClose(CL_ALL);                                                //CSk 15-04-2010 POD Logging
    GetSystemDate(&nowTime, &nowDate);                                 //CSk 15-04-2010 POD Logging
                                                                       //CSk 15-04-2010 POD Logging
    fsz = filesize (podok.pbFileName);                                 //CSk 15-04-2010 POD Logging
                                                                       //CSk 15-04-2010 POD Logging
    if (fsz > 0) {                                                     //CSk 15-04-2010 POD Logging
        // File exists - read keyed header to obtain creation month    //CSk 15-04-2010 POD Logging
        // We need to open as DIRECT in order to get Header Info       //CSk 15-04-2010 POD Logging
        lOutFile = s_open (A_READ | A_WRITE, podok.pbFileName);        //CSk 15-04-2010 POD Logging
        lRc = s_read( A_BOFOFF, lOutFile, &cMonth, 1, 32L);            //CSk 15-04-2010 POD Logging
        s_close( 0, lOutFile );                                        //CSk 15-04-2010 POD Logging
                                                                       //CSk 15-04-2010 POD Logging
        if (lRc == 1L) { //Compare PODOK creation & Controller dates   //CSk 15-04-2010 POD Logging

            if (nowDate.wDay == GetDaysInMonth(nowDate.wYear, nowDate.wMonth)) {  //CSk 03-08-2010 Defect 4522
                // Must be last day of the month so Backup PODOK to PODBKP        //CSk 03-08-2010 Defect 4522
                s_delete( 0, PODBKP );                                 //CSk 15-04-2010 POD Logging
                s_rename( 0x2000,                                      //CSk 15-04-2010 POD Logging
                          PODOK,                                       //CSk 15-04-2010 POD Logging
                          PODBKP );                                    //CSk 15-04-2010 POD Logging
                disp_msg("Backed up PODOK");                           //CSk 15-04-2010 POD Logging
            }                                                          //CSk 15-04-2010 POD Logging
        }                                                              //CSk 15-04-2010 POD Logging
    }                                                                  //CSk 15-04-2010 POD Logging
                                                                       //CSk 15-04-2010 POD Logging
    // BACKUP PODLOG                                                   //CSk 15-04-2010 POD Logging
    // Backup the PODLOG file. 6 backups will be kept: PODL1 to PODL6. //CSk 15-04-2010 POD Logging
    // Delete PODL6, then PODL5 is rolled to PODL6, PODL4 is rolled to //CSk 15-04-2010 POD Logging
    // PODL5 etc. until PODLOG is rolled to PODL1 (latest backup).     //CSk 15-04-2010 POD Logging
    s_delete( 0, PODL6 );    // PODL6 -> X                             //CSk 15-04-2010 POD Logging
    s_rename( 0x2000,        // PODL5 ->PODL6                          //CSk 15-04-2010 POD Logging
              PODL5,                                                   //CSk 15-04-2010 POD Logging
              PODL6 );                                                 //CSk 15-04-2010 POD Logging
    s_rename( 0x2000,        // PODL4 ->PODL5                          //CSk 15-04-2010 POD Logging
              PODL4,                                                   //CSk 15-04-2010 POD Logging
              PODL5 );                                                 //CSk 15-04-2010 POD Logging
    s_rename( 0x2000,        // PODL3 ->PODL4                          //CSk 15-04-2010 POD Logging
              PODL3,                                                   //CSk 15-04-2010 POD Logging
              PODL4 );                                                 //CSk 15-04-2010 POD Logging
    s_rename( 0x2000,        // PODL2 ->PODL3                          //CSk 15-04-2010 POD Logging
              PODL2,                                                   //CSk 15-04-2010 POD Logging
              PODL3 );                                                 //CSk 15-04-2010 POD Logging
    s_rename( 0x2000,        // PODL1 -> PODL2                         //CSk 15-04-2010 POD Logging
              PODL1,                                                   //CSk 15-04-2010 POD Logging
              PODL2 );                                                 //CSk 15-04-2010 POD Logging
    PodlogClose(CL_ALL);                                               //CSk 15-04-2010 POD Logging
    s_rename( 0x2000,        // PODLOG -> PODL1                        //CSk 15-04-2010 POD Logging
              PODLOG,                                                  //CSk 15-04-2010 POD Logging
              PODL1 );                                                 //CSk 15-04-2010 POD Logging
    disp_msg("Backed up PODLOG");                                      //CSk 15-04-2010 POD Logging

    // Recreate files
    prepare_logging();

    disp_msg("Logs cycled");

    // PAB 4.03 start

    // Close all files
    CloseAllFiles();

    disp_msg("(CYC) Flushing Orphan file handles");             // 07-04-04 PAB
    for (cls_loop=1; cls_loop<2000;  cls_loop++) {              // 16-03-05 SDH

        if (ftab[cls_loop].fnumx >=1000L) {
            if (debug) {
                sprintf( msg, "Closing orphan :%08lX",
                         ftab[cls_loop].fnumx );
                disp_msg(msg);
            }
            s_close(0,ftab[cls_loop].fnumx );
            ftab[cls_loop].fnumx = 0L;
        }

/*
        if (ftab[cls_loop] != NULL) {                           // 07-04-04 PAB
            if (ftab[cls_loop]->fnumx >=1000L) {                // 07-04-04 PAB
                if (debug) {                                    // 20-05-04 PAB
                    sprintf( msg, "Closing orphan :%08lX", ftab[cls_loop]->fnumx );
                    disp_msg(msg);                              // 20-05-04 PAB
                }                                               // 20-05-04 PAB
                s_close(0,ftab[cls_loop]->fnumx );              // 07-04-04 PAB
                ftab[cls_loop]->fnumx = 0L;                     // 07-04-04 PAB
            }                                                   // 07-04-04 PAB
        }                                                       // 07-04-04 PAB
*/

    }                                                           // 07-04-04 PAB

    // wait 200ms give the os chance purge file buffers         // 07-04-04 PAB
    s_timer(0,200);                                             // 07-04-04 PAB
    process_orphans(TRUE);                                      // SDH 9-May-2006

    //close report files                                        // 07-04-04 PAB
    disp_msg("Closing known open HHT sessions");                // 07-04-04 PAB
    for (hh_unitx=0; hh_unitx<=254; hh_unitx++) {               // 07-04-04 PAB
        if (lrtp[hh_unitx] != NULL) {                           // 07-04-04 PAB
            if (lrtp[hh_unitx]->fnum3!=0L) {                    // 07-04-04 PAB
                disp_msg("close open reports");                 // 07-04-04 PAB
                s_close( 0, lrtp[hh_unitx]->fnum3 );            // 07-04-04 PAB // BMG 1.1 11-09-2007
                lrtp[hh_unitx]->fnum3 = 0L;                     // 07-04-04 PAB
            }                                                   // 07-04-04 PAB
        }                                                       // 07-04-04 PAB
    }                                                           // 07-04-04 PAB

    // 07-04-04 PAB
    dealloc_lrt_table( ALL_UNITS );                             // BMG 1.1 11-09-2007
    sess = 0;
    // 07-04-04 PAB
    disp_msg ("log cycle complete...");                         // 07-04-04 PAB
    // PAb 4.03 end

}


static void process_sel_stack(void) {                                                          // BMG 1.5 01-05-2008

    LONG key,rc;                                                                        // PAB 26-1-2007
    UBYTE started;
    WORD i;
    BYTE fnm[32];
    DISKFILE df[1];                                                                      // PAB 26-1-2007

    started=FALSE;
    for (i = 0; i < MAX_CONC_UNITS && !started; i++) {

        if (( pq[i].state == PST_RUNNING ) && (!semaphore_active(pq[i].type))) {       // pab 26-1-2007
            // ---------------------------------------------------------------------
            // if the program is flagged as submitted but printsel is now not active
            // check to see if the work file is still there.
            // if it is then printsel failed so we try this file again. set status as
            // ready to reque otherwise mark it as competed ok.
            // ---------------------------------------------------------------------
            pq[i].state = PST_READY;                                                   // pab 26-1-2007
            key = 0L;                                                                  // pab 26-1-2007
            rc = s_lookup( T_FILE, A_HIDDEN | A_SYSFILE,                               // pab 26-1-2007
                       pq[i].fname, (BYTE *)df, sizeof(df), sizeof(DISKFILE), key );   // pab 26-1-2007
            // we shall have 5 attempts at each batch                                  // pab 26-1-2007 // 11-09-2007 5.5 BMG
            // then give up until adoption requeues the file                           // pab 26-1-2007
            if (rc == 0) {                                                             // pab 26-1-2007 // 11-09-2007 5.5 BMG
                sprintf(msg, "PQ slot %d file %s no longer running. "                  // Null fix SDH 29-Sep-2009
                        "Setting state to free", i, pq[i].fname);                      // Null fix SDH 29-Sep-2009
                disp_msg(msg);                                                         // Null fix SDH 29-Sep-2009
                pq[i].state = PST_FREE;                                                // pab 26-1-2007
                pq[i].submitcnt = 0;                                                   // pab 26-1-2007
                memset(pq[i].fname, 0x00, sizeof(pq[i].fname));                        // Null fix SDH 29-Sep-2009
                pq[i].last_access_time = 0;                                            // 11-09-2007 5.5 BMG
                pq[i].type = 0;                                                        // 11-09-2007 5.5 BMG
                pq[i].unit = 0;                                                        // 11-09-2007 5.5 BMG
                memset(pq[i].DevType, 0x20, 1);                                        // BMG 10-09-2008 18 MC70
            } else {                                                                   // 11-09-2007 5.5 BMG
                if (pq[i].submitcnt >= 5) {                                            // 11-09-2007 5.5 BMG
                    sprintf(msg, "PQ slot %d file %s no longer running but file "      // Null fix SDH 29-Sep-2009
                            "still present. Setting to stalled.", i, pq[i].fname);     // Null fix SDH 29-Sep-2009
                    disp_msg(msg);                                                     // Null fix SDH 29-Sep-2009
                    pq[i].state = PST_STALLED;                                         // 11-09-2007 5.5 BMG
                    pq[i].submitcnt = 0;                                               // 11-09-2007 5.5 BMG
                } else {                                                               // 11-09-2007 5.5 BMG
                    sprintf(msg, "PQ slot %d file %s no longer running. "              // Null fix SDH 29-Sep-2009
                            "Retrying submit.", i, pq[i].fname);                       // Null fix SDH 29-Sep-2009
                    disp_msg(msg);                                                     // Null fix SDH 29-Sep-2009
                }                                                                      // Null fix SDH 29-Sep-2009
            }                                                                          // 11-09-2007 5.5 BMG
        }                                                                              // psb 26-1-2007

        //If the current element in the queue is ready
        if (pq[i].state == PST_READY || pq[i].state == PST_ADOPTED) {       // SDH 22-June-2006

            // If the appropriate program is not already running
            if (!semaphore_active(pq[i].type)) {

                //Build queue file name into string                         // SDH 25-11-04 OSSR WAN
                strncpy(fnm, pq[i].fname, sizeof(fnm));                     // Null fix SDH 29-Sep-2009
                fnm[sizeof(fnm) - 1] = 0x00;                                // Null fix SDH 29-Sep-2009

                // Start appropriate program as a background task
                switch (pq[i].type) {

                //Case PRINTSEL
                case SYS_LAB:

                    rc = start_background_app("ADX_UPGM:PRINTSEL.286",
                                         fnm, "Starting PRINTSEL" );        // SDH 31-10-2005 BUG FIX
                    // mark as unused/processed
                    if (rc == 0) {                                          // SDH 31-10-2005 BUG FIX
                        sprintf(msg, "Started PRINTSEL: %s from slot %d",   // Null fix SDH 29-Sep-2009
                                fnm, i);                                    // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // SDH 05-01-06 BUG FIX
                        pq[i].state = PST_RUNNING;                          // PAB 26-1-2007
                        pq[i].submitcnt++;                                  // PAB 26-1-2007
                        started = TRUE;
                        status_wk_block = FALSE;
                    } else {
                        sprintf(msg, "Failed to start PRINTSEL: %s from "   // Null fix SDH 29-Sep-2009
                                "slot %d", fnm, i);                         // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // SDH 05-01-06 BUG FIX
                    }
                    break;

                    //Case GAP (PSS47)
                case SYS_GAP:

                    //Only kick off PSS47 if PSB30 isn't running.           // SDH 05-01-06 BUG
                    //Kick off PST47 if the device was an MC70 device       // BMG 10-09-2008 18 MC70
                    if (cStoreClosed == 'N') {                              // SDH 05-01-06 BUG
                        if (memcmp(pq[i].DevType, "M", 1)==0) {             // BMG 10-09-2008 18 MC70
                            rc = start_background_app( "ADX_UPGM:PST47.286",// BMG 10-09-2008 18 MC70
                                                 fnm, "Starting PST47" );   // BMG 10-09-2008 18 MC70
                        } else {                                            // BMG 10-09-2008 18 MC70
                            rc = start_background_app( "ADX_UPGM:PSS47.286",// SDH 25-11-04 OSSR WAN
                                                  fnm, "Starting PSS47" );  // SDH 25-11-04 OSSR WAN
                        }                                                   // BMG 10-09-2008 18 MC70
                        // mark as unused/processed                         // SDH 05-01-06 BUG
                        if (rc == 0) {                                      // SDH 31-10-2005 BUG FIX
                            sprintf(msg, "Started PSx47: %s from slot "     // Null fix SDH 29-Sep-2009
                                    "%d", fnm, i);                          // Null fix SDH 29-Sep-2009
                            disp_msg(msg);                                  // Null fix SDH 29-Sep-2009
                            pq[i].state = PST_FREE;                         // Null fix SDH 29-Sep-2009
                            memset(pq[i].fname, 0x00, sizeof(pq[i].fname)); // Null fix SDH 29-Sep-2009
                            pq[i].submitcnt = 0;                            // 11-09-2007 5.5 BMG
                            pq[i].last_access_time = 0;                     // 11-09-2007 5.5 BMG
                            pq[i].type = 0;                                 // 11-09-2007 5.5 BMG
                            pq[i].unit = 0;                                 // 11-09-2007 5.5 BMG
                            memset(pq[i].DevType, 0x20, 1);                 // BMG 10-09-2008 18 MC70
                            pq[i].last_access_time = 0;                     // 11-09-2007 5.5 BMG
                            started = TRUE;
                            status_wk_block = FALSE;
                        } else {                                            // SDH 31-10-2005 BUG FIX
                            sprintf(msg, "Failed to start PSx47: %s from "  // Null fix SDH 29-Sep-2009
                                    "slot %d", fnm, i);                     // Null fix SDH 29-Sep-2009
                            disp_msg(msg);                                  // Null fix SDH 29-Sep-2009
                        }
                    } else {                                                // SDH 05-01-06 BUG
                        if (memcmp(pq[i].DevType, "M", 1)==0) {             // BMG 10-09-2008 18 MC70
                            sprintf(msg, "PSB30 is running.  "              // BMG 10-09-2008 18 MC70
                                    "Holding queue and PST47 job: %s", fnm);// BMG 10-09-2008 18 MC70
                        } else {                                            // BMG 10-09-2008 18 MC70
                            sprintf(msg, "PSB30 is running.  "              // SDH 05-01-06 BUG
                                    "Holding queue and PSS47 job: %s", fnm);// SDH 05-01-06 BUG
                        }                                                   // BMG 10-09-2008 18 MC70
                        disp_msg(msg);                                      // SDH 05-01-06 BUG
                    }                                                       // SDH 05-01-06 BUG

                    break;
                                                                            // SDH 25-11-04 OSSR WAN
                case SYS_CB:                                                //BMG 01-09-2008 17 ASN/Directs
                    //Rename the temporary file                             //BMG 01-09-2008 17 ASN/Directs
                    sprintf(sbuf, "CB%s", fnm+2);                           // Null fix SDH 29-Sep-2009
                    rc = s_rename(0, fnm, sbuf);                            //BMG 01-09-2008 17 ASN/Directs
                    if (rc<0) {                                             //BMG 01-09-2008 17 ASN/Directs
                        if (debug) {                                        //BMG 01-09-2008 17 ASN/Directs
                            sprintf( msg, "Rename %s to %s failed",         //BMG 01-09-2008 17 ASN/Directs
                                     fnm, sbuf);                            //BMG 01-09-2008 17 ASN/Directs
                            disp_msg(msg);                                  //BMG 01-09-2008 17 ASN/Directs
                        }                                                   //BMG 01-09-2008 17 ASN/Directs
                        break;                                              //BMG 01-09-2008 17 ASN/Directs
                    }                                                       //BMG 01-09-2008 17 ASN/Directs
                    //If rename worked, launch program                      //BMG 01-09-2008 17 ASN/Directs
                    sprintf(sbuf, "CB:%s", fnm+2);                          // Null fix SDH 29-Sep-2009
                    rc = start_background_app( "ADX_UPGM:PSD62.286",        //BMG 01-09-2008 17 ASN/Directs
                                         sbuf, "Starting PSD62" );          //BMG 01-09-2008 17 ASN/Directs
                    if (rc == 0) {                                          //BMG 01-09-2008 17 ASN/Directs
                        sprintf(msg, "Started PSD62: %s from slot "         // Null fix SDH 29-Sep-2009
                                "%d", sbuf, i);                             // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // Null fix SDH 29-Sep-2009
                        pq[i].state = PST_FREE;                             //BMG 01-09-2008 17 ASN/Directs
                        memset(pq[i].fname, 0x00, sizeof(pq[i].fname));     // Null fix SDH 29-Sep-2009
                        pq[i].submitcnt = 0;                                //BMG 01-09-2008 17 ASN/Directs
                        pq[i].last_access_time = 0;                         //BMG 01-09-2008 17 ASN/Directs
                        pq[i].type = 0;                                     //BMG 01-09-2008 17 ASN/Directs
                        pq[i].unit = 0;                                     //BMG 01-09-2008 17 ASN/Directs
                        memset(pq[i].DevType, 0x20, 1);                     //BMG 01-09-2008 17 ASN/Directs
                        pq[i].last_access_time = 0;                         //BMG 01-09-2008 17 ASN/Directs
                        started = TRUE;                                     //BMG 01-09-2008 17 ASN/Directs
                        status_wk_block = FALSE;                            //BMG 01-09-2008 17 ASN/Directs
                    } else {                                                // Null fix SDH 29-Sep-2009
                        sprintf(msg, "Failed to start PSD62: %s from "      // Null fix SDH 29-Sep-2009
                                "slot %d", sbuf, i);                        // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // Null fix SDH 29-Sep-2009
                    }                                                       // Null fix SDH 29-Sep-2009
                    break;                                                  //BMG 01-09-2008 17 ASN/Directs

                case SYS_PUB:                                               //BMG 01-09-2008 17 ASN/Directs
                    //Rename the temporary file                             //BMG 01-09-2008 17 ASN/Directs
                    sprintf(sbuf, "UB%s", fnm+2);                           // Null fix SDH 29-Sep-2009
                    rc = s_rename(0, fnm, sbuf);                            //BMG 01-09-2008 17 ASN/Directs
                    if (rc<0) {                                             //BMG 01-09-2008 17 ASN/Directs
                        if (debug) {                                        //BMG 01-09-2008 17 ASN/Directs
                            sprintf( msg, "Rename %s to %s failed",         //BMG 01-09-2008 17 ASN/Directs
                                     fnm, sbuf);                            //BMG 01-09-2008 17 ASN/Directs
                            disp_msg(msg);                                  //BMG 01-09-2008 17 ASN/Directs
                        }                                                   //BMG 01-09-2008 17 ASN/Directs
                        break;                                              //BMG 01-09-2008 17 ASN/Directs
                    }                                                       //BMG 01-09-2008 17 ASN/Directs
                    //If rename worked, launch program                      //BMG 01-09-2008 17 ASN/Directs
                    sprintf(sbuf, "PUB:%s", fnm+2);                         // Null fix SDH 29-Sep-2009
                    sprintf(msg, "Starting SSC01: %s", sbuf);               //BMG 01-09-2008 17 ASN/Directs
                    disp_msg(msg);                                          //BMG 01-09-2008 17 ASN/Directs
                    rc = start_background_app( "ADX_UPGM:SSC01.286",        //BMG 01-09-2008 17 ASN/Directs
                                         sbuf, "Starting SSC01" );          //BMG 01-09-2008 17 ASN/Directs
                    if (rc == 0) {                                          //BMG 01-09-2008 17 ASN/Directs
                        sprintf(msg, "Started SSC01: %s from "              // Null fix SDH 29-Sep-2009
                                "slot %d", sbuf, i);                        // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // Null fix SDH 29-Sep-2009

                        pq[i].state = PST_FREE;                             //BMG 01-09-2008 17 ASN/Directs
                        memset(pq[i].fname, 0x00, sizeof(pq[i].fname));     // Null fix SDH 29-Sep-2009
                        pq[i].submitcnt = 0;                                //BMG 01-09-2008 17 ASN/Directs
                        pq[i].last_access_time = 0;                         //BMG 01-09-2008 17 ASN/Directs
                        pq[i].type = 0;                                     //BMG 01-09-2008 17 ASN/Directs
                        pq[i].unit = 0;                                     //BMG 01-09-2008 17 ASN/Directs
                        memset(pq[i].DevType, 0x20, 1);                     //BMG 01-09-2008 17 ASN/Directs
                        pq[i].last_access_time = 0;                         //BMG 01-09-2008 17 ASN/Directs
                        started = TRUE;                                     //BMG 01-09-2008 17 ASN/Directs
                        status_wk_block = FALSE;                            //BMG 01-09-2008 17 ASN/Directs
                    } else {
                        sprintf(msg, "Failed to start SSC01: %s from "      // Null fix SDH 29-Sep-2009
                                "slot %d", sbuf, i);                        // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // Null fix SDH 29-Sep-2009
                    }
                    break;                                                  //BMG 01-09-2008 17 ASN/Directs

                case SYS_DIR:                                               //BMG 01-09-2008 17 ASN/Directs
                    //Rename the temporary file                             //BMG 01-09-2008 17 ASN/Directs
                    sprintf(sbuf, "DB%s", fnm+2);                           // Null fix SDH 29-Sep-2009
                    rc = s_rename(0, fnm, sbuf);                            //BMG 01-09-2008 17 ASN/Directs
                    if (rc<0) {                                             //BMG 01-09-2008 17 ASN/Directs
                        if (debug) {                                        //BMG 01-09-2008 17 ASN/Directs
                            sprintf( msg, "Rename %s to %s failed",         //BMG 01-09-2008 17 ASN/Directs
                                     fnm, sbuf);                            //BMG 01-09-2008 17 ASN/Directs
                            disp_msg(msg);                                  //BMG 01-09-2008 17 ASN/Directs
                        }                                                   //BMG 01-09-2008 17 ASN/Directs
                        break;                                              //BMG 01-09-2008 17 ASN/Directs
                    }                                                       //BMG 01-09-2008 17 ASN/Directs
                    //If rename worked, launch program                      //BMG 01-09-2008 17 ASN/Directs
                    sprintf(sbuf, "LDB:%s", fnm+2);                         // Null fix SDH 29-Sep-2009
                    sprintf(msg, "Starting PSS79: %s", sbuf);               //BMG 01-09-2008 17 ASN/Directs
                    disp_msg(msg);                                          //BMG 01-09-2008 17 ASN/Directs
                    rc = start_background_app( "ADX_UPGM:SSC09.286",        //BMG 01-09-2008 17 ASN/Directs //BMG 24-02-2009 19
                                         sbuf, "Starting SSC09" );          //BMG 01-09-2008 17 ASN/Directs //BMG 24-02-2009 19
                    if (rc == 0) {                                          //BMG 01-09-2008 17 ASN/Directs
                        sprintf(msg, "Started SSC09: %s from "              // Null fix SDH 29-Sep-2009
                                "slot %d", sbuf, i);                        // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // Null fix SDH 29-Sep-2009

                        pq[i].state = PST_FREE;                             //BMG 01-09-2008 17 ASN/Directs
                        memset(pq[i].fname, 0x00, sizeof(pq[i].fname));     // Null fix SDH 29-Sep-2009
                        pq[i].submitcnt = 0;                                //BMG 01-09-2008 17 ASN/Directs
                        pq[i].last_access_time = 0;                         //BMG 01-09-2008 17 ASN/Directs
                        pq[i].type = 0;                                     //BMG 01-09-2008 17 ASN/Directs
                        pq[i].unit = 0;                                     //BMG 01-09-2008 17 ASN/Directs
                        memset(pq[i].DevType, 0x20, 1);                     //BMG 01-09-2008 17 ASN/Directs
                        pq[i].last_access_time = 0;                         //BMG 01-09-2008 17 ASN/Directs
                        started = TRUE;                                     //BMG 01-09-2008 17 ASN/Directs
                        status_wk_block = FALSE;                            //BMG 01-09-2008 17 ASN/Directs
                    } else {
                        sprintf(msg, "Failed to start SSC09: %s from "      // Null fix SDH 29-Sep-2009
                                "slot %d", sbuf, i);                        // Null fix SDH 29-Sep-2009
                        disp_msg(msg);                                      // Null fix SDH 29-Sep-2009
                    }
                    break;                                                  //BMG 01-09-2008 17 ASN/Directs
                }
            }                                                               // endif (semaphore active)
        }                                                                   // endif (READY)
    }                                                                       // next i

}


// spool GAPBF wrk file from screen application for OSSR                       13-9-04 PAB
static void spool_workfile() {                                              // 13-9-04 PAB // BMG 1.6 13-08-2008 Moved here from trans02.

    WORD i, found, log_unit, type;                                          // 13-9-04 PAB
    LONG time, rc;                                                          // 13-9-04 PAB
    TIMEDATE now;                                                           // 13-9-04 PAB
    BYTE sbuf[64];                                                          // 13-9-04 PAB

    if (debug) {                                                            // 13-9-04 PAB
        sprintf( msg, "SPOOL GAPBF for OSSR" );                             // 13-9-04 PAB
        disp_msg(msg);                                                      // 13-9-04 PAB
    }                                                                       // 13-9-04 PAB

    // Get current time                                                     // 13-9-04 PAB
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                              // 13-9-04 PAB
    time = now.td_time;                                                     // 13-9-04 PAB

    // Locate a free entry                                                  // 13-9-04 PAB
    found = -1;                                                             // 13-9-04 PAB
    log_unit = 255;                                                         // 13-9-04 PAB
    type = SYS_GAP;                                                         // 13-9-04 PAB

    //Always leave element 0 free, to protect against units using getting   // Null fix SDH 29-Sep-2009
    //confused between their default value of 0, and a real 0               // Null fix SDH 29-Sep-2009
    for (i=1; i<MAX_CONC_UNITS && found==-1; i++) {                         // Null fix SDH 29-Sep-2009

        if (pq[i].state==PST_FREE) {                                        // 13-9-04 PAB

            if (debug) {                                                    // 13-9-04 PAB
                sprintf( msg, "Queueing GAPBF file" );                      // 13-9-04 PAB
                disp_msg(msg);                                              // 13-9-04 PAB
            }                                                               // 13-9-04 PAB

            found = i;                                                      // 13-9-04 PAB
            // Set process queue entry to allocated and fill in table entry,// 13-9-04 PAB
            pq[i].state = PST_ALLOC;                                        // 13-9-04 PAB
            pq[i].unit = log_unit;                                          // 13-9-04 PAB
            pq[i].submitcnt = 0;                                            // 26-1-07 PAB
            memset(pq[i].DevType, 0x52, 1); // set it to "R"                // BMG 10-09-2008 18 MC70
            // Generate a unique filename                                   // 13-9-04 PAB
            sprintf(pq[i].fname, "GAPWK:%06ld.%03d",                        // Null fix SDH 29-Sep-2009
                    (time / 100L) % 999999L, log_unit);                     // Null fix SDH 29-Sep-2009
            pq[i].type = type;                                              // 13-9-04 PAB
            rc = s_rename(0, WOGAPBF, sbuf);                                // 13-9-04 PAB
            if (rc<0) {                                                     // 13-9-04 PAB
                // mark as unused/processed                                 // 13-9-04 PAB
                pq[i].state=PST_FREE;                                       // 13-9-04 PAB
                pq[i].submitcnt = 0;                                        // 11-09-2007 13 BMG
                pq[i].type = 0;                                             // 11-09-2007 13 BMG
                pq[i].unit = 0;                                             // 11-09-2007 13 BMG
                memset(pq[i].DevType, 0x20, 1);                             // BMG 10-09-2008 18 MC70
                memset(pq[i].fname, 0x00, sizeof(pq[i].fname));             // Null fix SDH 29-Sep-2009
                pq[i].last_access_time = 0;                                 // 11-09-2007 13 BMG
                sprintf( msg, "Rename GAPBF failed - SPOOL ABORTED" );      // 13-9-04 PAB
                disp_msg(msg);                                              // 13-9-04 PAB
            } else {                                                        // 13-9-04 PAB
                // rename succeeded so set state to process in queue        // 13-9-04 PAB
                pq[i].state = PST_READY;                                    // 13-9-04 PAB
                sprintf(msg, "Rename GAPBF to %s (slot %d) OK", sbuf, i);   // SDH Bug fix 3-Oct-2006
                disp_msg(msg);                                              // 13-9-04 PAB
            }                                                               // 13-9-04 PAB

        }                                                                   // 13-9-04 PAB
    }                                                                       // 13-9-04 PAB
}                                                                           // 13-9-04 PAB


// Wait 100mS for a command from the external comms pipe
static WORD check_command()                                                 // BMG 1.6 13-08-2008 Moved here from trans02.
{
    static LONG emask_t = 0L, emask_p = 0L;
    static BYTE cbuf[32];
    //WORD i, p;
    LONG emask, event, rc;
    UWORD hh_unitx;
    LONG cls_loop;

/* SDH 09-03-2005  Commented out the following code since it don't work!
    TIMEDATE start, now;

    if (rfsstat.sessions>0) {
        // Log status info to shared memory

        //Reset 'now' to avoid infinite loop on shared memory errors        //SDH 09-03-2005
        memset(&now, 0x00, sizeof(now));                                    //SDH 09-03-2005

        // Log time, to prevent hang in this non-critical code
        s_get(T_TD, 0L, (void *)&start, TIMESIZE);

        // Lock semaphore
        do {
            rc = s_read( 0, rfsstat.fnum, "", 1, 1 );
            if (rc<0L) {
                if ((rc&0xFFFFL)!=0x4001L) {
                    log_event101(rc, 0, __LINE__);
                } else {
                    s_timer( 0, 200 );
                    s_get(T_TD, 0L, (void *)&now, TIMESIZE);
                }
            }
        } while (rc!=0L && (start.td_time+0 <= now.td_time ));

        if (rc==0L) {
            for (i=0, p=0; i<MAX_UNIT; i++) {
                if (lrtp[i]!=NULL && p<MAX_UNIT) {
                    memcpy( (BYTE *)&(share[p].state),
                            (BYTE *)(&(lrtp[i]->state)), 1 );
                    memcpy( (BYTE *)&(share[p].last_active_time),
                            (BYTE *)(&(lrtp[i]->last_active_time)), 4 );
                    memcpy( (BYTE *)&(share[p].user),
                            (BYTE *)(&(lrtp[i]->user)), 3 );
                    memcpy( (BYTE *)&(share[p].txn),
                            (BYTE *)(&(lrtp[i]->txn)), 3 );
                    memcpy( (BYTE *)&(share[p].unique),
                            (BYTE *)(&(lrtp[i]->unique)), 5 );
                    p++;
                }
            }
            if (p<MAX_UNIT) {
                memset( (BYTE *)&share[p], 0x00, sizeof(SHARE_STAT_REC) );
            }

            // Unlock semaphore
            do {
                rc = s_write( 0, rfsstat.fnum, "", 1, 1 );
                if (rc<0L) {
                    if ((rc&0xFFFFL)!=0x4001L) {
                        log_event101(rc, 0, __LINE__);
                    } else {
                        s_timer( 0, 200 );
                        s_get(T_TD, 0L, (void *)&now, TIMESIZE);
                    }
                }
            } while (rc!=0L && (start.td_time+0 <= now.td_time ));

        }

        // If timeout occured then disable shared memory updates
        if (rc!=0L) {
            disp_msg("SHMEM Timeout occured - disabling");
            log_event101(0x01010101, 0, __LINE__);
            close_rfsstat( CL_ALL );
        }

    }
*/

    if (emask_p==0L) {
        // Initiate pipe read interrupt
        //emask_p = e_read( (far LONG (*)())0, A_FLUSH + A_FPOFF,
        //                  cpipe.fnum, cbuf, CPIPE_RECL, 0L );
        emask_p = e_read( (LONG (*)())0, A_FLUSH + A_FPOFF,
                          cpipe.fnum, cbuf, CPIPE_RECL, 0L );
        if (emask_p<=0L) {
            log_event101(emask_p, CPIPE_REP, __LINE__);
            if (debug) {
                sprintf( msg, "Err-AsyncR CPIPE. RC:%08lX",
                         emask_p );
                disp_msg(msg);
            }
        }
    }

    if (emask_t==0L) {
        // Initiate timer interrupt
        //emask_t = e_timer( (far LONG (*)())0, 0, CPIPE_TIMEOUT );
        emask_t = e_timer( (LONG (*)())0, 0, CPIPE_TIMEOUT );
        if (emask_t<=0L) {
            log_event101(emask_t, CPIPE_REP, __LINE__);
            if (debug) {
                sprintf( msg, "Err-AsyncR CPIPE. RC:%08lX",
                         emask_t );
                disp_msg(msg);
            }
        }
    }

    emask = emask_t | emask_p;
    event = s_wait(emask);

    // An event has occurred
    // Timeout ?
    if ((event & emask_t)!=0L) {
        rc = s_return(emask_t);
        emask_t=0L;                                                         // Enable next timer event
    }

    // Command ?
    if ((event & emask_p)!=0L) {
        rc = s_return(emask_p);
        if (rc<=0L) {
            log_event101(rc, CPIPE_REP, __LINE__);
            if (debug) {
                sprintf( msg, "Err-R CPIPE. RC:%08lX", rc );
                disp_msg(msg);
            }
        } else {

            // Debug mode controls
            if (strncmp(cbuf, "DBG", 3)==0) {
                if (strncmp(cbuf+4, "Y", 1)==0) {
                    oput = DBG_LOCAL;
                    debug = TRUE;
                    disp_msg("Debug ON");
                } else if (strncmp(cbuf+4, "F", 1)==0) {
                    oput = DBG_FILE;
                    debug = TRUE;
                    disp_msg("Debug ON");
                } else {
                    debug = FALSE;
                    disp_msg("Debug OFF");
                }
            }
            // if spool gapbf file from screen user
            if (strncmp(cbuf, "SPL", 3)==0) {                               // 13-9-04 PAB
                spool_workfile();                                           // 13-9-04 PAB
                emask_p=0L;                                                 // Enable next pipe read event
            }                                                               // 13-9-04 PAB

            if (strncmp(cbuf, "REC", 3)==0) {                               // 24-5-2007 PAB recalls
                StopRecalls();                                              // 24-5-2007 PAB recalls
                emask_p=0L;                                                 // 24-5-2007 PAB recalls
            }

            // Chain application
            if (strncmp(cbuf, "CHN", 3) == 0) {                             // 14-04-05 SDH
                //PINFO ProcInfo;                                             // 14-04-05 SDH // 04-03-2009 BMG
                //memset(&ProcInfo, 0, sizeof(ProcInfo));                     // 14-04-05 SDH // 04-03-2009 BMG
                //memcpy(ProcInfo.pi_pname, "TRANSACT  ", 10);                // 14-04-05 SDH // 04-03-2009 BMG
                //ProcInfo.pi_prior = 200;                                    // 14-04-05 SDH // 04-03-2009 BMG
                //disp_msg("Chaining application...");                        // 14-04-05 SDH // 04-03-2009 BMG
                //background_msg("Chaining application...");                  // 14-04-05 SDH // 04-03-2009 BMG
                //ShutDownAllSockets();                                       // 14-04-05 SDH // 04-03-2009 BMG
                //CloseAllFiles();                                            // 14-04-05 SDH // 04-03-2009 BMG

                //s_close(0, cpipe.fnum);     //Close comms pipe              // 14-04-05 SDH // 04-03-2009 BMG
                //RfscfClose( CL_ALL );                                       // 09-01-07 PAB // 04-03-2009 BMG
                //s_close( 0, lrtlg.fnum );                                   // 22-02-07 PAB // 04-03-2009 BMG
                //s_close(0, dbg.fnum);                                       // 09-01-07 PAB // 04-03-2009 BMG
                //s_timer(0, 2000);                                           // 14-04-05 SDH // 04-03-2009 BMG
                //s_command(A_CHAIN, "ADX_UPGM:TRANSACT.286", "", 0,          // 14-04-05 SDH // 04-03-2009 BMG
                //          &ProcInfo);                                       // 14-04-05 SDH // 04-03-2009 BMG

                ChainApplication();                                                           // 04-03-2009 BMG
            }                                                               // 14-04-05 SDH

            // close all report files for rfmaint mainteance                // 04-11-04 PAB
            if (strncmp(cbuf, "CLR", 3) == 0) {                             // 04-11-04 PAB
                close_rfrdesc( CL_ALL );                                    // 11-11-04 PAB
                for (hh_unitx=0; hh_unitx<=254; hh_unitx++) {               // 04-11-04 PAB
                    if (lrtp[hh_unitx] != NULL) {                           // 04-11-04 PAB
                        if (lrtp[hh_unitx]->fnum3!=0L) {                    // 04-11-04 PAB
                            if (debug) {                                    // 04-11-04 PAB
                                disp_msg("close open reports");             // 04-11-04 PAB
                            }                                               // 04-11-04 PAB
                            rc = s_close( 0, lrtp[hh_unitx]->fnum3 );       // 04-11-04 PAB
                            lrtp[hh_unitx]->fnum3 = 0L;                     // 04-11-04 PAB
                        }                                                   // 04-11-04 PAB
                    }                                                       // 04-11-04 PAB
                }                                                           // 04-11-04 PAB

                if (debug) disp_msg("(CLR) Closing Open Report handles");   // 11-11-04 PAB
                for (cls_loop=1; cls_loop<2000;  cls_loop++) {              // 16-03-05 SDH

                    if ((ftab[cls_loop].fnumx >=1000L) &&                   // 12-04-05 SDH
                        (ftab[cls_loop].ftype[0] == 'R')) {                 // 12-04-05 SDH
                        if (debug) {                                        // 12-04-05 SDH
                            sprintf( msg, "Closing report :%08lX",          // 12-04-05 SDH
                                     ftab[cls_loop].fnumx );                // 12-04-05 SDH
                            disp_msg(msg);                                  // 12-04-05 SDH
                        }                                                   // 12-04-05 SDH
                        rc = s_close(0,ftab[cls_loop].fnumx );              // 12-04-05 SDH
                        ftab[cls_loop].fnumx = 0L;                          // 12-04-05 SDH
                    }                                                       // 12-04-05 SDH

/*
                    if (ftab[cls_loop] != NULL) {                           // 11-11-04 PAB
                        if (debug) {                                        // 06-04-05 SDH
                            disp_msg("    Found pointer...");               // 06-04-05 SDH
                            dump((void*)&ftab[cls_loop], 4);                // 06-04-05 SDH
                        }                                                   // 06-04-05 SDH
                        if (ftab[cls_loop]->fnumx >=1000L &&                // 12-11-04 PAB
                            (memcmp(ftab[cls_loop]->ftype ,"R",1)==0)) {    // 12-11-04 PAB
                            if (debug) {                                    // 11-11-04 PAB
                                sprintf( msg, "Closing report :%08lX",      // 11-11-04 PAB
                                         ftab[cls_loop]->fnumx );           // 11-11-04 PAB
                                disp_msg(msg);                              // 11-11-04 PAB
                            }                                               // 11-11-04 PAB
                            rc = s_close(0,ftab[cls_loop]->fnumx );         // 11-11-04 PAB
                            ftab[cls_loop]->fnumx = 0L;                     // 11-11-04 PAB
                        }                                                   // 11-11-04 PAB
                    }                                                       // 11-11-04 PAB
*/

                }                                                           // 11-11-04 PAB

                disp_msg("Finished close open reports");                    // 04-11-04 PAB
                emask_p=0L;                                                 // Enable next pipe read event
                return 0;                                                   // 04-11-04 PAB
            }

            // Clean up application
            if (strncmp(cbuf, "CLS", 3)==0) {

                disp_msg("Closing sessions");

                // Close all files
                CloseAllFiles();

                disp_msg("(CLS) Flushing Orphan file handles");             // 20-05-04 PAB

                for (cls_loop=1; cls_loop<2000;  cls_loop++) {              // 12-04-05 SDH
                    if (ftab[cls_loop].fnumx >=1000L) {                     // 12-04-05 SDH
                        if (debug) {                                        // 12-04-05 SDH
                            sprintf( msg, "Closing orphan :%08lX",          // 12-04-05 SDH
                                     ftab[cls_loop].fnumx );                // 12-04-05 SDH
                            disp_msg(msg);                                  // 12-04-05 SDH
                        }                                                   // 12-04-05 SDH
                        rc = s_close(0,ftab[cls_loop].fnumx );              // 12-04-05 SDH
                        ftab[cls_loop].fnumx = 0L;                          // 12-04-05 SDH
                    }                                                       // 12-04-05 SDH
                }                                                           // 12-04-05 SDH

                // give the os time to catch up purging buffers             // 20-05-04 PAB
                s_timer(0,200);                                             // 20-05-04 PAB
                disp_msg("Flushing Complete");                              // 20-05-04 PAB

                //close report files                                        // 20-04-04 PAB
                disp_msg("Closing known open HHT sessions");                // 20-04-04 PAB

                // 20-05-04 PAB
                for (hh_unitx=0; hh_unitx<=254; hh_unitx++) {               // 20-05-04 PAB
                    if (lrtp[hh_unitx] != NULL) {                           // 20-05-04 PAB
                        if (lrtp[hh_unitx]->fnum3!=0L) {                    // 20-05-04 PAB
                            if (debug) {                                    // 20-05-04 PAB
                                disp_msg("close open reports");             // 20-05-04 PAB
                            }                                               // 20-05-04 PAB
                            rc = s_close( 0, lrtp[hh_unitx]->fnum3 );       // 20-05-04 PAB
                            lrtp[hh_unitx]->fnum3 = 0L;                     // 20-05-04 PAB
                        }                                                   // 20-05-04 PAB
                    }                                                       // 20-05-04 PAB
                }                                                           // 20-05-04 PAB

                // Read INVOK record
                InvokOpen();
                rc = InvokRead(0L, __LINE__);
                InvokClose( CL_SESSION );

                // Invalidate all current sessions
                dealloc_lrt_table( ALL_UNITS );
                sess = 0;

                // Rebuild the PQ table completely -- picking up any orphans and
                // flagging them for processing
                process_orphans(TRUE);

                disp_msg("CLS Complete");

            }

            // Cycle logs
            if (strncmp(cbuf, "CYC", 3)==0) {
               CycleLogs();                                                 // 11-09-2007 13 BMG
            }

            // Dump PQ (SEL stack)                                          // SDH 10-May-2006
            if (strncmp(cbuf, "DPQ", 3) == 0) {                             // SDH 10-May-2006
                dump_pq_stack();                                            // 11-09-2007 13 BMG
            }                                                               // SDH 10-May-2006

            // Do a Auto Fast Fill PSS10 run now                            // SDH 20-May-2009 Model
            if (strncmp(cbuf, "AFN", 3)==0) {                               // SDH 20-May-2009 Model
                AffcfRunNow();                                              // SDH 20-May-2009 Model
            }                                                               // SDH 20-May-2009 Model

            // Reload the Auto Fast Fill Control File                       // SDH 20-May-2009 Model
            if (strncmp(cbuf, "UAF", 3)==0) {                               // SDH 20-May-2009 Model
                AffcfProcess();                                             // SDH 20-May-2009 Model
            }                                                               // SDH 20-May-2009 Model

            // Report version
            if (strncmp(cbuf, "VER", 3)==0) {

                sprintf(msg, "%s", RFS_VER);
                disp_msg(msg);
                sprintf(msg, "DATE    : %s", RFS_DATE);
                memset(msg+17, 0x32, 1); // Override 20th Century           //CSk 11-09-2012 SFA
                memset(msg+18, 0x30, 1); // with 21st century               //CSk 11-09-2012 SFA
                disp_msg(msg);

            }

        }
        emask_p=0L;                                                         // Enable next pipe read event
    }

    return 0;

}

// process - reply to an inbound transaction
//
// INPUT:
// inbound = transaction request
// nbytes  = length of request
//
// OUTPUT:
// inbound = transaction response
// nbytes  = length of response
//
int process(char* inbound, int* nbytes) {

    static LONG lcmd;
    static BYTE ts[32];                                                     // 20-02-04 PAB
    static TIMEDATE now;
    time_t CurrTime;                                                        //11-09-2007 12 BMG
    int iNewMess = 0;                                                       //BMG 17-12-2009 22 RF Stabilisation
    int iNewMessLen=0;                                                      //BMG 17-12-2009 22 RF Stabilisation
    BYTE abHeader[5];                                                       //BMG 17-12-2009 22 RF Stabilisation
    WORD i = 0;                                                             //BMG 17-12-2009 22 RF Stabilisation

    static WORD cnt = 0;
    int rc = 0;


    hh_unit = (UWORD)CurrentHHT;
    if (lrtp[hh_unit] != NULL) {                                            //11-09-2007 12 BMG
       time(&CurrTime);                                                     //11-09-2007 12 BMG
       pq[lrtp[hh_unit]->pq_sub1].last_access_time = CurrTime;              //11-09-2007 12 BMG
       pq[lrtp[hh_unit]->pq_sub2].last_access_time = CurrTime;              // Null fix SDH 29-Sep-2009
       pq[lrtp[hh_unit]->pq_CB].last_access_time = CurrTime;                // Null fix SDH 29-Sep-2009
       pq[lrtp[hh_unit]->pq_PUB].last_access_time = CurrTime;               // Null fix SDH 29-Sep-2009
       pq[lrtp[hh_unit]->pq_DIR].last_access_time = CurrTime;               // Null fix SDH 29-Sep-2009
    }

    // Clear output buffer
    memset( out, 0x20, sizeof(out) );                                       // Null fix SDH 29-Sep-2009
    out_lth = 0;

    //Determine if we have a new message type with a header block          //BMG 17-12-2009 22 RF Stabilisation
    if (inbound[0] == 0xFF) {                                              //BMG 17-12-2009 22 RF Stabilisation
        iNewMess = 1;                                                      //BMG 17-12-2009 22 RF Stabilisation
        memcpy(abHeader, (BYTE *)inbound, 5);                              //BMG 17-12-2009 22 RF Stabilisation
        iNewMessLen = satoi((BYTE *)inbound+1, 4) - 5;                     //BMG 17-12-2009 22 RF Stabilisation
        //Strip out header block                                           //BMG 17-12-2009 22 RF Stabilisation
        *nbytes-=5;                                                        //BMG 17-12-2009 22 RF Stabilisation
        memcpy ((BYTE *)inbound, (BYTE *)inbound+5, *nbytes);              //BMG 17-12-2009 22 RF Stabilisation
        memset((BYTE *)inbound+(*nbytes), 0, 5); // Clear out last 5       //BMG 17-12-2009 22 RF Stabilisation
    } else {                                                               //BMG 17-12-2009 22 RF Stabilisation
        iNewMess = 0;                                                      //BMG 17-12-2009 22 RF Stabilisation
        iNewMessLen = 0;                                                   //BMG 17-12-2009 22 RF Stabilisation                                                                      //BMG 17-12-2009 22 RF Stabilisation
    }                                                                      //BMG 17-12-2009 22 RF Stabilisation

    // Get a pointer to the command string
    lcmd = (*(LONG*)inbound) & 0x00FFFFFFL;

    //Dump any received command to the trace file/console
    if (debug && (lcmd != CMD_XXX)) {
        form_timestamp( ts, sizeof(ts) );
        sprintf(msg, "hh_unit = %d", hh_unit);
        disp_msg(msg);
        sprintf(msg, "RX (%s) ", ts);
        disp_msg(msg);
        //Output header of new messages                                     //BMG 17-12-2009 22 RF Stabilisation
        if (iNewMess) { dump(abHeader, 5); }                                //BMG 17-12-2009 22 RF Stabilisation
        dump((BYTE *)inbound, *nbytes); //Dump all data received            //BMG 17-12-2009 22 RF Stabilisation
    }

    // Log shared tracking info
    if ((lcmd != CMD_SOR) && (lcmd != CMD_XXX)) {
        if (lrtp[hh_unit]!=NULL) {
            if (lrtp[hh_unit]->state != ST_FREE) {
                // Log current time
                s_get( T_TD, 0L, (void *)&now, TIMESIZE );
                lrtp[hh_unit]->last_active_time = now.td_time;
                memcpy( (BYTE *)&(lrtp[hh_unit]->txn), inbound, 3 );
                memset( (BYTE *)&(lrtp[hh_unit]->unique), 0x00, 5 );
            }
        }
    }

    // to prevent Priv Except potential
    // from null / corrupt CMDS being
    // received.
    if (lcmd != CMD_NULL) {                                                 // 15-7-2004 PAB

        //Prepare a default NAK to prevent client hangs on coding error     // Streamline SDH 24-Sep-2008
        if (lcmd != CMD_XXX) {                                              // Streamline SDH 24-Sep-2008
            prep_nak("ERRORDid not format a response. Please phone help desk.");// Streamline SDH 24-Sep-2008
        }                                                                   // Streamline SDH 24-Sep-2008

        switch (lcmd) {

        case CMD_SOR:                                                       // Master sign on request
            if (iNewMess && (iNewMessLen != CMD_SOR_LTH1 && iNewMessLen != CMD_SOR_LTH2 && iNewMessLen != CMD_SOR_LTH3)) { //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
                   
                if  (memcmp(inbound+3,"XXXXXX006",9) == 0){                 //BRG 07-07-2016 Fix for Automatic date & time

                     DummySignOn(inbound,*nbytes);                          //BRG 07-07-2016 Fix for Automatic date & time

                } else {                                                    //BRG 07-07-2016 Fix for Automatic Date & time

                    SignOn(inbound, *nbytes);
                }                                                           //BMG 17-12-2009 22 RF Stabilisation
            }
            break;
        case CMD_OFF:                                                       // Master log off
            if (iNewMess && (iNewMessLen != CMD_OFF_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               SignOff(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_GAS:                                                       // Gap monitor sign on
            if (iNewMess && (iNewMessLen != CMD_GAS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               GapSignOn(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_SIE:                                                       // return current store number                                // 7-7-04 PAB
            if (iNewMess && (iNewMessLen != CMD_SIE_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               GetStoreNumber(inbound);                                     // Streamline SDH 17-Sep-2008
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // 7-7-04 PAB
        case CMD_PCM:                                                       // Price mismatch
            if (iNewMess && (iNewMessLen != CMD_PCM_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               PriceMismatch(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_GAP:                                                       // Gap monitor - gap notification
            if (iNewMess && (iNewMessLen != CMD_GAP_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               GapNotification(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_GAX:                                                       // General item lookup
            if (iNewMess && (iNewMessLen != CMD_GAX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               GapSignOff(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_ENQ:                                                       // General - item lookup
            if (iNewMess && (iNewMessLen != CMD_ENQ_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               ItemEnquiry(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PRT:                                                       // General - print shelf-edge label
            if (iNewMess && (iNewMessLen != CMD_PRT_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               PrintSEL(inbound);                                           // BMG 13-Aug-2008 16
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PLO:                                                       // Picking list log on
            if (iNewMess && (iNewMessLen != CMD_PLO_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
               PListSignOn(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PLR:                                                       // Picking list get list of lists
            if (iNewMess && (iNewMessLen != CMD_PLR_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PListGetListOfLists(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PLS:                                                       // Picking list get List
            if (iNewMess && (iNewMessLen != CMD_PLS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PListGetList(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PLC:                                                       // Picking list update item
            if (iNewMess && (iNewMessLen != CMD_PLC_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PListUpdateItem(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PLX:                                                       // Picking list sign off
            if (iNewMess && (iNewMessLen != CMD_PLX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PListSignOffList(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PLF:                                                       // Picking list sign off
            if (iNewMess && (iNewMessLen != CMD_PLF_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PListSignOffSession(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PCS:                                                       // Price Check sign on
            if (iNewMess && (iNewMessLen != CMD_PCS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PriceCheckSignOn(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_PCX:                                                       // Price check exit
            if (iNewMess && (iNewMessLen != CMD_PCX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PriceCheckSignOff(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_INS:                                                       // Information sign on
            if (iNewMess && (iNewMessLen != CMD_INS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              InfoSignOn(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_INX:                                                       // Information exit
            if (iNewMess && (iNewMessLen != CMD_INX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              InfoSignOff(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_SSE:                                                       // Information - store sales
            if (iNewMess && (iNewMessLen != CMD_SSE_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              InfoStoreSales(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_ISE:                                                       // Information - item sales
            if (iNewMess && (iNewMessLen != CMD_ISE_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              InfoItemSales(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_RPO:                                                       // Reports - sign on
            if (iNewMess && (iNewMessLen != CMD_RPO_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ReportSignOn(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_RLE:                                                       // Reports - get reports
            if (iNewMess && (iNewMessLen != CMD_RLE_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ReportGetReport(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_RLS:                                                       // Reports - get level 0s
            if (iNewMess && (iNewMessLen != CMD_RLS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ReportGetTopLevel(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_RPS:                                                       // Reports - get level 1+
            if (iNewMess && (iNewMessLen != CMD_RPS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ReportGetSpecificLevel(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_RPX:                                                       // Reports exit
            if (iNewMess && (iNewMessLen != CMD_RPX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ReportSignOff(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_CLO:                                                       // Counting list log on
            if (iNewMess && (iNewMessLen != CMD_CLO_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              CountsSignOn(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_CLR:                                                       // Counting - Get List Of Lists
            if (iNewMess && (iNewMessLen != CMD_CLR_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              CountsGetListOfLists(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_CLS:                                                       // Counting - Get List
            if (iNewMess && (iNewMessLen != CMD_CLS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              CountsGetList(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_CLA:                                                       // Counting - Create List User Request
            if (iNewMess && (iNewMessLen != CMD_CLA_LTH)) {                 //CSk 12-03-2012 SFA
                prep_nak("Invalid Message Length");                         //CSk 12-03-2012 SFA
            } else {                                                        //CSk 12-03-2012 SFA
              CreateCountList(inbound);                                     //CSk 12-03-2012 SFA
            }                                                               //CSk 12-03-2012 SFA
            break;
        case CMD_CLC:                                                       // Counting - List Update Item
            if (iNewMess && (iNewMessLen != CMD_CLC_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              CountsUpdateItem(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_CLD:                                                       // Counting - Create List Item Request
            if (iNewMess && (iNewMessLen != CMD_CLD_LTH)) {                 //CSk 12-03-2012 SFA
                prep_nak("Invalid Message Length");                         //CSk 12-03-2012 SFA
            } else {                                                        //CSk 12-03-2012 SFA
              CreateListItem(inbound);                                      //CSk 12-03-2012 SFA
            }                                                               //CSk 12-03-2012 SFA
            break;
        case CMD_CLX:                                                       // Counting - List Sign Off and Commit
            if (iNewMess && (iNewMessLen != CMD_CLX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              CountsSignOffList(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_CLF:                                                       // Counting - Sign Off
            if (iNewMess && (iNewMessLen != CMD_CLF_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              CountsSignOffSession(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;
        case CMD_SUS:                                                       // Suspend Transaction
            suspend_transaction(inbound);                                   // 24-05-2007 PAB
            break;
        case CMD_UOS:
            if (iNewMess && (iNewMessLen != CMD_UOS_LTH)) {                //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                        //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                       //BMG 17-12-2009 22 RF Stabilisation
              UODStart(inbound);                                            // SDH 26-11-04 CREDIT CLAIM
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_UOA:                                                       // SDH 26-11-04 CREDIT CLAIM
            if (iNewMess && (iNewMessLen != CMD_UOA_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              UODAdd(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_UOX:                                                       // SDH 26-11-04 CREDIT CLAIM
            if (iNewMess && (iNewMessLen != CMD_UOX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              UODExit(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_DSS:                                                       // SDH 26-11-04 CREDIT CLAIM
            if (iNewMess && (iNewMessLen != CMD_DSS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              UODDirectSupplierListStart(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_DSG:                                                       // SDH 26-11-04 CREDIT CLAIM
            if (iNewMess && (iNewMessLen != CMD_DSG_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              UODGetDirectSupplierList(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_UOQ:                                                       // SDH 26-11-04 CREDIT CLAIM
            if (iNewMess && (iNewMessLen != CMD_UOQ_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              UODLabelEnquiry(inbound);
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_STQ:                                                       // SDH 26-11-04 CREDIT CLAIM
            if (iNewMess && (iNewMessLen != CMD_STQ_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              UODStockTakeQuery(inbound);                                   // Streamline SDH 21-Sep-2008
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 26-11-04 CREDIT CLAIM
        case CMD_DNQ:                                                       // SDH 15-12-04 PROMOTIONS
            if (iNewMess && (iNewMessLen != CMD_DNQ_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              DealEnquiry(inbound);                                         // 11-09-2007 12 BMG
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          // SDH 15-12-04 PROMOTIONS
        case CMD_PGS:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PGS_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogSessionStart(inbound);                                     //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PGX:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PGX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogSessionExit(inbound);                                      //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PGF:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PGF_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogLoadFamilies(inbound);                                     //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PGQ:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PGQ_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogListQuery(inbound);                                        //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PGM:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PGM_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogListModules(inbound);                                      //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PSL:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PSL_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogShelfLoadRequest(inbound);                                 //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PGL:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PGL_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogItemEnq(inbound);                                          //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_PGA:                                                       //SDH 20-May-2009 Model Day
            if (iNewMess && (iNewMessLen != CMD_PGA_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogSiteEnq(inbound);                                          //SDH 20-May-2009 Model Day
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 20-May-2009 Model Day
        case CMD_PRP:                                                       //SDH 14-Sep-2006 Planners
            if (iNewMess && (iNewMessLen != CMD_PRP_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PogPrint(inbound);                                            //SDH 14-Sep-2006 Planners
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //SDH 14-Sep-2006 Planners
        case CMD_RCA:                                                       //PAB 24-May-2007 Recalls
            if (iNewMess && (iNewMessLen != CMD_RCA_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              RecallStart(inbound);                                         //PAB 24-May-2007 Recalls
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //PAB 24-May-2007 Recalls
        case CMD_RCB:                                                       //PAB 24-May-2007 Recalls
            if (iNewMess && (iNewMessLen != CMD_RCB_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              RecallExit(inbound);                                          //PAB 24-May-2007 Recalls
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //PAB 24-May-2007 recalls
        case CMD_RCD:                                                       //PAB 24-May-2007 Recalls
            if (iNewMess && (iNewMessLen != CMD_RCD_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              RecallListRequest(inbound);                                   //PAB 24-May-2007 Recalls
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //PAB 24-May-2007 recalls
        case CMD_RCG:                                                       //PAB 24-May-2007 Recalls
            if (iNewMess && (iNewMessLen != CMD_RCG_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              RecallCount(inbound);                                         //PAB 24-May-2007 Recalls
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //PAB 24-May-2007 Recalls
        case CMD_RCH:                                                       //PAB 24-May-2007 Recalls
            if (iNewMess && (iNewMessLen != CMD_RCH_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              RecallSelectList(inbound);                                    //PAB 24-May-2007 Recalls
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //PAB 24-May-2007 recalls
        case CMD_RCI:                                                       //PAB 24-May-2007 recalls
            if (iNewMess && (iNewMessLen != CMD_RCI_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              RecallInstructions(inbound);                                  //PAB 24-May-2007 recalls
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //PAB 24-May-2007 recalls
        case CMD_WRF:                                                       //BMG 03-Jan-2008 14
            WriteToFile(inbound);                                           //BMG 03-Jan-2008 14
            break;                                                          //BMG 03-Jan-2008 14
        case CMD_MSA:                                                       //BMG 13-Aug-2008 16
            if (iNewMess && (iNewMessLen != CMD_MSA_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              PlistGetItemMultiSite(inbound);                               //SDH 20-May-2008 Model Day
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 13-Aug-2008 16
        case CMD_GIA:                                                       //BMG 01-09-2008 17 ASN/Directs
            if (iNewMess && (iNewMessLen != CMD_GIA_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              GIA_Start(inbound);                                           //BMG 01-09-2008 17 ASN/Directs
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 01-09-2008 17 ASN/Directs
        case CMD_GIF:                                                       //BMG 01-09-2008 17 ASN/Directs
            if (iNewMess && (iNewMessLen != CMD_GIF_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              GIF_Booking(inbound);                                         //BMG 01-09-2008 17 ASN/Directs
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 01-09-2008 17 ASN/Directs
        case CMD_GIQ:                                                       //BMG 01-09-2008 17 ASN/Directs
            if (iNewMess && (iNewMessLen != CMD_GIQ_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              GIQ_Enquiry(inbound);                                         //BMG 01-09-2008 17 ASN/Directs
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 01-09-2008 17 ASN/Directs
        case CMD_GIX:                                                       //BMG 01-09-2008 17 ASN/Directs
            if (iNewMess && (iNewMessLen != CMD_GIX_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              GIX_End(inbound);                                             //BMG 01-09-2008 17 ASN/Directs
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 01-09-2008 17 ASN/Directs
        case CMD_DAC:                                                       //AVS 20-04-2015 Dallas
           if (iNewMess && (iNewMessLen != CMD_DAC_LTH)) {                  //AVS 20-04-2015 Dallas
                prep_nak("Invalid Message Length");                         //AVS 20-04-2015 Dallas
            } else {                                                        //AVS 20-04-2015 Dallas
              DAC_Request();                                                //AVS 20-04-2015 Dallas
            }                                                               //AVS 20-04-2015 Dallas
            break;                                                          //AVS 20-04-2015 Dallas
        case CMD_DAL:                                                       //AVS 20-04-2015 Dallas
           if (iNewMess && (iNewMessLen != CMD_DAL_LTH)) {                  //AVS 20-04-2015 Dallas
                prep_nak("Invalid Message Length");                         //AVS 20-04-2015 Dallas
            } else {                                                        //AVS 20-04-2015 Dallas
              DAL_Request(inbound);                                         //AVS 20-04-2015 Dallas
            }                                                               //AVS 20-04-2015 Dallas
            break;                                                          //AVS 20-04-2015 Dallas
        case CMD_DAR:                                                       //AVS 20-04-2015 Dallas
           if (iNewMess && (iNewMessLen != CMD_DAR_LTH)) {                  //AVS 20-04-2015 Dallas
                prep_nak("Invalid Message Length");                         //AVS 20-04-2015 Dallas
            } else {                                                        //AVS 20-04-2015 Dallas
              DAR_Request(inbound);                                         //AVS 20-04-2015 Dallas
            }                                                               //AVS 20-04-2015 Dallas
            break;                                                          //AVS 20-04-2015 Dallas
        case CMD_ALR:                                                       //BMG 10-09-2008 18 MC70
            if (iNewMess && (iNewMessLen != CMD_ALR_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ALR_Request(inbound);                                         //BMG 10-09-2008 18 MC70
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 10-09-2008 18 MC70
        case CMD_ANK:                                                       //BMG 17-12-2009 22 RF Stabilisation
            if (iNewMess && (iNewMessLen != CMD_ANK_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              ANK_Request(inbound);                                         //BMG 17-12-2009 22 RF Stabilisation
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 17-12-2009 22 RF Stabilisation
        case CMD_RCN:                                                       //BMG 17-12-2009 22 RF Stabilisation
            if (iNewMess && (iNewMessLen != CMD_RCN_LTH)) {                 //BMG 17-12-2009 22 RF Stabilisation
                prep_nak("Invalid Message Length");                         //BMG 17-12-2009 22 RF Stabilisation
            } else {                                                        //BMG 17-12-2009 22 RF Stabilisation
              Reconnect(inbound);                                           //BMG 17-12-2009 22 RF Stabilisation
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            break;                                                          //BMG 17-12-2009 22 RF Stabilisation
        case CMD_LOG:                                                       //CSk 15-04-2010 POD Logging
            if (iNewMess && (iNewMessLen != CMD_LOG_LTH)) {                 //CSk 15-04-2010 POD Logging
                prep_nak("Invalid Message Length");                         //CSk 15-04-2010 POD Logging
            } else {                                                        //CSk 15-04-2010 POD Logging
              PODLOG_Request(inbound);                                      //CSk 15-04-2010 POD Logging
            }                                                               //CSk 15-04-2010 POD Logging
            break;                                                          //CSk 15-04-2010 POD Logging
        case CMD_XXX:                                                       //PAB 22-Nov-2006 recalls
            break;                                                          //PAB 22-Nov-2006 recalls
        }

        //usrrc = RC_OK;

        if (!debug && (lcmd != CMD_XXX)) {
            // Display status summary
            sprintf( msg, "RF Online units: %02d  Active On Unit: %03d",
                     sess,hh_unit );
            background_msg(msg);
        }

        if (debug && (lcmd != CMD_XXX)) {
            sprintf( msg,
                     "af    :%02d cimf  :%02d citem :%02d idf   :%02d "
                     "imfp  :%02d imstc :%02d irf   :%02d isf   :%02d",
                     af.sessions, cimf.sessions, citem.sessions, idf.sessions,
                     imfp.sessions, imstc.sessions, irf.sessions, isf.sessions );
            disp_msg(msg);
            sprintf( msg,
                     "plldb :%02d pllol :%02d psbt  :%02d rfrdes:%02d "
                     "stkmq :%02d stock :%02d tsf   :%02d wrf   :%02d",
                     plldb.sessions, pllol.sessions, psbt.sessions, rfrdesc.sessions,
                     stkmq.sessions, stock.sessions, tsf.sessions, wrf.sessions );
            disp_msg(msg);
            sprintf( msg,
                     "invok :%02d rfhist:%02d clolf :%02d clilf :%02d "
                     "minls :%02d                              ",
                     invok.sessions, rfhist.sessions, clolf.sessions, clilf.sessions,
                     minls.sessions );
            disp_msg(msg);
            sprintf( msg, "RFS Units Online: %02d sel:%s gap:%s Processing:",
                     hh_unit,
                     !status_wk_block?"OK":"ER",
                     !status_wg_block?"OK":"ER" );
            disp_msg(msg);
        }


        // Process next stack entry every CHECK_G passes
        // Fix PK/PB 27/11/97 Boots Issue 2 added CMD_XXX check
        if ((cnt++ == CHECK_G) || (lcmd == CMD_XXX)) {                    // PAB 01-03-07

            rc = check_command();

            // Check whether an Auto Fast Fill PSS10 run is due           // SDH 20-May-2009 Model Day
            if (AffcfRunRequired()) AffcfRunNow();                        // SDH 20-May-2009 Model Day

            // Process next outstanding work file
            process_sel_stack();

            //Unstall any stalled printer jobs (only does this every 15 minutes //11-09-2007 12 BMG
            unstall_sel_stack();                                                //11-09-2007 12 BMG

            //Trickle feed any deal updates                                 //SDH 15-12-04 PROMOTIONS
            DealFileTrickle();                                              //SDH 15-12-04 PROMOTIONS

            // so we dont try and start two PSS47s before the pipe is open  //PAB 01-12-2004
            cnt = 0;                                                        //PAB 01-12-2004
        }

        if (lcmd != CMD_XXX) {
            // Transfer outboud data to return buffer
            // If a new style message, set the response header data up      //BMG 17-12-2009 22 RF Stabilisation
            if (iNewMess) {                                                 //BMG 17-12-2009 22 RF Stabilisation
                // Move outbound data along by 5 bytes to allow for header  //BMG 17-12-2009 22 RF Stabilisation
                for (i=(out_lth-1);i>=0;i--) { out[i+5] = out[i]; }         //BMG 17-12-2009 22 RF Stabilisation
                //Set up header details and increment message length by 5   //BMG 17-12-2009 22 RF Stabilisation
                memset (out, 0xFF, 1);                                      //BMG 17-12-2009 22 RF Stabilisation
                out_lth+=5;                                                 //BMG 17-12-2009 22 RF Stabilisation
                WordToArray(out+1, 4, out_lth);                             //BMG 17-12-2009 22 RF Stabilisation
            }                                                               //BMG 17-12-2009 22 RF Stabilisation
            memcpy ( (BYTE *)(inbound), out, out_lth );
            //dump((BYTE *)out, out_lth);
            *nbytes = out_lth;
        }

        // Transfer outbound data to return buffer
        if (debug && (lcmd != CMD_XXX)) {
            form_timestamp( ts, sizeof(ts) );
            sprintf(msg, "TX (%s) ", ts);
            disp_msg(msg);
            dump((BYTE *)out, out_lth);
        }

        return( rc );

    }

}                                                                           // end not null command received


