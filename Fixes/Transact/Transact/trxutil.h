// ------------------------------------------------------------------------
//                           Boots The Chemists Ltd
//                      Radio Frequency Server Application
//                                  Header
// ------------------------------------------------------------------------
// Version 1.0  Stuart Highley                                  22-Sep-2008
// Version 1.1  Charles Skadorwa  (Stock File Accuracy)         12-Mar-2012
//              Added BYTE *cStockAccess to authorise_user().
// ------------------------------------------------------------------------

#ifndef TRXUTIL_H
#define TRXUTIL_H

typedef struct {
    WORD wDOW;  // 0-6 for the day of week
    WORD wDay;
    WORD wMonth;
    WORD wYear;
} B_DATE;

typedef struct {
    WORD wHour;
    WORD wMin;
    WORD wSec;
} B_TIME;


#define MEMCPY(a,b) memcpy(a,b,sizeof(a))                                   //SDH 14-Sep-2006 Planners
#define MEMSET(a,b) memset(a,b,sizeof(a))                                   //SDH 14-Sep-2006 Planners
#define MEMCMP(a,b) memcmp(a,b,sizeof(a))                                   //SDH 20-May-2009 Model Day
VOID form_timestamp( BYTE *buf, WORD lth );
WORD satoi( BYTE *str, WORD lth );
LONG satol( BYTE *str, WORD lth );
void WordToArray(BYTE *pbDest, UWORD uwDestLen, WORD wNum);
void LongToArray(BYTE *pbDest, UWORD uwDestLen, LONG lNum);
#define WORD_TO_ARRAY(a,b) WordToArray(a, sizeof(a), b)
#define LONG_TO_ARRAY(a,b) LongToArray(a, sizeof(a), b)
#define ARRAY_TO_WORD(a) satoi(a, sizeof(a))
#define ARRAY_TO_LONG(a) satol(a, sizeof(a))
void unpack(BYTE *dest, WORD dest_lth, BYTE *source, WORD source_lth,
            WORD start_nibble);
void pack(BYTE *dest, WORD dest_lth,
          BYTE *source, WORD source_lth, WORD start_nibble);
WORD unpack_to_word(BYTE *source, WORD source_lth);
URC alloc_report_buffer( RBUF **rbufp );
URC dealloc_report_buffer( RBUF *rbufp );
URC alloc_lrt_table(UWORD unit);
URC dealloc_lrt_table(UWORD uwUnit);
void dump(BYTE *buff, WORD lth);
void calc_boots_cd(BYTE *bccd, BYTE *bc);
void AddBootsCheck(BYTE *abOutput, BYTE *abInput);                      // Mult UOD Rcll 12-08-2011 SDH
void calc_ean13_cd(BYTE *bccd, BYTE *bc);
URC stock_enquiry( BYTE type, BYTE *item_code_unp, ENQUIRY *dest );
URC authorise_user(BYTE *user, BYTE *password, BYTE *auth, BYTE *username, BYTE *cStockAccess);   //CSk 12-03-2012 SFA
URC process_workfile(WORD log_unit, BYTE type);
UBYTE semaphore_active(BYTE type);
void sysdate( LONG *day, LONG *month, LONG *year,
              WORD *hour, WORD *min, LONG *sec );
void GetSystemDate(B_TIME* pTime, B_DATE* pDate);
void GetYYMMDDHHmm(BYTE* pbYYMMDDHHmm, B_TIME* pTime, B_DATE* pDate);
DOUBLE emu_round( DOUBLE num );
URC SetListUnpicked(WORD wListId);
WORD format_text( BYTE *rbuf, WORD rbuf_sz,
                  BYTE *wbuf, WORD wbuf_sz,
                  WORD wbuf_ll );
void translate_text( BYTE *buf, WORD buf_sz );
DOUBLE   ConvGJ(LONG day, LONG month, LONG year);
void     ConvJG(DOUBLE jd, LONG *day, LONG *month, LONG *year);
WORD     ConvDOW(DOUBLE jd);
BOOLEAN IsLeapYear(int year);                                                        //CSk 03-08-2010 Defect 4522
int GetDaysInMonth(int year, int month);                                             //CSk 03-08-2010 Defect 4522

#endif
