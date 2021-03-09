// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
// 
// Version 1.0               Paul Bowers       8th August 2007
// 
// MOdule TRANS02 reached 64K limit - split into next module.
// Entire module implemented for Mobile Printing.
//
// Version 1.1         Brian Greenfield        11th September 2007
// Added function unstall_sel_stack to set stack entries to ready.
// Added dump_pq_stack taken from rfs.c due to space.
// Added DealEnquiry taken from transact.c due to space.
// Added CycleLogs taken from rfs.c due to space.
//
// Version 1.2         Brian Greenfield        17th October 2007
// Fix for mobile printing.
//  
// Version 1.3         Paul Bowers              8th November 2007
// Changes to support painkiller warnings on the LPR transaction
// new function process painkiller and new file SELDESC.BIN  
//
// Version 1.4         Brian Greenfield         3rd January 2008
// Added WriteToFile function for WRF command.
//
// Version 1.5         Brian Greenfield         1st April 2008
// Moved process_sel_stakc into here from trans02.c.
// -----------------------------------------------------------------------

/* standard include files */
#include "transact.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <flexif.h>


#include "trans2.h"                                      
#include "file.h"  


// ----------------- transaction definitions ------------------------                     
#include "rfs.h"  
#include "rfs2.h" 
                
                 
// -----------------file definitions --------------------------------                     
#include "rfsfile.h"      // files used in all modules
#include "rfsfile2.h"     // files used only in TRANS02 and TRANS03
#include "rfsfile3.h"     // files used only in TRANS03


// ---------------- standard functions and globals -----------------------------
#include "dateconv.h"
#include "wrap.h"
#include "sockserv.h"
#include "rfglobal.h"                   
#include "rfglobal2.h" 
#include "rfglobal3.h"
#include "adxsrvfn.h"                   
#include "adxsrvst.h"  

static BYTE sbuf[64]; 
 
URC open_phf(void) {
    memset( (BYTE *)&phfrec,  0x00, sizeof(PHF_REC)  );
    phf.sessions   = 0;                                             
    phf.fnum       = -1L;                                           
    phf.pbFileName = PHF;                                        
    phf.wOpenFlags = PHF_OFLAGS;                                 
    phf.wReportNum = PHF_REP;                                    
    phf.pBuffer    = &phfrec;                                    
    phf.wRecLth    = PHF_RECL;                                   
    phf.wKeyLth    = PHF_KEYL;
    return keyed_open(&phf, TRUE);                                       
}    


URC close_phf(WORD type) {
    return close_file(type, &phf);                                       
}
 
URC open_iduf(void) {
    memset( (BYTE *)&idufrec,  0x00, sizeof(IDUF_REC)  );
    iduf.sessions   = 0;                                             
    iduf.fnum       = -1L;                                           
    iduf.pbFileName = IDUF;                                        
    iduf.wOpenFlags = IDUF_OFLAGS;                                 
    iduf.wReportNum = IDUF_REP;                                    
    iduf.pBuffer    = &idufrec;                                    
    iduf.wRecLth    = IDUF_RECL;                                   
    iduf.wKeyLth    = 0; 
    return direct_open(&iduf, TRUE);                                       
}    


URC close_iduf(WORD type) {
    return close_file(type, &iduf);                                       
}
 
//LONG ReadIduf(LONG lRecNum, LONG lLineNum) {                              // 14-11-07 PAB Mobile Printing 
//    return ReadDirect(&iduf, lRecNum, lLineNum, LOG_ALL);                 // 14-11-07 PAB Mobile Printing 
//} 

