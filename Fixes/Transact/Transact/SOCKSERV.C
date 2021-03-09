/*************************************************************************
*
*
* File: sockserv.c
*
* Author: Prashant Kotak
*
* Created: 18/08/97
*
* Purpose: High Level TCP/IP vendor-independant Sockets Server.
*
* History:
*
* Version B Paul Bowers
*
* Created 28/08/2004
*
* Purpose - Track PPC unit IP address to enable reconnection
* resource reuse rather than incremental allocation for each new PPC
* which connects without a valid disconnect. So if a unit connects and
* its IP addres is already known send the old UNIT number to the main
* core.
*
* 17/12/2009 BMG
* Changes to accomodate message fragmentation and new message
* formatting with message lengths for RF Stabilisation.
*
* 15/11/2010 BMG
* New functions to allow communication with the DEC Microbroker.
*
* 30/11/2010 Charles Skadorwa
* Defect 4191: TRANSACT - Removal of unneccessary trace message
* TRANSACT will open a number of sockets (depending on how many PPC's are
* connected). It then checks those sockets to see which ones have data
* waiting to be read from them. If Tracing is switched on, and if there is
* no data to be read from a socket, then it logs a failure message which is
* misleading. Error message commented out in ProcessPendingReceives().
*
* 8 Dec 2011  Stuart Highley
* Only abort sockets that are on port 800, as 4690 allows ANY socket to
* be aborted, leading to unfortunate circumstances.
* Also tidy up the DEC logic a bit.
*
*************************************************************************/

#include "transact.h"                                                       //SDH 19-May-2006

#include "osfunc.h"
#include <flexif.h>
#include "osfunc.h"
#include "trxfile.h"                                                        // Streamline SDH 22-Sep-2008
#include "rfglobal.h"                                                       //SDH 21-June-2006
#include "sockserv.h"
#include "trxutil.h"                                                        // BMG 17-12-2009
#include "MBfiles.h"                                                        // BMG 15-11-2010
#include <netlib.h>                                                         // BMG 15-11-2010
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
//#include "app_proc.h"

int getsockname(int s, struct sockaddr *name, int *namelen);                // SDH 8-Dec-2011

#define STKCF           "STKCF"
#define STKCF_OFLAGS    0x2018
#define STKCF_REP       0
#define POLL_FREQ       5000L

enum group_status {
    STATUS_READ,
    STATUS_WRITE,
    STATUS_EXCPT
};

enum sock_state {
    ERROR_STATE = -1,
    UNCONNECTED = 0,
    AWAITING_INPUT,
    AWAITING_OUTPUT,
    AWAITING_DISCONNECT
};


enum appmsg_type {
    MSG_TYPE_RBS,
    MSG_TYPE_BOOTS
};

//extern unsigned int LoopDelay ;
//extern unsigned int BootsHookFreq ;
//extern char PollControlFile ;
//extern long OpenDirectFile ( char *, unsigned int, int ) ;
//extern void CloseAllFiles ( void ) ;

CLIENT    *Clients[MAX_SOCKETS];              // client details TCP/IP table
sock_handle  PrimarySocketHandle;             // primary socket handle

sock_handle  DECSocketHandle;                 // DEC socket handle                             // BMG 15-11-2010
DECCONF      decconftab[100];                 // DECCONF messages table                        // BMG 15-11-2010
int          decconfentries=0;                // Number of entries in the DECCONF table        // BMG 15-11-2010
int          dectranslation=0;                // Inidicates if message translation is required // BMG 15-11-2010
BYTE         node[2];                         // Store for Node ID                             // BMG 15-11-2010

extern int tcperrno;

extern int process ( char *, int * ) ;  // v4.01

////////////////////////////////////////////////////////////////////////////////
///
///   PRIVATE FUNCTIONS
///
////////////////////////////////////////////////////////////////////////////////

static void CloseSocket (sock_handle *pSocketHandle) {                  // SDH 8-Dec-2011

    int iPort;                                                          // SDH 8-Dec-2011
    struct sockaddr Name;                                               // SDH 8-Dec-2011
    int iNameLen = sizeof(Name);                                        // SDH 8-Dec-2011

    //rc = SetBlockingMode(pSocketHandle, 0);                           // SDH 8-Dec-2011

    //Only abort socket if TRANSACT owns it (on port 800)               // SDH 8-Dec-2011
    if (getsockname (*pSocketHandle, &Name, &iNameLen) == 0) {          // SDH 8-Dec-2011
        iPort = 0x100*Name.sa_data[0] + Name.sa_data[1];                // SDH 8-Dec-2011
        if (iPort == HHTCOMMS_PORT) {                                   // SDH 8-Dec-2011
            soabort (*pSocketHandle);                                   // SDH 8-Dec-2011
        }                                                               // SDH 8-Dec-2011
    }                                                                   // SDH 8-Dec-2011

    soclose (*pSocketHandle);                                           // SDH 8-Dec-2011

}


static void ReleaseClientSlot (int SlotIndex)
{
   if ( Clients[SlotIndex] != (void *) NULL ) {
      // free the Buffer that has been allocated to this client
      FreeBuffer( (void *) Clients[SlotIndex]->Buffer );

      // now free the client itself
      FreeBuffer( (void *) Clients[SlotIndex] );
      //LogMessage( 10, "Client Slot %d Freed", SlotIndex ) ;

      //... and mark the slot as free
      Clients[SlotIndex] = NULL;
   }
}

static int AllocateSocket ( sock_handle *SockHandle )
{
   int rc ;

   /* allocate socket */
   rc = socket ( AF_INET, SOCK_STREAM, 0 ) ;
   if ( rc < 0 ) {
      sprintf(sbuf, "socket() failed with %d", tcperrno);
      disp_msg(sbuf);
   }
   *SockHandle = rc ;

   return rc ;
}

static int AllocateDECSocket ( sock_handle *SockHandle )  // BMG 15-11-2010
{
   int rc;
   int rc2 ;
   int noblock;

   /* allocate socket */
   rc = socket ( AF_INET, SOCK_STREAM, 0 ) ;
   if ( rc < 0 ) {
      sprintf(sbuf, "socket() failed with %d", tcperrno);
      disp_msg(sbuf);
   }
   *SockHandle = rc ;

   //Set up nonblocking mode
   noblock = 1 ;
   rc2 = ioctl (DECSocketHandle, FIONBIO, (char *)&noblock, sizeof (noblock) ) ;
   if ( rc2 != 0 ) {
      sprintf (sbuf, "DEC ioctl() failed with %d", tcperrno);
      disp_msg(sbuf);
      //soabort(DECSocketHandle);
      //soclose(DECSocketHandle);
      CloseSocket (&DECSocketHandle);                                   // SDH 8-Dec-2011
      rc= -1;
   }

   return rc ;
}

