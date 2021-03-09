//-----------------------------------------------------------------------------
//  Program : adxsrvfn.c - general ADXSERVE functions
//  Author  : Steve Wright
//  Date    : 31st August 1995
//  Version : A
//-----------------------------------------------------------------------------

#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <string.h>     // V4.01
#include <flexif.h>
#include "adxsrvfn.h"   // v4.01

#include "adxsrvst.h"   /* needed for ADXSRV_PARM144, etc... */

#include "file.h"   // v4.01
#include "debug.h"  // v4.01
#include "rfs.h"    // v4.01

//extern WORD debug, oput;      // v4.01
//extern FILE_CTRL dbg;         // v4.01

LONG srv_fnum;                  // adxserve driver file number

LONG start_background_app(BYTE *prog, BYTE *pparms, BYTE *message) {
   
    LONG srv_fnum, rc;                                                       //SDH 22-June-2006
    ADXSRV_PARM144 parms;

    srv_fnum = s_open(0x2090, "adxserve:");                                  //SDH 27-Sep-2006
    if (srv_fnum < 0) {                                                      //SDH 27-Sep-2006
        log_event101(srv_fnum, 0, __LINE__);                                 //SDH 28-Sep-2006
        sprintf(msg, "ADXSERVE: Cannot open ADXSERVE. RC=%08lX", srv_fnum);  //SDH 22-Jun-2006
        disp_msg(msg);                                                       //SDH 27-Sep-2006
        return srv_fnum;                                                     //SDH 27-Sep-2006
    }                                                                        //SDH 27-Sep-2006

    parms.function = 144; 
    parms.type = 0x80;
    parms.program = prog;
    parms.programl = strlen(prog);
    parms.parms = pparms;
    parms.parmsl = strlen(pparms);
    parms.message = message;
    parms.messagel = strlen(message);

    rc = s_special(0, 0, srv_fnum, 0L, 0L, (BYTE *)&parms, sizeof(parms));   //SDH 22-June-2006
    s_close(0, srv_fnum);                                                    //SDH 27-Sep-2006
    if (rc < 0) {                                                            //SDH 22-June-2006
        sprintf(msg, "%010ld", rc);                                          //SDH 27-Sep-2006
        log_event(23, 3, msg);                                               //SDH 27-Sep-2006
        sprintf(msg, "ADXSERVE: Cannot start background task. RC=%08lX", rc);//SDH 22-June-2006
        disp_msg(msg);                                                       //SDH 27-Sep-2006
        return rc;                                                           //SDH 27-Sep-2006
    }                                                                        //SDH 22-June-2006

    return 0;                                                                //SDH 27-Sep-2006

}

// 4.03 start PAB December 2003

void chain_app () {

  //LONG ret;                                                       // SDH 17-11-04 OSSR WAN
  
  PINFO procinfo;

  strcpy(&procinfo.pi_pname[0],"TRANSACT  ");
  procinfo.pi_prior = 200;
  procinfo.pi_rsvd1 = 0;
  procinfo.pi_maxmem = 00000000L;
  
  s_command (                                                       // SDH 17-11-04 OSSR WAN
                   (UWORD) 0x0020,
                   (BYTE *) "ADX_UPGM:TRANSACT.286",
                   (BYTE *) 0L,
                   (LONG)  0L,
                   &procinfo
                  );  
} 

void restrict_file (BYTE *filename)
{

  //LONG ret;                                                       // SDH 17-11-04 OSSR WAN
  BYTE com_parms [100];
  
  PINFO procinfo;

  strcpy(&procinfo.pi_pname[0],"RESTRICT " );
  procinfo.pi_prior = 200;
  procinfo.pi_rsvd1 = 0;
  procinfo.pi_maxmem = 00000000L;
  strncpy(com_parms, filename, strlen(filename));
  s_command (                                                       // SDH 17-11-04 OSSR WAN
                   (UWORD) 0x0040,
                   (BYTE *) "ADX_SPGM:RESTRICT.286",
                   (BYTE *) com_parms,
                   (LONG) strlen(com_parms) ,
                   &procinfo
                  );  
} 


