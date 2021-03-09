//*************************************************************************
//
// File    : TRXDAL.C
// Author  : Visakha Satya
// Created : 20th April 2015
//
// Overview: This module contains functions to process the new Dallas
//           Positive receiving process.
//
//-------------------------------------------------------------------------
// Version A: Visakha Satya                                 20th Apr 2015
// SC079 Dallas Positive Receiving
//            Initial version
//
// Version B: Charles Skadorwa //BCS                        11th Jun 2015
// SC079 Dallas Positive Receiving
//    Added the following to be included as part of Defect 1493:
//       - Increased size of stkmqrec from 128 bytes to 512 bytes in
//         order that the Txn Type 18 record can hold data for max of
//         17 items.
//       - Added tracing for STKMQ record if debug mode detected.
//
// Version C: Charles Skadorwa //CCS                        12th Jun 2015
// SC079 Dallas Positive Receiving
//    The following to be included as part of Defects 1496 & 1497:
//       - Initialised count to zero inside DAR_Request function.
//       - Initialised whuodrec.abRcrdTotItems and
//         whuodrec.abExpctDelDate in DAR_Request function.
//       - Set abSequenceNumber correctly in DAR_Request function.
//
// Version D: Charles Skadorwa //DCS                         1st Jul 2015
// SC079 Dallas Positive Receiving (F431)
//    The following to be included as part of Defect 1565: Stock Value
//    is getting updated twice when DALLAS positive receive is processed
//    in both controler and HHT batch device.
//       - Changes to function: DAR_Request() to check to see if the UOD
//         has NOT been RECEIPTED before booking it in. This is just in
//         case someone else has booked it in or the UOD has been
//         transmitted during the booking in process.
//       - Fix to remove references to strlen() which treats genuine
//         NULL data as end of string!
//       - Created new function: Process_Dallas_DAR_Msg() in order to
//         save duplicating logic in DAR_Request() function.
//
// Version E: Charles Skadorwa //ECS                         7th Jul 2015
// SC079 Dallas Positive Receiving (F431)
//    Change to Process_Dallas_DAR_Msg() function to add the
//    QtyExpected correctly to the STKMQ Type 18 record ie. it was
//    coded to output 2-digits instead of 4 (0000 - 9999).
//************************************************************************/

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
#include "trxDAL.h"
#include "DALfiles.h"

// ------------------------------------------------------------------------
// DAC - Dallas Positive Receiving Check Message
//
// INPUT:
// inbound = transaction request
//
// OUTPUT:
// inbound = transaction response
//
// RETURN VALUE: void
// ------------------------------------------------------------------------

void DAC_Request(void)
{
    LONG  whuod_rc,
          whindx_rc;

    //Initial checks
    if (IsStoreClosed())
    {
        return;
    }
    if (IsHandheldUnknown())
    {
        return;
    }

    UpdateActiveTime();

    whuod_rc  = filesize("WHUOD");
    whindx_rc = filesize("WHINDX");

    //Check for the presence of both WHUOD and WHINDX files.
    if ((whuod_rc != -1L) && (whindx_rc != -1L))
    {
       //Both WHUOD and WHINDX are present.
       //Store is in Dallas Positive Receiving.
       prep_ack("");
       if (debug)
       {
           disp_msg("Store is in Dallas Positive UOD Receiving");
       }
    }
    else if ((whuod_rc == -1L) && (whindx_rc == -1L))
    {
       //Both WHUOD and WHINDX are NOT present.
       //Store is NOT in Dallas Positive Receiving.
       prep_nak("");
       if (debug)
       {
           disp_msg("Store is not in Dallas Positive UOD Receiving");
       }
    }
    else if ((whuod_rc == -1L) && (whindx_rc != -1L))
    {
       //Store is in Dallas Positive Receiving.
       //WHINDX file exists but WHUOD file does NOT exist.
       prep_nak("ERROR WHUOD file not present");
       if (debug)
       {
           disp_msg("ERROR WHUOD file not present");
       }
    }
    else if ((whuod_rc != -1L) && (whindx_rc == -1L))
    {
       //Store is in Dallas Positive Receiving.
       //WHUOD file exists but WHINDX file does NOT exist.
       prep_nak("ERROR WHINDX file not present");
       if (debug)
       {
           disp_msg("ERROR WHINDX file not present");
       }
    }
}

