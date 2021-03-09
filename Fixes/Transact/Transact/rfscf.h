// ****************************************************************************
// v1.0
// v2.0 - 12-03-2012 - Stock File Accuracy - Charles Skadorwa (CSk)
//                     Replaced resvd1 with bPendPlanLeadTime on Record 1.
// ****************************************************************************
#ifndef RFSCF_H
#define RFSCF_H

#include "trxfile.h"

//RFSCF Record 1 layout
typedef struct {                                                            // 16-11-04 SDH
    WORD pmed_term;
    WORD qbust_term;
    WORD pmed_next_txn;
    WORD qbust_next_txn;
    LONG pmed_txn_cnt;
    LONG pmed_qty;
    LONG qbust_txn_cnt;
    LONG qbust_qty;
    BYTE phase[1]; // Phase 5 = MC70 Phase 4 = Pocket PC, Phase 3 = Palm Pilot, Phase 2 = Symbol RF LDT, Phase 1 = Telxon RF PPC
    LONG recount_qty;                                                       // v2.1
    LONG recount_val;                                                       // v2.1
    LONG percent_var;                                                       // v2.1
    WORD recheck_days;                                                      // v2.1
    LONG pchk_target;                                                       // v2.1
    LONG pchk_done;                                                         // v2.1
    LONG pchk_upper;                                                        // v2.1
    LONG pchk_lower;                                                        // v2.1
    WORD pchk_inc;                                                          // v2.1
    LONG pchk_target_def;                                                   // v2.1
    LONG pchk_errors_c;                                                     // v2.1
    LONG pchk_errors_l;                                                     // v2.1
    BYTE emu_active[1];                                                     // v2.1
    BYTE prim_curr[1];          // [S]terling / [E]cu                       // v2.1
    LONG emu_conv_fact;         // nnnn.nnnnnn                              // v2.1
    BYTE ossr_store;            // Store has OSSR W/Y/N (W = WAN active)    // 31/8/04 PAB
    WORD recount_days_retain;                                               // PAB 25 Jan 2007
    BYTE cResvd3; //SDH 20-May-2009 Model Day.  cPlannersActive;       // Y or N                                   // SDH 25-Oct-2006 Planners
    BYTE bPendPlanLeadTime;     // Packed. Holds 7 or 21 days which is used //CSk 12-03-2012 SFA
                                // to indicate if a pending planner should  //CSk 12-03-2012 SFA
                                // be counted if it becomes active in the   //CSk 12-03-2012 SFA
                                // next N days.                             //CSk 12-03-2012 SFA
    //Record 2 start
    //NOTE THAT THESE FIELDS ARE NOT READ DUE TO THE READ CODE ONLY READING
    //80 BYTES.  SINCE THE FIELDS ARE NOT USED I WILL NOT FIX THIS BUG - SDH
    WORD hht_ip_min;                                                        // v2.1
    WORD hht_ip_max;                                                        // v2.1
    UWORD activity;                                                         // bit 0 Price Check active                      // v3.0
                                                                            // bit 1 Gap Monitor
                                                                            // bit 1 Gap Monitor
                                                                            // bit 2 Picking
                                                                            // bit 3 Reports
                                                                            // bit 4 Cust Service (P/Med)
                                                                            // bit 5 Cust Service (Q/Bust)
                                                                            // bit 6
                                                                            // bit 7
                                                                            // bit 8 UOD Active
    BYTE resvd2[74];                                                        // SDH 25-Sep-2006 Planners
} RFSCF_REC_1AND2;

//RFSCF Record 3 layout
typedef struct RFSCF_Record_Layout_3 {                                      // 16-11-04 SDH
    BYTE bCCActive;                                                         // 16-11-04 SDH
    BYTE abBusCentres[30];                                                  // 16-11-04 SDH
    BYTE abCChistNumDays[3];                                                // 16-11-04 SDH
    BYTE bDirectsActive;                                                    // 16-10-2008 BMG ASN/Directs
    BYTE bASNActive;                                                        // 16-10-2008 BMG ASN/Directs
    BYTE bPOSUODActive;                                                     // 16-10-2008 BMG ASN/Directs
    BYTE bOvernightDeliv;                                                   // 16-10-2008 BMG ASN/Directs
    BYTE bOvernightScan;                                                    // 16-10-2008 BMG ASN/Directs
    BYTE abScanBatchSize[3];                                                // 16-10-2008 BMG ASN/Directs
    BYTE abFiller[39];                                                      // 16-11-04 SDH // 16-10-2008 BMG ASN/Directs
} RFSCF_REC_3;                                                              // 16-11-04 SDH

extern FILE_CTRL rfscf;
extern RFSCF_REC_1AND2 rfscfrec1and2;
extern RFSCF_REC_3 rfscfrec3;

void RfscfSet(void);
URC RfscfOpen(void);
URC RfscfClose(WORD type);
LONG RfscfRead(LONG lRecNum, LONG lLineNum);
LONG RfscfUpdate(LONG lLineNum);

#endif

