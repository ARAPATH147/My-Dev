#ifndef TRXPOG_H                                                     //SDH 14-Sep-2006 Planners
#define TRXPOG_H                                                     //SDH 14-Sep-2006 Planners

void PogSessionStart(char *inbound);                                 //SDH 14-Sep-2006 Planners
void PogSessionExit(char *inbound);                                  //SDH 14-Sep-2006 Planners
void PogLoadFamilies(char *inbound);                                 //SDH 14-Sep-2006 Planners
void PogListQuery(char *inbound);                                    //SDH 14-Sep-2006 Planners               
void PogListModules(char *inbound);                                  //SDH 14-Sep-2006 Planners
void PogShelfLoadRequest(char *inbound);                             //SDH 14-Sep-2006 Planners
void PogItemEnq(char *inbound);                                      //SDH 14-Sep-2006 Planners
void PogSiteEnq(char *inbound);                                      //SDH 20-May-2009 Model Day
void PogPrint(char *inbound);                                        //SDH 14-Sep-2006 Planners

#endif                                                               //SDH 14-Sep-2006 Planners