// ------------------------------------------------------------------------
// DAL - Dallas UOD Load Message
// DAD - Dallas UOD Detail Message
// DAE - Dallas UOD EOF Message
//
// INPUT:
// inbound = transaction request
//
// OUTPUT:
// inbound = transaction response
//
// RETURN VALUE: void
// ------------------------------------------------------------------------

void DAL_Request(char *inbound)
{
    LRT_DAL* pDAL = (LRT_DAL*)inbound;
    URC urc;
    LONG lrc, rec, found;

    //Initial checks
    if (IsStoreClosed())
    {
        return;
    }
    if (IsHandheldUnknown())
    {
        return;
    }

    UpdateActiveTime();

    urc = OpenWhindx();
    if (urc)
    {
        prep_nak("ERROR Unable to open WHINDX.");
        return;
    }

    found = 0;
    rec = satol(pDAL->abNextRecordNo, sizeof(pDAL->abNextRecordNo));
    disp_msg("RD WHINDX");
    lrc = ReadWhindx(rec, __LINE__);
    if (lrc <= 0L)
    {
        if ((lrc&0xFFFF) == 0x4003)  // End Of File
        {
            found = 0;
        }
        else
        {
            prep_nak("ERROR Unable to read WHINDX");
            return;
        }
    }
    else
    {
        found = lrc;
    }

    if (found == 0)     //End of WHINDX file reached. Create DAE message
    {
        LRT_DAE* pDAE = (LRT_DAE*)out;
        memcpy(pDAE->abCmd, DAE_MSG, sizeof(DAE_MSG));
        out_lth = LRT_DAE_LTH;
    }
    else
    {
        LRT_DAD* pDAD = (LRT_DAD*)out;  // Create DAD message.
        rec++;
        memcpy(pDAD->abCmd, DAD_MSG, sizeof(DAD_MSG));
        WordToArray(pDAD->abNextRecordNo, 4, rec);
        memcpy(pDAD->abDallasBarcode, whindxrec.abBarcodeNo,
               sizeof(whindxrec.abBarcodeNo));
        memcpy(pDAD->abExpectedDelDate,whindxrec.abExpctDelDate,
               sizeof(whindxrec.abExpctDelDate));
        pDAD->bStatus = whindxrec.bStatus;
        out_lth = LRT_DAD_LTH;
    }

    CloseWhindx(CL_ALL);
}


// ------------------------------------------------------------------------
// Process_Dallas_DAR_Msg                                               //DCS
//
//     INPUT: LRT_DAR* pDAR
//
//    OUTPUT: none
//
// Called By: DAR_Request()
//
// RETURN VALUE: void
// ------------------------------------------------------------------------

