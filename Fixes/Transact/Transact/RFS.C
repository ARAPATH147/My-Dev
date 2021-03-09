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
// ------------------------------------------------------------------------
#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <flexif.h>
#include <math.h>                                                       // SDH 17-11-04 OSSR WAN

// Boots specific files
#include "file.h"   // v4.01
#include "debugdef.h"  // v4.01
#include "adxsrvfn.h"       /* needed for disp_msg() */
#include "rfsfile.h"
//#include "rfs.h"
#include "wrap.h"
#include "c4680if.h"                                                    // SDH 17-11-04 OSSR WAN
#include "util.h"                                                       // SDH 17-11-04 OSSR WAN
#include "dateconv.h"                                                   // SDH 17-11-04 OSSR WAN
#include "sockserv.h"                                                   // SDH 04-05-05 Chain App
#include "rfglobal.h"                                                    // SDH 22-05-06 VSlick
// v4.01 START

///////////////////////////////////////////////////////////////////////////
///
///   Constansts
///
///////////////////////////////////////////////////////////////////////////

#define WOGAPBF            "D:\\ADX_UDT1\\WOGAPBF.DAT"                              // 13-9-04 pab


///////////////////////////////////////////////////////////////////////////
///                                                                      //
///   Static (private) variables                                         //
///                                                                      //
///////////////////////////////////////////////////////////////////////////

//static BYTE *look_xxx = { "RFS     "};                                      // SDH 17-11-04 OSSR WAN


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
    BYTE abBuffer[30];                                                      //SDH 14-Sep-2006 Planners
    if (pbDest == NULL) return;                                             //SDH 14-Sep-2006 Planners
    if (uwDestLen > 30) return;                                             //SDH 14-Sep-2006 Planners
    sprintf(abFormat, "%%0%dd", uwDestLen);                                 //SDH 14-Sep-2006 Planners
    sprintf(abBuffer, abFormat, wNum);                                      //SDH 14-Sep-2006 Planners
    memcpy(pbDest, abBuffer, uwDestLen);                                    //SDH 14-Sep-2006 Planners

}                                                                           //SDH 14-Sep-2006 Planners

void LongToArray(BYTE *pbDest, UWORD uwDestLen, LONG lNum) {                //SDH 14-Sep-2006 Planners

    BYTE abFormat[10];                                                      //SDH 14-Sep-2006 Planners
    BYTE abBuffer[30];                                                      //SDH 14-Sep-2006 Planners
    if (pbDest == NULL) return;                                             //SDH 14-Sep-2006 Planners
    if (uwDestLen > 30) return;                                             //SDH 14-Sep-2006 Planners
    sprintf(abFormat, "%%0%dld", uwDestLen);                                //SDH 14-Sep-2006 Planners
    sprintf(abBuffer, abFormat, lNum);                                      //SDH 14-Sep-2006 Planners
    memcpy(pbDest, abBuffer, uwDestLen);                                    //SDH 14-Sep-2006 Planners

}                                                                           //SDH 14-Sep-2006 Planners



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

