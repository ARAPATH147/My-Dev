//
// File:    rf.h
//
// Author:  Paul Armstrong
//
// Created: 11/8/97
//
// Purpose: Contains RF-related structures and defines
//

#ifndef RF_H
#define RF_H

#define     HHT_APPLICATION_FILE_NAME        "e:\\rfhht.exe"
#define     HOST_APPLICATION_FILE_NAME       "rf:rfhht.exe"

#define     HHT_SCRIPT_TRANSFER_NAME        "e:\\esd.lst"
#define     HOST_SCRIPT_TRANSFER_NAME       "rf:esd.lst"

#define     RF_ERROR_NO_RESPONSE          1         
#define     RF_MAXIMUM_EXPECTED_MESSAGES  5
#define     RF_MESSAGE_RETRY_COUNT        1
#define     RF_MESSAGE_TID_LENGTH         3
#define     RF_OPER_ID_LENGTH             RF_MESSAGE_TID_LENGTH
#define     RF_BARCODE_SIZE               13 
#define     RF_DESC_SIZE                  20
#define     RF_FULL_DESC_SIZE             45
#define     RF_QTY_SIZE                   4 
#define     RF_PRICE_SIZE                 6
#define     RF_VAL_SIZE                   8
#define     RF_BOOTS_CODE_SIZE            7
#define     RF_REPORT_TITLE_SIZE          20
#define     RF_REPORT_DATA_SIZE           20
#define     RF_REPORT_ID_SIZE             12
#define     RF_REPORT_SEQ_SIZE            4
#define     RF_REPORT_COUNT_SIZE          3

#define     RF_MAX_DATA_LINES_PER_RPT_HDR 10
#define     RF_MAX_LINES_PER_RPT_HDR      6
#define     RF_MAX_REPORTS                50

#define     SEQ_LENGTH                    3

typedef struct 
{
   char *   HHTMessageID ;
   unsigned MessageLength ;
}
HostHHTMessageLengthStruct ;

#define RF_HHT_MSG_SOR "SOR"
#define RF_HHT_MSG_SNR "SNR"
#define RF_HHT_MSG_OFF "OFF"
#define RF_HHT_MSG_GAS "GAS"
#define RF_HHT_MSG_GAP "GAP"
#define RF_HHT_MSG_GAX "GAX"
#define RF_HHT_MSG_PLO "PLO"
#define RF_HHT_MSG_PLR "PLR"
#define RF_HHT_MSG_PLS "PLS"
#define RF_HHT_MSG_PLC "PLC"
#define RF_HHT_MSG_PLF "PLF"
#define RF_HHT_MSG_PLX "PLX"
#define RF_HHT_MSG_PCS "PCS"
#define RF_HHT_MSG_PCD "PCD"
#define RF_HHT_MSG_PCR "PCR"
#define RF_HHT_MSG_PCM "PCM"
#define RF_HHT_MSG_PCX "PCX"
#define RF_HHT_MSG_ENQ "ENQ"
#define RF_HHT_MSG_SSE "SSE"
#define RF_HHT_MSG_ISE "ISE"
#define RF_HHT_MSG_INS "INS"
#define RF_HHT_MSG_INX "INX"
#define RF_HHT_MSG_PLU "PLU"
#define RF_HHT_MSG_PRT "PRT"
#define RF_HHT_MSG_RPO "RPO"
#define RF_HHT_MSG_RPX "RPX"
#define RF_HHT_MSG_RLE "RLE"
#define RF_HHT_MSG_RLR "RLR"
#define RF_HHT_MSG_RLS "RLS"
#define RF_HHT_MSG_RLD "RLD"
#define RF_HHT_MSG_RPS "RPS"
#define RF_HHT_MSG_RUP "RUP"
#define RF_HHT_MSG_RLF "RLF"
#define RF_HHT_MSG_RSP "RSP"
#define RF_HHT_MSG_UNK "UNK"
#define RF_HHT_MSG_ACK "ACK"
#define RF_HHT_MSG_NAK "NAK"  