static int BindSocket ( sock_handle *SockHandle, int iPortNum )             // Streamline SDH 16-Sep-08
{
   int rc ;
   struct sockaddr_in addr ;

   // clear out struct and set it up
   memset ( ( char * ) &addr, 0, sizeof ( addr ) ) ;
   addr.sin_family = AF_INET;
   addr.sin_port = BSWAP(iPortNum);                                         // SDH 19-05-2005
   addr.sin_addr.s_addr = INADDR_ANY;

   rc = bind ( *SockHandle, ( struct sockaddr * ) &addr, sizeof ( addr ) ) ;
   if (rc < 0) {
      sprintf(sbuf, "bind() failed with: %d, attempting retry...", tcperrno); // Streamline SDH 16-Sep-2008
      disp_msg (sbuf);
      rc = port_cancel(BSWAP(iPortNum));                                      // SDH 19-05-2005
      sprintf(sbuf, "Port Cancel returned: rc=%d, err=%d", rc, tcperrno);     // Streamline SDH 16-Sep-2008
      disp_msg(sbuf);                                                         // Streamline SDH 16-Sep-2008
      rc = bind ( *SockHandle, ( struct sockaddr * ) &addr, sizeof ( addr ) ) ;
      if ( rc < 0 ) {
         sprintf(sbuf, "bind() failed again with : %d, oh dear!", tcperrno);  // Streamline SDH 16-Sep-2008
         disp_msg(sbuf);                                                      // Streamline SDH 16-Sep-2008
         CloseSocket ( SockHandle ) ;
      } else {
         disp_msg("oh, we're ok!");                                           // Streamline SDH 16-Sep-2008
      }
   } else {
      sprintf(sbuf, "bind() succeeded, tcperrno: %d", tcperrno);              // Streamline SDH 16-Sep-2008
      disp_msg(sbuf);                                                         // Streamline SDH 16-Sep-2008
      rc = 1 ;
   }

   return rc ;
}

static int ListenOnSocket ( sock_handle *SockHandle )
{
   int rc1, rc2, noblock ;

   // listen with a max of 10 connection requests queued (more ignored)
   rc1 = listen ( *SockHandle, 10 ) ;
   if ( rc1 != 0 ) {
      sprintf(sbuf, "listen() failed with %d", tcperrno);                   // Streamline SDH 16-Sep-2008
      disp_msg(sbuf);                                                       // Streamline SDH 16-Sep-2008
      CloseSocket ( SockHandle ) ;
   } else {
      //sprintf(sbuf, "listen() succeeded with SocketHandle=%d", *SockHandle);// Streamline SDH 16-Sep-2008
      //disp_msg(sbuf);                                                       // Streamline SDH 16-Sep-2008
      // listen succeeded so place the socket in nonblocking mode
      noblock = 1 ;
      rc2 = ioctl ( *SockHandle, FIONBIO,
                    ( char * ) &noblock, sizeof ( noblock ) ) ;
      if ( rc2 != 0 ) {
         rc1 = -1 ;
         sprintf (sbuf, "ioctl() failed with %d", tcperrno);                // Streamline SDH 16-Sep-2008
         disp_msg(sbuf);                                                    // Streamline SDH 16-Sep-2008
         CloseSocket( SockHandle ) ;
      }
   }
   return rc1 ;
}

static int AcceptOnSocket ( sock_handle *OriginalSocket, CLIENT *client )
{
   int namelen ;

   namelen = sizeof ( client->SocketAddress );
   client->SocketHandle = accept( *OriginalSocket,
                                  &( client->SocketAddress ),
                                  &namelen ) ;

   if (client->SocketHandle < 0 ) {
      if ( tcperrno == EWOULDBLOCK ) {
         // we're here because there are no outstanding
         // connect requests and we're not in blocking mode
         //printf ("accept returned EWOULDBLOCK\n");

      } else {
         sprintf (sbuf, "accept() failed with %d", tcperrno);               // Streamline SDH 16-Sep-2008
         disp_msg (sbuf);                                                   // Streamline SDH 16-Sep-2008
         /* close the socket */
         CloseSocket ( &( client->SocketHandle ) ) ;
         /* mark failed for return */
         client->SocketHandle = -1 ;
      }
   } else {
      // we've connected...

      sprintf (sbuf, "Client Connected, Handle = %d", client->SocketHandle);// Streamline SDH 16-Sep-2008
      disp_msg (sbuf);                                                      // Streamline SDH 16-Sep-2008

      // record the length of the client address
      client->SocketAddrLen = namelen ;
      // mark this client as in use...
      client->InUse = 1 ;
      // ...and its awaiting input from client
      client->SocketState = AWAITING_INPUT ;
      // set default values                                                                // BMG 17-12-2009
      client->Continuation = 0;                                                            // BMG 17-12-2009
      client->NewFormMess = 0;                                                             // BMG 17-12-2009
      client->ApplicationType = 0;                                                         // BMG 15-11-2010
   }

   return client->SocketHandle ;
}

static int SocketGroupStatus ( int *sock_handles, int num_socks, int sock_type )
{
   int rc, st_read, st_write, st_excpt ;
//   long timeout = 16000 ;
   long timeout = 5000L ;

   st_read = st_write = st_excpt = 0 ;
   switch ( sock_type ) {
   case STATUS_READ:
      st_read = num_socks ;
//            ioctl ( sock_handles[0], FIONBIO,
//                      ( char * ) &noblock, sizeof ( noblock ) ) ;
      break ;

   case STATUS_WRITE:
      st_write = num_socks ;
      break ;

   case STATUS_EXCPT:
      st_excpt = num_socks ;
      break ;
   }

   // check num_socks handles for read availability
   //sprintf (sbuf, "Read=%d,Write=%d,Excp=%d", st_read, st_write, st_excpt); // Streamline SDH 16-Sep-2008
   //disp_msg (sbuf);                                                         // Streamline SDH 16-Sep-2008

   rc = select( sock_handles, st_read, st_write, st_excpt, timeout ) ;
   if ( rc < 0 ) {
      sprintf (sbuf, "select() failed with %d", tcperrno);                  // Streamline SDH 16-Sep-2008
      disp_msg (sbuf);                                                      // Streamline SDH 16-Sep-2008
      rc = 0 ;
   } else if ( rc == 0 ) {
      //disp_msg ("select() timed out");                                      // Streamline SDH 16-Sep-2008
      rc = 0 ;
   } else if ( rc > 0 ) {
      // success
      //disp_msg ("select() succeeded");                                      // Streamline SDH 16-Sep-2008
   } else {
      sprintf (sbuf, "unknown select() error: %d", tcperrno);               // Streamline SDH 16-Sep-2008
      disp_msg (sbuf);                                                      // Streamline SDH 16-Sep-2008
      rc = 0 ;
   }

   return rc ;
}

