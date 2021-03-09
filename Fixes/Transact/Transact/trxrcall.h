#ifndef TRXRCALL_H
#define TRXRCALL_H

void RecallStart(char *inbound);                                    // 24-5-2007 PAB Recalls
void RecallExit(char *inbound);                                     // 24-5-2007 PAB Recalls
void RecallListRequest(char *inbound);                              // 24-5-2007 PAB Recalls
void RecallCount(char *inbound);                                    // 25-5-2007 PAB Recalls
void RecallSelectList(char *inbound);                               // 25-5-2007 PAB recalls
void RecallInstructions(char *inbound);                             // 25-5-2007 PAB recalls
void StopRecalls(void);                                             // 25-5-2007 PAB recalls
URC process_recok(void);                                            // 25-5-2007 PAB recalls

#endif

