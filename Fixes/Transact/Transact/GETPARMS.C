//
// M.Gardiner, 15/7/97
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
//    char cType ;    - type 
//       - presently supported :-
//          - 'B' - binary data e.g. a number 15
//          - 'C' - a character e.g. '@'
//                        - 'Z' - null terminated string e.g. "hello"
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
//    POLLVARS
//
//    POLL_FILE   = "C:TLOG01"                                                                         
//    ASSIGN_FLAG = 'Y' 
//          RETRY_COUNT     = 99
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
#include "transact.h" //SDH 19-May-2006

#include "getparmx.h"
#include "util.h"

//SDH char pcReadBuffer[READ_BUFFER_SIZE];

int GetParms( char * strCntrlFile , char * strCntrlField, 
              KEYENTRY * pKeyTable, int iStructSize, int iNumOfFields )
{
   FILE_HNDL iHndl ;
   int iRc, iFieldIndex ;
   char * pcStart, pcLine[LINE_SIZE]; 
   char pcReadBuffer[READ_BUFFER_SIZE];
   char pstrNodeName[ NODE_NAME_SIZE ] = { 0} ;
   char pcKeyWord[FIELD_NAME_SIZE], pcKeyValue[FIELD_VALUE_SIZE]; 
   char cEOF = 0, cFoundControlField = 0, cLineEnd, cQuitParse = 0 ;

   //Touch iStructSize to prevent compiler error
   touch(iStructSize);

   if ( ( iHndl = FileOpen( strCntrlFile, BINARY_READ ) ) == -1 ) {
      return FILE_OPEN_ERROR ;
   }

   // if no control field specified then we'll parse the whole file
   if ( strcmp( strCntrlField, "" ) == 0 ) {
      cFoundControlField = 1 ;
   }

   while ( ( iRc = ReadUncommentedText( iHndl, pcReadBuffer, READ_BUFFER_SIZE, 
                                        &pcStart, &cEOF ) ) == 0 ) {
      if ( cEOF ) {
         break ;
      }

      if ( cQuitParse ) {
         break ;
      }

      if ( pcStart == 0 ) {
         continue ;
      }

      // loop thro the buffer extracting lines     
      //
      cLineEnd = 0 ;
      //                                      
      // Once we have found the control field - if required then parse any 
      // further fields writing them to the key entry structure - if they are
      // valid
      //
      while ( ( iRc = GetLine( &pcStart, pcLine, LINE_SIZE, &cLineEnd ) ) 
              == 0 ) {
         if ( cLineEnd ) {
            break ;
         }

         // re-loop thro buffer till we get to our control field
         //                                                     
         if ( cFoundControlField == 0 ) {
            if ( IsOurControlField( pcLine, strCntrlField ) == 0 ) {
               cFoundControlField = 1;
            }
            continue ;
         }

         //
         // we've have found our control field  
         //   
         else {
            // see if we've got to a new control field - inwhich case quit
            // the parse  
            if ( IsNewControlField( pcLine ) == 0 ) {
               cQuitParse = 1 ;
               break;
            }

            if ( IsNodeIdentifier( pcLine, pstrNodeName ) == 0 ) {
               continue ;                          
            }

            // See if this line is a keyword and value 
            // i.e. <KEYWORD> = <KEYVALUE>
            // 
            if ( ParseKeyWord( pcLine, pcKeyWord, pcKeyValue ) != 0 ) {
               continue ;                 
            }

            //
            // see if this key field is in the key table structure passed
            // to us
            // 
            if ( KeyWordInTable( pcKeyWord, pKeyTable, &iFieldIndex, 
                                 iNumOfFields ) != 0 ) {
               continue ;
            }

            //
            // check if we are now parsing node-dependent information
            //       
            if ( pstrNodeName[ 0 ] != 0 ) {
               if ( strcmp( pstrNodeName, GetNodeName( ) ) == 0 ) {
                  AddValuetoTable( pKeyTable[iFieldIndex] , pcKeyValue );              
               }
            } else {
               AddValuetoTable( pKeyTable[iFieldIndex] , pcKeyValue );
            }              
         }     
      }

      if ( iRc != 0 ) {
         FileClose( iHndl );  

         return iRc ;
      }
   }

   FileClose( iHndl );

   return 0;
}


