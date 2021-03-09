#ifndef NVURL_H
#define NVURL_H

#include "trxfile.h"

typedef struct {
    BYTE btc_parent[3];
    BYTE btc_link[3];
    BYTE url_info[60];                                                      // Info URL : (e.g. http://domain/dir/file.html)
    BYTE url_link[60];                                                      // Link URL : (e.g. http://domain/dir/file.html)
    UBYTE flags;                                                            // bits 1-0 = enforcement 00 = no enforcement
} NVURL_REC;

extern FILE_CTRL nvurl;
extern NVURL_REC nvurlrec;

void NvurlSet(void);
URC NvurlOpen(void);
URC NvurlClose(WORD type);
LONG NvurlRead(LONG lLineNum);

#endif

