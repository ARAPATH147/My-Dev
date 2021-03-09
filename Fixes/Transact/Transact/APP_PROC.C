/*************************************************************************
* 
* 
* File: app_proc.c
*
* Author: Prashant Kotak / Paul Bowers
*
* Created: 18/08/97
*
* Purpose: 
*
* History:  
*
* Version B 22 May 2007    Paul Bowers
* Removal of redundant code from previous RF trials
* 
*************************************************************************/

#include "transact.h" //SDH 19-May-2006
#include "flexif.h"
#include "output.h"
#include "sockserv.h"

extern int process ( char *, int * ) ;  // v4.01   
static void ProcessBootsCommand ( CLIENT * ) ;
 
void ProcessMessage ( CLIENT *client )
{
    ProcessBootsCommand ( client ) ;
}

static void ProcessBootsCommand ( CLIENT *client )
{ 
        
    CurrentHHT = client->HhtId ; // setup for process()
    process ( client->Buffer, &( client->MessageLen ) ) ; 
}

void IssueCmdOffToBootsApp(int HhtId)
{ 
    int SizeOfOff;
    char Buffer[BUFFER_SIZE]; // why? because process() may return a long answer

    SizeOfOff = RF_MESSAGE_TID_LENGTH;
    memcpy(Buffer, RF_HHT_MSG_OFF, SizeOfOff);  

    // issue it
    CurrentHHT = HhtId ; // setup for process()
    process(Buffer, &SizeOfOff); 
}

void SetupRbsNakCmd ( CLIENT *client, char *Msg1, char *Msg2, char *Msg3 )
{ 
    RBSNAKStruct *RbsNak;

    RbsNak = (RBSNAKStruct *) client->Buffer;

    memcpy( RbsNak->TID, RF_HHT_MSG_RBS_RESERVED, RF_MESSAGE_TID_LENGTH );
    RbsNak->Command = RF_HHT_MSG_RBS_NAK; 
    memcpy( RbsNak->MESSAGE1, Msg1, sizeof( Msg1 ) );  
    memcpy( RbsNak->MESSAGE2, Msg1, sizeof( Msg2 ) );   
    memcpy( RbsNak->MESSAGE3, Msg1, sizeof( Msg3 ) );  
    
}

void DummyCallToProcess ( void )
{ 
    char Buffer[256];
    RBSCommandStruct *RbsCmd;
    int SizeOfCmd;

    RbsCmd = ( RBSCommandStruct * ) Buffer;
    // setup dummy command
    memcpy( RbsCmd->TID, RF_HHT_MSG_RBS_RESERVED, RF_MESSAGE_TID_LENGTH );
    RbsCmd->Command = -1; // dummy, invalid command

    // issue it to hook into Boots code 
    CurrentHHT = -1 ; // setup for process() 
    process( ( char * ) RbsCmd , &SizeOfCmd);  
}
