#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <string.h>
#include <flexif.h>
#include <sw/swdebug.h>

#include "file.h" // v4.01
#include "rfs.h"
#include "rfsfile.h"

LONG rc, fnum, i, delay;
BYTE *share;
SMTABLE sharetab;

LONG open_rfsstat() {
   fnum = s_open( A_READ | A_WRITE | A_SHARE, SM_BUFFER_NAME );
   if ( fnum<0L ) {
      printf( "Error - Unable to open buffer. RC : %08lX\n", fnum );
      delay = 10000L;
      return fnum;
   }
   rc = s_get( T_SHMEM, fnum, (SMTABLE *)&sharetab, SMSIZE );
   if ( rc<0L ) {
      printf( "Error - Unable to get shmem buffer table. RC : %08lX\n", rc );
      s_close( 0, fnum );
      delay = 10000L;
      return fnum;
   }
   share = sharetab.ubuffer;
   return 0L;
}

void lock_rfsstat() {
   do {
      rc = s_read( 0, fnum, "", 1, 1 );
      if ( rc<0L ) {
         if ( (rc&0xFFFFL)!=0x4001L ) {
            printf( "Error - Unable to lock shmem semaphore. RC : %08lX\n", rc );
            s_close( 0, fnum );
            s_exit(0);
         } else {
            s_timer( 0, 200 );
         }
      }
   } while ( rc!=0L );
}

void unlock_rfsstat() {
   rc = s_write( 0, fnum, "", 1, 1 );
   if ( rc<0L ) {
      printf( "Error - Unable to unlock shmem semaphore. RC : %08lX\n", rc );
      s_close( 0, fnum );
      s_exit(0);
   }
}

void main()
{

   do {

      delay = 1000L;
      rc = open_rfsstat();

      if ( rc>=0L ) {
         lock_rfsstat();

         // Access shared memory buffer
         printf("%cE", 27);               // Clear screen
         dump( (BYTE *)share, 256 );

         // Unlock semaphore
         unlock_rfsstat();
         s_close( 0, fnum );
      }

      s_timer( 0, delay );
      
   } while ( TRUE );
   
   s_exit(0);

}