//
// Extracts a keyword and value from the line 
//                                            
// returns 0 if successful
//
int ParseKeyWord( char * pcLine, char * pcKeyWord, char * pcKeyValue )
{
   char * pc ;

   // find the separator - i.e. = sign
   //                                 
   if ( ( pc = strchr( pcLine, FIELD_SEPARATOR ) ) == NULL ) {
      return -1 ;
   }

   if ( strlen( pc + 1 ) >= FIELD_VALUE_SIZE ) {
      return -1 ;
   }


   if ( sscanf( pc + 1 , "%s", pcKeyValue ) != 1 ) {
      return -1 ;
   }

   pc = 0 ;                  

   if ( strlen( pcLine ) >= FIELD_NAME_SIZE ) {
      return -1 ;    
   }

   if ( sscanf( pcLine, "%s", pcKeyWord ) != 1 ) {
      return -1 ;
   }
   return 0;
}

//
// Returns 0 if the keyword specified is in our table
// 
// If the keyword is present then * piIndex is populated with the index
// into the key table array where this key word is defined
//
int KeyWordInTable( char * strKeyWord, KEYENTRY * pTable, int * piIndex, 
                    int iNumOfEntries )
{                                                                        

   for ( ( * piIndex ) = 0 ; ( * piIndex ) < iNumOfEntries ; ( * piIndex ) ++ ) {
      if ( strcmp( strKeyWord, pTable[ ( * piIndex ) ].strKeyWord ) == 0 ) {
         return 0 ;
      }
   }

   return -1 ;
}    

//
// Adds a key value to our table 
//                
void AddValuetoTable( KEYENTRY KeyTable, char * strKeyValue )
{        
   char * pc ;                      
   int iWork ;

   memset( ( char * ) KeyTable.pvAddr, 0, ( size_t ) KeyTable.cLen );

   switch ( KeyTable.cType ) {
   case T_STRING :
      {
         if ( ( pc = strrchr( strKeyValue, D_QUOTE ) ) != NULL ) {
            * pc = 0 ;

            strncpy( ( char * ) KeyTable.pvAddr, &strKeyValue[1], 
                     KeyTable.cLen - 1 ) ;
         }
         break ;
      }

   case T_FLAG :
      {         
         * ( ( char * ) KeyTable.pvAddr ) = strKeyValue[1] ;

         break ;
      }

   case T_BINARY :
      {
         if ( KeyTable.cLen == sizeof( int ) ) {
            sscanf( strKeyValue, "%d", ( int * ) KeyTable.pvAddr ) ;
         }

         else if ( KeyTable.cLen == sizeof( long ) ) {
            sscanf( strKeyValue, "%ld", ( long * ) KeyTable.pvAddr ) ;
         }

         //                      
         // if the data size is not does not fall into the above categories
         // then do not attempt to scan more than one byte of it. (In case
         // we should overflow this address).
         //  
         else {
            sscanf( strKeyValue, "%d", &iWork ) ;

            * ( ( char *) KeyTable.pvAddr ) = ( char ) iWork ;
         }      

         break ;
      }
   }
}

//
// Gets a line from the buffer pointed to by * ppcStart, modifies the contents
// of * ppcStart with the position of the next line. Copies the line into the
// buffer pcLine
//              
// returns 0 if a line was successfully read
//
int GetLine( char ** ppcStart, char * pcLine, int iBuffSize, char * pcEnd )
{  
   char * pc, * pcNull ;

   //Touch iBuffSize to prevent compiler error
   touch(iBuffSize);

   // end of read buffer
   if ( ** ppcStart == 0 ) {
      * pcEnd = 1 ;

      return 0;
   }

   // find the '\r' char             
   if ( ( pc = strchr( * ppcStart, '\r' ) ) != NULL ) {
      // null term here
      //               
      * pc = 0 ;

      if ( * ( pc + 1 ) == '\n' ) {
         pcNull = pc + 2 ;
      } else {
         pcNull = pc + 1 ;
      }
   }

   else if ( ( pc = strchr( * ppcStart, '\n' ) ) != NULL ) {
      * pc = 0 ;

      pcNull = pc + 1 ;
   }

   else if ( ( pc = strchr( * ppcStart, '\0' ) ) != NULL ) {
      pcNull = pc ;
   }

   else {
      return ERROR_PARSING_LINE ;
   }

   memset( pcLine, 0, LINE_SIZE );        

   strncpy( pcLine, * ppcStart, LINE_SIZE - 1 );                     

   * ppcStart = pcNull ;

   return 0;
}