// 4.03 end pab december 2003

// Display a background message
void background_msg(BYTE *text)
{
   UBYTE i, pad;
   BYTE tbuf[47];
   ADXSRV_PARM parms;

   for (i=0, pad=FALSE; i<46; i++) {
      if ((*(text+i)==0) || (*(text+i)==13)) {
         pad=TRUE;
      }
      if (!pad) {
         *(tbuf+i)=*(text+i);
      } else {
         *(tbuf+i)=0x20;
      }
   }

   parms.function = 26;
   parms.length = 46;
   parms.buffer = tbuf;

   srv_fnum = s_open(0x2090, "adxserve:");
   s_special(0x0000, 0x0000, srv_fnum, 0L, 0L, (BYTE *)&parms, 0L);
   s_close(0x0000, srv_fnum);
}

// Display message to background or foreground (dependant on bg - which
// should be set using bg=establish_start_method()
void disp_msg(BYTE *text)
{
   //LONG rc;                                                       // SDH 17-11-04 OSSR WAN
   BYTE temp[256];

   if (debug) {
      
      if ( oput==DBG_LOCAL ) {
         printf( "%s\n", text );
      } else {
         sprintf( temp, "%s\r\n", text );
        if (dbg.fnum>0L) {
           s_write( A_EOFOFF, dbg.fnum, temp, strlen(temp), 0L );   // SDH 17-11-04 OSSR WAN
        } else {
           printf( "%s\n", text );
        }  
      }

   }
}

// Log a general event
void log_event(UBYTE event, UBYTE sev, BYTE *unique) {

   ULONG log_fnum;
   LOG_BUF logx_buf;
   srv_fnum=s_open(A_EXEC, "adxserve:");
   log_fnum=s_open(0x0014, "ADXLOGX");
   logx_buf.b1=12;
   logx_buf.b2=18;
   logx_buf.w1=0x0000;
   logx_buf.b3=0x01;
   logx_buf.group='J';
   logx_buf.message_no=000;
   logx_buf.severity=sev;
   logx_buf.event=event;
   blkfill(logx_buf.unique, 0x00, 18);
   strcpy(logx_buf.unique, "TRANSACT");                                     //SDH
   memmove(logx_buf.unique+8, unique, 10);
   s_write(0, log_fnum, (BYTE *)&logx_buf, sizeof(logx_buf), 0L);
   s_close(0, log_fnum);
   s_close(0, srv_fnum);

}

// Log an event 101
void log_event101(LONG rc, WORD file_rep, WORD line_no)
{
   BYTE event_data[10];
   BYTE abTemp[7];                                                          //SDH 08-03-2005
   if (rc != 0x80204004) {
      blkfill(event_data, 0x00, 10);
      event_data[0]=(UBYTE)*((UBYTE *)&rc+3);
      event_data[1]=(UBYTE)*((UBYTE *)&rc+2);
      event_data[2]=(UBYTE)*((UBYTE *)&rc+1);
      event_data[3]=(UBYTE)*((UBYTE *)&rc);
      event_data[4]=(UBYTE)((file_rep>>8)&0xFF);
      event_data[5]=(UBYTE)(file_rep&0xFF);
      sprintf(abTemp, "%06d", line_no);                                     //SDH 08-03-2005
      pack(event_data+7, 3, abTemp, 6, 0);                                  //SDH 08-03-2005
      //event_data[8]=(UBYTE)((line_no>>8)&0xFF);
      //event_data[9]=(UBYTE)(line_no&0xFF);
      log_event(101, 3, event_data);
   }
}

// Determine whether appl started in background  or foreground
// Returns TRUE for background, FALSE for foreground
UBYTE establish_start_method() {

   BYTE buffer[256];
   s_get(T_CMDENV, 0L, buffer, sizeof(buffer));
   if (strncmp(buffer+128, "BACKGRND", 8)==0) {
      return (TRUE);   // Background application
   } else {
      return (FALSE);  // Foreground application
   }
}
