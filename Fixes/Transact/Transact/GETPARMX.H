//
// getparmx.h - definitions for getparms - i.e. to compile C code
//
// Applications should use getparms.h to interface to the function
//
//#define NEW_DOS_GETPARMS

#ifndef GETPARMX_H
#define GETPARMX_H

#define NEW_FLEXOS_GETPARMS

// FLEXOS defines
//   
#ifdef NEW_FLEXOS_GETPARMS
   #include <stdio.h>
   #include <stdlib.h>
   #include <flexif.h>
   #include <string.h>
   
   #define FILE_HNDL             long 
   #define FILE_NBYTES           long
   #define BINARY_READ           A_READ
   #define GP_SEEK_CUR           A_FPOFF
   #define DOS_EOF               -1
#endif
   
// DOS/Win95 defines
//   
#ifdef NEW_DOS_GETPARMS
   #include <stdio.h>
   #include <stdlib.h>
   #include <io.h>
   #include <string.h>
   #include <fcntl.h>
   
   #define FILE_HNDL             int 
   #define FILE_NBYTES           int
   #define BINARY_READ           _O_RDONLY|_O_BINARY
   #define GP_SEEK_CUR           SEEK_CUR
   #define DOS_EOF               0
#endif

#ifdef NEW_FTS_GETPARMS
   #include <stdio.h>
   #include <stdlib.h>
   #include <io.h>
   #include <string.h>
   #include <fcntl.h>
   #include "sdlccs50.h"

   #define FILE_HNDL             int
   #define FILE_NBYTES           int
   #define BINARY_READ           0
   #define GP_SEEK_CUR           1
   #define DOS_EOF               0
#endif

#define FILE_OPEN_ERROR          1
#define FILE_READ_ERROR          2
#define FILE_SEEK_ERROR          3
#define LINE_READ_ERROR          4
#define CONTROL_FIELD_NOT_FOUND  5
#define BAD_COMMENT              6
#define END_COMMENT_ERROR        7
#define ERROR_PARSING_LINE       8

#define READ_BUFFER_SIZE         256
#define LINE_SIZE                256
#define FIELD_VALUE_SIZE         LINE_SIZE
#define FIELD_NAME_SIZE          LINE_SIZE - 1
#define NODE_NAME_SIZE           16
#define getparms                 GetParms

#define COMMENT1                 '/'
#define COMMENT2                 '*'
#define FIELD_SEPARATOR          '='
#define D_QUOTE                  '"'
#define NODE_ID_ID               '#' 
#define S_QUOTE                  0x27

#define OLD_COMMENT_END          "*/"             

#define T_STRING                 'Z'
#define T_BINARY                 'B'
#define T_FLAG                   'C'

#define OLD_STYLE_COMMENT         0
#define NEW_STYLE_COMMENT         1

#define SYBOOT                    "C:\\SYBOOT"

#define EXISTS                    0 

extern int   access(char *path,int mode);  //SDH 21-June-2006

typedef struct 
{
   char * strKeyWord;
   char cType ;
   char cLen ;
   void * pvAddr ;
} KEYENTRY ;                      

const char * GetNodeName( void ) ;

void FindCommentEnd( char * pcStart, char ** ppcEnd, char cCommentStyle );
void AddValuetoTable( KEYENTRY KeyTable, char * strKeyValue );

int GetParms( char * strCntrlFile , char * strCntrlField, 
              KEYENTRY * pKeyTable, int iStructSize, int iNumOfFields );
int ReadUncommentedText( FILE_HNDL iHndl, char * pcReadBuffer, 
                         FILE_NBYTES iBuffSize, char ** ppcStart, char * pcEOF );
int GetLine( char ** ppcStart, char * pcLine, int iBuffSize, char * pcEnd );
int IsOurControlField( char * pcLine, char * strCntrl );
int IsNewControlField( char * pcLine );
int IsNodeIdentifier( char * pcLine, char * pcNodeName) ;
int ParseKeyWord( char * pcLine, char * pcKeyWord, char * pcKeyValue );
int KeyWordInTable( char * strKeyWord, KEYENTRY * pTable, int * piIndex, 
                    int iNumOfEntries );

FILE_HNDL FileOpen( char * strFile, int iFlag ); 
FILE_NBYTES FileRead( FILE_HNDL iHandle, void * pvBuffer, 
                      FILE_NBYTES iBuffSize ) ;
int FileClose( FILE_HNDL iHndl ) ;
int FileReadLine( FILE_HNDL iHandle, void * pvBuffer, FILE_NBYTES iBuffSize );

long FilePos( FILE_HNDL iHandle );
long FileSeek( FILE_HNDL iHandle, long lOffset, int iOrigin ); 

#endif

