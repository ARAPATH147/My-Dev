// -------------------------------------------------------------------------
//                           Boots The Chemists Ltd
//                      Radio Frequency Server Application
//                             Function Module
// ------------------------------------------------------------------------
// Version 1.0    Steve Wright / Paul Bowers                     1994,95,96,97
//
// 1.0 - Original live install version.
// 1.1 - Move file opens to where they are required, rather than leaving all
//       the files open all the time.
// 1.2 - Trap NO_NCADVR error on ncaopen - retry.
//     - Split Stock Enquiry logging to LRTLG to show whether item was on
//       file or not.
//     - Ensure IMSTC is closed when abnormal code path encountered.
//     - Trap null keys.
//     - Increase resiliancy to 'not on file's.
// ------------------------------------------------------------------------
// 2.0 - Major changes for stage 2 of the trial
// 2.1 - Fix Sales Enquiry function        Paul Bowers   28th August 1998
// 2.2 - Store Close file closure fix      Paul Bowers   24th Sept   1998
//       To close all files regardless of
//       the number of logical opens sessions. - to handle IPLed HHT's
//       which will never get signed off normally.
// 2.3 - Incorrectly formatted BTS codes   Steve Wright  28th October 1998
//       Correct Gap Monitor (GAP txn), which currently formats
//       boots codes written to the GAPBF when a bar code has been
//       scanned for a fill-up. The problem does not occur when a
//       SEL is scanned. Symptoms are 'NOT ON FILE's on Gap Report.
// 2.4 - Phase II enhancements             Steve Wright  30th October 1998
//       Add timestamp function to return formatted timestamp
//       Add extra fields to PLI record
//       Add file support for following files; MINLS, RFHIST, INVOK,
//       CLOLF, CLILF
//       Add shared memory buffer to allow other processes to monitor
//       server activity
// 3.0 - Envoy
// 3.1 - CLI problem fix
// 4.0 - 2003 Trial
//       Update to cope with new deals system
// 5.0 - RF Trial Phase2 2004 - introduce proxmity printing support
//       add PRTCTL file , update SNR and PRT transactions
//                                             Paul Bowers  February 2004
// 6.0 - Implement open file session stack, dynamiclly add / remove session
//       numbers as they are allocated and closed for Report Files and work
//       files, so in the CLS /CYC routing any disassociated sessions can be
//       closed.                               Paul Bowers   6th April 2004
// 7.0 - Implement changes to support PRE-SCAN functionality - EALSUSPT file
//                                             Paul Bowers 5th May 2004
// 8.0 - Impement changes for MyStoreNet and Various fixes for PPC reconnect
//       and various other bug fixes.          Paul Bowers 28th August 2004
// 9.0 - Offsite stock room support            Paul Bowers 31st August 2004
// 10 - introduce the JOBOK processing         Paul Bowers 12th December 2004
// 11 - Fix GAP transaction processing to determine if GAP is new or update
//      before incrementing count on PLLOL
//      move semaphore function to TRANS02 as this module reached 64K limit.
// 12 - Brian Greenfield (BMG) 10th September 2007
//      Set pFile->fnum to zero once a file is closed so that it is not
//      re-opened after a PPC reboot.
// 13 - Brian Greenfield (BMG) 11th September 2007
//      Set new pq.last_access_time value.
//      Had to change variable name time to ltime in prepare_workfile
//      Added Recalls files to closeallfiles.
//      Added call to dump_pq_stack as code blew 64K limit. Moved code
//      to trans03.c.
//      Added call to CycleLogs as code blew 64K limit. Moved code to
//      trans03.c.
//      Changed process_orphans so that it can safely re-adopt during the
//      day without re-adopting existing jobs.
//      Increased sales figure storage to doubles in store_sales_enquiry.
//      Minor fix to process_gap PLLOL read.
//
// 14 - Brian Greenfield (BMG) 6th Feb 2008
//      Added code to padd back the markdwon dtatus from the IDF.
//
// 15 - Brian Greenfield (BMG) 30th july 2008
//      Added test for Planner Leaver Recalls to set flag in EQR on.
//
// 16 - Brian Greenfield (BMG) 13th August 2008
//      Added test for Multi-Sited item into plldb_get_next function.
//      Moved check_command and spool_workfile from here into trans03 due to space.
//
// 17 - Brian Greenfield (BMG) 1st Sept 2008
//      Added bits for ASN/Directs. (Now not in this module due to Streamlining changes!)
//
// 18 - Brian Greenfield (BMG) 16th Sept 2008
//      Added file close for PDTASSET.
//      Removed static from WriteKeyed.
//
// 19 - Stuart Highley.  22-Sep-2008.
//      Moved all file related functions into TRXFILE.C. Uncommented.
//      Renamed module from RFS.C to TRXUTIL.C
//
// 20 - Brian Greenfield (BMG) 17th Dec 2009 RF Stabilisation
//      Changes for RF Stabilisation
//
// 21 - Charles Skadorwa (CSk) 11th May 2010 Auto Fast Fill
//      Added Product Group to EQR transaction.
//      Added Recall Type   to EQR transaction.
//
// 22 - Charles Skadorwa (CSk) 12th Mar 2012 Stock File Accuracy
//      Modified authorise_user() to set new stock adjustment flag.
//
// ------------------------------------------------------------------------
#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <flexif.h>
#include <math.h>                                                       // SDH 17-11-04 OSSR WAN

// Boots specific files
//#include "debugdef.h"  // v4.01
#include "osfunc.h"       /* needed for disp_msg() */
#include "rfsfile.h"
#include "trxutil.h"
//#include "wrap.h"
//#include "c4680if.h"                                                    // SDH 17-11-04 OSSR WAN
//#include "util.h"                                                       // SDH 17-11-04 OSSR WAN
//#include "dateconv.h"                                                   // SDH 17-11-04 OSSR WAN
#include "sockserv.h"                                                   // SDH 04-05-05 Chain App
#include "rfglobal.h"                                                    // SDH 22-05-06 VSlick
//#include "trans5.h"
//#include "prtctl.h"
//#include "prtlist.h"
#include "irf.h"
#include "idf.h"
#include "srfiles.h"
//#include "rfhist.h"
#include "isf.h"
//#include "nvurl.h"
#include "rfscf.h"
#include "rfhist.h"
#include "pgf.h"
//#include "invok.h"
//#include "bcsmf.h"
#include "af.h"
//#include "clfiles.h"
//#include "trxfile.h"

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

//static BYTE *look_xxx = { "RFS     "};                                      // SDH 17-11-04 OSSR WAN

