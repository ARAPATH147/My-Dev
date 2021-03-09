/*******************************************************************************
*
*  Module    : xshared.h
*  Desc.     : Header File for Common File Functions
*  Author    : Charles Skadorwa
*  Date      : 3rd March 2014
*
*-------------------------------------------------------------------------------
* Version A         Charles Skadorwa                               03-Mar-2014
* Original version.
*
*******************************************************************************/

#ifndef XSHARED_H

#define XSHARED_H 1

//Don't tell us about each step in the compile process
#pragma  On(Quiet);

#pragma Ipath("v:\FlexOS\HighC\inc;v:\FlexOS\usr\inc");

//Big memory model code generation
#pragma  Memory_model(BIG);
//#pragma  On(Optimize_for_space);

//Let the compiler use more efficient 386 instructions
#pragma  On(386);

//All controllers have an 8087 co-processor
#pragma  On(Floating_Point);

//Default to signed chars
//#pragma  Off(Char_default_unsigned);
#pragma  On(Char_is_rep);

//Enumarated types are always integers (2 bytes)
#pragma  On(Long_enums);

//Try and put as much static data in code segment where it will be shared
//between master and slaves.  Also, with these options on, if you use no static
//variables in a module the compiler is free to use DS as an extra segment
//register and isn't limited to just ES.  This produces 10% more efficient code.
#pragma  On(Read_only_strings);
#pragma  On(Literals_in_code);

//Constants only in code segment for non-debug versions.  Having constants
//in code segments appears to confuse the debugger for debug versions.
#ifndef DEBUG
#pragma  On(Const_in_code);
#endif

//Turn off optimizations for debug versions.
#ifdef DEBUG
#pragma  On(Codeview);
#pragma  Off(Optimize_xjmp);
#pragma  Off(Optimize_fp);
#pragma  Off(Optimize_xjmp_space);
#pragma  Off(Postpone_arg_pops);
#endif


//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  #include                                                            //
//                                                                      //
//////////////////////////////////////////////////////////////////////////

#define PORTLIB

#include <Portab.h>
#include <flexdef.h>
#include <flextab.h>
#include <flexif.h>
#include <stdio.h>
#include <string.h>

#define FALSE   0               /* Function FALSE value        */
#define TRUE    1               /* Function TRUE  value        */

#define AC_RETRY_DELAY  10
#define AC_MAX_RETRY    12

#define FULL_CLOSE  0
#define NULL        0          /* Null character value         */

//----------------------------------------------------------------------//
// BASSTR
// A BASIC string is a pointer to an area of heap that can be
// represented by the following C structure
//----------------------------------------------------------------------//
typedef struct BASSTR
{
    UWORD uwLen;
    BYTE bChar[0];
} *P_BASSTR;

//----------------------------------------------------------------------//
//  C32RETURN                                                           //
//  Assembler routine to return 32bit long result from C to BASIC       //
//----------------------------------------------------------------------//
extern LONG C32RETURN (LONG lResult);

//----------------------------------------------------------------------//
//  BASRETSTR                                                           //
//  Returns a BASIC string from C as a result of a function             //
//  NB The area of memory returned must not be on the local stack, nor  //
//  is it freed.                                                        //
//----------------------------------------------------------------------//
extern void BASRETSTR ( UWORD uwLen, void* pStr );


//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  Function Declarations                                               //
//                                                                      //
//////////////////////////////////////////////////////////////////////////
LONG OpenSequentialFile ( P_BASSTR pbsFileName );
LONG OpenFile ( BYTE *fname, UWORD flags );
VOID ReadSequentialFile ( LONG lFnum );
LONG WriteSequentialFile ( LONG lFnum, P_BASSTR pbFileRecord );
VOID CloseFile (LONG lFnum);


#pragma Data (Common);

//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  extern variables                                                    //
//                                                                      //
//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////

#pragma Data ();

#endif


