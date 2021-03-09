#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <string.h>
#include <flexif.h>
#include "wrap.h"
//#include <sw/swdebug.h>

// Get a word from a text string
BOOLEAN get_word( FTEXT *ftb, WORD *word_size )
{
   BOOLEAN tbufov,rbufov;  // Buffer overflow test
   BOOLEAN delim;          // Delimiter character found
   BOOLEAN term;           // Termination character found
   BYTE tc, nc;            // This, next character
   
   ftb->tbuf_os = 0;
   do {
      tc = *(ftb->rbuf + ftb->rbuf_os++);
      if ( tc!=0x20 && tc!=0x00 && tc!=0x0D && tc!=0x0A ) {
         *(ftb->tbuf + ftb->tbuf_os++) = tc;
      }
      nc = *(ftb->rbuf + ftb->rbuf_os);
      tbufov = (ftb->tbuf_os > TEMP_BUFF_SZ);
      rbufov = (ftb->rbuf_os >= ftb->rbuf_sz);
      delim = (nc==0x20);
      term = (nc==0x00 || nc==0x0D || nc==0x0A);
   } while ( !(tbufov || rbufov) && !term && (!delim || ftb->tbuf_os==0) );
             
   *word_size = ftb->tbuf_os;
   return !(tbufov || rbufov || term);
}

// Word format a block of text, wrap every 'wbuf_ll' characters
WORD format_text( BYTE *rbuf, WORD rbuf_sz,
                  BYTE *wbuf, WORD wbuf_sz,
                  WORD wbuf_ll )
{
   BOOLEAN finished = FALSE;
   WORD word_size, space, skip;
   FTEXT ftb;
   BYTE twbuf[TEMP_BUFF_SZ];
   
   ftb.rbuf = rbuf;
   ftb.rbuf_os = 0;
   ftb.rbuf_sz = rbuf_sz;
   ftb.wbuf = wbuf;
   ftb.wbuf_os = 0;
   ftb.wbuf_sz = wbuf_sz;
   ftb.wbuf_ll = wbuf_ll;
   ftb.tbuf = twbuf;

   // Initialise the output buffer to spaces   
   memset( ftb.wbuf, 0x20, ftb.wbuf_sz );

   // Get each word from the input buf. and concatanate it to the output buf.
   skip=0;
   do {
      finished = !get_word( &ftb, &word_size );
      if ( ftb.tbuf_os>0 ) {
         space = ftb.wbuf_ll - (ftb.wbuf_os % ftb.wbuf_ll);
         if ( (skip + ftb.tbuf_os) > space ) {
            // Not enough space on line for word
            ftb.wbuf_os+=space;
            skip=0;
         }
         if ( ftb.wbuf_os + skip + ftb.tbuf_os <= ftb.wbuf_sz ) {
            memcpy( ftb.wbuf + ftb.wbuf_os + skip, ftb.tbuf, ftb.tbuf_os );
            ftb.wbuf_os+=(skip + ftb.tbuf_os);
            if ( (ftb.wbuf_os % ftb.wbuf_ll)==0 ) {
               skip=0;
            } else {
               skip=1;
            }
         } else {
            finished=TRUE;
         }
      }
   } while( !finished );
   
   return ftb.wbuf_os;
   
}

// v4.0 START
// Handle code page translations
void translate_text( BYTE *buf, WORD buf_sz )
{
   WORD i;
   
   // Translate pound sign (simple)
   for ( i=0; i<buf_sz; i++, buf++ ) {
      if ( *buf==0x9C ) {
         *buf = 0xA3;
      }
   }
   
}
// v4.0 END

/*
void main()
{
   WORD lth;
   BYTE orig_text[128];
   BYTE formatted_text[256];
   BYTE temp[512];
   
   memset( formatted_text, 0x20, 256 );

   strncpy( orig_text, "Uaxijul Maxijul Sol 200G 000 ", 128 );

   lth = format_text( orig_text, 24,
                      formatted_text, 45, 15 );
   
   memcpy( temp, formatted_text, lth );
   *(temp+lth) = 0x00;
   printf( "[%s]\n", temp );
   
   dump( formatted_text, 256 );

}
*/