#define RF_HHT_MSG_GAR "GAR"
#define RF_HHT_MSG_EQR "EQR"
#define RF_HHT_MSG_SSR "SSR" 
#define RF_HHT_MSG_ISR "ISR" 
#define RF_HHT_MSG_PLL "PLL"
#define RF_HHT_MSG_PLE "PLE"
#define RF_HHT_MSG_PLI "PLI"

#define RF_HHT_MSG_PAL "PAL"
#define RF_HHT_MSG_PAR "PAR"
#define RF_HHT_MSG_MAX "MAX"

#define RF_HHT_MSG_RBS_RESERVED     "XXX"

#define RF_HHT_MSG_RBS_RX_FILE_DETAILS      0
#define RF_HHT_MSG_RBS_TX_FILE_DETAILS      1
#define RF_HHT_MSG_RBS_RX_FILE              2
#define RF_HHT_MSG_RBS_TX_FILE              3
#define RF_HHT_MSG_RBS_RX_FILE_ACK          4
#define RF_HHT_MSG_RBS_SRV_LSTN             5
#define RF_HHT_MSG_RBS_GET_TIME             6
#define RF_HHT_MSG_RBS_SEND_TIME            7
#define RF_HHT_MSG_RBS_TX_FILE_ACK          8
#define RF_HHT_MSG_RBS_TX_FILE_DATA         9
#define RF_HHT_MSG_RBS_NAK                 10

// This application relies on RF_HHT_MAX_FILENAME_LENGTH 
// being >= the size of 'RBSNAKStruct'. 

#define RF_HHT_MAX_FILENAME_LENGTH        200
#define RF_HHT_MAX_DATA_LENGTH                        1024
#define RF_SERVER_MAX_DATA_LENGTH                        256
#define RF_DATE_NAK_MSG_LENGTH                          8
#define RF_TIME_NAK_MSG_LENGTH                          6
#define RF_RBS_NAK_MSG_LENGTH                            20

typedef struct
{
   char TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;  
   char MESSAGE1 [RF_RBS_NAK_MSG_LENGTH] ;
   char MESSAGE2 [RF_RBS_NAK_MSG_LENGTH] ;
   char MESSAGE3 [RF_RBS_NAK_MSG_LENGTH] ;
}
RBSNAKStruct ;         
                
typedef struct
{
   char     TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;  
   char     DateYYYYMMDD [RF_DATE_NAK_MSG_LENGTH] ;
   char     TimeHHMMSS [RF_TIME_NAK_MSG_LENGTH] ;
  
}
RBSRecvTimeStruct ;

typedef struct
{
   char     TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;
   char     FullyQualifiedNullTerminatedFilename [RF_HHT_MAX_FILENAME_LENGTH] ;
}
RBSRXFileDetailsStruct ;

typedef struct
{
   char           TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned       Command ;
   char           FileDateYYYYMMDD [RF_DATE_NAK_MSG_LENGTH] ;
   char           FileTimeHHMMSS [RF_TIME_NAK_MSG_LENGTH] ;
   unsigned long  FileSize ;
   unsigned long  FileAttributes ;
}
RBSTXFileDetailsStruct ;

#define RF_FILE_REPLACE       1
#define RF_FILE_APPEND        2

typedef struct
{
   char     TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;
   unsigned FileAttributes ; // Added to allow append/replace file etc.
   unsigned PacketSize ;
   char     FullyQualifiedNullTerminatedFilename [RF_HHT_MAX_FILENAME_LENGTH] ;
}
RBSRXFileStruct ;

typedef struct
{
   char     TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;
}
RBSRXFileAckStruct ;

typedef struct
{
   char           TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned       Command ;
   unsigned long  FileSize ;
   unsigned char  Data [RF_HHT_MAX_DATA_LENGTH] ;
}
RBSTXFileStruct ;

typedef struct   
{   
   char           TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned       Command ;
   unsigned long  FileSize ;   
}
RBSTXFileHeaderStruct ;
   
typedef struct
{
   char     TID [RF_MESSAGE_TID_LENGTH] ;
   unsigned Command ;
}
RBSCommandStruct ;

typedef struct
{ 
   char TID [RF_MESSAGE_TID_LENGTH] ;
   char ID [3] ;
   char PSWD [3] ;
} 
SORStruct ;

