/*************************************************************************
* 
* 
* File: sockfunc.c
*
* Author: Prashant Kotak
*
* Created: f18/08/97
*
* Purpose: High Level TCP/IP vendor-dependant Sockets like function calls.
*
* History: 
* 
*************************************************************************/
#include "transact.h" //SDH 19-May-2006

#include "output.h"
#include "sockserv.h"

int IsTcpLoaded(void)
{
   /* check if the protocol stack is loaded
   return TRUE if loaded
   return FALSE otherwise
   */
   /* initialize with sockets */
   return sock_init();
}

int AllocateSocket ( sock_handle *SockHandle )
{
   int rc ;

   /* allocate socket */
   rc = socket ( AF_INET, SOCK_STREAM, 0 ) ;
   if ( rc < 0 ) {
      LogMessage ( 1, "socket() failed with %d", tcperrno ); 
   }
   *SockHandle = rc ;

   return rc ;
}


int BindSocket ( sock_handle *SockHandle, int iPortNum )                    // SDH 19-05-2005
{
   int rc ;
   struct sockaddr_in addr ;

   memset ( ( char * ) &addr, 0, sizeof ( addr ) ) ;// clear out struct

   // and  set it up
   addr.sin_family = AF_INET;
   addr.sin_port = BSWAP(iPortNum);                                         // SDH 19-05-2005
   addr.sin_addr.s_addr = INADDR_ANY;

   rc = bind ( *SockHandle, ( struct sockaddr * ) &addr, sizeof ( addr ) ) ;
   if (rc < 0) {
      LogMessage ( 1, "bind() failed with: %d, attempting retry...", tcperrno ) ;
      rc = port_cancel(BSWAP(iPortNum));                                    // SDH 19-05-2005
      LogMessage ( 2, "Port Cancel returned: rc=%d, err=%d", rc, tcperrno);
      rc = bind ( *SockHandle, ( struct sockaddr * ) &addr, sizeof ( addr ) ) ;
      if ( rc < 0 ) {
         LogMessage( 1, "bind() failed again with : %d, oh dear!", tcperrno) ;
         CloseSocket ( SockHandle ) ;
      } else {
         LogMessage( 2, "oh, we're ok!" ) ;
      }
   } else {
      LogMessage( 8, "bind() succeeded, tcperrno: %d", tcperrno) ;
      rc = 1 ;
   }

   return rc ;
}


int ListenOnSocket ( sock_handle *SockHandle )
{
   int rc1, rc2, noblock ;

   // listen with a max of 10 connection requests queued (more ignored)
   rc1 = listen ( *SockHandle, 10 ) ;
   if ( rc1 != 0 ) {
      LogMessage ( 1, "listen() failed with %d", tcperrno ) ;
      CloseSocket ( SockHandle ) ;
   } else {
      LogMessage ( 6, "listen() succeeded with SocketHandle=%d", *SockHandle ) ;
      // listen succeeded so place the socket in nonblocking mode
      noblock = 1 ;
      rc2 = ioctl ( *SockHandle, FIONBIO, 
                    ( char * ) &noblock, sizeof ( noblock ) ) ;
      if ( rc2 != 0 ) {
         rc1 = -1 ;
         LogMessage ( 1, "ioctl() failed with %d", tcperrno) ;
         CloseSocket( SockHandle ) ;
      }
   }
   return rc1 ;
}

