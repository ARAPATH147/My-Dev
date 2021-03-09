/*************************************************************************
* 
* 
* File: output.c
*
* Author: Prashant Kotak
*
* Created: 18/08/97
*
* Purpose: 
*
* History: 
* 
*************************************************************************/

#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <stdarg.h>

#include "output.h"

//extern char DebugFile[] ;
//extern char OutputToFile ;

#define MAX_MESSAGES 7
/* Messages array */
char *sMessage[MAX_MESSAGES+1] = { 
   /* 0 MSG_DEFAULT */"Eh?",
   /* 1 MSG_NOSTACK */ "The TCP/IP protocol stack is not loaded",
   /* 2 MSG_STARTING */ "Socket Server Starting...",
   /* 3 MSG_STOPPED */ "Socket Server Stopped",  
   /* 4 MSG_READY */ "Socket Server Initialised and Ready",
   /* 5 MSG_SOCKFAIL */ "Socket Call Failed",
   /* 6 MSG_BINDFAIL */ "Bind Call Failed",
   /* 7 MSG_LSTNFAIL */ "Listen Call Failed"
};

void OutputMessage(int iMessageNumber)
{
   int i;

   if (iMessageNumber > MAX_MESSAGES || iMessageNumber < 1) {
      i = MSG_DEFAULT;
   } else {
      i = iMessageNumber;
   }

   printf(sMessage[i]);
   printf("\r\n");
} 

void LogMessage ( int Grade, char *Msg, ... )
{
   va_list FormatArgs ; 

   // is the Debug Grading high enough to trigger debug output ?    
   if ( Debug > 0 && Debug >= Grade ) {

      va_start ( FormatArgs, Msg ) ;
      // Output to file or screen
      if ( OutputToFile == 'Y' ) {
         vfprintf ( DebugFileHandle, Msg, FormatArgs ) ;
         fprintf ( DebugFileHandle, "\n" ) ;
         fflush ( DebugFileHandle ) ;
      } else {
         vprintf ( Msg, FormatArgs ) ; 
         printf ( "\n" ) ;
      }
      va_end ( FormatArgs ) ;
   }
}
