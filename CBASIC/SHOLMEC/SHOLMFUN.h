//*************************************************************************
//
// File   : SHOLMECFun.h
// Author : Ranjith Gopalankutty
// Created: 15-04-2018
// Purpose: Header file for SholmecFun.c
//
//-------------------------------------------------------------------------
// Version A: Ranjith Gopalankutty                            6th April 2015
//            Initial version
//
//************************************************************************/

#ifndef SHOLMECFUN_H

   #define SHOLMECFUN_H 
 #define PORTLIB

	   #include <Portab.h>
	   #include <flexdef.h>
	   #include <flextab.h>
	   #include <flexif.h>
	   #include <stdio.h>
	   #include <stdlib.h>
   

   // File level definitions
   #define FILE_HNDL                long
   #define FILE_NBYTES              long
   #define BINARY_READ              A_READ
   #define GP_SEEK_CUR              A_FPOFF
   #define DOS_EOF                  -1
   #define FILE_OPEN_ERROR          1
   #define FILE_READ_ERROR          2
   #define FILE_SEEK_ERROR          3
   #define LINE_READ_ERROR          4

   // Function for opening a file
   FILE_HNDL FileOpen( char * strFile, int iFlag );

   // Function for sequential file read
   FILE_NBYTES FileRead( FILE_HNDL iHandle, void * pvBuffer,
                      FILE_NBYTES iBuffSize ) ;

   // Function for file close
   int FileClose( FILE_HNDL iHndl ) ;

   // Function for reading a line from the file
   int FileReadLine( FILE_HNDL iHandle, void * pvBuffer, FILE_NBYTES iBuffSize );

   // Function for getting the file pointer position
   long FilePos( FILE_HNDL iHandle );

   
   long FileSize( BYTE *fname );
   
   // Function for changing the file pointer
   long FileSeek( FILE_HNDL iHandle, long lOffset, int iOrigin );

 
#endif /* #ifndef SHOLMECFUN_H*/