LONG ProcessPainkiller(void) {                                               // 08-11-07 PAB Mobile Printing
                                                                             // 08-11-07 PAB Mobile Printing
    LRT_LPR* pLPR = (LRT_LPR*)out;                                           // 08-11-07 PAB Mobile Printing
    LONG fnum;                                                               // 08-11-07 PAB Mobile Printing
    BYTE seldescrec[512];                                                    // 08-11-07 PAB Mobile Printing
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
                                                                            // 08-11-07 PAB Mobile Printing
    fnum=s_open(SELDESC_OFLAGS, SELDESC);                                   // 08-11-07 PAB Mobile Printing
    if (fnum<=0L) {                                                         // 08-11-07 PAB Mobile Printing
        if (debug) {                                                        // 08-11-07 PAB Mobile Printing
            sprintf(msg, "OPEN SELDESC Err %d ", fnum);                     // 08-11-07 PAB Mobile Printing
            disp_msg(msg);                                                  // 08-11-07 PAB Mobile Printing
        }                                                                   // 08-11-07 PAB Mobile Printing
        // file open error return spaces as the painkiller message          // 08-11-07 PAB Mobile Printing
        memset(pLPR->PainKillerMessage,0x20,                                // 08-11-07 PAB Mobile Printing
               sizeof(pLPR->PainKillerMessage));                            // 08-11-07 PAB Mobile Printing
        return -1;                                                          // 08-11-07 PAB Mobile Printing
    }                                                                       // 08-11-07 PAB Mobile Printing
    memset( (BYTE *)&seldescrec, 0x00, sizeof(SELDESC_REC) );               // 08-11-07 PAB Mobile Printing
    // read the entire file in one go                                       // 08-11-07 PAB Mobile Printing
    rc = s_read( A_BOFOFF,fnum,                                             // 08-11-07 PAB Mobile Printing
                    (void *)&seldescrec, SELDESC_RECL, 0L);                 // 08-11-07 PAB Mobile Printing 
                                                                            // 08-11-07 PAB Mobile Printing
    wIdx=1;                                                                 // 08-11-07 PAB Mobile Printing
    wPtr=0;
    wPtr2=0;                                                                // 08-11-07 PAB Mobile Printing
    while (rc >0L) {                                                        // 08-11-07 PAB Mobile Printing
       if (memcmp(&seldescrec[wPtr],&bEOL,1)==0) {                          // 08-11-07 PAB Mobile Printing
         wIdx++;  //ODOA found, increment record count                      // 08-11-07 PAB Mobile Printing
         wPtr++;  //skip over OA to point to start of next record           // 08-11-07 PAB Mobile Printing
         wPtr++;  //point to start of next record
       }  
       if (wIdx == wRecordNumber) {  // if record number matches required   // 08-11-07 PAB Mobile Printing
         memset(messagerec,0x20,sizeof(messagerec));                        // 08-11-07 PAB Mobile Printing
         // extract out the message record we want                          // 08-11-07 PAB Mobile Printing
         while(memcmp(&seldescrec[wPtr],&bEOL,1)!=0){                       // 08-11-07 PAB Mobile Printing
            memcpy(&messagerec[wPtr2],&seldescrec[wPtr],1);                 // 08-11-07 PAB Mobile Printing
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
    s_close(0, fnum);                                                       // 08-11-07 PAB Mobile Printing
    return 0;                                                               // 08-11-07 PAB Mobile Printing
}                                                                           // 08-11-07 PAB Mobile Printing


URC BuildLPR(char *inbound) {                                               // 07-08-07 PAB Mobile Printing

   LRT_PRT* pPRT = (LRT_PRT*)inbound;                                       // 07-08-07 PAB Mobile Printing
   LRT_LPR* pLPR = (LRT_LPR*)out;                                           // 07-08-07 PAB Mobile Printing
                                                                            // 07-08-07 PAB Mobile Printing
   LONG usrrc = RC_OK;                                                      // 07-08-07 PAB Mobile Printing
   UWORD lUnitNameCounter;
   LONG lIdufRecNum;
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
    usrrc = ReadIrf(__LINE__);
    if (usrrc<=0L) {
        return 1;
    } 

    //IRF read OK
    irf.present=TRUE;

    usrrc = ProcessPainkiller();
    
    calc_boots_cd(idfrec.boots_code, irfrec.boots_code);
    // read the IDF for the current item.
    usrrc = ReadIdf(__LINE__);

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
        usrrc = open_phf();
        pack(phfrec.ubPHFBarCode, 6, pPRT->item_code, 12, 0);
        phf.present=TRUE;
        usrrc = s_read( 0, phf.fnum, (void *)&phfrec,   
                        PHF_RECL, PHF_KEYL );          

        if (usrrc<=0L) {
           phf.present=FALSE;
        }
        usrrc = close_phf( CL_SESSION );
    }

    // read the isf to get the weee prf price (item qty field)  
    isf.present=TRUE;
    lUnitQty = 0;

    MEMCPY(isfrec.boots_code, idfrec.boots_code);                              
    usrrc = ReadIsf(__LINE__); 

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
               sprintf(msg, "Read IDUF for message %d",lUnitNameCounter);           // 14-11-07 PAB Mobile Printing
               disp_msg(msg);                                                       // 14-11-07 PAB Mobile Printing
    }   
            // if not weee then determine the unit price attributes
            if (lUnitNameCounter > 0) {
                // read the IUDF 
                usrrc = open_iduf();
                // compute the IDUF record number for the unit type
                // remember in C we compute the byte location of the start of each record.
                lIdufRecNum = ((lUnitNameCounter - 1) * IDUF_RECL) ;
                //usrrc = ReadIduf(lIdufRecNum, __LINE__);                         // 14-11-07 PAB Mobile Printing
                // read as a direct file remove function to improve performance.
                usrrc = s_read( A_BOFOFF,iduf.fnum,                                // 14-11-07 PAB Mobile Printing            
                    (void *)&idufrec, IDUF_RECL, lIdufRecNum);                     // 14-11-07 PAB Mobile Printing
                if (usrrc <= 0L) {
                    // the unit type was not found set to spaces
                    memset(pLPR->unit_type,0x20,sizeof(pLPR->unit_type));
                    memset(pLPR->unit_price_flag, 'N' ,sizeof(pLPR->unit_price_flag));
                } else {
                    memcpy(pLPR->unit_type,idufrec.abDescription,sizeof(pLPR->unit_type));
                    memset(pLPR->unit_price_flag, 'Y' ,sizeof(pLPR->unit_price_flag));
                }
                usrrc = close_iduf ( CL_SESSION );
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
    usrrc =open_sritml();

    MEMCPY(sritmlrec.abItemCode, irfrec.boots_code);                        
    sritmlrec.ubRecChain = 0;                                               
    usrrc = ReadSritml(__LINE__);
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
    usrrc = close_sritml( CL_SESSION );

    // return LPR transaction 
    memcpy(pLPR->cmd, "LPR", sizeof(pLPR->cmd)); 
    out_lth = LRT_LPR_LTH;     

    return RC_OK;
}


void unstall_sel_stack(void) {                                                          // BMG 1.1 11-09-2007

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
              pq[i].state = PST_READY;
           }
           // While we're going through all entries, if an in-progress file was last 
           // modified more than an hour ago then set it to Adopted to get it processed
           if ( (pq[i].state == PST_ALLOC) && (difftime(CurrTime, pq[i].last_access_time) >= 3600) ) {
              process_workfile( pq[i].unit, SYS_LAB );
              process_workfile( pq[i].unit, SYS_GAP );
           }
       }
       //Call a non-destructive process orphans
       process_orphans(FALSE);
    }
}