//
// return 0 if the line parsed is our control field
//                         
int IsOurControlField( char * pcLine, char * strCntrl )
{
   char strWork[FIELD_NAME_SIZE];

   if ( sscanf( pcLine, "%s", strWork ) == 1 ) {
      if ( strcmp( strWork, strCntrl ) == 0 ) {
         return 0 ;
      }
   }

   return -1 ;
}

//
// return 0 if the line parsed is a new control field
//                         
int IsNewControlField( char * pcLine )
{
   char strWork[FIELD_NAME_SIZE];

   if ( strchr( pcLine, FIELD_SEPARATOR ) == NULL ) {
      if ( sscanf( pcLine, "%s", strWork ) == 1 ) {
         if ( ( strWork[0] >= 'A' ) && ( strWork[0] <= 'Z' ) ) {
            return 0 ;
         }
      }
   }

   return -1 ;
}

//
// return 0 if the char * string is a node identifier heading a block of
// node dependant information and populates the pcNodeName var with the node 
// name less the # 
//
int IsNodeIdentifier( char * pcLine, char * pcNodeName)
{                                                   
   char pcLineBuffer[ LINE_SIZE ] = { 0} ;

   sscanf( pcLine, "%s", pcLineBuffer );

   if ( pcLineBuffer[ 0 ]  != NODE_ID_ID ) {
      return -1 ;
   }

   if ( ( strlen( pcLineBuffer ) - 1 ) <= ( NODE_NAME_SIZE - 1 ) ) {
      strcpy( pcNodeName, &pcLineBuffer[1] ) ;
   }

   return 0 ;
}

//
// read sections of uncommented text from the file pointed to by iHndl
// populate *ppcStart with the start of the block. Sections will be NULL-
// terminated.
//
// There are four scenarios dealt with...
//
// 1. The block read was totally uncommented. 
// Populate *ppcStart with pcReadBuffer
//
// 2. Comment start halfway into the buffer - can or cannot find the 
// comment end.
// Populate *ppcStart with pcReadBuffer, NULL terminate where the comment
// start character is read, move file handle to point to the byte where
// comment starts 
//
// 3. Comment start at first byte - comment end in buffer.
// Make *ppcStart=0 and move file handle to the next byte after the comment
// end.
//
// 4. Comment start at first byte but cant find end - reread from file
//
// Returns 0 if OK otherwise an error code
//
int ReadUncommentedText( FILE_HNDL iHndl, char * pcReadBuffer, 
                         FILE_NBYTES iBuffSize, char ** ppcStart, char * pcEOF )
{                                                      
   FILE_NBYTES iBytesRead ;
   char * pcCommentChar, * pcCommentEnd, cCommentStyle, cReread = 0 ;

   for ( ; ; ) {
      memset( pcReadBuffer, 0, iBuffSize);


      iBytesRead = FileRead( iHndl, pcReadBuffer, iBuffSize - 1 ) ;

      if ( iBytesRead == DOS_EOF ) {
         * pcEOF = 1 ;
         return ( iBytesRead == DOS_EOF ) ? 0 : FILE_READ_ERROR ;
      }

      // if we are not re-reading then search for a comment start character
      if ( !cReread ) {
         // scenario 1.
         //
         if ( ( pcCommentChar = strchr( pcReadBuffer, COMMENT1 ) ) == NULL ) {
            // no comments!!
            * ppcStart = pcReadBuffer ;      

            // null term the end
            pcReadBuffer[ iBytesRead ] = 0 ;      

            return 0;
         }

         // See what type of comments we have here
         cCommentStyle = ( char ) ( * ( pcCommentChar + 1 ) == COMMENT1 ) ?
                         ( char )NEW_STYLE_COMMENT :
                         ( ( * ( pcCommentChar + 1 ) == COMMENT2 ) ?
                           ( char ) OLD_STYLE_COMMENT : ( char ) BAD_COMMENT ) ;

         if ( cCommentStyle == BAD_COMMENT ) {
            return BAD_COMMENT ;
         }

         // scenario 2.
         //
         if ( pcCommentChar != pcReadBuffer ) {
            * ppcStart = pcReadBuffer ;

            * pcCommentChar = 0 ;

            // move handle back 
            //            
            if ( FileSeek( iHndl, ( long ) - ( iBytesRead - ( int ) ( pcCommentChar - pcReadBuffer ) ), 
                           GP_SEEK_CUR ) == -1 ) {
               return FILE_SEEK_ERROR ;
            }

            return 0;
         }
      }

      FindCommentEnd( cReread ? pcReadBuffer : pcCommentChar + 2 , 
                      &pcCommentEnd, cCommentStyle );      

      // scenario 3.
      //
      if ( pcCommentEnd != 0 ) {
         * ppcStart = 0 ;

         // move handle back 
         //            
         if ( FileSeek( iHndl, 
                        ( long ) - ( iBytesRead - ( int ) ( pcCommentEnd - pcReadBuffer ) ), 
                        GP_SEEK_CUR ) == -1 ) {
            return FILE_SEEK_ERROR ;
         }

         return 0;
      }

      // if we have got this far down without returning then we need to 
      // re-read from the file as we didnot find the comment end             
      cReread = 1 ;
   }

}