void Process_Dallas_DAR_Msg( LRT_DAR* pDAR )
{

    BYTE abConfirmDate[3],
         stkmqrec[512];   // Allow space for max 17 items in Txn Type 18
                          // STKMQ record.

    WORD hour= 0,
         min = 0;


    LONG count    = 0,
         day      = 0,
         found    = 0,
         invCount = 0,
         lrc      = 0,
         month    = 0,
         offset   = 0,
         pos      = 0,
         rec      = 0,
         sec      = 0,
         year     = 0;

    if ( whuodrec.bStatus != RECEIPTED )
    {
        whuodrec.bStatus = RECEIPTED;
        memcpy(whuodrec.abScannedDate,pDAR->abUodScanDate,
                               sizeof(pDAR->abUodScanDate));

        // Update record in WHUOD file
        lrc = WriteWhuod(__LINE__);
        if (lrc <= 0L)
        {
            prep_nak("ERROR Unable to write into WHUOD");
            return;
        }

        while(found != 1)
        {
            lrc = ReadWhindx(rec, __LINE__);
            if (lrc <= 0L)
            {
                prep_nak("ERROR Unable to read WHINDX");
                return;
            }

            // Check if barcode obtained from WHINDX file
            // is same as the barcode obtained from HHT.
            if(memcmp(pDAR->abDallasBarcode,
                      whindxrec.abBarcodeNo,
                      sizeof(whindxrec.abBarcodeNo)) == 0)
            {
                whindxrec.bStatus = RECEIPTED;
                lrc = WriteWhindx(rec,__LINE__);
                if (lrc <= 0L)
                {
                    prep_nak("ERROR Unable to write into WHINDX");
                    whuodrec.bStatus = UNRECEIPTED;
                    memcpy(whuodrec.abScannedDate,"000000",
                            sizeof(whuodrec.abScannedDate));

                    // Update record in WHINDX file.
                    lrc = WriteWhuod(__LINE__);
                    if (lrc <= 0L)
                    {
                        prep_nak("ERROR Unable to write to WHUOD while"
                        "reverting, write error in WHINDX");
                        return;
                    }
                    return;
                }
                else
                {
                    found = 1; // Record found
                }
            }
            else
            {
                rec++;
            }
        }

        pos = 0;
        while(pos < 45)
        {
            // Preparing key for INVCE file.
            memcpy(invcerec.abRecKey,whuodrec.abPoNos + pos,
                                sizeof(invcerec.abRecKey));
            lrc = ReadInvce(__LINE__);
            if (lrc <= 0L)
            {
                if(memcmp(invcerec.abRecKey, "000000000",
                           sizeof(invcerec.abRecKey)) == 0)
                {
                    break;
                }
                else
                {
                    prep_nak("ERROR Unable to read INVCE");
                    return;
                }
            }

            // Prepare STKMQ Record Type 18.
            memset(stkmqrec, '\0', sizeof(stkmqrec));
            stkmqrec[0] = 0x22;
            stkmqrec[1] = 0x18;
            stkmqrec[2] = 0x3B;
            sysdate(&day, &month, &year, &hour, &min, &sec);
            sprintf(sbuf,"%02ld%02ld%02ld",year%100,month,day);
            pack( stkmqrec + 3, 3, sbuf, 6, 0 );
            pack(abConfirmDate, 3, sbuf, 6, 0);
            sprintf(sbuf,"%02d%02d%02d",hour,min,(WORD)sec);
            pack( stkmqrec + 6, 3, sbuf, 6, 0 );

            // As the data is copied byte by byte, sizeof()
            // function is not used in some cases of memcpy
            memcpy(stkmqrec + 9, invcerec.abRecKey, 1);
            memcpy(stkmqrec + 10, invcerec.abFolioYear,
                              sizeof(invcerec.abFolioYear));
            memcpy(stkmqrec + 11, invcerec.abRecKey+1, 1);
            memcpy(stkmqrec + 12, invcerec.abRecKey+2, 2);
            memcpy(stkmqrec + 14, invcerec.abRecKey+4, 1);
            memcpy(stkmqrec + 15, invcerec.abCount,
                                  sizeof(invcerec.abCount));
            memcpy(stkmqrec + 16, invcerec.abDate+2, 1);
            memcpy(stkmqrec + 17, invcerec.abExpDelDate,
                             sizeof(invcerec.abExpDelDate));
            stkmqrec[20] = invcerec.bDallasMkr;

            offset = 21;

            memset(sbuf, '\0', sizeof(sbuf));
            unpack(sbuf, 2, invcerec.abCount, 1, 0);
            invCount = satol(sbuf, 2);

            count = 0;

            while(count < invCount)
            {
                stkmqrec[offset + 0] = 0x3B;
                memcpy(stkmqrec + offset + 1,
                   invcerec.abDetailItem[count].abBootsCode,
                   sizeof(invcerec.abDetailItem[count].abBootsCode));
                stkmqrec[offset + 5] = 0x3B;

                sprintf( stkmqrec + offset + 6, "%04d",                 //ECS
                         invcerec.abDetailItem[count].QtyExpected);     //ECS

                //WordToArray(stkmqrec + offset + 6, 2,                 //ECS
                //  invcerec.abDetailItem[count].QtyExpected);          //ECS
                //stkmqrec[offset + 8] = 0x3B;                          //ECS
                stkmqrec[offset + 10] = 0x3B;                           //ECS

                //memcpy(stkmqrec + offset + 9,                         //ECS
                memcpy(stkmqrec + offset + 11,                          //ECS
                   invcerec.abDetailItem[count].abCsrMarker,
                   sizeof(invcerec.abDetailItem[count].abCsrMarker));

                //offset = offset + 10;                                 //ECS
                offset = offset + 12;                                   //ECS
                count++;
            }

            stkmqrec[offset]     = 0x3B;   // STKMQ Field  Delimiter
            stkmqrec[offset + 1] = 0x22;   // STKMQ Record Delimiter

            // Add a CR/LF character.
            stkmqrec[offset + 2] = 0x0d;
            stkmqrec[offset + 3] = 0x0a;

            offset = offset + 4;

            // Write Type 18 record to STKMQ file
            lrc = s_write(A_EOFOFF, stkmq.fnum, (void *)&stkmqrec,
                         offset, 0L );

            if (lrc <= 0L)
            {
                log_event101(lrc, STKMQ_REP, __LINE__);
                if (debug)
                {
                    sprintf(msg, "Write STKMQ ERROR. RC:%08lX. "
                                 "Record follows:" , lrc);
                    disp_msg(msg);
                    dump(stkmqrec, offset);
                }
                prep_nak("ERROR Unable to write to STKMQ");
                return;
            }

            if (debug)
            {
                disp_msg("Write STKMQ OK. Record follows:");
                dump(stkmqrec, offset);
            }


            invcerec.bConfirmFlag = AMENDED;
            memcpy(invcerec.abConfirmDate, abConfirmDate,
                                     sizeof(abConfirmDate));

            // Update record in INVCE file.
            lrc = WriteInvce(__LINE__);
            if (lrc <= 0L)
            {
                prep_nak("ERRORUnable to write to INVCE");
                return;
            }

            pos = pos + 9;
        }
    }
}


