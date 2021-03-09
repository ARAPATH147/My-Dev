#ifndef INVOK_H
#define INVOK_H

#include "trxfile.h"

typedef struct {
    BYTE serial_no[5];
    BYTE date[3];
    BYTE success_flag[1];
    BYTE store_no[4];
    BYTE inventory_srlno[5];
    BYTE inventory_success[1];
    BYTE sales_srlno[5];
    BYTE sales_success[1];
    BYTE new_list_srlno[5];
    BYTE new_list_success[1];
    BYTE csr_ident[1];
    BYTE csr_delivery_no[5];
    BYTE csr_delivery_date[6];
    BYTE csr_psc11_flag[1];
    BYTE csr_psc12_flag[1];
    BYTE csr_psc13_flag[1];
    BYTE csr_psc12_days[1];
    BYTE pss33_run_date[3];
    BYTE pss33_success_flag[1];
    BYTE dir_impl_flag[1];
    BYTE psc30_run_date[3];
    BYTE psc30_success_flag[1];
    BYTE uod_impl_flag[1];
    BYTE last_uod_date[3];
    BYTE prev_serial_no[5];
    BYTE prev_success_flag[1];
    BYTE suppress_excep_report[1];
    BYTE csr_started_by_sup[1];
    BYTE csr_psc14_flag[1];
    BYTE csr_conversion_status_flag[1];
    BYTE pts_events_ok_date[3];
    BYTE rp_days[1];
    BYTE filler[6];
} INVOK_REC;

extern FILE_CTRL invok;
extern INVOK_REC invokrec;

void InvokSet(void);
URC InvokOpen(void);
URC InvokClose(WORD type);
LONG InvokRead(LONG lRecNum, LONG lLineNum);

#endif

