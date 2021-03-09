// Charles Skadorwa                                          15th Apr 2010
// Changes to support POD Logging Enhancements.
//
// Charles Skadorwa                                          12th Mar 2012
// Changes to support Stock File Accuracy  (commented: //CSk 12-03-2012 SFA).
// Added CreateListItem() function.
// Added CreateCountList() function.
//
// -----------------------------------------------------------------------


#ifndef TRXBASE_H
#define TRXBASE_H

void SignOn(char* inbound, WORD wReqLen);
void SignOff(char* inbound);
void GapSignOn(char* inbound);
void GetStoreNumber(char* inbound);
void PriceMismatch(char* inbound);
void GapNotification(char* inbound);
void GapSignOff(char* inbound);
void ItemEnquiry(char* inbound);
void PrintSEL(char* inbound);
void PListSignOn(char* inbound);
void PListGetListOfLists(char* inbound);
void PListGetList(char* inbound);
void PListUpdateItem(char* inbound);
void PListSignOffList(char* inbound);
void PListSignOffSession(char* inbound);
void PriceCheckSignOn(char* inbound);
void PriceCheckSignOff(char* inbound);
void InfoSignOn(char* inbound);
void InfoSignOff(char* inbound);
void InfoStoreSales(char* inbound);
void InfoItemSales(char* inbound);
void ReportSignOn(char* inbound);
void ReportGetReport(char* inbound);
void ReportGetTopLevel(char* inbound);
void ReportGetSpecificLevel(char* inbound);
void ReportSignOff(char* inbound);
void CountsSignOn(char* inbound);
void CountsGetListOfLists(char* inbound);
void CountsGetList(char* inbound);
void CountsUpdateItem(char* inbound);
void CreateListItem(char* inbound);                            //CSk 12-03-2012 SFA
void CreateCountList(char* inbound);                           //CSk 12-03-2012 SFA
void CountsSignOffList(char* inbound);
void CountsSignOffSession(char* inbound);
void WriteToFile(char *inbound);
void suspend_transaction(char* inbound);
void PlistGetItemMultiSite(char *inbound);                                  //SDH 20-May-2009 Model Day
void ALR_Request(char *inbound);
void ANK_Request(char *inbound);                                            //BMG 17-12-2009 RF Stabilisation
void Reconnect(char* inbound);                                              //BMG 17-12-2009 RF Stabilisation
void PODLOG_Request(char* inbound);                                         //CSk 15-04-2010 POD Logging
void SignOffNak ( char * pNAK, char * pMsg, WORD * iLength );
void prep_nak( BYTE *msg );
void prep_ack( BYTE *msg );
void prep_pq_full_nak(void);
BOOLEAN IsStoreClosed(void);
BOOLEAN IsHandheldUnknown(void);
void UpdateActiveTime (void);
BOOLEAN IsReportMntActive(void);

#endif

