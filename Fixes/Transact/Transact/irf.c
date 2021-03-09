// ------------------------------------------------------------------------
// Version 1.1              Brian Greenfield            1st Dec 2009
//    Changes to account for price embeeded barcodes.
// ------------------------------------------------------------------------

#include "transact.h"

#include <string.h>  /*1.1 BMG 01-12-2009*/

#include "trxfile.h"
#include "trxutil.h" /*1.1 BMG 01-12-2009*/
#include "irf.h"

FILE_CTRL irf;
FILE_CTRL irfdex;
IRF_REC irfrec;
IRFDEX_REC irfdexrec;

void IrfSet(void) {
    irf.sessions   = 0;
    irf.fnum       = -1L;
    irf.pbFileName = "IRF";
    irf.wOpenFlags = 0x2018;
    irf.wReportNum = 7;
    irf.wRecLth    = 50L;
    irf.wKeyLth    = 11;
    irf.pBuffer    = &irfrec;
}

URC IrfOpen(void) {
    return keyed_open(&irf, TRUE);
}

URC IrfClose(WORD type) {
    return close_file(type, &irf);
}

LONG IrfRead(LONG lLineNum) {
    LONG lRC;                                                                               /*1.1 BMG 01-12-2009*/
    BYTE sbuf[10];                                                                          /*1.1 BMG 01-12-2009*/
    BYTE price[10];                                                                         /*1.1 BMG 01-12-2009*/
    BYTE bar_code_unp[22];                                                                  /*1.1 BMG 01-12-2009*/
    BOOLEAN retry = FALSE;                                                                  /*1.1 BMG 01-12-2009*/

    lRC = ReadKeyed(&irf, lLineNum, LOG_CRITICAL);                                          /*1.1 BMG 01-12-2009*/
    if ((lRC&0xFFFF)==0x06C8 || (lRC&0xFFFF)==0x06CD) {                                     /*1.1 BMG 01-12-2009*/
        // Record not found so check it for price-embedded record                           /*1.1 BMG 01-12-2009*/
        unpack( bar_code_unp, 22, irfrec.bar_code, 11, 0 );                                 /*1.1 BMG 01-12-2009*/
        memset(price, '0', 10);                                                             /*1.1 BMG 01-12-2009*/
        if (memcmp(bar_code_unp+10, "2", 1) == 0) {                                         /*1.1 BMG 01-12-2009*/
            // Found a "2" prefixed barcode                                                 /*1.1 BMG 01-12-2009*/
            retry = TRUE;                                                                   /*1.1 BMG 01-12-2009*/
            //Extract price from barcode and put zero's in barcode where the price was      /*1.1 BMG 01-12-2009*/
            memcpy(price+6, bar_code_unp+18, 4);                                            /*1.1 BMG 01-12-2009*/
            memset(bar_code_unp+18, '0', 4);                                                /*1.1 BMG 01-12-2009*/
        } else {                                                                            /*1.1 BMG 01-12-2009*/
            memcpy(sbuf, "02", 2);                                                          /*1.1 BMG 01-12-2009*/
            if (memcmp(bar_code_unp+10, sbuf, 2) == 0) {                                    /*1.1 BMG 01-12-2009*/
                // Found an "02" prefixed barcode                                           /*1.1 BMG 01-12-2009*/
                retry = TRUE;                                                               /*1.1 BMG 01-12-2009*/
                //Extract price from barcode and put zero's in barcode where the price was  /*1.1 BMG 01-12-2009*/
                memcpy(price+5, bar_code_unp+17, 5);                                        /*1.1 BMG 01-12-2009*/
                memset(bar_code_unp+17, '0', 5);                                            /*1.1 BMG 01-12-2009*/
            }                                                                               /*1.1 BMG 01-12-2009*/
        }                                                                                   /*1.1 BMG 01-12-2009*/
        if (retry) {                                                                        /*1.1 BMG 01-12-2009*/
            pack(irfrec.bar_code, 11, bar_code_unp, 22, 0);                                 /*1.1 BMG 01-12-2009*/
            lRC = ReadKeyed(&irf, lLineNum, LOG_CRITICAL);                                  /*1.1 BMG 01-12-2009*/
            if (lRC>0) {                                                                    /*1.1 BMG 01-12-2009*/
                //Set price from stored price                                               /*1.1 BMG 01-12-2009*/
                pack(irfrec.salepric, 5, price, 10, 0);                                     /*1.1 BMG 01-12-2009*/
            }                                                                               /*1.1 BMG 01-12-2009*/
        }                                                                                   /*1.1 BMG 01-12-2009*/
    }                                                                                       /*1.1 BMG 01-12-2009*/

    return lRC;                                                                             /*1.1 BMG 01-12-2009*/
}

void IrfdexSet(void) {
    irfdex.sessions   = 0;
    irfdex.fnum       = -1L;
    irfdex.pbFileName = "IRFDEX";
    irfdex.wOpenFlags = 0x2018;
    irfdex.wReportNum = 673;
    irfdex.wRecLth    = 84L;                // 18-05-2009 BMG Increased from 17 to 84
    irfdex.wKeyLth    = 3;
    irfdex.pBuffer    = &irfdexrec;
}

URC IrfdexOpen(void) {
    return keyed_open(&irfdex, TRUE);
}

URC IrfdexClose(WORD type) {
    return close_file(type, &irfdex);
}

LONG IrfdexRead(LONG lLineNum) {
    return ReadKeyed(&irfdex, lLineNum, LOG_ALL);
}

