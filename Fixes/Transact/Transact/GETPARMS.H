//
// getparms.h
//
// Interface to function "getparms"
//
//
// getparms.c - contains the definitions of a routine designed to 
// parse ascii control files. The control file should be of the following
// format :-
//
// CONTROL1 
//    FIELD_NUMBER = 2
//    CM_MESSAGE   = "Hello world"
//    CONTROL_CHAR = 'C'
//
// CONTROL2 
//    FIELD_NUMBER = 15
//    CM_MESSAGE   = "Goodbye world"
//    CONTROL_CHAR = 'H'
//    TS_MESSAGE   = "Cheers!"
//
// The parsing of node dependent information is also possible - node dependent
// control words can be flagged by the placement of a node control name 
// (#<node-name>) preceding the key word and key value pair e.g. VFM.CTL 
//
// VFMSERV
//    CONCMODE = 1
//    
//    #CTLR
//       DRIVEIDS = 'X'
//    #ADMIN
//       DRIVEIDS = 'Y' 
//
//    ....
//
// This ensures that the driveids value is different on the different nodes 
// without having to have different variables defined for these nodes.
//
// The file is of ascii format and comprises of key field names and values.
// these fields may of may not be headed by control fields. Heading groups of
// key fields allows the routine to parse only those key fields after the 
// specified control field and before the next control field.
//
// If no control field specified then the routine will parse all the data in 
// the file until a control field is found.
// 
// The routine is called with :-
// strCntrlFile - name of the control file
// strCntrlField - name of the control field to parse from 
// pKeyTable - the address of a table of key entry structures
//    this holds information required by getparms for each field parsed            
// iStructSize - this is the size of a key entry structure
// iNumOfFields - this is maximum number of fields which getparms can parse
//    and should not be more than the number of key entry structures that 
//    pKeyTable points to.
//                        
// The key entry structure defines each key field to input and is as follows:-
//
// typedef struct 
// {                                     
//
//    char * strKeyword;   - name of the control field used in the ascii file
//                         - no longer than 31 please
//
//    char cType ;         - type 
//                         - presently supported :-
//                         - 'B' - binary data e.g. a number 15
//                         - 'C' - a character e.g. '@'
//                         - 'Z' - null terminated string e.g. "hello"
//
//    char cLen ;          - max length of data from the ascii file to parse                             
//
//    void * pvAddr ;      - the address which to write this data 
//
// } KEYENTRY ;                      
//
// Example :- We would like extract a filename, a character flag and the 
// number of retries from a control file called POLL.CTL.
//                                                    
// 1. Setup the POLL.CTL as follows :-
//
// POLLVARS
//
//    POLL_FILE      = "C:TLOG01"                                                                         
//    ASSIGN_FLAG    = 'Y'
//    RETRY_COUNT    = 99
//
// 2. Setup a table of key entry structures as follows :-
//                                                       
// #defines ENTRIES 3
//                   
// char strFile[12];
// char cFlag;
// int i;
//
// KEYENTRY KeyTable[ENTRIES] = 
//    {  { "POLL_FILE", 'Z', 12, ( void * ) strFile}, 
//       { "ASSIGN_FLAG", 'C', 1, ( void * ) &cFlag }, 
//       { "RETRY_COUNT", 'B', sizeof(int), ( void * ) &i } };
//
// 3. Call the 'C' code like this
//
//    iRc = GetParms( "C:POLL.CTL", "POLLVARS", KeyTable, sizeof(KEYENTRY),
//                     ENTRIES ); 
//
// Return of 0 indicates success else error codes as follows:-
//
// FILE_OPEN_ERROR          1
// FILE_READ_ERROR          2
// FILE_SEEK_ERROR          3
// LINE_READ_ERROR          4
// CONTROL_FIELD_NOT_FOUND  5
// BAD_COMMENT              6
// END_COMMENT_ERROR        7
// ERROR_PARSING_LINE       8
// 
// Note1 that when parsing binary values - field type 'B', if you want the data
// to be written to char then set cLen=1, if you want the data written to an
// int set cLen=sizeof(int), if you want the data written to a long set 
// cLen=sizeof(long)
//                         
// Note2 a call to this function may increase stack usage by about 512 bytes
// during the duration of the call
//

#ifndef GETPARMS_H
#define GETPARMS_H

#define FIELD_NAME_SIZE          32 // don't make keywords longer than 31

#define T_STRING                 'Z'
#define T_BINARY                 'B'
#define T_FLAG                   'C'

//
// errors!!!
//
#define FILE_OPEN_ERROR          1
#define FILE_READ_ERROR          2
#define FILE_SEEK_ERROR          3
#define LINE_READ_ERROR          4
#define CONTROL_FIELD_NOT_FOUND  5
#define BAD_COMMENT              6
#define END_COMMENT_ERROR        7
#define ERROR_PARSING_LINE       8

typedef struct 
{
   char * strKeyWord;
   char cType ;
   char cLen ;
   void * pvAddr ;
} KEYENTRY ;                      

int GetParms( char * strCntrlFile , char * strCntrlField, 
              KEYENTRY * pKeyTable, int iStructSize, int iNumOfFields );

#endif

