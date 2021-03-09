//*****************************************************************************
//  SHOLMEC.C          Ranjith Gopalankutty               14-10-2018
//   
//  SHOLMEC is a client socket application , which will send the 
//  OS messages in real time to the server Application RADAR.
//  Messages will be read from the program DRWATSON using the real
//  time pipe, based on the allowed configuration messages in 
//  SHOLMEC.BIN to avoid flooding of network with unnecessary messages.
//  Which also gives the flexibility to add/remove any messages which 
//  we need  based on need

///////////////////////////////////////////////////////////////////////////////
 
#include <SHOLMEC.H>
#include <SHOLMFUN.h> 
#include <stdio.h>
#include <stdlib.h>
#include <types.h>
#include <socket.h>
#include <tcpip/in.h>
#include <netdb.h>
#include <tcpsock.h>
#include <string.h>
#include <time.h> 
#include<flexif.h>
  
////////////////////////////////////////////////////////////////////////
//  Global Variable Declarations 
   
int    EventYear,
	   EventMonth,
	   EventDay,
	   EventHour,    
	   EventMin;
	   
char   lPFileName[24],
       EvFileName[24],           
       EventProgram[8],
       SwappedNode[2];	   

long   FileHndlPtr,
       FileHndlEv,
       PipeHndl,	  
	   giSockHndl;
	  
BYTE RADAR[64] = "RADAR";

BYTE Store[5];     

LONG lpoffset;

//BYTE CR_LF_NULL[] = "\r\n";
	  
long   EvFileHndl = 0L,      
       EvFileSize = -1L;	  
		  
struct sockaddr_in serv_addr;
struct hostent *server;



// Reused struct from CHKIP program to get EvenData.
// if required we can post it to eventlog

typedef struct EvenData 
{	
	 int   			EventDate; 
	 unsigned int   EventTime;
	 unsigned char 	EventNode[2];
	 int   			EventTerminal;
	 char       	EventSource;
	 unsigned char  EventGroup;	 	 
	 int            EventMessage;
	 unsigned char  EventSeverity;
	 unsigned char 	EventNumber;
	 char  			EventUnique[18];	
	 unsigned char 	EventBucket;
	 unsigned char 	EventFormat;
	 int 			EventCount;
	
} EvnData;

EvnData EvData;
// Below is a reused function from CheckIP to get store number and can
// up in future if we nee
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

// Struct reused from CHKIP program to define adxserve function
typedef struct ADXSERVE_Parm_General 
{
    UWORD function;
    UWORD length;
    BYTE *buffer;
} ADXSRV_PARM;

///////////////////////////////////////////////////////////////////////////////
///////////////////   Major Function starts here  /////////////////////////////
///////////////////////////////////////////////////////////////////////////////	

BOOLEAN FireAndForget()
{ 	 
 	if (send(giSockHndl,RADAR,strlen(RADAR),0) < 0)         
    {         
         printf("Error sending the message.\n");	  
    }
    else
    {
         printf("sent successfuly\n");
	     strcpy(RADAR, "");
    }
	
	return 0;
}

// Function is taken from CHKIP program to get store number
void GetStoreDetails()
{
	 ULONG srv_fnum;
	 LONG ret;
     ADXSRV_PARM parm_list;
     ADXSRV_STATUS StoreNumber;    
    
	 // Below is to get the store number by calling ADXSERVE and pass function type 4
	 parm_list.function = 4;
	 parm_list.length = 0;
	 parm_list.buffer = 0;

	 srv_fnum=s_open(A_EXEC, "adxserve:");
	 ret=s_special(0, 0, srv_fnum, (BYTE *)&StoreNumber, 0L, (BYTE *)&parm_list, 0L);
	 strncpy(Store,(BYTE *)&StoreNumber,4);
	 s_close(0, srv_fnum);	 
}

