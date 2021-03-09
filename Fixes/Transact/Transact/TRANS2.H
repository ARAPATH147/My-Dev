// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
// 
// Version 4.0               Steve Wright                28th August 2003
//     2003 Trial
//
// -----------------------------------------------------------------------

#ifndef TRANS2_H
#define TRANS2_H

/* include files */

/*#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <flexif.h>*/

//extern char cStoreClosed;

int startup();
void SignOffNak ( char * pNAK, char * pMsg, WORD * iLength );
void prep_nak( BYTE *msg );                                                 // SDH 23-Aug-06 Planners
void prep_ack( BYTE *msg );                                                 // SDH 23-Aug-06 Planners
void prep_pq_full_nak(void);                                                // SDH 22-12-04
BOOLEAN IsStoreClosed(void);                                                // SDH 26-11-04 CREDIT CLAIM
BOOLEAN IsHandheldUnknown(void);                                            // SDH 26-11-04 CREDIT CLAIM
void UpdateActiveTime(void);                                                // SDH 26-11-04 CREDIT CLAIM
BOOLEAN IsReportMntActive(void);                                            // SDH 26-11-04 CREDIT CLAIM
void DealFileTrickle (void);                                                // SDH 26-11-04 Promotions

#endif

