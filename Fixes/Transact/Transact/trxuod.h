#ifndef TRXUOD_H
#define TRXUOD_H

void UODStart(char* inbound);
void UODAdd(char* inbound);
void UODExit(char* inbound);
void UODDirectSupplierListStart(char* inbound);
void UODGetDirectSupplierList(char* inbound);
void UODLabelEnquiry(char* inbound);
void UODStockTakeQuery(char* inbound);

#endif

