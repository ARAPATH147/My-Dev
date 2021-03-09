/*************************************************************************
*
*
* File: osfunc.c
*
* Author: Prashant Kotak
*
* Created: 18/08/97
*
* Purpose:
*
* History:
* Version B             Stuart Highley                23-Sep-2008
* Merged module with ADXSRVFN.C
*
* Version C             Brian Greenfield              04-Mar-2009
* Added new functions to handle protection exception and divide by 
* zero errors. Also added generic chaining function.
*
* Version D             Brian Greenfield              15-11-2010
* Added function to obtain the controller's Node ID
*
*************************************************************************/

#include "transact.h"
#include <flexif.h>
#include <string.h>
#include "sockserv.h"
#include "osfunc.h"                                                     // Streamline SDH 16-Sep-2008
#include "trxutil.h"                                                    // Streamline SDH 23-Sep-2008
#include "rfscf.h"   /*BMG*/

////////////////////////////////////////////////////////////////////////////////
///
///   Private structure definitions
///
////////////////////////////////////////////////////////////////////////////////


// Get system info  // BMG 15-11-2010 uncommented as now required
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



////////////////////////////////////////////////////////////////////////////////
///
///   Private functions
///
////////////////////////////////////////////////////////////////////////////////

static void ExitApplication ( void )
{
   s_exit ( 0L ) ;
}


////////////////////////////////////////////////////////////////////////////////
///
///   Public functions
///
////////////////////////////////////////////////////////////////////////////////

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


// Display a background message
void background_msg(BYTE *text)
{
   LONG srv_fnum;
   ADXSRV_PARM parms;
   UBYTE i, pad;
   BYTE tbuf[47];

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


// Get the controller's Node ID // BMG 15-11-2010
int get_node_ID(BYTE *node)
{
   LONG srv_fnum;
   ADXSRV_PARM parms;
   ADXSRV_STATUS status;
   LONG ret;
   
   parms.function = 4;
   parms.length = 0;
   parms.buffer = 0;

   srv_fnum = s_open(0x2090, "adxserve:");
   ret=s_special(0x0000, 0x0000, srv_fnum, (BYTE *)&status, 0L, (BYTE *)&parms, 0L);
   s_close(0x0000, srv_fnum);
   if (ret==0) {
		strncpy(node, status.node_id + 2, 2);
		*(node+2)=0;
	} else {
		/* adxserve driver call failed */
      return -1;
	}  
   return 0;
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

    LONG srv_fnum;

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


void * AllocateBuffer ( long BufferSize )
{
   MPB mpb ; // param block for s_malloc
   long rc ;

   mpb.mp_start = 0L ;
   mpb.mp_min = BufferSize ;
   mpb.mp_max = BufferSize ;

   rc = s_malloc ( O_NEWHEAP, ( MPB * ) &mpb ) ;
   if ( rc < 0L ) {
      sprintf(sbuf, "s_malloc Failed with %lx !!", rc);                     // Streamline SDH 16-Sep-2008
      disp_msg (sbuf);                                                      // Streamline SDH 16-Sep-2008
   }

   return ( void * ) mpb.mp_start ;
}

void FreeBuffer ( void * AllocatedBuffer )
{
   s_mfree ( ( BYTE * ) AllocatedBuffer ) ;
}

void CtlBrkHandler ( void )
{
   disp_msg ("CtkBrkHandler!!");                                            // Streamline SDH 16-Sep-2008

   ShutDownAllSockets () ;

//   if ( DebugFileHandle != NULL ) {                                       // Streamline SDH 16-Sep-2008
//      printf ("Closing Debug File\r\n") ;                                 // Streamline SDH 16-Sep-2008
//      fclose ( DebugFileHandle ) ;                                        // Streamline SDH 16-Sep-2008
//   }                                                                      // Streamline SDH 16-Sep-2008

   ExitApplication();   //SDH 04-05-2005

}

void ChainApplication ( void ) /*BMG*/
{
    PINFO ProcInfo;
    memset(&ProcInfo, 0, sizeof(ProcInfo));
    memcpy(ProcInfo.pi_pname, "TRANSACT  ", 10);
    ProcInfo.pi_prior = 200;
    disp_msg("Chaining application...");
    background_msg("Chaining application...");
    ShutDownAllSockets();
    CloseAllFiles();
    s_close(0, cpipe.fnum);     //Close comms pipe
    RfscfClose( CL_ALL );
    s_close( 0, lrtlg.fnum );
    s_close(0, dbg.fnum);
    s_timer(0, 5000);
    s_command(A_CHAIN, "ADX_UPGM:TRANSACT.286", "", 0,
              &ProcInfo);
}

void DivideExceptHandler ( void ) //BMG C 04-03-2009
{
    disp_msg ("ExceptionHandler!!");
    log_event101(0x80004018, 0, __LINE__);
    ChainApplication();
}

void ProtExceptHandler ( void ) //BMG C 04-03-2009
{
    disp_msg ("ExceptionHandler!!");
    log_event101(0x8000401b, 0, __LINE__);
    ChainApplication();
}

long DivideExceptEvent ( void )  //BMG C 04-03-2009
{
   long emask ;

   emask = s_exception(3, ( LONG (*) () ) DivideExceptHandler);

   return emask ;
}

long ProtExceptEvent ( void )  //BMG C 04-03-2009
{
   long emask ;

   emask = s_exception(6, ( LONG (*) () ) ProtExceptHandler);

   return emask ;
}

long TermEvent ( void )
{
   long emask ;

   //emask = e_termevent( ( far LONG (*) () ) CtlBrkHandler, 0L ) ;
   emask = e_termevent( ( LONG (*) () ) CtlBrkHandler, 0L ) ; // SDH 19-May-2006

   return emask ;
}

