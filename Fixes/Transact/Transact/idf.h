#ifndef IDF_H
#define IDF_H

#include "trxfile.h"

typedef struct {
    BYTE boots_code[4];
    BYTE first_bar_code[6];
    BYTE second_bar_code[6];
    BYTE no_of_bar_codes[2];
    BYTE product_grp[3];
    BYTE stndrd_desc[24];
    BYTE status_1[1];
    BYTE intro_date[3];
    BYTE bsns_cntr[1];
    BYTE filler[1];
    UBYTE bit_flags_1;
    UBYTE bit_flags_2;
    BYTE parent_code[4];
    BYTE date_of_last_sale[3];
} IDF_REC;

extern IDF_REC idfrec;
extern FILE_CTRL idf;

void IdfSet(void);
URC IdfOpen(void);
URC IdfClose(WORD type);
LONG IdfRead(LONG lLineNum);

#endif

