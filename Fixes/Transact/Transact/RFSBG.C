// ------------------------------------------------------------------------
//                     Boots The Chemists Store Systems
//                     Radio Frequency Counts Processor
// 
// v1.0 - Steve Wright                                  24th November 1998
// Reentrant background task to process individual count lists when the RF
// Transaction Server (TRANSACT.286) has received a CLX commit transaction
// from a handheld.
// List to be processed is passed as a parameter. This program must be run
// as soon after the CLX commit has been received. Due to the time it may
// take to process all items within the list it was decided to move this
// processing to a separate thread to TRANSACT, in order not to block
// future RF transactions.
//    
// V2.0 - Paul Bowers                                  28th April 2004
// Introduce the PIITM file to the applcation to be able to determine
// head office count list status at item level for RECOUNTs so that
// stock support does not throw them away and shedule recounts for ever
// on an HO list. 
// 
// V3.0 - Paul Bowers                                  1st September 2004
// File changes to the CLILF and CLOLF for the introduction of OSSR
// count locations.        
// 
// V4.0 - Stuart Highley                               10th Jan 2005
// Handle OSSR WAN stores.  We perform the same checks on the CLILF data
// for OSSR WAN Stores that we did for offline OSSR stores.
// -----------------------------------------------------------------------
#include "transact.h" //SDH 19-May-2006

// #pragma Memory_Model(Big)    // v4.0

/* include files */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <flexif.h>

#include "adxsrvfn.h"          /* needed for disp_msg() */
#include "adxsrvst.h"          /* needed for bg */
#include "file.h"                                        // v4.1.1
#include "debug.h"                                       // v4.1.1
#include "rfs.h"               
#include "rfsfile.h"
#include "wrap.h"

// Shared globals (with RFS.C)
#include "globals.h"                                     // v4.0


static BYTE *rfsbg = { "RFSBG   "};
//extern UBYTE bg;        // background appl flag for use with disp_msg()

