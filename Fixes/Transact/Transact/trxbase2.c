/*************************************************************************
*
* File: trxbase2.c
*
* 
* Original Version           Ranjith Gopalankutty         7th July  2016 
* Fix for defect raised during 16A with batch devices. Due to the change
* brought by RS5, PSB90 locks Transact at 12.00 am. McLoader starts
* at the same time so it gets a NAK response. Fix is that a new function
* has been added to  handle the dummy SignOn request HHT is sending.
* Transact will not check if store is closed if the wAppID is 6 instead 
* it responds back with SNR. Initial idea was to add the new logic in 
* trxbase.c but it is failing with memory error. So created a new module
* and referenced appropriately in required moduls.
*
*************************************************************************/

#include "transact.h"
#include <string.h>
#include "osfunc.h"
#include "prtctl.h"
#include "prtlist.h" 
#include "rfscf.h"
#include "trxutil.h"
 
typedef struct {
   BYTE cmd[3];
   BYTE msg[150];                              
} LRT_NAK; 
#define LRT_NAK_LTH sizeof(LRT_NAK)

typedef struct LRT_SNR_Txn {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE auth;                  
   BYTE name[15];              
   BYTE stamp[12];             
   BYTE prtnum[10];            
   BYTE bOssrWanActive;        
   BYTE cStockAccess;          
   BYTE snrprtdesc[200];       
} LRT_SNR; 
#define LRT_SNR_LTH sizeof(LRT_SNR) 

typedef struct {
   BYTE cmd[3];
   BYTE opid[3];
   BYTE pass[3];
   BYTE abAppID[3];                            
   BYTE AppVer[4];                             
   BYTE MAC[12];                               
   BYTE DevType[1];                            
   BYTE IPADDR[15];                            
   BYTE FreeMem[8];                            
} LRT_SOR;                                     
#define LRT_SOR_LTH sizeof(LRT_SOR) 

typedef struct {
    BYTE authority[1];                                                       
    BYTE resv[11];
} LRTLG_SOR;

static void buildSNR(LRT_SNR* pSNR, BYTE* pbPrtNum, BYTE* pbOpID, BYTE bAuth, 
              BYTE* pbUserName, BYTE cOssrStore, BYTE cStockAccess) {       

    LONG sec, day, month, year;                                             
    WORD hour, min;                                                         

    memcpy(pSNR->cmd, "SNR", 3);                                            
    memcpy(pSNR->prtnum, pbPrtNum, 10);                                     
    memcpy(pSNR->opid, pbOpID, 3);                                          
    pSNR->auth = bAuth;                                                     
    memcpy(pSNR->name, pbUserName, 15);                                     
    sysdate( &day, &month, &year, &hour, &min, &sec );                      
    sprintf(msg, "%04ld%02ld%02ld%02d%02d", year, month, day, hour, min );  
    memcpy(pSNR->stamp, msg, sizeof(pSNR->stamp));                          
    pSNR->bOssrWanActive = (cOssrStore == 'W') ? 'Y':'N';                   
    pSNR->cStockAccess = cStockAccess;                                      
}                                                                           

void prep_nak( BYTE *msg ) {                                                
   memcpy(((LRT_NAK*)out)->cmd, "NAK", 3);                                  
   strncpy(((LRT_NAK*)out)->msg, msg, sizeof(((LRT_NAK*)out)->msg));        
   out_lth = LRT_NAK_LTH;                                                   
}

void Log_Asset(char *inbound) {  //Log asset data passed in the SOR message // BMG 1.7 10-09-2008 MC70

    B_DATE nowDate;
    B_TIME nowTime;
    WORD wAppID;

    LRT_SOR* pSOR = (LRT_SOR*)inbound;

    //Convert the application ID to an int
    wAppID = satoi(pSOR->abAppID, sizeof(pSOR->abAppID));

    if (pdtasset.fnum >0) {
        memcpy( pdtassetrec.MAC, pSOR->MAC, 12);
        memcpy( pdtassetrec.Dev_Type, pSOR->DevType, 1);
        memcpy( pdtassetrec.IP_Addr, pSOR->IPADDR,15);
        if (wAppID >= 0 && wAppID < 10) {
            memcpy(pdtassetrec.AppVer[wAppID].AppVer, pSOR->AppVer, sizeof(pSOR->AppVer));
        }
        GetSystemDate(&nowTime, &nowDate);
        sprintf( sbuf, "%04d%02d%02d", nowDate.wYear, nowDate.wMonth, nowDate.wDay);
        memcpy( pdtassetrec.Date_Last_Dock, sbuf, 8);
        sprintf( sbuf, "%02d%02d", nowTime.wHour, nowTime.wMin);
        memcpy( pdtassetrec.Time_Last_Dock, sbuf, 4);
        memcpy( pdtassetrec.User_Last_Dock, pSOR->opid, 3);
        memcpy( pdtassetrec.Mem_Free, pSOR->FreeMem, 8);
        WritePDTAsset(__LINE__);
    }

}