typedef struct
{ 
   char TID [RF_MESSAGE_TID_LENGTH] ;
   char ID [3] ;
   char AUTH ;
   char UNAME [15] ;
   char DATE [8] ;
   char TIME [4] ;
} 
SNRStruct ;

typedef struct
{ 
   char TID [RF_MESSAGE_TID_LENGTH] ;
} 
OFFStruct ;

typedef struct
{
   char TID [RF_MESSAGE_TID_LENGTH] ;
   char SVT [8] ;
   char SVPREV [8] ; 
}
SSRStruct ;

typedef struct
{
   char TID [RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char LIST_ID [3] ;
   char SEQ [SEQ_LENGTH];
   char BARCODE [13] ;
   char BCDE [7] ;
   char CURRENT [4] ;
   char FILL [4] ;
   char GAP_FLAG ;   
}
GAPStruct;
 
typedef struct
{
   char TID [RF_MESSAGE_TID_LENGTH];
   char ID [3];
}
GASStruct;
 
typedef struct
{         
   char TID[RF_MESSAGE_TID_LENGTH];
   char LIST_ID[3];
}
GARStruct;
                       
typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[3];
   char LIST_ID[3];
   char LINES[4];
   char SELS[4];
}  
GAXStruct;
                       
typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[3];
}
PLOStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char SEQ [SEQ_LENGTH] ;
   char AUTH ;
}
PLRStruct;
              
typedef struct
{          
   char TID[RF_MESSAGE_TID_LENGTH];
   char LIST_ID [3] ;
   char SEQ [SEQ_LENGTH] ;
   char STATUS ;
   char DATETIME [12] ; 
   char LINES [4] ;
   char UNAME [15] ;
}
PLLStruct ;              
              
typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char LIST_ID [3] ;
   char SEQ [SEQ_LENGTH] ;
}
PLSStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char LIST_ID [3] ;
//   char GAP_FLAG;
}
PLEStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH] ;
   char LIST_ID [3] ;
   char SEQ [SEQ_LENGTH] ;
   char BCDE [7] ;
   char PARENT [7] ;
   char DESC [20] ;
   char REQUIRED [4] ;
   char STATUS ;   
   char GAP_FLAG ;
}
PLIStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char LIST_ID [3] ;
   char SEQ [SEQ_LENGTH] ;   
   char BCDE [7] ;
   char COUNT [4] ;
   char GAP_FLAG ;   
}
PLCStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
}
PLFStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char LIST_ID [3] ;
   char LINES [4] ;
   char ITEMS [6] ;
   char COMPLETE;
}
PLXStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
}
PCSStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char ITEM_CODE [13] ;
   char CHECK_PRICE [6] ;
}
PCDStruct; 

typedef struct
{          
   char TID[RF_MESSAGE_TID_LENGTH];
   char BCDE [7] ;
   char DESCRIPTION [20] ;
   char PRICE [6] ;
   char ITEM_SEL [RF_FULL_DESC_SIZE] ;
}
PCRStruct ;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char BCDE [7] ;
   char VAR [6] ;
}
PCMStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
}
SSEStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
}
INSStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [RF_OPER_ID_LENGTH] ;
}
INXStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3] ;
   char ITEMS [4] ;
   char SELS [4] ;
}
PCXStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[RF_OPER_ID_LENGTH];
   char BARCODE[RF_BARCODE_SIZE];
}
PLUStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[RF_OPER_ID_LENGTH];
   char BARCODE[RF_BARCODE_SIZE];
}
ISEStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char DESC[RF_DESC_SIZE];
   char CQTY[RF_QTY_SIZE];
   char CVAL[RF_VAL_SIZE];
   char PQTY[RF_QTY_SIZE];
   char PVAL[RF_VAL_SIZE];
}
ISRStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
}
RPOStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
}
RPXStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[RF_OPER_ID_LENGTH];
   char SEQ[RF_REPORT_SEQ_SIZE];
}
RLEStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char TITLE[RF_REPORT_TITLE_SIZE];
   char REPORT_ID[RF_REPORT_ID_SIZE];
}                                  
RLRStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[RF_OPER_ID_LENGTH];
   char REPORT_ID[RF_REPORT_ID_SIZE];
   char SEQ[RF_REPORT_SEQ_SIZE];
}
RLSStruct;

