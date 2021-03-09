//*************************************************************************
//
// File    : TRANSMSG.H
// Author  : Visakha Satya
// Created : 20th April 2015
//
// Overview: Header file for Dallas messages.
//
//-------------------------------------------------------------------------
// Version A: Visakha Satya                                 20th Apr 2015
// SC079 Dallas Positive Receiving
//            Initial version
//
//************************************************************************/
#ifndef TRANSMSG_H
#define TRANSMSG_H

    #define CMD(a,b,c) ((LONG)c<<16) + ((LONG)b<<8) + (LONG)a

    // Dallas Positive Receiving Messages (inbound commands)
    // For inbound messages, add the command message definition

    // DAC - Dallas Positive Receiving Check Message
    #define CMD_DAC     CMD('D','A','C')
    #define CMD_DAC_LTH 6   // DAC Message Length is 6

    // DAL - Dallas UOD load Message
    #define CMD_DAL     CMD('D','A','L')
    #define CMD_DAL_LTH 10   // DAL Message Length is 10

    // DAR - Dallas UOD Receipt Message
    #define CMD_DAR     CMD('D','A','R')
    #define CMD_DAR_LTH 27   // DAR Message Length is 27

#endif