URC process_workfile(WORD log_unit, BYTE type) {

    PROG_PQ *pPQ = NULL;                                                    //SDH 22-June-2006
    LONG    *plFileNum = NULL;                                              //SDH 22-June-2006
    LONG    lMinSize, fsz;                                                  //SDH 22-June-2006
    WORD    *pwPQRec = NULL;                                                //SDH 22-June-2006
    BYTE fnm[32];

    //Check that unit entry exists                                          //SDH 22-June-2006
    if (lrtp[log_unit] == NULL) {                                           //SDH 22-June-2006
        return RC_OK;                                                       //SDH 22-June-2006
    }                                                                       //SDH 22-June-2006

    //Setup pointers                                                        //SDH 22-June-2006
    switch (type) {                                                         //SDH 22-June-2006
    case SYS_LAB:                                                           //SDH 22-June-2006
        disp_msg("Process workfile type : SYS_LAB");                        //SDH 22-June-2006
        pwPQRec = &lrtp[log_unit]->pq_sub1;                                 //SDH 22-June-2006
        pPQ = &pq[*pwPQRec];                                                //SDH 22-June-2006
        plFileNum = &lrtp[log_unit]->fnum1;                                 //SDH 22-June-2006
        lMinSize = 1;                                                       //SDH 22-June-2006
        break;                                                              //SDH 22-June-2006
    case SYS_GAP:                                                           //SDH 22-June-2006
        disp_msg("Process workfile type : SYS_GAP");                        //SDH 22-June-2006
        pwPQRec = &lrtp[log_unit]->pq_sub2;                                 //SDH 22-June-2006
        pPQ = &pq[*pwPQRec];                                                //SDH 22-June-2006
        plFileNum = &lrtp[log_unit]->fnum2;                                 //SDH 22-June-2006
        lMinSize = 32;                                                      //SDH 22-June-2006
        break;                                                              //SDH 22-June-2006
    case SYS_CB:                                                            //BMG 17-10-2008 ASN/Directs
        disp_msg("Process workfile type : SYS_CB");                         //BMG 17-10-2008 ASN/Directs
        pwPQRec = &lrtp[log_unit]->pq_CB;                                   //BMG 17-10-2008 ASN/Directs
        pPQ = &pq[*pwPQRec];                                                //BMG 17-10-2008 ASN/Directs
        plFileNum = &lrtp[log_unit]->lCBfnum;                               //BMG 17-10-2008 ASN/Directs
        lMinSize = 1;                                                       //BMG 17-10-2008 ASN/Directs
        break;                                                              //BMG 17-10-2008 ASN/Directs
    case SYS_PUB:                                                           //BMG 17-10-2008 ASN/Directs
        disp_msg("Process workfile type : SYS_PUB");                        //BMG 17-10-2008 ASN/Directs
        pwPQRec = &lrtp[log_unit]->pq_PUB;                                  //BMG 17-10-2008 ASN/Directs
        pPQ = &pq[*pwPQRec];                                                //BMG 17-10-2008 ASN/Directs
        plFileNum = &lrtp[log_unit]->lPUBfnum;                              //BMG 17-10-2008 ASN/Directs
        lMinSize = 1;                                                       //BMG 17-10-2008 ASN/Directs
        break;                                                              //BMG 17-10-2008 ASN/Directs
    case SYS_DIR:                                                           //BMG 17-10-2008 ASN/Directs
        disp_msg("Process workfile type : SYS_DIR");                        //BMG 17-10-2008 ASN/Directs
        pwPQRec = &lrtp[log_unit]->pq_DIR;                                  //BMG 17-10-2008 ASN/Directs
        pPQ = &pq[*pwPQRec];                                                //BMG 17-10-2008 ASN/Directs
        plFileNum = &lrtp[log_unit]->lDIRfnum;                              //BMG 17-10-2008 ASN/Directs
        lMinSize = 1;                                                       //BMG 17-10-2008 ASN/Directs
        break;                                                              //BMG 17-10-2008 ASN/Directs
    default:                                                                //SDH 22-June-2006
        sprintf(msg, "ERROR: Unknown workfile type: %d", type);             //SDH 22-June-2006
        disp_msg(msg);                                                      //SDH 22-June-2006
        return RC_SERIOUS_ERR;                                              //SDH 22-June-2006
    }                                                                       //SDH 22-June-2006

    //Skip processing if there is no file                                   //SDH 22-June-2006
    if (*plFileNum == 0L) {                                                 // Null fix SDH 29-Sep-2009
        *pwPQRec = 0;                                                       // Null fix SDH 29-Sep-2009
        disp_msg("    No file to process");                                 // Null fix SDH 29-Sep-2009
        return RC_OK;                                                       //SDH 22-June-2006
    }                                                                       // Null fix SDH 29-Sep-2009

    // Close workfile                                                       //SDH 22-June-2006
    s_close (0, *plFileNum);                                                //SDH 22-June-2006
    log_file_close (*plFileNum);                                            //SDH 22-June-2006

    // Set process queue entry to READY to enable processing of workfile
    // (unless file is emtpy, in which case delete it, and clear pq entry)
    strncpy(fnm, pPQ->fname, sizeof(fnm));                                  // Null fix SDH 29-Sep-2009
    fnm[sizeof(fnm) - 1] = 0x00;                                            // Null fix SDH 29-Sep-2009
    fsz = filesize (fnm);
    if (fsz >= lMinSize) {                                                  //SDH 22-June-2006
        // Set process queue entry to READY                                 //SDH Bug fix 3-Oct-2006
        sprintf(msg, "Setting status of %s to READY, slot %d",              // Null fix SDH 29-Sep-2009
                fnm, *pwPQRec);                                             // Null fix SDH 29-Sep-2009
        disp_msg(msg);                                                      //SDH Bug fix 3-Oct-2006
        pPQ->state = PST_READY;                                             //SDH 22-June-2006
    } else {
        sprintf(msg, "File is null, deleting %s, slot %d", fnm, *pwPQRec);  // Null fix SDH 29-Sep-2009
        disp_msg(msg);                                                      // Null fix SDH 29-Sep-2009
        // Delete workfile                                                  //SDH Bug fix 3-Oct-2006
        s_delete(0, pPQ->fname);                                            //SDH 22-June-2006
        // Free-up process queue entry and remove link from active table
        pPQ->state = PST_FREE;                                              //SDH 22-June-2006
        pPQ->submitcnt = 0;                                                 // 11-09-2007 13 BMG
        pPQ->type = 0;                                                      // 11-09-2007 13 BMG
        pPQ->unit = 0;                                                      // 11-09-2007 13 BMG
        memset(pPQ->DevType, 0x20, 1);                                      // BMG 16-09-2008 MC70
        memset(pPQ->fname, 0x00, sizeof(pPQ->fname));                       // Null fix SDH 29-Sep-2009
        pPQ->last_access_time = 0;                                          // 11-09-2007 13 BMG
    }
    *plFileNum = 0L;                                                        //SDH 22-June-2006
    *pwPQRec = 0;                                                           //SDH 22-June-2006

    return RC_OK;

}


URC dealloc_report_buffer( RBUF *rbufp )
{
    LONG rc;

    // Deallocate report buffer
    rc = s_mfree( (void *)rbufp->buff );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
        disp_msg(msg);
        return RC_SERIOUS_ERR;
    }
    rbufp->buff = (void *)NULL;

    // Deallocate RBUF instance
    rc = s_mfree( (void *)rbufp );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        sprintf(msg, "Unable to deallocate storage. RC:%08lX", rc);
        disp_msg(msg);
        return RC_SERIOUS_ERR;
    }

    return RC_OK;
}


URC dealloc_lrt_table(UWORD uwUnit) {                                       //SDH Bug Fix 29-July-2006

    UWORD uwFirstUnit = uwUnit;                                             //SDH Bug Fix 29-July-2006
    UWORD uwLastUnit = uwUnit;                                              //SDH Bug Fix 29-July-2006
    LONG rc;
    URC temprc = RC_OK;

    //If ALL_UNITS then get loop to process all units                       //SDH Bug Fix 29-July-2006
    if (uwUnit == ALL_UNITS) {                                              //SDH Bug Fix 29-July-2006
        uwFirstUnit = 0;                                                    //SDH Bug Fix 29-July-2006
        uwLastUnit = MAX_UNIT - 1;                                          //SDH Bug Fix 29-July-2006
    }                                                                       //SDH Bug Fix 29-July-2006

    for (uwUnit = uwFirstUnit; uwUnit <= uwLastUnit; uwUnit++) {            //SDH Bug Fix 29-July-2006

        if (lrtp[uwUnit] != NULL) {                                         //SDH Bug Fix 29-July-2006

            //Process any outstanding workfiles                             //SDH Bug fix 29-July-2006
            process_workfile(uwUnit, SYS_LAB);                              //SDH Bug fix 29-July-2006
            process_workfile(uwUnit, SYS_GAP);                              //SDH Bug fix 29-July-2006
            process_workfile(uwUnit, SYS_CB);                               //BMG 01-09-2008 17 ASN/Directs
            process_workfile(uwUnit, SYS_PUB);                              //BMG 01-09-2008 17 ASN/Directs
            process_workfile(uwUnit, SYS_DIR);                              //BMG 01-09-2008 17 ASN/Directs

            // Free up report buffer (if necessary)
            if (lrtp[uwUnit]->rbufp != NULL) {                              //SDH Bug Fix 29-July-2006
                dealloc_report_buffer(lrtp[uwUnit]->rbufp);                 //SDH Bug Fix 29-July-2006
                lrtp[uwUnit]->rbufp = (void *)NULL;                         //SDH Bug Fix 29-July-2006
            }
            rc = s_mfree((void *)lrtp[uwUnit]);                             //SDH Bug Fix 29-July-2006
            if (rc==0L) {
                lrtp[uwUnit] = NULL;                                        //SDH Bug Fix 29-July-2006
                temprc = _min(temprc, RC_OK);
            } else {
                // log_error
                temprc = _min(temprc, RC_IGNORE_ERR);
            }
        }
    }

    return temprc;
}


///////////////////////////////////////////////////////////////////////////
///                                                                      //
///   Return formatted timestamp                                         // SW2.4
///                                                                      //
///////////////////////////////////////////////////////////////////////////

VOID form_timestamp( BYTE *buf, WORD lth )
{
    TIMEDATE now;
    LONG ms, hh, mm, ss;
    BYTE tbuf[32];
    s_get(T_TD, 0L, (void *)&now, TIMESIZE);
    ms = now.td_time;
    hh = ms / 3_600_000L;
    ms -= hh * 3_600_000L;
    mm = ms / 60_000L;
    ms -= mm * 60_000L;
    ss = ms / 1_000L;
    ms -= ss * 1_000L;
    sprintf( tbuf, "%04d/%02d/%02d %02ld:%02ld:%02ld.%03ld",
             now.td_year, now.td_month, now.td_day,
             hh, mm, ss, ms );
    memcpy( buf, tbuf, lth );

}


///////////////////////////////////////////////////////////////////////////
///
///   Convert known length string str to WORD
///
///////////////////////////////////////////////////////////////////////////

WORD satoi( BYTE *str, WORD lth )
{
    char temp[8];
    memcpy( temp, (char *)str, lth);
    *(temp+lth) = 0x00;
    return atoi(temp);
}


///////////////////////////////////////////////////////////////////////////
///
///   Convert known length string str to WORD
///
///////////////////////////////////////////////////////////////////////////

LONG satol( BYTE *str, WORD lth )
{
    char temp[16];
    memcpy( temp, (char *)str, lth);
    *(temp+lth) = 0x00;
    return atol(temp);
}


///////////////////////////////////////////////////////////////////////////
///
///   Load an array of bytes with an ASCII converted integer
///
///////////////////////////////////////////////////////////////////////////

void WordToArray(BYTE *pbDest, UWORD uwDestLen, WORD wNum) {                //SDH 14-Sep-2006 Planners

    BYTE abFormat[10];                                                      //SDH 14-Sep-2006 Planners
    BYTE abBuffer[31];                                                      //SDH 20-May-2009 Model Day
    if (pbDest == NULL) return;                                             //SDH 14-Sep-2006 Planners
    if (uwDestLen > 30) return;                                             //SDH 14-Sep-2006 Planners
    sprintf(abFormat, "%%0%dd", uwDestLen);                                 //SDH 14-Sep-2006 Planners
    sprintf(abBuffer, abFormat, wNum);                                      //SDH 14-Sep-2006 Planners
    memcpy(pbDest, abBuffer, uwDestLen);                                    //SDH 14-Sep-2006 Planners

}                                                                           //SDH 14-Sep-2006 Planners

void LongToArray(BYTE *pbDest, UWORD uwDestLen, LONG lNum) {                //SDH 14-Sep-2006 Planners

    BYTE abFormat[10];                                                      //SDH 14-Sep-2006 Planners
    BYTE abBuffer[31];                                                      //SDH 20-May-2009 Model Day
    if (pbDest == NULL) return;                                             //SDH 14-Sep-2006 Planners
    if (uwDestLen > 30) return;                                             //SDH 14-Sep-2006 Planners
    sprintf(abFormat, "%%0%dld", uwDestLen);                                //SDH 14-Sep-2006 Planners
    sprintf(abBuffer, abFormat, lNum);                                      //SDH 14-Sep-2006 Planners
    memcpy(pbDest, abBuffer, uwDestLen);                                    //SDH 14-Sep-2006 Planners

}                                                                           //SDH 14-Sep-2006 Planners


