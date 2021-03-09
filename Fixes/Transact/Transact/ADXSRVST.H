//-----------------------------------------------------------------------------
// Program : adxsrvst.h - ADXSERVE structure definitions
// Author   : Steve Wright
// Date  : 31st August 1995
// Version : A
//-----------------------------------------------------------------------------

#ifndef ADXSRVST_H
#define ADXSRVST_H

// Get system info
typedef struct ADXSERVE_Status
{
    BYTE store_number[4];
    BYTE date_form[1];
    BYTE time_form[1];
    BYTE monetary_form[1];
    BYTE display_type[1];
    BYTE terminals[3];
    BYTE node_id[4];
    BYTE resv1[2];
    BYTE master_id[2];
    BYTE ptr_lines[3];
    BYTE dec_digits[1];
    BYTE lan_system[1];
    BYTE acting_id[2];
    BYTE con_ptr[1];
    BYTE con_assg[1];
    BYTE resv2[52];
} ADXSRV_STATUS;

// Background message etc.
typedef struct ADXSERVE_Parm_General
{
    UWORD function;
    UWORD length;
    BYTE *buffer;
} ADXSRV_PARM;

typedef struct ADXSERVE_Parm_144
{
    WORD function;
    WORD type;
    BYTE *program;
    WORD programl;
    BYTE *parms;
    WORD parmsl;
    BYTE *message;
    WORD messagel;
} ADXSRV_PARM144;

// Log an event
typedef struct Event_Log
{
    BYTE b1;
    BYTE b2;
    WORD w1;
    BYTE b3;
    BYTE group;
    WORD message_no;
    BYTE severity;
    BYTE event;
    BYTE unique[18];
} LOG_BUF;

#endif

