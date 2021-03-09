#ifndef AF_H
#define AF_H

#include "trxfile.h"

// EALAUTH record layout
typedef struct {
    BYTE operator_no[4];
    BYTE password[4];
    UBYTE options_key[1];
    UWORD indicat1;
    UWORD indicat2;
    UWORD indicat3;
    UBYTE indicat4;
    UBYTE indicat5;
    UBYTE indicat6;
    UBYTE indicat7;
    UBYTE indicat8;
    UBYTE indicat9;
    UBYTE indicat10;
    UBYTE indicat11;
    UBYTE indicat12;
    BYTE operator_name[20];
    UBYTE indicat13;
    UBYTE indicat14;
    UBYTE indicat15;
    UBYTE indicat16;
    BYTE reserved[12];
    BYTE user[9];
    BYTE date_pswd_change[3];
    UWORD model_flags_1;
    UWORD model_flags_2;
    BYTE sup_flag[1];
    BYTE op_model[3];
} EALAUTH_REC;

extern FILE_CTRL af;
extern EALAUTH_REC afrec;

void AfSet(void);
URC AfOpen(void);
URC AfClose(WORD type);
LONG AfRead(LONG lLineNum);

#endif

