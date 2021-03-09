// -------------------------------------------------------------------------
//                           Boots The Chemists Ltd
//                      Radio Frequency Server Application
//                             Function Module
// ------------------------------------------------------------------------
// Version 1.0                Stuart Highley              22-Sep-2008
// Created separate module for common file functions.
//
// Version 1.1                Brian Greenfield            9th March 2009
// When creating a SYS_GAP file, only write the standard header if the
// device is not MC70.
//
// Version 1.2                Brian Greenfield            5th May 2009
// Fix so that the correct length is tested in process_orphans for
// ASN, Direct and UOD buffer files.
//
// Version 1.3               Charles Skadorwa           15th Apr 2010
// Changes to support POD Logging Enhancements.
//
// Version 1.4               Charles Skadorwa           12th Mar 2012
// Stock File Accuracy  (commented: //CSk 12-03-2012 SFA)
// Revised clilf_get_next() to populate new CLI message (8 items/message).
//
// Version 1.5               Charles Skadorwa           12th Mar 2012
// Stock File Accuracy  (commented: //CSk 12-03-2012 SFA - Locked Picking List Fix)
// Locked Picking List Fix - Picker ID added to PLL message - see pllol_get_next().
//
// Version 1.6               Tittoo Thomas              04th Jul 2012
// Stock File Accuracy  (commented: //Tat 04-07-2012 SFA)
// Missing count lists message Fix - Last count date added to CLL message -
// see clolf_get_next().
//
// Version 1.7               Tittoo Thomas              10th Jul 2012
// Stock File Accuracy  (commented: //Tat 10-07-2012 SFA)
// Fixed to changes the GetFuturePendingSaleplanFlag to be picked based on the
// pending sales planner instead of the live one
//
// Version 1.8               Charles Skadorwa              7th Aug 2012
// Stock File Accuracy  (commented: //CSk 07-08-2012 SFA)
// Fix to clilf_get_next() function which was incrementing the Uncounted item
// count for every item regardless of status which resulted in an incorrect
// CLI message response.
//
// Version 1.9               Tittoo Thomas               07th Sept 2012
// Stock File Accuracy  (commented: //TAT 07-09-2012 SFA)
// Fix clolf_get_next() function to exclude count lists with status set to
// ' ' (User created with no items added yet).
//
// Version 1.10               Tittoo Thomas               11th Sept 2012
// Stock File Accuracy  (commented: //TAT 11-09-2012 SFA)
// Corrected OSSR item flag.
//
// Version 1.11               Tittoo Thomas               25th Sept 2012
// Stock File Accuracy  (commented: //TAT 25-09-2012 SFA)
// Fixed to use the correct sequence number in CLI messages.
//
// Version 1.12               Tittoo Thomas               24rd Oct 2012
// Stock File Accuracy  (commented: //TAT 24-10-2012 SFA)
// Fixed to sent count lists with 'Active' and 'In creation' status as well to
// the HH in a CLL. This will cause a locked count list issue which needs to be
// fixed as part of merging in the locked picking list issue. (QC Defect: 770)
// Also Fixed to resolve the issue where the the PILST id and PIITM number in
// head office counts while updating the STKMQ, were over written. (QC Defect 767 & 777)
//
// Version 1.13               Tittoo Thomas               02nd Nov 2012
// Stock File Accuracy  (commented: //TAT 02-11-2012 SFA)
// Fixed to ensure only fully counted items are excluded in a CLI message.
//
// Version 1.14               Tittoo Thomas               27th Nov 2012
// Stock File Accuracy  (commented: //TAT 27-11-2012 SFA)
// An item in an active sales plan planner included in pending list if there
// is another fragment beyond PSP lead time. (SFA defect 818)
//
// Version 1.15               Tittoo Thomas               29th Nov 2012
// Stock File Accuracy  (commented: //TAT 29-11-2012 SFA)
// Replaced the SEL description with item description for items with the
// SEL description marked as 'X'. (SFA defect 821)
//
// Version 1.16               Tittoo Thomas               06th Dec 2012
// Stock File Accuracy  (commented: //TAT 06-12-2012 SFA)
// Fixed the plldb_get_next function to use the item description for an item
// with SEL description marked with a "X " or "x ". (SFA defect 821)
//
// Version 1.17               Tittoo Thomas               15th Feb 2013
// Stock File Accuracy  (commented: //TAT 15-02-2013 SFA)
// Changes as per CR08 for SFA, added the counter_id field to CLL message
//
// Version 1.18               Rejiya Nair                28th June 2013
// Event Log Rationalisation  (commented: // RJN 28-06-2013 ELR)
// Changes to remove the unnecessary application events logged from
// TRANSACT that are not supposed to be actually logged.
// ------------------------------------------------------------------------
#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <flexif.h>
#include <math.h>

//#include "debugdef.h"
#include "osfunc.h"
#include "rfsfile.h"
//#include "wrap.h"
#include "srfiles.h"
//#include "util.h"
//#include "dateconv.h"
#include "sockserv.h"
#include "rfglobal.h"
#include "ccfiles.h"
#include "bcsmf.h"
#include "pgf.h"
#include "idf.h"
#include "isf.h"
#include "irf.h"
#include "prtlist.h"
#include "prtctl.h"
#include "invok.h"
#include "rfhist.h"
#include "clfiles.h"
#include "trxutil.h"
#include "af.h"
#include "rfscf.h"                      // BMG 16-09-2008 18 MC70
#include "GIAfiles.h"                   // BMG 01-09-2008 17 ASN/Directs
#include "podok.h"                      //CSk 15-04-2010 POD Logging
#include "podlog.h"                     //CSk 15-04-2010 POD Logging

///////////////////////////////////////////////////////////////////////////
///
///   Constansts
///
///////////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////////
///                                                                      //
///   Static (private) variables                                         //
///                                                                      //
///////////////////////////////////////////////////////////////////////////



///////////////////////////////////////////////////////////////////////////
///
///    Constants and structures used by CreateKeyedFile function.
///
///////////////////////////////////////////////////////////////////////////
//Size of a sector in bytes                                                //CSk 15-04-2010 POD Logging
#define SECTORSIZE 0x0200L                                                 //CSk 15-04-2010 POD Logging
//Maximum size of a memory buffer in 16 bit code (64K)                     //CSk 15-04-2010 POD Logging
#define BUFFSIZE 0x00010000L                                               //CSk 15-04-2010 POD Logging
//Number of sectors per 64K block (128)                                    //CSk 15-04-2010 POD Logging
#define BUFFSECTORS (BUFFSIZE / SECTORSIZE)                                //CSk 15-04-2010 POD Logging
//Keyed file timestamp record                                              //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
typedef struct KTIMESTAMP {                                                //CSk 15-04-2010 POD Logging
    UWORD uwYear;                                                          //CSk 15-04-2010 POD Logging
    UBYTE ubMonth;                                                         //CSk 15-04-2010 POD Logging
    UBYTE ubDay;                                                           //CSk 15-04-2010 POD Logging
    ULONG ulTime;                                                          //CSk 15-04-2010 POD Logging
    UWORD uwTimeZone;                                                      //CSk 15-04-2010 POD Logging
} KTIMESTAMP;                                                              //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
//Keyed file control record (header) structure                             //CSk 15-04-2010 POD Logging
typedef struct KEYEDHEADER {                                               //CSk 15-04-2010 POD Logging
    BYTE abReserved1[12];                                                  //CSk 15-04-2010 POD Logging
    BYTE abFilename[18];                                                   //CSk 15-04-2010 POD Logging
    KTIMESTAMP Created;                                                    //CSk 15-04-2010 POD Logging
    UWORD uwBlockSize;                                                     //CSk 15-04-2010 POD Logging
    ULONG ulBlockCount;                                                    //CSk 15-04-2010 POD Logging
    UWORD uwRecordSize;                                                    //CSk 15-04-2010 POD Logging
    ULONG ulRandomDivisor;                                                 //CSk 15-04-2010 POD Logging
    UWORD uwKeyOffset;                                                     //CSk 15-04-2010 POD Logging
    UWORD uwKeyLength;                                                     //CSk 15-04-2010 POD Logging
    ULONG ulChainThreshold;                                                //CSk 15-04-2010 POD Logging
    ULONG ulLongestChain;                                                  //CSk 15-04-2010 POD Logging
    ULONG ulFailedRequests;                                                //CSk 15-04-2010 POD Logging
    ULONG ulReads;                                                         //CSk 15-04-2010 POD Logging
    ULONG ulReadsForUpdate;                                                //CSk 15-04-2010 POD Logging
    ULONG ulWrites;                                                        //CSk 15-04-2010 POD Logging
    ULONG ulWirtesForUpdate;                                               //CSk 15-04-2010 POD Logging
    ULONG ulReleaseRequests;                                               //CSk 15-04-2010 POD Logging
    ULONG ulDeleteRequests;                                                //CSk 15-04-2010 POD Logging
    ULONG ulCountDeep[4];                                                  //CSk 15-04-2010 POD Logging
    ULONG ulFileOpens;                                                     //CSk 15-04-2010 POD Logging
    ULONG ulClosesWORelease;                                               //CSk 15-04-2010 POD Logging
    ULONG ulClosesRelease;                                                 //CSk 15-04-2010 POD Logging
    KTIMESTAMP StatsCleared;                                               //CSk 15-04-2010 POD Logging
    BYTE abReserved2[30];                                                  //CSk 15-04-2010 POD Logging
    BYTE abSignature[9];                                                   //CSk 15-04-2010 POD Logging
    BYTE abReserved3[10];                                                  //CSk 15-04-2010 POD Logging
    UBYTE ubHash;                                                          //CSk 15-04-2010 POD Logging
    UWORD uwChainType;                                                     //CSk 15-04-2010 POD Logging
    BYTE abReserved4[266];                                                 //CSk 15-04-2010 POD Logging
    BYTE abReserved5[60];                                                  //CSk 15-04-2010 POD Logging
    BYTE abCreator[4];                                                     //CSk 15-04-2010 POD Logging
} KEYEDHEADER, *P_KEYEDHEADER;                                             //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
//Flags for POSFILE and PSEXTN keyed file extenstions                      //CSk 15-04-2010 POD Logging
#define F4690_KEYED      0x0001                                            //CSk 15-04-2010 POD Logging
#define F4690_SEQUENTIAL 0x0002                                            //CSk 15-04-2010 POD Logging
#define F4690_RANDOM     0x0004                                            //CSk 15-04-2010 POD Logging
#define F4690_DIRECT     0x0008                                            //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
#define F4690_MIRRORED   0x0010                                            //CSk 15-04-2010 POD Logging
#define F4690_COMPOUND   0x0020                                            //CSk 15-04-2010 POD Logging
#define F4690_LOCAL      0x0080                                            //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
#define F4690_ATCLOSE    0x0100                                            //CSk 15-04-2010 POD Logging
#define F4690_PERUPDATE  0x0200                                            //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
#define F4690_32M        0x0800                                            //CSk 15-04-2010 POD Logging
#define F4690_USERCLEAR  0x1000                                            //CSk 15-04-2010 POD Logging

//Structure for creating a keyed file < 32MB                               //CSk 15-04-2010 POD Logging
typedef struct POSFILE_PARAM1 {                                            //CSk 15-04-2010 POD Logging
    UWORD pf_flags;                                                        //CSk 15-04-2010 POD Logging
    UWORD pf_numblocks;                                                    //CSk 15-04-2010 POD Logging
    UWORD pf_randivsr;                                                     //CSk 15-04-2010 POD Logging
    UWORD pf_keyrecl;                                                      //CSk 15-04-2010 POD Logging
    UWORD pf_keylen;                                                       //CSk 15-04-2010 POD Logging
    UWORD pf_cthresh;                                                      //CSk 15-04-2010 POD Logging
    UWORD pf_reserved;                                                     //CSk 15-04-2010 POD Logging
} POSFILE_PARAM1;                                                          //CSk 15-04-2010 POD Logging
                                                                           //CSk 15-04-2010 POD Logging
//Structure for creating a keyed file > 32MB                               //CSk 15-04-2010 POD Logging
typedef struct POSEXTN_PARAM1 {                                            //CSk 15-04-2010 POD Logging
    UWORD pe_flags;                                                        //CSk 15-04-2010 POD Logging
    ULONG pe_numblocks;                                                    //CSk 15-04-2010 POD Logging
    ULONG pe_randivsr;                                                     //CSk 15-04-2010 POD Logging
    UWORD pe_keyrecl;                                                      //CSk 15-04-2010 POD Logging
    UWORD pe_keylen;                                                       //CSk 15-04-2010 POD Logging
    UWORD pe_cthresh;                                                      //CSk 15-04-2010 POD Logging
    UWORD pe_reserved;                                                     //CSk 15-04-2010 POD Logging
} POSEXTN_PARAM1;                                                          //CSk 15-04-2010 POD Logging


///////////////////////////////////////////////////////////////////////////
///
///   Functions to read and write with record locking.
///
///////////////////////////////////////////////////////////////////////////

#define SYNC    0
#define ASYNC   1

#define F_READ  7
#define F_WRITE  8
#define F_SPECIAL 9

struct _pblk {
   char pb_mode;
   char pb_option;
   int pb_flags;
   long pb_swi;
   long pb_p1;
   long pb_p2;
   long pb_p3;
   long pb_p4;
   long pb_p5;
};

LONG u_read(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                 LONG bufsiz, LONG offset, ULONG ulTimeoutMilli)        // SDH 17-11-04 OSSR WAN
{

    struct _pblk p;
    ULONG ulWait;
    LONG lRc;                                                           // SDH 17-11-04 OSSR WAN

    BOOLEAN retry = TRUE;                                               // SDH 17-11-04 OSSR WAN
    while(retry) {                                                      // SDH 17-11-04 OSSR WAN

        p.pb_mode = SYNC;                                               // SDH 17-11-04 OSSR WAN
        p.pb_option = option;
        p.pb_flags = flags;
        p.pb_swi = 0;
        p.pb_p1 = fnum;
        p.pb_p2 = (long)buffer;
        p.pb_p3 = bufsiz;
        p.pb_p4 = offset;
        p.pb_p5 = 0;
        //lRc = _osif(F_READ, &p);
        lRc = __osif(F_READ, (LONG)&p);

        //If there was a lock access conflict                           // SDH 17-11-04 OSSR WAN
        if ((lRc & 0xFFFF) == 0x430D) {                                 // SDH 17-11-04 OSSR WAN

            //Wait for the lesser of the remaining timeout and 500ms    // SDH 17-11-04 OSSR WAN
            ulWait = _min(500, ulTimeoutMilli);                         // SDH 17-11-04 OSSR WAN
            s_timer(0, ulWait);                                         // SDH 17-11-04 OSSR WAN
            ulTimeoutMilli -= ulWait;                                   // SDH 17-11-04 OSSR WAN

        } else {                                                        // SDH 17-11-04 OSSR WAN
            retry = FALSE;                                              // SDH 17-11-04 OSSR WAN
        }                                                               // SDH 17-11-04 OSSR WAN

    }                                                                   // SDH 17-11-04 OSSR WAN

    return lRc;                                                         // SDH 17-11-04 OSSR WAN

}

LONG u_write(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                  LONG bufsiz, LONG offset)
{

   struct _pblk p;

   p.pb_mode = SYNC;                                                    // SDH 17-11-04 OSSR WAN
   p.pb_option = option;
   p.pb_flags = flags;
   p.pb_swi = 0;
   p.pb_p1 = fnum;
   p.pb_p2 = (long)buffer;
   p.pb_p3 = bufsiz;
   p.pb_p4 = offset;
   p.pb_p5 = 0;
   //return (_osif(F_WRITE, &p));
   return (__osif(F_WRITE, (LONG)&p));

}


///////////////////////////////////////////////////////////////////////////
///
///   OpenDirectFile
///
///////////////////////////////////////////////////////////////////////////

LONG OpenDirectFile(BYTE *fname, UWORD flags,
                    WORD report, BOOLEAN fLogError) {                       //SDH 10-03-2005 EXCESS
    UBYTE ac;
    WORD try;
    LONG fnum, rc;
    try=0;
    do {
        fnum = s_open( flags, fname );
        if ((ac=((fnum&0xFFFF)==0x400C))!=0) {
            s_timer(0, AC_RETRY_DELAY*1000L);
        }
    } while ((ac && try++<AC_MAX_RETRY)!=0);
    if (fnum>0) {
        rc = s_special( 0x36, 0, fnum, 0L, 0x04, 0L, 0L );
        if (rc!=0) {
            s_close(0, fnum);
            if (fLogError) log_event101(rc, report, __LINE__);              //SDH 10-03-2005 EXCESS
            return rc;
        }
    } else {
        if (fLogError) log_event101(fnum, report, __LINE__);           //SDH 10-03-2005 EXCESS
    }
    return fnum;
}


///////////////////////////////////////////////////////////////////////////
///
///   OpenKeyedFile
///
///////////////////////////////////////////////////////////////////////////

static LONG OpenKeyedFile(BYTE *fname, UWORD flags,
                          WORD report, BOOLEAN fLogError) {                 //SDH 10-03-2005 EXCESS
    UBYTE ac;
    WORD try;
    LONG fnum, rc;
    BYTE pb[16];

    try=0;
    do {
        fnum = s_open( flags, fname );
        if ((ac=((fnum&0xFFFF)==0x400C))!=0) {
            s_timer(0, AC_RETRY_DELAY*1000L);
        }
    } while ((ac && try++<AC_MAX_RETRY)!=0);
    if (fnum>0) {
        rc = s_special( 0x36, 0, fnum, 0L, 0x01, (BYTE *)&pb, sizeof(pb) );
        if (rc!=0) {
            s_close(0, fnum);
            if (fLogError) log_event101(rc, report, __LINE__);         //SDH 10-03-2005 EXCESS
            return rc;
        }
    } else {
        if (fLogError) log_event101(fnum, report, __LINE__);           //SDH 10-03-2005 EXCESS
    }
    return fnum;
}

///////////////////////////////////////////////////////////////////////////
///
///   keyed_open
///
///   Open a generic keyed file
///
//////////////////////////////////////////////////////////////////////////

URC keyed_open(FILE_CTRL* pFile, BOOLEAN fLogError) {                // SDH 17-03-05 EXCESS

    //Re-initialise corrupt number of sessions                              // SDH 26-11-04 CREDIT CLAIMING
    //Negative, or about to wrap to zero                                    // SDH 26-11-04 CREDIT CLAIMING
    if ((pFile->sessions < 0) || ((UBYTE)(pFile->sessions + 1) == 0)) {     // SDH 26-11-04 CREDIT CLAIMING
        if (debug) {                                                        // SDH 26-11-04 CREDIT CLAIMING
            sprintf(msg, "%s sessions = %d. Closing and re-initialising...",// SDH 26-11-04 CREDIT CLAIMING
                    pFile->pbFileName, pFile->sessions);                    // SDH 26-11-04 CREDIT CLAIMING
            disp_msg(msg);                                                  // SDH 26-11-04 CREDIT CLAIMING
        }                                                                   // SDH 26-11-04 CREDIT CLAIMING
        s_close(0, pFile->fnum);                                            // SDH 26-11-04 CREDIT CLAIMING
        pFile->fnum = -1L;                                                  // SDH 26-11-04 CREDIT CLAIMING
        pFile->sessions = 0;                                                // SDH 26-11-04 CREDIT CLAIMING
    }                                                                       // SDH 26-11-04 CREDIT CLAIMING

    if (pFile->sessions == 0) {                                             // SDH 26-11-04 CREDIT CLAIMING

        pFile->fnum = OpenKeyedFile(pFile->pbFileName,                      // SDH 26-11-04 CREDIT CLAIMING
                                    pFile->wOpenFlags,                      // SDH 26-11-04 CREDIT CLAIMING
                                    pFile->wReportNum,                      // SDH 17-03-05 EXCESS
                                    fLogError);                             // SDH 17-03-05 EXCESS

        if (pFile->fnum <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIMING
            if (debug) {                                                    // SDH 26-11-04 CREDIT CLAIMING
                sprintf(msg, "Physical open of %s failed.  RC:%08lX",       // SDH 26-11-04 CREDIT CLAIMING
                        pFile->pbFileName,                                  // SDH 26-11-04 CREDIT CLAIMING
                        pFile->fnum);                                       // SDH 26-11-04 CREDIT CLAIMING
                disp_msg(msg);                                              // SDH 26-11-04 CREDIT CLAIMING
            }                                                               // SDH 26-11-04 CREDIT CLAIMING
            return RC_DATA_ERR;                                             // SDH 26-11-04 CREDIT CLAIMING
        }                                                                   // SDH 26-11-04 CREDIT CLAIMING
        if (debug) sprintf(msg, "Physical open of %s OK.", pFile->pbFileName);//SDH 26-11-04 CREDIT CLAIMING
    } else {                                                                // SDH 26-11-04 CREDIT CLAIMING
        if (debug) sprintf(msg, "Logical open of %s OK. New sessions = %d", // SDH 26-11-04 CREDIT CLAIMING
                           pFile->pbFileName, pFile->sessions);             // SDH 26-11-04 CREDIT CLAIMING
    }                                                                       // SDH 26-11-04 CREDIT CLAIMING

    //Print debug                                                           // SDH 26-11-04 CREDIT CLAIMING
    if (debug) disp_msg(msg);                                               // SDH 26-11-04 CREDIT CLAIMING

    pFile->sessions += 1;                                                   // SDH 26-11-04 CREDIT CLAIMING
    return RC_OK;                                                           // SDH 26-11-04 CREDIT CLAIMING

}                                                                           // SDH 26-11-04 CREDIT CLAIMING

