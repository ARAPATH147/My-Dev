/*******************************************************************************
*
*  Module    : xshared.c
*
*  Desc.     : Source File for Common File Functions.
*
*              This module contains file access functions that can be called
*              by a BASIC program for accessing sequential files when running
*              under Supplementals (or more commonly known as Supps Mode).
*
*              BASIC provides limited file handling under Supps Mode and this
*              is the reason for providing these functions written in C.
*
*              More detail can be found in the Hybrid BASIC & C Programming
*              Guide.doc located in the EPoS Knowledge Base directory.
*
*
*  Author    : Charles Skadorwa
*
*  Date      : 3rd March 2014
*
*-------------------------------------------------------------------------------
* Version A         Charles Skadorwa                               03-Mar-2014
* Original version.
*
* Version A         Jaya Kumar Inbaraj                             04-Sep-2014
* FOD260 - Enhanced Backup and Recovery
* Prefix FUNC added to the functions
*
*******************************************************************************/

#include "xshared.h"

////////////////////////////////////////////////////////////////////////////////
//
// Function : OpenFile
//
// Purpose  : Open a generic file. It will make a pre-defined number of
//            attempts to open the file before timing out
//
// Parameter: Name of the File, Flag value
// Returns  : Unique File reference number
//
///////////////////////////////////////////////////////////////////////////////
LONG OpenFile ( BYTE *pbFname, UWORD uFlags )
{
    UBYTE ubFileConflict;
    WORD wRetryCnt;
    LONG lFnum;
    wRetryCnt = 0;

    do
    {
        lFnum = s_open ( uFlags, pbFname );

        if ( (ubFileConflict = ((lFnum&0xFFFF) == 0x400C)) != 0 )
        {
            s_timer ( 0, AC_RETRY_DELAY*1000L );
        }
    } while ( (ubFileConflict && wRetryCnt++ < AC_MAX_RETRY) != 0 );

    return lFnum;
}

///////////////////////////////////////////////////////////////////////////////
//
// Function  : OpenSequentialFile
//
// Purpose   : Open a generic sequential file
//
// Parameter : The file name to be opened in full path
//             Example: D:/ADX_UDT1/BKPSCRPT.TXT
//
// Returns   : SUCCESS -> File Handle
//             FAILURE -> -1L
//
///////////////////////////////////////////////////////////////////////////////
LONG OpenSequentialFile ( P_BASSTR pbsFileName )
{
    #pragma Alias ( OpenSequentialFile, "FUNC.OPEN.SEQUENTIAL.FILE" );

    LONG lFileHandle;
    BYTE abfileName[100];

    lFileHandle = -1L;

    memset (abfileName, '\0', sizeof(abfileName));
    //Null or empty string passed?
    if ( (pbsFileName == NULL) || pbsFileName->uwLen == 0 )
    {
        lFileHandle = -1L;
    }
    else
    {
        memcpy (abfileName, pbsFileName->bChar, pbsFileName->uwLen );

        lFileHandle = OpenFile ( abfileName, A_READ | A_SHARE | A_WRITE );

        if ( lFileHandle <= 0L )
        {
            lFileHandle = -1L;
        }
    }
    return C32RETURN(lFileHandle);
}

////////////////////////////////////////////////////////////////////////////////
//
// Function   : ReadSequentialFile
//
// Purpose    : Read the next line/record according to current file pointer
//              position. Newline '\n' character marks end of line. This
//              function reads in a byte at a time.
//
// Parameters : File Handle
//
//              Reference: Hybrid BASIC & C Programming Guide.doc
//              [2.5.4 Returning BASIC Strings from C Functions]
//
// Returns    : Void
//              The line/record is availanble in the BYTE array; abBuffer.
//              The read errors including end-of-file will be denoted by
//              a null value.
//
////////////////////////////////////////////////////////////////////////////////
VOID ReadSequentialFile ( LONG lFnum )
{
    #pragma Alias (ReadSequentialFile, "FUNC.READ.SEQUENTIAL.FILE");

    BOOLEAN blReadWholeLine = FALSE;

    BYTE    abBuffer[16000];
    BYTE    cByte;
    BYTE    *pByte;
    LONG    lRc;
    WORD    wCnt;

    lRc    = 1;
    wCnt   = 0;

    memset (abBuffer, 0x0, sizeof(abBuffer));
    pByte  = &abBuffer[0];
    *pByte = NULL;
    cByte  = '\0';

    lRc = s_read ( A_FPOFF, lFnum, &cByte, 1L, 0 );

    // Handles non-empty records
    while ( lRc == 1 && !blReadWholeLine )
    {
        if ( cByte != '\r' )
        {
            *(pByte + wCnt) = cByte;
             wCnt++;
        }
        else
        {
            // Handles the blank lines in the file if any
            // (Considered CR and LF)

            if (wCnt == 0)
            {
                *(pByte + wCnt) = cByte;
                wCnt++;
            }
            blReadWholeLine = TRUE;
            *(pByte+wCnt) = '\0';
        }

        lRc = s_read ( A_FPOFF, lFnum, &cByte, 1L, 0 );
    }

    BASRETSTR ( wCnt, &abBuffer );
}

////////////////////////////////////////////////////////////////////////////////
//
// Function   : WriteSequentialFile
//
// Purpose    : Write the next record relative to end of file position.
//              The pBuffer must be NULL terminated in order
//              that the record length can be determined of the variable
//              length record.
//
// Parameters : 1. File Handle
//              2. The record to be written including CR and LF characters
//
// Returns    : SUCCESS -> Number of Bytes written
//              FAILURE -> -1L
//
////////////////////////////////////////////////////////////////////////////////
LONG WriteSequentialFile ( LONG lFnum, P_BASSTR pbFileRecord )
{
    #pragma Alias ( WriteSequentialFile, "FUNC.WRITE.SEQUENTIAL.FILE" );

    LONG lBytesWritten;

    lBytesWritten = -1L;

    //Null or empty string passed?
    if ((pbFileRecord == NULL) || pbFileRecord->uwLen == 0)
    {
        lBytesWritten = -1L;
    }
    else
    {
        lBytesWritten  = s_write ( A_EOFOFF,         // Relative to end of file
                                   lFnum,                  // File no.
                                   pbFileRecord->bChar,    // Record
                                   pbFileRecord->uwLen,    // Record Length
                                   0 );                    // File Offset

    }

    if ( lBytesWritten <= 0L )
    {
        lBytesWritten = -1L;
    }

    return C32RETURN ( lBytesWritten );

}

///////////////////////////////////////////////////////////////////////////////
//
// Function   : CloseFile
//
// Purpose    : Close a generic file
//
// Parameters : File Handle
//
// Returns    : VOID
//
///////////////////////////////////////////////////////////////////////////////
VOID CloseFile ( LONG lFnum )
{
    #pragma Alias ( CloseFile, "FUNC.CLOSE.FILE" );
    s_close ( FULL_CLOSE, lFnum );
}
