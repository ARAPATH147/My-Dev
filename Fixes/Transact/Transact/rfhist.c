#include "transact.h"

#include "trxfile.h"
#include "rfhist.h"
#include "rfscf.h"
#include <string.h>

FILE_CTRL rfhist;
RFHIST_REC rfhistrec;

void RfhistSet(void) {
    rfhist.sessions   = 0;
    rfhist.fnum       = -1L;
    rfhist.pbFileName = "RFHIST";
    rfhist.wOpenFlags = 0x201C;
    rfhist.wReportNum = 7;
    rfhist.wRecLth    = 18L;
    rfhist.wKeyLth    = 4;
    rfhist.pBuffer    = &rfhistrec;
}

URC RfhistOpen(void) {
    return keyed_open(&rfhist, TRUE);
}

URC RfhistClose(WORD type) {
    return close_file(type, &rfhist);
}

LONG RfhistRead(LONG lLineNum) {
    return ReadKeyed(&rfhist, lLineNum, LOG_CRITICAL);
}

LONG RfhistWrite(LONG lLineNumber) {
    return WriteKeyed(&rfhist, lLineNumber, LOG_ALL);
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

//Essentially reads the RFHIST, but also updates the OSSR flag              // SDH 17-11-04 OSSR WAN
URC ProcessRfhist(BYTE* pbBootsCode,                                        // SDH 17-11-04 OSSR WAN
                  BYTE cUpdateOssritm,                                     // SDH 17-11-04 OSSR WAN
                  LONG lLineNumber) {                                       // SDH 17-11-04 OSSR WAN
    LONG rc;                                                                // SDH 17-11-04 OSSR WAN
    BOOLEAN fTemp;                                                          // SDH 18-03-05 OSSR WAN

    // Attempt read of RFHIST                                               // SDH 17-11-04 OSSR WAN
    memcpy(rfhistrec.boots_code, pbBootsCode,                               // SDH 17-11-04 OSSR WAN
           sizeof(rfhistrec.boots_code));                                   // SDH 17-11-04 OSSR WAN
    rc = RfhistRead(lLineNumber);                                           // SDH 17-11-04 OSSR WAN

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
        if (RfhistWrite(__LINE__) <= 0) return RC_DATA_ERR;                 // SDH 17-11-04 OSSR WAN
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

    if (RfhistWrite(lLineNumber) < 0) return RC_DATA_ERR;                   // SDH 17-11-04 OSSR WAN

}                                                                           // SDH 17-11-04 OSSR WAN
*/