//////////////////////////////////////////////////////////////////////////
///
///   direct_open
///
///   Open a generic direct file
///
//////////////////////////////////////////////////////////////////////////

URC direct_open(FILE_CTRL* pFile, BOOLEAN fLogError) {                      // SDH 17-03-05 EXCESS

    //if (pFile->fnum > 0) { return RC_OK; }                                // BMG 10-09-2007

    //Re-initialise corrupt number of sessions                              // SDH 26-11-04 CREDIT CLAIMING
    //Negative, or about to wrap to zero                                    // SDH 26-11-04 CREDIT CLAIMING
    if ((pFile->sessions < 0) || ((UBYTE)(pFile->sessions + 1) == 0)) {     // SDH 26-11-04 CREDIT CLAIMING
        sprintf(msg, "%s sessions = %d. Closing and re-initialising...",    // SDH 26-11-04 CREDIT CLAIMING
                pFile->pbFileName, pFile->sessions);                        // SDH 26-11-04 CREDIT CLAIMING
        disp_msg(msg);                                                      // SDH 26-11-04 CREDIT CLAIMING
        s_close(0, pFile->fnum);                                            // SDH 26-11-04 CREDIT CLAIMING
        pFile->fnum = -1L;                                                  // SDH 26-11-04 CREDIT CLAIMING
        pFile->sessions = 0;                                                // SDH 26-11-04 CREDIT CLAIMING
    }                                                                       // SDH 26-11-04 CREDIT CLAIMING

    if (pFile->sessions == 0) {                                             // SDH 26-11-04 CREDIT CLAIMING

        pFile->fnum = OpenDirectFile(pFile->pbFileName,                     // SDH 26-11-04 CREDIT CLAIMING
                                     pFile->wOpenFlags,                     // SDH 26-11-04 CREDIT CLAIMING
                                     pFile->wReportNum,                     // SDH 17-03-05 EXCESS
                                     fLogError);                            // SDH 17-03-05 EXCESS

        if (pFile->fnum <= 0L) {                                            // SDH 26-11-04 CREDIT CLAIMING
            sprintf(msg, "Physical open of %s failed.  RC:%08lX",           // SDH 26-11-04 CREDIT CLAIMING
                    pFile->pbFileName,                                      // SDH 26-11-04 CREDIT CLAIMING
                    pFile->fnum);                                           // SDH 26-11-04 CREDIT CLAIMING
            disp_msg(msg);                                                  // Streamline SDH 23-Sep-2008
            return RC_DATA_ERR;                                             // SDH 26-11-04 CREDIT CLAIMING
        }                                                                   // SDH 26-11-04 CREDIT CLAIMING

        sprintf(msg, "Physical open of %s OK.", pFile->pbFileName);         // SDH 26-11-04 CREDIT CLAIMING

    } else {                                                                // SDH 26-11-04 CREDIT CLAIMING
        sprintf(msg, "Logical open of %s OK.", pFile->pbFileName);          // SDH 26-11-04 CREDIT CLAIMING
    }                                                                       // SDH 26-11-04 CREDIT CLAIMING

    disp_msg(msg);                                                          // SDH 26-11-04 CREDIT CLAIMING

    pFile->sessions += 1;                                                   // SDH 26-11-04 CREDIT CLAIMING
    return RC_OK;                                                           // SDH 26-11-04 CREDIT CLAIMING

}                                                                           // SDH 26-11-04 CREDIT CLAIMING


//////////////////////////////////////////////////////////////////////////
///
///   close_file
///
///   Close a generic file
///
///////////////////////////////////////////////////////////////////////////

URC close_file(WORD type, FILE_CTRL* pFile) {

    if (pFile->sessions != 0) {

        if (pFile->sessions < 0) {                                          //SDH 10-03-2005 EXCESS
            pFile->sessions = 0;                                            //SDH 10-03-2005 EXCESS
        } else if (type == CL_SESSION) {
            pFile->sessions = _max(pFile->sessions - 1, 0);                 //SDH 10-03-2005 EXCESS
        } else {
            pFile->sessions = 0;
        }

        if (pFile->sessions == 0) {
            s_close(0, pFile->fnum);
            pFile->fnum = 0;                                                //BMG 10-09-2007
            if (debug) {                                                    //SDH 10-03-2005 EXCESS
                sprintf(msg, "Physical close of %s.", pFile->pbFileName);   //SDH 10-03-2005 EXCESS
                disp_msg(msg);                                              //SDH 10-03-2005 EXCESS
            }                                                               //SDH 10-03-2005 EXCESS
        } else {
            if (debug) {                                                    //SDH 10-03-2005 EXCESS
                sprintf(msg, "Logical close of %s.  New sessions: %d",      //SDH 10-03-2005 EXCESS
                        pFile->pbFileName, pFile->sessions);                //SDH 10-03-2005 EXCESS
                disp_msg(msg);                                              //SDH 10-03-2005 EXCESS
            }                                                               //SDH 10-03-2005 EXCESS
        }                                                                   //SDH 10-03-2005 EXCESS

    }

    return RC_OK;

}

//////////////////////////////////////////////////////////////////////////////
///   ReadDirect
///   Read the keyed file without a lock.
///   IMPORTANT NOTICE: First record is record 0.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDirect(FILE_CTRL* pFile, LONG lRecNum,
                       LONG lLineNumber, WORD wLogLevel) {                  // SDH 29-11-04 CREDIT CLAIM

    LONG lRc;                                                               // SDH 29-11-04 CREDIT CLAIM

    lRc = s_read(A_BOFOFF, pFile->fnum, pFile->pBuffer,                     // SDH 29-11-04 CREDIT CLAIM
                 pFile->wRecLth, pFile->wRecLth * lRecNum);                 // SDH 29-11-04 CREDIT CLAIM

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x4003) {                                   // SDH 26-05-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 29-11-04 CREDIT CLAIM
        if (lRc <= 0L) {                                                    // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Read %s record %ld ERROR. RC:%08lX",              // SDH 29-11-04 CREDIT CLAIM
                    pFile->pbFileName, lRecNum, lRc);                       // SDH 29-11-04 CREDIT CLAIM
            disp_msg(msg);                                                  // SDH 29-11-04 CREDIT CLAIM
        } else {                                                            // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Read %s record %ld OK. Record follows:",          // SDH 29-11-04 CREDIT CLAIM
                    pFile->pbFileName, lRecNum);                            // SDH 29-11-04 CREDIT CLAIM
            disp_msg(msg);                                                  // SDH 29-11-04 CREDIT CLAIM
            dump(pFile->pBuffer, pFile->wRecLth);                           // SDH 29-11-04 CREDIT CLAIM
        }                                                                   // SDH 29-11-04 CREDIT CLAIM
    }                                                                       // SDH 29-11-04 CREDIT CLAIM

    return lRc;                                                             // SDH 29-11-04 CREDIT CLAIM

}                                                                           // SDH 29-11-04 CREDIT CLAIM

//////////////////////////////////////////////////////////////////////////////
///   ReadDirectLock
///   Read the direct file with a record lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDirectLock(FILE_CTRL* pFile, LONG lRecNum,                         // SDH 22-02-2005 EXCESS
                    LONG lLineNumber, WORD wLogLevel) {                     // SDH 22-02-2005 EXCESS

    //Always wait for 250ms for the lock to release                         // SDH 22-02-2005 EXCESS
    LONG lRc = u_read(1, A_BOFOFF, pFile->fnum, pFile->pBuffer,             // SDH 22-02-2005 EXCESS
                      pFile->wRecLth, pFile->wRecLth * lRecNum, 250);       // SDH 22-02-2005 EXCESS

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x4003) {                                   // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 22-02-2005 EXCESS
        if (lRc <= 0L) {                                                    // SDH 22-02-2005 EXCESS
            sprintf(msg, "Read %s LOCKED ERROR. RC:%08lX. Key follows:",    // SDH 22-02-2005 EXCESS
                    pFile->pbFileName, lRc);                                // SDH 22-02-2005 EXCESS
            disp_msg(msg);                                                  // SDH 22-02-2005 EXCESS
            dump(pFile->pBuffer, pFile->wKeyLth);                           // SDH 22-02-2005 EXCESS
        } else {                                                            // SDH 22-02-2005 EXCESS
            sprintf(msg, "Read %s LOCKED OK. Record follows:",              // SDH 22-02-2005 EXCESS
                    pFile->pbFileName);                                     // SDH 22-02-2005 EXCESS
            disp_msg(msg);                                                  // SDH 22-02-2005 EXCESS
            dump(pFile->pBuffer, pFile->wRecLth);                           // SDH 22-02-2005 EXCESS
        }                                                                   // SDH 22-02-2005 EXCESS
    }                                                                       // SDH 22-02-2005 EXCESS

    return lRc;                                                             // SDH 22-02-2005 EXCESS

}                                                                           // SDH 22-02-2005 EXCESS

//////////////////////////////////////////////////////////////////////////////
///   WriteDirect
///   Write the direct file without a lock.
///   First record is record 0.
//////////////////////////////////////////////////////////////////////////////
LONG WriteDirect(FILE_CTRL* pFile, LONG lRecNum,                            // SDH 29-11-04 CREDIT CLAIM
                        LONG lLineNumber, WORD wLogLevel) {                 // SDH 29-11-04 CREDIT CLAIM

    LONG lRc;                                                               // SDH 29-11-04 CREDIT CLAIM

    lRc = s_write(A_FLUSH | A_BOFOFF, pFile->fnum, pFile->pBuffer,          // SDH 29-11-04 CREDIT CLAIM
                pFile->wRecLth, pFile->wRecLth * lRecNum);                  // SDH 29-11-04 CREDIT CLAIM

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x4003) {                                   // SDH 26-05-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 29-11-04 CREDIT CLAIM
        if (lRc <= 0L) {                                                    // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Write %s record %ld ERROR. RC:%08lX. "            // SDH 29-11-04 CREDIT CLAIM
                    "Record follows:", pFile->pbFileName, lRecNum, lRc);    // SDH 29-11-04 CREDIT CLAIM
        } else {                                                            // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Write %s record %ld OK. Record follows:",         // SDH 29-11-04 CREDIT CLAIM
                    pFile->pbFileName, lRecNum);                            // SDH 29-11-04 CREDIT CLAIM
        }                                                                   // SDH 29-11-04 CREDIT CLAIM
        disp_msg(msg);                                                      // SDH 29-11-04 CREDIT CLAIM
        dump(pFile->pBuffer, pFile->wRecLth);                               // SDH 29-11-04 CREDIT CLAIM
    }                                                                       // SDH 29-11-04 CREDIT CLAIM

    return lRc;                                                             // SDH 29-11-04 CREDIT CLAIM

}                                                                           // SDH 29-11-04 CREDIT CLAIM

//////////////////////////////////////////////////////////////////////////////
///   WriteDirectUnlock
///   Write the direct file with an unlock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteDirectUnlock(FILE_CTRL* pFile, LONG lRecNum,                      // SDH 22-02-2005 EXCESS
                       LONG lLineNumber, WORD wLogLevel) {                  // SDH 22-02-2005 EXCESS

    LONG lRc = u_write(1, A_FLUSH | A_BOFOFF, pFile->fnum, pFile->pBuffer,  // SDH 22-02-2005 EXCESS
                       pFile->wRecLth, pFile->wRecLth * lRecNum);           // SDH 22-02-2005 EXCESS

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 22-02-2005 EXCESS
        if (lRc <= 0L) {                                                    // SDH 22-02-2005 EXCESS
            sprintf(msg, "Write %s UNLOCK ERROR. RC:%08lX. "                // SDH 22-02-2005 EXCESS
                    "Record follows:", pFile->pbFileName, lRc);             // SDH 22-02-2005 EXCESS
        } else {                                                            // SDH 22-02-2005 EXCESS
            sprintf(msg, "Write %s UNLOCK OK. Record follows:",             // SDH 22-02-2005 EXCESS
                    pFile->pbFileName);                                     // SDH 22-02-2005 EXCESS
        }                                                                   // SDH 22-02-2005 EXCESS
        disp_msg(msg);                                                      // SDH 22-02-2005 EXCESS
        dump(pFile->pBuffer, pFile->wRecLth);                               // SDH 22-02-2005 EXCESS
    }                                                                       // SDH 22-02-2005 EXCESS

    return lRc;                                                             // SDH 22-02-2005 EXCESS

}                                                                           // SDH 22-02-2005 EXCESS


////////////////////////////////////////////////////////////////////////////////    //CSk 15-04-2010 POD Logging
//                                                                                  //CSk 15-04-2010 POD Logging
//  ValidKeyedHeader:                                                               //CSk 15-04-2010 POD Logging
//  Checks a keyed file control record (header) to see if the file appears          //CSk 15-04-2010 POD Logging
//  to be a valid keyed file                                                        //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
UBYTE ValidKeyedHeader (P_KEYEDHEADER pKFH, ULONG ulFileSize) {                     //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    ULONG ulBlockCount;                                                             //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //Checks on the file size:                                                      //CSk 15-04-2010 POD Logging
    //Convert file size to sector count                                             //CSk 15-04-2010 POD Logging
    ulBlockCount = ulFileSize / SECTORSIZE;                                         //CSk 15-04-2010 POD Logging
    //Check file is exact multiple of sectors                                       //CSk 15-04-2010 POD Logging
    if (ulBlockCount * SECTORSIZE != ulFileSize) return 0;                          //CSk 15-04-2010 POD Logging
    //Check sector count against header                                             //CSk 15-04-2010 POD Logging
    if (ulBlockCount != pKFH->ulBlockCount) return 0;                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //Checks on the Keyed File Control Record                                       //CSk 15-04-2010 POD Logging
    //Block size must be 1 sector                                                   //CSk 15-04-2010 POD Logging
    if (pKFH->uwBlockSize != SECTORSIZE) return 0;                                  //CSk 15-04-2010 POD Logging
    //Must be at least 3 sectors (1 header + 2 data)                                //CSk 15-04-2010 POD Logging
    if (pKFH->ulBlockCount < 2) return 0;                                           //CSk 15-04-2010 POD Logging
    //Record size must fit in sector                                                //CSk 15-04-2010 POD Logging
    if (pKFH->uwRecordSize == 0 ||                                                  //CSk 15-04-2010 POD Logging
        pKFH->uwRecordSize > 508) return 0;                                         //CSk 15-04-2010 POD Logging
    //Divisor must hash to sector that exists!                                      //CSk 15-04-2010 POD Logging
    if (pKFH->ulRandomDivisor == 0 ||                                               //CSk 15-04-2010 POD Logging
        pKFH->ulRandomDivisor >= ulBlockCount) return 0;                            //CSk 15-04-2010 POD Logging
    //Only keys at start of record valid                                            //CSk 15-04-2010 POD Logging
    if (pKFH->uwKeyOffset != 0) return 0;                                           //CSk 15-04-2010 POD Logging
    //Key length must be within record size                                         //CSk 15-04-2010 POD Logging
    if (pKFH->uwKeyLength == 0 ||                                                   //CSk 15-04-2010 POD Logging
        pKFH->uwKeyLength > pKFH->uwRecordSize) return 0;                           //CSk 15-04-2010 POD Logging
    //Check signature FSFACADX                                                      //CSk 15-04-2010 POD Logging
    if (strcmp (pKFH->abSignature, "FSFACADX") != 0) return 0;                      //CSk 15-04-2010 POD Logging
    //Chack hash algorithm is 0, 1 or 2                                             //CSk 15-04-2010 POD Logging
    if (pKFH->ubHash > 2) return 0;                                                 //CSk 15-04-2010 POD Logging
    //Check chain type is 0 (absolute) or 1 (relative)                              //CSk 15-04-2010 POD Logging
    if (pKFH->uwChainType > 1) return 0;                                            //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //Else Keyed file is valid                                                      //CSk 15-04-2010 POD Logging
    return 1;                                                                       //CSk 15-04-2010 POD Logging
}                                                                                   //CSk 15-04-2010 POD Logging