BOOLEAN MakeMasterString()
{
	     strncpy(EventProgram,EvData.EventUnique+10,8);
	// Now Lets Start copying to the master string RADAR
	 
	     strncpy(RADAR + 5, (BYTE *)&EventYear,2);
	     strncpy(RADAR + 7, ",",1);
	     strncpy(RADAR + 8, (BYTE *)&EventMonth,2);
	     strncpy(RADAR + 10, ",",1);
	     strncpy(RADAR + 11, (BYTE *)&EventDay,2);
	     strncpy(RADAR + 13, ",",1);
	     strncpy(RADAR + 14,(BYTE *)&EventHour,2);
	     strncpy(RADAR + 16, ",",1);
	     strncpy(RADAR + 17,(BYTE *)&EventMin,2);
	     strncpy(RADAR + 19, ",",1);
         strncpy(RADAR + 20,(BYTE *)&SwappedNode,2);
	     strncpy(RADAR + 22, ",",1);
	     strncpy(RADAR + 23,(BYTE *)&EvData.EventTerminal,2);
	     strncpy(RADAR + 25, ",",1);
	     strncpy(RADAR + 26,(BYTE *)&EvData.EventSource,2);
	     strncpy(RADAR + 28, ",",1);
	     strncpy(RADAR + 29,(BYTE *)&EvData.EventGroup,1);
	     strncpy(RADAR + 30, ",",1);
	     strncpy(RADAR + 31,(BYTE *)&EvData.EventMessage,3);
	     strncpy(RADAR + 34, ",",1);
	     strncpy(RADAR + 35,(BYTE *)&EvData.EventSeverity,1);
	     strncpy(RADAR + 36, ",",1);
	     strncpy(RADAR + 37,(BYTE *)&EvData.EventNumber,1);
	     strncpy(RADAR + 38, ",",1);
	     strncpy(RADAR + 39,(BYTE *)&EventProgram,8);
	     strncpy(RADAR + 47, ",",1);
	     strncpy(RADAR + 48,(BYTE *)&EvData.EventBucket,1);
	     strncpy(RADAR + 49, ",",1);
	     strncpy(RADAR + 50,(BYTE *)&EvData.EventFormat,1);
	     strncpy(RADAR + 51, ",",1);
	     strncpy(RADAR + 52,(BYTE *)&EvData.EventCount,1);		 
	     return 0;
}


// Function to format the Node ID correctly as system gives in low value high value combination like FC,EC
WORD SwapNode()
{
	char a = EvData.EventNode[1];
	char b = EvData.EventNode[0];		 
	memcpy(SwappedNode,(BYTE *)&a,1);
	memcpy(SwappedNode + 1,(BYTE *)&b,1);
	return 0;
}

BOOLEAN FormatString()
{      
	BOOLEAN bReturnCode = -1; 
	
	// Just to ensure no junk data is being tried to be formatted.
	if (EvData.EventDate != 0)	
	{	     
		 EventYear  = (EvData.EventDate >> 9) + 1980;
		 EventMonth = ((EvData.EventDate &0x01e0) >> 5) ;
		 EventDay   = (EvData.EventDate & 0x0001f);		 
		 // EvData.EventTime = (EvData.EventTime &0xffe);
		 EventHour  = ((EvData.EventTime & 0x0F800) >> 11); 
		 EventMin   = ((EvData.EventTime & 0x07e0)  >> 5); 
		 	 
		 // Below Function will Swap the Node ID and format it correctly
		 SwapNode();
		 // Below Function will copy the formmated data to Radar String
		 MakeMasterString();
		 
		  printf("%.4s%s%d%s%d%s%d%s%d%s%d%s%.2s%s%d%s%d%s%c%d%s%d%s%d%s%.8s%s%d%s%d%s%d\n",Store,",",EventYear,"-",EventMonth,"-",EventDay,","      \
		     ,EventHour,":",EventMin,",",SwappedNode,",",EvData.EventTerminal,",",EvData.EventSource,","                             \
		     ,EvData.EventGroup,EvData.EventMessage,",",EvData.EventSeverity,",",EvData.EventNumber,","                              \
	     	 ,EventProgram,",",EvData.EventBucket,",",EvData.EventFormat,",",EvData.EventCount);
		
         //printf("Event Year is - %d\n",EventYear);
		 //printf("Event Month is -  %d\n",EventMonth);
		 //printf("Event Day is - %d\n", EventDay);
		 //printf("Event Hour is - %d\n", EventHour);
		 //printf("Event Minutes are - %d\n", EventMin);
		 //printf("Node id is %.2s\n",SwappedNode);
		 //strncpy(SwappedNode," ",2);
		 //printf("Terminal Number is %d\n",EvData.EventTerminal);
		 //printf("Event source is %d\n",EvData.EventSource);
		 //printf("Event group is %c\n",EvData.EventGroup);
		 //printf("Event Number is %d\n",EvData.EventMessage);
		 //printf("Severity is %d\n",EvData.EventSeverity);
		 //printf("Event even is %d\n",EvData.EventNumber);
		 //printf("Event program is %.8s\n", EventProgram);
		 //printf("unique data is %x\n",EvData.EventUnique);
		 //printf("Bucket of event is %d\n",EvData.EventBucket);
		 //printf("Format is %d\n",EvData.EventFormat);
		 //printf("Repeated count is  %d\n",EvData.EventCount);
	}
	
     return bReturnCode;
}