// Unpack a binary coded decimal (BCD) number into an ASCII string
// dest, dest_lth     = destination string pointer and length (bytes)
// source, source_lth = source string pointer and length (bytes)
// start_nibble       = nibble to start unpacking (from high nibble in
//                        leftmost byte in source string)
// notes : An odd number of nibbles can safely be unpacked.
//         Dest_lth ensures string overflows will not occur.
void unpack(BYTE *dest, WORD dest_lth, BYTE *source, WORD source_lth,
            WORD start_nibble)
{
    WORD dp, sp;
    sp=dp=0;
    while (sp<source_lth && dp<dest_lth) {
        if (start_nibble--<=0) {
            *(dest+dp++)=((*(source+sp)&0xF0)>>4)+'0';
        }
        if (start_nibble--<=0 && dp<dest_lth) {
            *(dest+dp++)=(*(source+sp)&0x0F)+'0';
        }
        sp++;
    }
    while (dp<dest_lth) {
        *(dest+dp++)=0;
    }
}


// Pack an ASCII string into a binary coded decimal (BCD) number
// dest, dest_lth     = destination string pointer and length (bytes)
// source, source_lth = source string pointer and length (bytes)
// start_nibble       = nibble to start packing (from high nibble in
//                        leftmost byte in dest string)
// notes : An odd number of nibbles can be safely packed.
//         dest_lth ensures string overflows will not occur.
void pack(BYTE *dest, WORD dest_lth,
          BYTE *source, WORD source_lth, WORD start_nibble)
{
    WORD dnp, sp;

    //Touch variable to supress compiler warning.  It is actually never used.
    UNUSED(source_lth);

    memset(dest, 0x00, dest_lth);
    dnp=sp=0;
    while ((dnp/2)<dest_lth) {
        if (dnp>=start_nibble) {
            *(dest+dnp/2)|=((*(source+sp++)&0x0F)<<((1-(dnp%2))*4));
        }
        dnp++;
    }
}

WORD unpack_to_word(BYTE *source, WORD source_lth)
{
    BYTE temp[32];
    unpack(temp, source_lth*2, source, source_lth, 0);
    *(temp+source_lth*2)=0x00;
    return(atoi(temp));
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
        sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
        disp_msg(msg);
        *rbufp = (void *)NULL;
        return RC_SERIOUS_ERR;
    } else {
        sprintf(msg, "rbuf allocated ok : %08lX", (void *)mem.mp_start);
        disp_msg(msg);
        *rbufp = (RBUF *)mem.mp_start;
    }

    // Allocate report buffer
    mem.mp_start = 0L;
    mem.mp_min = (LONG)REPORT_BUFFER;
    mem.mp_max = mem.mp_min;
    rc = s_malloc( (UBYTE)O_NEWHEAP, (MPB *)&mem );
    if (rc<0L) {
        log_event101(rc, 0, __LINE__);
        sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
        disp_msg(msg);
        return RC_SERIOUS_ERR;
    } else {
        sprintf(msg, "buffer allocated ok : %08lX", (void *)mem.mp_start);
        disp_msg(msg);
        (*rbufp)->base = -1L;
        (*rbufp)->end = -1L;
        (*rbufp)->buff = (BYTE *)mem.mp_start;
        return RC_OK;
    }

}




URC alloc_lrt_table(UWORD unit)
{
    LONG rc;
    MPB mem;

    // If this is a reconnection unit then no need to allocate storage as it already
    // has a memory allocation on the LRTP table.

    if (lrtp[unit] != NULL) {                                               // 23-8-2004 PAB
        sprintf(msg, "RFS - Reconnect unit %d", unit);                      // 24-8-2004 PAB
        disp_msg(msg);                                                      // 24-8-2004 PAB
        // simulate a device logoff and then proceed as nornal logon        // 24-8-2004 PAB
        // if there were any outstanding workfiles for this unit then       // 24-8-2004 PAB
        // submit them for processing, then clean up resources              // 24-8-2004 PAB
        if (unit != 255) {                                                  // 24-8-2004 PAB
            //Workfile processing now done within dealloc routine.          // SDH Bug Fix 29-July-2006
            //rc = process_workfile( unit, SYS_LAB );                         // 24-8-2004 PAB
            //rc = process_workfile( unit, SYS_GAP );                         // 24-8-2004 PAB
            rc = dealloc_lrt_table( unit );                                 // 24-8-2004 PAB
        }                                                                   // 24-8-2004 PAB
        //Don't close thesse files -- potentially another unit is using
        //these sessions.
        //close_rfhist( CL_SESSION );                                         // 24-8-2004 PAB
        //close_isf( CL_SESSION );                                            // 24-8-2004 PAB
        //close_idf( CL_SESSION );                                            // 24-8-2004 PAB
        //close_irf( CL_SESSION );                                            // 24-8-2004 PAB
        //close_rfscf( CL_SESSION );                                          // 24-8-2004 PAB
        //close_pgf( CL_SESSION );                                            // 24-8-2004 PAB
        sess -= 1;                                                          // 24-8-2004 PAB
        if (sess < 0) sess = 0;                                             // SDH Bug Fix 29-July-2006
    }                                                                       // 23-8-2004 PAB

    sprintf(msg, "RFS - Allocate unit %d", unit);
    disp_msg(msg);
    mem.mp_start = 0L;
    mem.mp_min = (LONG)sizeof(ACTIVE_LRT);
    mem.mp_max = mem.mp_min;
    rc = s_malloc((UBYTE)O_NEWHEAP, &mem);

    if (rc<0) {
        log_event101(rc, 0, __LINE__);
        sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
        disp_msg(msg);
        lrtp[unit]=(void *)NULL;
        return RC_SERIOUS_ERR;
    } else {
        lrtp[unit]=(ACTIVE_LRT *)mem.mp_start;
        lrtp[unit]->rbufp = (void *)NULL;
        lrtp[unit]->bLocation = 'U';                                        //SDH 19-01-2005 OSSR WAN
        lrtp[unit]->fnum1 = 0;                                              //SDH Bug Fix 30-07-2006
        lrtp[unit]->fnum2 = 0;                                              //SDH Bug Fix 30-07-2006
        lrtp[unit]->fnum3 = 0;                                              //SDH Bug Fix 30-07-2006
        lrtp[unit]->lCBfnum = 0;                                            //BMG 17-10-2008 ASN/Directs
        lrtp[unit]->lPUBfnum = 0;                                           //BMG 17-10-2008 ASN/Directs
        lrtp[unit]->lDIRfnum = 0;                                           //BMG 17-10-2008 ASN/Directs
        lrtp[unit]->pq_sub1 = 0;                                            //SDH Bug Fix 30-07-2006
        lrtp[unit]->pq_sub2 = 0;                                            //SDH Bug Fix 30-07-2006
        lrtp[unit]->pq_CB = 0;                                              //BMG 17-10-2008 ASN/Directs
        lrtp[unit]->pq_PUB = 0;                                             //BMG 17-10-2008 ASN/Directs
        lrtp[unit]->pq_DIR = 0;                                             //BMG 17-10-2008 ASN/Directs
        lrtp[unit]->count1 = 0;                                             //SDH Bug Fix 30-07-2006
        lrtp[unit]->count2 = 0;                                             //SDH Bug Fix 30-07-2006
        memset(lrtp[unit]->MAC, ' ', sizeof(lrtp[unit]->MAC));              //BMG 20 17-12-2009 RF Stabilisation
        memset(lrtp[unit]->abOpName, ' ', sizeof(lrtp[unit]->abOpName));    //SDH 19-01-2005 OSSR WAN
        lrtp[unit]->wRecallCclol = 0;                                   // Mult UOD Rcll 12-8-2011 SDH
        return RC_OK;
    }

}


void dump(BYTE *buff, WORD lth)
{
    WORD i, max;
    BYTE ascii[17];
    UBYTE b;
    BYTE tmsg[128];

    if (!debug) return;                                                     //Streamline SDH 17-Sep-2008

    *(ascii+16) = 0x00;
    max = ((lth/16)+((lth%16)>0))*16;
    for (i=0; i<max; i++) {
        if ((i%16)==0) {
            sprintf( tmsg, "%04X - ", i );
            if (oput==DBG_LOCAL) {
                printf( "%s", tmsg );
            } else {
                s_write( A_EOFOFF, dbg.fnum, tmsg, strlen(tmsg), 0L );
            }
        }
        if (i<lth) {
            b = (UBYTE)*((UBYTE *)(buff)+i);
            sprintf( tmsg, "%02X ", b );
            if (oput==DBG_LOCAL) {
                printf( "%s", tmsg );
            } else {
                s_write( A_EOFOFF, dbg.fnum, tmsg, strlen(tmsg), 0L );
            }
            if (b<32 || b>126) {
                *(ascii+(i%16)) = '.';
            } else {
                *(ascii+(i%16)) = b;
            }
        } else {
            sprintf( tmsg, "   " );
            if (oput==DBG_LOCAL) {
                printf( "%s", tmsg );
            } else {
                s_write( A_EOFOFF, dbg.fnum, tmsg, strlen(tmsg), 0L );
            }
            *(ascii+(i%16)) = ' ';
        }
        if ((i%16)==15) {
            if (oput==DBG_LOCAL) {
                printf( " - [%s]\n", ascii );
            } else {
                sprintf( tmsg, " - [%s]\r\n", ascii );
                s_write( A_EOFOFF, dbg.fnum, tmsg, strlen(tmsg), 0L );
            }
        }
    }
}

