// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                  Radio Frequency Transaction Processor
//
//                           Goods In Application Functions Header
//
// Version 1.0               Brian Greenfield            10th October 2008
//     Initial Version
//
// ------------------------------------------------------------------------

#ifndef TRXGIA_H
#define TRXGIA_H

void GIA_Start(char *inbound);
void GIF_Booking(char *inbound);
void GIQ_Enquiry(char *inbound);
void GIX_End(char *inbound);

#endif
