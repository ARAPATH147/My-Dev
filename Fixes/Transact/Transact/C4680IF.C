#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <flexif.h>
#include "c4680if.h"    // v4.01

/******************************************************************************
C4680IF.C - SVC call for 4680 specific functions

Version B.            Stuart Highley                19th Nov 2004
OS calls have been made synchronous so that return codes are corrected handled.
the read also now waits for the appropriate time before returning record
already locked return code.
******************************************************************************/

#define SYNC    0
#define ASYNC   1

#define F_READ  7
#define F_WRITE  8
#define F_SPECIAL 9

struct _pblk {
   char pb_mode;
   char pb_option;
   int pb_flags;
   long pb_swi;
   long pb_p1;
   long pb_p2;
   long pb_p3;
   long pb_p4;
   long pb_p5;
};

LONG u_read(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                 LONG bufsiz, LONG offset, ULONG ulTimeoutMilli)        // SDH 17-11-04 OSSR WAN
{

    struct _pblk p;
    ULONG ulWait;
    LONG lRc;                                                           // SDH 17-11-04 OSSR WAN

    BOOLEAN retry = TRUE;                                               // SDH 17-11-04 OSSR WAN
    while(retry) {                                                      // SDH 17-11-04 OSSR WAN
       
        p.pb_mode = SYNC;                                               // SDH 17-11-04 OSSR WAN
        p.pb_option = option;
        p.pb_flags = flags;
        p.pb_swi = 0;
        p.pb_p1 = fnum;
        p.pb_p2 = (long)buffer;
        p.pb_p3 = bufsiz;
        p.pb_p4 = offset;
        p.pb_p5 = 0;
        //lRc = _osif(F_READ, &p);
        lRc = __osif(F_READ, (LONG)&p);

        //If there was a lock access conflict                           // SDH 17-11-04 OSSR WAN
        if ((lRc & 0xFFFF) == 0x430D) {                                 // SDH 17-11-04 OSSR WAN
            
            //Wait for the lesser of the remaining timeout and 500ms    // SDH 17-11-04 OSSR WAN
            ulWait = _min(500, ulTimeoutMilli);                         // SDH 17-11-04 OSSR WAN
            s_timer(0, ulWait);                                         // SDH 17-11-04 OSSR WAN
            ulTimeoutMilli -= ulWait;                                   // SDH 17-11-04 OSSR WAN
        
        } else {                                                        // SDH 17-11-04 OSSR WAN
            retry = FALSE;                                              // SDH 17-11-04 OSSR WAN
        }                                                               // SDH 17-11-04 OSSR WAN
    
    }                                                                   // SDH 17-11-04 OSSR WAN

    return lRc;                                                         // SDH 17-11-04 OSSR WAN

}

LONG u_write(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                  LONG bufsiz, LONG offset)
{

   struct _pblk p;

   p.pb_mode = SYNC;                                                    // SDH 17-11-04 OSSR WAN
   p.pb_option = option;
   p.pb_flags = flags;
   p.pb_swi = 0;
   p.pb_p1 = fnum;
   p.pb_p2 = (long)buffer;
   p.pb_p3 = bufsiz;
   p.pb_p4 = offset;
   p.pb_p5 = 0;
   //return (_osif(F_WRITE, &p));
   return (__osif(F_WRITE, (LONG)&p));

}