void DummySignOn(char* inbound, WORD wReqLen) {                        

    //Working variables 
    TIMEDATE now;                                                      
    WORD wAppID;                                                       
    URC usrrc;                                                         
    BYTE cStockAccess; 
    BYTE authority[1], username[32];
                                                                       
                                                                       
    //Setup views                                                      
    LRT_SOR* pSOR = (LRT_SOR*)inbound;                                 
                                                                       
    cStockAccess = 'Y';                                                
                                                                       
    wAppID = satoi(pSOR->abAppID, sizeof(pSOR->abAppID));              


    // Allocate handheld a table entry
    disp_msg("Allocate lrt table...");                                 
    usrrc = alloc_lrt_table( hh_unit );                                
    if (usrrc<RC_IGNORE_ERR) {                                         
        prep_nak("ERROR unable to allocate storage. "                  
                  "Check appl event logs " );                          
        return;                                                        
    }

    // Log current time
    disp_msg("Log current time...");                                   
    s_get( T_TD, 0L, (void *)&now, TIMESIZE );                         
    lrtp[hh_unit]->last_active_time = now.td_time;                     
    memcpy( (BYTE *)&(lrtp[hh_unit]->txn), (BYTE *)inbound, 3);        
    memset( (BYTE *)&(lrtp[hh_unit]->unique), 0x00, 5 );               

    // Set state
    lrtp[hh_unit]->state = ST_LOGGED_ON;                               

    // Reset misc counts (Unused at present)
    lrtp[hh_unit]->count1 = 0;                                         
    lrtp[hh_unit]->count2 = 0;                                         

    //Set the device type in the table
    memset( (BYTE *)&(lrtp[hh_unit]->Type),'R', 1);                    

    //Call the asset logging function.                          
    if (wReqLen > 12) {                                               
        memcpy(lrtp[hh_unit]->Type, pSOR->DevType, 1);                
        memcpy(lrtp[hh_unit]->MAC, pSOR->MAC, 12);                    
        Log_Asset(inbound);                                           
    }                                                                 


    if  (wAppID == 6) {                                               

        // User authorised
        // Save user ID & name
        memcpy(lrtp[hh_unit]->user, ((LRT_SOR *)(inbound))->opid, 3 );
        memcpy(lrtp[hh_unit]->abOpName, username,                     
               sizeof(lrtp[hh_unit]->abOpName));                      

        // Prepare SNR                                          
        buildSNR((LRT_SNR *)&out, prtctlrec.prtnum,                   
                 ((LRT_SOR *)(inbound))->opid, authority[0],          
                 username, rfscfrec1and2.ossr_store,                  
                 cStockAccess);                                       
        

        out_lth = LRT_SNR_LTH;                                        
        //authorised = TRUE;                                   
                                                      
        ((LRTLG_SOR *)dtls)->authority[0] = 'Y';                      
        memset( ((LRTLG_SOR *)dtls)->resv, 0x00, 11 );                
        lrt_log( LOG_SOR, hh_unit, dtls );                            

    } else {                                                          

        //authorised = FALSE;
        memcpy( ((LRTLG_SOR *)dtls)->authority, "N", 1);              
        memset( ((LRTLG_SOR *)dtls)->resv, 0x00, 11 );                
        lrt_log( LOG_SOR, hh_unit, dtls );                            

        // Deallocate handheld's table entry
        usrrc = dealloc_lrt_table( hh_unit );                         

        // User not authorised
        prep_nak("User ID unknown or incorrect password");            
        return;                                                       
    }                                                                 

}