// ------------------------------------------------------------------------
// DAR - Dallas UOD Request Message
//
// INPUT:
// inbound = transaction request
//
// OUTPUT:
// inbound = transaction response
//
// RETURN VALUE: void
// ------------------------------------------------------------------------

void DAR_Request(char *inbound)
{
    LRT_DAR* pDAR = (LRT_DAR*)inbound;
    URC urc;
    BYTE abBarcode[6],
         abDalNo[3],
         abSeqNo[1],
         abSequenceNumber[2];

    LONG check    = 0,
         lrc      = 0,
         seqno    = 0;

    // Set sequence number as 0

    //Initial checks
    if (IsStoreClosed())
    {
        return;
    }
    if (IsHandheldUnknown())
    {
        return;
    }

    UpdateActiveTime();

    // Open WHUOD, WHINDX, INVCE and STKMQ files.
    urc = OpenWhuod();
    if (urc)
    {
        prep_nak("ERROR Unable to open WHUOD.");
        return;
    }

    urc = OpenWhindx();
    if (urc)
    {
        prep_nak("ERROR Unable to open WHINDX.");
        return;
    }

    urc = OpenInvce();
    if (urc)
    {
        prep_nak("ERROR Unable to open INVCE.");
        return;
    }

    urc = open_stkmq();
    if (urc)
    {
        prep_nak("ERROR Unable to open STKMQ.");
        return;
    }

    // If UOD Scan Status id Receipted.
    if(pDAR->bUodScanStatus == RECEIPTED)
    {
       // Preparing Barcode Number as part of key for WHUOD file
        memcpy(abBarcode, pDAR->abDallasBarcode + 8, sizeof(abBarcode));
        pack(abDalNo, 3, abBarcode, 6, 0);
        memcpy(whuodrec.abWhuodKey, abDalNo, sizeof(abDalNo));

        while(check == 0)
        {
            // Preparing Sequence Number as part of key for WHUOD file.
            WordToArray(abSequenceNumber,2, seqno);
            pack(abSeqNo, 1, abSequenceNumber, 2, 0);
            memcpy(whuodrec.abWhuodKey + 3, abSeqNo, sizeof(abSeqNo));

            lrc = ReadWhuod(__LINE__);
            if (lrc <= 0L)
            {
                if ((lrc&0xFFFF) == 0x06C8)   // Record not found
                {
                    check = 1;
                }
                else
                {
                    prep_nak("ERROR Unable to read WHUOD");
                    return;
                }
            }

            if (check == 0)
            {
                Process_Dallas_DAR_Msg( pDAR );                         //DCS
                seqno++;                                                //DCS
            }
        }
        prep_ack("");
    }

    // If UOD Scan Status is Banked.
    if(pDAR->bUodScanStatus == BANKED)
    {
        memcpy(abBarcode, pDAR->abDallasBarcode + 8, sizeof(abBarcode));
        pack(abDalNo, 3, abBarcode, 6, 0);

        WordToArray(abSequenceNumber,2, seqno);                         //CCS
        pack(abSeqNo, 1, abSequenceNumber, 2, 0);

        memcpy(whuodrec.abWhuodKey, abDalNo, sizeof(abDalNo));
        memcpy(whuodrec.abWhuodKey + 3, abSeqNo, sizeof(abSeqNo));
        lrc = ReadWhuod(__LINE__);
        if (lrc <= 0L)
        {
            blkfill(whuodrec.abPoNos, 0x30, 45);
            memcpy(whuodrec.abScannedDate,pDAR->abUodScanDate,
                                       sizeof(pDAR->abUodScanDate));
            whuodrec.bStatus = BANKED;

            memcpy(whuodrec.abRcrdTotItems, "0000",                     //CCS
                               sizeof(whuodrec.abRcrdTotItems));        //CCS

            memcpy(whuodrec.abExpctDelDate, "000000",                   //CCS
                               sizeof(whuodrec.abExpctDelDate));        //CCS

            lrc = WriteWhuod(__LINE__);
            if (lrc <= 0L)
            {
                prep_nak("ERROR Unable to write into WHUOD");
                return;
            }
            prep_ack("");
        }
        else
        {
            if (whuodrec.bStatus == BANKED)
            {
                prep_ack("");
                return;
            }
            else                                                        //DCS
            {                                                           //DCS
                //Preparing Barcode Number as part of key for WHUOD file//DCS
                memcpy(abBarcode, pDAR->abDallasBarcode + 8,            //DCS
                                                    sizeof(abBarcode)); //DCS
                pack(abDalNo, 3, abBarcode, 6, 0);                      //DCS
                memcpy(whuodrec.abWhuodKey, abDalNo, sizeof(abDalNo));  //DCS
                                                                        //DCS
                while(check == 0)                                       //DCS
                {                                                       //DCS
                    // Preparing Sequence Number as part of key for     //DCS
                    // WHUOD file.                                      //DCS
                    WordToArray(abSequenceNumber,2, seqno);             //DCS
                    pack(abSeqNo, 1, abSequenceNumber, 2, 0);           //DCS
                    memcpy(whuodrec.abWhuodKey + 3, abSeqNo,            //DCS
                                                     sizeof(abSeqNo));  //DCS
                                                                        //DCS
                    lrc = ReadWhuod(__LINE__);                          //DCS
                    if (lrc <= 0L)                                      //DCS
                    {                                                   //DCS
                        if ((lrc&0xFFFF) == 0x06C8) // Record not found //DCS
                        {                                               //DCS
                            check = 1;                                  //DCS
                        }                                               //DCS
                        else                                            //DCS
                        {                                               //DCS
                            prep_nak("ERROR Unable to read WHUOD");     //DCS
                            return;                                     //DCS
                        }                                               //DCS
                    }                                                   //DCS
                                                                        //DCS
                    if (check == 0)                                     //DCS
                    {                                                   //DCS
                        Process_Dallas_DAR_Msg( pDAR );                 //DCS
                        seqno++;                                        //DCS
                    }                                                   //DCS
                }                                                       //DCS
            }                                                           //DCS
            prep_ack("");                                               //DCS
        }
    }

    CloseWhuod(CL_ALL);
    CloseWhindx(CL_ALL);
    CloseInvce(CL_ALL);
    close_stkmq(CL_ALL);

}

