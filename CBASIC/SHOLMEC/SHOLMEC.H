//*************************************************************************
//
// File        : SHOLMEC.h
// Author      : Ranjith Gopalankutty
// Created     : 14/04/2018
//
// Description : C header file for SHOLMEC client application
//               This configuration is based on transact.h
//               
//
 
#ifndef SHOLMEC_H

    #define SHOLMEC_H


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


    #define PORT_NUMBER                2002
    #define Socket_Message_Length      57L
    #define Port_Read_Message_Length   57L
    #define SHOLMEC_PIPE               "pi:drwatson"
    #define SHOLMEC_CFLAGS             0X3519
    #define SHOLMEC_Record_length      57L	
	 
    #define lPFileName_REP             800
    #define lPFileName_RECL            4
    #define lPFileName_OFLAGS          0x2014
    #define lPFileName_CFLAGS          0x2014
	
 



  
#endif /* #ifndef SHOLMEC_H */

