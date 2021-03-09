// Ranjith Gopalankutty                                      07th July 2016
// Added a new function reference DummySignOn() to handle dummy sign on
// request during Transact Disabled scenarios with RS5 Project
//--------------------------------------------------------------------------

#ifndef TRXBASE2_H
#define TRXBASE2_H
void DummySignOn(char* inbound, WORD wReqLen);                              //BRG 07-07-2016 Fix for Automatic date & Time 
#endif