// Generate a boots check digit
// bc   = 3 byte right aligned BCD boots code (i.e. cc cc cc)
// bccd = 4 byte right aligned BCD boots code with check dig (i.e. 0c cc cc cd)
void calc_boots_cd(BYTE *bccd, BYTE *bc)
{
    LONG tot, i, nb;

    memset(bccd, 0x00, 4);
    tot=0L;
    for (i=0; i<3; i++) {
        // high nibble
        nb=(*(bc+i)&0xF0)>>4;
        *(bccd+i)|=nb;
        tot+=((7-(i*2))*nb);
        // low nibble
        nb=*(bc+i)&0x0F;
        *(bccd+i+1)|=(nb<<4);
        tot+=((6-(i*2))*nb);
    }
    tot=11-(tot%11);
    if (tot<10) {
        nb=tot;
    } else {
        nb=0;
    }
    *(bccd+3)|=nb;

}

// Calculate and add Boots check digit                                  // Mult UOD Rcll 12-08-2011 SDH
// abInput  = 6 byte boots code                                         // Mult UOD Rcll 12-08-2011 SDH
// abOutput = 7 byte boots code with check digit                        // Mult UOD Rcll 12-08-2011 SDH
void AddBootsCheck(BYTE *abOutput, BYTE *abInput) {                     // Mult UOD Rcll 12-08-2011 SDH

    LONG lTot;                                                          // Mult UOD Rcll 12-08-2011 SDH
    WORD i;                                                             // Mult UOD Rcll 12-08-2011 SDH

    lTot = 0L;                                                          // Mult UOD Rcll 12-08-2011 SDH
    for (i = 0; i < 6; i++) {                                           // Mult UOD Rcll 12-08-2011 SDH
        abOutput[i] = abInput[i];                                       // Mult UOD Rcll 12-08-2011 SDH
        lTot = lTot + ((abInput[i] - '0') * (7L - i));                  // Mult UOD Rcll 12-08-2011 SDH
    }                                                                   // Mult UOD Rcll 12-08-2011 SDH
    lTot = 11L - (lTot % 11L);                                          // Mult UOD Rcll 12-08-2011 SDH
    if (lTot >= 10L) lTot = 0L;                                         // Mult UOD Rcll 12-08-2011 SDH
    abOutput[6] = '0' + lTot;                                           // Mult UOD Rcll 12-08-2011 SDH

}                                                                       // Mult UOD Rcll 12-08-2011 SDH

// Generate an EAN13 check digit
// bc   = 6 byte right aligned BCD bar code (i.e. cc cc cc cc cc cc)
// bccd = 7 byte right aligned BCD bar code with check dig
//        (i.e. 0c cc cc cc cc cc cd)
void calc_ean13_cd(BYTE *bccd, BYTE *bc)
{
    LONG tot, i, nb;

    //if (debug) {
    //   disp_msg(" barcode :");
    //   dump((BYTE *)bc, 6);
    //}

    memset(bccd, 0x00, 7);
    tot=0L;
    for (i=0; i<6; i++) {
        // high nibble
        nb=(*(bc+i)&0xF0)>>4;
        *(bccd+i)|=nb;
        tot+=nb;
        // low nibble
        nb=*(bc+i)&0x0F;
        *(bccd+i+1)|=(nb<<4);
        tot+=(nb*3);
    }
    tot=10-(tot%10);
    if (tot<10) {
        nb=tot;
    } else {
        nb=0;
    }
    *(bccd+6)|=nb;

    //if (debug) {
    //   disp_msg(" barcode(with cd) :");
    //   dump((BYTE *)bccd, 7);
    //}

}

/*SDH 22-Sep-2008

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE item_code[13];
} LRT_PAL;                                    // PAstraMI item info lookup
#define LRT_PAL_LTH sizeof(LRT_PAL)

typedef struct {
   BYTE cmd[3];
   BYTE enforcement[1];        // "1"=none, "2"=force info, "3"=force link
   BYTE url_item[60];
   BYTE banner_item[60];
   BYTE url_link[60];
   BYTE banner_link[60];
} LRT_PAR;                                    // PAstraMI item info response
#define LRT_PAR_LTH sizeof(LRT_PAR)

URC url_enquiry( BYTE *item_code_unp, LRT_PAR *parp )
{
    LONG rc;
    URC urcTemp;
    //BYTE item_code[11];                        // BCD boots/bar code
    BYTE bar_code[11];                                                      // BCD
    BYTE boots_code[4];                                                     // BCD incl cd  (0c cc cc cd)
    BYTE boots_code_ncd[4];                                                 // BCD excl cd  (00 cc cc cc)
    BYTE packed_zeros[8];

    BYTE sbuf[64];
    //BOOLEAN blRedeem;
    BYTE cPrice[8], cBootsCodeUnpCd[8];
    FLOAT fPrice;

    urcTemp = RC_FATAL_ERR;
    memset( (BYTE *)&nvurlrec, 0x00, sizeof(NVURL_REC) );

    // Open files
    if (NvurlOpen()  <= RC_DATA_ERR) return RC_DATA_ERR;
    if (IrfOpen()    <= RC_DATA_ERR) return RC_DATA_ERR;
    if (IrfdexOpen() <= RC_DATA_ERR) return RC_DATA_ERR;                   //SDH 14-01-2005 Promotions

    SrpogOpen();                                                           // SDH 12-Oct-2006 Planners
    SrmodOpen();                                                           // SDH 12-Oct-2006 Planners
    SritmlOpen();                                                          // SDH 12-Oct-2006 Planners
    SritmpOpen();                                                          // SDH 12-Oct-2006 Planners
    SrpogifOpen();                                                         // SDH 12-Oct-2006 Planners
    SrpogilOpen();                                                         // SDH 12-Oct-2006 Planners
    SrpogipOpen();                                                         // SDH 12-Oct-2006 Planners
    SrcatOpen();                                                           // SDH 12-Oct-2006 Planners
    SrsxfOpen();                                                           // SDH 12-Oct-2006 Planners

    // Prepare boots code
    memset(packed_zeros, 0x00, 8);
    memset(boots_code, 0x00, 4);
    memset(boots_code_ncd, 0x00, 4);
    memset(bar_code, 0x00, 11);
    pack(bar_code+5, 6, item_code_unp, 12, 0);

    // Read IRF
    memcpy( irfrec.bar_code, bar_code, 11 );
    rc = IrfRead(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            urcTemp = 1;
        } else {
            urcTemp = RC_DATA_ERR;
        }
    } else {

        // Read NVURL
        memcpy( nvurlrec.btc_parent, irfrec.boots_code, 3 );
        rc = NvurlRead(__LINE__);
        if (rc<=0L) {
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                urcTemp = 1;
            } else {
                urcTemp = RC_DATA_ERR;
            }
        } else {

            // Add Boots check digit and unpack primary and link codes
            sprintf( sbuf, "%01d", (nvurlrec.flags & 0x03 ) + 1 );
            memcpy( parp->enforcement, sbuf, 1 );
            memset( parp->enforcement, 0x00, sizeof(parp->enforcement));
            memcpy( parp->url_item, nvurlrec.url_info,
                    sizeof(nvurlrec.url_info) );

            if (memcmp( nvurlrec.btc_link, packed_zeros , 3 )!=0) {
                memcpy( parp->url_link, nvurlrec.url_link,
                        sizeof(nvurlrec.url_link) );
            } else {
                memset( parp->url_link, 0x00, sizeof(nvurlrec.url_link) );
            }

            // Prepare item banner
            calc_boots_cd( boots_code, irfrec.boots_code );
            unpack( cBootsCodeUnpCd, 7, boots_code, 4, 1 );
            *(cBootsCodeUnpCd+7)=0x00;

            memset( cPrice, 0x00, sizeof(cPrice) );
            unpack( cPrice, 6, irfrec.salepric+2, 3, 0 );

            sscanf( cPrice, "%.2f", &fPrice );
            fPrice/=100.0;

            //blRedeem = (irfrec.indicat3 & 0x04) >> 2;

            memset( parp->banner_item, 0x00, sizeof(parp->banner_item) );
            // Banner disabled
            /*sprintf( parp->banner_item, "%c%c-%c%c-%c%c%c (%.2f) %s",
                     *cBootsCodeUnpCd, *(cBootsCodeUnpCd+1),
                     *(cBootsCodeUnpCd+2), *(cBootsCodeUnpCd+3),
                     *(cBootsCodeUnpCd+4), *(cBootsCodeUnpCd+5), *(cBootsCodeUnpCd+6),
                     fPrice,
                     blRedeem?"REDEEM":"" );*/

/*SDH 22-Sep-2008

            memset( parp->banner_link, 0x00, sizeof(parp->banner_link) );
            if (memcmp( nvurlrec.btc_link, packed_zeros , 3 )!=0) {

                // Read IRF for link item
                memset( irfrec.bar_code, 0x00, 11 );
                memcpy( irfrec.bar_code+8, nvurlrec.btc_link , 3 );
                rc = IrfRead(__LINE__);                                     // SDH 14-03-2005 EXCESS

                if (rc > 0L) {

                    // Prepare link banner
                    calc_boots_cd( boots_code, irfrec.boots_code );
                    unpack( cBootsCodeUnpCd, 7, boots_code, 4, 1 );
                    *(cBootsCodeUnpCd+7)=0x00;

                    unpack( cPrice, 6, irfrec.salepric+2, 3, 0 );
                    *(cPrice+6) = 0x00;
                    sscanf( cPrice, "%f", &fPrice );
                    fPrice/=100.0;

                    //blRedeem = (irfrec.indicat3 & 0x04) >> 2;
                    memset( parp->banner_link, 0x00, sizeof(parp->banner_link) );
                    // Banner disabled
                    /*sprintf( parp->banner_link, "%c%c-%c%c-%c%c%c (%.2f) %s",
                             *cBootsCodeUnpCd, *(cBootsCodeUnpCd+1),
                             *(cBootsCodeUnpCd+2), *(cBootsCodeUnpCd+3),
                             *(cBootsCodeUnpCd+4), *(cBootsCodeUnpCd+5), *(cBootsCodeUnpCd+6),
                             fPrice/100,
                             blRedeem?"REDEEM":"" );*/