//////////////////////////////////////////////////////////////////////////////      //CSk 15-04-2010 POD Logging
//                                                                                  //CSk 15-04-2010 POD Logging
//  CreateKeyedFile:                                                                //CSk 15-04-2010 POD Logging
//  Creates a new keyed file with the required attributes by using the              //CSk 15-04-2010 POD Logging
//  4690 Keyed File Extension s_special calls.                                      //CSk 15-04-2010 POD Logging
//  Returns  0 for success                                                          //CSk 15-04-2010 POD Logging
//          -1 for failure                                                          //CSk 15-04-2010 POD Logging
//////////////////////////////////////////////////////////////////////////////      //CSk 15-04-2010 POD Logging
LONG CreateKeyedFile (BYTE *pbFilename,                                             //CSk 15-04-2010 POD Logging
                      WORD  wReportNum,                                             //CSk 15-04-2010 POD Logging
                      ULONG ulBlockCount,                                           //CSk 15-04-2010 POD Logging
                      ULONG ulDivisor,                                              //CSk 15-04-2010 POD Logging
                      ULONG ulChainThreshold,                                       //CSk 15-04-2010 POD Logging
                      UBYTE ubHash,                                                 //CSk 15-04-2010 POD Logging
                      UWORD uwRecordSize,                                           //CSk 15-04-2010 POD Logging
                      UWORD uwKeyLength,                                            //CSk 15-04-2010 POD Logging
                      UWORD uwDistType,                                             //CSk 15-04-2010 POD Logging
                      UBYTE fOverwrite) {                                           //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    UWORD uwFlags;                                                                  //CSk 15-04-2010 POD Logging
    LONG lRc, lBytes, lFileSize, lOutFile;                                          //CSk 15-04-2010 POD Logging
    KEYEDHEADER    OutKFH;                                                          //CSk 15-04-2010 POD Logging
    POSFILE_PARAM1 PosFile;                                                         //CSk 15-04-2010 POD Logging
    POSEXTN_PARAM1 PosExtn;                                                         //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    lFileSize = ulBlockCount * SECTORSIZE;                                          //CSk 15-04-2010 POD Logging
    uwFlags = A_READ | A_WRITE;                                                     //CSk 15-04-2010 POD Logging
    if (fOverwrite) uwFlags |= A_DELETE;                                            //CSk 15-04-2010 POD Logging
    lOutFile = s_create (O_FILE, uwFlags, pbFilename, SECTORSIZE, FS_NO, lFileSize);//CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    if (lOutFile < 0) {                                                             //CSk 15-04-2010 POD Logging
        switch (lOutFile & 0xffff) {                                                //CSk 15-04-2010 POD Logging
        case E_EXISTS:                                                              //CSk 15-04-2010 POD Logging
            log_event101(lOutFile, wReportNum, __LINE__);                           //CSk 15-04-2010 POD Logging
            if (debug) {                                                            //CSk 15-04-2010 POD Logging
                sprintf(msg, "File already exists. Rep No: %08lX", wReportNum);     //CSk 15-04-2010 POD Logging
                disp_msg(msg);                                                      //CSk 15-04-2010 POD Logging
            }                                                                       //CSk 15-04-2010 POD Logging
            break;                                                                  //CSk 15-04-2010 POD Logging
        case E_CONFLICT:                                                            //CSk 15-04-2010 POD Logging
            log_event101(lOutFile, wReportNum, __LINE__);                           //CSk 15-04-2010 POD Logging
            if (debug) {                                                            //CSk 15-04-2010 POD Logging
                sprintf(msg, "File already open. Rep No: %08lX", wReportNum);       //CSk 15-04-2010 POD Logging
                disp_msg(msg);                                                      //CSk 15-04-2010 POD Logging
            }                                                                       //CSk 15-04-2010 POD Logging
            break;                                                                  //CSk 15-04-2010 POD Logging
        default:                                                                    //CSk 15-04-2010 POD Logging
            log_event101(lOutFile, wReportNum, __LINE__);                           //CSk 15-04-2010 POD Logging
            if (debug) {                                                            //CSk 15-04-2010 POD Logging
                sprintf(msg, "Error creating keyed file. Rep No: %08lX", wReportNum);//CSk 15-04-2010 POD Logging
                disp_msg(msg);                                                      //CSk 15-04-2010 POD Logging
            }                                                                       //CSk 15-04-2010 POD Logging
            break;                                                                  //CSk 15-04-2010 POD Logging
        }                                                                           //CSk 15-04-2010 POD Logging
        return -1L;                                                                 //CSk 15-04-2010 POD Logging
    }                                                                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    uwFlags = F4690_KEYED;                                                          //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    switch (uwDistType) {                                                           //CSk 15-04-2010 POD Logging
    case 0:                                                                         //CSk 15-04-2010 POD Logging
    case 1:                                                                         //CSk 15-04-2010 POD Logging
        uwFlags |= F4690_LOCAL;                                                     //CSk 15-04-2010 POD Logging
        break;                                                                      //CSk 15-04-2010 POD Logging
    case 2:                                                                         //CSk 15-04-2010 POD Logging
        uwFlags |= F4690_MIRRORED | F4690_PERUPDATE;                                //CSk 15-04-2010 POD Logging
        break;                                                                      //CSk 15-04-2010 POD Logging
    case 3:                                                                         //CSk 15-04-2010 POD Logging
        uwFlags |= F4690_MIRRORED | F4690_ATCLOSE;                                  //CSk 15-04-2010 POD Logging
        break;                                                                      //CSk 15-04-2010 POD Logging
    case 4:                                                                         //CSk 15-04-2010 POD Logging
        uwFlags |= F4690_COMPOUND | F4690_PERUPDATE;                                //CSk 15-04-2010 POD Logging
        break;                                                                      //CSk 15-04-2010 POD Logging
    case 5:                                                                         //CSk 15-04-2010 POD Logging
        uwFlags |= F4690_COMPOUND | F4690_ATCLOSE;                                  //CSk 15-04-2010 POD Logging
        break;                                                                      //CSk 15-04-2010 POD Logging
    }                                                                               //CSk 15-04-2010 POD Logging
    if (ulBlockCount < 0x00010000) {                                                //CSk 15-04-2010 POD Logging
        PosFile.pf_flags = uwFlags;                                                 //CSk 15-04-2010 POD Logging
        PosFile.pf_numblocks = ulBlockCount;                                        //CSk 15-04-2010 POD Logging
        PosFile.pf_randivsr = ulDivisor;                                            //CSk 15-04-2010 POD Logging
        PosFile.pf_keyrecl = uwRecordSize;                                          //CSk 15-04-2010 POD Logging
        PosFile.pf_keylen = uwKeyLength;                                            //CSk 15-04-2010 POD Logging
        PosFile.pf_cthresh = ulChainThreshold;                                      //CSk 15-04-2010 POD Logging
        PosFile.pf_reserved = 0;                                                    //CSk 15-04-2010 POD Logging
        lRc = s_special (0x77, 0, lOutFile, (BYTE*)&PosFile, sizeof(PosFile), 0, SECTORSIZE);
    } else {                                                                        //CSk 15-04-2010 POD Logging
        PosExtn.pe_flags = uwFlags | F4690_32M;                                     //CSk 15-04-2010 POD Logging
        PosExtn.pe_numblocks = ulBlockCount;                                        //CSk 15-04-2010 POD Logging
        PosExtn.pe_randivsr = ulDivisor;                                            //CSk 15-04-2010 POD Logging
        PosExtn.pe_keyrecl = uwRecordSize;                                          //CSk 15-04-2010 POD Logging
        PosExtn.pe_keylen = uwKeyLength;                                            //CSk 15-04-2010 POD Logging
        PosExtn.pe_cthresh = ulChainThreshold;
        PosExtn.pe_reserved = 0;
        lRc = s_special (0x77, 0, lOutFile, (BYTE*)&PosExtn, sizeof(PosExtn), 0, SECTORSIZE);
    }                                                                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    if (lRc < 0) {                                                                  //CSk 15-04-2010 POD Logging
        log_event101(lRc, wReportNum, __LINE__);                                    //CSk 15-04-2010 POD Logging
        if (debug) {                                                                //CSk 15-04-2010 POD Logging
            sprintf(msg, "Error initialising keyed file. Rep No: %08lX", wReportNum);//CSk 15-04-2010 POD Logging
            disp_msg(msg);                                                          //CSk 15-04-2010 POD Logging
        }                                                                           //CSk 15-04-2010 POD Logging
        return -1L;                                                                 //CSk 15-04-2010 POD Logging
    }                                                                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //Close file                                                                    //CSk 15-04-2010 POD Logging
    s_close (0, lOutFile);                                                          //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //And re-open direct                                                            //CSk 15-04-2010 POD Logging
    lOutFile = s_open (A_READ | A_WRITE, pbFilename);                               //CSk 15-04-2010 POD Logging
    if (lOutFile < 0) {                                                             //CSk 15-04-2010 POD Logging
        log_event101(lOutFile, wReportNum, __LINE__);                               //CSk 15-04-2010 POD Logging
        if (debug) {                                                                //CSk 15-04-2010 POD Logging
            sprintf(msg, "Error opening keyed file. Rep No: %08lX", wReportNum);    //CSk 15-04-2010 POD Logging
            disp_msg(msg);                                                          //CSk 15-04-2010 POD Logging
        }                                                                           //CSk 15-04-2010 POD Logging
        return -1L;                                                                 //CSk 15-04-2010 POD Logging
    }                                                                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //Read keyed file header                                                        //CSk 15-04-2010 POD Logging
    lRc = s_read(A_BOFOFF, lOutFile, (BYTE*)&OutKFH, sizeof(OutKFH), 0);            //CSk 15-04-2010 POD Logging
    if (lRc != sizeof(OutKFH)) {                                                    //CSk 15-04-2010 POD Logging
        log_event101(lRc, wReportNum, __LINE__);                                    //CSk 15-04-2010 POD Logging
        if (debug) {                                                                //CSk 15-04-2010 POD Logging
            sprintf(msg, "Error reading keyed file control record. Rep No: %08lX", wReportNum);//CSk 15-04-2010 POD Logging
            disp_msg(msg);                                                          //CSk 15-04-2010 POD Logging
        }                                                                           //CSk 15-04-2010 POD Logging
        return -1L;                                                                 //CSk 15-04-2010 POD Logging
    }                                                                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    OutKFH.ubHash = ubHash;                                                         //CSk 15-04-2010 POD Logging
    strncpy (OutKFH.abCreator, "TKFU", 4);                                          //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    lBytes = OutKFH.ulBlockCount * SECTORSIZE;                                      //CSk 15-04-2010 POD Logging
    if (!ValidKeyedHeader (&OutKFH, lBytes)) {                                      //CSk 15-04-2010 POD Logging
        log_event101(-1L, wReportNum, __LINE__);                                    //CSk 15-04-2010 POD Logging
        if (debug) {                                                                //CSk 15-04-2010 POD Logging
            sprintf(msg, "Keyed file control record is corrupt. Rep No: %08lX", wReportNum);//CSk 15-04-2010 POD Logging
            disp_msg(msg);                                                          //CSk 15-04-2010 POD Logging
        }                                                                           //CSk 15-04-2010 POD Logging
        return -1L;                                                                 //CSk 15-04-2010 POD Logging
    }                                                                               //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    //Close file                                                                    //CSk 15-04-2010 POD Logging
    s_close (0, lOutFile);                                                          //CSk 15-04-2010 POD Logging
                                                                                    //CSk 15-04-2010 POD Logging
    return 0L;                                                                      //CSk 15-04-2010 POD Logging
}                                                                                   //CSk 15-04-2010 POD Logging



//////////////////////////////////////////////////////////////////////////////
///   ReadKeyed
///   Read the keyed file without a lock.
///
//////////////////////////////////////////////////////////////////////////////
LONG ReadKeyed(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel) {        // SDH 17-11-04 OSSR WAN

    LONG lRc = s_read(0, pFile->fnum, pFile->pBuffer,                       // SDH 17-11-04 OSSR WAN
                     pFile->wRecLth, pFile->wKeyLth);                       // SDH 17-11-04 OSSR WAN

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 17-11-04 OSSR WAN
        if (lRc <= 0L) {                                                    // SDH 17-11-04 OSSR WAN
            sprintf(msg, "Read %s ERROR. RC:%08lX. Key follows:",           // SDH 17-11-04 OSSR WAN
                    pFile->pbFileName, lRc);                                // SDH 17-11-04 OSSR WAN
            disp_msg(msg);                                                  // SDH 17-11-04 OSSR WAN
            dump(pFile->pBuffer, pFile->wKeyLth);                           // SDH 17-11-04 OSSR WAN
        } else {                                                            // SDH 17-11-04 OSSR WAN
            sprintf(msg, "Read %s OK. Record follows:", pFile->pbFileName); // SDH 17-11-04 OSSR WAN
            disp_msg(msg);                                                  // SDH 17-11-04 OSSR WAN
            dump(pFile->pBuffer, pFile->wRecLth);                           // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 17-11-04 OSSR WAN
    }

    return lRc;                                                             // SDH 17-11-04 OSSR WAN

}                                                                           // SDH 17-11-04 OSSR WAN

//////////////////////////////////////////////////////////////////////////////
///   ReadKeyedLock
///   Read the keyed file with a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadKeyedLock(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel) {    // SDH 22-02-2005 EXCESS

    //Always wait for 250ms for the lock to release                         // SDH 22-02-2005 EXCESS
    LONG lRc = u_read(1, 0, pFile->fnum, pFile->pBuffer,                    // SDH 22-02-2005 EXCESS
                     pFile->wRecLth, pFile->wKeyLth, 250);                  // SDH 22-02-2005 EXCESS

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 22-02-2005 EXCESS
        if (lRc <= 0L) {                                                    // SDH 22-02-2005 EXCESS
            sprintf(msg, "Read %s LOCKED ERROR. RC:%08lX. Key follows:",    // SDH 22-02-2005 EXCESS
                    pFile->pbFileName, lRc);                                // SDH 22-02-2005 EXCESS
            disp_msg(msg);                                                  // SDH 22-02-2005 EXCESS
            dump(pFile->pBuffer, pFile->wKeyLth);                           // SDH 22-02-2005 EXCESS
        } else {                                                            // SDH 22-02-2005 EXCESS
            sprintf(msg, "Read %s LOCKED OK. Record follows:",              // SDH 22-02-2005 EXCESS
                    pFile->pbFileName);                                     // SDH 22-02-2005 EXCESS
            disp_msg(msg);                                                  // SDH 22-02-2005 EXCESS
            dump(pFile->pBuffer, pFile->wRecLth);                           // SDH 22-02-2005 EXCESS
        }                                                                   // SDH 22-02-2005 EXCESS
    }                                                                       // SDH 22-02-2005 EXCESS

    return lRc;                                                             // SDH 22-02-2005 EXCESS

}                                                                           // SDH 22-02-2005 EXCESS

//////////////////////////////////////////////////////////////////////////////
///   WriteKeyed
///   Write the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteKeyed(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel) {       // SDH 17-11-04 OSSR WAN

    LONG lRc = s_write( 0, pFile->fnum, pFile->pBuffer,                     // SDH 17-11-04 OSSR WAN
                        pFile->wRecLth, 0L);                                // SDH 17-11-04 OSSR WAN

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 29-11-04 CREDIT CLAIM
        if (lRc <= 0L) {                                                    // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Write %s ERROR. RC:%08lX. "                       // SDH 29-11-04 CREDIT CLAIM
                    "Record follows:", pFile->pbFileName, lRc);             // SDH 29-11-04 CREDIT CLAIM
        } else {                                                            // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Write %s OK. Record follows:", pFile->pbFileName);// SDH 29-11-04 CREDIT CLAIM
        }                                                                   // SDH 29-11-04 CREDIT CLAIM
        disp_msg(msg);                                                      // SDH 29-11-04 CREDIT CLAIM
        dump(pFile->pBuffer, pFile->wRecLth);                               // SDH 29-11-04 CREDIT CLAIM
    }                                                                       // SDH 29-11-04 CREDIT CLAIM

    return lRc;                                                             // SDH 17-11-04 OSSR WAN

}                                                                           // SDH 17-11-04 OSSR WAN

//////////////////////////////////////////////////////////////////////////////
///   WriteKeyedUnlock
///   Write the keyed file with an unlock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteKeyedUnlock(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel) { // SDH 22-02-2005 EXCESS

    LONG lRc = u_write(1, 0, pFile->fnum, pFile->pBuffer,                   // SDH 22-02-2005 EXCESS
                       pFile->wRecLth, 0L);                                 // SDH 22-02-2005 EXCESS

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 22-02-2005 EXCESS
        if (lRc <= 0L) {                                                    // SDH 22-02-2005 EXCESS
            sprintf(msg, "Write %s UNLOCK ERROR. RC:%08lX. "                // SDH 22-02-2005 EXCESS
                    "Record follows:", pFile->pbFileName, lRc);             // SDH 22-02-2005 EXCESS
        } else {                                                            // SDH 22-02-2005 EXCESS
            sprintf(msg, "Write %s UNLOCK OK. Record follows:",             // SDH 22-02-2005 EXCESS
                    pFile->pbFileName);                                     // SDH 22-02-2005 EXCESS
        }                                                                   // SDH 22-02-2005 EXCESS
        disp_msg(msg);                                                      // SDH 22-02-2005 EXCESS
        dump(pFile->pBuffer, pFile->wRecLth);                               // SDH 22-02-2005 EXCESS
    }                                                                       // SDH 22-02-2005 EXCESS

    return lRc;                                                             // SDH 22-02-2005 EXCESS

}                                                                           // SDH 22-02-2005 EXCESS

//////////////////////////////////////////////////////////////////////////////
///   DeleteKeyedRecord
///   Delete a record from a keyed file
//////////////////////////////////////////////////////////////////////////////
static DeleteKeyedRecord(FILE_CTRL* pFile, LONG lLineNumber,                // SDH 22-02-2005 EXCESS
                         WORD wLogLevel) {                                  // SDH 22-02-2005 EXCESS

    LONG lRc = s_special(0x74, 0, pFile->fnum, pFile->pBuffer,              // SDH 22-02-2005 EXCESS
                        pFile->wKeyLth, 0L, 0L);                            // SDH 22-02-2005 EXCESS

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);          // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);              // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 10-03-2005 EXCESS
    }                                                                       // SDH 17-11-04 OSSR WAN

    if (debug) {                                                            // SDH 22-02-2005 EXCESS
        if (lRc <= 0L) {                                                    // SDH 22-02-2005 EXCESS
            sprintf(msg, "Delete %s record ERROR. RC:%08lX.  Key follows:", // SDH 22-02-2005 EXCESS
                    pFile->pbFileName, lRc);                                // SDH 22-02-2005 EXCESS
        } else {                                                            // SDH 22-02-2005 EXCESS
            sprintf(msg, "Delete %s record OK.  Key follows:",              // SDH 22-02-2005 EXCESS
                    pFile->pbFileName);                                     // SDH 22-02-2005 EXCESS
        }                                                                   // SDH 22-02-2005 EXCESS
        disp_msg(msg);                                                      // SDH 22-02-2005 EXCESS
        dump(pFile->pBuffer, pFile->wKeyLth);                               // SDH 22-02-2005 EXCESS
    }                                                                       // SDH 22-02-2005 EXCESS

    return lRc;                                                             // SDH 22-02-2005 EXCESS

}


///////////////////////////////////////////////////////////////////////////
///
///   lrt_log
///
///   Log info to LRT log
///
///////////////////////////////////////////////////////////////////////////

