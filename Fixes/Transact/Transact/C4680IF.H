/******************************************************************************
C4680IF.H - SVC call prototypes for 4680 specific functions
******************************************************************************/
/*
RFAR LONG u_read(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                 LONG bufsiz, LONG offset);
RFAR LONG u_write(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                  LONG bufsiz, LONG offset);
*/

LONG u_read(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                 LONG bufsiz, LONG offset, ULONG ulTimeoutMilli);
LONG u_write(BYTE option, UWORD flags, LONG fnum, far BYTE *buffer,
                  LONG bufsiz, LONG offset);
