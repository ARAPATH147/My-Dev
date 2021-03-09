//-----------------------------------------------------------------------------
// Program : adxsrvfn.h - general ADXSERVE functions
// Author   : Steve Wright
// Date  : 31st August 1995
// Version : A
//-----------------------------------------------------------------------------

#if !defined ADXSRVFN_H
#define ADXSRVFN_H 1

// v4.01 START
LONG start_background_app(BYTE *prog, BYTE *pparms, BYTE *message);
void background_msg(BYTE *text);
void disp_msg(BYTE *text);
void log_event(UBYTE event, UBYTE sev, BYTE *unique);
void log_event101(LONG rc, WORD file_rep, WORD line_no);
void restrict_file (BYTE *filename);                                    //SDH 09-02-2005
UBYTE establish_start_method();

#endif

// v4.01 END