typedef struct
{
   char SEQ[RF_REPORT_SEQ_SIZE];
   char DATA[RF_REPORT_DATA_SIZE];
}
REPORT_HEADER_TYPE;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char COUNT[RF_REPORT_COUNT_SIZE];
   REPORT_HEADER_TYPE HEADER[RF_MAX_LINES_PER_RPT_HDR];
}
RLDStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[RF_OPER_ID_LENGTH];
   char REPORT_ID[RF_REPORT_ID_SIZE];
   char SEQ[RF_REPORT_SEQ_SIZE];
}
RPSStruct;

typedef struct
{
   char LEVEL;
   char FUNC;
   char DATA[RF_REPORT_DATA_SIZE];
}
REPORT_DATA_TYPE;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char COUNT[RF_REPORT_COUNT_SIZE];
   REPORT_DATA_TYPE DATA[RF_MAX_DATA_LINES_PER_RPT_HDR];
}
RUPStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
}
RLFStruct;


typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char BCDE[RF_BOOTS_CODE_SIZE];
   char DESC[RF_DESC_SIZE];
   char PRICE[RF_PRICE_SIZE];
   char Tsf[4];                         //SDH 13-12-2004
   char SELD[RF_FULL_DESC_SIZE];
   char SUPPLY;
   char REDEAM;   
}
RSPStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
}
UNKStruct;

typedef struct
{
   char TID [RF_MESSAGE_TID_LENGTH] ;
   char MESSAGE1 [21] ;
   char MESSAGE2 [21] ;
   char MESSAGE3 [21] ;
}
NAKStruct ;         
                
typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
}
ACKStruct;

typedef struct
{         
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID[3];
   char TYPE;
   char DESC;
   char BARCODE[13]; 
}
ENQStruct;                
                
typedef struct
{         
   char TID[RF_MESSAGE_TID_LENGTH];
   char BCDE[7];
   char PARENT[7];
   char DESC[20];
   char PRICE[6];
   char SELD[RF_FULL_DESC_SIZE];
   char STATUS;
   char SUPPLY;
   char REDEAM;
}
EQRStruct;                      

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3];
   char BARCODE[13];
   char METHOD;
}              
PRTStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ID [3];
   char BARCODE[13];
}
PALStruct;

typedef struct
{
   char TID[RF_MESSAGE_TID_LENGTH];
   char ENFORCEMENT[1];
   char URL_ITEM[60];
   char BANNER_ITEM[60];
   char URL_LINK[60];
   char BANNER_LINK[60];
}
PARStruct;

typedef struct
{
   char big[768];                                                           //SDH Planners
}
MAXStruct;

typedef union
{
   char TID [RF_MESSAGE_TID_LENGTH] ;
   SORStruct SOR ;
   SNRStruct SNR ;
   OFFStruct OFS ;
   GAPStruct GAP ;
   GASStruct GAS ;
   GARStruct GAR ;
   GAXStruct GAX ;
   PLOStruct PLO ;
   PLRStruct PLR ;
   PLSStruct PLS ;
   PLCStruct PLC ;
   PLFStruct PLF ;
   PLXStruct PLX ;
   PCSStruct PCS ;
   PCDStruct PCD ;
   PCRStruct PCR ;
   PCMStruct PCM ;
   SSEStruct SSE ;
   INSStruct INS ;
   INXStruct INX ;
   PCXStruct PCX ;
   PLUStruct PLU ;
   ISEStruct ISE ;
   RPOStruct RPO ;
   RPXStruct RPX ;
   RLEStruct RLE ;
   RLSStruct RLS ;
   RPSStruct RPS ;
   RSPStruct RSP ;
   UNKStruct UNK ;
   NAKStruct NAK ;
   ACKStruct ACK ;
   ENQStruct ENQ ;
   EQRStruct EQR ;
   PRTStruct PRT ;
   SSRStruct SSR ;
   PLEStruct PLE ;
   PLIStruct PLI ;  
   PLLStruct PLL ;
   RUPStruct RUP ;
   
   PALStruct PAL ;
   PARStruct PAR ;
   
   MAXStruct MAX ; // Buffer size place holder
   
   // RBS-specific utility/support commands
   RBSRXFileDetailsStruct RBSClientRXFileDetails ;
   RBSTXFileDetailsStruct RBSServerTXFileDetails ;
   RBSRXFileStruct        RBSRXFile ;
   //RBSTXFileStruct      RBSTXFile ;
   RBSRXFileAckStruct     RBSRXFileAck ;
   RBSNAKStruct           RBSNAK ;         
} 
RFHHTUnion ;  
                     