BOOLEAN ReadEvFile()
{
	 long lRc; 
	 long RecordNumber;	  
	 RecordNumber = 0;
	 long NumberRecords;
	 
	 //Function call to get store number as we dont want it to be in
	 //while loop
	 GetStoreDetails();
	 
	  if (FileHndlPtr != 0) 
		{
			 RecordNumber = (lpoffset/ 36); 
			 printf("The record number i should be reading is %d\n", RecordNumber);
			 
		}
	 
	 while ( 1 == 1)
	 {
		 EvFileSize  = FileSize(EvFileName);
		 NumberRecords = (EvFileSize / 36 );	 
		 
		 while (RecordNumber < NumberRecords)
		 { 
		 	lRc = s_read(A_BOFOFF,FileHndlEv,(void *)&EvData,
                 36, 36 * RecordNumber);
			RecordNumber = RecordNumber + 1;			  
			//printf("The Record Number is %d\n" , RecordNumber);			
			//s_write( A_FLUSH | A_BOFOFF,                         
            //                   FileHndlPtr,
            //                   (BYTE *)&EvFileSize,
            //                   sizeof(EvFileSize),
            //                   0L );
			
      		 FormatString(); 
		 }
	  }
	
	return 0;
}

BOOLEAN NeverEndLoop()
{
	BOOLEAN bReturnCode = 1;
	//if ( ReadPipe() == 0)
	//{
	//	bReturnCode = 0;		
	//}
	ReadEvFile();
	return bReturnCode;
}

BOOLEAN MainProcessing()
{
	 BOOLEAN bReturnCode = 1;
	 printf("Starting main processing.\n");
	 if ( NeverEndLoop() == 0 );
	 {
	 	 bReturnCode = 0;
	 }
	 return bReturnCode;
}	  

BOOLEAN InitSocket()
{
		
     BOOLEAN  bReturnCode = 1;
     long lRecvWaitTime   = 10000L;
     printf("Initialising socket.\n");
     printf("Getting the IP address from hostname.\n");
     // Get server IP address from hostname

     server = gethostbyname("CE");    
   
   // If host not found
   if (server == NULL)
   {
      printf("ERROR, no such host.\n");
   }
   else
   {
      bzero((char *) &serv_addr, sizeof(serv_addr));	   

      serv_addr.sin_family = AF_INET; 	  

      bcopy((char *)server->h_addr,(char *)&serv_addr.sin_addr.s_addr,
            server->h_length);

      serv_addr.sin_port = htons(PORT_NUMBER);
	   

      // Open socket
	  
      giSockHndl = socket(AF_INET, SOCK_STREAM, 0);

      printf("Opening the socket.\n");
      // If error opening socket
      if (giSockHndl < 0)
      {
        printf("ERROR opening socket.\n");
      }
      else
      {
         // Establish connection
         printf("Connecting to the socket.\n");
         if (connect(giSockHndl,(struct sockaddr *)&serv_addr,
                   sizeof(serv_addr)) < 0)
         {
            printf("ERROR connecting to the socket.\n");
         }
         else
         {
            // Set the socket receive wait time
            if (setsockopt(giSockHndl,SOL_SOCKET,SO_RCVTIMEO,
                            (char *)&lRecvWaitTime, sizeof(lRecvWaitTime))< 0)
            {
               printf( "Error setting the socket receive time out.\n");
            }
            else
            {
               printf( "Socket Initialization successful\n");
                
               bReturnCode = 0;
            }
         }
      }

   }
   
   return bReturnCode;	
}
		  
BOOLEAN CreatePtr() 
{
	  EvFileSize  = FileSize(EvFileName);
	  BOOLEAN bReturnCode = 1;	   
	  s_create (O_FILE,A_READ | A_WRITE,
                           lPFileName,
                           4L,
                           5,
                           0L);
	  FileHndlPtr = FileOpen( lPFileName, A_READ | A_WRITE );
	  printf("Now updating the value from Event log file Size size %d " , EvFileSize);
	  s_write( A_FLUSH | A_BOFOFF,                         
                               FileHndlPtr,
                               (BYTE *)&EvFileSize,
                               sizeof(EvFileSize),
                               0L );    
					
	 // Now update the pointer file with the size of the queue
     // as its the current size of the queue
	 
	 bReturnCode = 0;
	 return bReturnCode;	  
}