static int ReadSocket ( CLIENT *client )
{
   int rc ;
   int iMessLen;                                                                           // BMG 17-12-2009
   int iFragPoint;                                                                         // BMG 17-12-2009
   int iFullRead;                                                                          // BMG 17-12-2009
   BYTE TempBuff[BUFFER_SIZE];                                                             // BMG 17-12-2009

   sprintf (sbuf, "ReadSocket: SocketHandle = %d, BufferSize = %d",         // Streamline SDH 16-Sep-2008
                client->SocketHandle, client->BufferSize);
   disp_msg (sbuf);                                                         // Streamline SDH 16-Sep-2008

   //rc = recv ( client->SocketHandle, client->Buffer, client->BufferSize, 0 ) ;           // BMG 17-12-2009 Replaced with routine below

   // New message handling socket read                                                     // BMG 17-12-2009
   iFullRead = 1;
   if (client->Continuation) {
       //We are expecting more data from previous request so
       //see if the incoming is the rest of the previous message
       rc = recv ( client->SocketHandle, TempBuff, client->BufferSize, MSG_PEEK );
       if (rc > 0) {
           if (TempBuff[0] == 0xFF) {
               //Got a new message so nothing we can do but process this message below
               client->Continuation = 0;
               client->NewFormMess = 0;
               client->MessageLen = 0;
           } else {
               //Add the new fragment onto the previous message.
               sprintf (sbuf, "Fragmented message - got more of a message on this socket"); disp_msg (sbuf);
               iFragPoint = client->MessageLen;
               rc = recv ( client->SocketHandle, client->Buffer+iFragPoint, rc, 0 ) ;
               rc+=client->MessageLen;
               client->MessageLen = rc;
               //Check to see if we now have the whole message
               iMessLen = satoi(client->Buffer+1, 4);
               if (iMessLen == client->MessageLen) {
                   client->Continuation = 0;
               } else rc = 0;
               //Don't do socket message read again
               iFullRead = 0;
           }
       }
   }
   if (iFullRead) {
       rc = recv ( client->SocketHandle, client->Buffer, client->BufferSize, 0 ) ;
       if (rc > 5) { //no point testing if less than 5 - just let it through as not new style messages
           if (client->Buffer[0] == 0xFF) {
               //Got new style message so validate message length
               client->NewFormMess = 1;
               iMessLen = satoi(client->Buffer+1, 4);
               if (iMessLen != rc) {
                   sprintf (sbuf, "*** BAD message length. Expected %i, got %i", iMessLen, rc); disp_msg (sbuf);
                   dump((BYTE *)client->Buffer, rc);
                   if (iMessLen > rc) {
                       //More of the message to come (fragmented message) so set up pointers and wait for more data
                       sprintf (sbuf, "Fragmented message - wait for more of the message"); disp_msg (sbuf);
                       client->Continuation = 1;
                       client->MessageLen = rc;
                       rc = 0;
                   } else {
                       //Too much data so create an ANK response
                       sprintf (sbuf, "Message too long - converting to ANK/NAK"); disp_msg (sbuf);
                       sprintf(client->Buffer+1, "0045ANKInvalid Message - Too long           ");
                       rc = 45;
                       client->MessageLen = rc;
                   }
               } //Message good so let through
           } else client->NewFormMess = 0; // else just let existing message type through
       } else client->NewFormMess = 0;
   }

   sprintf (sbuf, "recv(): rc=%d, TCPERRNO=%d", rc, tcperrno);              // Streamline SDH 16-Sep-2008
   disp_msg (sbuf);                                                         // Streamline SDH 16-Sep-2008

   if (rc <= 0) {
      // we failed, test error
      if ( (rc == 0 && client->Continuation == 0) || ( rc < 0 && tcperrno != EWOULDBLOCK ) ) { // BMG 17-12-2009 rc will be zero if more data to come!

         /* mark failed for return */
         client->SocketState = ERROR_STATE ;

         if (rc == 0) {
             sprintf(sbuf, "Client (handle=%d) requested disconnect", client->SocketHandle);
         } else {
             sprintf(sbuf, " Socket (handle=%d) marked for Disconnect after recv() failed",
                     client->SocketHandle);
         }
         disp_msg(sbuf);                                                    // Streamline SDH 16-Sep-2008

      }
   } else {
      // success
      // Set message length - not set up before for some reason             //BMG 1.1 12/09/2008
      client->MessageLen = rc;                                              //BMG 1.1 12/09/2008
   }

   return rc ;
}

static int WriteSocket ( CLIENT *client )
{
   int rc ;

   rc = send ( client->SocketHandle, client->Buffer, client->MessageLen, 0 ) ;
   if (rc < 0) {
      /* we failed because of an error*/
      sprintf (sbuf, "send() failed rc=%d, TCPERRNO=%d", rc, tcperrno);     // Streamline SDH 16-Sep-2008
      disp_msg (sbuf);

      /* mark failed for return */
      client->SocketState = ERROR_STATE ;

      sprintf(sbuf, " Socket (handle=%d) marked for Disconnect after send() failed", // Streamline SDH 16-Sep-2008
                   client->SocketHandle);
      disp_msg (sbuf);                                                      // Streamline SDH 16-Sep-2008
   }

   // clear down the buffer
   memset ( client->Buffer, 0, client->BufferSize ) ;

   return rc ;
}

static void ProcessPendingConnects ( sock_handle *SockHandle )
{
   static int index = -1 ; // initalise to 'not assigned'
   int result ;

   //LogMessage ( 10, "Processing pending connects" ) ;

   do {
      // get a new slot if the memory for one is not already allocated
      if ( index < 0 ) {
         index = GetFreeClientSlot () ;
      }

      if ( index >= 0 ) {
         // Got an assigned slot. Try to accept on the socket
         //disp_msg ("About to attempt AcceptOnSocket");                    // Streamline SDH 16-Sep-2008
         result = AcceptOnSocket ( SockHandle, Clients[index] ) ;
         if ( result >= 0 ) {
            // Conncetion was accepted. Check if this HHT (IP address)
            // is already logged on and if so, use the old connection instead
            index = ClearOldClientConnection ( index ) ;                    //SDH 16-05-2005

            // record who it is, for when we have
            // to tell Boots app the HH_UNIT ID
            Clients[index]->HhtId = index ;

            index = -1 ; // we'll need a new ClientSlot next time
         }
         //else
         //{
         //    // accept did not have anyone awaiting connects or accept failed
         //    ReleaseClientSlot ( index ) ;
         //}
      } else {
         result = 0 ; // no more free Client slots available
      }
   }
   while ( result >= 0 ) ;
}

static void DummyCallToProcess ( void )
{
    char Buffer[BUFFER_SIZE];                                    // Streamline SDH 21-Sep-2008
    //RBSCommandStruct *RbsCmd;
    int SizeOfCmd = 3;                                           // Streamline SDH 21-Sep-2008

    //RbsCmd = ( RBSCommandStruct * ) Buffer;                    // Streamline SDH 21-Sep-2008
    // setup dummy command
    //memcpy( RbsCmd->TID, RF_HHT_MSG_RBS_RESERVED, RF_MESSAGE_TID_LENGTH );
    memcpy( Buffer, "XXX", 3);                                   // Streamline SDH 21-Sep-2008
    //RbsCmd->Command = -1; // dummy, invalid command            // Streamline SDH 21-Sep-2008

    // issue it to hook into Boots code
    CurrentHHT = -1 ; // setup for process()
    //process( ( char * ) RbsCmd , &SizeOfCmd);
    process(Buffer, &SizeOfCmd);                                 // Streamline SDH 21-Sep-2008
}

