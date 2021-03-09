/*************************************************************************
 *
 *
 * File: sockserv.h
 *
 * Author:  Prashant Kotak
 *
 * Created: 18/08/97
 *
 * Purpose:
 *
 * History:
 *
 * V1.1       Brian Greenfield        08/01/2008
 * Replaced size of RFHHTUnion with fixed zie buffer value.
 *
 * V1.2       Brian Greenfield        01/08/2008
 * Inreased MAX_Sockets from 40 to 200 to accomodate West Hallam warehouse useage.
 *
 * V1.3       Brian Greenfield        17/12/2009
 * For RF Stabilisation, extended CLIENT structure to accomodate New 
 * message type flag and flag to indicate more of the message is to 
 * come (used for message fragmentation handling.)
 *
 * V1.4       Brian Greenfield        15/11/2010
 * Changes to accomodate the new DEC Microbroker service.
 *
 *************************************************************************/

#ifndef SOCKSERV_H
#define SOCKSERV_H

#include <flexsock.h>
//#include "rf.h"

#define  HHTCOMMS_PORT  800
#define  MAX_SOCKETS    200                      //BMG 1.2 01-08-2008
/*#define BUFFER_SIZE     sizeof (RFHHTUnion) */ //BMG 1.1 08-01-2008
#define BUFFER_SIZE     2100                     //BMG 1.1 08-01-2008
//#define MAX_TIMEOUT     1

typedef int sock_handle;
typedef struct sockaddr sock_addr;

/*
typedef struct
{
   char     TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;
   unsigned FileAttributes ;
   unsigned PacketSize ;
   char     FileName [RF_HHT_MAX_FILENAME_LENGTH];
   long     FileSize;
   long      FilePointer;
   int       Done;
} FILEINFO;
*/

typedef struct {
    int InUse;
    int HhtId;
    int SignedOn;
    time_t LastMsgDateTime;
    sock_addr SocketAddress;
    int SocketAddrLen;
    sock_handle SocketHandle;
    int SocketState;
    char *Buffer;
    int BufferSize;
    int MessageLen;
    int NewFormMess;        //New style message flag - (message with FF and message size) //BMG 1.3 17-12-2009
    int Continuation;       //Flags more of the message is to come                        //BMG 1.3 17-12-2009
    int ApplicationType;    //Flags whether this is a client or server style entry. 0 = client, 1 = DEC. // BMG 1.4 15-11-2010
    //FILEINFO *FileInfo;
} CLIENT;

/* sockserv prototypes */
int  TcpLoaded (void);
void InitialiseSocketServer (void);
void SocketServerLoop (void);
void ShutDownAllSockets(void);
int InitialiseDECSocket(int recon);                                     // BMG 1.4 15-11-2010
ULONG SendToDEC(char* msgID, int msgIdSize, char* payload, int nbytes); // BMG 1.4 15-11-2010

#endif