BOOLEAN CheckPtr()
{
	
   BOOLEAN bReturnCode = 1;    
   long lRc;
   strncpy(lPFileName, "D:/ADX_UDT1/SHOLMEC.BIN", 24);  
   long lPFileSize      = -1L; 
 
   //Check the file size, if it returns -1 then file is not present.
   
   lPFileSize = FileSize(lPFileName);
 
   if( lPFileSize == -1 )
   {
      printf("Pointer file is missing, Creating the Pointer file\n", lPFileName);
	  CreatePtr();
   }
   
   else if( lPFileSize <= 1 )
	   
   {
         printf("Pointer file %s is of invalid size %ld bytes updating the value from the queue\n", lPFileName, lPFileSize);
	  
	     FileHndlPtr = FileOpen( lPFileName, A_READ | A_WRITE );	   
	     s_write( A_FLUSH | A_BOFOFF,                         
                               FileHndlPtr,
                               (BYTE *)&EvFileSize,
                               sizeof(EvFileSize),
                               0L );  
		 			     	  
   }
   else
   {
         //printf("Value inside of Pointer is %ld bytes.\n",  EvFileSize);
         printf("Trying to open the Pointer file.\n");

         // Open the file
         FileHndlPtr = FileOpen( lPFileName, A_READ | A_WRITE );
		 printf("Value inside of Pointer is %d bytes.\n",  FileHndlPtr);
		 lRc = s_read(A_BOFOFF,
                      FileHndlPtr,
                      (void *)&lpoffset,
                      4L,
                      0L );

         // Check for file open error
	  
         if (FileHndlPtr <= 0)
         {
             printf("Cannot open Pointer file, return code %ld.\n", FileHndlPtr);
         }
	  
          else
         {
             printf("Pointer file opened successfully.\n");
			 printf("pointer value is %ld\n",lpoffset);
             bReturnCode = 0;		  
         }
   }

   return bReturnCode;		
}

BOOLEAN CheckFile( void )
{
    
     BOOLEAN bReturnCode = 1;
   
     strncpy(EvFileName, "D:/ADX_IDT1/EVLOG00", 24);  
   
 
     //Check the file size, if it returns -1 then file is not present.
   
     EvFileSize  = FileSize(EvFileName);
 
     if( EvFileSize  == -1 )
     {
      printf("Input file %s not found, exiting..\n", EvFileName);
     }
     else if( EvFileSize  <= 1 )
     {
      printf("Input file %s is of invalid size %ld bytes, exiting\n",
	  
                                                EvFileName, EvFileSize );   
	 }
   
     else
     {
      printf("Size of input file %s is %ld bytes.\n", EvFileName, EvFileSize );    

      // Open the file
      FileHndlEv = FileOpen( EvFileName, A_READ ); 
	  

      // Check for file open error
      if( FileHndlEv <= 0 )
      {
         printf("Cannot open Event log file, return code %ld.\n", EvFileHndl);
      }
	  
      else
		  
      {
         printf("Event file opened successfully.\n");
         bReturnCode = 0;
      }
    }

     return bReturnCode;
}	 	  

BOOLEAN  CheckRunPipe( void )
{
	 BOOLEAN bReturnCode = 1;  
     PipeHndl = 0L;   
     PipeHndl = s_create(O_FILE, SHOLMEC_CFLAGS, SHOLMEC_PIPE, 1, 
                                                      0x0FFF, SHOLMEC_Record_length);      

       if  ( PipeHndl <= 0 )
       {
          printf("DRWATSON is active, Checking Queue and Pointer file\n");         
       }

      else
       {
          printf("DRWATSON is not running, ending.\n");
          exit(1);
	  
       }

    return bReturnCode;
}

BOOLEAN SholmeInitialize( void )   
{		 
	 BOOLEAN bReturnCode = 1;	
     printf("Starting initialization \n");
	 
   //  if (CheckRunPipe() == 0);          // To check if the DRWATSON pipe is running.
	// {   
	    if (CheckFile() == 0)           // Check if the Event log file is existing   
		 {                 		 
	        if (CheckPtr() == 0)        // Check if Pointer file is existing
	         {               
	           if ( InitSocket() == 0)  // Socket initialization for sending message
			     {			   	   
	 	               bReturnCode = 0;	
			     }
			 } 

	     }
   //  }	

	 return bReturnCode;
}	

// Main code starts from Here and all other functions in the top
// it initializes pipe,files check configuration files etc 
// once everything okay will call mainprocessing

int main( void )
{ 	    
      
     BOOLEAN bReturnCode = 1;
	 printf("Starting SHOLMEC \n");  
	
  	 if ( SholmeInitialize() == 0)
    {
		printf("Initialization is successful\n");
         		
	    MainProcessing();	

	}
	
	else  
		
	{
	     printf ("Could not initialize properly ,plese check error logs \n");
         exit(1);			   		
	}
	 bReturnCode = 0;
	
	 return (bReturnCode);
	
} 


 