URC lrt_log(BYTE type, WORD unit, BYTE *details)
{
    LONG rc;
    TIMEDATE now;
    LRTLG_REC lrtlgrec;

    lrtlgrec.type = type;
    lrtlgrec.bLocation = lrtp[unit]->bLocation;                             //SDH 19-01-2005 OSSR WAN

    s_get(T_TD, 0L, (void *)&now, TIMESIZE);
    lrtlgrec.year    = now.td_year;
    lrtlgrec.month   = now.td_month;
    lrtlgrec.day     = now.td_day;
    lrtlgrec.time_ms = now.td_time;

    memmove(lrtlgrec.user, lrtp[unit]->user, 3);
    memmove(lrtlgrec.details, details, 12);
    rc = s_write( A_EOFOFF, lrtlg.fnum, (void *)&lrtlgrec, LRTLG_RECL, 0L );
    if (rc<=0L) {
        log_event101(lrtlg.fnum, LRTLG_REP, __LINE__);
        if (debug) {
            sprintf(msg, "Err-W LRTLG. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_DATA_ERR;
    }
    return RC_OK;
}


///////////////////////////////////////////////////////////////////////////
///
///   log_file_open
///
///////////////////////////////////////////////////////////////////////////

URC log_file_open(LONG filenum, BYTE ftype) {                               // 1-2-2004 PAB

    UWORD counter;                                                          // 1-2-2004 PAB
    //LONG lRc;                                                             // 12-4-2005 SDH
    //MPB mem;                                                              // 1-2-2004 PAB

    for (counter=1; counter<2000; counter++) {                              // 1-2-2004 PAB

        if (ftab[counter].fnumx == 0L) {                                    // 12-4-2005 SDH
            ftab[counter].fnumx = filenum;                                  // 12-4-2005 SDH
            ftab[counter].ftype[0] = ftype;                                 // 12-4-2005 SDH
            return RC_OK;                                                   // 12-4-2005 SDH
        }                                                                   // 12-4-2005 SDH

/*
        if (ftab[counter] == NULL) {                                        // 1-2-2004 PAB

            //rc = alloc_file_table(counter);                               // 1-2-2004 PAB

            mem.mp_start = 0L;                                              // 1-2-2004 PAB
            mem.mp_min = (LONG)sizeof(FILE_TABLE);                          // 1-2-2004 PAB
            mem.mp_max = mem.mp_min;                                        // 1-2-2004 PAB
            lRc = s_malloc((UBYTE)O_NEWHEAP, &mem);                         // 12-4-2005 SDH
            if (lRc < 0) {                                                  // 12-4-2005 SDH
                //log_event101(rc, 0, __LINE__);                            // 1-2-2004 PAB
                if (debug) {                                                // 1-2-2004 PAB
                    sprintf(msg, "Unable to allocate storage. RC:%08lX",    // 1-2-2004 PAB
                            lRc);                                           // 1-2-2004 PAB
                    disp_msg(msg);                                          // 1-2-2004 PAB
                }                                                           // 1-2-2004 PAB
                ftab[counter] = (void *)NULL;                               // 1-2-2004 PAB
                return RC_SERIOUS_ERR;                                      // 1-2-2004 PAB
            }                                                               // 12-4-2005 SDH

            ftab[counter] = (FILE_TABLE *)mem.mp_start;                     // 1-2-2004 PAB
            //ftab[counter]->fnumx=(void *)NULL;                            // 1-2-2004 PAB
            ftab[counter]->fnumx = filenum;                                 // 1-2-2004 PAB

            if (ftype == 'R') {                                             // 1-2-2004 PAB
                ftab[counter]->ftype[0] = 'R';                              // 12-4-2005 SDH
            } else {                                                        // 1-2-2004 PAB
                ftab[counter]->ftype[0] = 'W';                              // 12-4-2005 SDH
            }                                                               // 1-2-2004 PAB
            return RC_OK;                                                   // 1-2-2004 PAB
        }                                                                   // 1-2-2004 PAB

        if (ftab[counter]->fnumx == 0L) {                                   // 17-3-2005 SDH
            ftab[counter]->fnumx = filenum;                                 // 1-2-2004 PAB
            return RC_OK;                                                   // 1-2-2004 PAB
        }                                                                   // 1-2-2004 PAB
*/

    }                                                                       // 1-2-2004 PAB

    return RC_OK;                                                           // 1-2-2004 PAB
    //Shouldn't this read?: return RC_SERIOUS_ERR;                          // 12-4-2005 SDH

}                                                                           // 1-2-2004 PAB

///////////////////////////////////////////////////////////////////////////
///
///   log_file_close
///
///////////////////////////////////////////////////////////////////////////

URC log_file_close(LONG filenum) {                                          // 1-2-2004 PAB

    UWORD counter;                                                          // 1-2-2004 PAB
    for (counter=1; counter<2000; counter++) {                              // 1-2-2004 PAB
        if (ftab[counter].fnumx == filenum) {                               // 1-2-2004 PAB
            ftab[counter].fnumx = 0L;                                       // 1-2-2004 PAB
            return RC_OK;                                                   // 1-2-2004 PAB
        }                                                                   // 1-2-2004 PAB
    }                                                                       // 1-2-2004 PAB

    return RC_OK;                                                           // 1-2-2004 PAB

}                                                                           // 1-2-2004 PAB


URC open_citem(void) {
    return keyed_open(&citem, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_citem(WORD type) {
    return close_file(type, &citem);                                        //SDH 10-03-2005 EXCESS
}

URC open_imstc(void) {
    return keyed_open(&imstc, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_imstc(WORD type) {
    return close_file(type, &imstc);                                        //SDH 10-03-2005 EXCESS
}

URC open_cimf(void) {
    return keyed_open(&cimf, TRUE);                                         //SDH 10-03-2005 EXCESS
}
URC close_cimf(WORD type) {
    return close_file(type, &cimf);                                         //SDH 10-03-2005 EXCESS
}

URC open_stkmq(void) {
    return direct_open(&stkmq, TRUE);                                       //SDH 10-03-2005 EXCESS
}
URC close_stkmq(WORD type) {
    return close_file(type, &stkmq);                                        //SDH 10-03-2005 EXCESS
}

URC open_imfp(void) {
    return keyed_open(&imfp, TRUE);                                         //SDH 10-03-2005 EXCESS
}
URC close_imfp(WORD type) {
    return close_file(type, &imfp);                                         //SDH 10-03-2005 EXCESS
}

URC open_pllol(void) {
    return direct_open(&pllol, TRUE);                                       //SDH 10-03-2005 EXCESS
}
URC close_pllol(WORD type) {
    return close_file(type, &pllol);                                        //SDH 10-03-2005 EXCESS
}

//////////////////////////////////////////////////////////////////////////////
///   ReadPllol
///   Read the direct file without a lock.
///   Always populates the global record structure.
///   The header record is 0.
//////////////////////////////////////////////////////////////////////////////
LONG ReadPllol(LONG lRecNum, LONG lLineNum) {                               // SDH 29-11-04 CREDIT CLAIM
    return ReadDirect(&pllol, lRecNum, lLineNum, LOG_CRITICAL);             // RJN 28-06-2013 ELR
}                                                                           // SDH 29-11-04 CREDIT CLAIM

//////////////////////////////////////////////////////////////////////////////
///   ReadPllolLock
///   Read the direct file with a lock.
///   Always populates the global record structure.
///   The header record is 0.
//////////////////////////////////////////////////////////////////////////////
LONG ReadPllolLock(LONG lRecNum, LONG lLineNum) {                           // SDH 20-May-2009 Model Day
    return ReadDirectLock(&pllol, lRecNum, lLineNum, LOG_ALL);              // SDH 20-May-2009 Model Day
}                                                                           // SDH 20-May-2009 Model Day

//////////////////////////////////////////////////////////////////////////////
///
///   ReadPllolLog
///
///   Read the direct file without a lock.
///   Always populates the global record structure.
///   The header record is 0.
///
//////////////////////////////////////////////////////////////////////////////
LONG ReadPllolLog(LONG lRecNum, LONG lLineNum, WORD wLogLevel) {            // SDH 29-11-04 CREDIT CLAIM
    return ReadDirect(&pllol, lRecNum, lLineNum, wLogLevel);                // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

//////////////////////////////////////////////////////////////////////////////
///   WritePllol
///   Write the direct file without a lock.
///   Always uses global record structure.
///   The header record is 0.
//////////////////////////////////////////////////////////////////////////////
LONG WritePllol(LONG lRecNum, LONG lLineNum) {                              // SDH 29-11-04 CREDIT CLAIM
    return WriteDirect(&pllol, lRecNum, lLineNum, LOG_ALL);                 // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

//////////////////////////////////////////////////////////////////////////////
///   WritePllolUnlock
///   Write the direct file with  unlock.
///   Always uses global record structure.
///   The header record is 0.
//////////////////////////////////////////////////////////////////////////////
LONG WritePllolUnlock(LONG lRecNum, LONG lLineNum) {                        // SDH 20-May-2009 Model Day
    return WriteDirectUnlock(&pllol, lRecNum, lLineNum, LOG_ALL);           // SDH 20-May-2009 Model Day
}                                                                           // SDH 20-May-2009 Model Day

URC open_plldb(void) {
    return keyed_open(&plldb, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_plldb(WORD type) {
    return close_file(type, &plldb);                                        //SDH 10-03-2005 EXCESS
}
//////////////////////////////////////////////////////////////////////////////
///   ReadPlldb
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadPlldb(LONG lLineNumber) {                                          // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&plldb, lLineNumber, LOG_ALL);                         // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   ReadPlldbLog
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadPlldbLog(LONG lLineNumber, WORD wLogLevel) {                       // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&plldb, lLineNumber, wLogLevel);                       // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   ReadPlldbLock
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadPlldbLock(LONG lLineNumber) {                                      // SDH 17-11-04 OSSR WAN
    return ReadKeyedLock(&plldb, lLineNumber, LOG_ALL);                     // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   WritePlldb
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WritePlldb(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&plldb, lLineNumber, LOG_ALL);                        // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   WritePlldbLock
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WritePlldbUnlock(LONG lLineNumber) {                                   // SDH 17-11-04 OSSR WAN
    return WriteKeyedUnlock(&plldb, lLineNumber, LOG_ALL);                  // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN

URC open_minls(void) {
    return keyed_open(&minls, TRUE);                                        // SDH 17-03-05 EXCESS
}
URC close_minls(WORD type) {
    return close_file(type, &minls);                                        //SDH 10-03-2005 EXCESS
}

//////////////////////////////////////////////////////////////////////////////
///   ReadMinls
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadMinls(LONG lLineNumber) {                                          //SDH 10-03-2005 EXCESS
    return ReadKeyed(&minls, lLineNumber, LOG_CRITICAL);                    //SDH 10-03-2005 EXCESS
}                                                                           //SDH 10-03-2005 EXCESS

//////////////////////////////////////////////////////////////////////////////
///   WriteMinls
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteMinls(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&minls, lLineNumber, LOG_ALL);                        // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   DeleteMinlsRecord
///   Delete a record from the keyed file
//////////////////////////////////////////////////////////////////////////////
LONG DeleteMinlsRecord(LONG lLineNumber) {                                  //SDH 10-03-2005 EXCESS
    return DeleteKeyedRecord(&minls, lLineNumber, LOG_CRITICAL);            //SDH 10-03-2005 EXCESS
}                                                                           //SDH 10-03-2005 EXCESS

URC open_clolf(void) {
    return direct_open(&clolf, TRUE);                                       //SDH 10-03-2005 EXCESS
}
URC close_clolf(WORD type) {
    return close_file(type, &clolf);                                        //SDH 10-03-2005 EXCESS
}

URC open_clilf(void) {
    return keyed_open(&clilf, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_clilf(WORD type) {
    return close_file(type, &clilf);                                        //SDH 10-03-2005 EXCESS
}

//DEAL (Deal file)
//Routines to open and close the DEAL file.  This file is opened by
//background processing rather than to service a terminal, so there
//is no concept of sessions with HHTs and so the session count is
//only incremented when the file is physically opened
URC open_deal(void) {
    if (deal.sessions == 0) {
        deal.fnum = OpenKeyedFile(DEAL, DEAL_OFLAGS, DEAL_REP, TRUE);       //SDH 10-03-2005 EXCESS
        if (deal.fnum <= 0L) {
            if (debug) {
                sprintf(msg, "Err-O DEAL. RC:%08lX", deal.fnum);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
        deal.sessions = 1;
    }
    return RC_OK;
}

URC close_deal(void) {
    if (deal.sessions != 0) {
        s_close(0, deal.fnum);
        deal.sessions = 0;
    }
    return RC_OK;
}
//////////////////////////////////////////////////////////////////////////////
///   ReadDeal
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDeal(LONG lLineNumber) {                                           // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&deal, lLineNumber, LOG_ALL);                          // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   ReadDealQuick
///   Read the keyed file without a lock.  Don't do any debugging and
///   don't log any errors
//////////////////////////////////////////////////////////////////////////////
LONG ReadDealQuick(void) {                                                  // SDH 17-11-04 OSSR WAN
    return s_read(0, deal.fnum, deal.pBuffer, deal.wRecLth, deal.wKeyLth);  // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN

//TDTFF (Deal trickle file)
//Routines to open and close the TDTFF.  This file is opened by
//background processing rather than to service a terminal, so there
//is no concept of sessions with HHTs and so the session count is
//only incremented when the file is physically opened
URC open_tdtff(void) {
    if (tdtff.sessions == 0) {
        tdtff.fnum = OpenDirectFile(TDTFF, TDTFF_OFLAGS, TDTFF_REP, TRUE);  //SDH 10-03-2005 EXCESS
        if (tdtff.fnum <= 0L) {
            if (debug) {
                sprintf(msg, "Err-O TDTFF. RC:%08lX", tdtff.fnum);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
        tdtff.sessions = 1;
    }
    return RC_OK;
}

URC close_tdtff(void) {
    if (tdtff.sessions != 0) {
        s_close(0, tdtff.fnum);
        tdtff.sessions = 0;
    }
    return RC_OK;
}

LONG ReadTDTFFHeader(LONG lLineNumber) {

    LONG rc;                                                                // SDH 29-11-04 CREDIT CLAIM

    rc = s_read(A_BOFOFF, tdtff.fnum, (void*)(&TdtffHeader),                // SDH 29-11-04 CREDIT CLAIM
                sizeof(TdtffHeader), 0L);                                   // SDH 29-11-04 CREDIT CLAIM
    if (rc <= 0L) {                                                         // SDH 29-11-04 CREDIT CLAIM
        log_event101(rc, TDTFF_REP, lLineNumber);                           // SDH 29-11-04 CREDIT CLAIM
        if (debug) {                                                        // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Err-R TDTFF. RC:%08lX", rc);                      // SDH 29-11-04 CREDIT CLAIM
            disp_msg(msg);                                                  // SDH 29-11-04 CREDIT CLAIM
        }                                                                   // SDH 29-11-04 CREDIT CLAIM
    }                                                                       // SDH 29-11-04 CREDIT CLAIM

    return rc;                                                              // SDH 29-11-04 CREDIT CLAIM

}                                                                           // SDH 29-11-04 CREDIT CLAIM

URC open_jobok(void) {                                                      // 13-12-2004 PAB
    return direct_open(&jobok, FALSE);                                      // SDH 17-03-05 EXCESS
}                                                                           // 13-12-2004 PAB
URC close_jobok(WORD type) {                                                // 13-12-2004 PAB
    return close_file(type, &jobok);                                        // SDH 17-03-05 EXCESS
}

LONG open_recok(void) {                                                     // 24-5-2007 PAB
    return direct_open(&recok, TRUE);                                       // 24-5-2007 PAB
}                                                                           // 24-5-2007 PAB
URC close_recok(WORD type) {                                                // 24-5-2007 PAB
    return close_file(type, &recok);                                        // 24-5-2007 PAB
}

LONG open_rcindx(void) {                                                    // 24-5-2007 PAB
    return direct_open(&rcindx, TRUE);                                      // 24-5-2007 PAB
}                                                                           // 24-5-2007 PAB
URC close_rcindx(WORD type) {                                               // 24-5-2007 PAB
    return close_file(type, &rcindx);                                       // 24-5-2007 PAB
}

URC open_recall(void) {
    return keyed_open(&recall, TRUE);                                       // 25-5-07 PAB
}                                                                           // 25-5-07 PAB
URC close_recall(WORD type) {
    return close_file(type, &recall);                                       // 25-5-07 PAB
}
LONG ReadRecall(LONG lLineNumber) {                                         // Mult UOD Rcll 12-8-2011 SDH
    return ReadKeyed(&recall, lLineNumber, LOG_ALL);                        // Mult UOD Rcll 12-8-2011 SDH
}                                                                           // Mult UOD Rcll 12-8-2011 SDH
LONG WriteRecall(LONG lLineNumber) {                                        // 25-5-07 PAB
    return WriteKeyed(&recall, lLineNumber, LOG_ALL);
}

URC open_rcspi(void) {
    return keyed_open(&rcspi, TRUE);                                        // 25-5-07 PAB
}                                                                           // 25-5-07 PAB
URC close_rcspi(WORD type) {
    return close_file(type, &rcspi);                                        // 25-5-07 PAB
}

URC open_rfok(void) {                                                       // 24-8-2004 PAB
    return direct_open(&rfok, TRUE);                                        // SDH 17-03-05 EXCESS
}                                                                           // 24-8-2004 PAB
URC close_rfok(WORD type) {                                                 // 24-8-2004 PAB
    return close_file(type, &rfok);                                         // SDH 17-03-05 EXCESS
}

URC open_rfrdesc(void) {
    return direct_open(&rfrdesc, TRUE);                                     //SDH 10-03-2005 EXCESS
}
URC close_rfrdesc(WORD type) {
    return close_file(type, &rfrdesc);                                      //SDH 10-03-2005 EXCESS
}

URC open_stock(void) {
    return keyed_open(&stock, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_stock(WORD type) {
    return close_file(type, &stock);                                        //SDH 10-03-2005 EXCESS
}
LONG ReadStock(LONG lLineNumber) {                                          //SDH 10-03-2005 EXCESS
    return ReadKeyed(&stock, lLineNumber, LOG_CRITICAL);                    //SDH 10-03-2005 EXCESS
}                                                                           //SDH 10-03-2005 EXCESS

URC open_tsfD(void) {
    return direct_open(&tsf, TRUE);                                         //SDH 10-03-2005 EXCESS
}
URC close_tsf(WORD type) {
    return close_file(type, &tsf);                                          //SDH 10-03-2005 EXCESS
}

URC open_psbtD(void) {
    return direct_open(&psbt, TRUE);
}
URC close_psbt(WORD type) {
    return close_file(type, &psbt);                                         //SDH 10-03-2005 EXCESS
}

URC open_wrfD(void) {
    return direct_open(&wrf, TRUE);                                         //SDH 10-03-2005 EXCESS
}
URC close_wrf(WORD type) {
    return close_file(type, &wrf);                                          //SDH 10-03-2005 EXCESS
}

// v4.0 START
URC open_epsom(void) {
    return direct_open(&epsom, TRUE);                                       //SDH 10-03-2005 EXCESS
}
URC close_epsom(WORD type) {
    return close_file(type, &epsom);                                        //SDH 10-03-2005 EXCESS
}

URC open_pchk(void) {
    return direct_open(&pchk, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_pchk(WORD type) {
    return close_file(type, &pchk);                                         //SDH 10-03-2005 EXCESS
}
// v4.0 END

URC open_pilst(void) {
    return keyed_open(&pilst, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_pilst(WORD type) {
    return close_file(type, &pilst);                                        //SDH 10-03-2005 EXCESS
}

URC open_piitm(void) {
    return keyed_open(&piitm, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_piitm(WORD type) {
    return close_file(type, &piitm);                                        //SDH 10-03-2005 EXCESS
}

URC open_rfsstat(void) {

    LONG rc;
    SMTABLE sharetab;                                                       // Streamline SDH 23-Sep-2008

    if (rfsstat.sessions==0) {
        // Create a block of shared memory
        rfsstat.fnum = s_create( 0,
                                 A_WRITE | A_READ | A_SHARE | A_DELETE | A_TEMP,
                                 SM_BUFFER_NAME, 0, 0,
                                 (LONG)MAX_UNIT * (LONG)sizeof(SHARE_STAT_REC) );
        if (rfsstat.fnum<0L) {
            //log_event101(rfsstat.fnum, 0, __LINE__);
            return RC_DATA_ERR;
        }
        rc = s_get( T_SHMEM, rfsstat.fnum, (SMTABLE *)&sharetab, SMSIZE );
        if (rc<0L) {
            log_event101(rc, 0, __LINE__);
            s_close( 0, rfsstat.fnum );
            rfsstat.fnum=-1L;
            return RC_DATA_ERR;
        }
        share = (SHARE_STAT_REC *)sharetab.ubuffer;
        // Initialise
        memset( share, 0x00, (WORD)MAX_UNIT * sizeof(SHARE_STAT_REC) );
        // Unlock semaphore(this must be done, otherwise initial lock will block)
        rc = s_write( 0, rfsstat.fnum, "", 1, 1 );
        if (rc<0L) {
            log_event101(rc, 0, __LINE__);
            s_close( 0, rfsstat.fnum );
            rfsstat.fnum=-1L;
            return RC_DATA_ERR;
        }

        if (rfsstat.fnum<=0L) {
            if (debug) {
                sprintf(msg, "Err-C rfsstat. RC:%08lX", rfsstat.fnum);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
    }
    rfsstat.sessions+=1;
    return RC_OK;
}

URC close_rfsstat(WORD type) {
    return close_file(type, &rfsstat);                                      //SDH 10-03-2005 EXCESS
}

// Include EALSUSPT suspended transaction file for Pre-Scan
URC open_suspt(void) {
    return keyed_open(&suspt, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_suspt(WORD type) {
    return close_file(type, &suspt);                                        //SDH 10-03-2005 EXCESS
}


URC process_jobok(void) {                                             // 13-12-04 PAB

    URC usrrc = RC_OK;
                                                                      // 13-12-04 PAB
    // verify that batch suite is not running                         // 13-12-04 PAB
    usrrc = open_jobok();                                             // 13-12-04 PAB
    if ( usrrc<=RC_DATA_ERR) {                                        // 13-12-04 PAB
       memset( (BYTE *)&jobokrec,  0x00, sizeof(JOBOK_REC)  );        // PAB 13-12-04
       return RC_DATA_ERR;                                            // 13-12-04 PAB
    }                                                                 // 13-12-04 PAB
    // read the current status record & close the file                // 13-12-04 PAB
    usrrc = s_read( A_BOFOFF,                                         // 13-12-04 PAB
         jobok.fnum, (void *)&jobokrec, JOBOK_RECL, 0L);              // 13-12-04 PAB
    if ( usrrc<=0L ){                                                 // 13-12-04 PAB
         log_event101(usrrc, JOBOK_REP, __LINE__);                    // 13-12-04 PAB
         printf("ERR-R JOBOK. RC:%081X", usrrc);                      // 13-12-04 PAB
    }                                                                 // 13-12-04 PAB
                                                                      // 13-12-04 PAB
    close_jobok ( CL_SESSION );                                       // 13-12-04 PAB
    return RC_OK;                                                     // 13-12-04 PAB
}                                                                     // 13-12-04 PAB

URC process_rfok(void)                                                // 04-11-04 PAB
{                                                                     // 04-11-04 PAB
    URC usrrc = RC_OK;                                                // 21-12-04 SDH
    LONG rc;                                                          // 21-12-04 SDH
    // 04-11-04 PAB
    // verify that batch suite is not running                         // 04-11-04 PAB
    usrrc = open_rfok();                                              // 24-08-04 PAB
    if (usrrc<=RC_DATA_ERR) {                                         // 24-08-04 PAB
        return RC_DATA_ERR;                                           // 04-11-04 PAB
    }                                                                 // 24-08-04 PAB
    // read the current status record & close the file                // 24-08-04 PAB
    rc = s_read( A_BOFOFF,                                            // 24-08-04 PAB
                 rfok.fnum, (void *)&rfokrec, RFOK_RECL, 0L);         // 24-08-04 PAB
    close_rfok ( CL_SESSION );                                        // 24-08-04 PAB
    if (rc <= 0L) {                                                   // 21-12-04 SDH
        log_event101(rc, RFOK_REP, __LINE__);                         // 21-12-04 SDH
        printf("ERR-R RFOK. RC:%081X", rc);                           // 24-08-04 PAB
        return RC_DATA_ERR;                                           // 21-12-04 SDH
    }                                                                 // 24-08-04 PAB
    // 24-08-04 PAB

    return RC_OK;                                                     // 04-11-04 PAB
}                                                                     // 04-11-04 PAB


// Prepare LRT log & RFDEBUG log file - open or create if not present
void prepare_logging()
{
    LONG lRc;                                                         //CSk 15-04-2010 POD Logging

    // Open LRTLG
    lrtlg.fnum=s_open(LRTLG_OFLAGS, LRTLG);
    if (lrtlg.fnum<=0L) {
        if ((lrtlg.fnum&0xFFFF)==0x4010) {
            // Create LRTLG
            lrtlg.fnum=s_create( O_FILE, LRTLG_CFLAGS,
                                 LRTLG, LRTLG_RECL, 0x0FFF, 0);
            if (lrtlg.fnum<=0L) {
                //log_event101(lrtlg.fnum, LRTLG_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-C LRTLG. RC:%08lX", lrtlg.fnum);
                    disp_msg(msg);
                }
            } else {
                s_close(0, lrtlg.fnum);
                lrtlg.fnum=s_open(LRTLG_OFLAGS, LRTLG);
                if (lrtlg.fnum<=0L) {
                    log_event101(lrtlg.fnum, LRTLG_REP, __LINE__);
                    if (debug) {
                        sprintf(msg, "Err-O LRTLG. RC:%08lX", lrtlg.fnum);
                        disp_msg(msg);
                    }
                }
            }
        } else {
            log_event101(lrtlg.fnum, LRTLG_REP, __LINE__);
            if (debug) {
                sprintf(msg, "Err-O LRTLG. RC:%08lX", lrtlg.fnum);
                disp_msg(msg);
            }
        }
    }

    // Open RFDEBUG
    dbg.fnum=s_open(DBG_OFLAGS, DBG);
    if (dbg.fnum<=0L) {
        if ((dbg.fnum&0xFFFF)==0x4010) {
            // Create RFDEBUG
            dbg.fnum=s_create( O_FILE, DBG_CFLAGS,
                               DBG, DBG_RECL, 0x0FFF, 0);
            if (dbg.fnum<=0L) {
                log_event101(dbg.fnum, DBG_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-C DBG. RC:%08lX", dbg.fnum);
                    disp_msg(msg);
                }
            } else {
                s_close(0, dbg.fnum);
                dbg.fnum=s_open(DBG_OFLAGS, DBG);
                if (dbg.fnum<=0L) {
                    log_event101(dbg.fnum, DBG_REP, __LINE__);
                    if (debug) {
                        sprintf(msg, "Err-O DBG. RC:%08lX", dbg.fnum);
                        disp_msg(msg);
                    }
                }
            }
        } else {
            log_event101(dbg.fnum, DBG_REP, __LINE__);
            if (debug) {
                sprintf(msg, "Err-O DBG. RC:%08lX", dbg.fnum);
                disp_msg(msg);
            }
        }
    }

    // Open or Create PODOK                                             //CSk 15-04-2010 POD Logging
    PodokOpen();                                                        //CSk 15-04-2010 POD Logging

    if (podok.fnum<=0L) {                                               //CSk 15-04-2010 POD Logging
        if ((podok.fnum&0xFFFF)==0x4010) {                              //CSk 15-04-2010 POD Logging
                                                                        //CSk 15-04-2010 POD Logging
            lRc = CreateKeyedFile (                                     //CSk 15-04-2010 POD Logging
                             podok.pbFileName, // Filename              //CSk 15-04-2010 POD Logging
                             PODOK_REP,        // File Report No.       //CSk 15-04-2010 POD Logging
                             (ULONG)100,       // No. of recs           //CSk 15-04-2010 POD Logging
                             (ULONG)99,        // Randomising Divisor   //CSk 15-04-2010 POD Logging
                             (ULONG)4,         // Chaining Threshold    //CSk 15-04-2010 POD Logging
                             (UBYTE)2,         // Hashing Algorithm     //CSk 15-04-2010 POD Logging
                             (UWORD)PODOK_RECL,// RecordSize            //CSk 15-04-2010 POD Logging
                             (UWORD)12,        // Key Length            //CSk 15-04-2010 POD Logging
                             (UWORD)2,         // Distribution Type     //CSk 15-04-2010 POD Logging
                             (UBYTE)0          // Overwrite Current file//CSk 15-04-2010 POD Logging
                  );                                                    //CSk 15-04-2010 POD Logging
                                                                        //CSk 15-04-2010 POD Logging
            if (lRc < 0L) {                                             //CSk 15-04-2010 POD Logging
                log_event101(podok.fnum, PODOK_REP, __LINE__);          //CSk 15-04-2010 POD Logging
                if (debug) {                                            //CSk 15-04-2010 POD Logging
                    sprintf(msg, "Err-C PODOK. RC:%08lX", podok.fnum);  //CSk 15-04-2010 POD Logging
                    disp_msg(msg);                                      //CSk 15-04-2010 POD Logging
                }                                                       //CSk 15-04-2010 POD Logging
            } else {                                                    //CSk 15-04-2010 POD Logging
                PodokOpen();                                            //CSk 15-04-2010 POD Logging
                if (podok.fnum<=0L) {                                   //CSk 15-04-2010 POD Logging
                    log_event101(podok.fnum, PODOK_REP, __LINE__);      //CSk 15-04-2010 POD Logging
                    if (debug) {                                        //CSk 15-04-2010 POD Logging
                        sprintf(msg, "Err-O PODOK. RC:%08lX", podok.fnum);//CSk 15-04-2010 POD Logging
                        disp_msg(msg);                                  //CSk 15-04-2010 POD Logging
                    }                                                   //CSk 15-04-2010 POD Logging
                }                                                       //CSk 15-04-2010 POD Logging
            }                                                           //CSk 15-04-2010 POD Logging
        } else {                                                        //CSk 15-04-2010 POD Logging
            log_event101(podok.fnum, PODOK_REP, __LINE__);              //CSk 15-04-2010 POD Logging
            if (debug) {                                                //CSk 15-04-2010 POD Logging
                sprintf(msg, "Err-O PODOK. RC:%08lX", podok.fnum);      //CSk 15-04-2010 POD Logging
                disp_msg(msg);                                          //CSk 15-04-2010 POD Logging
            }                                                           //CSk 15-04-2010 POD Logging
        }                                                               //CSk 15-04-2010 POD Logging
    }                                                                   //CSk 15-04-2010 POD Logging

    // Open or Create PODLOG                                            //CSk 15-04-2010 POD Logging
    PodlogOpen();                                                       //CSk 15-04-2010 POD Logging
    if (podlog.fnum<=0L) {                                              //CSk 15-04-2010 POD Logging
        if ((podlog.fnum&0xFFFF)==0x4010) {                             //CSk 15-04-2010 POD Logging
            // Create PODLOG                                            //CSk 15-04-2010 POD Logging
            podlog.fnum=s_create( O_FILE, PODLOG_CFLAGS,                //CSk 15-04-2010 POD Logging
                               PODLOG, PODLOG_RECL, 0x0FFF, 0);         //CSk 15-04-2010 POD Logging
            if (podlog.fnum<=0L) {                                      //CSk 15-04-2010 POD Logging
                log_event101(podlog.fnum, PODLOG_REP, __LINE__);        //CSk 15-04-2010 POD Logging
                if (debug) {                                            //CSk 15-04-2010 POD Logging
                    sprintf(msg, "Err-C PODLOG. RC:%08lX", podlog.fnum);//CSk 15-04-2010 POD Logging
                    disp_msg(msg);                                      //CSk 15-04-2010 POD Logging
                }                                                       //CSk 15-04-2010 POD Logging
            } else {                                                    //CSk 15-04-2010 POD Logging
                s_close(0, podlog.fnum);                                //CSk 15-04-2010 POD Logging
                PodlogOpen();                                           //CSk 15-04-2010 POD Logging
                if (podlog.fnum<=0L) {                                  //CSk 15-04-2010 POD Logging
                    log_event101(podlog.fnum, PODLOG_REP, __LINE__);    //CSk 15-04-2010 POD Logging
                    if (debug) {                                        //CSk 15-04-2010 POD Logging
                        sprintf(msg, "Err-O PODLOG. RC:%08lX", podlog.fnum);//CSk 15-04-2010 POD Logging
                        disp_msg(msg);                                  //CSk 15-04-2010 POD Logging
                    }                                                   //CSk 15-04-2010 POD Logging
                }                                                       //CSk 15-04-2010 POD Logging
            }                                                           //CSk 15-04-2010 POD Logging
        } else {                                                        //CSk 15-04-2010 POD Logging
            log_event101(podlog.fnum, PODLOG_REP, __LINE__);            //CSk 15-04-2010 POD Logging
            if (debug) {                                                //CSk 15-04-2010 POD Logging
                sprintf(msg, "Err-O PODLOG. RC:%08lX", podlog.fnum);    //CSk 15-04-2010 POD Logging
                disp_msg(msg);                                          //CSk 15-04-2010 POD Logging
            }                                                           //CSk 15-04-2010 POD Logging
        }                                                               //CSk 15-04-2010 POD Logging
    }                                                                   //CSk 15-04-2010 POD Logging

    // Open PATRACE
    if (pst.fnum==0L) {
        pst.fnum=s_open(PST_OFLAGS, PST);
    }
    if (pst.fnum<=0L) {
        if ((pst.fnum&0xFFFF)==0x4010) {
            // Create PATRACE
            pst.fnum=s_create( O_FILE, PST_CFLAGS,
                               PST, PST_RECL, 0x0FFF, 0);
            if (pst.fnum<=0L) {
                log_event101(pst.fnum, PST_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-C PST. RC:%08lX", pst.fnum);
                    disp_msg(msg);
                }
            } else {
                s_close(0, pst.fnum);
                pst.fnum=s_open(PST_OFLAGS, PST);
                if (pst.fnum<=0L) {
                    log_event101(pst.fnum, PST_REP, __LINE__);
                    if (debug) {
                        sprintf(msg, "Err-O PST. RC:%08lX", pst.fnum);
                        disp_msg(msg);
                    }
                }
            }
        } else {
            log_event101(pst.fnum, PST_REP, __LINE__);
            if (debug) {
                sprintf(msg, "Err-O PST. RC:%08lX", pst.fnum);
                disp_msg(msg);
            }
        }
    }

}


URC alloc_report_buffer( RBUF **rbufp )
{
    LONG rc;
    MPB mem;

    // Allocate RBUF instance
    mem.mp_start = 0L;
    mem.mp_min = (LONG)sizeof(RBUF);
    mem.mp_max = mem.mp_min;
    rc = s_malloc( (UBYTE)O_NEWHEAP, (MPB *)&mem );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        *rbufp = (void *)NULL;
        return RC_SERIOUS_ERR;
    } else {
        if (debug) {
            sprintf(msg, "rbuf allocated ok : %08lX", (void *)mem.mp_start);
            disp_msg(msg);
        }
        *rbufp = (RBUF *)mem.mp_start;
    }

    // Allocate report buffer
    mem.mp_start = 0L;
    mem.mp_min = (LONG)REPORT_BUFFER;
    mem.mp_max = mem.mp_min;
    rc = s_malloc( (UBYTE)O_NEWHEAP, (MPB *)&mem );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    } else {
        if (debug) {
            sprintf(msg, "buffer allocated ok : %08lX", (void *)mem.mp_start);
            disp_msg(msg);
        }
        (*rbufp)->base = -1L;
        (*rbufp)->end = -1L;
        (*rbufp)->buff = (BYTE *)mem.mp_start;
        return RC_OK;
    }

}

URC dealloc_report_buffer( RBUF *rbufp )
{
    LONG rc;

    // Deallocate report buffer
    rc = s_mfree( (void *)rbufp->buff );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }
    rbufp->buff = (void *)NULL;

    // Deallocate RBUF instance
    rc = s_mfree( (void *)rbufp );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        return RC_SERIOUS_ERR;
    }

    return RC_OK;
}



URC create_new_plist( BYTE *ret_list_id, BYTE *user ) {

    B_DATE nowDate;                                                         // SDH 26-11-04 CREDIT CLAIM
    B_TIME nowTime;                                                         // SDH 26-11-04 CREDIT CLAIM
    WORD wListId;                                                           // SDH 26-11-04 CREDIT CLAIM
    LONG rc;
    //BYTE sbuf[64];

    // Read pllol header
    rc = ReadPllolLock(0L, __LINE__);                                       // SDH 20-May-2009 Model Day
    if (rc <= 0L) return RC_DATA_ERR;                                       // SDH 26-11-04 CREDIT CLAIM

    // Get next list id
    wListId = (satoi(pllolrec.list_id, 3)%999) + 1;                         // SDH 26-11-04 CREDIT CLAIM
    sprintf( sbuf, "%03d", wListId);                                        // SDH 26-11-04 CREDIT CLAIM
    memcpy( pllolrec.list_id, sbuf, 3 );

    // Update pllol header
    rc = WritePllolUnlock(0L, __LINE__);                                    // SDH 20-May-2009 Model Day
    if (rc<=0L) return RC_DATA_ERR;

    // Write new pllol record
    memset( (BYTE *)&pllolrec, 0x00, PLLOL_RECL );
    sprintf(sbuf, "%03d", wListId);                                         // SDH 26-11-04 CREDIT CLAIM
    memcpy( &pllolrec.list_id, sbuf, 3 );
    memcpy( pllolrec.creator, user, 3 );
    memset( pllolrec.picker, '0', 3 );                                      // SDH 26-11-04 CREDIT CLAIM
    memcpy( pllolrec.list_status, "A", 1 );                                 // Status : Active    // 19-0504 PAB

    //Get date/time
    GetSystemDate(&nowTime, &nowDate);                                      // SDH 26-11-04 CREDIT CLAIM
    sprintf( sbuf, "%02d%02d%02d",                                          // SDH 26-11-04 CREDIT CLAIM
             nowDate.wYear%100, nowDate.wMonth, nowDate.wDay);              // SDH 26-11-04 CREDIT CLAIM
    memcpy( pllolrec.create_date, sbuf, 6 );
    sprintf( sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin);                // SDH 26-11-04 CREDIT CLAIM
    memcpy( pllolrec.create_time, sbuf, 4 );

    memset( pllolrec.pick_start_time, '0', 4);                              // SDH 26-11-04 CREDIT CLAIM
    memset( pllolrec.pick_end_time, '0', 4);                                // SDH 26-11-04 CREDIT CLAIM
    memset( pllolrec.item_count, '0', 4);                                   // SDH 26-11-04 CREDIT CLAIM
    pllolrec.cLocation = 'N';                                               // 31-8-04 PAB OSSR

    rc = WritePllol(wListId, __LINE__);                                     // SDH 26-11-04 CREDIT CLAIM
    if (rc<=0L) return RC_DATA_ERR;                                         // SDH 26-11-04 CREDIT CLAIM

    // Return new list id in 'ret_list_id'
    sprintf(sbuf, "%03d", wListId);                                         // SDH 26-11-04 CREDIT CLAIM
    memcpy(ret_list_id, sbuf, 3);

    return RC_OK;

}


// Create a unique workfile for a handheld to use during it's session
URC prepare_workfile(WORD log_unit, BYTE type) {

    WORD i, wFoundEntry;
    LONG ltime, rc;                                                     // 11-09-2007 13 BMG
    TIMEDATE now;
    BYTE gapbfrec[32];
    //BYTE sbuf[64];                                                    // Null fix SDH 29-Sep-2009
    time_t CurrTime;                                                    // 11-09-2007 13 BMG

    // Get current time
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                          // 11-09-2007 13 BMG
    ltime = now.td_time;

    sprintf( msg, "unit = %d, time = %ld\n", log_unit, ltime );         // 11-09-2007 13 BMG
    disp_msg(msg);

    //Do nothing if a workfile already exists
    if (type == SYS_LAB) {
        rc = lrtp[log_unit]->fnum1;
    } else if (type == SYS_CB) {                                        // BMG 01-09-2008 17 ASN/Directs
        rc = lrtp[log_unit]->lCBfnum;                                   // BMG 01-09-2008 17 ASN/Directs
    } else if (type == SYS_PUB) {                                       // BMG 01-09-2008 17 ASN/Directs
        rc = lrtp[log_unit]->lPUBfnum;                                  // BMG 01-09-2008 17 ASN/Directs
    } else if (type == SYS_DIR) {                                       // BMG 01-09-2008 17 ASN/Directs
        rc = lrtp[log_unit]->lDIRfnum;                                  // BMG 01-09-2008 17 ASN/Directs
    } else {                                                            // BMG 01-09-2008 17 ASN/Directs
        rc = lrtp[log_unit]->fnum2;
    }
    if (rc > 0) return RC_OK;

    // Locate a free entry
    //Always leave element 0 free, to protect against units using getting   // Null fix SDH 29-Sep-2009
    //confused between their default value of 0, and a real 0               // Null fix SDH 29-Sep-2009
    wFoundEntry = -1;
    for (i=1; i<MAX_CONC_UNITS && wFoundEntry == -1; i++) {                 // Null fix SDH 29-Sep-2009
        if (pq[i].state==PST_FREE) {

            wFoundEntry = i;

            // Set process queue entry to allocated and fill in table entry,
            pq[i].state = PST_ALLOC;
            pq[i].unit = log_unit;
            pq[i].submitcnt = 0;                                        // 26-1-2007 PAB
            memcpy(pq[i].DevType, lrtp[log_unit]->Type, 1);             // BMG 16-09-2008 18 MC70

            time(&CurrTime);                                            // 11-09-2007 13 BMG
            pq[i].last_access_time = CurrTime;                          // 11-09-2007 13 BMG

            // Generate a unique filename, appropriate to current 'type'
            switch (type) {
            case SYS_LAB:
                sprintf( pq[i].fname, "CHKWK:%06ld.%03d",               // Null fix SDH 29-Sep-2009
                         ((ltime/100L)%999999L), log_unit );            // 11-09-2007 13 BMG
                break;
            case SYS_GAP:
                sprintf( pq[i].fname, "GAPWK:%06ld.%03d",               // Null fix SDH 29-Sep-2009
                         ((ltime/100L)%999999L), log_unit );            // 11-09-2007 13 BMG
                break;
            case SYS_CB:                                                // BMG 01-09-2008 17 ASN/Directs
                sprintf( pq[i].fname, "WC%06ld.%03d",                   // Null fix SDH 29-Sep-2009
                         ((ltime/100L)%999999L), log_unit );            // BMG 01-09-2008 17 ASN/Directs
                break;                                                  // BMG 01-09-2008 17 ASN/Directs
            case SYS_PUB:                                               // BMG 01-09-2008 17 ASN/Directs
                sprintf( pq[i].fname, "WU%06ld.%03d",                   // Null fix SDH 29-Sep-2009
                         ((ltime/100L)%999999L), log_unit );            // BMG 01-09-2008 17 ASN/Directs
                break;                                                  // BMG 01-09-2008 17 ASN/Directs
            case SYS_DIR:                                               // BMG 01-09-2008 17 ASN/Directs
                sprintf( pq[i].fname, "WD%06ld.%03d",                   // Null fix SDH 29-Sep-2009
                         ((ltime/100L)%999999L), log_unit );            // BMG 01-09-2008 17 ASN/Directs
                break;                                                  // BMG 01-09-2008 17 ASN/Directs
            }
            pq[i].type = type;

            // Create workfile ready for use and link pq into lrtp
            switch (type) {

            case SYS_LAB:
                lrtp[log_unit]->pq_sub1 = i;
                lrtp[log_unit]->fnum1 =
                s_create( O_FILE, SELBF_CFLAGS,
                          pq[lrtp[log_unit]->pq_sub1].fname,
                          SELBF_RECL, 0x0FFF, 0 );
                //wk_fnum=lrtp[log_unit]->fnum1;
                log_file_open (lrtp[log_unit]->fnum1,'W');                  // 07-04-04 PAB
                if (lrtp[log_unit]->fnum1 < 0L) {
                    if (debug) {
                        sprintf( msg, "Err-C %s. RC:%08lX",
                                 pq[lrtp[log_unit]->pq_sub1].fname,
                                 lrtp[log_unit]->fnum1 );
                        disp_msg( msg );
                    }
                    return RC_DATA_ERR;
                }
                break;

            case SYS_GAP:
                lrtp[log_unit]->pq_sub2 = i;
                lrtp[log_unit]->fnum2 =
                s_create( O_FILE, GAPBF_CFLAGS,
                          pq[lrtp[log_unit]->pq_sub2].fname,
                          GAPBF_RECL, 0x0FFF, 0 );
                //wg_fnum=lrtp[log_unit]->fnum2;
                log_file_open (lrtp[log_unit]->fnum2,'W');                  // 07-04-04 PAB
                if (lrtp[log_unit]->fnum2 < 0L) {
                    if (debug) {
                        sprintf( msg, "Err-C %s. RC:%08lX",
                                 pq[lrtp[log_unit]->pq_sub2].fname,
                                 lrtp[log_unit]->fnum2 );
                        disp_msg( msg );
                    }
                    return RC_DATA_ERR;
                }
                if (memcmp(lrtp[hh_unit]->Type, "M", 1)) {                      // 09-04-09 BMG 1.1
                    // Device is NOT an MC70 so write standard header record    // 09-04-09 BMG 1.1
                    *(gapbfrec) = 0x22;
                    memcpy( (BYTE *)&gapbfrec+1, lrtp[log_unit]->user, 3 );
                    memcpy( (BYTE *)&gapbfrec+4, "000000000", 9 );
                    *(gapbfrec+13) = 0x22;
                    *(gapbfrec+14) = 0x0D;
                    *(gapbfrec+15) = 0x0A;
                    if (debug) {
                        disp_msg( "WR GAPBF :" );
                        dump( (BYTE *)&gapbfrec, 16L );
                    }
                    rc = s_write( A_FLUSH | A_FPOFF, lrtp[log_unit]->fnum2,
                                  (void *)&gapbfrec, 16L, 0L );
                    if (rc<=0L) {
                        log_event101(rc, GAPBF_REP, __LINE__);
                        if (debug) {
                            sprintf( msg, "Err-W to %s. RC:%08lX",
                                     pq[lrtp[log_unit]->pq_sub2].fname, rc );
                            disp_msg(msg);
                        }
                        return RC_DATA_ERR;
                    }
                }                                                               // 09-04-09 BMG 1.1
                break;

            case SYS_CB:                                                                // BMG 01-09-2008 17 ASN/Directs
                lrtp[log_unit]->pq_CB = i;                                              // BMG 01-09-2008 17 ASN/Directs
                lrtp[log_unit]->lCBfnum =                                               // BMG 01-09-2008 17 ASN/Directs
                s_create( O_FILE, CBBF_CFLAGS,                                          // BMG 01-09-2008 17 ASN/Directs
                          pq[lrtp[log_unit]->pq_CB].fname,                              // BMG 01-09-2008 17 ASN/Directs
                          CBBF_RECL, 0x0FFF, 0 );                                       // BMG 01-09-2008 17 ASN/Directs
                log_file_open (lrtp[log_unit]->lCBfnum,'W');                            // BMG 01-09-2008 17 ASN/Directs
                if (lrtp[log_unit]->lCBfnum < 0L) {                                     // BMG 01-09-2008 17 ASN/Directs
                    if (debug) {                                                        // BMG 01-09-2008 17 ASN/Directs
                        sprintf( msg, "Err-C %s. RC:%08lX",                             // BMG 01-09-2008 17 ASN/Directs
                                 pq[lrtp[log_unit]->pq_CB].fname,                       // BMG 01-09-2008 17 ASN/Directs
                                 lrtp[log_unit]->lCBfnum );                             // BMG 01-09-2008 17 ASN/Directs
                        disp_msg( msg );                                                // BMG 01-09-2008 17 ASN/Directs
                    }                                                                   // BMG 01-09-2008 17 ASN/Directs
                    return RC_DATA_ERR;                                                 // BMG 01-09-2008 17 ASN/Directs
                }                                                                       // BMG 01-09-2008 17 ASN/Directs
                break;                                                                  // BMG 01-09-2008 17 ASN/Directs

            case SYS_PUB:                                                               // BMG 01-09-2008 17 ASN/Directs
                lrtp[log_unit]->pq_PUB = i;                                             // BMG 01-09-2008 17 ASN/Directs
                lrtp[log_unit]->lPUBfnum =                                              // BMG 01-09-2008 17 ASN/Directs
                s_create( O_FILE, PUBBF_CFLAGS,                                         // BMG 01-09-2008 17 ASN/Directs
                          pq[lrtp[log_unit]->pq_PUB].fname,                             // BMG 01-09-2008 17 ASN/Directs
                          PUBBF_RECL, 0x0FFF, 0 );                                      // BMG 01-09-2008 17 ASN/Directs
                log_file_open (lrtp[log_unit]->lPUBfnum,'W');                           // BMG 01-09-2008 17 ASN/Directs
                if (lrtp[log_unit]->lPUBfnum < 0L) {                                    // BMG 01-09-2008 17 ASN/Directs
                    if (debug) {                                                        // BMG 01-09-2008 17 ASN/Directs
                        sprintf( msg, "Err-C %s. RC:%08lX",                             // BMG 01-09-2008 17 ASN/Directs
                                 pq[lrtp[log_unit]->pq_PUB].fname,                      // BMG 01-09-2008 17 ASN/Directs
                                 lrtp[log_unit]->lPUBfnum );                            // BMG 01-09-2008 17 ASN/Directs
                        disp_msg( msg );                                                // BMG 01-09-2008 17 ASN/Directs
                    }                                                                   // BMG 01-09-2008 17 ASN/Directs
                    return RC_DATA_ERR;                                                 // BMG 01-09-2008 17 ASN/Directs
                }                                                                       // BMG 01-09-2008 17 ASN/Directs
                break;                                                                  // BMG 01-09-2008 17 ASN/Directs

            case SYS_DIR:                                                               // BMG 01-09-2008 17 ASN/Directs
                lrtp[log_unit]->pq_DIR = i;                                             // BMG 01-09-2008 17 ASN/Directs
                lrtp[log_unit]->lDIRfnum =                                              // BMG 01-09-2008 17 ASN/Directs
                s_create( O_FILE, DIRBF_CFLAGS,                                         // BMG 01-09-2008 17 ASN/Directs
                          pq[lrtp[log_unit]->pq_DIR].fname,                             // BMG 01-09-2008 17 ASN/Directs
                          DIRBF_RECL, 0x0FFF, 0 );                                      // BMG 01-09-2008 17 ASN/Directs
                log_file_open (lrtp[log_unit]->lDIRfnum,'W');                           // BMG 01-09-2008 17 ASN/Directs
                if (lrtp[log_unit]->lDIRfnum < 0L) {                                    // BMG 01-09-2008 17 ASN/Directs
                    if (debug) {                                                        // BMG 01-09-2008 17 ASN/Directs
                        sprintf( msg, "Err-C %s. RC:%08lX",                             // BMG 01-09-2008 17 ASN/Directs
                                 pq[lrtp[log_unit]->pq_DIR].fname,                      // BMG 01-09-2008 17 ASN/Directs
                                 lrtp[log_unit]->lDIRfnum );                            // BMG 01-09-2008 17 ASN/Directs
                        disp_msg( msg );                                                // BMG 01-09-2008 17 ASN/Directs
                    }                                                                   // BMG 01-09-2008 17 ASN/Directs
                    return RC_DATA_ERR;                                                 // BMG 01-09-2008 17 ASN/Directs
                }                                                                       // BMG 01-09-2008 17 ASN/Directs
                break;                                                                  // BMG 01-09-2008 17 ASN/Directs

            }
        }
    }

    if (wFoundEntry == -1) {
        disp_msg("No free units");
        return RC_SERIOUS_ERR;
    }

    sprintf(msg, "Unit %d allocated [%s] in slot %d, type %d",              //SDH Bug fix 3-Oct-2006
            log_unit, pq[wFoundEntry].fname, wFoundEntry, type);            //SDH Bug fix 3-Oct-2006
    disp_msg(msg);
    return RC_OK;

}


// return filesize of a file (only accurate below 32 bits)
LONG filesize( BYTE *fname ) {
    LONG rc;
    DISKFILE dir;
    rc = s_lookup( T_FILE, A_FORCE, fname, (void *)&dir, sizeof(DISKFILE),
                   sizeof(DISKFILE), 0L );
    if (rc>0) {
        // found file
        return(dir.df_size);
    } else {
        // file not found
        return(-1);                                                         //SDH Bug Fix 29-July-2006
    }
}



// return month of a file                                                  //CSk 15-04-2010 POD Logging
UBYTE MonthKeyedFileCreated( BYTE *fname ) {                               //CSk 15-04-2010 POD Logging
    LONG rc;                                                               //CSk 15-04-2010 POD Logging
    DISKFILE dir;                                                          //CSk 15-04-2010 POD Logging
    rc = s_lookup( T_FILE, A_FORCE, fname, (void *)&dir, sizeof(DISKFILE),
                   sizeof(DISKFILE), 0L );                                 //CSk 15-04-2010 POD Logging
    if (rc>0) {                                                            //CSk 15-04-2010 POD Logging
        // found file                                                      //CSk 15-04-2010 POD Logging
        return(dir.df_modmonth);                                           //CSk 15-04-2010 POD Logging
    } else {                                                               //CSk 15-04-2010 POD Logging
        // file not found                                                  //CSk 15-04-2010 POD Logging
        return(0);                                                         //CSk 15-04-2010 POD Logging
    }                                                                      //CSk 15-04-2010 POD Logging
}                                                                          //CSk 15-04-2010 POD Logging


/* SDH 5-May-2006  Removed redundant routine
LONG rename_workfile(BYTE *curr_name, UBYTE type)
{
    LONG rc;
    if (type==SYS_LAB) {
        rc = s_rename(0, curr_name, "SELBF");
        if (rc<0) {
            // ok its there try to delete it see if PRINTSEL is stallaed
            s_delete(0, "SELBF");
            //log_event101(rc, SELBF_REP, __LINE__);
        }
    } else {
        rc = s_rename(0, curr_name, "GAPBF");
        if (rc<0) {
            // it there try to delete it see if PSS47 is stalled
            s_delete(0, "GAPBF");
            //log_event101(rc, GAPBF_REP, __LINE__);
        }
    }
    return rc;
}
*/


URC pllol_get_next( /*WORD log_unit,*/ BYTE *list_id, LRT_PLL *pllp ) {

    UBYTE repeat;
    LONG rc, rec;
    URC found, usrrc;
    BYTE sbuf[64];

    rec = satol( list_id, 3 );
    do {
        repeat = FALSE;
        rc = ReadPllolLog(rec, __LINE__, LOG_CRITICAL);                     // SDH 29-05-2005
        if (rc<=0L) {
            pllol.present=FALSE;
            if ((rc&0xFFFF)==0x4003) {
                found = 0;
            } else {
                return RC_DATA_ERR;
            }
        } else {
            // Record found
            pllol.present=TRUE;
            // Ignore deleted lists
            if (*pllolrec.list_status == 'L' ||
                *pllolrec.list_status == 'P' ||
                *pllolrec.list_status == 'X' ||
                *pllolrec.item_count == 0x00000000) {                       // flush empty lists
                found = 0;
                repeat = TRUE;
            } else {
                found = 1;
            }
            // Remove security on picking list access
            //   if ( *lrtp[log_unit]->authority == ' ' ) {
            //    if ( *pllolrec.list_status != 'U' ) {
            //     found = 0;
            //     repeat = TRUE;
            //    }
            //   } else {
            //    if ( *pllolrec.list_status != 'A' &&
            //      *pllolrec.list_status != 'U' ) {
            //     found = 0;
            //     repeat = TRUE;
            //    }
            //   }
        }
        rec++;
    } while (repeat);

    if (found==1) {

        //If the list was automatically created then get the business       //SDH 20-May-2009 Model Day
        //centre description rather than the username                       //SDH 20-May-2009 Model Day
        //If an all store list then just set the desc to AUTO-ALL           //SDH 20-May-2009 Model Day
        if (pllolrec.cLocation == 'A') {                                    //SDH 20-May-2009 Model Day
            WORD wCreator = satoi(pllolrec.creator, sizeof(pllolrec.creator));//SDH 20-May-2009 Model Day
            memcpy(afrec.operator_name, "AUTO-          ", 15);             //SDH 20-May-2009 Model Day
            if (wCreator == 0) {                                            //SDH 20-May-2009 Model Day
                memcpy(afrec.operator_name + 5, "ALL", 3);                  //SDH 20-May-2009 Model Day
            } else {                                                        //SDH 20-May-2009 Model Day
                BcsmfOpen();                                                //SDH 20-May-2009 Model Day
                bcsmfrec.cBusCentre = (BYTE)wCreator;                       //SDH 20-May-2009 Model Day
                rc = BcsmfRead(__LINE__);                                   //SDH 20-May-2009 Model Day
                BcsmfClose(CL_SESSION);                                     //SDH 20-May-2009 Model Day
                if (rc <= 0) {                                              //SDH 20-May-2009 Model Day
                    afrec.operator_name[5] = bcsmfrec.cBusCentre;           //SDH 20-May-2009 Model Day
                } else {                                                    //SDH 20-May-2009 Model Day
                    //WARNING: This loses the last 4 chars of the BCSMF desc//SDH 20-May-2009 Model Day
                    memcpy(afrec.operator_name + 5, bcsmfrec.abDesc, 10);   //SDH 20-May-2009 Model Day
                }                                                           //SDH 20-May-2009 Model Day
            }                                                               //SDH 20-May-2009 Model Day

        //Else must be a manually-created list so look up the user name     //SDH 20-May-2009 Model Day
        } else {                                                            //SDH 20-May-2009 Model Day
            usrrc = AfOpen();
            if (usrrc < RC_IGNORE_ERR) return usrrc;

            memset( afrec.operator_no, 0x00, 4 );
            pack( afrec.operator_no+2, 2, pllolrec.creator, 3, 1 );
            rc = AfRead(__LINE__);
            AfClose( CL_SESSION );
            if (rc <= 0L) {
                if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                    // User not on EALAUTH
                    strncpy( afrec.operator_name, "* UNKNOWN *    ", 15);
                } else {
                    return RC_DATA_ERR;
                }
            }
        }                                                                   //SDH 20-May-2009 Model Day

        // move pllolrec into LRT_PLL struct
        memcpy( pllp->list_id, pllolrec.list_id, 3 );
        sprintf( sbuf, "%03d", rec-1 );
        memcpy( pllp->seq, sbuf, 3 );
        memcpy( pllp->list_status, pllolrec.list_status, 1 );

        sprintf( sbuf, "%04d%02d%02d",
                 (satoi(pllolrec.create_date, 2)<50 ? 2000 : 1900) +
                 satoi(pllolrec.create_date, 2),
                 satoi(pllolrec.create_date+2, 2),
                 satoi(pllolrec.create_date+4, 2) );
        memcpy( pllp->stamp, sbuf, 8 );
        memcpy( pllp->stamp+8, pllolrec.create_time, 4 );
        memcpy( pllp->lines, pllolrec.item_count, 4 );
        memcpy( pllp->username, afrec.operator_name, 15 );                  // 20 bytes avail.
        pllp->cLocation = pllolrec.cLocation;                               // SDH 20-May-2009 Model Day
        memcpy( pllp->abPickerId, pllolrec.picker, 3 );                     // CSk 12-03-2012 SFA - Locked Picking List Fix
    }

    return found;
}

URC plldb_get_next( /*WORD log_unit,*/ BYTE *list_id, BYTE *seq, LRT_PLI *plip )
{

    LONG rc, tseq;
    URC urc;
    ENQUIRY enqbuf;
    BYTE item_code_unp[13], sbuf[64], txtbuf[256];                          // 24-02-04 PAB
    WORD wHoles = 0;                                                        // 21-12-04 SDH

    tseq = satol( seq, 3 );

    //Keep reading until we get an error or find an unpicked record
    while (TRUE) {

        //Build the key and attempt the read
        memcpy( plldbrec.list_id, list_id, 3 );
        sprintf(sbuf, "%03ld", tseq );
        memcpy(plldbrec.seq, sbuf, sizeof(plldbrec.seq));

        //Attempt read of PLLDB                                             // SDH 22-02-2005 EXCESS
        //If record not found                                               // SDH 22-02-2005 EXCESS
        //Allow 5 'holes' in the file before giving up                      // SDH 22-02-2005 EXCESS
        rc = ReadPlldbLog(__LINE__, LOG_CRITICAL);                          // SDH 29-05-2005 EXCESS
        if (rc<=0L) {
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                wHoles++;                                                   // 21-12-04 SDH
                if (debug) {                                                // 21-12-04 SDH
                    sprintf(msg, "Holes = %d", wHoles);                     // 21-12-04 SDH
                    disp_msg(msg);                                          // 21-12-04 SDH
                }                                                           // 21-12-04 SDH
                if (wHoles > 5) return 0;                                   // 21-12-04 SDH
                tseq++;                                                     // SDH 22-02-2005 EXCESS
                continue;                                                   // SDH 22-02-2005 EXCESS
            }
            return RC_DATA_ERR;
        }

        wHoles = 0;                                                         // 21-12-04 SDH
        if (*plldbrec.item_status == 'U') break;                            // 21-12-04 SDH
        tseq++;                                                             // SDH 22-02-2005 EXCESS
    }                                                                       // 21-12-04 SDH

    //We can only get here after finding a good unpicked record             // 21-12-04 SDH

    // lookup item details
    memset( item_code_unp, 0x20, 13 );
    unpack( item_code_unp+6, 7, plldbrec.boots_code, 4, 1 );
    if (debug) {
        disp_msg("RD PLLDB :");
        dump( (BYTE *)&item_code_unp, 13 );
    }
    urc = stock_enquiry( SENQ_TSF, (BYTE *)item_code_unp, &enqbuf );
    if (debug) {
        sprintf(msg, "stock_enquiry() rc:%d", urc);
        disp_msg(msg);
    }

    //If enquiry failed then return bad rc
    if (urc != RC_OK) return RC_DATA_ERR;

    // Build LRT_PLI response structure
    memcpy( plip->list_id, plldbrec.list_id, 3 );
    memcpy( plip->seq, plldbrec.seq, 3 );
    unpack( plip->boots_code, 7, plldbrec.boots_code, 4, 1);
    memcpy( plip->parent_code, enqbuf.parent_code, 7 );
    memcpy( plip->item_desc, enqbuf.item_desc, 20 );
    memcpy( plip->fill, plldbrec.fill_qty, 4 );
    plip->status[0] = enqbuf.status[0];                                     // SDH 17-11-04 OSSR WAN
    plip->gap_flag[0] = plldbrec.gap_flag[0];                               // SDH 17-11-04 OSSR WAN
    memcpy( plip->pli_qtyshelf, plldbrec.qty_on_shelf, 4);                  // 22-02-04 PAB
    plip->cOssrItem = (enqbuf.cOssrItem == 'Y' ? 'O' : 'N');                // SDH 17-11-04 OSSR WAN
    memcpy(plip->abBackCount, plldbrec.stock_room_count,                    // SDH 17-11-04 OSSR WAN
           sizeof(plip->abBackCount));                                      // SDH 17-11-04 OSSR WAN
    //*(plip->active_deal_flag) =                                           // v4.0
    //   ((satoi( enqbuf.deal_num, 4 )==0) ? 'N' : 'Y');                    // v4.0
    plip->active_deal_flag[0] = enqbuf.active_deal_flag[0];                 // SDH 17-11-04 OSSR WAN
    sprintf( sbuf, "%06d", satoi( enqbuf.stock_figure, 6 ) );
    memcpy( plip->stock_figure, sbuf, 6 );
    if (strncmp( enqbuf.sel_desc, "X ", 2 )!=0 &&
        strncmp( enqbuf.sel_desc, "x ", 2 )!=0) {                           //TAT 06-12-2012 SFA
        memcpy( plip->sel_desc, enqbuf.sel_desc, 45 );
    } else {
        memset( txtbuf, 0x20, 45 );
        format_text( enqbuf.item_desc, 24, txtbuf, 45, 15 );
        translate_text( txtbuf, 45 );                                       // v4.0
        memcpy( plip->sel_desc, txtbuf, 45 );
    }
    memcpy( plip->item_code, enqbuf.item_code, 13 );

    //Read planner database (assume live planners) and set Multisite         // 13-08-2008 16 BMG
    //flag to Y if item in more than one location                            // 13-08-2008 16 BMG
    plip->MultiSited[0] = ' ';                                               // 13-08-2008 16 BMG
    //Need 3 byte packed item code for sriteml read                          // 13-08-2008 16 BMG
    pack(sritmlrec.abItemCode, 3, plip->boots_code, 6, 0);                   // 13-08-2008 16 BMG
    sritmlrec.ubRecChain = 0;                                                // 13-08-2008 16 BMG
    rc = SritmlRead(__LINE__);                                               // 13-08-2008 16 BMG
    if (rc > 0) {                                                            // 13-08-2008 16 BMG
        if ( (sritmlrec.uwCoreItemCount + sritmlrec.uwNonCoreItemCount) > 1) // 13-08-2008 16 BMG
        {                                                                    // 13-08-2008 16 BMG
            plip->MultiSited[0] = 'Y';                                       // 13-08-2008 16 BMG
        }                                                                    // 13-08-2008 16 BMG
    }                                                                        // 13-08-2008 16 BMG

    //Return found                                                          // 21-12-04 SDH
    return 1;                                                               // 21-12-04 SDH

}

URC clolf_get_next( /*WORD log_unit,*/ BYTE *list_id, LRT_CLL *cllp )
{
    UBYTE repeat;
    LONG rc, rec;
    URC found;
    //CLOLF_REC clolfrec;
    BYTE sbuf[64];

    rec = satol( list_id, 3 ) - 1;
    do {
        repeat = FALSE;
        //os = rec * CLOLF_RECL;
        disp_msg("RD CLOLF");
        rc = ClolfRead(rec, __LINE__);
        if (rc <= 0L) {
            clolf.present=FALSE;
            if ((rc&0xFFFF)==0x4003) {
                found = 0;
            } else {
                return RC_DATA_ERR;
            }
        } else {
            // Record found
            clolf.present=TRUE;

            // Ignore lists with following status:                             //CSk 12-03-2012 SFA
            //    [C] - Completed                                              //CSk 12-03-2012 SFA
            //    [ ] - In creation                                            //TAT 24-10-2012 SFA
            if (clolfrec.bListStatus == 'C' ||                                 //TAT 24-10-2012 SFA
                clolfrec.bListStatus == ' ') {                                 //TAT 24-10-2012 SFA
                found = 0;
                repeat = TRUE;
            } else {
                // even if there are zero left to count sent it as the use may have found
                // some and want to submit it.      2/7/4 PAB
                // v4.0 START
                // if ( satol((BYTE*)&(clolfrec.items_shopfloor), 3)==0L &&
                //     satol((BYTE*)&(clolfrec.items_backshop),  3)==0L ) {    // v4.0
                //    found = 0;
                //} else {
                found = 1;                                                  // made conditional in         v4.0
                //}
                // v4.0 END
            }
        }
        rec++;
    } while (repeat);

    if (found==1) {
        // move clolfrec into LRT_CLL struct
        memcpy( cllp->list_id, clolfrec.abListId, 3 );                         //CSk 12-03-2012 SFA
        sprintf( sbuf, "%03d", rec );
        memcpy( cllp->seq, sbuf, 3 );
        sprintf( cllp->num_items, "%03d", clolfrec.uiTotalItems );             //CSk 12-03-2012 SFA
        sprintf( cllp->items_shopfloor, "%03d", clolfrec.uiOutSalesFloorCnt ); //CSk 12-03-2012 SFA
        sprintf( cllp->items_backshop, "%03d", clolfrec.uiOutBackShopCnt );    //CSk 12-03-2012 SFA
        memcpy( cllp->list_type, &clolfrec.bListType, 1 );                     //CSk 12-03-2012 SFA
        memcpy( cllp->last_count_date, "        ", 8 );                        //Tat 04-07-2012 SFA
        memcpy( cllp->bus_unit_name, clolfrec.abListName, 30 );                //CSk 12-03-2012 SFA
        memcpy( cllp->active, &clolfrec.bListStatus, 1 );                      //CSk 12-03-2012 SFA
        sprintf( cllp->abItemsOssr, "%03d", clolfrec.uiOutOSSRCnt );           //CSk 12-03-2012 SFA
        memcpy( cllp->counter_id, clolfrec.abPickerId, 3 );                    //TAT 15-02-2013 SFA
    }

    return found;
}



//------------------------------------------------------------------------------
//                       GetFuturePendingSalesplanFlag                            //CSk 12-03-2012 SFA
//
// If the item is on a Pending Salesplan planner that is active in the next N
// days (defined in the RFSCF), then set the Pending Sales Flag to TRUE. This is
// achieved by checking if the item is on the Pending Planers database (SRITEMP),
// if it is then we need to reference the SRMAP using the POGDB keys in the
// SRITEMP record. However, we only need to check the SRMAP where the SRITEMP
// Core Planner Flag is N (non-core).
//
// Nb. In order to speed up this process it was decided to only read the 1st
//     chain of the SRITEMP record because 99% of items do not have a chain.
//     We only need to find ONE instance, so we can exit the loop early.
//     Also note that we need to read ALL chains for each POGDB in the SRMAP file
//     since the start date is not in ascending chain order - again, we can exit
//     early if we find an instance. To clarify, for each POGDB for the item in the
//     SRITEMP, we need to reference all chanins in the SRMAP for each POGDB.
//------------------------------------------------------------------------------
URC GetFuturePendingSalesplanFlag( /*WORD log_unit,*/ BYTE *bItemCode, BYTE *bPspFlag)
{
    LONG rc;

    WORD wMod;
    WORD hour, min;
    LONG sec, day, month, year;
    WORD wChn;
    WORD wLeadTime;
    DOUBLE dPlannerActiveDate;
    DOUBLE dTodayDate;                                                            // TAT 27-11-2012 SFA
    DOUBLE dTodayDatePlusN;

    //Get date, time, current day of week and date time in string format
    sysdate( &day, &month, &year, &hour, &min, &sec );
    dTodayDate = ConvGJ( day, month, year );                                      // TAT 27-11-2012 SFA
    unpack(sbuf, 2, &rfscfrec1and2.bPendPlanLeadTime, 1, 0);
    wLeadTime = satoi(sbuf,2);
    dTodayDatePlusN = dTodayDate + wLeadTime; // add N days from RFSCF            // TAT 27-11-2012 SFA

    bPspFlag[0] = ' ';  // Default to Not on Pending Planner
    pack(sritmprec.abItemCode, 3, bItemCode, 6, 0);  // ignore check digit
    sritmprec.ubRecChain = 0;
    rc = SritmpRead(__LINE__);
    if (rc > 0) {

        //Populate response
        for (wMod = 0; wMod <= SRITM_NUM_MODS; wMod++) {

            //Get current POG key and read
//            if (sritmlrec.aModuleKey[wMod].ulPOGKey == 0) {                     // TAT 10-07-2012 SFA
            if (sritmprec.aModuleKey[wMod].ulPOGKey == 0) {                       // TAT 10-07-2012 SFA
                break; // no point checking any further as rest will be 0!
            }

            // Only lookup non-core planners
//            if (sritmlrec.aModuleKey[wMod].bCoreFlag == 'N') {                  // TAT 10-07-2012 SFA
            if (sritmprec.aModuleKey[wMod].bCoreFlag == 'N') {                    // TAT 10-07-2012 SFA

//                srmaprec.ulPOGDB = sritmlrec.aModuleKey[wMod].ulPOGKey;         // TAT 10-07-2012 SFA
                srmaprec.ulPOGDB = sritmprec.aModuleKey[wMod].ulPOGKey;           // TAT 10-07-2012 SFA

                for (wChn = 0; wChn < 255; wChn++) {

                    srmaprec.ubRecChain = wChn;
                    rc = SrmapRead(__LINE__);
                    if (rc < 0) {
                       break;       // no more chains
                    }

                    unpack( sbuf, 8, srmaprec.FragmentStartDate, 4, 0 );  //YYYYMMDD (packed)
                    day   = satol( sbuf+6, 2 );
                    month = satol( sbuf+4, 2 );
                    year  = satol( sbuf,   4 );
                    dPlannerActiveDate = ConvGJ( day, month, year );

                    if (dTodayDatePlusN >= dPlannerActiveDate &&                  // TAT 27-11-2012 SFA
                        dTodayDate < dPlannerActiveDate) {                        // TAT 27-11-2012 SFA
                        bPspFlag[0] = 'Y';
                        break;  // jump out as found ONE instance
                    }
                } //for (wChn = 0
            } //if (sritmprec
            if (bPspFlag[0] == 'Y') {   // found an instance so break
                break;
            }
        } // for (wMod
    } //if (rc > 0)
    return 1;
}




//-------------------------------------------------------------
//                       clilf_get_next                                     //CSk 12-03-2012 SFA
//
// Rewritten so each call will return upto 8 items instead of 1
//-------------------------------------------------------------
URC clilf_get_next( /*WORD log_unit,*/ BYTE *list_id, BYTE *seq, LRT_CLI *clip )
{
    UBYTE repeat = TRUE;
    UINT uiTseq  = satol( seq, 3 );                     //Temp sequence
    UINT uiCnt = 0;                                     //Temp count
    LONG rc;
    URC found = 0;
    URC urc;
    BYTE item_code_unp[13];
    BYTE sbuf[64];
    ENQUIRY enqbuf;

    memcpy( clilfrec.abListId, list_id, 3 );

    do {  // Attempt to read up to 8 item records

        sprintf( clilfrec.abSeq, "%03d", uiTseq);

        rc = ClilfRead(__LINE__);

        if (rc<=0L) {
            //clilf.present=FALSE;
            repeat = FALSE;
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {

                if (uiCnt == 0) {   //  Trying to read 1st item
                    found = 0;
                } else {
                    found = 1;               // at least 1 read in successfully
                    clip->cMoreToCome = 'N'; // so return what we've got
                    sprintf( sbuf, "%03d", uiCnt);
                    memcpy( clip->abNumItemsInList, sbuf, 3 );
                    memcpy( clip->abNextItemSeq, clilfrec.abSeq, 3);
                }
            } else {
                return RC_DATA_ERR;
            }
        } else {  // Record found

            if (clilfrec.bCountStatus != 'C') {  // Only include Uncounted items     //TAT 02-11-2012 SFA
                //clilf.present=TRUE;
                found = 1;

                if (uiCnt == 8) {               // if we've found a 9th record then we
                    clip->cMoreToCome = 'Y';    // set the More To Come flag & return.
                    sprintf( sbuf, "%03d", uiCnt);
                    memcpy( clip->abNumItemsInList, sbuf, 3 );
                    memcpy( clip->abNextItemSeq, clilfrec.abSeq, 3);
                    repeat = FALSE;
                } else {
                    // lookup item details
                    memset( item_code_unp, 0x20, 13 );
                    unpack( item_code_unp+6, 7, clilfrec.abItemCode, 4, 1 );

                    // Need to do this because data needed by CLI removed from clilfrec
                    urc = stock_enquiry( SENQ_TSF, (BYTE *)item_code_unp, &enqbuf );
                    if (debug) {
                        sprintf(msg, "stock_enquiry() rc:%d", urc);
                        disp_msg(msg);
                    }

                    //If enquiry failed then return bad rc
                    if (urc != RC_OK) return RC_DATA_ERR;

                    //-------------------------------------------------------------
                    // Move clilfrec into LRT_CLI struct - changes to the clilfrec
                    // means that CLI data also needs to be sourced from elsewhere
                    //-------------------------------------------------------------
                    memcpy( clip->list_id, clilfrec.abListId, 3 );
                    sprintf(clip->CLI_items[uiCnt].abItemSeq, "%03d", uiTseq);                 //TAT 25-09-2012 SFA
                    memcpy( clip->CLI_items[uiCnt].abItemcode,   enqbuf.boots_code,  7 );
                    memcpy( clip->CLI_items[uiCnt].abParentCode, enqbuf.parent_code, 7 );
                    memcpy( clip->CLI_items[uiCnt].abBarcode,    enqbuf.item_code,   13);

                    //Copy relevant SEL description into place
                    //If the first two chars of the SEL desc are "X " then
                    //replace it with the IDF description
                    if (strncmp( enqbuf.sel_desc, "X ", 2 )!=0 &&                              //TAT 29-11-2012 SFA
                        strncmp( enqbuf.sel_desc, "x ", 2 )!=0) {                              //TAT 29-11-2012 SFA
                        memcpy( clip->CLI_items[uiCnt].abSelDesc, enqbuf.sel_desc, 45);        //TAT 29-11-2012 SFA
                    } else {                                                                   //TAT 29-11-2012 SFA
                        memset( sbuf, 0x20, 45 );                                              //TAT 29-11-2012 SFA
                        format_text( enqbuf.item_desc, 24, sbuf, 45, 15 );                     //TAT 29-11-2012 SFA
                        memcpy( clip->CLI_items[uiCnt].abSelDesc, sbuf, 45);                   //TAT 29-11-2012 SFA
                    }                                                                          //TAT 29-11-2012 SFA

                    clip->CLI_items[uiCnt].cActiveDealFlag = enqbuf.active_deal_flag[0];

                    memcpy(clip->CLI_items[uiCnt].abProductGroup, enqbuf.cProductGrp,  6 );

                    sprintf(clip->CLI_items[uiCnt].abCountBackshop,    "%04d", clilfrec.wMainBSCount);
                    sprintf(clip->CLI_items[uiCnt].abCountPendingBackshop, "%04d", clilfrec.wPendingBSCount);
                    sprintf(clip->CLI_items[uiCnt].abCountShopfloor,   "%04d", clilfrec.wSalesFloorCount);
                    sprintf(clip->CLI_items[uiCnt].abOSSRCount,        "%04d", clilfrec.wOSSRCount);
                    sprintf(clip->CLI_items[uiCnt].abPendingOSSRCount, "%04d", clilfrec.wPendingOSSRCount);

                    clip->CLI_items[uiCnt].cStatus   = enqbuf.status[0];
                    clip->CLI_items[uiCnt].cOssrItem = (enqbuf.cOssrItem == 'Y' ? 'O' : 'N');  //TAT 11-09-2012 SFA

                    memcpy(sbuf, "00000000", 8); // Initialise CCYYMMDD
                    if (stock.present) {
                        unpack( sbuf+2, 6, stockrec.date_last_count, 3, 0 );
                        if (*sbuf+2 != '0') {
                            memcpy(sbuf, "20", 2);  // Add century
                        }
                    }
                    memcpy( clip->CLI_items[uiCnt].abLastCountDate, sbuf,  8 );

                    // Check if item on a Pending Salesplanner that is active in the next N days
                    GetFuturePendingSalesplanFlag( enqbuf.boots_code,
                                                   &clip->CLI_items[uiCnt].cPendingSaleFlag);


//xxx                    //---------------------------------------------------------------------------
//xxx                    // (defined in the RFSCF), then set the Pending Sales Flag to TRUE. This is
//xxx                    // achieved by checking if the item is on the Pending Planers database (SRITMP),
//xxx                    // if it is then we need to reference the SRPOG using the POGDB keys in the
//xxx                    // SRITMP record. However, we only need to check the SRPOG where the SRITMP
//xxx                    // Core Planner Flag is N (non-core).
//xxx                    //
//xxx                    // Nb. In order to speed up this process it was decided to only read the 1st
//xxx                    //     chain of the SRITMP record because 99% of items do not have a chain.
//xxx                    //     We only need to find ONE instance, so we can exit the loop early.
//xxx                    //---------------------------------------------------------------------------
//xxx                    cclip->CLI_items[uiCnt].cPendingSaleFlag = ' ';  // Default to Not on Pending Planner
//xxx                    pack(sritmprec.abItemCode, 3, enqbuf.boots_code, 6, 0);  // ignore check digit
//xxx                    sritmprec.ubRecChain = 0;
//xxx                    rc = SritmpRead(__LINE__);
//xxx                    if (rc > 0) {
//xxx
//xxx                        //Populate response
//xxx                        for (wMod = 0; wMod <= SRITM_NUM_MODS; wMod++) {
//xxx
//xxx                            //Get current POG key and read
//xxx                            if (sritmlrec.aModuleKey[wMod].ulPOGKey == 0) {
//xxx                                break; // no point checking any further as rest will be 0!
//xxx                            }
//xxx
//xxx                            // Only lookup non-core planners
//xxx                            if (sritmlrec.aModuleKey[wMod].bCoreFlag == 'N') {
//xxx
//xxx                                srpogrec.ulKey = sritmlrec.aModuleKey[wMod].ulPOGKey);
//xxx                                rc2 = SrpogRead(__LINE__);
//xxx                                if (rc2 < 0) {
//xxx                                   break;
//xxx                                }
//xxx
//xxx                                unpack( sbuf, 8, srpogrec.abActiveDate, 4, 0 );  //YYYYMMDD (packed)
//xxx                                day   = satol( sbuf+6, 2 );
//xxx                                month = satol( sbuf+4, 2 );
//xxx                                year  = satol( sbuf,   4 );
//xxx                                dPlannerActiveDate = ConvGJ( day, month, year );
//xxx
//xxx                                if (dTodayDatePlusN >= dPlannerActiveDate) {
//xxx                                    cclip->CLI_items[uiCnt].cPendingSaleFlag = 'Y';
//xxx                                    break;  // jump out as found ONE instance
//xxx                                }
//xxx                            }
//xxx                        } // for (wMod
//xxx                    } //if (rc > 0)
                    memcpy(clip->CLI_items[uiCnt].abStockFigure,  enqbuf.stock_figure, 6 );
                    uiCnt++;                                                           //CSk 07-08-2012 SFA
                }
            } //(*clilfrec.bCountStatus != 'C')                                      //TAT 02-11-2012 SFA
        }

        //uiCnt++;               //MOVED                                               //CSk 07-08-2012 SFA
        uiTseq++;


    } while (repeat);


    return found;
}




URC rfrdesc_get_next( /*WORD log_unit,*/ BYTE *seq, LRT_RLR *rlrp )
{
    UBYTE repeat;
    LONG rc, os, rec;
    URC found;
    RFRDESC_REC rfrdescrec;
    BYTE sbuf[64];

    memset( (BYTE *)&rfrdescrec, 0x00, sizeof(RFRDESC_REC) );

    rec = satol( seq, REPORT_SEQ_SIZE );                                    // changed by PK 10/10/97
    do {
        repeat = FALSE;
        os = rec * RFRDESC_RECL;
        rc = s_read( A_BOFOFF, rfrdesc.fnum, (void *)&rfrdescrec,
                     RFRDESC_RECL, os );
        if (rc<=0L) {
            rfrdesc.present=FALSE;
            if ((rc&0xFFFF)==0x4003) {
                if (debug) {
                    sprintf( msg,
                             "Attempted to read beyond EOF on RFRDESC. O/S=%ld",
                             os );
                    disp_msg(msg);
                }
                found = 0;
            } else {
                log_event101(rc, RFRDESC_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R RFRDESC. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }
        } else {
            // Record found
            rfrdesc.present=TRUE;
            if (*(rfrdescrec.reptype) == 'B') {
                // Report is designed for both controller and handheld
                found = 1;
            } else {
                // Report is designed for controller only, so get next report
                found = 0;
                repeat = TRUE;
            }
        }
        rec++;
    } while (repeat);

    if (found==1) {
        sprintf( sbuf, "%0*ld", REPORT_SEQ_SIZE, rec );                     // NEW
        memcpy( rlrp->seq, sbuf, REPORT_SEQ_SIZE );                         // NEW
        memcpy( rlrp->title, rfrdescrec.title, REPORT_DATASIZE );
        memcpy( rlrp->fname, rfrdescrec.fname, 12 );
    }

    return found;
}

// Get next level 0 record, starting from record 'seq', from report fnum
// held in lrtp table[unit]
// IN  : log_unit, seq
// OUT : rldp = RLD record, rec_cnt = number of sub records repeated in RLD
URC rfrep_get_next_lev0( WORD log_unit, BYTE *seq,
                         LRT_RLD *rldp, WORD *rec_cnt )
{
    UBYTE repeat;
    //WORD rec_sb;
    LONG rec_sb;
    LONG rc, os, rec;
    URC found;
    RFREP_REC rfreprec;
    BYTE sbuf[64];

    // Return if no report buffer available
    if (lrtp[log_unit]->rbufp == NULL) {
        return RC_SERIOUS_ERR;
    }

    rec_sb = 0;
    rec = (LONG)satol( seq, REPORT_SEQ_SIZE );
    do {
        repeat = FALSE;
        os = rec * RFREP_RECL;
        // if no data read or record required is outside buffered range
        if (lrtp[log_unit]->rbufp->base == -1L ||
            ( lrtp[log_unit]->rbufp->base != -1L &&
              ( os<lrtp[log_unit]->rbufp->base ||
                os>(lrtp[log_unit]->rbufp->base +
                    lrtp[log_unit]->rbufp->end) ) )) {
            // PHYSICAL READ
            rc = s_read( A_BOFOFF, lrtp[log_unit]->fnum3,
                         lrtp[log_unit]->rbufp->buff,
                         REPORT_BUFFER, os );
            if (debug) {
                sprintf(msg, "RD RFREP (PHYSICAL) - %08lX", rc);
                disp_msg(msg);
            }
            if (rc>0) {
                lrtp[log_unit]->rbufp->base = os;
                lrtp[log_unit]->rbufp->end = ((rc/RFREP_RECL)*RFREP_RECL)-1;
            } else {
                lrtp[log_unit]->rbufp->base = -1L;
                lrtp[log_unit]->rbufp->end = -1L;
            }
        } else {
            // LOGICAL READ
            rc = REPORT_BUFFER;
            //if (debug) {
            //   sprintf(msg, "RD RFREP (LOGICAL) - %08lX", rc);
            //   disp_msg(msg);
            //}
        }
        if (rc<=0L) {
            if ((rc&0xFFFF)==0x4003) {
                if (debug) {
                    sprintf( msg,
                             "Attempted to read beyond EOF on RFREP. O/S=%ld",
                             os );
                    disp_msg(msg);
                }
                if (rec_sb>0) {
                    // Data outstanding
                    found = 1;
                } else {
                    // No data outstanding
                    found = 0;
                }
            } else {
                log_event101(rc, RFREP_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R RFREF. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }
        } else {
            // Process available data
            memcpy( (BYTE *)&rfreprec,
                    lrtp[log_unit]->rbufp->buff + os -
                    lrtp[log_unit]->rbufp->base,
                    RFREP_RECL );
            if (*rfreprec.level=='0') {
                // Save this record
                sprintf( sbuf, "%0*ld", REPORT_SEQ_SIZE, rec );
                memcpy( rldp->rep[rec_sb].seq, sbuf, REPORT_SEQ_SIZE );
                memcpy( rldp->rep[rec_sb].data,
                        rfreprec.data, REPORT_DATASIZE );
                if ((++rec_sb)>=HEADER_MAX) {
                    // Filled RLD block of HEADER_MAX records
                    found = 1;
                } else {
                    // Space still available in RLD block of HEADER_MAX records
                    found = 0;
                    repeat = TRUE;
                }
            } else {
                found = 0;
                repeat = TRUE;
            }
        }
        rec++;
    } while (repeat);
    *rec_cnt = rec_sb;

    return found;
}

// Get next non-level 0 record, starting from record 'seq', from report fnum
// held in lrtp table[unit]
// IN  : log_unit, seq
// OUT : rupp = RUP record, rec_cnt = number of sub records repeated in RUP
URC rfrep_get_next( WORD log_unit, BYTE *seq,
                    LRT_RUP *rupp, WORD *rec_cnt )
{
    UBYTE repeat;
    WORD rec_sb;
    LONG rc, os, rec;
    URC found;
    RFREP_REC rfreprec;

    // Return if no report buffer available
    if (lrtp[log_unit]->rbufp == NULL) {
        return RC_SERIOUS_ERR;
    }

    rec_sb = 0;
    rec = (LONG)satol( seq, REPORT_SEQ_SIZE );
    do {
        repeat = FALSE;
        os = rec * RFREP_RECL;
        // if no data read or record required is outside buffered range
        if (lrtp[log_unit]->rbufp->base == -1L ||
            ( lrtp[log_unit]->rbufp->base != -1L &&
              ( os<lrtp[log_unit]->rbufp->base ||
                os>(lrtp[log_unit]->rbufp->base +
                    lrtp[log_unit]->rbufp->end) ) )) {

            // PHYSICAL READ
            rc = s_read( A_BOFOFF, lrtp[log_unit]->fnum3,
                         lrtp[log_unit]->rbufp->buff,
                         REPORT_BUFFER, os );
            if (rc>0) {
                lrtp[log_unit]->rbufp->base = os;
                lrtp[log_unit]->rbufp->end = ((rc/RFREP_RECL)*RFREP_RECL)-1;
            } else {
                lrtp[log_unit]->rbufp->base = -1L;
                lrtp[log_unit]->rbufp->end = -1L;
            }
        } else {
            // LOGICAL READ
            rc = REPORT_BUFFER;
        }
        if (rc<=0L) {
            if ((rc&0xFFFF)==0x4003) {
                if (debug) {
                    sprintf( msg,
                             "Attempted to read beyond EOF on RFREP. O/S=%ld",
                             os );
                    disp_msg(msg);
                }
                if (rec_sb>0) {
                    // Data outstanding
                    found = 1;
                } else {
                    // No data outstanding
                    found = 0;
                }
            } else {
                log_event101(rc, RFREP_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R RFREF. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }
        } else {
            // Process available data
            memcpy( (BYTE *)&rfreprec,
                    lrtp[log_unit]->rbufp->buff + os -
                    lrtp[log_unit]->rbufp->base,
                    RFREP_RECL );
            if (*rfreprec.level!='0') {
                // Save this record
                memcpy( rupp->rep[rec_sb].level, rfreprec.level, 1 );
                memcpy( rupp->rep[rec_sb].exp, rfreprec.exp, 1 );
                memcpy( rupp->rep[rec_sb].data,
                        rfreprec.data, REPORT_DATASIZE );
                if ((++rec_sb)>=DETAIL_MAX) {
                    // Filled RUP block of HEADER_MAX records
                    found = 1;
                } else {
                    // Space still available in RUP block of DETAIL_MAX records
                    found = 0;
                    repeat = TRUE;
                }
            } else {
                if (rec_sb>0) {
                    // Data outstanding
                    found = 1;
                } else {
                    // No data outstanding
                    found = 0;
                }
            }
        }
        rec++;
    } while (repeat);
    *rec_cnt = rec_sb;

    return found;
}

////////////////////////////////////////////////////////////////////////////
//
// Process orphan workfiles - ones transact is not aware of
//
// Note: Since this destroys and rebuilds the orphan queue, the order
// of files to process is not conserved
//
////////////////////////////////////////////////////////////////////////////
#define ORPHAN_DIR "H1:\\ADX_UDT1\\"

void process_orphans(BOOLEAN fAction) {                                     //SDH 9-May-06

    DISKFILE df[10];
    LONG key, rc;
    WORD i, j;
    BYTE cPrefix;
    BYTE sbuf[32];                                                          // 11-09-2007 13 BMG
                                                                            //SDH 9-May-06
    disp_msg("Process Orphans");                                            // 11-09-2007 13 BMG

    //Destroy current pq queue                                              //SDH 20-12-2004
    //Set rec count to 0 to build from scratch                              //SDH 20-12-2004
    if (fAction) {                                                          //SDH 9-May-06
        disp_msg("Destroy PQ table");                                       // Null fix SDH 29-Sep-2009
        for (i = 0; i < MAX_CONC_UNITS; i++) {                              //SDH 20-12-2004
            pq[i].state = PST_FREE;                                         //SDH 20-12-2004
            pq[i].submitcnt = 0;                                            // 11-09-2007 13 BMG
            pq[i].type = 0;                                                 // 11-09-2007 13 BMG
            pq[i].unit = 0;                                                 // 11-09-2007 13 BMG
            memset(pq[i].DevType, 0x20, 1);                                 // BMG 16-09-2008 18 MC70
            memset(pq[i].fname, 0x00, sizeof(pq[i].fname));                 // Null fix SDH 29-Sep-2009
            pq[i].last_access_time = 0;                                     // 11-09-2007 13 BMG
        }                                                                   //SDH 20-12-2004
    }                                                                       //SDH 9-May-06

    //Change default directory to D:\ADX_UDT1
    s_define(0, "default:", ORPHAN_DIR, sizeof(ORPHAN_DIR) - 1);

    //Initialise key for s_lookup                                           //SDH 20-12-2004
    key = 0L;                                                               //SDH 20-12-2004

    do {

        //Get next 10 files that match 'D:/ADX_UDT1/W*.*'
        //skip if bad return code
        rc = s_lookup( T_FILE, A_HIDDEN | A_SYSFILE,                        //SDH 20-12-2004
                       "W*.*", (BYTE *)df, sizeof(df), sizeof(DISKFILE), key );//SDH 20-12-2004
        if (rc <= 0L) break;                                                //SDH 20-12-2004
        key = df[rc - 1].df_key;                                            //SDH 20-12-2004

        //For each file returned by s_lookup
        for (i = 0; i < rc; i++) {

            //Send file details to trace
            if (debug) {                                                            //SDH 22-June-2006 // 11-09-2007 13 BMG
                sprintf( msg, "    *%-32s* %ld byte(s)",
                         df[i].df_name, df[i].df_size );
                disp_msg(msg);
            }

            //Get file type as 2nd char of filename (K = SEL print, G = gap)        //SDH 20-12-2004
            //Ignore if neither                                                     //SDH 20-12-2004
            cPrefix = df[i].df_name[1];                                             //SDH 20-12-2004
            if (cPrefix != 'K' && cPrefix != 'G' &&                                                         // BMG 01-09-2008 17 ASN/Directs
                cPrefix != 'C' && cPrefix != 'U' && cPrefix != 'D') continue;       //SDH 20-12-2004        // BMG 01-09-2008 17 ASN/Directs

            //Have to test against logical LFN as this is what is stored in pq      // 11-09-2007 13 BMG
            //if (cPrefix == 'K') {                                                   // 11-09-2007 13 BMG  // BMG 01-09-2008 17 ASN/Directs
            switch (cPrefix) {                                                                              // BMG 01-09-2008 17 ASN/Directs

            case 'K':                                                                                       // BMG 01-09-2008 17 ASN/Directs
                sprintf( sbuf, "CHKWK:");                                           // 11-09-2007 13 BMG
                memcpy(sbuf + 6, df[i].df_name + 2, sizeof(df[i].df_name) - 2);         // 11-09-2007 13 BMG
                break;                                                                                      // BMG 01-09-2008 17 ASN/Directs

            case 'G':                                                                                       // BMG 01-09-2008 17 ASN/Directs
                sprintf( sbuf, "GAPWK:");                                           // 11-09-2007 13 BMG
                memcpy(sbuf + 6, df[i].df_name + 2, sizeof(df[i].df_name) - 2);                             // BMG 01-09-2008 17 ASN/Directs
                break;                                                                                      // BMG 01-09-2008 17 ASN/Directs

            case 'C':                                                                                       // BMG 01-09-2008 17 ASN/Directs
            case 'U':                                                                                       // BMG 01-09-2008 17 ASN/Directs
            case 'D':                                                                                       // BMG 01-09-2008 17 ASN/Directs
            default:                                                                                        // Null fix SDH 29-Sep-2009
                memcpy(sbuf, df[i].df_name, sizeof(df[i].df_name));                                         // BMG 01-09-2008 17 ASN/Directs
                break;                                                                                      // BMG 01-09-2008 17 ASN/Directs
            }                                                                       // 11-09-2007 13 BMG

            if (!fAction) {                                                         //SDH 9-May-06

                //If the file is flagged as free in the PQ table then something     //SDH 9-May-06
                //may have gone wrong.  Equally, PSS47 may currently be processing  //SDH 9-May-06
                //it.  Either way, adopt it, as if the file is finished by PSS47    //SDH 9-May-06
                //then it will be deleted, and then PSS47 will fall over next time  //SDH 9-May-06
                //and re-adoption will not take place.                              //SDH 9-May-06
                BOOLEAN fActiveInPQ = FALSE;                                        //SDH 9-May-06

                //Always leave element 0 free, to protect against units using       // Null fix SDH 29-Sep-2009
                //getting confused between their default value of 0, and a real 0   // Null fix SDH 29-Sep-2009
                for (j = 1; j < MAX_CONC_UNITS; j++) {                              // Null fix SDH 29-Sep-2009
                    if (strncmp(pq[j].fname, sbuf, sizeof(pq[j].fname)) == 0) {     // Null fix SDH 29-Sep-2009
                        if (pq[j].state != PST_FREE) fActiveInPQ = TRUE;            // Null fix SDH 29-Sep-2009
                        break;                                                      // Null fix SDH 29-Sep-2009
                    }                                                               // Null fix SDH 29-Sep-2009
                }                                                                   //SDH 9-May-06
                if (fActiveInPQ) continue;                                          //SDH 9-May-06

            }                                                                       //SDH 9-May-06

            //If the size of the file is less than or equal to 20 bytes             //SDH 20-12-2004
            //then it's just a header so delete it                                  //SDH 20-12-2004
            //NOTE that we don't use rc here, as it controls the loop               //SDH 20-12-2004
            //and we don't want to corrupt it                                       //SDH 20-12-2004
            //Also if it is a Directs buffer file, simply delete it                                   // BMG 01-09-2008 17 ASN/Directs
            if (df[i].df_size <= 20L || cPrefix == 'D') {                           //SDH 20-12-2004  // BMG 01-09-2008 17 ASN/Directs
                if (s_delete(0x2000, (void*)df[i].df_name) < 0 ) {                  //SDH 20-12-2004
                    restrict_file((void*)df[i].df_name);                            //SDH 20-12-2004
                    s_delete(0x2000, (void*)df[i].df_name);                         //SDH 20-12-2004
                }                                                                   //SDH 20-12-2004
                continue;                                                           //SDH 20-12-2004
            }                                                                       //SDH 20-12-2004

            //Let's locate a free entry in the table for this orphan                //SDH 9-May-2006
            //Always leave element 0 free, to protect against units using           // Null fix SDH 29-Sep-2009
            //getting confused between their default value of 0, and a real 0       // Null fix SDH 29-Sep-2009
            BOOLEAN fFoundFreeSlot = FALSE;                                         //SDH 9-May-2006
            for (j = 1; j < MAX_CONC_UNITS; j++) {                                  // Null fix SDH 29-Sep-2009

                //If this slot is free then adopt it                                //SDH 9-May-2006
                if (pq[j].state == PST_FREE) {                                      //SDH 9-May-2006

                    //Flag it                                                       //SDH 9-May-2006
                    fFoundFreeSlot = TRUE;                                          //SDH 9-May-2006

                    sprintf(msg, "Adopting %s as %s, slot %d",                      // Null fix SDH 29-Sep-2009
                            df[i].df_name, sbuf, j);                                // Null fix SDH 29-Sep-2009
                    disp_msg(msg);                                                  //SDH 9-May-2006

                    //Record file and increment for next                            //SDH 9-May-2006
                    pq[j].state = PST_ADOPTED;                                      //SDH 9-May-2006
                    pq[i].submitcnt = 0;                                            //PAB 26-1-2007
                    pq[j].unit = 999;                                               //SDH 9-May-2006
                    strncpy(pq[j].fname, sbuf, sizeof(pq[j].fname));                // Null fix SDH 29-Sep-2009
                    pq[j].fname[sizeof(pq[j].fname) - 1] = 0x00;                    // Null fix SDH 29-Sep-2009
                    cPrefix = df[i].df_name[1];                                     // 11-09-2007 13 BMG
                    //pq[j].type = (cPrefix == 'K' ? SYS_LAB : SYS_GAP);              //SDH 9-May-2006 // BMG 01-09-2008 17 ASN/Directs
                    switch (cPrefix) {                                              // BMG 01-09-2008 17 ASN/Directs
                    case 'K':                                                       // BMG 01-09-2008 17 ASN/Directs
                        pq[j].type = SYS_LAB;                                       // BMG 01-09-2008 17 ASN/Directs
                        break;                                                      // BMG 01-09-2008 17 ASN/Directs
                    case 'G':                                                       // BMG 01-09-2008 17 ASN/Directs
                        pq[j].type = SYS_GAP;                                       // BMG 01-09-2008 17 ASN/Directs
                        break;                                                      // BMG 01-09-2008 17 ASN/Directs
                    case 'C':                                                       // BMG 01-09-2008 17 ASN/Directs
                        pq[j].type = SYS_CB;                                        // BMG 01-09-2008 17 ASN/Directs
                        break;                                                      // BMG 01-09-2008 17 ASN/Directs
                    case 'U':                                                       // BMG 01-09-2008 17 ASN/Directs
                        pq[j].type = SYS_PUB;                                       // BMG 01-09-2008 17 ASN/Directs
                        break;                                                      // BMG 01-09-2008 17 ASN/Directs
                    // No case for 'D' as file is deleted above                     // BMG 01-09-2008 17 ASN/Directs
                    }                                                               // BMG 01-09-2008 17 ASN/Directs

                    pq[j].last_access_time = 0;                                                      // 11-09-2007 13 BMG
                    if ( satol(rfscfrec1and2.phase,1)==5 ) {                                         // BMG 16-09-2008 18 MC70
                        memset(pq[j].DevType, 0x4D, 1); //Set it to "M"                              // BMG 16-09-2008 18 MC70
                    }                                                                                // BMG 16-09-2008 18 MC70
                    break;                                                          //SDH 9-May-2006

                }

            }

            if (!fFoundFreeSlot) {
                if (debug) disp_msg("WARNING - No free pq entries");        //SDH 20-12-2004
                background_msg("ERROR: SELs/gap report stalled.");          //SDH 20-12-2004
                return;
            }

        }                                                                   //SDH 20-12-2004

    } while (rc > 0L);

    disp_msg("Orphan adoption Complete");                        //SDH 22-June-2006 // 11-09-2007 13 BMG

}


void CloseAllFiles ( void )
{

    disp_msg("Close all files");

    close_pllol( CL_ALL );
    close_plldb( CL_ALL );
    IsfClose( CL_ALL );
    IdfClose( CL_ALL );
    IrfClose( CL_ALL );
    IrfdexClose( CL_ALL );                                                 //SDH 14-01-2005 Promotions
    close_imstc( CL_ALL );
    close_stock( CL_ALL );
    close_cimf( CL_ALL );
    close_citem( CL_ALL );
    AfClose( CL_ALL );
    close_stkmq( CL_ALL );
    close_imfp( CL_ALL );
    close_rfrdesc( CL_ALL );
    close_tsf( CL_ALL );
    close_psbt( CL_ALL );
    close_wrf( CL_ALL );
    close_minls( CL_ALL );
    RfhistClose( CL_ALL );
    InvokClose( CL_ALL );
    close_clolf( CL_ALL );
    close_clilf( CL_ALL );
    close_pilst( CL_ALL );
    PgfClose( CL_ALL );
    close_suspt( CL_ALL );                                                  // PAB 5-5-4
    PrtctlClose( CL_ALL );                                                  // Streamline SDH 17-Sep-2008
    PrtlistClose( CL_ALL );                                                 // Streamline SDH 17-Sep-2008
    BcsmfClose( CL_ALL );                                                  // SDH 26-11-04 OSSR WAN
    CclolClose( CL_ALL );                                                  // SDH 26-11-04 OSSR WAN
    CcilfClose( CL_ALL );                                                  // SDH 26-11-04 OSSR WAN
    CchistClose( CL_ALL );                                                 // SDH 26-11-04 OSSR WAN
    CcdirsuClose( CL_ALL );                                                // SDH 26-11-04 OSSR WAN
    close_deal();                                                           // SDH 10-12-04 PROMOTIONS
    PogokClose(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    SrpogClose(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    SrmodClose(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    SrmapClose(CL_ALL);                                                    //CSk 12-03-2012 SFA
    SritmlClose(CL_ALL);                                                   // SDH 12-Oct-2006 Planners
    SritmpClose(CL_ALL);                                                   // SDH 12-Oct-2006 Planners
    SrpogifClose(CL_ALL);                                                  // SDH 12-Oct-2006 Planners
    SrpogilClose(CL_ALL);                                                  // SDH 12-Oct-2006 Planners
    SrpogipClose(CL_ALL);                                                  // SDH 12-Oct-2006 Planners
    SrcatClose(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    SrsxfClose(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    close_recall( CL_ALL );                                                 // 11-09-2007 13 BMG
    close_rcindx ( CL_ALL );                                                // 11-09-2007 13 BMG
    close_rcspi ( CL_ALL );                                                 // 11-09-2007 13 BMG
    close_carton (CL_ALL );                                                 // BMG 01-09-2008 17 ASN/Directs
    close_delvsmry (CL_ALL );                                               // BMG 01-09-2008 17 ASN/Directs
    close_delvindx (CL_ALL );                                               // BMG 01-09-2008 17 ASN/Directs
    close_delvlist (CL_ALL );                                               // BMG 01-09-2008 17 ASN/Directs
    close_diror (CL_ALL );                                                  // BMG 01-09-2008 17 ASN/Directs
    close_dirsu (CL_ALL );                                                  // BMG 01-09-2008 17 ASN/Directs
    close_uodot (CL_ALL );                                                  // BMG 01-09-2008 17 ASN/Directs
    close_uodin (CL_ALL );                                                  // BMG 01-09-2008 17 ASN/Directs
    close_pdtasset (CL_ALL);                                                // BMG 16-09-2008 18 MC70
}

URC open_pdtasset(void) {
    return keyed_open(&pdtasset, TRUE);
}
URC close_pdtasset(WORD type) {
    return close_file(type, &pdtasset);
}
LONG WritePDTAsset(LONG lLineNumber) {
    return WriteKeyed(&pdtasset, lLineNumber, LOG_ALL);
}