//
// Populates a char * with the postion of the next byte after a comment
// end 
//                
void FindCommentEnd( char * pcStart, char ** ppcEnd, char cCommentStyle )
{
   * ppcEnd = 0 ;

   if ( cCommentStyle == OLD_STYLE_COMMENT ) {
      // if we can find the end then move the pointer on 2
      if ( ( * ppcEnd = strstr( pcStart, OLD_COMMENT_END ) ) != NULL ) {
         * ppcEnd = ( * ppcEnd ) + 2 ;
      }
   } else {
      //
      //    
      if ( ( * ppcEnd = strchr( pcStart, '\r' ) ) != NULL ) {
         if ( * ( ( * ppcEnd ) + 1 ) == '\n' ) {
            * ppcEnd = ( * ppcEnd ) + 2 ;
         } else {
            * ppcEnd = ( * ppcEnd ) + 1 ;
         }
      } else if ( ( * ppcEnd = strchr( pcStart, '\n' ) ) != NULL ) {
         * ppcEnd = ( * ppcEnd ) + 1 ;         
      }
   }
}  

//
// returns the local machines node name - as read from c:\syboot
//                                      
const char * GetNodeName( void )
{
   static char pstrNodeName[ NODE_NAME_SIZE ] = { 0} ;
   char pcBuffer[ NODE_NAME_SIZE ] = { 0} ;
   int iHndl ;

   if ( access( SYBOOT, EXISTS ) == 0 ) {
      if ( pstrNodeName[0] == 0 ) {
         if ( ( iHndl = FileOpen( SYBOOT, BINARY_READ ) ) != -1 ) {

            if ( FileRead( iHndl, pcBuffer, NODE_NAME_SIZE - 1 ) != -1 ) {
               sscanf( pcBuffer, "%s", pstrNodeName );               
            }

            FileClose( iHndl ) ;
         }
      }
   }

   return ( const char * ) pstrNodeName ;
}

// implementation dependent file routines 
//             
// FLEXOS development 
//
#ifdef NEW_FLEXOS_GETPARMS

FILE_HNDL FileOpen( char * strFile, int iFlag ) 
{
   FILE_HNDL rc;

   rc = s_open( iFlag, strFile ) ;

   return rc;
}

FILE_NBYTES FileRead( FILE_HNDL iHandle, void * pvBuffer, 
                      FILE_NBYTES iBuffSize ) 
{
   FILE_HNDL rc;

   rc = s_read ( A_FPOFF, iHandle, pvBuffer, iBuffSize, 0L ) ; 
   if ( rc < 0 ) {
      rc = -1 ;
   }

   return rc;
}

long FileSeek( FILE_HNDL iHandle, long lOffset, int iOrigin ) 
{ 
   FILE_HNDL rc;

   rc = s_seek ( iOrigin, iHandle, lOffset ) ;

   return rc;
}

long FilePos( FILE_HNDL iHandle )
{
   FILE_HNDL rc ;

   rc = s_seek ( GP_SEEK_CUR, iHandle, 0L ) ; 

   return rc;
}

// fgets type function
int FileReadLine( FILE_HNDL iHandle, void * pvBuffer, FILE_NBYTES iBuffSize )
{
   int iBytes ;
   char * pc, * pcNext = NULL ;

   memset( pvBuffer, 0, iBuffSize );


   if ( ( iBytes = FileRead( iHandle, pvBuffer, iBuffSize - 1) ) == -1 ) {
      return -1;
   }

   if ( iBytes == DOS_EOF ) {
      return DOS_EOF ;
   }

   if ( ( pc = strchr( pvBuffer, '\r') ) != NULL ) {
      if ( * ( pc + 1) == '\n' ) {
         pcNext = pc + 2 ;            
      } else {
         pcNext = pc + 1 ;            
      }
   }

   else if ( ( pc = strchr( pvBuffer, '\n') ) != NULL ) {
      pcNext = pc + 1 ;            
   }

   // move file pointer back to just past line break;
   //
   if ( pcNext != NULL ) {
      if ( FileSeek( iHandle, ( long ) - ( iBytes - ( int ) ( pcNext - (char *) pvBuffer ) ), 
                     GP_SEEK_CUR ) == -1 ) {
         return FILE_SEEK_ERROR ;
      }

      * pcNext = ( char ) 0 ;
   }

   return iBytes ;
}

