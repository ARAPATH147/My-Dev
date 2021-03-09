//*************************************************************************
//
// File   : SHOLMECFUN.c
// Author : Ranjith Gopalankutty
// Created: 15-April - 2018
//
// Purpose: Low level file functions are defined here
//
// 

#include "SHOLMEC.H"
#include "SHOLMFUN.H"
#include <types.h>
#include <string.h>


///////////////////////////////////////////////////////////////////////////////
//
// Function: FileOpen
//
// Purpose : opens a file and returns the file handle
//
///////////////////////////////////////////////////////////////////////////////

FILE_HNDL FileOpen( char * strFile, int iFlag )
{
   FILE_HNDL rc;

   rc = s_open( iFlag, strFile ) ;

   return rc;
}

///////////////////////////////////////////////////////////////////////////////
//
// Function: FileRead
//
// Purpose : sequential file read
//
///////////////////////////////////////////////////////////////////////////////

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

///////////////////////////////////////////////////////////////////////////////
//
// Function: FileSeek
//
// Purpose : changes the file pointer
//
///////////////////////////////////////////////////////////////////////////////

long FileSeek( FILE_HNDL iHandle, long lOffset, int iOrigin )
{
   FILE_HNDL rc;

   rc = s_seek ( iOrigin, iHandle, lOffset ) ;

   return rc;
}

///////////////////////////////////////////////////////////////////////////////
//
// Function: FilePos
//
// Purpose : returns current file pointer
//
///////////////////////////////////////////////////////////////////////////////
long FilePos( FILE_HNDL iHandle )
{
   FILE_HNDL rc ;

   rc = s_seek ( GP_SEEK_CUR, iHandle, 0L ) ;

   return rc;
}

///////////////////////////////////////////////////////////////////////////////
//
// Function: FileReadLine
//
// Purpose : returns a line which ends with CRLF
//
///////////////////////////////////////////////////////////////////////////////
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

///////////////////////////////////////////////////////////////////////////////
//
// Function: FileClose
//
// Purpose : Function for closing a  file
//
///////////////////////////////////////////////////////////////////////////////
int FileClose( FILE_HNDL iHndl )
{

   return s_close( 0, iHndl );
}

///////////////////////////////////////////////////////////////////////////////
//
// Function: FileSize
//
// Purpose : returnd file size of a file (only accurate below 32 bits)
//
///////////////////////////////////////////////////////////////////////////////
long FileSize( BYTE *fname )
{
    LONG rc;
    DISKFILE dir;
    
    rc = s_lookup( T_FILE,
                   A_FORCE,
                   fname,
                   (void *)&dir,
                   sizeof(DISKFILE),
                   sizeof(DISKFILE),
                   0L );
				   
				   
    if (rc>0)  {
         
		 
		return(dir.df_size);  // found file
		
    }
    else
    {
		 
        return(-1); // file not found
    }
}