/*SDH 22-Sep-2008

                }
            }

            urcTemp = RC_OK;
        }
    }

    // Close files
    IrfClose( CL_SESSION );
    IrfdexClose( CL_SESSION );                                             //SDH 14-01-2005 Promotions
    NvurlClose( CL_SESSION );
    SrpogClose(CL_SESSION);                                                // SDH 12-Oct-2006 Planners
    SrmodClose(CL_SESSION);                                                // SDH 12-Oct-2006 Planners
    SritmlClose(CL_SESSION);                                               // SDH 12-Oct-2006 Planners
    SritmpClose(CL_SESSION);                                               // SDH 12-Oct-2006 Planners
    SrpogifClose(CL_SESSION);                                              // SDH 12-Oct-2006 Planners
    SrpogilClose(CL_SESSION);                                              // SDH 12-Oct-2006 Planners
    SrpogipClose(CL_SESSION);                                              // SDH 12-Oct-2006 Planners
    SrcatClose(CL_SESSION);                                                // SDH 12-Oct-2006 Planners
    SrsxfClose(CL_SESSION);                                                // SDH 12-Oct-2006 Planners

    return urcTemp;
}

*/ //SDH 22-Sep-2008

// Do a general stock enquiry on item and fill in an ENQUIRY
// type          = SENQ_BOOTS - basic Boots code lookup
//                 SENQ_DESC  - plus IDF info, including description
//                 SENQ_SELD  - plus SEL decription
//                 SENQ_TSF   - plus TSF
// item_code_unp = pointer to a 13 byte boots / barcode
// dest          = pointer to an ENQUIRY structure
URC stock_enquiry( BYTE type, BYTE *item_code_unp, ENQUIRY *dest ) {

    LONG rc;
    //LONG nopgf;                              // PAB
    //WORD txtlth;
    BYTE bar_code[11];                                                      // BCD
    BYTE boots_code[4];                                                     // BCD incl cd  (0c cc cc cd)
    BYTE boots_code_ncd[4];                                                 // BCD excl cd  (00 cc cc cc)
    BYTE packed_zeros[8];
    //ISF_REC isfrec = {0};
    IMSTC_REC imstcrec = {0};
    BYTE sbuf[64], txtbuf[256];
    BOOLEAN fIrfRecFound = FALSE;                                           // Streamline SDH 17-Sep-2008
    BOOLEAN fPgfRecFound = FALSE;                                           // Streamline SDH 17-Sep-2008

    disp_msg("In: stock_enquiry()");

//    // return error if required files are not open
//    if (irf.sessions==0 || irfdex.sessions==0 ||                            //SDH 14-01-2005 Promotions
//        idf.sessions==0 || isf.sessions==0) {                               //SDH 14-01-2005 Promotions
//        return RC_FILE_ERR;
//    }

    //memset( (BYTE *)&irfrec, 0x00, sizeof(IRF_REC) );
    memset(packed_zeros, 0x00, 8);
    memset(boots_code, 0x00, 4);
    memset(boots_code_ncd, 0x00, 4);

    // Pack ASCII item code
    memset(bar_code, 0x00, 11);
    pack(bar_code+5, 6, item_code_unp, 12, 0);

    // Read IRF
    memcpy( irfrec.bar_code, bar_code, 11 );
    rc = IrfRead(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            fIrfRecFound = FALSE;                                           // Streamline SDH 17-Sep-2008
            return 1;
        }
        return RC_DATA_ERR;
    }

    //IRF read OK
    fIrfRecFound = TRUE;                                                    // Streamline SDH 17-Sep-2008

    calc_boots_cd(boots_code, irfrec.boots_code);
    memcpy(boots_code_ncd+1, irfrec.boots_code, 3);
    unpack(dest->boots_code, 7, boots_code, 4, 1);
    unpack(dest->item_price, 6, irfrec.salepric+2, 3, 0);
    dest->redeemable[0] = ((irfrec.indicat3 & 0x04) != 0 ? '*' : ' ');
    // unpack( dest->deal_num, 4, irfrec.deal_num, 2, 0 );        // v4.0
    // v4.0 START
    if (irfrec.dealdata0.uwDealNum == 0 &&                                  //SDH 14-01-2005 Promotions
        irfrec.dealdata1.uwDealNum == 0 &&                                  //SDH 14-01-2005 Promotions
        irfrec.dealdata2.uwDealNum == 0) {                                  //SDH 14-01-2005 Promotions
        *dest->active_deal_flag = 'N';
    } else {
        *dest->active_deal_flag = 'Y';
    }
    // v4.0 END


    //   Set the Recall Type to E if IRF Indicat0 bit 5 is set and IRF Indicat8 bits 6 & 7 are not set
    //   Set the Recall Type to W if IRF Indicat0 bit 5 is set and IRF Indicat8 bit 6 is set but bit 7 is not set
    //   Set the Recall Type to R if IRF Indicat0 bit 5 is set and IRF Indicat8 bit 6 is not set but bit 7 is set
    //   Set the Recall Type to P if IRF Indicat8 bits 6 & 7 are set - ignore IRF Indicat0 bit 5 here
    //   Set the Recall Type to " " otherwise.

    dest->cRecallType = ' ';   //Set to default value - not on recall       // CSk 11-05-2010 Recalls Phase 1
                                                                            // CSk 11-05-2010 Recalls Phase 1
    if (irfrec.indicat0 & 0x10) {                                           // CSk 11-05-2010 Recalls Phase 1
        // If item not authorised for sale (recall active)                  // CSk 11-05-2010 Recalls Phase 1
        if ((!(irfrec.indicat8 & 0x20)) &&  //Bit 6 not set                 // CSk 11-05-2010 Recalls Phase 1
            (!(irfrec.indicat8 & 0x40))) {  //Bit 7 not set                 // CSk 11-05-2010 Recalls Phase 1
            dest->cRecallType = 'E';   //Customer/Emergency Recall          // CSk 11-05-2010 Recalls Phase 1
        } else if ((irfrec.indicat8 & 0x20) &&                              // CSk 11-05-2010 Recalls Phase 1
                 (!(irfrec.indicat8 & 0x40))) {                             // CSk 11-05-2010 Recalls Phase 1
            dest->cRecallType = 'W';   //Withdrawn Recall                   // CSk 11-05-2010 Recalls Phase 1
        } else if ((!(irfrec.indicat8 & 0x20)) &&                           // CSk 11-05-2010 Recalls Phase 1
                     (irfrec.indicat8 & 0x40)) {                            // CSk 11-05-2010 Recalls Phase 1
            dest->cRecallType = 'R';   //100% Returns                       // CSk 11-05-2010 Recalls Phase 1
        }                                                                   // CSk 11-05-2010 Recalls Phase 1
    }                                                                       // CSk 11-05-2010 Recalls Phase 1
                                                                            // CSk 11-05-2010 Recalls Phase 1
    if ((irfrec.indicat8 & 0x20) &&  //Bit 6 set                            // CSk 11-05-2010 Recalls Phase 1
        (irfrec.indicat8 & 0x40)) {  //Bit 7 set                            // CSk 11-05-2010 Recalls Phase 1
        dest->cRecallType = 'P';   //Planner Leaver Recall                  // CSk 11-05-2010 Recalls Phase 1
    }                                                                       // CSk 11-05-2010 Recalls Phase 1


    //If the last deal slot is used then read the IRFDEX                    //SDH 14-01-2005 Promotions
    memset(&irfdexrec, 0x00, sizeof(irfdexrec));                            //SDH 14-01-2005 Promotions
    if (irfrec.dealdata2.uwDealNum != 0) {                                  //SDH 14-01-2005 Promotions
        memcpy(irfdexrec.abItemCodePD, irfrec.boots_code,                   //SDH 14-01-2005 Promotions
               sizeof(irfdexrec.abItemCodePD));                             //SDH 14-01-2005 Promotions
        rc = IrfdexRead(__LINE__);                                          //SDH 14-01-2005 Promotions
    }                                                                       //SDH 14-01-2005 Promotions

    //Populate deals from IRF                                               //SDH 14-01-2005 Promotions
    BYTE bRewd;                                                             //SDH 14-01-2005 Promotions
    WORD wDealNum = irfrec.dealdata0.uwDealNum;                             //SDH 14-01-2005 Promotions
    bRewd = ((wDealNum > 0 && wDealNum <= 9999) ?                           //SDH 14-01-2005 Promotions
             pDealRewdMsg[wDealNum-1]:0);                                   //SDH 14-01-2005 Promotions
    sprintf(sbuf, "%04d%02d", wDealNum, bRewd);                             //SDH 14-01-2005 Promotions
    memcpy(&(dest->Deal[0]), sbuf, sizeof(dest->Deal[0]));                  //SDH 14-01-2005 Promotions

    wDealNum = irfrec.dealdata1.uwDealNum;                                  //SDH 14-01-2005 Promotions
    bRewd = ((wDealNum > 0 && wDealNum <= 9999) ?                           //SDH 14-01-2005 Promotions
             pDealRewdMsg[wDealNum-1]:0);                                   //SDH 14-01-2005 Promotions
    sprintf(sbuf, "%04d%02d", wDealNum, bRewd);                             //SDH 14-01-2005 Promotions
    memcpy(&(dest->Deal[1]), sbuf, sizeof(dest->Deal[1]));                  //SDH 14-01-2005 Promotions

    wDealNum = irfrec.dealdata2.uwDealNum;                                  //SDH 14-01-2005 Promotions
    bRewd = ((wDealNum > 0 && wDealNum <= 9999) ?                           //SDH 14-01-2005 Promotions
             pDealRewdMsg[wDealNum-1]:0);                                   //SDH 14-01-2005 Promotions
    sprintf(sbuf, "%04d%02d", wDealNum, bRewd);                             //SDH 14-01-2005 Promotions
    memcpy(&(dest->Deal[2]), sbuf, sizeof(dest->Deal[2]));                  //SDH 14-01-2005 Promotions

    //Populate deals from IRFDEX                                            //SDH 14-01-2005 Promotions
    for (WORD i = 3; i < 10; i++) {                                         //SDH 14-01-2005 Promotions
        wDealNum = irfdexrec.aDealData[i-3].uwDealNum;                      //SDH 14-01-2005 Promotions
        bRewd = ((wDealNum > 0 && wDealNum <= 9999) ?                       //SDH 14-01-2005 Promotions
                 pDealRewdMsg[wDealNum-1]:0);                               //SDH 14-01-2005 Promotions
        sprintf(sbuf, "%04d%02d", wDealNum, bRewd);                         //SDH 14-01-2005 Promotions
        memcpy(&(dest->Deal[i]), sbuf, sizeof(dest->Deal[i]));              //SDH 14-01-2005 Promotions
    }                                                                       //SDH 14-01-2005 Promotions

    //Read planner database (assume live planners)                          //SDH 14-Sep-2006 Planners
    MEMCPY(sritmlrec.abItemCode, irfrec.boots_code);                        //SDH 14-Sep-2006 Planners
    sritmlrec.ubRecChain = 0;                                               //SDH 14-Sep-2006 Planners
    rc = SritmlRead(__LINE__);                                              //SDH 14-Sep-2006 Planners
    if (rc > 0) {                                                           //SDH 14-Sep-2006 Planners
        WORD_TO_ARRAY(dest->abCoreCount, sritmlrec.uwCoreItemCount);        //SDH 14-Sep-2006 Planners
        WORD_TO_ARRAY(dest->abNonCoreCount, sritmlrec.uwNonCoreItemCount);  //SDH 14-Sep-2006 Planners
    } else {                                                                //SDH 14-Sep-2006 Planners
        MEMSET(dest->abCoreCount, 'F');                                     //SDH 14-Sep-2006 Planners
        MEMSET(dest->abNonCoreCount, 'F');                                  //SDH 14-Sep-2006 Planners
    }                                                                       //SDH 14-Sep-2006 Planners

    //Early exit for less detailed enquiries
    if (type <= SENQ_BOOTS) {
        disp_msg("SENQ_BOOTS exit");
        return RC_OK;
    }

    // Read IDF
    if (debug) dump(irfrec.boots_code, sizeof(irfrec.boots_code));          //SDH 24-01-2005 Bug fix
    calc_boots_cd(idfrec.boots_code, irfrec.boots_code);                    //SDH 24-01-2005 Bug fix
    rc = IdfRead(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            return 1;
        }
        return RC_DATA_ERR;
    }
    unpack( dest->parent_code, 7, idfrec.parent_code, 4, 1 );
    if (strncmp( dest->parent_code, "0000000", 7 )==0) {
        unpack( dest->parent_code, 7, idfrec.boots_code, 4, 1 );
    }
    memcpy( dest->item_desc, idfrec.stndrd_desc, 24 );
    memcpy( dest->status, idfrec.status_1, 1 );

    if ((idfrec.bit_flags_1&0x07)!=0) {                                 // SDH Bug Fix- bits 2, 1 or 0 set?
        dest->supply_method[0] = 'D';                                   // SDH Bug Fix- Direct
    } else if ((idfrec.bit_flags_1&0x08)!=0) {                          // SDH Bug Fix- bit 3 set?
        dest->supply_method[0] = 'C';                                   // SDH Bug Fix- CSR
    } else if ((idfrec.bit_flags_1&0x10) != 0) {                        // SDH Bug Fix- bit 4 set?
        dest->supply_method[0] = 'W';                                   // SDH Bug Fix- Warehouse
    } else {                                                            // SDH 07-03-2005
        dest->supply_method[0] = ' ';                                   // SDH 07-03-2005  Unknown
    }

