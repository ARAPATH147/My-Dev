//*************************************************************************
//
// File    : DALFILES.C
// Author  : Visakha Satya
// Created : 20th April 2015
//
// Overview: File functions for WHUOD, WHINDX and INVCE files.
//
//-------------------------------------------------------------------------
// Version A: Visakha Satya                                 20th Apr 2015
// SC079 Dallas Positive Receiving
//            Initial version
//
// Version B: Charles Skadorwa                               3rd Jun 2015
// SC079 Dallas Positive Receiving (F431)
//     - change to suppress non-critical READ errors being written to
//       the Application Event Log by changing the log status from
//       LOG_ALL to LOG_CRITICAL.
//
//************************************************************************/

#include "transact.h"

#include "DALfiles.h"
#include "osfunc.h"
#include "trxfile.h"

FILE_CTRL whuod, whindx, invce;

// ------------------------------------------------------------------------
// WHUOD File
// ------------------------------------------------------------------------

WHUOD_REC   whuodrec;

//Initialise file structures
void WhuodSet(void)
{
    whuod.sessions   = 0;
    whuod.fnum       = -1L;
    whuod.pbFileName = "WHUOD";
    whuod.wOpenFlags = A_READ | A_SHARE | A_WRITE;
    whuod.wReportNum = 890;
    whuod.pBuffer    = &whuodrec;
    whuod.wRecLth    = 66L;
    whuod.wKeyLth    = 4L;
}

URC OpenWhuod(void)
{
    return keyed_open(&whuod, TRUE);
}

URC CloseWhuod(WORD type)
{
    return close_file(type, &whuod);
}

LONG ReadWhuod(LONG lLineNumber)
{
    return ReadKeyed(&whuod, lLineNumber, LOG_CRITICAL);                //BCS
}

LONG WriteWhuod(LONG lLineNumber)
{
    return WriteKeyed(&whuod, lLineNumber, LOG_ALL);
}

// ------------------------------------------------------------------------
// WHINDX File
// ------------------------------------------------------------------------

WHINDX_REC whindxrec;

//Initialise file structures
void WhindxSet (void)
{
    whindx.sessions   = 0;
    whindx.fnum       = -1L;
    whindx.pbFileName = "WHINDX";
    whindx.wOpenFlags = A_READ | A_SHARE | A_WRITE;
    whindx.wReportNum = 891;
    whindx.pBuffer    = &whindxrec;
    whindx.wRecLth    = 21L;
}

URC OpenWhindx (void)
{
    return direct_open(&whindx, TRUE);
}

URC CloseWhindx (WORD type)
{
    return close_file(type, &whindx);
}

LONG ReadWhindx (LONG lRecNum, LONG lLineNum)
{
    return ReadDirect(&whindx, lRecNum, lLineNum, LOG_CRITICAL);
}

LONG WriteWhindx (LONG lRecNum, LONG lLineNum)
{
    return WriteDirect(&whindx, lRecNum, lLineNum, LOG_ALL);            //BCS
}

// ------------------------------------------------------------------------
// INVCE File
// ------------------------------------------------------------------------

INVCE_REC   invcerec;

//Initialise file structures
void InvceSet(void)
{
    invce.sessions   = 0;
    invce.fnum       = -1L;
    invce.pbFileName = "INVCE";
    invce.wOpenFlags = A_READ | A_SHARE | A_WRITE;
    invce.wReportNum = 82;
    invce.pBuffer    = &invcerec;
    invce.wRecLth    = 508L;
    invce.wKeyLth    = 9L;
}

URC OpenInvce(void)
{
    return keyed_open(&invce, TRUE);
}

URC CloseInvce(WORD type)
{
    return close_file(type, &invce);
}

LONG ReadInvce(LONG lLineNumber)
{
    return ReadKeyed(&invce, lLineNumber, LOG_CRITICAL);                //BCS
}

LONG WriteInvce(LONG lLineNumber)
{
    return WriteKeyed(&invce, lLineNumber, LOG_ALL);
}
