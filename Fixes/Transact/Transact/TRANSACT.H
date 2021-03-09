
#ifndef TRANSACT_H
#define TRANSACT_H 1
/*************************************************************************
**                                                                      **
**  Application Name  : Transact.286                                    **
**  Module Name       : transact.h                                      **
**                                                                      **
**  C header file for transact socket server                            **
**                                                                      **
**  A   Stuart Highley                                     19 May 2006  **
**      Initial Version.                                                **
**                                                                      **
*************************************************************************/

//Don't tell us about each step in the compile process
#pragma  On(Quiet);

#pragma Ipath("v:\FlexOS\HighC\inc;v:\FlexOS\usr\inc");

//Big memory model code generation
#pragma  Memory_model(BIG);
//#pragma  On(Optimize_for_space);

//Let the compiler use more efficient 386 instructions                  // Streamline SDH 24-Sep-2008
#pragma  On(386);                                                       // Streamline SDH 24-Sep-2008

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

#include "rfs.h"
#include "rfglobal.h"

//////////////////////////////////////////////////////////////////////////
//                                                                      //
//  #define                                                             //
//                                                                      //
//////////////////////////////////////////////////////////////////////////

/*
//This macro creates a BASIC like string as static data.  A BASIC string is
//preceeded with the string length in bytes.
//  e.g. DEF_BASSTR (MY_BASSTR, "Hello World!");
#define DEF_BASSTR(x,y) static const struct {UWORD i; BYTE c[sizeof(y)];}x={sizeof(y)-1,y}

#define VAR_BASSTR(x,y) static struct {UWORD i; BYTE c[sizeof(y)];}x={sizeof(y)-1,y}

typedef struct BASARRAY {
    void* pElmts;
    UBYTE ubType;
    UBYTE ubDimensions;
    ULONG ulElmtSize;
    UWORD uwMaxDim1;
    UWORD uwElmtSizeDim1;
} BASARRAY, *P_BASARRAY;

typedef struct BASSTR {
    UWORD uwLen;
    BYTE bChar[0];
} BASSTR, *P_BASSTR;


//Macros to get at basic array elements
#define ARRAY_STR(x) ((P_BASSTR*)(x->pElmts))
#define ARRAY_INT1(x) ((BYTE*)(x->pElmts))
#define ARRAY_INT2(x) ((WORD*)(x->pElmts))
#define ARRAY_INT4(x) ((LONG*)(x->pElmts))
#define ARRAY_UINT1(x) ((UBYTE*)(x->pElmts))
#define ARRAY_UINT2(x) ((UWORD*)(x->pElmts))
#define ARRAY_UINT4(x) ((ULONG*)(x->pElmts))
*/

//Macro to stop MWC complaining about unused parameter x
#define UNUSED(x) if(x);

//A quick and short way to test a far pointer agianst NULL in 16 bit code:
//Just check the selector in the high word is zero.
#define ISNULL(x) ((unsigned short)((unsigned long)(x)>>16)==0)
#define NOTNULL(x) (!ISNULL(x))

//Macro to get number of elements in an array
#define ELEMENTS(x) (sizeof(x)/sizeof(*x))

//Macro to get size of an element within a structure
#define SSIZEOF(s,e) (sizeof(((s*)NULL)->e))

////////

//extern UBYTE bg;    // background appl flag for use with disp_msg()

//UWORD activity;

//int process( char *inbound, int *nbytes );

#endif /* TRANSACT_H not defined */