//    if ((idfrec.bit_flags_1&0x10) != 0) {                               // bit 4 set?
//        dest->supply_method[0] = 'W';                                   // Warehouse
//    } else if ((idfrec.bit_flags_1&0x08)!=0) {                          // bit 3 set?
//        dest->supply_method[0] = 'C';                                   // CSR
//    } else if ((idfrec.bit_flags_1&0x07)!=0) {                          // bits 2, 1 or 0 set?
//        dest->supply_method[0] = 'D';                                   // Direct
//    } else {                                                            //SDH 07-03-2005
//        dest->supply_method[0] = ' ';                                   //SDH 07-03-2005  Unknown
//    }

    unpack( sbuf, 4, idfrec.no_of_bar_codes, 2, 0 );
    if (satol( sbuf, 4 )<2L) {
        calc_ean13_cd( sbuf, idfrec.first_bar_code );
        disp_msg("CHK 1 IDF : barcodes < 2");
    } else {
        calc_ean13_cd( sbuf, idfrec.second_bar_code );
        disp_msg("CHK 1 IDF : barcodes >= 2");
    }
    if (debug) {
        disp_msg("CHK 2 IDF :");
        dump(sbuf, 7);
    }
    unpack( dest->item_code, 13, sbuf, 7, 1 );
    dest->idf_bit_flags_2 = idfrec.bit_flags_2;

    if ((idfrec.bit_flags_1 & 0x20) == 0)                               // 06-02-2008 14 BMG
    {                                                                   // 06-02-2008 14 BMG
        dest->cMarkdown[0] = ' ';                                       // 06-02-2008 14 BMG
    } else {                                                            // 06-02-2008 14 BMG
        dest->cMarkdown[0] = 'Y';                                       // 06-02-2008 14 BMG
    }                                                                   // 06-02-2008 14 BMG

    unpack(sbuf, 6, idfrec.product_grp, 3, 0 );                         // CSk 11-05-2010 Auto Fast Fill
    memcpy(dest->cProductGrp, sbuf, 6);                                 // CSk 11-05-2010 Auto Fast Fill

    PgfOpen();                                                          // 11-09-2007 13 BMG
    memcpy(pgfrec.prod_grp_no, idfrec.product_grp, 3);                  // PAB 23-10-03
    dest->cBusCentre = idfrec.bsns_cntr[0];                             // SDH 17-11-04 OSSR WAN
    rc = PgfRead(__LINE__);                                             // PAB 23-10-03
    PgfClose(CL_SESSION);                                               // Streamline SDH 23-Sep-2008
    if (rc <= 0L) {                                                     // PAB 23-10-03
        fPgfRecFound = FALSE;                                           // PAB 23-10-03
        pgfrec.cOssrFlag = 'N';                                         // SDH 05-01-05
        dest->pcheck_exempt[0] = 'Y';                                   // SDH 17-11-04 OSSR WAN
    } else {                                                            // PAB 23-10-03
        fPgfRecFound = TRUE;                                            // PAB 23-10-03
        dest->pcheck_exempt[0] = pgfrec.price_check_notexempt[0];       // PAB 23-10-03
    }                                                                   // PAB 23-10-03
    //Required for _ENQ code to create new RFHIST record                // SDH 18-03-05
    dest->cPgfOssrFlag = pgfrec.cOssrFlag;                              // SDH 18-03-05

    //Read RFHIST                                                       // SDH 25-01-2005 OSSR WAN
    //Return OSSR item flag if on RFHIST, else the prod grp ossr flag   // SDH 25-01-2005 OSSR WAN
    //Also reset the RFHIST OSSR marker if the PGF has changed.         // SDH 25-01-2005 OSSR WAN
    memcpy(rfhistrec.boots_code, idfrec.boots_code,                     // SDH 25-01-2005 OSSR WAN
           sizeof(rfhistrec.boots_code));                               // SDH 25-01-2005 OSSR WAN
    rc = RfhistRead(__LINE__);                                          // SDH 25-01-2005 OSSR WAN
    if (rc <= 0L) {                                                     // SDH 25-01-2005 OSSR WAN
        dest->cOssrItem = pgfrec.cOssrFlag;                             // SDH 25-01-2005 OSSR WAN
    } else {
        UBYTE ubPgfOssrFlag = (pgfrec.cOssrFlag == 'Y' ? TRUE : FALSE); // SDH 18-03-2005 OSSR WAN
        //Only update the RFHIST OSSR status if                         // SDH 20-04-2005 OSSR WAN
        //the PGF was successfully read                                 // SDH 20-04-2005 OSSR WAN
        if ((ubPgfOssrFlag != rfhistrec.ubPgfOssrFlag) &&               // SDH 20-04-2005 OSSR WAN
            (fPgfRecFound)) {                                           // SDH 20-04-2005 OSSR WAN
            disp_msg("Mismatched OSSR prod group... resetting");        //SDH 18-03-2005 OSSR WAN
            rfhistrec.ubPgfOssrFlag = ubPgfOssrFlag;                    // SDH 18-03-2005 OSSR WAN
            rfhistrec.ubItemOssrFlag = ubPgfOssrFlag;                   // SDH 18-03-2005 OSSR WAN
            RfhistWrite(__LINE__);                                      // SDH 18-03-2005 OSSR WAN
        }                                                               // SDH 18-03-2005 OSSR WAN
        dest->cOssrItem = (rfhistrec.ubItemOssrFlag ? 'Y':'N');         // SDH 25-01-2005 OSSR WAN
    }                                                                   // SDH 25-01-2005 OSSR WAN
    if (rfscfrec1and2.ossr_store != 'W') dest->cOssrItem = 'N';         // SDH 25-01-2005 OSSR WAN

    //Early exit for less detailed enquiries
    if (type <= SENQ_DESC) {
        disp_msg("SENQ_DESC exit");
        return RC_OK;
    }

    // Read ISF
    MEMSET(isfrec.boots_code, 0x00);                                    // SDH 9-Oct-2006 Planners
    MEMCPY(isfrec.boots_code, boots_code);                              // SDH 9-Oct-2006 Planners
    rc = IsfRead(__LINE__);                                             // SDH 9-Oct-2006 Planners
    if (rc <= 0L) {
        if ((rc&0xFFFF) == 0x06C8 || (rc&0xFFFF) == 0x06CD) {
            // No ISF record, so return word-wrapped IDF description
            memset(txtbuf, 0x20, 45);
            format_text(idfrec.stndrd_desc, 24, txtbuf, 45, 15);
            translate_text(txtbuf, 45);                                   // v4.0
            memcpy(dest->sel_desc, txtbuf, 45);
        } else {
            return RC_DATA_ERR;
        }
    } else {
        memcpy(dest->sel_desc, isfrec.sel_desc, 45);
    }

    if (type <= SENQ_SELD) {
        disp_msg("SENQ_SELD exit");
        return RC_OK;
    }

    // Read STOCK record - to determine start of day stock figure
    memcpy(stockrec.boots_code, boots_code, 4);
    open_stock();
    rc = ReadStock(__LINE__);
    close_stock( CL_SESSION );
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            stock.present=FALSE;
        } else {
            return RC_DATA_ERR;
        }
    } else {
        stock.present=TRUE;
        // use date of last delivery into structure
        memcpy( (BYTE *)dest->date_last_delivery, stockrec.date_last_rec,3);    // 20-10-04 PAB
    }

    // look up item on IMSTC (to get latest stock figure)
    open_imstc();
    memset ( imstcrec.bar_code, 0x00, 11 ) ;
    memcpy ( imstcrec.bar_code + 7, boots_code_ncd, 4 ) ;
    if (debug) {
        disp_msg ( "RD IMSTC : " ) ;
        dump ( imstcrec.bar_code, 11 ) ;
    }
    rc = s_read(0, imstc.fnum, (void *)&imstcrec, IMSTC_RECL, IMSTC_KEYL);
    close_imstc(CL_SESSION);
    if (rc<=0L) {
        imstc.present=FALSE;
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            imstc.present=FALSE;
            if (debug) disp_msg("NOF");
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
        if (debug) disp_msg("OK");
    }

    if (imstc.present) {
        // Use stock figure from IMSTC
        sprintf( sbuf, "%06d", imstcrec.stock_figure );
        memcpy( (BYTE *)dest->stock_figure, sbuf, 6 );
        sprintf( sbuf, "%06ld", imstcrec.numitems/100L );
        memcpy( (BYTE *)dest->items_sold_today, sbuf, 6 );
    } else {
        if (stock.present) {
            // use date of last delivery into structure
            memcpy( (BYTE *)dest->date_last_delivery, stockrec.date_last_rec,3);    // 20-10-04 PAB
            // Use stock figure from STOCK
            sprintf( sbuf, "%06d", stockrec.stock_fig );
            memcpy( (BYTE *)dest->stock_figure, sbuf, 6 );
        } else {
            memcpy( (BYTE *)dest->stock_figure, "      ", 6 );
            // Log event 8 to get the item refreshed

        }
        memset( (BYTE *)dest->items_sold_today, '0', 6 );
    }

    if (fIrfRecFound) {
       // if the IRF record was found determine recall status for this item
       // and set new flag on the EQR response.
       dest->cRecallFlag[0] = 'N';                                              // PAB 23-05-2007 Recalls
       dest->cRecallFlag[0] = ((irfrec.indicat0 & 0x10) != 0 ? 'Y' : 'N');      // PAB 23-05-2007 Recalls
       if ((irfrec.indicat8 & 0x60) == 0x60) { dest->cRecallFlag[0] = 'Y'; }    // 30-07-2008 15 BMG
    }

    disp_msg("SENQ exit");
    return RC_OK;

}