static void ProcessPendingReceives ( void )
{
   int i, j, rc ;
   int sock_read  [MAX_SOCKETS] = { 0} ;
   int sock_index [MAX_SOCKETS] = { 0} ;

   // Check the Client structure for sockets who can
   // receive and build a handles array of those who can

   // Set to test for reads on primary socket
   sock_read [0] = PrimarySocketHandle ;
   sock_index [0] = 0 ;

   // Set up child sockets
   for ( j = 1, i = 0 ; i < MAX_SOCKETS ; i++ ) {
      if ( Clients[i] != ( void * ) NULL ) {
         if ( Clients[i]->InUse == 1 &&
              Clients[i]->SocketHandle >= 0 &&
              Clients[i]->SocketState == AWAITING_INPUT ) {
            // add the socket handle on which we expect a read to our array
            sock_read[j] = Clients[i]->SocketHandle ;
            // record the index in parallel
            sock_index[j++] = i ;
         }
      }
   }

   //LogMessage( 7, "Awaiting Receive from %d clients", j ) ;
   // printf( "Awaiting Receive from %d clients\n", j ) ;

   // setup multiplexed test to find which sockets are ready to be read
   rc = SocketGroupStatus ( &sock_read[0], j, STATUS_READ ) ;

   // printf ( "Select ended rc = %d\n", rc ) ;

   if ( sock_read[0] != -1 ) {
      ProcessPendingConnects ( &PrimarySocketHandle ) ;
   } else {
      if ( rc > 0 ) { // the group status check shows 1 or more sockets to read
         // process all the ready sockets
         for ( i = 1 ; i < j ; i++ ) {
            if ( sock_read[i] != -1 ) {

               //LogMessage( 10, "Socket Handle %d is ready to be read", i );

               // found socket ready with data, so read it
               rc = ReadSocket ( Clients[sock_index[i]] ) ;

               // we've got some data from the client, so process it
               // ...and set state 'awaiting send to client'
               if ( rc > 0 ) {

                  if (Clients[sock_index[i]]->ApplicationType>0) {                      // BMG 15-11-2010
                      //Got a non-client type response                                  // BMG 15-11-2010
                      //Match up type - only one at present!                            // BMG 15-11-2010
                      switch (Clients[sock_index[i]]->ApplicationType) {                // BMG 15-11-2010
                                                                                        // BMG 15-11-2010
                      case 1:       //DEC Server Response                               // BMG 15-11-2010
                          ProcessDECResponse( Clients[sock_index[i]]->Buffer,           // BMG 15-11-2010
                               &( Clients[sock_index[i]]->MessageLen ) );               // BMG 15-11-2010
                          break;                                                        // BMG 15-11-2010
                      }                                                                 // BMG 15-11-2010
                  } else {                                                              // BMG 15-11-2010

                     CurrentHHT = Clients[sock_index[i]]->HhtId ; // setup for process()// Streamline SDH 16-Sep-2008
                     process ( Clients[sock_index[i]]->Buffer,                          // Streamline SDH 16-Sep-2008
                               &( Clients[sock_index[i]]->MessageLen ) ) ;              // Streamline SDH 16-Sep-2008
                     Clients[sock_index[i]]->SocketState =  AWAITING_OUTPUT ;           // Streamline SDH 16-Sep-2008
                  }                                                                     // BMG 15-11-2010
               }

            }
            //else {  // Commented out Defect 4191                                   // CSk 30-11-2010 Recall Improvements
            //   sprintf (sbuf, "Client %d (SocketHandle=%d) failed on READ select()", // Streamline SDH 16-Sep-2008
            //            sock_index[i], Clients[sock_index[i]]->SocketHandle );
            //   disp_msg (sbuf);                                             // Streamline SDH 16-Sep-2008
            //}
         }
      }
   }
}

static void ProcessPendingSends(void)
{
   int i, j, rc;
   int sock_write[MAX_SOCKETS], sock_index[MAX_SOCKETS];

   // Check the Client structure for sockets to whom
   // we have to send and build a handles array of these
   for (j = 0, i = 0; i < MAX_SOCKETS; i++) {
      if (Clients[i] != NULL) {
         // we have to send an ACK _before_ we disconnect
         // so we still send even if we are awaiting disconnect.
         if (Clients[i]->InUse == 1 &&
             Clients[i]->SocketHandle >= 0 &&
             (Clients[i]->SocketState == AWAITING_OUTPUT ||
              Clients[i]->SocketState == AWAITING_DISCONNECT)) {
            // add socket handle on which we expect a read to our array
            sock_write[j] = Clients[i]->SocketHandle ;
            // record the index in parallel
            sock_index[j++] = i ;
         }
      }
   }

   if (j > 0 ) {
      //LogMessage( 9, "Awaiting Send to %d clients", j ) ;
      // setup multiplexed test to find which sockets are ready to be read
      rc = SocketGroupStatus(sock_write, j, STATUS_WRITE);

      //if (rc > 0)
      {
         // process all the ready sockets
         for (i = 0; i < j; i++) {
            if (sock_write[i] != -1) {
               //LogMessage( 10, "Socket Handle %d is ready to be written", i );

               // socket ready, so write to it whatever we've prepared
               rc = WriteSocket(Clients[sock_index[i]]);

               if (rc > 0 &&
                   Clients[sock_index[i]]->SocketState == AWAITING_OUTPUT) {
                  // ...and set state to 'awaiting message from client'
                  Clients[sock_index[i]]->SocketState = AWAITING_INPUT;
               }
            } else {
               sprintf (sbuf, "Client %d (SocketHandle=%d) failed on select() WRITE", // Streamline SDH 16-Sep-2008
                        sock_index[i], Clients[sock_index[i]]->SocketHandle );
               disp_msg (sbuf);                                             // Streamline SDH 16-Sep-2008
            }
         }
      }
   }
}

static void ProcessPendingDisconnects ( void )
{
   int i ;

   // Check the Client structure for sockets we can disconnect immediately
   for (i = 0; i < MAX_SOCKETS; i++) {
      if (Clients[i] != NULL) {
         // look for sockets awaiting disconnect.
         if ( Clients[i]->InUse == 1 &&
              Clients[i]->SocketHandle >= 0 &&
              Clients[i]->SocketState == ERROR_STATE ) {
            sprintf (sbuf, "Disconnecting Client %d (SocketHandle=%d)...",  // Streamline SDH 16-Sep-2008
                        i, Clients[i]->SocketHandle );
            disp_msg (sbuf);                                                // Streamline SDH 16-Sep-2008
            CloseSocket( &(Clients[i]->SocketHandle));
            ReleaseClientSlot(i);
            //LogMessage( 7, "...Client %d Disconnected!", i);
         }
      }
   }
}

static int GetFreeClientSlot(void)
{
   int found, i;

   //LogMessage( 10, "In GetFreeClientSlot");

   // Check the Client structure for any empty slots
   for (found = 0, i = 1; i < MAX_SOCKETS; i++) { // BMG 15-11-2010 Changed to search from 1 because 0 is DEC entry.
      if (Clients[i] == NULL) {
         // found an unused slot so assign memory to it
         Clients[i] = (CLIENT *) AllocateBuffer ( sizeof ( CLIENT ) ) ;
         memset(Clients[i], 0, sizeof(CLIENT)) ;

         // allocate a default size for the buffer
         Clients[i]->Buffer = ( char * ) AllocateBuffer(BUFFER_SIZE) ;
         memset(Clients[i]->Buffer, 0, BUFFER_SIZE);

         //LogMessage( 10, "Client slot allocation successful" ) ;

         if (Clients[i] != NULL) {
            Clients[i]->BufferSize = BUFFER_SIZE;
            found = 1; // mark it assigned
            //LogMessage( 10, "Buffer Size for allocated Client = %d",
            //            Clients[i]->BufferSize);
         }

         // we've finished, so break out of loop
         break;
      }
   }
   return (found ? i : -1); // if found then i else -1
}