void dump_pq_stack(void){                                       // BMG 1.1 11-09-2007 Moved from rfs.c.

    if (debug) {                                                // SDH 10-May-2006
        disp_msg("Dump of PQ table follows...");                // SDH 10-May-2006
        disp_msg("States: 0=PST_FREE 1=PST_ALLOC "              // SDH 10-May-2006
                 "2=PST_READY 3=PST_ADOPTED "                   // SDH 10-May-2006 // BMG 1.1 11-09-2007
                 "4=PST_RUNNING 5=PST_STALLED");                                   // BMG 1.1 11-09-2007
        disp_msg("Types:  0=SYS_LAB  1=SYS_GAP   2=SYS_ORPHAN");// SDH 10-May-2006
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

void DealEnquiry(char *inbound) {                                   // BMG 1.1 11-09-2007 Moved from transact.c.

    int rc = 0;
    LONG rc2 = 0;

    LONG usrrc = RC_OK;
    UNUSED(inbound);

        //Input and output views                                    // SDH 15-12-04 PROMOTIONS
        LRT_DNQ* pDNQ = (LRT_DNQ*)inbound;                          // SDH 15-12-04 PROMOTIONS
        LRTLG_DNQ* pLGDNQ = (LRTLG_DNQ*)dtls;                       // SDH 15-12-04 PROMOTIONS
        LRT_DQR* pDQR = (LRT_DQR*)out;                              // SDH 15-12-04 PROMOTIONS

        //Initial checks                                            // SDH 15-12-04 PROMOTIONS
        if (IsStoreClosed()) return;                                // SDH 15-12-04 PROMOTIONS
        if (IsHandheldUnknown()) return;                            // SDH 15-12-04 PROMOTIONS
        UpdateActiveTime();                                         // SDH 15-12-04 PROMOTIONS

        //Open deal file                                            // SDH 15-12-04 PROMOTIONS
        usrrc = open_deal();                                        // SDH 15-12-04 PROMOTIONS
        if (usrrc < RC_OK) {                                        // SDH 15-12-04 PROMOTIONS
            prep_nak("ERRORUnable to "                              // SDH 23-Aug-2006 Planners
                     "open DEAL file. "                             // SDH 15-12-04 PROMOTIONS
                     "Check appl event logs");                      // SDH 15-12-04 PROMOTIONS
            return;                                                 // SDH 15-12-04 PROMOTIONS
        }                                                           // SDH 15-12-04 PROMOTIONS

        //Build deal file record key, read DEAL, handle errors      // SDH 15-12-04 PROMOTIONS
        pack(dealrec.abDealNumPD, 2, pDNQ->abDealNum, 4, 0);        // SDH 15-12-04 PROMOTIONS
        rc2 = ReadDeal(__LINE__);                                   // SDH 15-12-04 PROMOTIONS
        close_deal();                                               // SDH 15-12-04 PROMOTIONS
        if ((rc & 0xFFFF) == 0x06C8) {                              // SDH 15-12-04 PROMOTIONS
            prep_nak("Deal not on file");                           // SDH 23-Aug-2006 Planners
            return;                                                 // SDH 15-12-04 PROMOTIONS
        } else if (rc2 <= 0) {                                      // SDH 15-12-04 PROMOTIONS
            prep_nak("DEAL Information is "                         // PAB 27-Nov-2006 Planners
                     "out of step. Ring "                           // PAB 27-11-06 PROMOTIONS
                     "the Help Desk");                              // PAB 27-11-06 PROMOTIONS
            return;                                                 // SDH 15-12-04 PROMOTIONS
        }                                                           // SDH 15-12-04 PROMOTIONS

        //Build DQR                                                 // SDH 15-12-04 PROMOTIONS
        memcpy(pDQR->abCmd, "DQR", sizeof(pDQR->abCmd));            // SDH 15-12-04 PROMOTIONS
        memcpy(pDQR->abDealNum, pDNQ->abDealNum,                    // SDH 15-12-04 PROMOTIONS
               sizeof(pDQR->abDealNum));                            // SDH 15-12-04 PROMOTIONS
        unpack(pDQR->abStartDate, 8, dealrec.abStartDatePD, 4, 0);  // SDH 15-12-04 PROMOTIONS
        unpack(pDQR->abEndDate, 8, dealrec.abEndDatePD, 4, 0);      // SDH 15-12-04 PROMOTIONS
        memcpy(pDQR->abDealDesc, dealrec.abDealDesc,                // SDH 15-12-04 PROMOTIONS
               sizeof(pDQR->abDealDesc));                           // SDH 15-12-04 PROMOTIONS
        out_lth = LRT_DQR_LTH;                                      // SDH 15-12-04 PROMOTIONS

        //Build audit                                               // SDH 15-12-04 PROMOTIONS
        memcpy(pLGDNQ->abDealNum, pDNQ->abDealNum,                  // SDH 15-12-04 PROMOTIONS
               sizeof(pLGDNQ->abDealNum));                          // SDH 15-12-04 PROMOTIONS
        lrt_log(LOG_DNQ, hh_unit, dtls);                            // SDH 15-12-04 PROMOTIONS
}


void CycleLogs(void) {                                              // BMG 1.1 11-09-2007 Moved from transact.c.

    LONG cls_loop;                                                  // BMG 1.1 11-09-2007
    UWORD hh_unitx;                                                 // BMG 1.1 11-09-2007
    
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


void process_sel_stack(void) {                                                          // BMG 1.5 01-05-2008

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
                pq[i].state = PST_FREE;                                                // pab 26-1-2007
                pq[i].submitcnt = 0;                                                   // pab 26-1-2007
                memset(pq[i].fname, 0x00, 18);                                         // PAB 26-1-2007 
                pq[i].last_access_time = 0;                                            // 11-09-2007 5.5 BMG
                pq[i].type = 0;                                                        // 11-09-2007 5.5 BMG
                pq[i].unit = 0;                                                        // 11-09-2007 5.5 BMG
            } else {                                                                   // 11-09-2007 5.5 BMG
                if (pq[i].submitcnt >= 5) {                                            // 11-09-2007 5.5 BMG
                    pq[i].state = PST_STALLED;                                         // 11-09-2007 5.5 BMG
                    pq[i].submitcnt = 0;                                               // 11-09-2007 5.5 BMG
                }                                                                      // 11-09-2007 5.5 BMG
            }                                                                          // 11-09-2007 5.5 BMG
        }                                                                              // psb 26-1-2007

        //If the current element in the queue is ready
        if (pq[i].state == PST_READY || pq[i].state == PST_ADOPTED) {       // SDH 22-June-2006

            // If the appropriate program is not already running
            if (!semaphore_active(pq[i].type)) {

                //Build queue file name into string                         // SDH 25-11-04 OSSR WAN
                memcpy( fnm, pq[i].fname, 18);                              // SDH 25-11-04 OSSR WAN
                *(fnm+18) = 0x00;                                           // SDH 25-11-04 OSSR WAN

                // Start appropriate program as a background task
                switch (pq[i].type) {

                //Case PRINTSEL
                case SYS_LAB:

                    sprintf(msg, "Starting PRINTSEL: %s", fnm);             // SDH 05-01-06 BUG FIX
                    disp_msg(msg);                                          // SDH 05-01-06 BUG FIX
                    rc = start_background_app("ADX_UPGM:PRINTSEL.286",
                                         fnm, "Starting PRINTSEL" );        // SDH 31-10-2005 BUG FIX
                    // mark as unused/processed
                    if (rc == 0) {                                          // SDH 31-10-2005 BUG FIX
                        pq[i].state = PST_RUNNING;                          // PAB 26-1-2007
                        pq[i].submitcnt++;                                  // PAB 26-1-2007
                        started = TRUE;
                        status_wk_block = FALSE;
                    }                                                       // SDH 31-10-2005 BUG FIX
                    break;

                    //Case GAP (PSS47)
                case SYS_GAP:
                    
                    //Only kick off PSS47 if PSB30 isn't running.           // SDH 05-01-06 BUG
                    if (cStoreClosed == 'N') {                              // SDH 05-01-06 BUG
                        sprintf(msg, "Starting PSS47: %s", fnm);            // SDH 05-01-06 BUG
                        disp_msg(msg);                                      // SDH 05-01-06 BUG
                        rc = start_background_app( "ADX_UPGM:PSS47.286",    // SDH 25-11-04 OSSR WAN
                                              fnm, "Starting PSS47" );      // SDH 25-11-04 OSSR WAN
                        // mark as unused/processed                         // SDH 05-01-06 BUG
                        if (rc == 0) {                                      // SDH 31-10-2005 BUG FIX
                            pq[i].state = PST_FREE;
                            memset(pq[i].fname, 0x00, 18);                  // PAB 26-01-2007 
                            pq[i].submitcnt = 0;                            // 11-09-2007 5.5 BMG
                            pq[i].last_access_time = 0;                     // 11-09-2007 5.5 BMG
                            pq[i].type = 0;                                 // 11-09-2007 5.5 BMG
                            pq[i].unit = 0;                                 // 11-09-2007 5.5 BMG
                            pq[i].last_access_time = 0;                     // 11-09-2007 5.5 BMG
                            started = TRUE;
                            status_wk_block = FALSE;
                        }                                                   // SDH 31-10-2005 BUG FIX
                    } else {                                                // SDH 05-01-06 BUG
                        sprintf(msg, "PSB30 is running.  "                  // SDH 05-01-06 BUG
                                "Holding queue and PSS47 job: %s", fnm);    // SDH 05-01-06 BUG
                        disp_msg(msg);                                      // SDH 05-01-06 BUG
                    }                                                       // SDH 05-01-06 BUG
                                        
                    break;                                                  // SDH 25-11-04 OSSR WAN
                }
            }                                                               // endif (semaphore active)
        }                                                                   // endif (READY)
    }                                                                       // next i

}
