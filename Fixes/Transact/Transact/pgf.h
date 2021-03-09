#ifndef PGF_H
#define PGF_H

#include "trxfile.h"

// record layout
typedef struct {                                                            // PAB 23-10-03
    BYTE prod_grp_no[3];                                                    // UPD PAB 23-10-03
    BYTE abProdGrpName[18];                                                 // SDH 24-12-04
    BYTE price_check_notexempt[1];                                          // ASCII [Y/N]     PAB 23-10-03
    BYTE cOssrFlag; //'Y' or 'N' (' ' means 'N')                            // SDH 24-12-2004
    BYTE pgf_filler2[7];                                                    // SDH 24-12-2004
} PGF_REC;

extern FILE_CTRL pgf;
extern PGF_REC pgfrec;

void PgfSet(void);
URC PgfOpen(void);
URC PgfClose(WORD type);
LONG PgfRead(LONG lLineNum);

#endif