static int ClearOldClientConnection(int SlotIndex)                          //SDH 16-05-2005
{
   int i, rc;
   struct sockaddr_in *addr1;
   struct sockaddr_in *addr2;

   // work out IP address of New connection
   addr1 = (struct sockaddr_in *) &(Clients[SlotIndex]->SocketAddress);

   if (debug) {                                                //SDH 16-05-2005
        sprintf(msg, "New connection from IP address: "        //SDH 16-05-2005
                "%d.%d.%d.%d", *((char*)(&addr1->sin_addr)),   //SDH 16-05-2005
                *((char*)(&addr1->sin_addr)+1),                //SDH 16-05-2005
                *((char*)(&addr1->sin_addr)+2),                //SDH 16-05-2005
                *((char*)(&addr1->sin_addr)+3));               //SDH 16-05-2005
        disp_msg(msg);                                         //SDH 16-05-2005
   }                                                           //SDH 16-05-2005

   // we are going to cycle round the clients to see if the
   // IP address of the new connection matches any of the old clients
   for (i = 0; i < MAX_SOCKETS; i++) {
      if (Clients[i] != NULL) {
         // In use and not the new one
         if (Clients[i]->InUse == 1 && i != SlotIndex) {
            addr2 = (struct sockaddr_in *) &(Clients[i]->SocketAddress);

            rc = memcmp((char *) &(addr2->sin_addr),
                        (char *) &(addr1->sin_addr),
                        4);

            if (rc == 0) { // Same HHT (IP addr) has tried to log on again

                if (debug) {                                                //SDH 16-05-2005
                    sprintf(msg, "HHT previously on slot:%d handle:%d!",    //SDH 16-05-2005
                           i, Clients[i]->SocketHandle);                    //SDH 16-05-2005
                    disp_msg(msg);                                          //SDH 16-05-2005
                }                                                           //SDH 16-05-2005

                //We're going to use the old slot rather than the new.      //SDH 16-05-2005
                //Close the old socket, replace the socket address in the   //SDH 16-05-2005
                //old slot, then delete the new slot                        //SDH 16-05-2005
                CloseSocket(&(Clients[i]->SocketHandle));                   //SDH 16-05-2005
                Clients[i]->SocketAddress = Clients[SlotIndex]->SocketAddress;//SDH 16-05-2005
                Clients[i]->SocketHandle = Clients[SlotIndex]->SocketHandle;//SDH 16-05-2005
                Clients[i]->SocketState = Clients[SlotIndex]->SocketState;  //SDH 16-05-2005
                ReleaseClientSlot(SlotIndex);                               //SDH 16-05-2005
                return i;                                                   //SDH 16-05-2005

               /* //SDH 16-05-2005
               if ( Clients[i]->SignedOn == 1 ) {
                  // Tell Boots app to tidy up its structures for old unit...
                  IssueCmdOffToBootsApp(i);
                  // and turn of the Signed On flag
                  Clients[i]->SignedOn = 0;
               }

               // now shut down the old socket
               CloseSocket(&(Clients[i]->SocketHandle));
               ReleaseClientSlot(i);
               LogMessage( 6, "Closed slot %i" );
               */ //SDH 16-05-2005

            }
         }
      }
   }

   if (debug) disp_msg("New IP address");                                   //SDH 16-05-2005

   //Nothing found - return new slot                                        //SDH 16-05-2005
   return SlotIndex;                                                        //SDH 16-05-2005

}


////////////////////////////////////////////////////////////////////////////////
///
///   PUBLIC FUNCTIONS
///
////////////////////////////////////////////////////////////////////////////////

int TcpLoaded ( void )
{
    /* check if the protocol stack is loaded
    return TRUE if loaded
    return FALSE otherwise
    */
    /* initialize with sockets */
    return sock_init();
}

void InitialiseSocketServer ( void )
{
   int i ;

   // initialise the Client structure
   for ( i = 0 ; i < MAX_SOCKETS ; i++ ) {
      // mark all client slots as not allocated
      Clients[i] = NULL ;
   }

   // allocate socket
   if ( AllocateSocket ( &PrimarySocketHandle ) < 0 ) {
      disp_msg ("Socket Call Failed");                                      // Streamline SDH 16-Sep-2008
      exit (1);
   }

   // bind address/port to socket
   if ( BindSocket ( &PrimarySocketHandle, HHTCOMMS_PORT ) < 0 ) {          // SDH 19-05-2005
      disp_msg ("Bind Call Failed");                                        // Streamline SDH 16-Sep-2008
      exit (1);
   }

   if ( ListenOnSocket ( &PrimarySocketHandle ) < 0 ) {
      disp_msg ("Listen Call Failed");                                      // Streamline SDH 16-Sep-2008
      exit (1);
   }

   // if we got here, the socket setup succeeded
   disp_msg ("Socket Server Initialised and Ready");                        // Streamline SDH 16-Sep-2008

}

void SocketServerLoop(void)
{

//   int key;
//   clock_t lCurrentTime = 0 ;
//   clock_t lPreviousTime = 0 ;
   LONG lCurrentTime=0L, lPreviousTime=0L;
   TIMEDATE now;

   // loop forever, waiting for:
   //        i.  Anyone to disconnect
   //       ii.  New connections
   //      iii.  Any Receives
   //       iv.  Anything to send
   // no calls in the loop will block
   // N.B: The order in which ProcessPendingXXXX funcs are called is important!
   for (;;) {
      // anyone already connected tried to break connection?
      ProcessPendingDisconnects () ;

      // has anyone new attempted connection?
      // ProcessPendingConnects ( &PrimarySocketHandle ) ;

      // anyone already connected tried to talk to us?
      ProcessPendingReceives () ;

      // reply to anyone already connected
      ProcessPendingSends () ;

/*
      if ( BreakLoop != 0 ) {
         // wait...
         disp_msg("Press ESC to END or any key to continue...");            // Streamline SDH 16-Sep-2008
         key = getchar () ;
         if ( key == 27 ) {
            CtlBrkHandler () ;
            break;
         }
      }
*/

      // call into Boots Code every POLL_FREQ mS
      // also reload control file if flag set
      // also check if HHT logs need recycling

//      lCurrentTime = clock () / CLOCKS_PER_SEC ;
      s_get( T_TD, 0L, (void *)&now, TIMESIZE );
      lCurrentTime = now.td_time;

      if ( ( ( lPreviousTime + POLL_FREQ ) < lCurrentTime ) ||
           ( lCurrentTime < lPreviousTime ) ) {
         static int iStoreCloseCheck = 0 ;

         if ( iStoreCloseCheck++ % 4 == 0 ) {
            long lSTKCFHandle = 0 ;

            lSTKCFHandle = OpenDirectFile (STKCF, STKCF_OFLAGS, STKCF_REP, TRUE);    //SDH

            if ( lSTKCFHandle ) {
               char pBuff[12] ;
               //int rc = 0 ;

               //rc = s_read ( 0, lSTKCFHandle, pBuff, sizeof pBuff, 0L ) ;
               s_read ( 0, lSTKCFHandle, pBuff, sizeof pBuff, 0L ) ;

               if ( pBuff[10] == 'N' ) {
                  CloseAllFiles () ;
                  cStoreClosed = 'Y' ;
                  background_msg("RFS - Closed");
               } else {
                  sprintf(msg,"RFS - Ready and Waiting... - Online: %02d",sess);
                  background_msg(msg);                               // 25-8-2004 PAB
                  cStoreClosed = 'N' ;
               }

               s_close ( 0, lSTKCFHandle ) ;
            } else {
               disp_msg ("Could not open STKCF");                           // Streamline SDH 16-Sep-2008
            }
         }

         //disp_msg ("Dummy call to Boots code");                             // Streamline SDH 16-Sep-2008
         // call into Boots code
         DummyCallToProcess () ;

         // re-parse control file
         //if ( PollControlFile == 'Y' ) {
         //   ParseControlFile () ;
         //}

         /* SDH - 26-11-2004  Redundant code
         // cycle HHT logs
         if ( DateHasChanged () ) {
            CycleHhtLogs () ;
         }
         */

//         lPreviousTime = clock () / CLOCKS_PER_SEC ;

      }

      lPreviousTime = lCurrentTime;

      // add a delay using system calls so that OS gives other processes
      // a chance ( LoopDelay controlled by control file parameter )
      //DelayProcess ( ( long ) LoopDelay ) ;


      //lEndTime = clock () ;

      //lElapsedTime = lEndTime - lStartTime ;

      //LogMessage ( 10, "Loop time  %ld    \n", lElapsedTime ) ;

      //lStartTime = clock () ;
   }
}


void ShutDownAllSockets(void) {

    //Remember, client 0 is the DEC connection                           // SDH 8-Dec-2011
    disp_msg ("Shutting down all sockets...");                           // Streamline SDH 16-Sep-2008
    for (int i = 0; i < MAX_SOCKETS; i++) {                              // SDH 8-Dec-2011
       if (Clients[i] != NULL) {
          if (Clients[i]->InUse == 1) {
             CloseSocket(&(Clients[i]->SocketHandle));
          }
       }
    }

    // Close listener                                                    // SDH 8-Dec-2011
    CloseSocket(&PrimarySocketHandle);

}