void init()
{
   URC usrrc;
   LONG rc2;

   background_msg("Initialising...");
   
   irf.sessions    = 0;
   irfdex.sessions = 0;                                                     //SDH 14-01-2005 Promotions
   idf.sessions    = 0;
   stkmq.sessions  = 0;
   clolf.sessions  = 0;
   clilf.sessions  = 0;
   imstc.sessions  = 0;
   minls.sessions  = 0;
   pilst.sessions  = 0;
   piitm.sessions  = 0;                               // PAB 29-4-04
   rfscf.sessions  = 0;                               // PAB 3-9-04
   
   irf.fnum        = -1L;
   irfdex.fnum     = -1L;                                                   //SDH 14-01-2005 Promotions
   idf.fnum        = -1L;
   stkmq.fnum      = -1L;
   clolf.fnum      = -1L;
   clilf.fnum      = -1L;
   imstc.fnum      = -1L;
   minls.fnum      = -1L;
   pilst.fnum      = -1L;
   piitm.fnum      = -1L;
   rfscf.fnum      = -1L;                             // PAB 3-9-04
   
   
   usrrc = open_piitm();                                             // PAB 28-4-4
   if ( usrrc<=RC_DATA_ERR ) {                                       // PAB 28-4-4
      background_msg( "ERROR - Unable to open PIITM (check logs)" ); // PAB 28-4-4
      s_exit(0L);                                                    // PAB 28-4-4
   }                                                                 // PAB 28-4-4
   
   usrrc = open_rfscf();                                             // PAB 3-9-04
  if ( usrrc<=RC_DATA_ERR ) {                                        // PAB 3-9-04
     background_msg( "ERROR - Unable to open RFSCF (check logs)" );  // PAB 3-9-04
     s_exit(0L);                                                     // PAB 3-9-04
  }                                                                  // PAB 3-9-04
  
   usrrc = open_idf();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open IDF (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_irf();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open IRF (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_irfdex();                                                   //SDH 14-01-2005 Promotions
   if ( usrrc<=RC_DATA_ERR ) {                                              //SDH 14-01-2005 Promotions
      background_msg( "ERROR - Unable to open IRFDEX (check logs)" );       //SDH 14-01-2005 Promotions
      s_exit(0L);                                                           //SDH 14-01-2005 Promotions
   }                                                                        //SDH 14-01-2005 Promotions

   usrrc = open_clolf();
   if ( usrrc<=RC_DATA_ERR ) { background_msg( "ERROR - Unable to open CLOLF (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_clilf();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open CLILF (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_stkmq();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open STKMQ (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_imstc();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open IMSTC (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_minls();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open MINLS (check logs)" );
      s_exit(0L);
   }
   
   usrrc = open_pilst();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open PILST (check logs)" );
      s_exit(0L);
   }

   // v4.0 START
   // Read INVOK record
   usrrc = open_invok();
   if ( usrrc<=RC_DATA_ERR ) {
      background_msg( "ERROR - Unable to open INVOK file. Check appl. event logs\n" );
      s_exit(0L);
   }
   rc2 = s_read( A_BOFOFF, invok.fnum, (void *)&invokrec, INVOK_RECL, 0L );
   if ( rc2<=0L ) {
      log_event101(rfsbg, rc2, INVOK_REP, __LINE__);
      sprintf(msg, "Err-R INVOK. RC:%08lX", rc2);
      background_msg( msg );
   }
   close_invok( CL_SESSION );

   rc2 = s_read( A_BOFOFF, rfscf.fnum,                                 // 3-9-2004 PAB
                       (void *)&rfscfrec1and2, RFSCF_RECL, 0L );       // 16-11-2004 SDH
   if ( rc2<=0L ) {                                                    // 3-9-2004 PAB
       log_event101(rfsbg, rc2, RFSCF_REP, __LINE__);                  // 3-9-2004 PAB
   }                                                                   // 3-9-2004 PAB

   // v4.0 END   
}

void shutdown()
{

   close_pilst( CL_ALL );
   close_piitm( CL_ALL );                                                   // PAB 28-4-4
   close_minls( CL_ALL );
   close_imstc( CL_ALL );
   close_clilf( CL_ALL );
   close_clolf( CL_ALL );
   close_stkmq( CL_ALL );
   close_idf( CL_ALL );
   close_irf( CL_ALL );                                                     
   close_irfdex( CL_ALL );                                                  //SDH 14-01-2005 Promotions
   close_rfscf( CL_ALL );                                                   // 3-9-2004
   
}

void main( int args, char *arg[] )
{
   BOOLEAN finished, count_pending, process, write_trailer;
   WORD seq;
   WORD hour, min;
   LONG rc, rc2;
   LONG sec, day, month, year;
   LONG list_id, items_sold_today, count, sales_figure, variance, new_count;
   LONG sf_count, bs_count, os_count;                                         // 13-9-04 PAB
   LONG mismatch_qty;                                                         // v4.0
   DOUBLE mjd1, mjd2;                                                         // v4.0
   BYTE sbuf[64], stkmqrec[128];
   //RFSCF_REC_1AND2 rfscfrec1and2;
   IMSTC_REC imstcrec;
   CLOLF_REC clolfrec;
   CLILF_REC clilfrec;
   IRF_REC irfrec;
   IDF_REC idfrec;
   MINLS_REC minlsrec;
   PILST_DETAIL_REC pilstrec;
   PIITM_REC piitmrec;                                                   // PAB 28-4-4
  

   init();
   
   // Parse parameters
   if ( args<2 ) {
      shutdown();
      sprintf( msg, "ERROR - Incorrect number of paramaters" );
      background_msg(msg);
      s_exit(0L);
   }
   list_id = satol( arg[args-1], 3 );

   if ( list_id<0L || list_id>999L ) {
      shutdown();
      sprintf( msg, "ERROR - List %ld number out of range", list_id );
      background_msg(msg);
      s_exit(0L);
   }

   sprintf( msg, "Processing list %03ld", list_id );
   background_msg(msg);

   // Read CLOLF list header
   rc = s_read( A_BOFOFF, clolf.fnum, (void *)&clolfrec,
                CLOLF_RECL, (list_id-1L) * CLOLF_RECL );
   if ( rc<=0L ) {
      log_event101( rfsbg, rc, CLOLF_REP, __LINE__ );
      if ( (rc&0xFFFF)==0x4003 ) {
         sprintf( msg, "ERROR - List %03ld is not on file", list_id );
      } else {
         sprintf( msg, "ERROR - Reading CLOLF. RC : %08lX", rc );
      }
      background_msg(msg);
      shutdown();
      s_exit(0L);
   }

 
   if ( *(clolfrec.list_type) == 'H' ) {
      // Read PILST
      memcpy( pilstrec.list_no, clolfrec.head_off_list_id, 4 );
      rc = s_read( 0, pilst.fnum, (void *)&pilstrec, PILST_RECL, PILST_KEYL );
      if ( rc<=0L ) {
         log_event101( rfsbg, rc, PILST_REP, __LINE__ );
         sprintf( msg, "ERROR - Reading PILST. list no : %04d", pilstrec.list_no );     //3-9-04 PAB
         background_msg(msg);
         shutdown();
         s_exit(0);
      }
    }

   write_trailer = FALSE;
   finished = FALSE;
   for ( seq=1; !finished; seq++ ) {
   
      process = TRUE;
   
      // Read CLILF for list item details
      sprintf( sbuf, "%03ld", list_id );
      memcpy( clilfrec.list_id, sbuf , 3 );
      sprintf( sbuf, "%03d", seq );
      memcpy( clilfrec.seq, sbuf, 3 );
      rc = s_read( 0, clilf.fnum, (void *)&clilfrec, CLILF_RECL, CLILF_KEYL );
      if ( rc<=0L ) {
         if ( (rc&0xFFFF)!=0x06C8 && (rc&0xFFFF)!=0x06CD ) {
            log_event101( rfsbg, rc, CLILF_REP, __LINE__ );
            sprintf( msg, "ERROR - Reading CLILF. RC : %08lX", rc );
            background_msg(msg);
         }
         process = FALSE;
         finished = TRUE;
      }

      if ( *(clolfrec.list_type) == 'H' ) {                                          // 28-4-4 PAB 
          memcpy( piitmrec.list_no, clolfrec.head_off_list_id, 4 );                  // 28-4-4 PAB
          memcpy( piitmrec.list_seq, clilfrec.ho_seq_no, 2);                         // 28-4-4 PAB
          rc = s_read( 0, piitm.fnum, (void *)&piitmrec, PIITM_RECL, PIITM_KEYL );   // 28-4-4 PAB
          if ( rc<=0L ) {                                                            // 28-4-4 PAB
             if ( (rc&0xFFFF)!=0x06C8 && (rc&0xFFFF)!=0x06CD ) {                     // 28-4-4 PAB
                log_event101( rfsbg, rc, PIITM_REP, __LINE__ );                      // 28-4-4 PAB
                sprintf( msg, "ERROR - Reading PIITM. RC : %08lX", rc );             // 28-4-4 PAB
                background_msg(msg);                                                 // 28-4-4 PAB
             }                                                                       // 28-4-4 PAB
          process = FALSE;                                                           // 28-4-4 PAB
          finished = TRUE;                                                           // 28-4-4 PAB
          }                                                                          // 28-4-4 PAB
      }
      
      if (process) {   
         // Determine item status from IDF
         memset( idfrec.boots_code, 0x00, 4 );
         pack( idfrec.boots_code, 4, clilfrec.boots_code, 7, 1 );
         rc = s_read( 0, idf.fnum, (void *)&idfrec, IDF_RECL, IDF_KEYL );
         if ( rc<=0L ) {
            log_event101( rfsbg, rc, IDF_REP, __LINE__ );
            process = FALSE;
         }
         count_pending = ((idfrec.bit_flags_2 & 0x04)!=0);
      }

      if (process) {          
         // Determine current price from IRF
         memset( irfrec.bar_code, 0x00, 11 );
         pack( irfrec.bar_code+8, 3, clilfrec.boots_code, 6, 0 );
            rc = s_read( 0, irf.fnum, (void *)&irfrec,
                      IRF_RECL, IRF_KEYL );
         if ( rc<=0L ) {
            log_event101( rfsbg, rc, IRF_REP, __LINE__ );
            process = FALSE;
         }
      }
   
      if (process) {
         // Determine counted item's current sales from IMSTC
         memcpy( imstcrec.bar_code, irfrec.bar_code, 11 );
         rc = s_read( 0, imstc.fnum, (void *)&imstcrec,
                      IMSTC_RECL, IMSTC_KEYL );
         if ( rc<=0L ) {
            if ( (rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD ) {
               // Item not on IMSTC
               items_sold_today = 0L;
            } else {
               log_event101( rfsbg, rc, IMSTC_REP, __LINE__ );
               process = FALSE;
            }
         } else {
            // Item on IMSTC
            items_sold_today = imstcrec.numitems / 100L;
         }
         sf_count = satol( clilfrec.count_shopfloor, 4 );
         bs_count = satol( clilfrec.count_backshop, 4 );
         os_count = satol( clilfrec.abOSSRCount, 4 );                               // SDH 10-Jan-2005

         if ( sf_count == -1L) {
             // if no shop floor count in any store type then ignore the item
             process = FALSE;
         }
         
         //Also handle a space as a non-OSSR store                                  // 04-04-05 SDH OSSR WAN
         if (rfscfrec1and2.ossr_store == ' ') rfscfrec1and2.ossr_store = 'N';       // 04-04-05 SDH OSSR WAN

         if ( sf_count == -1L && bs_count == -1L && rfscfrec1and2.ossr_store == 'N') { // 16-11-04 SDH
            // if not OSSr store and both shop floor and backshop not counted
            // Ignore item                                                         // 13-9-04 PAB
            process = FALSE;                                                       // 13-9-04 PAB
         }

         if (bs_count == -1L && os_count == -1L && rfscfrec1and2.ossr_store != 'N') {  // 16-11-04 SDH OSSR WAN
             // if ossr store and at least two locations not counted which
             // one must be shop floor
             // Ignore item
             process = FALSE;                                                      // 3-9-2004 PAB
         } 
                                                                             
                                                                                
      }
      
      if (process) {

         if ( bs_count == -1L ) {                         
            bs_count = 0L;
         }
         if ( os_count == -1L ) {                                                // 13-9-04 PAB
             os_count = 0L;                                                      // 13-9-04 PAB
         }                                                                       // 13-9-04 PAB

         count = sf_count + bs_count + os_count;                                 // 13-9-04 PAB
         sales_figure = satol( clilfrec.sales, 4 );
         variance = items_sold_today - sales_figure;
         new_count = count - variance;

         memcpy( minlsrec.boots_code, idfrec.boots_code, 4 );
         rc = s_read( 0, minls.fnum, (void *)&minlsrec,
                      MINLS_RECL, MINLS_KEYL );
         if ( rc<=0L ) {
            if ((rc&0xFFFF)==0x06C8 || (rc&0xFFFF)==0x06CD) {
               minls.present=FALSE;
            } else {
               log_event101( rfsbg, rc, MINLS_REP, __LINE__);
               break;
            }
         } else {
            minls.present=TRUE;
         }
         
         // v4.0 START
         
         // if minls record is found and count status = 2 update it
         // to 3 so that stock support will process the recount     v4.0 PAB

         if (*(clolfrec.list_type) != 'H') {
            if ( minls.present && *minlsrec.count_status == '2') {
               *minlsrec.count_status = '3';
               rc2 = s_write( 0, minls.fnum,(void *)&minlsrec, MINLS_RECL, 0L );
            }
         }
         
         // Write record to MINLS
         if ( !minls.present){
           if (*(clolfrec.list_type) != 'H')  {                       // v4.0 PAB 
            memcpy( minlsrec.boots_code, idfrec.boots_code, 4 );      // v4.0 PAB
           // Set recount date to today plus INVOK,RP.DAYS            // 15-9-04 PAB
           // sysdate( &day, &month, &year, &hour, &min, &sec );      // 15-9-04 PAB       
           // mjd1 = ConvGJ( &day, &month, &year );                   // v4.0 PAB
           // unpack( sbuf, 2, invokrec.rp_days, 1, 0 );              // 15-9-04 PAB
           // mjd1 += (DOUBLE)satoi( sbuf, 2 );                       // 15-9-04 PAB
           // ConvJG( mjd1, &day, &month, &year );                    // 15-9-04 PAB
           // sprintf( sbuf, "%02d", (UBYTE)(year%100L) );            // 15-9-04 PAB
           // pack( minlsrec.recount_date, 1, sbuf, 2, 0 );           // 15-9-04 PAB
           // sprintf( sbuf, "%02d", (UBYTE)(month) );                // 15-9-04 PAB
           // pack( minlsrec.recount_date+1, 1, sbuf, 2, 0 );         // 15-9-04 PAB
           // sprintf( sbuf, "%02d", (UBYTE)(day) );                  // 15-9-04 PAB
           // pack( minlsrec.recount_date+2, 1, sbuf, 2, 0 );         // 15-9-04 PAB
            mismatch_qty = 0;                                         // v4.0 PAB
            sprintf( sbuf, "%06ld", mismatch_qty );
            pack( minlsrec.discrepancy, 3, sbuf, 6, 0 ); 
            *minlsrec.count_status = '1';                             // v4.0 PAB
            memset( minlsrec.recount_date, 0x00, 3);                  // 15-9-04 PAB
            sprintf( sbuf, "%06ld", mismatch_qty );
                        
            }
         }
         // v4.0 END

         if (*minlsrec.count_status == '2') {                          // 16-9-04 PAB
            *minlsrec.count_status = '3';                              // 16-9-04 PAB
         }  

         if ((*minlsrec.count_status == '3') && 
              (minlsrec.recount_date[1] == 0x00)) {                     // 16-9-04 PAB
            // Set recount date to today                                // 16-9-04 PAB
            sysdate( &day, &month, &year, &hour, &min, &sec );          // 16-9-04 PAB       
            //mjd1 = ConvGJ( &day, &month, &year );                     // 16-9-04 PAB
            mjd1 = ConvGJ( day, month, year );                          // 16-9-04 PAB
            ConvJG( mjd1, &day, &month, &year );                        // 16-9-04 PAB
            sprintf( sbuf, "%02d", (UBYTE)(year%100L) );                // 16-9-04 PAB
            pack( minlsrec.recount_date, 1, sbuf, 2, 0 );               // 16-9-04 PAB
            sprintf( sbuf, "%02d", (UBYTE)(month) );                    // 16-9-04 PAB
            pack( minlsrec.recount_date+1, 1, sbuf, 2, 0 );             // 16-9-04 PAB
            sprintf( sbuf, "%02d", (UBYTE)(day) );                      // 16-9-04 PAB
            pack( minlsrec.recount_date+2, 1, sbuf, 2, 0 );             // 1-9-04 PAB                                
           }  
         
                 
         // if not found add otherwise update
         // no need to check recount date, as these only come from count lists
         // which have been scheduled for today. 

         rc2 = s_write( 0, minls.fnum,
                        (void *)&minlsrec, MINLS_RECL, 0L );


         sysdate( &day, &month, &year, &hour, &min, &sec );            

         // Write type 13 record to STKMQ
         sprintf( sbuf, "%c%c%c", 0x22, 0x13, 0x3B);
         memcpy( stkmqrec, sbuf, 3 );
         sprintf( sbuf, "%02ld%02ld%02ld", year%100, month, day );
         pack( stkmqrec+3, 3, sbuf, 6, 0 );
         sprintf( sbuf, "%02ld%02ld%02ld", day, month, year%100 );
         pack( stkmqrec+20, 3, sbuf, 6, 0 );
         sprintf( sbuf, "%02d%02d%02d", hour, min, (WORD)sec );
         pack( stkmqrec+6, 3, sbuf, 6, 0 );
         pack( stkmqrec+23, 3, sbuf, 4, 0 );
         memcpy( stkmqrec+9, clolfrec.head_off_list_id, 4 );
         memcpy( stkmqrec+13, clilfrec.ho_seq_no, 2 );
         *(stkmqrec+15) = ((!minls.present && !count_pending) ?'C':'R');  // PAB 27-04-04
         if ( *(clolfrec.list_type) == 'H' ) {                            // PAB 28-04-04
            memcpy ( stkmqrec+15, piitmrec.status, 1);                    // PAB 28-04-04
            }                                                             // PAB 28-04-04
         pack( stkmqrec+16, 4, clilfrec.boots_code, 7, 1 );
         memset( stkmqrec+25, 0x00, 5);
         memcpy( stkmqrec+25+2, irfrec.salepric+2, 3 );
         memset( stkmqrec+31-1, 0x3B, 1 );
         if ( *clolfrec.list_type == 'R' ) {
            sprintf( sbuf, "%04ld%cXXXX%c%c%c",
                           new_count, 0x3B, 0x22, 0x0D, 0x0A );
         } else {
            sprintf( sbuf, "%04ld%c%04ld%c%c%c",
                           new_count, 0x3B, 0L, 0x22, 0x0D, 0x0A );
         }
         memcpy( stkmqrec+31, sbuf, 16 );
         rc = s_write( A_EOFOFF, stkmq.fnum, (void *)&stkmqrec,
                       STKMQ_T13_LTH, 0L );
         if ( rc<=0L ) {
            log_event101( rfsbg, rc, STKMQ_REP, __LINE__ );
            sprintf( msg, "ERROR - Writing STKMQ o/s eof. RC : %08lX", rc );
            background_msg(msg);
            shutdown();
            s_exit(0L);
         } else {
            write_trailer = TRUE;
         }
         
      }
         
   } // next CLILF record
   
   // Set CLOLF status to Complete
   *clolfrec.list_status = 'C';
      
   // Update CLOLF list header
   rc = s_write( A_FLUSH | A_BOFOFF, clolf.fnum, (void *)&clolfrec,
                 CLOLF_RECL, (list_id-1L) * CLOLF_RECL );
   if ( rc<=0L ) {
      log_event101( rfsbg, rc, CLOLF_REP, __LINE__ );
      sprintf( msg, "ERROR - Writing CLOLF o/s %ld. RC : %08lX",
                    (list_id-1L) * CLOLF_RECL, rc );
      background_msg(msg);
      shutdown();
      s_exit(0L);
   }

   // Only write type 14 STKMQ txn for [H]ead Office counts when at least
   // one type 13 STKMQ txn has been written
   if ( *(clolfrec.list_type) == 'H' && write_trailer ) {
      // Write type 14 record to STKMQ
      sprintf( sbuf, "%c%c%c", 0x22, 0x14, 0x3B);
      memcpy( stkmqrec, sbuf, 3 );
      sprintf( sbuf, "%02ld%02ld%02ld", year%100, month, day );
      pack( stkmqrec+3, 3, sbuf, 6, 0 );
      sprintf( sbuf, "%02d%02d%02d", hour, min, (WORD)sec );
      pack( stkmqrec+6, 3, sbuf, 6, 0 );
      memcpy( stkmqrec+9, clolfrec.head_off_list_id, 4 );
      memcpy( stkmqrec+13, pilstrec.status, 1 );   
      sprintf( sbuf, "%c%c%c", 0x22, 0x0D, 0x0A );
      memcpy( stkmqrec+14, sbuf, 3 );
      rc = s_write( A_EOFOFF, stkmq.fnum, (void *)&stkmqrec,
                    STKMQ_T14_LTH, 0L );
      if ( rc<=0L ) {
         log_event101( rfsbg, rc, STKMQ_REP, __LINE__ );
         sprintf( msg, "ERROR - Writing STKMQ o/s eof. RC : %08lX", rc );
         background_msg(msg);
         shutdown();
         s_exit(0L);
      }
   }
         
   shutdown();
   
   sprintf( msg, "Completed list %03ld, %d items processed",
                 list_id, seq-1 );
   background_msg(msg);

   s_exit(0);

}