int FileClose( FILE_HNDL iHndl ) 
{

   return s_close( 0, iHndl );
}

#endif

//             
// DOS/Win95 development 
//
#ifdef NEW_DOS_GETPARMS

int FileOpen( char * strFile, int iFlag ) 
{
   return open( strFile, iFlag);
}

int FileRead( int iHandle, void * pvBuffer, int iBuffSize ) 
{
   return read( iHandle, pvBuffer, iBuffSize ) ;
}

long FileSeek( int iHandle, long lOffset, int iOrigin ) 
{
   return lseek( iHandle, lOffset, iOrigin ) ;
}

long FilePos( int iHandle )
{
   return lseek( iHandle, 0, GP_SEEK_CUR ) ;
}

// fgets type function
int FileReadLine( int iHandle, void * pvBuffer, int iBuffSize )
{
   int iBytes ;
   char * pc, * pcNext = NULL ;

   memset( pvBuffer, 0, iBuffSize );


   if ( ( iBytes = FileRead( iHandle, pvBuffer, iBuffSize - 1) ) == -1 ) {
      return -1;
   }

   if ( iBytes == DOS_EOF ) {
      return DOS_EOF ;
   }

   if ( ( pc = strchr( pvBuffer, '\r') ) != NULL ) {
      if ( * ( pc + 1) == '\n' ) {
         pcNext = pc + 2 ;            
      } else {
         pcNext = pc + 1 ;            
      }
   }

   else if ( ( pc = strchr( pvBuffer, '\n') ) != NULL ) {
      pcNext = pc + 1 ;            
   }

   // move file pointer back to just past line break;
   //
   if ( pcNext != NULL ) {
      if ( FileSeek( iHandle, ( long ) - ( iBytes - ( int ) ( pcNext - (char *) pvBuffer ) ), 
                     GP_SEEK_CUR ) == -1 ) {
         return FILE_SEEK_ERROR ;
      }

      * pcNext = ( char ) 0 ;
   }

   return iBytes ;
}

int FileClose( int iHndl ) 
{
   return close( iHndl );
}

#endif

// FTS development 
//
#ifdef NEW_FTS_GETPARMS

int FileOpen( char * strFile, int iFlag ) 
{
   return open2( strFile, iFlag);
}

int FileRead( int iHandle, void * pvBuffer, int iBuffSize ) 
{
   return read2( iHandle, pvBuffer, iBuffSize ) ;
}

long FileSeek( int iHandle, long lOffset, int iOrigin ) 
{
   return lseek2( iHandle, lOffset, iOrigin ) ;
}

long FilePos( int iHandle )
{
   return lseek2( iHandle, 0, GP_SEEK_CUR ) ;
}

// fgets type function
int FileReadLine( int iHandle, void * pvBuffer, int iBuffSize )
{
   int iBytes ;
   char * pc, * pcNext = NULL ;

   memset( pvBuffer, 0, iBuffSize );


   if ( ( iBytes = FileRead( iHandle, pvBuffer, iBuffSize - 1) ) == -1 ) {
      return -1;
   }

   if ( iBytes == DOS_EOF ) {
      return DOS_EOF ;
   }

   if ( ( pc = strchr( pvBuffer, '\r') ) != NULL ) {
      if ( * ( pc + 1) == '\n' ) {
         pcNext = pc + 2 ;            
      } else {
         pcNext = pc + 1 ;            
      }
   }

   else if ( ( pc = strchr( pvBuffer, '\n') ) != NULL ) {
      pcNext = pc + 1 ;            
   }

   // move file pointer back to just past line break;
   //
   if ( pcNext != NULL ) {
      if ( FileSeek( iHandle, ( long ) - ( iBytes - ( int ) ( pcNext - (char *) pvBuffer ) ), 
                     GP_SEEK_CUR ) == -1 ) {
         return FILE_SEEK_ERROR ;
      }

      * pcNext = ( char ) 0 ;
   }

   return iBytes ;
}

int FileClose( int iHndl ) 
{
   return close2( iHndl );
}

#endif