int ConnectDECSocket(struct sockaddr_in DECaddr){ // BMG 15-11-2010

    int rc;
    int rc2;

    rc = connect(DECSocketHandle, (struct sockaddr*)&DECaddr, sizeof(DECaddr));
    if (rc==-1 && tcperrno==EINPROGRESS) {
        //Connection in progress
        //sock_read[0] = DECSocketHandle;
        rc2 = select(&DECSocketHandle, 0, 1, 0, 2000);
        if (rc2==0) {
            sprintf (sbuf, "Primary DEC connect timed out on handle with error %d", tcperrno);
            disp_msg(sbuf);
            //soabort(DECSocketHandle);
            //soclose(DECSocketHandle);
            CloseSocket (&DECSocketHandle);                             // SDH 8-Dec-2011
            rc=-1;
        } else {
            if (rc2==-1) {
                sprintf (sbuf, "DEC connect failed with error %d", tcperrno);
                disp_msg(sbuf);
                //soabort(DECSocketHandle);
                //soclose(DECSocketHandle);
                CloseSocket (&DECSocketHandle);                         // SDH 8-Dec-2011
                rc=-1;
            } else {
                //Try a connect again to ensure we really do have a connection
                //This is because the select above will still return a 1 even though it is not connected!
                rc = connect(DECSocketHandle, (struct sockaddr*)&DECaddr, sizeof(DECaddr));
                if (rc==-1 && tcperrno==EISCONN) {
                    //Returned already connected so we are good
                    rc=0;
                } else rc=-1;
            }
        }
    }

    return rc;
}

int InitialiseDECSocket ( int recon ) // BMG 15-11-2010
{
    struct sockaddr_in DECaddr ;
    URC urc;
    LONG lrc;
    int rc;
    int i, s;
    unsigned int port = 0;
    long unsigned int first_sopts_addr = 0;
    long unsigned int sec_sopts_addr = 0;
    long unsigned int prim_addr = 0;
    long unsigned int sec_addr = 0;

    if (recon) {
        //Force close the socket when attempting reconnects
        //soabort(DECSocketHandle);
        //soclose(DECSocketHandle);
        CloseSocket (&DECSocketHandle);                                 // SDH 8-Dec-2011
        if ( Clients[0] != (void *) NULL ) {Clients[0]->SocketState = UNCONNECTED;}
    } else {

        //Set the Node ID
        memset(node, 0, 2);
        rc = get_node_ID(node);
        if (rc!=0) {
            if (debug) disp_msg("ERROR obtaining Node ID from ADXSERVE");
            return -1;
        }

        //Load in DECCONF
        urc = open_decconf();
        if (urc) {
            if (debug) disp_msg("ERROR Unable to open DECCONF - check event logs");
            return -1;
        } else {
            if (debug) disp_msg("RD DECCONF");
            for (i=0;i<100;i++) {
                lrc = ReadDecconf(i, __LINE__);
                if (lrc <= 0L) {
                    if (i==0) {
                        if (debug) disp_msg("ERROR Unable to read DECCONF - file is empty - check event logs");
                        close_decconf(CL_ALL);
                        return -1;
                    } else {
                        decconfentries=i;
                        i=100;
                    }
                } else {
                    memcpy(decconftab[i].abMessageID,decconfrec.abMessageID, sizeof(decconfrec.abMessageID));
                    memcpy(decconftab[i].abMessageName,decconfrec.abMessageName, sizeof(decconfrec.abMessageName));
                    decconftab[i].bWritetype = decconfrec.bWritetype;
                }
            }
            close_decconf(CL_ALL);
        }
    }

    urc = open_ealsopts();
    if (urc) {
        if (debug) disp_msg("ERROR Unable to open EALSOPTS - check event logs");
        return -1;
    } else {
        if (debug) disp_msg("RD EALSOPTS");
        lrc = ReadEalsopts(155, __LINE__); //Actually SOPTS record 156 but runs from 0 in c
        if (lrc <= 0L) {
            close_ealsopts(CL_ALL);
            return -1;
        } else {
            close_ealsopts(CL_ALL);
            s=0;
            //Extract port number by finding the first comma
            memset(sbuf, 0x00, sizeof(sbuf));
            for (i=0;i<ealsopts.wRecLth;i++ ) {
                if ( ealsoptsrec.abRecord[i] == ',') {
                    memcpy(sbuf, ealsoptsrec.abRecord+s, i-s);
                    port = satoi(sbuf,i-s);
                    s=i+1;
                    i=ealsopts.wRecLth;
                }
            }
            //Extract first IP address by finding the next comma
            memset(sbuf, 0x00, sizeof(sbuf));
            for (i=s;i<ealsopts.wRecLth;i++ ) {
                if ( ealsoptsrec.abRecord[i] == ',') {
                    memcpy(sbuf, ealsoptsrec.abRecord+s, i-s);
                    first_sopts_addr=inet_addr(sbuf);
                    //See if this address matches a File Server in a twin (IP ends in 28) and if so set primary address
                    if (ealsoptsrec.abRecord[i-2]=='2' && ealsoptsrec.abRecord[i-1]=='8') {
                        prim_addr=first_sopts_addr;
                    }
                    s=i+1;
                    i=ealsopts.wRecLth;
                }
            }
            //Extract secondary IP address by finding the next comma
            memset(sbuf, 0x00, sizeof(sbuf));
            for (i=s;i<ealsopts.wRecLth;i++ ) {
                if ( ealsoptsrec.abRecord[i] == ',') {
                    memcpy(sbuf, ealsoptsrec.abRecord+s, i-s);
                    sec_sopts_addr=inet_addr(sbuf);
                    //If primary address not set, see if this address is a File Server in a twin (IP ends in 28) and if so set primary address
                    if (prim_addr==0 && ealsoptsrec.abRecord[i-2]=='2' && ealsoptsrec.abRecord[i-1]=='8') {
                        prim_addr=sec_sopts_addr;
                    }
                    s=i+1;
                    i=ealsopts.wRecLth;
                }
            }
            //Now test for the translation flag
            if (ealsoptsrec.abRecord[s] == '-' && ealsoptsrec.abRecord[s+1] == '1') {
                dectranslation = 1;
            } else {
                dectranslation = 0;
            }

            //Now ensure primary and secondary IP's are set up
            //Need to ensure TRANSACT looks at the File Server's DEC in a twin as much as possible
            if (prim_addr==first_sopts_addr) {
                    sec_addr=sec_sopts_addr;
            } else {
                if (prim_addr==sec_sopts_addr) {
                    sec_addr=first_sopts_addr;
                } else {
                    prim_addr = first_sopts_addr;
                    sec_addr=sec_sopts_addr;
                }
            }

            //Set up Client table entry
            if ( Clients[0] == (void *) NULL ) {
                Clients[0] = (CLIENT *) AllocateBuffer ( sizeof ( CLIENT ) );
                memset(Clients[0], 0, sizeof(CLIENT));
                Clients[0]->InUse = 0;
                Clients[0]->HhtId = 0;
                Clients[0]->SocketState = UNCONNECTED;
                // allocate a default size for the buffer
                Clients[0]->Buffer = ( char * ) AllocateBuffer(BUFFER_SIZE);
                memset(Clients[0]->Buffer, 0, BUFFER_SIZE);
                if (Clients[0] != NULL) {
                    Clients[0]->BufferSize = BUFFER_SIZE;
                }
                Clients[0]->MessageLen = 0;
                Clients[0]->NewFormMess = 0;
                Clients[0]->Continuation = 0;
                Clients[0]->ApplicationType = 1;
            }

            //Try a connection to the primary IP
            if ( AllocateDECSocket ( &DECSocketHandle ) < 0 ) {
                if (debug) disp_msg ("DEC Socket Call Failed");
                return -1;
            }
            memset(&DECaddr, 0, sizeof(DECaddr));
            DECaddr.sin_family = AF_INET;
            DECaddr.sin_port = BSWAP(port);
            DECaddr.sin_addr.s_addr = prim_addr;
            rc=ConnectDECSocket(DECaddr);

            if (rc==-1) {
                //Try secondary IP
                //soabort(DECSocketHandle);
                //soclose(DECSocketHandle);
                CloseSocket (&DECSocketHandle);                         // SDH 8-Dec-2011
                DECSocketHandle = 0;
                //Get another handle
                if ( AllocateDECSocket ( &DECSocketHandle ) < 0 ) {
                    if (debug) disp_msg ("DEC Socket Call Failed");
                    return -1;
                }
                memset(&DECaddr, 0, sizeof(DECaddr));
                DECaddr.sin_family = AF_INET;
                DECaddr.sin_port = BSWAP(port);
                DECaddr.sin_addr.s_addr = sec_addr;
                rc=ConnectDECSocket(DECaddr);
                if (rc==-1) {
                    //soabort(DECSocketHandle);
                    //soclose(DECSocketHandle);
                    CloseSocket (&DECSocketHandle);                     // SDH 8-Dec-2011
                    DECSocketHandle = 0;
                    return rc;
                }
            }
            Clients[0]->InUse = 1;
            Clients[0]->SocketHandle = DECSocketHandle;
            Clients[0]->SocketState = AWAITING_INPUT;
        }
    }

    return 0;
}