// authorise_user
//----------------
// Given a user id and password lookup user on local EALAUTH
// Returns : TRUE if user authorised, FALSE if not
// 'Auth' and 'username' will be loaded with supervisor flag and username if
// user authorised, otherwise they will be left unchanged.
URC authorise_user(BYTE *user, BYTE *password, BYTE *auth, BYTE *username, BYTE *cStockAccess)  //CSk 12-03-2012 SFA
{
    LONG rc;
    BYTE password_p[4];

    // Pack password
    memset( password_p, 0x00, 4 );
    pack( password_p+2, 2, password, 3, 1 );

    // Get password for user
    memset( afrec.operator_no, 0x00, 4 );
    pack( afrec.operator_no+2, 2, user, 3, 1 );
//   if (debug) {
//      disp_msg("RD EALAUTH :");
//      dump( afrec.operator_no, 4 );
//   }

    rc = AfRead(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            // User not on EALAUTH
            return 1;
        } else {
            return RC_DATA_ERR;
        }
    } else {

        // Removed override to make everyone a supervisor to support new functionality
        // in the pocket PC release of the application.
        //*afrec.sup_flag = 'Y'; // Give all users Supervisor Status

        // Compare presented password with one on file
        if (memcmp(password_p, afrec.password, 4) == 0) {
            // Password matches, determine level of authorisation
            if (*afrec.sup_flag == 'Y') {
                *auth = 'S';
            } else {
                *auth = ' ';
            }
            memcpy( username, afrec.operator_name, 15 );
            // Determine if user is authorised to adjust stock figures        //CSk 12-03-2012 SFA
            if (afrec.model_flags_2 & 0x0100) {                               //CSk 12-03-2012 SFA
                *cStockAccess = 'Y';                                          //CSk 12-03-2012 SFA
            } else {                                                          //CSk 12-03-2012 SFA
                *cStockAccess = 'N';                                          //CSk 12-03-2012 SFA
            }                                                                 //CSk 12-03-2012 SFA

            return RC_OK;
        } else {
            return 1;
        }
    }

}


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

UBYTE semaphore_active(BYTE type)
{
    LONG rc;
    PIPETAB pt;

    // determine whether program is already running (via semaphore)
//    rc = s_lookup( T_PIPE, 0,                                        //BMG 17-10-2008 ASN/Directs
//                   (type==SYS_GAP)?SEM_47_RUNNING:SEM_LAB_RUNNING,   //BMG 17-10-2008 ASN/Directs
//                   (void *)&pt, PIPESIZE, PIPESIZE, 0L );            //BMG 17-10-2008 ASN/Directs
//   if (debug) {
//      sprintf( msg, "checking : [%s] RC:%08lX\n",
//               (type==SYS_GAP)?SEM_47_RUNNING:SEM_LAB_RUNNING, rc );
//      disp_msg(msg);
//   }
//    if (rc<0L) {                                                     //BMG 17-10-2008 ASN/Directs
        // log_event101(lrtlg.fnum, 0, __LINE__);
//        if (debug) {                                                 //BMG 17-10-2008 ASN/Directs
//            sprintf( msg, "Err-O pipe %s. RC:%08lX",                 //BMG 17-10-2008 ASN/Directs
//                     (type==SYS_GAP)?SEM_47_RUNNING:SEM_LAB_RUNNING, //BMG 17-10-2008 ASN/Directs
//                     rc );                                           //BMG 17-10-2008 ASN/Directs
//            disp_msg(msg);                                           //BMG 17-10-2008 ASN/Directs
//        }                                                            //BMG 17-10-2008 ASN/Directs
//    }                                                                //BMG 17-10-2008 ASN/Directs


    // Reworked whole function to allow multiple types.     //BMG 17-10-2008 ASN/Directs
    // Code above commented out and all code below is new.  //BMG 17-10-2008 ASN/Directs

    switch (type) {

    case SYS_GAP:
        rc = s_lookup( T_PIPE, 0, SEM_47_RUNNING, (void *)&pt, PIPESIZE, PIPESIZE, 0L );
        if (rc<0L) {
            if (debug) {
                sprintf( msg, "Err-O pipe %s. RC:%08lX", SEM_47_RUNNING, rc );
                disp_msg(msg);
            }
        }
        break;

    case SYS_LAB:
        rc = s_lookup( T_PIPE, 0, SEM_LAB_RUNNING,(void *)&pt, PIPESIZE, PIPESIZE, 0L );
        if (rc<0L) {
            if (debug) {
                sprintf( msg, "Err-O pipe %s. RC:%08lX", SEM_LAB_RUNNING, rc );
                disp_msg(msg);
            }
        }
        break;

    case SYS_CB:
        rc = s_lookup( T_PIPE, 0, SEM_CB_RUNNING,(void *)&pt, PIPESIZE, PIPESIZE, 0L );
        if (rc<0L) {
            if (debug) {
                sprintf( msg, "Err-O pipe %s. RC:%08lX", SEM_CB_RUNNING, rc );
                disp_msg(msg);
            }
        }
        break;

    case SYS_PUB:
        rc = s_lookup( T_PIPE, 0, SEM_PUB_RUNNING,(void *)&pt, PIPESIZE, PIPESIZE, 0L );
        if (rc<0L) {
            if (debug) {
                sprintf( msg, "Err-O pipe %s. RC:%08lX", SEM_PUB_RUNNING, rc );
                disp_msg(msg);
            }
        }
        break;

    case SYS_DIR:
        rc = s_lookup( T_PIPE, 0, SEM_PUB_RUNNING,(void *)&pt, PIPESIZE, PIPESIZE, 0L );
        if (rc<0L) {
            if (debug) {
                sprintf( msg, "Err-O pipe %s. RC:%08lX", SEM_DIR_RUNNING, rc );
                disp_msg(msg);
            }
        }
        break;
    }

    return(rc>0);

}


// v4.0 START
// following functions moved from transact.c due to module size >64KB
void sysdate( LONG *day, LONG *month, LONG *year,
              WORD *hour, WORD *min, LONG *sec )
{
    TIMEDATE now;

    s_get( T_TD, 0L, (void *)&now, TIMESIZE );
    *sec = now.td_time / 1000L;
    *hour = (WORD)(*sec / 3600L);
    *sec -= (LONG)*hour * 3600L;
    *min = (WORD)(*sec / 60L);
    *sec -= (LONG)*min * 60L;
    *day = (LONG)now.td_day;
    *month = (LONG)now.td_month;
    *year = (LONG)now.td_year;
}