URC direct_open(FILE_CTRL* pFile, BOOLEAN fLogError) {               // SDH 17-03-05 EXCESS

    //if (pFile->fnum > 0) { return RC_OK; }                                  // BMG 10-09-2007
    
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
///   First record is record 0.
//////////////////////////////////////////////////////////////////////////////
LONG ReadDirect(FILE_CTRL* pFile, LONG lRecNum,
                       LONG lLineNumber, WORD wLogLevel) {                  // SDH 29-11-04 CREDIT CLAIM

    LONG lRc;                                                               // SDH 29-11-04 CREDIT CLAIM

    lRc = s_read(A_BOFOFF, pFile->fnum, pFile->pBuffer,                     // SDH 29-11-04 CREDIT CLAIM
                pFile->wRecLth, pFile->wRecLth * (lRecNum));                // SDH 29-11-04 CREDIT CLAIM
    
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
///   WriteDirect
///   Write the direct file without a lock.
///   First record is record 0.
//////////////////////////////////////////////////////////////////////////////
static LONG WriteDirect(FILE_CTRL* pFile, LONG lRecNum,                     // SDH 29-11-04 CREDIT CLAIM
                        LONG lLineNumber, WORD wLogLevel) {                 // SDH 29-11-04 CREDIT CLAIM

    LONG lRc;                                                               // SDH 29-11-04 CREDIT CLAIM

    lRc = s_write(A_FLUSH | A_BOFOFF, pFile->fnum, pFile->pBuffer,          // SDH 29-11-04 CREDIT CLAIM
                pFile->wRecLth, pFile->wRecLth * (lRecNum));                // SDH 29-11-04 CREDIT CLAIM
    
    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x4003) {                                   // SDH 26-05-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);// SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);    // SDH 17-11-04 OSSR WAN
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
///   ReadKeyed
///   Read the keyed file without a lock.
///
//////////////////////////////////////////////////////////////////////////////
LONG ReadKeyed(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel) {  // SDH 17-11-04 OSSR WAN

    LONG lRc = s_read(0, pFile->fnum, pFile->pBuffer,                       // SDH 17-11-04 OSSR WAN
                     pFile->wRecLth, pFile->wKeyLth);                       // SDH 17-11-04 OSSR WAN
    
    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);// SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);    // SDH 17-11-04 OSSR WAN
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
///   ReadKeyed
///   Read the keyed file with a lock.
//////////////////////////////////////////////////////////////////////////////
static LONG ReadKeyedLock(FILE_CTRL* pFile, LONG lLineNumber,               // SDH 22-02-2005 EXCESS
                          WORD wLogLevel) {                                 // SDH 22-02-2005 EXCESS

    //Always wait for 250ms for the lock to release                         // SDH 22-02-2005 EXCESS
    LONG lRc = u_read(1, 0, pFile->fnum, pFile->pBuffer,                    // SDH 22-02-2005 EXCESS
                     pFile->wRecLth, pFile->wKeyLth, 250);                  // SDH 22-02-2005 EXCESS

    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);// SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);    // SDH 17-11-04 OSSR WAN
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
static LONG WriteKeyed(FILE_CTRL* pFile, LONG lLineNumber, WORD wLogLevel) {// SDH 17-11-04 OSSR WAN
    
    LONG lRc = s_write( 0, pFile->fnum, pFile->pBuffer,                     // SDH 17-11-04 OSSR WAN
                        pFile->wRecLth, 0L);                                // SDH 17-11-04 OSSR WAN
    
    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);// SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);    // SDH 17-11-04 OSSR WAN
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
static LONG WriteKeyedUnlock(FILE_CTRL* pFile, LONG lLineNumber,            // SDH 22-02-2005 EXCESS
                             WORD wLogLevel) {                              // SDH 22-02-2005 EXCESS
    
    LONG lRc = u_write(1, 0, pFile->fnum, pFile->pBuffer,                   // SDH 22-02-2005 EXCESS
                       pFile->wRecLth, 0L);                                 // SDH 22-02-2005 EXCESS
    
    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);// SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);    // SDH 17-11-04 OSSR WAN
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
                         WORD wLogLevel) {                               // SDH 22-02-2005 EXCESS
    
    LONG lRc = s_special(0x74, 0, pFile->fnum, pFile->pBuffer,              // SDH 22-02-2005 EXCESS
                        pFile->wKeyLth, 0L, 0L);                            // SDH 22-02-2005 EXCESS
    
    if (lRc <= 0L) {                                                        // SDH 10-03-2005 EXCESS
        if (wLogLevel == LOG_CRITICAL) {                                    // SDH 10-03-2005 EXCESS
            if ((lRc&0xFFFF) != 0x06C8 && (lRc&0xFFFF) != 0x06CD) {         // SDH 10-03-2005 EXCESS
                log_event101(lRc, pFile->wReportNum, lLineNumber);// SDH 17-11-04 OSSR WAN
            }                                                               // SDH 10-03-2005 EXCESS
        } else if (wLogLevel >= LOG_ALL) {                                  // SDH 10-03-2005 EXCESS
            log_event101(lRc, pFile->wReportNum, lLineNumber);    // SDH 17-11-04 OSSR WAN
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
                //log_event101(rc, 0, __LINE__);                       // 1-2-2004 PAB
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

// 1-2-2004 PAB
///////////////////////////////////////////////////////////////////////////
///
///   open_prtlist
///   close_prtlist
///
///////////////////////////////////////////////////////////////////////////
URC open_prtlist(void) {                                                    // 1-7-2004 PAB
    return direct_open(&prtlist, TRUE);                                     // SDH 17-03-05 EXCESS
}                                                                           // 1-7-2004 PAB
URC close_prtlist(WORD type) {                                              // 1-7-2004 PAB
    return close_file(type, &prtlist);                                      // 10-03-2005 SDH EXCESS
}                                                                           // 1-7-2004 PAB

URC open_prtctl(void) {                                                     // 1-7-2004 PAB
    return direct_open(&prtctl, TRUE);                                      // SDH 17-03-05 EXCESS
}                                                                           // 1-7-2004 PAB
URC close_prtctl(WORD type) {                                               // 1-7-2004 PAB
    return close_file(type, &prtctl);                                       // SDH 17-03-05 EXCESS
}                                               

URC open_rfscf(void) {                                                      // SDH 17-03-05 EXCESS
    return direct_open(&rfscf, TRUE);                                       // SDH 17-03-05 EXCESS
}                                                                           // SDH 17-03-05 EXCESS
URC close_rfscf(WORD type) {
    return close_file(type, &rfscf);                                        // SDH 17-03-05 EXCESS
}

URC open_rfok(void) {                                                       // 24-8-2004 PAB
    return direct_open(&rfok, TRUE);                                        // SDH 17-03-05 EXCESS
}                                                                           // 24-8-2004 PAB
URC close_rfok(WORD type) {                                                 // 24-8-2004 PAB
    return close_file(type, &rfok);                                         // SDH 17-03-05 EXCESS
}        

URC open_jobok(void) {                                                      // 13-12-2004 PAB
    return direct_open(&jobok, FALSE);                                      // SDH 17-03-05 EXCESS
}                                                                           // 13-12-2004 PAB
URC close_jobok(WORD type) {                                                // 13-12-2004 PAB
    return close_file(type, &jobok);                                        // SDH 17-03-05 EXCESS
}        

URC open_irf(void) {
    return keyed_open(&irf, TRUE);                                          //SDH 10-03-2005 EXCESS
}
URC close_irf(WORD type) {
    return close_file(type, &irf);                                          //SDH 10-03-2005 EXCESS
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

URC open_rcspi(void) {
    return keyed_open(&rcspi, TRUE);                                        // 25-5-07 PAB 
}                                                                           // 25-5-07 PAB
URC close_rcspi(WORD type) {
    return close_file(type, &rcspi);                                        // 25-5-07 PAB
}

LONG WriteRecall(LONG lLineNumber) {                                        // 25-5-07 PAB
    return WriteKeyed(&recall, lLineNumber, LOG_ALL); 
}


//////////////////////////////////////////////////////////////////////////////
///   ReadIrf
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadIrf(LONG lLineNumber) {                                            // SDH 10-03-2005 EXCESS
    return ReadKeyed(&irf, lLineNumber, LOG_CRITICAL);                      // SDH 10-03-2005 EXCESS
}                                                                           // SDH 10-03-2005 EXCESS


URC open_irfdex(void) {                                                     //SDH 14-01-2005 Promotions
    return keyed_open(&irfdex, TRUE);                                       //SDH 10-03-2005 EXCESS
}                                                                           //SDH 14-01-2005 Promotions
URC close_irfdex(WORD type) {                                               //SDH 14-01-2005 Promotions
    return close_file(type, &irfdex);                                       //SDH 10-03-2005 EXCESS
}                                                                           //SDH 14-01-2005 Promotions
//////////////////////////////////////////////////////////////////////////////
///   ReadIrfdex
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadIrfdex(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&irfdex, lLineNumber, LOG_CRITICAL);                   // SDH 16-05-05 A5C Fixes
}                                                                           // SDH 17-11-04 OSSR WAN

URC open_idf(void) {
    return keyed_open(&idf, TRUE);
}
URC close_idf(WORD type) {
    return close_file(type, &idf);                                          //SDH 10-03-2005 EXCESS
}
LONG ReadIdf(LONG lLineNumber) {                                            // SDH 10-03-2005 EXCESS
    return ReadKeyed(&idf, lLineNumber, LOG_CRITICAL);                      // SDH 10-03-2005 EXCESS
}                                                                           // SDH 10-03-2005 EXCESS
LONG ReadIsf(LONG lLineNumber) {                                            // SDH 9-Oct-2006 Planners
    return ReadKeyed(&isf, lLineNumber, LOG_CRITICAL);                      // SDH 9-Oct-2006 Planners
}                                                                           // SDH 9-Oct-2006 Planners


URC open_stock(void) {
    return keyed_open(&stock, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_stock(WORD type) {
    return close_file(type, &stock);                                        //SDH 10-03-2005 EXCESS
}
LONG ReadStock(LONG lLineNumber) {                                          //SDH 10-03-2005 EXCESS
    return ReadKeyed(&stock, lLineNumber, LOG_CRITICAL);                    //SDH 10-03-2005 EXCESS
}                                                                           //SDH 10-03-2005 EXCESS

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

URC open_af(void) {
    return keyed_open(&af, TRUE);                                           //SDH 10-03-2005 EXCESS
}
URC close_af(WORD type) {
    return close_file(type, &af);                                           //SDH 10-03-2005 EXCESS
}

URC open_isf(void) {
    return keyed_open(&isf, TRUE);                                          //SDH 10-03-2005 EXCESS
}
URC close_isf(WORD type) {
    return close_file(type, &isf);                                          //SDH 10-03-2005 EXCESS
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
///
///   ReadPllol
///
///   Read the direct file without a lock.
///   Always populates the global record structure.
///   The header record is 0.
///
//////////////////////////////////////////////////////////////////////////////
LONG ReadPllol(LONG lRecNum, LONG lLineNum) {                               // SDH 29-11-04 CREDIT CLAIM
    return ReadDirect(&pllol, lRecNum, lLineNum, LOG_ALL);                  // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

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
///
///   WritePllol
///
///   Write the direct file without a lock.
///   Always uses global record structure.
///   The header record is 0.
///
//////////////////////////////////////////////////////////////////////////////
LONG WritePllol(LONG lRecNum, LONG lLineNum) {                              // SDH 29-11-04 CREDIT CLAIM
    return WriteDirect(&pllol, lRecNum, lLineNum, LOG_ALL);                 // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

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

URC open_rfhist(void) {
    return keyed_open(&rfhist, TRUE);                                       //SDH 10-03-2005 EXCESS
}

URC close_rfhist(WORD type) {
    return close_file(type, &rfhist);                                       //SDH 10-03-2005 EXCESS
}
//////////////////////////////////////////////////////////////////////////////
///   ReadRfhist
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadRfhist(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&rfhist, lLineNumber, LOG_CRITICAL);                   // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   WriteRfhist
///   Write the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteRfhist(LONG lLineNumber) {                                        // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&rfhist, lLineNumber, LOG_ALL);                       // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN


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

URC open_rfrdesc(void) {
    return direct_open(&rfrdesc, TRUE);                                     //SDH 10-03-2005 EXCESS
}

URC close_rfrdesc(WORD type) {
    return close_file(type, &rfrdesc);                                      //SDH 10-03-2005 EXCESS
}

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

URC open_nvurl( void ) {
    return keyed_open(&nvurl, TRUE);                                        //SDH 10-03-2005 EXCESS
}
URC close_nvurl( WORD type ) {
    return close_file(type, &nvurl);                                        //SDH 10-03-2005 EXCESS
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

URC open_invok(void) {
    return direct_open(&invok, TRUE);                                       //SDH 10-03-2005 EXCESS
}
URC close_invok(WORD type) {
    return close_file(type, &invok);                                        //SDH 10-03-2005 EXCESS
}

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
         log_event101(usrrc, JOBOK_REP, __LINE__);               // 13-12-04 PAB
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
        log_event101(rc, RFOK_REP, __LINE__);                    // 21-12-04 SDH
        printf("ERR-R RFOK. RC:%081X", rc);                           // 24-08-04 PAB
        return RC_DATA_ERR;                                           // 21-12-04 SDH
    }                                                                 // 24-08-04 PAB 
    // 24-08-04 PAB
    
    return RC_OK;                                                     // 04-11-04 PAB
}                                                                     // 04-11-04 PAB


// Prepare LRT log & RFDEBUG log file - open or create if not present
void prepare_logging()
{

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
// PAB 23-10-03 Include PGF file to read SEL not printed flag to
// implement a new Price Check Exempt feature for non SELed product
// groups

URC open_pgf(void) {
    return keyed_open(&pgf, TRUE);                                          //SDH 10-03-2005 EXCESS
}
URC close_pgf(WORD type) {
    return close_file(type, &pgf);                                          //SDH 10-03-2005 EXCESS
}
//////////////////////////////////////////////////////////////////////////////
///   ReadPgf
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadPgf(LONG lLineNumber) {                                            // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&pgf, lLineNumber, LOG_ALL);                           // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   WritePgf
///   Write the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WritePgf(LONG lLineNumber) {                                           // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&pgf, lLineNumber, LOG_ALL);                          // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN

/*****************************************************************************
***
*** BCSMF file functions
***
*****************************************************************************/
URC open_bcsmf(void) {
    return keyed_open(&bcsmf, TRUE);                                        // SDH 17-03-05 EXCESS
}
URC close_bcsmf(WORD type) {
    return close_file(type, &bcsmf);
}

//////////////////////////////////////////////////////////////////////////////
///   ReadBcsmf
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadBcsmf(LONG lLineNumber) {                                          // SDH 17-11-04 OSSR WAN

    LONG rc = s_read(0, bcsmf.fnum, (void*)&bcsmfrec,                       // SDH 17-11-04 OSSR WAN
                     BCSMF_RECL, BCSMF_KEYL);                               // SDH 17-11-04 OSSR WAN
    if (rc <= 0L) {                                                         // SDH 17-11-04 OSSR WAN
        log_event101(rc, BCSMF_REP, lLineNumber);                 // SDH 17-11-04 OSSR WAN
        if (debug) {                                                        // SDH 17-11-04 OSSR WAN
            sprintf(msg, "Err-R BCSMF. RC:%08lX", rc);                      // SDH 17-11-04 OSSR WAN
            disp_msg(msg);                                                  // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

    return rc;                                                              // SDH 17-11-04 OSSR WAN

}                                                                           // SDH 17-11-04 OSSR WAN

/*****************************************************************************
***
*** CCLOL file functions
***
*****************************************************************************/
URC open_cclol(void) {
    return direct_open(&cclol, TRUE);                                       // SDH 17-03-05 EXCESS
}
URC close_cclol(WORD type) {
    return close_file(type, &cclol);
}
//////////////////////////////////////////////////////////////////////////////
///
///   ReadCclol
///
///   Read the direct file without a lock.
///   Always populates the global record structure.
///   The header record is 0.
///
//////////////////////////////////////////////////////////////////////////////
LONG ReadCclol(LONG lRecNum, LONG lLineNum) {                               // SDH 29-11-04 CREDIT CLAIM
    return ReadDirect(&cclol, lRecNum, lLineNum, LOG_ALL);                  // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

//////////////////////////////////////////////////////////////////////////////
///
///   WriteCclol
///
///   Write the direct file without a lock.
///   Always uses global record structure.
///   The header record is 0.
///
//////////////////////////////////////////////////////////////////////////////
LONG WriteCclol(LONG lRecNum, LONG lLineNum) {                              // SDH 29-11-04 CREDIT CLAIM
    return WriteDirect(&cclol, lRecNum, lLineNum, LOG_ALL);                 // SDH 29-11-04 CREDIT CLAIM
}                                                                           // SDH 29-11-04 CREDIT CLAIM

/*****************************************************************************
***
*** CCILF file functions
***
*****************************************************************************/
URC open_ccilf(void) {
    return keyed_open(&ccilf, TRUE);                                        // SDH 17-03-05 EXCESS
}
URC close_ccilf(WORD type) {
    return close_file(type, &ccilf);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadCcilf
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadCcilf(LONG lLineNumber) {                                          // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&ccilf, lLineNumber, LOG_CRITICAL);                    // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   WriteCcilf
///   Write the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteCcilf(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&ccilf, lLineNumber, LOG_ALL);                        // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN


URC open_cchist(void) {
    return keyed_open(&cchist, TRUE);                                       // SDH 17-03-05 EXCESS
}
URC close_cchist(WORD type) {
    return close_file(type, &cchist);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadCchist
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadCchist(LONG lLineNumber) {                                         // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&cchist, lLineNumber, LOG_CRITICAL);                   // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
//////////////////////////////////////////////////////////////////////////////
///   WriteCchist
///   Write the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG WriteCchist(LONG lLineNumber) {                                        // SDH 17-11-04 OSSR WAN
    return WriteKeyed(&cchist, lLineNumber, LOG_ALL);                       // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN


URC open_ccdirsu(void) {
    return keyed_open(&ccdirsu, TRUE);                                      // SDH 17-03-05 EXCESS
}
URC close_ccdirsu(WORD type) {
    return close_file(type, &ccdirsu);
}
//////////////////////////////////////////////////////////////////////////////
///   ReadCcdirsu
///   Read the keyed file without a lock.
//////////////////////////////////////////////////////////////////////////////
LONG ReadCcdirsu(LONG lLineNumber) {                                        // SDH 17-11-04 OSSR WAN
    return ReadKeyed(&ccdirsu, lLineNumber, LOG_CRITICAL);                  // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN


//////////////////////////////////////////////////////////////////////////////
///
///   Planogram file functions
///
//////////////////////////////////////////////////////////////////////////////

URC open_pogok(void) {return direct_open(&pogok, TRUE);}
URC close_pogok(WORD type) {return close_file(type, &pogok);}
LONG ReadPogok(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&pogok, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG WritePogok(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&pogok, lRecNum, lLineNumber, LOG_ALL);}

URC open_srpog(void) {return keyed_open(&srpog, TRUE);}
URC close_srpog(WORD type) {return close_file(type, &srpog);}
LONG ReadSrpog(LONG lLineNumber) {return ReadKeyed(&srpog, lLineNumber, LOG_CRITICAL);}
LONG WriteSrpog(LONG lLineNumber) {return WriteKeyed(&srpog, lLineNumber, LOG_ALL);}

URC open_srmod(void) {return keyed_open(&srmod, TRUE);}
URC close_srmod(WORD type) {return close_file(type, &srmod);}
LONG ReadSrmod(LONG lLineNumber) {return ReadKeyed(&srmod, lLineNumber, LOG_CRITICAL);}
LONG WriteSrmod(LONG lLineNumber) {return WriteKeyed(&srmod, lLineNumber, LOG_ALL);}

URC open_sritml(void) {return keyed_open(&sritml, TRUE);}
URC close_sritml(WORD type) {return close_file(type, &sritml);}
LONG ReadSritml(LONG lLineNumber) {return ReadKeyed(&sritml, lLineNumber, LOG_CRITICAL);}
LONG WriteSritml(LONG lLineNumber) {return WriteKeyed(&sritml, lLineNumber, LOG_ALL);}

URC open_sritmp(void) {return keyed_open(&sritmp, TRUE);}
URC close_sritmp(WORD type) {return close_file(type, &sritmp);}
LONG ReadSritmp(LONG lLineNumber) {return ReadKeyed(&sritmp, lLineNumber, LOG_CRITICAL);}
LONG WriteSritmp(LONG lLineNumber) {return WriteKeyed(&sritmp, lLineNumber, LOG_ALL);}

URC open_srpogif(void) {return direct_open(&srpogif, TRUE);}
URC close_srpogif(WORD type) {return close_file(type, &srpogif);}
LONG ReadSrpogif(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srpogif, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG WriteSrpogif(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srpogif, lRecNum, lLineNumber, LOG_ALL);}

URC open_srpogil(void) {return direct_open(&srpogil, TRUE);}
URC close_srpogil(WORD type) {return close_file(type, &srpogil);}
LONG ReadSrpogil(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srpogil, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG WriteSrpogil(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srpogil, lRecNum, lLineNumber, LOG_ALL);}

URC open_srpogip(void) {return direct_open(&srpogip, TRUE);}
URC close_srpogip(WORD type) {return close_file(type, &srpogip);}
LONG ReadSrpogip(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srpogip, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG WriteSrpogip(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srpogip, lRecNum, lLineNumber, LOG_ALL);}

URC open_srcat(void) {return direct_open(&srcat, TRUE);}
URC close_srcat(WORD type) {return close_file(type, &srcat);}
LONG ReadSrcat(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srcat, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG WriteSrcat(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srcat, lRecNum, lLineNumber, LOG_ALL);}

URC open_srsdf(void) {return keyed_open(&srsdf, TRUE);}
URC close_srsdf(WORD type) {return close_file(type, &srsdf);}
LONG ReadSrsdf(LONG lLineNumber) {return ReadKeyed(&srsdf, lLineNumber, LOG_CRITICAL);}
LONG WriteSrsdf(LONG lLineNumber) {return WriteKeyed(&srsdf, lLineNumber, LOG_ALL);}

URC open_srsxf(void) {return keyed_open(&srsxf, TRUE);}
URC close_srsxf(WORD type) {return close_file(type, &srsxf);}
LONG ReadSrsxf(LONG lLineNumber) {return ReadKeyed(&srsxf, lLineNumber, LOG_CRITICAL);}
LONG WriteSrsxf(LONG lLineNumber) {return WriteKeyed(&srsxf, lLineNumber, LOG_ALL);}


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
        log_event101(rc, TDTFF_REP, lLineNumber);                 // SDH 29-11-04 CREDIT CLAIM
        if (debug) {                                                        // SDH 29-11-04 CREDIT CLAIM
            sprintf(msg, "Err-R TDTFF. RC:%08lX", rc);                      // SDH 29-11-04 CREDIT CLAIM
            disp_msg(msg);                                                  // SDH 29-11-04 CREDIT CLAIM
        }                                                                   // SDH 29-11-04 CREDIT CLAIM
    }                                                                       // SDH 29-11-04 CREDIT CLAIM

    return rc;                                                              // SDH 29-11-04 CREDIT CLAIM

}                                                                           // SDH 29-11-04 CREDIT CLAIM

//Routines to open and close the CCDMY.  This file is opened locked
//to prevent PDTs from accessing credit claiming functionality
//if RF credit claiming is enabled.  As such, there is no concept
//of sessions with HHTs and so the session count is onmly incremented
//when the file is physically opened
URC open_ccdmy_locked(void) {
    if (ccdmy.sessions==0) {
        ccdmy.fnum = s_open(CCDMY_OFLAGS, CCDMY);
        if (ccdmy.fnum<=0L) {
            if (debug) {
                sprintf(msg, "Err-O CCDMY. RC:%08lX", ccdmy.fnum);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
        ccdmy.sessions = 1;
    }
    return RC_OK;
}

URC close_ccdmy(void) {
    if (ccdmy.sessions != 0) {
        s_close(0, ccdmy.fnum);
        ccdmy.sessions = 0;
    }
    return RC_OK;
}


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
    touch(source_lth);

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



URC alloc_lrt_table(UWORD unit)
{
    LONG rc;
    MPB mem;

    // If this is a reconnection unit then no need to allocate storage as it already 
    // has a memory allocation on the LRTP table. 

    if (lrtp[unit] != NULL) {                                               // 23-8-2004 PAB
        if (debug) {                                                        // 24-8-2004 PAB 
            sprintf(msg, "RFS - Reconnect unit %d", unit);                  // 24-8-2004 PAB  
            disp_msg(msg);                                                  // 24-8-2004 PAB 
        }                                                                   // 24-8-2004 PAB
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

    if (debug) {
        sprintf(msg, "RFS - Allocate unit %d", unit);
        disp_msg(msg);
    }
    mem.mp_start = 0L;
    mem.mp_min = (LONG)sizeof(ACTIVE_LRT);
    mem.mp_max = mem.mp_min;
    rc = s_malloc((UBYTE)O_NEWHEAP, &mem);

    if (rc<0) {
        log_event101(rc, 0, __LINE__);
        if (debug) {
            sprintf(msg, "Unable to allocate storage. RC:%08lX", rc);
            disp_msg(msg);
        }
        lrtp[unit]=(void *)NULL;
        return RC_SERIOUS_ERR;
    } else {
        lrtp[unit]=(ACTIVE_LRT *)mem.mp_start;
        lrtp[unit]->rbufp = (void *)NULL;
        lrtp[unit]->bLocation = 'U';                                        //SDH 19-01-2005 OSSR WAN
        lrtp[unit]->fnum1 = 0;                                              //SDH Bug Fix 30-07-2006
        lrtp[unit]->fnum2 = 0;                                              //SDH Bug Fix 30-07-2006
        lrtp[unit]->fnum3 = 0;                                              //SDH Bug Fix 30-07-2006
        lrtp[unit]->pq_sub1 = 0;                                            //SDH Bug Fix 30-07-2006
        lrtp[unit]->pq_sub2 = 0;                                            //SDH Bug Fix 30-07-2006
        lrtp[unit]->count1 = 0;                                             //SDH Bug Fix 30-07-2006
        lrtp[unit]->count2 = 0;                                             //SDH Bug Fix 30-07-2006
        memset(lrtp[unit]->abOpName, ' ', sizeof(lrtp[unit]->abOpName));    //SDH 19-01-2005 OSSR WAN
        return RC_OK;
    }

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


void dump(BYTE *buff, WORD lth)
{
    WORD i, max;
    BYTE ascii[17];
    UBYTE b;
    BYTE tmsg[128];

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

//////////////////////////////////////////////////////////////////////////////
///
///   UpdateRfhistOssrFlag
///
///   Read RFHIST with no lock
///   If the record exists then,
///      Check OSSR flag
///      If it doesn't match what we want it to be then,
///         Read RFHIST locked
///         Change OSSR item flag
///         Write record unlocked
///   Else (record doesn't exist),
///      Initialise record including OSSR item flag
///      Write it
///
///   NOTE: pBootsCode is packed with check digit
///
//////////////////////////////////////////////////////////////////////////////
/*
URC UpdateRfhistOssrFlag(BYTE cUpdateOssritm, BYTE* pBootsCode) {          // SDH 17-11-04 OSSR WAN

    URC rc;                                                                 // SDH 17-11-04 OSSR WAN
   
    //Quick exit if not OSSR WAN store                                      // SDH 17-11-04 OSSR WAN
    if (rfscfrec1and2.ossr_store != 'W') return RC_OK;                      // SDH 17-11-04 OSSR WAN

    //Quick exit if handheld didn't pass through the flag                   // SDH 17-11-04 OSSR WAN
    if (cUpdateOssritm == ' ') return RC_OK;                               // SDH 17-11-04 OSSR WAN
    
    //Translate the OSSR flag passed into RFHIST speak                      // SDH 17-11-04 OSSR WAN
    if (cUpdateOssritm == 'O') cUpdateOssritm = 'Y';                      // SDH 17-11-04 OSSR WAN

    //Build RFHIST key                                                      // SDH 17-11-04 OSSR WAN
    memcpy( rfhistrec.boots_code, pBootsCode, 4);                           // SDH 17-11-04 OSSR WAN
    if (debug) {                                                            // SDH 17-11-04 OSSR WAN
        sprintf(msg, "RD RFHIST :");                                        // SDH 17-11-04 OSSR WAN
        disp_msg(msg);                                                      // SDH 17-11-04 OSSR WAN
        dump( rfhistrec.boots_code, RFHIST_KEYL );                          // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

    //Read RFHIST without a lock                                            // SDH 17-11-04 OSSR WAN
    rc = s_read( 0, rfhist.fnum, (void *)&rfhistrec,                        // SDH 17-11-04 OSSR WAN
                 RFHIST_RECL, RFHIST_KEYL );                                // SDH 17-11-04 OSSR WAN
    if (rc <= 0L) {                                                         // SDH 17-11-04 OSSR WAN
        //If error is record not found                                      // SDH 17-11-04 OSSR WAN
        if ((rc&0xFFFF) == 0x06C8 || (rc&0xFFFF) == 0x06CD) {               // SDH 17-11-04 OSSR WAN
            rfhist.present = FALSE;                                         // SDH 17-11-04 OSSR WAN
        } else {                                                            // SDH 17-11-04 OSSR WAN
            log_event101(rc, RFHIST_REP, __LINE__);               // SDH 17-11-04 OSSR WAN
            if (debug) {                                                    // SDH 17-11-04 OSSR WAN
                sprintf(msg, "Err-R RFHIST. RC:%08lX", rc);                 // SDH 17-11-04 OSSR WAN
                disp_msg(msg);                                              // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 17-11-04 OSSR WAN
            return RC_DATA_ERR;                                             // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 17-11-04 OSSR WAN
    } else {                                                                // SDH 17-11-04 OSSR WAN
        rfhist.present = TRUE;                                              // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

    //Check the OSSR flag if the record was read successfully               // SDH 17-11-04 OSSR WAN
    //If there's a mismatch then read the file again with a lock, update the// SDH 17-11-04 OSSR WAN 
    //field and write it back unlock                                        // SDH 17-11-04 OSSR WAN
    if (rfhist.present) {                                                   // SDH 17-11-04 OSSR WAN
        
        //If it doesn't already match                                       // SDH 17-11-04 OSSR WAN
        if (rfhistrec.cOssritm != cUpdateOssritm) {                       // SDH 17-11-04 OSSR WAN
                                                                            
            //Read RFHIST with lock and handle errors                       // SDH 17-11-04 OSSR WAN
            //Wait for 250ms for the lock to become free                    // SDH 17-11-04 OSSR WAN
            rc = u_read( 1, 0, rfhist.fnum, (void *)&rfhistrec,             // SDH 17-11-04 OSSR WAN
                         RFHIST_RECL, RFHIST_KEYL, 250 );                   // SDH 17-11-04 OSSR WAN
            if (rc<=0L) {                                                   // SDH 17-11-04 OSSR WAN
                log_event101(rc, RFHIST_REP, __LINE__);                // SDH 17-11-04 OSSR WAN
                if (debug) {                                                // SDH 17-11-04 OSSR WAN
                    sprintf(msg, "Err-R (lock) RFHIST. RC:%08lX", rc);      // SDH 17-11-04 OSSR WAN
                    disp_msg(msg);                                          // SDH 17-11-04 OSSR WAN
                }                                                           // SDH 17-11-04 OSSR WAN
                return RC_DATA_ERR;                                         // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 17-11-04 OSSR WAN
                                                                            
            //Update the OSSR flag                                          // SDH 17-11-04 OSSR WAN
            rfhistrec.cOssritm = cUpdateOssritm;                          // SDH 17-11-04 OSSR WAN
                                                                            
            //Write unlock the PLLDB and handle errors                      // SDH 17-11-04 OSSR WAN
            rc = u_write( 1, 0, rfhist.fnum,                                // SDH 17-11-04 OSSR WAN
                          (void *)&rfhistrec, RFHIST_RECL, 0L );            // SDH 17-11-04 OSSR WAN
            if (rc <= 0L) {                                                 // SDH 17-11-04 OSSR WAN
                if (debug) {                                                // SDH 17-11-04 OSSR WAN
                    sprintf(msg, "Err-W (unlk) RFHIST. RC:%08lX", rc);      // SDH 17-11-04 OSSR WAN
                    disp_msg(msg);                                          // SDH 17-11-04 OSSR WAN
                }                                                           // SDH 17-11-04 OSSR WAN
                return RC_DATA_ERR;                                         // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 17-11-04 OSSR WAN

        //Else the RFHIST record was not found, so we create it             // SDH 17-11-04 OSSR WAN
    } else {                                                                // SDH 17-11-04 OSSR WAN

        //Format the record to default values                               // SDH 17-11-04 OSSR WAN
        memcpy(rfhistrec.boots_code, pBootsCode,                            // SDH 17-11-04 OSSR WAN
               sizeof(rfhistrec.boots_code));                               // SDH 17-11-04 OSSR WAN
        memset(rfhistrec.date_last_pchk, 0x00,                              // SDH 17-11-04 OSSR WAN
               sizeof(rfhistrec.date_last_pchk));                           // SDH 17-11-04 OSSR WAN
        memset(rfhistrec.price_last_pchk, 0x00,                             // SDH 17-11-04 OSSR WAN
               sizeof(rfhistrec.price_last_pchk));                          // SDH 17-11-04 OSSR WAN
        memset(rfhistrec.date_last_gap, 0x00,                               // SDH 17-11-04 OSSR WAN
               sizeof(rfhistrec.date_last_gap));                            // SDH 17-11-04 OSSR WAN
        rfhistrec.cOssritm = cUpdateOssritm;                              // SDH 17-11-04 OSSR WAN
        memset(rfhistrec.resrv, 0xFF, sizeof(rfhistrec.resrv));             // SDH 17-11-04 OSSR WAN

        //Write RFHIST and handle errors                                    // SDH 17-11-04 OSSR WAN
        rc = s_write( 0, rfhist.fnum, (void *)&rfhistrec,                   // SDH 17-11-04 OSSR WAN
                      RFHIST_RECL, 0L );                                    // SDH 17-11-04 OSSR WAN
        if (rc <= 0L) {                                                     // SDH 17-11-04 OSSR WAN
            log_event101(rc, RFHIST_REP, __LINE__);               // SDH 17-11-04 OSSR WAN
            if (debug) {                                                    // SDH 17-11-04 OSSR WAN
                sprintf(msg, "Err-W RFHIST. RC:%08lX", rc);                 // SDH 17-11-04 OSSR WAN
                disp_msg(msg);                                              // SDH 17-11-04 OSSR WAN
            }                                                               // SDH 17-11-04 OSSR WAN
            return RC_DATA_ERR;                                             // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 17-11-04 OSSR WAN

    }                                                                       // SDH 17-11-04 OSSR WAN
    return RC_OK;                                                           // SDH 17-11-04 OSSR WAN
}                                                                           // SDH 17-11-04 OSSR WAN
*/

URC create_new_plist( BYTE *ret_list_id, BYTE *user ) {

    B_DATE nowDate;                                                         // SDH 26-11-04 CREDIT CLAIM
    B_TIME nowTime;                                                         // SDH 26-11-04 CREDIT CLAIM
    WORD wListId;                                                           // SDH 26-11-04 CREDIT CLAIM
    LONG rc;
    BYTE sbuf[64];

    // Read pllol header
    rc = ReadPllol(0L, __LINE__);                                           // SDH 26-11-04 CREDIT CLAIM
    if (rc <= 0L) return RC_DATA_ERR;                                       // SDH 26-11-04 CREDIT CLAIM

    // Get next list id
    wListId = (satoi(pllolrec.list_id, 3)%999) + 1;                         // SDH 26-11-04 CREDIT CLAIM
    sprintf( sbuf, "%03d", wListId);                                        // SDH 26-11-04 CREDIT CLAIM
    memcpy( pllolrec.list_id, sbuf, 3 );

    // Update pllol header
    rc = WritePllol(0L, __LINE__);                                          // SDH 26-11-04 CREDIT CLAIM
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

URC url_enquiry( BYTE *item_code_unp, LRT_PAR *parp )
{
    LONG rc;
    URC urcTemp;
    //BYTE item_code[11];                        // BCD boots/bar code
    BYTE bar_code[11];                                                      // BCD
    BYTE boots_code[4];                                                     // BCD incl cd  (0c cc cc cd)
    BYTE boots_code_ncd[4];                                                 // BCD excl cd  (00 cc cc cc)
    BYTE packed_zeros[8];

    NVURL_REC nvurlrec;
    BYTE sbuf[64];
    //BOOLEAN blRedeem;
    BYTE cPrice[8], cBootsCodeUnpCd[8];
    FLOAT fPrice;

    urcTemp = RC_FATAL_ERR;
    memset( (BYTE *)&nvurlrec, 0x00, sizeof(NVURL_REC) );   

    // Open files
    if (open_nvurl() <= RC_DATA_ERR) return RC_DATA_ERR;
    if (open_irf() <= RC_DATA_ERR) return RC_DATA_ERR;
    if (open_irfdex() <= RC_DATA_ERR) return RC_DATA_ERR;                   //SDH 14-01-2005 Promotions
    
    open_srpog();                                                           // SDH 12-Oct-2006 Planners
    open_srmod();                                                           // SDH 12-Oct-2006 Planners
    open_sritml();                                                          // SDH 12-Oct-2006 Planners
    open_sritmp();                                                          // SDH 12-Oct-2006 Planners
    open_srpogif();                                                         // SDH 12-Oct-2006 Planners
    open_srpogil();                                                         // SDH 12-Oct-2006 Planners
    open_srpogip();                                                         // SDH 12-Oct-2006 Planners
    open_srcat();                                                           // SDH 12-Oct-2006 Planners
    open_srsxf();                                                           // SDH 12-Oct-2006 Planners

    // Prepare boots code
    memset(packed_zeros, 0x00, 8);
    memset(boots_code, 0x00, 4);
    memset(boots_code_ncd, 0x00, 4);
    memset(bar_code, 0x00, 11);
    pack(bar_code+5, 6, item_code_unp, 12, 0);

    // Read IRF
    memcpy( irfrec.bar_code, bar_code, 11 );
    rc = ReadIrf(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            irf.present=FALSE;
            urcTemp = 1;
        } else {
            urcTemp = RC_DATA_ERR;
        }
    } else {

        // Read NVURL
        memcpy( nvurlrec.btc_parent, irfrec.boots_code, 3 );

        if (debug) {
            disp_msg("RD NVURL :");
            dump(nvurlrec.btc_parent, 3);
        }
        rc = s_read( 0, nvurl.fnum, (void *)&nvurlrec, NVURL_RECL, NVURL_KEYL );
        if (rc<=0L) {
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                if (debug) {
                    disp_msg("Not on NVURL");
                }
                urcTemp = 1;
            } else {
                log_event101(rc, NVURL_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R NVURL. RC:%08lX", rc);
                    disp_msg(msg);
                }
                urcTemp = RC_DATA_ERR;
            }
        } else {

            if (debug) {
                sprintf(msg, "OK");
                disp_msg(msg);
            }

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

            memset( parp->banner_link, 0x00, sizeof(parp->banner_link) );
            if (memcmp( nvurlrec.btc_link, packed_zeros , 3 )!=0) {
                
                // Read IRF for link item
                memset( irfrec.bar_code, 0x00, 11 );
                memcpy( irfrec.bar_code+8, nvurlrec.btc_link , 3 );
                rc = ReadIrf(__LINE__);                                     // SDH 14-03-2005 EXCESS
                
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

                }
            }

            urcTemp = RC_OK;         
        }
    }

    // Close files
    close_irf( CL_SESSION );
    close_irfdex( CL_SESSION );                                             //SDH 14-01-2005 Promotions
    close_nvurl( CL_SESSION );

    // Close planner files                                                  // SDH 12-Oct-2006 Planners
    close_srpog(CL_SESSION);                                                // SDH 12-Oct-2006 Planners
    close_srmod(CL_SESSION);                                                // SDH 12-Oct-2006 Planners
    close_sritml(CL_SESSION);                                               // SDH 12-Oct-2006 Planners
    close_sritmp(CL_SESSION);                                               // SDH 12-Oct-2006 Planners
    close_srpogif(CL_SESSION);                                              // SDH 12-Oct-2006 Planners
    close_srpogil(CL_SESSION);                                              // SDH 12-Oct-2006 Planners
    close_srpogip(CL_SESSION);                                              // SDH 12-Oct-2006 Planners
    close_srcat(CL_SESSION);                                                // SDH 12-Oct-2006 Planners
    close_srsxf(CL_SESSION);                                                // SDH 12-Oct-2006 Planners

    return urcTemp;
}

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
    PGF_REC pgfrec = {0};                                                   // PAB 23-10-03
    IMSTC_REC imstcrec = {0};
    BYTE sbuf[64], txtbuf[256];

    disp_msg("In: stock_enquiry()");

    // return error if required files are not open 
    if (irf.sessions==0 || irfdex.sessions==0 ||                            //SDH 14-01-2005 Promotions
        idf.sessions==0 || isf.sessions==0) {                               //SDH 14-01-2005 Promotions
        return RC_FILE_ERR;
    }

    //memset( (BYTE *)&irfrec, 0x00, sizeof(IRF_REC) );
    memset(packed_zeros, 0x00, 8);
    memset(boots_code, 0x00, 4);
    memset(boots_code_ncd, 0x00, 4);

    // Pack ASCII item code
    memset(bar_code, 0x00, 11);
    pack(bar_code+5, 6, item_code_unp, 12, 0);

    // Read IRF
    memcpy( irfrec.bar_code, bar_code, 11 );
    rc = ReadIrf(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            irf.present=FALSE;
            return 1;
        } 
        return RC_DATA_ERR;
    } 

    //IRF read OK
    irf.present=TRUE;
    
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

    //If the last deal slot is used then read the IRFDEX                    //SDH 14-01-2005 Promotions
    memset(&irfdexrec, 0x00, sizeof(irfdexrec));                            //SDH 14-01-2005 Promotions
    if (irfrec.dealdata2.uwDealNum != 0) {                                  //SDH 14-01-2005 Promotions
        memcpy(irfdexrec.abItemCodePD, irfrec.boots_code,                   //SDH 14-01-2005 Promotions
               sizeof(irfdexrec.abItemCodePD));                             //SDH 14-01-2005 Promotions
        rc = ReadIrfdex(__LINE__);                                          //SDH 14-01-2005 Promotions
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
    rc = ReadSritml(__LINE__);                                              //SDH 14-Sep-2006 Planners
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
    rc = ReadIdf(__LINE__);
    if (rc<=0L) {
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            idf.present=FALSE;
            return 1;
        }
        return RC_DATA_ERR;
    }
    idf.present=TRUE;
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
        if (debug) {
            disp_msg("CHK 1 IDF : barcodes < 2");
        }
    } else {
        calc_ean13_cd( sbuf, idfrec.second_bar_code );
        if (debug) {
            disp_msg("CHK 1 IDF : barcodes >= 2");
        }
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

    open_pgf();                                                         // 11-09-2007 13 BMG
    // Read PGF                                                         // SDH 17-11-04 OSSR WAN
    memcpy(pgfrec.prod_grp_no, idfrec.product_grp, 3);                  // PAB 23-10-03
    dest->cBusCentre = idfrec.bsns_cntr[0];                             // SDH 17-11-04 OSSR WAN
    rc = s_read( 0, pgf.fnum, (void *)&pgfrec, PGF_RECL, PGF_KEYL );    // PAB 23-10-03
    if (rc<=0L) {                                                       // PAB 23-10-03
        pgf.present = FALSE;                                            // PAB 23-10-03
        pgfrec.cOssrFlag = 'N';                                         // SDH 05-01-05
        dest->pcheck_exempt[0] = 'Y';                                   // SDH 17-11-04 OSSR WAN
    } else {                                                            // PAB 23-10-03
        pgf.present = TRUE;                                             // PAB 23-10-03
        dest->pcheck_exempt[0] = pgfrec.price_check_notexempt[0];       // PAB 23-10-03
    }                                                                   // PAB 23-10-03
    //Required for _ENQ code to create new RFHIST record                // SDH 18-03-05
    dest->cPgfOssrFlag = pgfrec.cOssrFlag;                              // SDH 18-03-05

    //Read RFHIST                                                       // SDH 25-01-2005 OSSR WAN
    //Return OSSR item flag if on RFHIST, else the prod grp ossr flag   // SDH 25-01-2005 OSSR WAN
    //Also reset the RFHIST OSSR marker if the PGF has changed.         // SDH 25-01-2005 OSSR WAN
    memcpy(rfhistrec.boots_code, idfrec.boots_code,                     // SDH 25-01-2005 OSSR WAN
           sizeof(rfhistrec.boots_code));                               // SDH 25-01-2005 OSSR WAN
    rc = ReadRfhist(__LINE__);                                          // SDH 25-01-2005 OSSR WAN
    if (rc <= 0L) {                                                     // SDH 25-01-2005 OSSR WAN
        dest->cOssrItem = pgfrec.cOssrFlag;                             // SDH 25-01-2005 OSSR WAN
    } else {
        UBYTE ubPgfOssrFlag = (pgfrec.cOssrFlag == 'Y' ? TRUE : FALSE); // SDH 18-03-2005 OSSR WAN
        //Only update the RFHIST OSSR status if                         // SDH 20-04-2005 OSSR WAN
        //the PGF was successfully read                                 // SDH 20-04-2005 OSSR WAN
        if ((ubPgfOssrFlag != rfhistrec.ubPgfOssrFlag) &&               // SDH 20-04-2005 OSSR WAN
            (pgf.present)) {                                            // SDH 20-04-2005 OSSR WAN
            if (debug) disp_msg("Mismatched OSSR prod group... resetting");//SDH 18-03-2005 OSSR WAN
            rfhistrec.ubPgfOssrFlag = ubPgfOssrFlag;                    // SDH 18-03-2005 OSSR WAN
            rfhistrec.ubItemOssrFlag = ubPgfOssrFlag;                   // SDH 18-03-2005 OSSR WAN
            WriteRfhist(__LINE__);                                      // SDH 18-03-2005 OSSR WAN
        }                                                               // SDH 18-03-2005 OSSR WAN
        dest->cOssrItem = (rfhistrec.ubItemOssrFlag ? 'Y':'N');         // SDH 25-01-2005 OSSR WAN
    }                                                                   // SDH 25-01-2005 OSSR WAN
    if (rfscfrec1and2.ossr_store != 'W') dest->cOssrItem = 'N';         // SDH 25-01-2005 OSSR WAN

    //Early exit for less detailed enquiries
    if (type <= SENQ_DESC) {
        if (debug) disp_msg("SENQ_DESC exit");
        return RC_OK;
    }

    // Read ISF
    MEMSET(isfrec.boots_code, 0x00);                                    // SDH 9-Oct-2006 Planners
    MEMCPY(isfrec.boots_code, boots_code);                              // SDH 9-Oct-2006 Planners
    rc = ReadIsf(__LINE__);                                             // SDH 9-Oct-2006 Planners
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

    if (irf.present==TRUE) {
       // if the IRF record was found determine recall status for this item
       // and set new flag on the EQR response.
       dest->cRecallFlag[0] = 'N';                                              // PAB 23-05-2007 Recalls
       dest->cRecallFlag[0] = ((irfrec.indicat0 & 0x10) != 0 ? 'Y' : 'N');      // PAB 23-05-2007 Recalls    
       if ((irfrec.indicat8 & 0x60) == 0x60) { dest->cRecallFlag[0] = 'Y'; }    // 30-07-2008 15 BMG 
    }

    if (debug) {
        disp_msg("SENQ exit");
    }
    return RC_OK;
}

URC sales_enquiry(BYTE *item_code_unp, LONG item_price, LRT_ISR *isrp)
{
    LONG rc;
    BYTE bar_code[11];                                                      // BCD
    URC usrrc;
    IMSTC_REC imstcrec = {0};
    IMF_REC imfprec = {0};
    BYTE sbuf[64];

    touch(item_price);                                                      //Prevent compiler warning - variable not used

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

URC store_sales_enquiry(LRT_SSR *ssrp)
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
    touch(wrf_lth);                                                         //SDH - Prevent compiler warning - var not used

    // Accumulate totals
    tsf_rps = 508L / TSF_RECL;                                              // Records per sector
    psbt_rps = 508L / PSBT_RECL;                                            // Records per sector
    wrf_rps = 508L / WRF_RECL;                                              // Records per sector
    touch(wrf_rps);                                                         //SDH - Prevent compiler warning - var not used

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

//////////////////////////////////////////////////////////////////////////////////
///
///   process_gap
///
///   Called from CMD_GAP to perform the majority of the gap processing
///
//////////////////////////////////////////////////////////////////////////////////

URC process_gap( BYTE *pass_list_id, BYTE *seq,
                 BYTE *item_code_unp, BYTE *boots_code_unp,
                 BYTE *current, BYTE *fill_qty, BYTE gap_flag,
                 BYTE cUpdateOssritm, UWORD log_unit ) {                   // 17-11-04 SDH OSSR WAN

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

    //Avoid compiler warning                                                // 16-11-04 SDH
    touch(log_unit);                                                        // 16-11-04 SDH
    
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
    urc = open_rfscf();                                                     // 07-04-04 PAB
    if (urc!=RC_OK) return urc;                                             // 07-04-04 PAB
    rc = s_read(A_BOFOFF, rfscf.fnum, (void *)&rfscfrec1and2, RFSCF_RECL, 0L);//16-11-04 SDH
    if (rc<=0L) {                                                           // 07-04-04 PAB
        log_event101(rc, RFSCF_REP, __LINE__);                         // 07-04-04 PAB
        //sprintf(msg, "Err-R RFSCF. RC:%08lX", urc);                       // 07-04-04 PAB
        //disp_msg(msg);                                                    // 07-04-04 PAB
        return RC_DATA_ERR;                                                 // 16-11-04 SDH
    }                                                                       // 07-04-04 PAB
    urc = close_rfscf(CL_SESSION);                                          // 07-04-04 PAB
    write_gap = satol(rfscfrec1and2.phase,1);                               // 16-11-04 PAB

    //Read RFHIST                                                           // SDH 17-11-04 OSSR WAN
    //Missing RFHIST Records will not be tolerated - since the ENQ          // SDH 17-11-04 OSSR WAN
    //should have added one                                                 // SDH 17-11-04 OSSR WAN
    memcpy(rfhistrec.boots_code, boots_code, sizeof(rfhistrec.boots_code)); // SDH 17-11-04 OSSR WAN
    rc = ReadRfhist(__LINE__);                                              // SDH 17-11-04 OSSR WAN
    if (rc <= 0) return RC_DATA_ERR;                                        // SDH 17-11-04 OSSR WAN

    //Ensure that the RFHIST OSSR item flag is up to date                   // SDH 17-11-04 OSSR WAN
    if ((rfscfrec1and2.ossr_store == 'W') &&                                // SDH 17-11-04 OSSR WAN
        (cUpdateOssritm != ' ')) {                                         // SDH 17-11-04 OSSR WAN
        rfhistrec.ubItemOssrFlag = (cUpdateOssritm == 'O' ? TRUE:FALSE);   // SDH 19-03-05 OSSR WAN
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
        rc = WriteRfhist(__LINE__);                                         // SDH 17-11-04 OSSR WAN
        if (rc <= 0) return RC_DATA_ERR;                                    // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

    // Build the PLLDB record                                               // SDH 22-02-05 EXCESS
    memset(&plldbrec, '0', sizeof(plldbrec));                               // SDH 22-02-05 EXCESS
    memset(plldbrec.time_sf_count, 0xFF, sizeof(plldbrec.time_sf_count));   // SDH 22-02-05 EXCESS
    memset(plldbrec.time_bs_count, 0xFF, sizeof(plldbrec.time_bs_count));   // SDH 22-02-05 EXCESS
    memset(plldbrec.time_ossr_count, 0xFF,                                  // SDH 22-02-05 EXCESS
           sizeof(plldbrec.time_ossr_count));                               // SDH 22-02-05 EXCESS
    memset(plldbrec.filler, 'X', sizeof(plldbrec.filler));                  // SDH 22-02-05 EXCESS
    memcpy(plldbrec.list_id, pass_list_id, 3);                              // SDH 22-02-05 EXCESS
    memcpy(plldbrec.seq, seq, 3);                                           // SDH 22-02-05 EXCESS
    memcpy(plldbrec.boots_code, boots_code, 4);                             // SDH 22-02-05 EXCESS
    plldbrec.gap_flag[0] = gap_flag;                                        // SDH 22-02-05 EXCESS
    memcpy(plldbrec.sales_figure, (enqbuf.items_sold_today)+2, 4 );         // SDH 22-02-05 EXCESS
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

// authorise_user
//----------------
// Given a user id and password lookup user on local EALAUTH
// Returns : TRUE if user authorised, FALSE if not
// 'Auth' and 'username' will be loaded with supervisor flag and username if
// user authorised, otherwise they will be left unchanged.
URC authorise_user(BYTE *user, BYTE *password, BYTE *auth, BYTE *username)
{
    LONG rc;
    BYTE password_p[4];
    EALAUTH_REC afrec = {0};

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
    rc = s_read(0, af.fnum, (void *)&afrec, EALAUTH_RECL, EALAUTH_KEYL);
    if (rc<=0L) {
        af.present=FALSE;
        if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
            // User not on EALAUTH
            if (debug) {
                sprintf(msg, "NOF");
                disp_msg(msg);
            }
            return 1;
        } else {
            log_event101(rc, EALAUTH_REP, __LINE__);
            if (debug) {
                sprintf(msg, "Err-R EALAUTH. RC:%08lX", rc);
                disp_msg(msg);
            }
            return RC_DATA_ERR;
        }
    } else {
        af.present=TRUE;
        if (debug) {
            disp_msg("OK");
        }

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
            return RC_OK;
        } else {
            return 1;
        }
    }

}

// Create a unique workfile for a handheld to use during it's session
URC prepare_workfile(WORD log_unit, BYTE type) {
    
    WORD i, wFoundEntry;
    LONG ltime, rc;                                                     // 11-09-2007 13 BMG
    TIMEDATE now;
    BYTE gapbfrec[32];
    BYTE sbuf[64];
    time_t CurrTime;                                                    // 11-09-2007 13 BMG
    
    // Get current time
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                          // 11-09-2007 13 BMG
    ltime = now.td_time;

    sprintf( msg, "unit = %d, time = %ld\n", log_unit, ltime );         // 11-09-2007 13 BMG
    disp_msg(msg);

    //Do nothing if a workfile already exists
    if (type == SYS_LAB) {
        rc = lrtp[log_unit]->fnum1;
    } else {
        rc = lrtp[log_unit]->fnum2;
    }
    if (rc > 0) return RC_OK;

    // Locate a free entry
    wFoundEntry = -1;
    for (i=0; i<MAX_CONC_UNITS && wFoundEntry == -1; i++) {
        if (pq[i].state==PST_FREE) {
            
            wFoundEntry = i;

            // Set process queue entry to allocated and fill in table entry,
            pq[i].state = PST_ALLOC;
            pq[i].unit = log_unit;
            pq[i].submitcnt = 0;                                                        // 26-1-2007 PAB
            
            time(&CurrTime);                                                            // 11-09-2007 13 BMG
            pq[i].last_access_time = CurrTime;                                          // 11-09-2007 13 BMG

            // Generate a unique filename, appropriate to current 'type'
            switch (type) {
            case SYS_LAB:
                sprintf( sbuf, "CHKWK:%06ld.%03d",
                         ((ltime/100L)%999999L), log_unit );                            // 11-09-2007 13 BMG
                memcpy( pq[i].fname, sbuf, 16 );
                break;
            case SYS_GAP:
                sprintf( sbuf, "GAPWK:%06ld.%03d",
                         ((ltime/100L)%999999L), log_unit );                            // 11-09-2007 13 BMG
                memcpy( pq[i].fname, sbuf, 16 );
                break;
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
                // Write header record
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
                break;

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
        if (debug) disp_msg("Process workfile type : SYS_LAB");             //SDH 22-June-2006
        pwPQRec = &lrtp[log_unit]->pq_sub1;                                 //SDH 22-June-2006
        pPQ = &pq[*pwPQRec];                                                //SDH 22-June-2006
        plFileNum = &lrtp[log_unit]->fnum1;                                 //SDH 22-June-2006
        lMinSize = 1;                                                       //SDH 22-June-2006
        break;                                                              //SDH 22-June-2006
    case SYS_GAP:                                                           //SDH 22-June-2006
        if (debug) disp_msg("Process workfile type : SYS_GAP");             //SDH 22-June-2006
        pwPQRec = &lrtp[log_unit]->pq_sub2;                                 //SDH 22-June-2006
        pPQ = &pq[*pwPQRec];                                                //SDH 22-June-2006
        plFileNum = &lrtp[log_unit]->fnum2;                                 //SDH 22-June-2006
        lMinSize = 32;                                                      //SDH 22-June-2006
        break;                                                              //SDH 22-June-2006
    default:                                                                //SDH 22-June-2006
        if (debug) {                                                        //SDH 22-June-2006
            sprintf(msg, "ERROR: Unknown workfile type: %d", type);         //SDH 22-June-2006
            disp_msg(msg);                                                  //SDH 22-June-2006
        }                                                                   //SDH 22-June-2006
        return RC_SERIOUS_ERR;                                              //SDH 22-June-2006
    }                                                                       //SDH 22-June-2006

    //Skip processing if there is no file                                   //SDH 22-June-2006
    if (*plFileNum == 0) return RC_OK;                                      //SDH 22-June-2006

    // Close workfile                                                       //SDH 22-June-2006
    s_close (0, *plFileNum);                                                //SDH 22-June-2006
    log_file_close (*plFileNum);                                            //SDH 22-June-2006
    
    // Set process queue entry to READY to enable processing of workfile
    // (unless file is emtpy, in which case delete it, and clear pq entry)
    memcpy( fnm, pPQ->fname, 18);                                           //SDH 22-June-2006
    *(fnm+18) = 0x00;
    fsz = filesize (fnm);
    if (fsz >= lMinSize) {                                                  //SDH 22-June-2006
        // Set process queue entry to READY                                 //SDH Bug fix 3-Oct-2006
        sprintf(msg, "Setting status of %s to READY", fnm);                 //SDH Bug fix 3-Oct-2006
        disp_msg(msg);                                                      //SDH Bug fix 3-Oct-2006
        pPQ->state = PST_READY;                                             //SDH 22-June-2006
    } else {
        sprintf(msg, "File is null, deleting %s", fnm);                     //SDH Bug fix 3-Oct-2006
        // Delete workfile                                                  //SDH Bug fix 3-Oct-2006
        s_delete(0, pPQ->fname);                                            //SDH 22-June-2006
        // Free-up process queue entry and remove link from active table
        pPQ->state = PST_FREE;                                              //SDH 22-June-2006
        pPQ->submitcnt = 0;                                                 // 11-09-2007 13 BMG
        pPQ->type = 0;                                                      // 11-09-2007 13 BMG
        pPQ->unit = 0;                                                      // 11-09-2007 13 BMG
        memset(pPQ->fname, 0x00, 18);                                       // 11-09-2007 13 BMG
        pPQ->last_access_time = 0;                                          // 11-09-2007 13 BMG
    }
    *plFileNum = 0L;                                                        //SDH 22-June-2006
    *pwPQRec = 0;                                                           //SDH 22-June-2006

    return RC_OK;

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
    rc = s_lookup( T_PIPE, 0,
                   (type==SYS_GAP)?SEM_47_RUNNING:SEM_LAB_RUNNING,
                   (void *)&pt, PIPESIZE, PIPESIZE, 0L );
//   if (debug) {
//      sprintf( msg, "checking : [%s] RC:%08lX\n",
//               (type==SYS_GAP)?SEM_47_RUNNING:SEM_LAB_RUNNING, rc );
//      disp_msg(msg);
//   }
    if (rc<0L) {
        // log_event101(lrtlg.fnum, 0, __LINE__);
        if (debug) {
            sprintf( msg, "Err-O pipe %s. RC:%08lX",
                     (type==SYS_GAP)?SEM_47_RUNNING:SEM_LAB_RUNNING,
                     rc );
            disp_msg(msg);
        }
    }

    return(rc>0);

}


URC pllol_get_next( /*WORD log_unit,*/ BYTE *list_id, LRT_PLL *pllp )
{
    UBYTE repeat;
    LONG rc, rec;
    URC found, usrrc;
    EALAUTH_REC afrec = {0};
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
        // Look up user name
        usrrc = open_af();
        if (usrrc<RC_IGNORE_ERR) {
            return usrrc;
        }
        memset( afrec.operator_no, 0x00, 4 );
        pack( afrec.operator_no+2, 2, pllolrec.creator, 3, 1 );
        if (debug) {
            disp_msg("RD EALAUTH :");
            dump((BYTE *)&afrec, 4);
        }
        rc = s_read(0, af.fnum, (void *)&afrec, EALAUTH_RECL, EALAUTH_KEYL);
        close_af( CL_SESSION );
        if (rc<=0L) {
            af.present=FALSE;
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                // User not on EALAUTH
                if (debug) {
                    sprintf(msg, "NOF");
                    disp_msg(msg);
                }
                strncpy( afrec.operator_name, "* UNKNOWN *    ", 15);
            } else {
                log_event101(rc, EALAUTH_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R EALAUTH. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }
        } else {
            af.present=TRUE;
            if (debug) disp_msg("OK");
        }

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
        pllp->cListStatus = pllolrec.cLocation;                             // SDH 12-01-2005 OSSR WAN
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
    if (strncmp( enqbuf.sel_desc, "X ", 2 )!=0) {
        memcpy( plip->sel_desc, enqbuf.sel_desc, 45 );
    } else {
        memset( txtbuf, 0x20, 45 );
        //txtlth = format_text( enqbuf.item_desc, 24, txtbuf, 45, 15 );
        format_text( enqbuf.item_desc, 24, txtbuf, 45, 15 );
        translate_text( txtbuf, 45 );                                       // v4.0
        memcpy( plip->sel_desc, txtbuf, 45 );
    }
    memcpy( plip->item_code, enqbuf.item_code, 13 );
    
    //Return found                                                          // 21-12-04 SDH
    return 1;                                                               // 21-12-04 SDH

}

URC clolf_get_next( /*WORD log_unit,*/ BYTE *list_id, LRT_CLL *cllp )
{
    UBYTE repeat;
    LONG rc, os, rec;
    URC found;
    CLOLF_REC clolfrec;
    BYTE sbuf[64];

    rec = satol( list_id, 3 ) - 1;
    do {
        repeat = FALSE;
        os = rec * CLOLF_RECL;
        if (debug) {
            disp_msg("RD CLOLF");
        }
        rc = s_read( A_BOFOFF, clolf.fnum, (void *)&clolfrec, CLOLF_RECL, os );
        if (rc<=0L) {
            clolf.present=FALSE;
            if ((rc&0xFFFF)==0x4003) {
                if (debug) {
                    sprintf( msg, "Attempted to read beyond EOF on CLOLF. O/S=%ld",
                             os );
                    disp_msg(msg);
                }
                found = 0;
            } else {
                log_event101(rc, CLOLF_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R CLOLF. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }
        } else {
            // Record found
            clolf.present=TRUE;
            // Ignore lists with following status:
            //    [C]ompleted
            //    [P]rocessing in progress
            if (*clolfrec.list_status == 'C' ||
                *clolfrec.list_status == 'P') {
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
        memcpy( cllp->list_id, clolfrec.list_id, 3 );
        sprintf( sbuf, "%03d", rec );
        memcpy( cllp->seq, sbuf, 3 );
        memcpy( cllp->num_items, clolfrec.total_items, 3 );
        memcpy( cllp->items_shopfloor, clolfrec.items_shopfloor, 3 );
        memcpy( cllp->items_backshop, clolfrec.items_backshop, 3 );
        memcpy( cllp->list_type, clolfrec.list_type, 1 );
        memcpy( cllp->bus_unit_name, clolfrec.bus_unit_name, 15 );
        memcpy( cllp->active, clolfrec.list_status, 1 );
        memcpy( cllp->abItemsOssr, clolfrec.items_ossr,                     //SDH 19-01-2005 OSSR WAN
                sizeof(cllp->abItemsOssr));                                 //SDH 19-01-2005 OSSR WAN
    }

    return found;
}

URC clilf_get_next( /*WORD log_unit,*/ BYTE *list_id, BYTE *seq, LRT_CLI *clip )
{
    UBYTE repeat;
    //WORD txtlth;
    LONG rc, tseq;
    URC found;
    CLILF_REC clilfrec = {0};
    BYTE sbuf[64], txtbuf[256];

    tseq = satol( seq, 3 );
    do {
        repeat = FALSE;
        if (debug) disp_msg("RD CLILF :");
        memcpy( clilfrec.list_id, list_id, 3 );
        sprintf( sbuf, "%03ld", tseq );
        memcpy( clilfrec.seq, sbuf, 3 );
        if (debug) {
            disp_msg("RD CLILF :");
            dump( clilfrec.list_id, 6 );
        }
        rc = s_read( 0, clilf.fnum, (void *)&clilfrec,
                     CLILF_RECL, CLILF_KEYL );
        if (rc<=0L) {
            clilf.present=FALSE;
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                if (debug) disp_msg("Record not found on CLILF");
                found = 0;
            } else {
                log_event101(rc, CLILF_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Err-R CLILF. RC:%08lX", rc);
                    disp_msg(msg);
                }
                return RC_DATA_ERR;
            }
        } else {
            // Record found
            if (debug) disp_msg("OK");
            clilf.present=TRUE;
//-      if ( correct list ) {      // Use this code to make record
            // selection conditional
            found = 1;
//-      } else {
//-         found = 0;
//-         repeat = TRUE;
//-      }
        }
        tseq++;
    } while (repeat);

    if (found==1) {

        // Read IDF
        memset( idfrec.boots_code, 0x00, 4);
        pack( idfrec.boots_code, 4, clilfrec.boots_code, 7, 1 );
        rc = ReadIdf(__LINE__);
        if (rc<=0L) {
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
                idf.present = FALSE;
            } else {
                return RC_DATA_ERR;
            }
        } else {
            idf.present = TRUE;
        }

        //Open, read, close PGF
        memcpy(pgfrec.prod_grp_no, idfrec.product_grp,
               sizeof(pgfrec.prod_grp_no));
        open_pgf();
        rc = ReadPgf(__LINE__);
        close_pgf(CL_SESSION);
        if (rc <= 0L) pgfrec.cOssrFlag = 'N';
        UBYTE ubPgfOssrFlag = (pgfrec.cOssrFlag == 'Y' ? TRUE : FALSE);    // SDH 18-03-2005 OSSR WAN

        //Read RFHIST
        memcpy(rfhistrec.boots_code, idfrec.boots_code,
               sizeof(rfhistrec.boots_code));
        rc = ReadRfhist(__LINE__);
        if (rc <= 0L) {
            rfhistrec.ubItemOssrFlag = ubPgfOssrFlag;                       // SDH 18-03-2005 OSSR WAN
            rfhistrec.ubPgfOssrFlag = ubPgfOssrFlag;                        // SDH 18-03-2005 OSSR WAN
        }
         
        //Set RFHIST OSSR status to PGF value if PGF changed                // SDH 18-03-2005 OSSR WAN
        //Ignore errors                                                     // SDH 18-03-2005 OSSR WAN
        if (ubPgfOssrFlag != rfhistrec.ubPgfOssrFlag) {                     // SDH 18-03-2005 OSSR WAN
            if (debug) disp_msg("Mismatched OSSR prod group... resetting"); // SDH 18-03-2005 OSSR WAN
            rfhistrec.ubPgfOssrFlag = ubPgfOssrFlag;                        // SDH 18-03-2005 OSSR WAN
            rfhistrec.ubItemOssrFlag = ubPgfOssrFlag;                       // SDH 18-03-2005 OSSR WAN
            WriteRfhist(__LINE__);                                          // SDH 18-03-2005 OSSR WAN
        }                                                                   // SDH 18-03-2005 OSSR WAN

        // move clilfrec into LRT_CLI struct
        memcpy( clip->list_id, clilfrec.list_id, 3 );
        memcpy( clip->seq, clilfrec.seq, 3 );
        memcpy( clip->boots_code, clilfrec.boots_code, 7 );
        if (idf.present) {                                                  //sw3.1
            unpack( clip->parent_code, 7, idfrec.parent_code, 4, 1 );       //sw3.1
            clip->status[0] = idfrec.status_1[0];
        } else {                                                            //sw3.1
            memcpy( clip->parent_code, "???????", 7 );
            clip->status[0] = '?';
        }                                                                   //sw3.1
        memcpy( clip->item_code, clilfrec.item_code, 13 );
        if ((strncmp( clilfrec.sel_desc, "X ", 2 )==0) & idf.present) {
            memset( txtbuf, 0x20, 45 );
            //txtlth = format_text( idfrec.stndrd_desc, 24, txtbuf, 45, 15 );
            format_text( idfrec.stndrd_desc, 24, txtbuf, 45, 15 );
            translate_text( txtbuf, 45 );                                   // v4.0
            memcpy( clip->sel_desc, txtbuf, 45 );
        } else {
            memcpy( clip->sel_desc, clilfrec.sel_desc, 45 );
        }
        memcpy( clip->active_deal_flag, clilfrec.active_deal_flag, 1 );
        memcpy( clip->product_group, clilfrec.product_group, 6 );
        memcpy( clip->count_backshop, clilfrec.count_backshop, 4 );
        memcpy( clip->count_shopfloor, clilfrec.count_shopfloor, 4 );
        memcpy(clip->abOSSRCount, clilfrec.abOSSRCount,                     // SDH 11-01-2005 OSSR WAN
               sizeof(clip->abOSSRCount));                                  // SDH 11-01-2005 OSSR WAN
        clip->cOssrItem = (rfhistrec.ubItemOssrFlag ? 'O':'N');             // SDH 18-03-2005 OSSR WAN

    }

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
    if (debug) disp_msg("Process Orphans");                                 // 11-09-2007 13 BMG

    //Destroy current pq queue                                              //SDH 20-12-2004
    //Set rec count to 0 to build from scratch                              //SDH 20-12-2004
    if (fAction) {                                                          //SDH 9-May-06
        for (i = 0; i < MAX_CONC_UNITS; i++) {                              //SDH 20-12-2004
            pq[i].state = PST_FREE;                                         //SDH 20-12-2004
            pq[i].submitcnt = 0;                                            // 11-09-2007 13 BMG
            pq[i].type = 0;                                                 // 11-09-2007 13 BMG
            pq[i].unit = 0;                                                 // 11-09-2007 13 BMG
            memset(pq[i].fname, 0x00, 18);                                  // 11-09-2007 13 BMG
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
            if (cPrefix != 'K' && cPrefix != 'G') continue;                         //SDH 20-12-2004

            //Have to test against logical LFN as this is what is stored in pq      // 11-09-2007 13 BMG
            if (cPrefix == 'K') {                                                   // 11-09-2007 13 BMG
                sprintf( sbuf, "CHKWK:");                                           // 11-09-2007 13 BMG
            } else {                                                                // 11-09-2007 13 BMG
                sprintf( sbuf, "GAPWK:");                                           // 11-09-2007 13 BMG
            }                                                                       // 11-09-2007 13 BMG
            
            memcpy(sbuf + 6, df[i].df_name + 2, sizeof(df[i].df_name) - 2);         // 11-09-2007 13 BMG

            if (!fAction) {                                                         //SDH 9-May-06
                                                                                    
                //If the file is flagged as free in the PQ table then something     //SDH 9-May-06
                //may have gone wrong.  Equally, PSS47 may currently be processing  //SDH 9-May-06
                //it.  Either way, adopt it, as if the file is finished by PSS47    //SDH 9-May-06
                //then it will be deleted, and then PSS47 will fall over next time  //SDH 9-May-06
                //and re-adoption will not take place.                              //SDH 9-May-06
                BOOLEAN fActiveInPQ = FALSE;                                        //SDH 9-May-06

                for (j = 0; j < MAX_CONC_UNITS; j++) {                              //SDH 9-May-06
                    if (memcmp(pq[j].fname, sbuf, 16) == 0) {                       //SDH 9-May-06 // 11-09-2007 13 BMG
                        if (pq[j].state != PST_FREE) fActiveInPQ = TRUE;            //SDH 9-May-06
                        break;                                                      //SDH 9-May-06
                    }                                                               //SDH 9-May-06
                }                                                                   //SDH 9-May-06
                if (fActiveInPQ) continue;                                          //SDH 9-May-06

            }                                                                       //SDH 9-May-06

            //If the size of the file is less than or equal to 20 bytes             //SDH 20-12-2004
            //then it's just a header so delete it                                  //SDH 20-12-2004
            //NOTE that we don't use rc here, as it controls the loop               //SDH 20-12-2004
            //and we don't want to corrupt it                                       //SDH 20-12-2004
            if (df[i].df_size <= 20L) {                                             //SDH 20-12-2004
                if (s_delete(0x2000, (void*)df[i].df_name) < 0 ) {                  //SDH 20-12-2004
                    restrict_file((void*)df[i].df_name);                            //SDH 20-12-2004
                    s_delete(0x2000, (void*)df[i].df_name);                         //SDH 20-12-2004
                }                                                                   //SDH 20-12-2004
                continue;                                                           //SDH 20-12-2004
            }                                                                       //SDH 20-12-2004


            if (debug) {  //TEMP BMG
                sprintf( msg, "BMG Looking for Free slot");
                disp_msg(msg);
            }
                                                                                    
            //Let's locate a free entry in the table for this orphan                //SDH 9-May-2006
            BOOLEAN fFoundFreeSlot = FALSE;                                         //SDH 9-May-2006
            for (j = 0; j < MAX_CONC_UNITS; j++) {                                  //SDH 9-May-2006
                
                //If this slot is free then adopt it                                //SDH 9-May-2006
                if (pq[j].state == PST_FREE) {                                      //SDH 9-May-2006

                    //Flag it                                                       //SDH 9-May-2006
                    fFoundFreeSlot = TRUE;                                          //SDH 9-May-2006
                    
                    if (debug) {                                                    //SDH 9-May-2006
                        sprintf(msg, "Adopting %32s as %16s", df[i].df_name, sbuf); //SDH 9-May-2006 // 11-09-2007 13 BMG
                        disp_msg(msg);                                              //SDH 9-May-2006
                    }                                                               //SDH 9-May-2006

                    //Record file and increment for next                            //SDH 9-May-2006
                    pq[j].state = PST_ADOPTED;                                      //SDH 9-May-2006
                    pq[i].submitcnt = 0;                                            //PAB 26-1-2007
                    pq[j].unit = 999;                                               //SDH 9-May-2006
                    memcpy(pq[j].fname, sbuf, 32);                                  //SDH 9-May-2006 // 11-09-2007 13 BMG
                    cPrefix = df[i].df_name[1];                                                      // 11-09-2007 13 BMG
                    pq[j].type = (cPrefix == 'K' ? SYS_LAB : SYS_GAP);              //SDH 9-May-2006
                    pq[j].last_access_time = 0;                                                      // 11-09-2007 13 BMG
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

    if (debug) disp_msg("Orphan adoption Complete");                        //SDH 22-June-2006 // 11-09-2007 13 BMG

}

// spool GAPBF wrk file from screen application for OSSR                       13-9-04 PAB
void spool_workfile() {                                                     // 13-9-04 PAB

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
    
    for (i=0; i<MAX_CONC_UNITS && found==-1; i++) {                         // 13-9-04 PAB
        
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
            // Generate a unique filename                                   // 13-9-04 PAB
            sprintf(sbuf, "GAPWK:%06ld.%03d", ((time/100L)%999999L), log_unit);// 13-9-04 PAB
            memcpy( pq[i].fname, sbuf, 16 );                                // 13-9-04 PAB
            pq[i].type = type;                                              // 13-9-04 PAB
            rc = s_rename(0, WOGAPBF, sbuf);                                // 13-9-04 PAB
            if (rc<0) {                                                     // 13-9-04 PAB
                // mark as unused/processed                                 // 13-9-04 PAB
                pq[i].state=PST_FREE;                                       // 13-9-04 PAB
                pq[i].submitcnt = 0;                                        // 11-09-2007 13 BMG
                pq[i].type = 0;                                             // 11-09-2007 13 BMG
                pq[i].unit = 0;                                             // 11-09-2007 13 BMG
                memset(pq[i].fname, 0x00, 18);                              // 11-09-2007 13 BMG
                pq[i].last_access_time = 0;                                 // 11-09-2007 13 BMG
                if (debug) {                                                // 13-9-04 PAB
                    sprintf( msg, "Rename GAPBF failed - SPOOL ABORTED" );  // 13-9-04 PAB
                    disp_msg(msg);                                          // 13-9-04 PAB
                }                                                           // 13-9-04 PAB
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
WORD check_command()
{
    static LONG emask_t = 0L, emask_p = 0L;
    static BYTE cbuf[32];
    //WORD i, p;
    LONG emask, event, rc;
    UWORD hh_unitx;                   
    LONG cls_loop;
    URC usrrc;

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
                PINFO ProcInfo;                                             // 14-04-05 SDH
                memset(&ProcInfo, 0, sizeof(ProcInfo));                     // 14-04-05 SDH
                memcpy(ProcInfo.pi_pname, "TRANSACT  ", 10);                // 14-04-05 SDH
                ProcInfo.pi_prior = 200;                                    // 14-04-05 SDH
                if (debug) disp_msg("Chaining application...");             // 14-04-05 SDH
                background_msg("Chaining application...");                  // 14-04-05 SDH
                ShutDownAllSockets();                                       // 14-04-05 SDH
                CloseAllFiles();                                            // 14-04-05 SDH

                s_close(0, cpipe.fnum);     //Close comms pipe              // 14-04-05 SDH
                close_rfscf( CL_ALL );                                      // 09-01-07 PAB
                s_close( 0, lrtlg.fnum );                                   // 22-02-07 PAB
                s_close(0, dbg.fnum);                                       // 09-01-07 PAB
                s_timer(0, 2000);                                           // 14-04-05 SDH
                s_command(A_CHAIN, "ADX_UPGM:TRANSACT.286", "", 0,          // 14-04-05 SDH
                          &ProcInfo);                                       // 14-04-05 SDH
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

                if (debug) {                                                // 04-11-04 PAB
                    disp_msg("Finished close open reports");                // 04-11-04 PAB
                }                                                           // 04-11-04 PAB
                emask_p=0L;                                                 // Enable next pipe read event
                return 0;                                                   // 04-11-04 PAB
            }

            // Clean up application  
            if (strncmp(cbuf, "CLS", 3)==0) {

                disp_msg("Closing sessions");

                // Close all files
                CloseAllFiles();

                if (debug) disp_msg("(CLS) Flushing Orphan file handles");  // 20-05-04 PAB
                
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
                if (debug) disp_msg("Flushing Complete");                   // 20-05-04 PAB
                
                //close report files                                        // 20-04-04 PAB
                if (debug) disp_msg("Closing known open HHT sessions");     // 20-04-04 PAB
                
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
                usrrc = open_invok();
                if (usrrc<=RC_DATA_ERR) {
                    printf("ERROR - Unable to open INVOK file. Check appl. event logs\n");
                }
                rc = s_read( A_BOFOFF, invok.fnum,
                             (void *)&invokrec, INVOK_RECL, 0L );
                if (rc<=0L) {
                    log_event101(rc, INVOK_REP, __LINE__);
                    printf("Err-R INVOK. RC:%08lX", rc);
                }
                close_invok( CL_SESSION );

                // Invalidate all current sessions
                usrrc = dealloc_lrt_table( ALL_UNITS );
                sess = 0;

                // Rebuild the PQ table completely -- picking up any orphans and
                // flagging them for processing 
                process_orphans(TRUE);

                if (debug) disp_msg("CLS Complete");

            }

            // Cycle logs
            if (strncmp(cbuf, "CYC", 3)==0) {
               CycleLogs();                                                 // 11-09-2007 13 BMG
            }

            // Dump PQ (SEL stack)                                          // SDH 10-May-2006
            if (strncmp(cbuf, "DPQ", 3) == 0) {                             // SDH 10-May-2006
                dump_pq_stack();                                            // 11-09-2007 13 BMG
            }                                                               // SDH 10-May-2006

            // Report version
            if (strncmp(cbuf, "VER", 3)==0) {

                sprintf(msg, "%s", RFS_VER);
                disp_msg(msg);
                sprintf(msg, "DATE    : %s", RFS_DATE);
                disp_msg(msg);

            }

        }
        emask_p=0L;                                                         // Enable next pipe read event
    }

    return 0; 

}

void CloseAllFiles ( void )
{

    disp_msg("Close all files");

    close_pllol( CL_ALL );   
    close_plldb( CL_ALL );   
    close_isf( CL_ALL );     
    close_idf( CL_ALL );     
    close_irf( CL_ALL );     
    close_irfdex( CL_ALL );                                                 //SDH 14-01-2005 Promotions
    close_imstc( CL_ALL );   
    close_stock( CL_ALL );   
    close_cimf( CL_ALL );    
    close_citem( CL_ALL );   
    close_af( CL_ALL );      
    close_stkmq( CL_ALL );   
    close_imfp( CL_ALL );    
    close_rfrdesc( CL_ALL ); 
    close_tsf( CL_ALL );     
    close_psbt( CL_ALL );    
    close_wrf( CL_ALL );     
    close_minls( CL_ALL );   
    close_rfhist( CL_ALL );  
    close_invok( CL_ALL );   
    close_clolf( CL_ALL );   
    close_clilf( CL_ALL );   
    close_pilst( CL_ALL );   
    close_pgf( CL_ALL );     
    close_suspt( CL_ALL );                                                  // PAB 5-5-4
    close_prtlist( CL_ALL );                                                // PAB 17-05-04
    close_bcsmf( CL_ALL );                                                  // SDH 26-11-04 OSSR WAN
    close_cclol( CL_ALL );                                                  // SDH 26-11-04 OSSR WAN
    close_ccilf( CL_ALL );                                                  // SDH 26-11-04 OSSR WAN
    close_cchist( CL_ALL );                                                 // SDH 26-11-04 OSSR WAN
    close_ccdirsu( CL_ALL );                                                // SDH 26-11-04 OSSR WAN
    close_deal();                                                           // SDH 10-12-04 PROMOTIONS
    close_srpog(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    close_srmod(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    close_sritml(CL_ALL);                                                   // SDH 12-Oct-2006 Planners
    close_sritmp(CL_ALL);                                                   // SDH 12-Oct-2006 Planners
    close_srpogif(CL_ALL);                                                  // SDH 12-Oct-2006 Planners
    close_srpogil(CL_ALL);                                                  // SDH 12-Oct-2006 Planners
    close_srpogip(CL_ALL);                                                  // SDH 12-Oct-2006 Planners
    close_srcat(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    close_srsxf(CL_ALL);                                                    // SDH 12-Oct-2006 Planners
    close_recall( CL_ALL );                                                 // 11-09-2007 13 BMG
    close_rcindx ( CL_ALL );                                                // 11-09-2007 13 BMG
    close_rcspi ( CL_ALL) ;                                                 // 11-09-2007 13 BMG

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


DOUBLE emu_round( DOUBLE num ) {
    DOUBLE ab = abs(num);
    if (num - ab < 0.5) {
        return abs(num);
    } else {
        return abs(num) + 1;
    }
}

/*
//Create a new RFHIST record with default contents                          // SDH 17-11-04 OSSR WAN
//Only handles creates where the OSSR status is being overridden            // SDH 17-11-04 OSSR WAN
URC CreateRfhist(BYTE cUpdateOssritm,                                      // SDH 17-11-04 OSSR WAN
                 LONG lLineNumber) {                                        // SDH 17-11-04 OSSR WAN

    if (cUpdateOssritm == ' ') return RC_DATA_ERR;                         // SDH 17-11-04 OSSR WAN

    memset(rfhistrec.date_last_pchk, 0x00,                                  // SDH 17-11-04 OSSR WAN
           sizeof(rfhistrec.date_last_pchk));                               // SDH 17-11-04 OSSR WAN
    memset(rfhistrec.date_last_gap, 0x00,                                   // SDH 17-11-04 OSSR WAN
           sizeof(rfhistrec.date_last_gap));                                // SDH 17-11-04 OSSR WAN
    memset(rfhistrec.price_last_pchk, 0x00,                                 // SDH 17-11-04 OSSR WAN
           sizeof(rfhistrec.price_last_pchk));                              // SDH 17-11-04 OSSR WAN
    memset(rfhistrec.resrv, 0xFF,                                           // SDH 17-11-04 OSSR WAN
           sizeof(rfhistrec.resrv));                                        // SDH 17-11-04 OSSR WAN
    
    rfhistrec.ubItemOssrFlag = (cUpdateOssritm == 'O');                    // SDH 17-11-04 OSSR WAN

    if (WriteRfhist(lLineNumber) < 0) return RC_DATA_ERR;                   // SDH 17-11-04 OSSR WAN

}                                                                           // SDH 17-11-04 OSSR WAN
*/

//Essentially reads the RFHIST, but also updates the OSSR flag              // SDH 17-11-04 OSSR WAN
URC ProcessRfhist(BYTE* pbBootsCode,                                        // SDH 17-11-04 OSSR WAN
                  BYTE cUpdateOssritm,                                     // SDH 17-11-04 OSSR WAN
                  LONG lLineNumber) {                                       // SDH 17-11-04 OSSR WAN
    LONG rc;                                                                // SDH 17-11-04 OSSR WAN
    BOOLEAN fTemp;                                                          // SDH 18-03-05 OSSR WAN

    // Attempt read of RFHIST                                               // SDH 17-11-04 OSSR WAN
    memcpy(rfhistrec.boots_code, pbBootsCode,                               // SDH 17-11-04 OSSR WAN
           sizeof(rfhistrec.boots_code));                                   // SDH 17-11-04 OSSR WAN
    rc = ReadRfhist(lLineNumber);                                           // SDH 17-11-04 OSSR WAN
    
    //If success then update RFHIST if neccessary                           // SDH 17-11-04 OSSR WAN
    //WARNING: Possible that RFHIST has been                                // SDH 17-11-04 OSSR WAN
    //updated in the meantime, but not by TRANSACT                          // SDH 17-11-04 OSSR WAN
    if (rc > 0) {                                                           // SDH 17-11-04 OSSR WAN
        if (rfscfrec1and2.ossr_store != 'W') return RC_OK;                  // SDH 17-11-04 OSSR WAN
        fTemp = (cUpdateOssritm == 'O');                                   // SDH 17-11-04 OSSR WAN
        if ((cUpdateOssritm != ' ') &&                                     // SDH 17-11-04 OSSR WAN
            (fTemp != rfhistrec.ubItemOssrFlag)) {                          // SDH 17-11-04 OSSR WAN
            rfhistrec.ubItemOssrFlag = fTemp;                               // SDH 17-11-04 OSSR WAN
        }                                                                   // SDH 17-11-04 OSSR WAN
        if (WriteRfhist(__LINE__) <= 0) return RC_DATA_ERR;                 // SDH 17-11-04 OSSR WAN
        return RC_OK;                                                       // SDH 17-11-04 OSSR WAN
/*
    // If RFHIST record not found                                           // SDH 17-11-04 OSSR WAN
    } else if ((rc&0xFFFF) == 0x06C8 ||                                     // SDH 17-11-04 OSSR WAN
               (rc&0xFFFF) == 0x06CD) {                                     // SDH 17-11-04 OSSR WAN
        if (rfscfrec1and2.ossr_store != 'W') return RC_OK;                  // SDH 17-11-04 OSSR WAN
        if (cUpdateOssritm == ' ') return RC_OK;                           // SDH 17-11-04 OSSR WAN
        return CreateRfhist(cUpdateOssritm, __LINE__);                     // SDH 17-11-04 OSSR WAN
*/    
    //Else other error                                                      // SDH 17-11-04 OSSR WAN
    } else {                                                                // SDH 17-11-04 OSSR WAN
        return RC_DATA_ERR;                                                 // SDH 17-11-04 OSSR WAN
    }                                                                       // SDH 17-11-04 OSSR WAN

}                                                                           // SDH 17-11-04 OSSR WAN

//NOTE: This routine assumes that the caller has already read the PLLOL,    // 23-05-2005 SDH Excess
// and will write the PLLOL on return                                       // 23-05-2005 SDH Excess
URC SetListUnpicked(WORD wListId) {                                         // 23-05-2005 SDH Excess

    LONG lRc = 0;                                                           // 23-05-2005 SDH Excess
    WORD wSeqNum = 1;                                                       // 23-05-2005 SDH Excess
    WORD wHoles = 0;                                                        // 23-05-2005 SDH Excess
    WORD wItemCount = 0;                                                    // 23-05-2005 SDH Excess
    BYTE abListId[4];                                                       // 23-05-2005 SDH Excess
    BYTE abTemp[5];                                                         // 23-05-2005 SDH Excess

    if (debug) disp_msg("Entered SetListUnpicked");                         // 23-05-2005 SDH Excess
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

void buildSNR(LRT_SNR* pSNR, BYTE* pbPrtNum, BYTE* pbOpID, BYTE bAuth,      // 16-11-2004 SDH
              BYTE* pbUserName, BYTE cOssrStore, BYTE cPlannersActive) {    // 25-Sep-2006 SDH Planners

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
    pSNR->cPlannersActive = (cPlannersActive == 'Y') ? 'Y':'N';             // 25-Sep-2006 SDH

}                                                                           // 16-11-2004 SDH

LONG UpdateRfscf(LONG lLineNumber) {                                        // 16-11-2004 SDH

    LONG rc;                                                                // 16-11-2004 SDH
    WORD wRetries = 1;                                                      // 16-11-2004 SDH

    if (debug) {                                                            // 16-11-2004 SDH
        sprintf( msg, "WR RFSCF : (rec : 0)" );                             // 16-11-2004 SDH
        disp_msg(msg);                                                      // 16-11-2004 SDH
        dump((BYTE *)&rfscfrec1and2, RFSCF_RECL);                           // 16-11-2004 SDH
    }                                                                       // 16-11-2004 SDH

    // if error then reopen and try again before nak the handheld,          // 27-10-2004 PAB
    while (wRetries > 0) {                                                  // 16-11-2004 SDH

        rc = s_write(A_BOFOFF, rfscf.fnum, (void *)&rfscfrec1and2,          // 16-11-2004 SDH
                     RFSCF_RECL, 0L);                                       // 16-11-2004 SDH

        if (rc > 0L) break;                                                 // 16-11-2004 SDH

        if (wRetries > 0) {                                                 // 16-11-2004 SDH
            open_rfscf();                                                   // 16-11-2004 SDH
            wRetries--;                                                     // 16-11-2004 SDH
        }                                                                   // 16-11-2004 SDH
    }                                                                       // 16-11-2004 SDH

    if (rc <= 0L) {                                                         // 16-11-2004 SDH
        log_event101(rc, RFSCF_REP, lLineNumber);                 // 16-11-2004 SDH
        if (debug) {                                                        // 16-11-2004 SDH
            sprintf(msg, "Err-W RFSCF. RC:%08lX", rc);                      // 16-11-2004 SDH
            disp_msg(msg);                                                  // 16-11-2004 SDH
        }                                                                   // 16-11-2004 SDH
    }                                                                       // 16-11-2004 SDH

    return rc;                                                              // 16-11-2004 SDH

}                                                                           // 16-11-2004 SDH