ULONG SendToDEC (char* msgID, int msgIdSize, char* payload, int nbytes) // BMG 15-11-2010
{
    int rc=0;
    int i, j;
    BYTE bWritetype;
    BYTE abMessageName[sizeof(decconfrec.abMessageName)+1];
    URC urc;
    LONG lrc;
    LONG lDQfile;
    ULONG UniqueID;
    LONG sec, day, month, year;
    WORD hour, min;
    BYTE temp[14];
    BYTE *pBuffer = NULL;
    int buffsize=0;
    BYTE DQFile[10];

    if (debug) disp_msg("Received a DEC message request");

    //Match msgID against DECCONF table to determine real-time or queue
    bWritetype = ' ';
    for (i=0;i<decconfentries;i++) {
        if (memcmp(decconftab[i].abMessageID, msgID,msgIdSize)==0 ) {
            memcpy(abMessageName, decconftab[i].abMessageName, sizeof(decconfrec.abMessageName));
            abMessageName[sizeof(decconfrec.abMessageName)] = '\0';
            bWritetype = decconftab[i].bWritetype;
            i=decconfentries;
        }
    }
    if (bWritetype==' ') return -1; //Drop out as there is no matching DECCONF table entry
    //Strip out trailing spaces from MessageName
    for (i=sizeof(decconfrec.abMessageName)-1; i>0;i--) {
        if (abMessageName[i]!= ' ') {
            abMessageName[i+1] = '\0';
            i=0;
        }
    }

    //Determine buffer space required - different sizes depending on if translation is required
    j=0;
    if (dectranslation) { //Translation required
        //Pre-read input buffer to determine how many translations needed and work out how much extra buffer space is required
        for (i=0;i<nbytes;i++) {
            if (payload[i]=='"') j+=6;
            if (payload[i]==',') j+=7;
            if (payload[i]=='&') j+=5;
            if (payload[i]=='\r') {
                if ((payload[i+1]=='\n')) j+=7;
            }
        }
    }
    //Allocate buffer space - add extra to accomodate unique ID, message ID and CR/LF, etc. if needed
    pBuffer = AllocateBuffer(nbytes+50+j);
    memset(pBuffer, 0, nbytes+50+j);

    if (dectranslation) {
        if (debug) disp_msg("Performing DEC message translation");
        buffsize=0;
        j=0;
        for (i=0;i<nbytes;i++) {

            switch(payload[i]){

            case '&': memcpy(pBuffer+j, "&amp;", 5);
                      j+=5;
                      break;
            case '"': memcpy(pBuffer+j, "&quot;", 6);
                      j+=6;
                      break;
            case '|': memcpy(pBuffer+j, "&pipe;", 6);
                      j+=6;
                      break;
            case ',': memcpy(pBuffer+j, "&comma;", 7);
                      j+=7;
                      break;
            case '\r': if (payload[i+1]=='\n') {
                           memcpy(pBuffer+j, "&crtlf;", 7);
                           j+=7;
                           i++; //Move past both translated characters!
                       } else {
                           pBuffer[j]=payload[i];
                           j++;
                       }
                       break;
            default: pBuffer[j]=payload[i];
                     j++;
                     break;
            }
        }
        buffsize=j;
    } else {
        //No translation so just copy in the payload
        buffsize=nbytes;
        memcpy(pBuffer, payload, buffsize);
    }

    UniqueID=0;
    if (bWritetype == 'S') { //Write to socket

        //Ensure we are connected to the DEC
        if ( Clients[0] == (void *) NULL ) {
            rc = InitialiseDECSocket(1);
            if (rc == -1) return rc;
        } else {
            if (Clients[0]->SocketState == UNCONNECTED) {
                rc = InitialiseDECSocket(1);
                if (rc == -1) return rc;
            }
        }

        //Obtain unique ID
        urc = open_rfscache();
        if (urc) {
            if (debug) disp_msg("ERROR Unable to open RFSCACHE - check event logs");
            rc=-1;
        } else {
            if (debug) disp_msg("RD RFSCACHE");
            memset(rfscachehomerec.abkey, 0xFF, sizeof(rfscachehomerec.abkey));
            lrc = ReadRFSCacheHomeLog(__LINE__, LOG_CRITICAL);
            if (lrc > 0L) {
                UniqueID = rfscachehomerec.ulNextMessNum;
                UniqueID++;
                if (UniqueID>3999999999) UniqueID=1;
                rfscachehomerec.ulNextMessNum = UniqueID;
                if (debug) disp_msg("WR RFSCACHE");
                lrc = WriteRFSCacheHome(__LINE__);
                if (lrc <= 0L) {
                    if (debug) disp_msg("ERROR Unable to write RFSCACHE home record - check event logs");
                    close_rfscache(CL_ALL);
                    rc=-1;
                }
            } else {
                if (debug) disp_msg("ERROR Unable to read RFSCACHE home record - check event logs");
                close_rfscache(CL_ALL);
                rc=-1;
            }
        }

        //Insert MessageName and Unique ID into the start of the payload
        sprintf(sbuf, "%s|%lu,\0", abMessageName,UniqueID);
        j=strlen(sbuf);
        //Shift everything up j bytes working backwards
        for (i=buffsize-1;i>=0;i--) {
            pBuffer[i+j]=pBuffer[i];
        }
        memcpy(pBuffer, sbuf, j);
        buffsize+=j;
        //Add a CR/LF to the end of the payload.
        memset(pBuffer+buffsize, 0x0d, 1);
        memset(pBuffer+buffsize+1, 0x0a, 1);
        buffsize+=2;

        if (debug) {
            sprintf(sbuf, "Sending message to the DEC Socket - Unique ID:%lu :", UniqueID);
            disp_msg (sbuf);
            dump(pBuffer, buffsize);
        }

        if (rc==0) {
           rc = send(DECSocketHandle, pBuffer, buffsize, 0);
           if (rc==-1) {
               //Failed to send so set to unconnected so it re-initialises on the next send
               Clients[0]->SocketState = UNCONNECTED;
               if (debug) disp_msg ("DEC socket write failed");
           } else {
               //Write RFSCACHE holding record with date/time, etc.
               rc=0;
               sysdate( &day, &month, &year, &hour, &min, &sec );
               sprintf(temp, "%04ld%02ld%02ld%02d%02d%02d", year, month, day, hour, min, sec );
               pack(rfscacherec.abSentDateTime, 6, temp+2, 12, 0);
               rfscacherec.ulID = UniqueID;
               rfscacherec.bSeq = 0;
               rfscacherec.bStatus = 'S';
               memset(rfscacherec.abMessage, 0, sizeof(rfscacherec.abMessage));
               if (debug) disp_msg("WR RFSCACHE");
               lrc = WriteRFSCache(__LINE__);
               if (lrc <= 0L) {
                   if (debug) disp_msg("ERROR Unable to write RFSCACHE home record - check event logs");
                   rc=-1;
               }
               close_rfscache(CL_ALL);
           }
        }
    } else {
        if (bWritetype == 'Q') { //Write to queue file

            //Insert quotes, and MessageName into the start of the payload
            sprintf(sbuf, "\"%s|\0", abMessageName);
            j=strlen(sbuf);
            //Shift everything up j bytes working backwards
            for (i=buffsize-1;i>=0;i--) {
                pBuffer[i+j]=pBuffer[i];
            }
            memcpy(pBuffer, sbuf, j);
            buffsize+=j;
            //Add a quote & CR/LF to the end of the payload.
            memset(pBuffer+buffsize, '"', 1);
            memset(pBuffer+buffsize+1, 0x0d, 1);
            memset(pBuffer+buffsize+2, 0x0a, 1);
            buffsize+=3;

            if (debug) {
                disp_msg ("Writing message to the DEC Queue");
                dump((BYTE *)pBuffer, buffsize);
            }

            //Set up DQ filename and write message
            memcpy(DQFile, "DQ:\0\0.BIN\0", 10);
            memcpy(DQFile+3, node, 2);
            lDQfile = s_open (A_READ | A_WRITE, DQFile);
            if (lDQfile < 0) {
                log_event101(lDQfile, DQ_REP, __LINE__);
                if (debug) {
                    sprintf(msg, "Error opening %s file. Check event logs", DQFile);
                    disp_msg(msg);
                }
                rc=-1;
            } else {
                lrc = s_write( A_FLUSH | A_EOFOFF, lDQfile, pBuffer, buffsize, 0L );
                if (lrc!=buffsize) {
                    log_event101(lDQfile, DQ_REP, __LINE__);
                    if (debug) {
                        sprintf(msg, "Error opening %s file. Check event logs", DQFile);
                        disp_msg(msg);
                    }
                    rc=-1;
                }
                s_close( 0, lDQfile );
            }
        } else {
            if (debug) {
                sprintf(sbuf, "INVALID DEC REQUEST MESSAGE ID:%s", (BYTE *)msgID);
                disp_msg (sbuf);
            }
            rc=-1;
        }
    }
    //Release buffer
    FreeBuffer(pBuffer);
    pBuffer = NULL;

    //Ensure Unique ID is passed back if no failure. This will be 0 for queue messages.
    if (rc==0) return UniqueID;
    return rc;
}