// Sockets functions

int RFGetLastError () ;
int RFNetworkActive () ;
int RFInRange () ;
void RFWaitUntilInRange () ;
int RFEstablishConnection () ;
int RFSendData (char *Data, unsigned DataLength) ;
int RFSendMessageData (char *SendBuffer, unsigned MaxDataLength, char *MessagePrintFormat, ...) ;
int RFReadData (char *Data, unsigned MaxDataLength, unsigned *DataLength) ;
int RFCloseConnection () ;
void MakeFullLengthIPAddress (char *IPAddress) ;
void RFSetHostPortNumber (unsigned int PortNumber) ;
void RFSetHostIPAddress (char *HostIPAddress) ;
void RFSetAPNetID (unsigned int NetID) ;
void RFSetHHTIPAddress (char *HHTIPAddress) ;
void RFSetHHTIPRouter (char *HHTIPRouter) ;
void RFSetHHTIPNetMask (char *HHTIPNetMask) ;
unsigned RFGetHostPortNumber () ;
void RFGetHostIPAddress (char * HostIPAddress) ;
unsigned RFGetAPNetID () ;
void RFGetHHTIPAddress (char * HHTIPAddress) ;
void RFGetHHTIPRouter (char * HHTIPRouter) ;
void RFGetHHTIPNetMask (char * HHTIPNetMask) ;
int RFIPConfigurationSet () ;

// High level RF interface - RBS-only utility functions

int RFGetFileDateTimeSize (char *HostFileName, RBSTXFileDetailsStruct *TXFD) ;
int RFCheckFileDateTimeSize (char *HostFileName, char *HHTFileName, int *bHHTFileDiffersFromHost) ;
int RFReceiveFile (char *HostFileName, char *HHTFileName, int bDisplayProgressIndicator, char *ProgressTitle) ;
int RFReceiveFileIfLater (char *HostFileName, char *HHTFileName, int bDisplayProgressIndicator, char *ProgressTitle, int *bHHTFileDiffersFromHost) ;
int RFSendFile (char *HHTFileName, char *HostFileName, unsigned FileAttributes, int bDisplayProgressIndicator, char *ProgressTitle) ;
int RFGetHostTime ( void ) ;
int RFProcessScriptFile (char *HHTScriptReceiveFile) ;

// High level RF interface - Boots functions

int RFConnectAndSendSignOnRequest (char *SignOn, SNRStruct *SNR, int *bSuccess, ...) ;
int RFSendSignOffRequestAndDisconnect () ;
                  
//
// Send a message to the FlexOS base station. 
//
// *SendMessage = the address of a message structure of a type within RFHHTUnion.
//                        
// SendMessageLength = the length of the structure pointed to by *SendMessage
//
// *ReceiveMessage = the address of a RFHHTUnion for this function to write it's response to.
// (Interrogating the TID field of this union enables the type of response to be determined) 
//
// *bReturnToSignOn = the address of an integer flag which indicates that the application should 
//                    return to the sign on state
//                    NOTE This flag has to take precidence over whether or not the function 
//                         returns success.
//
// ... Valid reponse types are put here and are terminated with a NULL

int RFSendMessageWaitResponse (void *SendMessage, unsigned SendMessageLength,
                               void *ReceiveMessage, 
                               int *bReturnToSignOn,
                               ...) ;
                               
#endif
                               
