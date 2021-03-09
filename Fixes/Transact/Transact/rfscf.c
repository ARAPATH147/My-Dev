#include "transact.h"

#include "osfunc.h"
#include "trxfile.h"
#include "trxutil.h"
#include "rfscf.h"

FILE_CTRL rfscf;
RFSCF_REC_1AND2 rfscfrec1and2;
RFSCF_REC_3 rfscfrec3;

void RfscfSet(void) {
    rfscf.sessions   = 0;
    rfscf.fnum       = -1L;
    rfscf.pbFileName = "RFSCF";
    rfscf.wOpenFlags = 0x201C;
    rfscf.wReportNum = 517;
    rfscf.wRecLth    = 80L;
    rfscf.wKeyLth    = 0;
    rfscf.pBuffer    = &rfscfrec1and2;
}

URC RfscfOpen(void) {
    return direct_open(&rfscf, TRUE);
}

URC RfscfClose(WORD type) {
    return close_file(type, &rfscf);
}

LONG RfscfRead(LONG lRecNum, LONG lLineNum) {
    if (lRecNum == 0L) {
        rfscf.pBuffer = &rfscfrec1and2;
    } else if (lRecNum == 1L) {
        rfscf.pBuffer = &rfscfrec1and2 + rfscf.wRecLth;
    } else if (lRecNum == 2L) {
        rfscf.pBuffer = &rfscfrec3;
    }
    return ReadDirect(&rfscf, lRecNum, lLineNum, LOG_ALL);
}

LONG RfscfUpdate(LONG lLineNumber) {                                        // 16-11-2004 SDH

    LONG rc;                                                                // 16-11-2004 SDH
    WORD wRetries = 1;                                                      // 16-11-2004 SDH

    sprintf( msg, "WR RFSCF : (rec : 0)" );                                 // 16-11-2004 SDH
    disp_msg(msg);                                                          // 16-11-2004 SDH
    dump((BYTE *)&rfscfrec1and2, rfscf.wRecLth);                            // 16-11-2004 SDH

    // if error then reopen and try again before nak the handheld,          // 27-10-2004 PAB
    while (wRetries > 0) {                                                  // 16-11-2004 SDH

        rc = s_write(A_BOFOFF, rfscf.fnum, (void *)&rfscfrec1and2,          // 16-11-2004 SDH
                     rfscf.wRecLth, 0L);                                    // 16-11-2004 SDH

        if (rc > 0L) break;                                                 // 16-11-2004 SDH

        if (wRetries > 0) {                                                 // 16-11-2004 SDH
            RfscfOpen();                                                    // 16-11-2004 SDH
            wRetries--;                                                     // 16-11-2004 SDH
        }                                                                   // 16-11-2004 SDH
    }                                                                       // 16-11-2004 SDH

    if (rc <= 0L) {                                                         // 16-11-2004 SDH
        log_event101(rc, rfscf.wReportNum, lLineNumber);                    // 16-11-2004 SDH
        sprintf(msg, "Err-W RFSCF. RC:%08lX", rc);                          // 16-11-2004 SDH
        disp_msg(msg);                                                      // 16-11-2004 SDH
    }                                                                       // 16-11-2004 SDH

    return rc;                                                              // 16-11-2004 SDH

}                                                                           // 16-11-2004 SDH