int ProcessDECResponse(char* inbound, int* nbytes) // BMG 15-11-2010
{
    BYTE *pBuffer = NULL;
    int buffsize=0;
    int i, j;
    int in_size;
    int rc;
    URC urc;
    LONG lrc;
    ULONG UniqueID;

    in_size=*nbytes;

    if (debug) disp_msg ("*** Got a DEC response:");


    if (memcmp(inbound,"NAK", 3)==0) {
        //DEC sent back a NAK to a request so this has to be discarded
        return -1;
    }

    //Allocate buffer space - if under one record, ensure it is one record in size
    if (in_size<sizeof(rfscacherec.abMessage)) {
        pBuffer = AllocateBuffer(sizeof(rfscacherec.abMessage));
        memset(pBuffer, 0, sizeof(rfscacherec.abMessage));
    } else {
        pBuffer = AllocateBuffer(in_size);
        memset(pBuffer, 0, in_size);
    }

    //Translate message if required
    if (dectranslation) {
        if (debug) disp_msg("Performing DEC message translation");
        buffsize=0;
        j=0;
        for (i=0;i<in_size;i++) {
            if (inbound[i]=='&') {
                if (memcmp(inbound+i,"&amp;", 5)==0) {
                    pBuffer[j]='&';
                    j++;
                    i+=4;
                } else {
                    if (memcmp(inbound+i,"&quot;", 6)==0) {
                        pBuffer[j]='"';
                        j++;
                        i+=5;
                    } else {
                        if (memcmp(inbound+i,"&comma;", 7)==0) {
                            pBuffer[j]=',';
                            j++;
                            i+=6;
                        } else {
                            if (memcmp(inbound+i,"&pipe;", 6)==0) {
                                pBuffer[j]='|';
                                j++;
                                i+=5;
                            } else {
                                if (memcmp(inbound+i,"&crtlf;", 7)==0) {
                                    pBuffer[j]='\r';
                                    j++;
                                    pBuffer[j]='\n';
                                    j++;
                                    i+=6;
                                } else {
                                    pBuffer[j]=inbound[i];
                                    j++;
                                }
                            }
                        }
                    }
                }
            } else {
                pBuffer[j]=inbound[i];
                j++;
            }
        }
        buffsize=j;
    } else {
        //No translation so just copy in the payload
        buffsize=in_size;
        memcpy(pBuffer, inbound, buffsize);
    }

    if (debug) dump((BYTE *)pBuffer, buffsize);

    //Obtain UniqueID from response, comes after the pipe seperator
    for (i=1;i<(sizeof(decconfrec.abMessageName)+2);i++) {
        if (pBuffer[i]=='|') {
            memcpy(sbuf, (pBuffer+i+1), 12);
            UniqueID=atol(sbuf); //No need to do anything else because the function stops at a non-numeric
            i=(sizeof(decconfrec.abMessageName)+2);
        }
    }

    //Read RFSCACHE to find holding record
    urc = open_rfscache();
    if (urc) {
        if (debug) disp_msg("ERROR Unable to open RFSCACHE - check event logs");
        rc=-1;
    } else {
        if (debug) disp_msg("RD RFSCACHE");
        rfscacherec.ulID = UniqueID;
        rfscacherec.bSeq = 0;
        lrc = ReadRFSCacheLog(__LINE__, LOG_CRITICAL);
        if (lrc > 0L) {
            //Update RFSCACHE - Only updating seq 00 record for now
            //FOR MESSAGES LARGER THAN 498 BYTES, THIS CODE WILL NEED EXTENDING
            rfscacherec.bStatus = 'R';
            memcpy(rfscacherec.abMessage, pBuffer, sizeof(rfscacherec.abMessage));
            if (debug) disp_msg("WR RFSCACHE");
            lrc = WriteRFSCache(__LINE__);
            if (lrc <= 0L) {
                if (debug) disp_msg("ERROR Unable to write RFSCACHE record - check event logs");
                close_rfscache(CL_ALL);
                rc=-1;
            }
        } else {
            if (debug) disp_msg("ERROR Unable to read RFSCACHE record - check event logs");
            close_rfscache(CL_ALL);
            rc=-1;
        }
        close_rfscache(CL_ALL);
    }

    //Release buffer
    FreeBuffer(pBuffer);
    pBuffer = NULL;

    return rc;
}