int AcceptOnSocket ( sock_handle *OriginalSocket, CLIENT *client )
{
   int namelen ;

   namelen = sizeof ( client->SocketAddress ) ;
   client->SocketHandle = accept( *OriginalSocket, 
                                  &( client->SocketAddress ), 
                                  &namelen ) ;

   if (client->SocketHandle < 0 ) {
      if ( tcperrno == EWOULDBLOCK ) {
         // we're here because there are no outstanding
         // connect requests and we're not in blocking mode
         //printf ("accept returned EWOULDBLOCK\n");
         
      } else {
         LogMessage ( 1, "accept() failed with %d", tcperrno ) ;
         /* close the socket */
         CloseSocket ( &( client->SocketHandle ) ) ;
         /* mark failed for return */
         client->SocketHandle = -1 ;
      }
   } else {
      // we've connected...

      LogMessage ( 7, "Client Connected, Handle = %d", client->SocketHandle ) ;

      // record the length of the client address
      client->SocketAddrLen = namelen ;
      // mark this client as in use...
      client->InUse = 1 ;
      // ...and its awaiting input from client
      client->SocketState = AWAITING_INPUT ;
   }

   return client->SocketHandle ;
}

int SocketGroupStatus ( int *sock_handles, int num_socks, int sock_type )
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
   LogMessage ( 7, "Read=%d,Write=%d,Excp=%d", st_read,st_write,st_excpt ) ;

   rc = select( sock_handles, st_read, st_write, st_excpt, timeout ) ;
   if ( rc < 0 ) {
      LogMessage ( 1, "select() failed with %d", tcperrno);
      rc = 0 ;
   } else if ( rc == 0 ) {
      LogMessage( 7, "select() timed out") ;
      rc = 0 ;
   } else if ( rc > 0 ) {
      // success
      LogMessage( 10, "select() succeeded") ;
   } else {
      LogMessage ( 1, "unknown select() error: %d", tcperrno ) ;
      rc = 0 ;
   }

   return rc ;
}


int ReadSocket ( CLIENT *client )
{
   int rc ;

   LogMessage ( 9, "ReadSocket: SocketHandle = %d, BufferSize = %d", 
                client->SocketHandle, client->BufferSize ) ;
   rc = recv ( client->SocketHandle, client->Buffer, client->BufferSize, 0 ) ;

   LogMessage ( 9, "recv(): rc=%d, TCPERRNO=%d", rc, tcperrno ) ;

   if (rc <= 0) {
      // we failed, test error 

      if ( rc == 0 || ( rc < 0 && tcperrno != EWOULDBLOCK ) ) {
         /* mark failed for return */
         client->SocketState = ERROR_STATE ;

         LogMessage ( 3, " Socket (handle=%d) marked for Disconnect after recv() failed", 
                      client->SocketHandle ) ;
      }
   } else {
      // success 
      LogMessage ( rc > 0 ? 7 : 10, "recv() succeeded with %d", rc ) ;
   }

   return rc ;
}

int WriteSocket ( CLIENT *client )
{
   int rc ;

   LogMessage ( 9, "WriteSocket: Socket Handle = %d, MsgLen = %d", 
                client->SocketHandle, client->MessageLen ) ;
   //LogMessage( 10, "WriteSocket Buffer: %30.30s", client->Buffer ) ;

   rc = send ( client->SocketHandle, client->Buffer, client->MessageLen, 0 ) ;
   if (rc < 0) {
      /* we failed because of an error*/
      LogMessage ( 1, "send() failed rc=%d, TCPERRNO=%d", rc, tcperrno ) ;


      /* mark failed for return */
      client->SocketState = ERROR_STATE ;

      LogMessage ( 3, " Socket (handle=%d) marked for Disconnect after send() failed", 
                   client->SocketHandle ) ;
   } else {
      // success
      LogMessage ( 8, "send succeeded with: %d", rc ) ;
   }

   // clear down the buffer
   memset ( client->Buffer, 0, client->BufferSize ) ;

   return rc ;
}

void CloseSocket ( sock_handle *SocketHandle )
{
   soabort ( *SocketHandle ) ;
   soclose ( *SocketHandle ) ;
}

int SetBlockingMode ( sock_handle *SocketHandle, int mode )
{
   int rc, noblock ;

   noblock = ( mode = 0 ? 0 : 1 ) ;
   rc = ioctl ( *SocketHandle, FIONBIO, ( char * ) &noblock, 
                sizeof ( noblock ) );

   return rc ;
}