void GetSystemDate(B_TIME* pTime, B_DATE* pDate) {                          // 17-11-04 SDH OSSR WAN

    TIMEDATE now;                                                           // 17-11-04 SDH OSSR WAN
    LONG lWork;                                                             // 17-11-04 SDH OSSR WAN

    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                              // 17-11-04 SDH OSSR WAN
    lWork = now.td_time / 1000L;                                            // 17-11-04 SDH OSSR WAN
    pTime->wHour = (WORD)(lWork / 3600L);                                   // 17-11-04 SDH OSSR WAN
    lWork = lWork % 3600L;                                                  // 17-11-04 SDH OSSR WAN
    pTime->wMin = (WORD)(lWork / 60L);                                      // 17-11-04 SDH OSSR WAN
    pTime->wSec = (WORD)(lWork % 60L);                                      // 17-11-04 SDH OSSR WAN
    pDate->wDay = now.td_day;                                               // 17-11-04 SDH OSSR WAN
    pDate->wMonth = now.td_month;                                           // 17-11-04 SDH OSSR WAN
    pDate->wYear = now.td_year;                                             // 17-11-04 SDH OSSR WAN
    pDate->wDOW = now.td_weekday;

    //pDate->wDOW = ConvDOW(ConvGJ( pDate->wDay,                              // 17-11-04 SDH OSSR WAN
    //                              pDate->wMonth,                            // 17-11-04 SDH OSSR WAN
    //                              pDate->wYear ));                          // 17-11-04 SDH OSSR WAN

}                                                                           // 17-11-04 SDH OSSR WAN

void GetYYMMDDHHmm(BYTE* pbYYMMDDHHmm, B_TIME* pTime, B_DATE* pDate) {      //SDH 20-May-2009 Model Day                                                     //SDH 20-May-2009 Model Day
    BYTE temp[11];
    sprintf(temp, "%02d%02d%02d%02d%02d", pDate->wYear % 100,               //SDH 20-May-2009 Model Day
                                          pDate->wMonth,                    //SDH 20-May-2009 Model Day
                                          pDate->wDay,                      //SDH 20-May-2009 Model Day
                                          pTime->wHour,                     //SDH 20-May-2009 Model Day
                                          pTime->wMin);                     //SDH 20-May-2009 Model Day
    memcpy(pbYYMMDDHHmm, temp, 10);                                         //SDH 20-May-2009 Model Day
}                                                                           //SDH 20-May-2009 Model Day

DOUBLE emu_round( DOUBLE num ) {
    DOUBLE ab = abs(num);
    if (num - ab < 0.5) {
        return abs(num);
    } else {
        return abs(num) + 1;
    }
}


//NOTE: This routine assumes that the caller has already read the PLLOL,    // 23-05-2005 SDH Excess
// and will write the PLLOL on return                                       // 23-05-2005 SDH Excess
URC SetListUnpicked(WORD wListId) {                                         // 23-05-2005 SDH Excess

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

#define TEMP_BUFF_SZ 64

typedef struct{
   BYTE *rbuf;       // pointer to input text buffer
   WORD rbuf_sz;     // size of input text buffer
   WORD rbuf_os;     // input text buffer - next byte to process
   BYTE *wbuf;       // pointer to output text buffer
   WORD wbuf_sz;     // size of output text buffer
   WORD wbuf_os;     // output text buffer - next free byte
   WORD wbuf_ll;     // output buffer line length (wrap at this character)
   BYTE *tbuf;       // temporary word buffer
   WORD tbuf_os;     // temp word buffer - next free byte
} FTEXT;

// Get a word from a text string
static BOOLEAN get_word( FTEXT *ftb, WORD *word_size )
{
   BOOLEAN tbufov,rbufov;  // Buffer overflow test
   BOOLEAN delim;          // Delimiter character found
   BOOLEAN term;           // Termination character found
   BYTE tc, nc;            // This, next character

   ftb->tbuf_os = 0;
   do {
      tc = *(ftb->rbuf + ftb->rbuf_os++);
      if ( tc!=0x20 && tc!=0x00 && tc!=0x0D && tc!=0x0A ) {
         *(ftb->tbuf + ftb->tbuf_os++) = tc;
      }
      nc = *(ftb->rbuf + ftb->rbuf_os);
      tbufov = (ftb->tbuf_os > TEMP_BUFF_SZ);
      rbufov = (ftb->rbuf_os >= ftb->rbuf_sz);
      delim = (nc==0x20);
      term = (nc==0x00 || nc==0x0D || nc==0x0A);
   } while ( !(tbufov || rbufov) && !term && (!delim || ftb->tbuf_os==0) );

   *word_size = ftb->tbuf_os;
   return !(tbufov || rbufov || term);
}

// Word format a block of text, wrap every 'wbuf_ll' characters
WORD format_text( BYTE *rbuf, WORD rbuf_sz,
                  BYTE *wbuf, WORD wbuf_sz,
                  WORD wbuf_ll )
{
   BOOLEAN finished = FALSE;
   WORD word_size, space, skip;
   FTEXT ftb;
   BYTE twbuf[TEMP_BUFF_SZ];

   ftb.rbuf = rbuf;
   ftb.rbuf_os = 0;
   ftb.rbuf_sz = rbuf_sz;
   ftb.wbuf = wbuf;
   ftb.wbuf_os = 0;
   ftb.wbuf_sz = wbuf_sz;
   ftb.wbuf_ll = wbuf_ll;
   ftb.tbuf = twbuf;

   // Initialise the output buffer to spaces
   memset( ftb.wbuf, 0x20, ftb.wbuf_sz );

   // Get each word from the input buf. and concatanate it to the output buf.
   skip=0;
   do {
      finished = !get_word( &ftb, &word_size );
      if ( ftb.tbuf_os>0 ) {
         space = ftb.wbuf_ll - (ftb.wbuf_os % ftb.wbuf_ll);
         if ( (skip + ftb.tbuf_os) > space ) {
            // Not enough space on line for word
            ftb.wbuf_os+=space;
            skip=0;
         }
         if ( ftb.wbuf_os + skip + ftb.tbuf_os <= ftb.wbuf_sz ) {
            memcpy( ftb.wbuf + ftb.wbuf_os + skip, ftb.tbuf, ftb.tbuf_os );
            ftb.wbuf_os+=(skip + ftb.tbuf_os);
            if ( (ftb.wbuf_os % ftb.wbuf_ll)==0 ) {
               skip=0;
            } else {
               skip=1;
            }
         } else {
            finished=TRUE;
         }
      }
   } while( !finished );

   return ftb.wbuf_os;

}

// v4.0 START
// Handle code page translations
void translate_text( BYTE *buf, WORD buf_sz )
{
   WORD i;

   // Translate pound sign (simple)
   for ( i=0; i<buf_sz; i++, buf++ ) {
      if ( *buf==0x9C ) {
         *buf = 0xA3;
      }
   }

}

// Convert Gregorian date to Julian day (MJD)
DOUBLE ConvGJ(LONG d, LONG m, LONG y) {

    LONG f, z;
    z = y + (LONG)floor((m - 14L) / 12L);
    f = (LONG)floor((979L * (m - 12L * (LONG)floor((m - 14L) / 12L)) - 2918L) / 32L);
    return d + f + 365.0 * z + (z / 4L) - (z / 100L) + (z / 400L) + 1721118.5;

}

// Convert Julian day (MJD) to Gregorian date
void ConvJG(DOUBLE jd, LONG *day, LONG *month, LONG *year)
{

   LONG d, m, y, a, b, z, c, h;

   z = (LONG)(jd - 1721118.5);
   h = 100 * z - 25;
   a = (LONG)floor(h / 3652425L);
   b = a - (LONG)floor(a / 4);
   y = (LONG)floor((100 * b + h) / 36525L);
   c = b + z - 365 * y - (LONG)floor(y / 4);
   m = (LONG)floor((5 * c + 456) / 153);
   d = c - (LONG)floor((153 * m - 457) / 5);
   if (m > 12L) {
      y += 1L;
      m -= 12L;
   }

   *day = d;
   *month = m;
   *year = y;

}

// Return day of week (0=Sunday, 6=Saturday) given Julian day (MJD)
WORD ConvDOW(DOUBLE jd)
{
   LONG temp;
   temp = (LONG)floor(jd + 1.5) % 7L;
   return (WORD)temp;
}


BOOLEAN IsLeapYear(int year)                                                        //CSk 03-08-2010 Defect 4522
{                                                                                   //CSk 03-08-2010 Defect 4522
        if (year % 4 != 0)   return FALSE;                                          //CSk 03-08-2010 Defect 4522
        if (year % 400 == 0) return FALSE;                                          //CSk 03-08-2010 Defect 4522
        if (year % 100 == 0) return FALSE;                                          //CSk 03-08-2010 Defect 4522
        return TRUE;                                                                //CSk 03-08-2010 Defect 4522
}                                                                                   //CSk 03-08-2010 Defect 4522
                                                                                    //CSk 03-08-2010 Defect 4522
int daysInMonths[] = {00, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};          //CSk 03-08-2010 Defect 4522

int GetDaysInMonth(int year, int month)                                             //CSk 03-08-2010 Defect 4522
{   // Month in range 1 to 12                                                       //CSk 03-08-2010 Defect 4522
    int days = daysInMonths[month];                                                 //CSk 03-08-2010 Defect 4522
                                                                                    //CSk 03-08-2010 Defect 4522
    if (month == 2 && IsLeapYear(year)) // February of a leap year                  //CSk 03-08-2010 Defect 4522
            days += 1;                                                              //CSk 03-08-2010 Defect 4522
                                                                                    //CSk 03-08-2010 Defect 4522
    return days;                                                                    //CSk 03-08-2010 Defect 4522
}                                                                                   //CSk 03-08-2010 Defect 4522


