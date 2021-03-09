/******************************************************************************
portab.h	FlexOS Portability File

 Use of this file may make your code compatible with all C compilers listed.

*******************************************************************************
Copyright (c) 1985,1990  Digital Research Inc. 
All rights reserved. The Software Code contained in this listing is copyrighted
and may be used and copied only under terms of the Digital  Research  Inc.  End
User License Agreement.  This code may be used only by the registered user, and
may not be resold or transferred without the consent of Digital  Research  Inc.
Unauthorized reproduction,  transfer, or use of this material may be a criminal
offense under Federal and/or State law.
			U.S. GOVERNMENT RESTRICTED RIGHTS
This software product is provided with RESTRICTED RIGHTS.  Use, duplication  or
disclosure  by the Government  is subject  to restrictions  as set forth in FAR
52.227-19 (c) (2) (June, 1987) when applicable  or the applicable provisions of
the DOD FAR supplement  252.227-7013  subdivision  (b) (3) (ii) (May,  1981) or
subdivision  (c)  (1) (ii)  (May,  1987).   Contractor/manufacturer  is Digital
Research Inc. / 70 Garden Court / BOX DRI / Monterey, CA 93940.
*******************************************************************************
Date	Who	SPR#	Comments
-------	-------	----	-------------------------------------------------------
900215	rs		shel_rdef & shel_wdef were defined to be nothing under
			X/GEM. Removed this definition by taking | XGEM out of
			condition.
******************************************************************************/

/*****************************************************************************/
/* ENVIRONMENT                                                               */
/*****************************************************************************/
#ifndef __PORTAB__
#define __PORTAB__

#ifndef __portab_included                    /* for flexif.h ...             */
#define __portab_included
#endif

/*--------------------- Operating Systems -----------------------------------*/
#define GEMDOS     0                          /* Digital Research GEMDOS     */
#define MSDOS      0                          /* Microsoft MSDOS             */
#define OS2        0                          /* Microsoft OS/2              */
#define FLEXOS     1                          /* Digital Research FlexOS     */
/*------------------------ CPU Types ----------------------------------------*/
#define M68000     0                          /* Motorola Processing Unit    */
#define I8086      1                          /* Intel Processing Unit       */
#define I80186     0                          /* Intel Processing Unit       */
#define I80286     0                          /* Intel Processing Unit       */
#define I80386     0                          /* Intel Processing Unit       */
#define I80486     0                          /* Intel Processing Unit       */
/*------------------------ Memory Model -------------------------------------*/
#define FLAT_SPACE 0
/*------------------------ Compilers ----------------------------------------*/
#define DR_C       0                          /* Digital Research C Compiler */
#define LASER_C    0                          /* Laser C Compiler            */
#define LATTICE_C  0                          /* Lattice C Compiler          */
#define MW_C       0                          /* Mark Williams C Compiler    */
#define TURBO_C    0                          /* Turbo C Compiler            */
#define MS_C       0                          /* Microsoft C Compiler        */
#define HIGH_C     1                          /* Metaware High C Compiler    */
/*------------------------ GEM Versions -------------------------------------*/
#define GEM1       0x0001                     /* ATARI GEM version           */
#define GEM2       0x0002                     /* MSDOS GEM 2.x versions      */
#define GEM3       0x0004                     /* MSDOS GEM/3 version         */
#define XGEM       0x0100                     /* OS/2,FLEXOS X/GEM version   */
/*---------------------------------------------------------------------------*/
#ifndef GEM
#if GEMDOS
#define GEM        GEM1                       /* GEMDOS default is GEM1      */
#endif /* GEMDOS */

#if MSDOS
#define GEM        GEM3                       /* MSDOS default is GEM3       */
#endif /* MSDOS */

#if OS2
#define GEM        XGEM                       /* OS/2 default is X/GEM       */
#endif /* MSDOS */

#if FLEXOS
#define GEM        XGEM                       /* FLEXOS default is X/GEM     */
#endif /* FLEXOS */
#endif /* GEM */

/*****************************************************************************/
/* STANDARD TYPE DEFINITIONS                                                 */
/*****************************************************************************/

typedef char BYTE;                            /* Signed byte                 */
typedef unsigned char UBYTE;                  /* Unsigned byte               */

typedef short WORD;                           /* Signed word (16 bits)       */
typedef unsigned short UWORD;                 /* unsigned word               */

typedef long LONG;                            /* signed long (32 bits)       */
typedef unsigned long ULONG;                  /* Unsigned long               */

#define BOOLEAN WORD                          /* 2 valued (true/false)       */

typedef float FLOAT;                          /* single precision float      */
typedef double DOUBLE;                        /* double precision float      */

typedef int INT;                              /* a machine dependent int     */
typedef unsigned int UINT;                    /* a machine dependent uint    */

#define REG     register                      /* register variable           */
#define AUTO    auto                          /* Local to function           */
#define EXTERN  extern                        /* External variable           */
#define LOCAL   static                        /* Local to module             */
#define MLOCAL  LOCAL                         /* Local to module             */
#define GLOBAL                                /* Global variable             */

/*****************************************************************************/
/* COMPILER DEPENDENT DEFINITIONS                                            */
/*****************************************************************************/

#if GEMDOS                                    /* GEMDOS compilers            */
#if DR_C
#define void WORD                             /* DR_C doesn't know void      */
#endif /* DR_C */

#if LASER_C
#define graf_mbox graf_movebox                /* Wrong GEM binding           */
#define graf_rubbox graf_rubberbox            /* Wrong GEM binding           */
#endif /* LASER_C */

#if LATTICE_C
#define graf_mbox graf_movebox                /* Wrong GEM binding           */
#define graf_rubbox graf_rubberbox            /* Wrong GEM binding           */
#endif /* LATTICE_C */

#if TURBO_C
#define graf_mbox graf_movebox                /* Wrong GEM binding           */
#define graf_rubbox graf_rubberbox            /* Wrong GEM binding           */
#endif /* TURBO_C */

#if MW_C
#define VOID WORD                             /* MW_C doesn't know (void *)  */
#endif /* MW_C */

#if LATTICE_C
#define ADR(A) (LONG)A >> 16, (LONG)A & 0xFFFF
#else
#define ADR(A) (WORD)((LONG)A >> 16), (WORD)((LONG)A & 0xFFFF)
#endif /* LATTICE_C */
#endif /* GEMDOS */

/*
 * FAR and NEAR for FlexOS and X/GEM are declared in xgem.pro
 */
#if !FLEXOS
#if GEMDOS | FLAT_SPACE
#define FAR                                   /* Far Pointer                 */
#define NEAR                                  /* Near Pointer                */
#else
#if HIGH_C
#define FAR    _far
#define NEAR   _near
#else
#define FAR     far                           /* Far Pointer                 */
#define NEAR    near                          /* Near Pointer                */
#endif /* HIGH_C */
#endif /* GEMDOS */
#endif /* !FLEXOS */

#if !FLAT_SPACE & (MSDOS | OS2 | FLEXOS)      /* MSDOS or OS2 compilers      */
#define FPOFF(a)  (UWORD)(a)
#define FPSEG(a)  ((UWORD)((ULONG)(a) >> 16))
#define MKFP(a,b) ((VOID FAR *)(((ULONG)(a) << 16) | (UWORD)(b)))
#endif /* MSDOS | OS2 | FLEXOS */

#if FLEXOS & GEMAPP                           /* FLEXOS compilers            */
#define main GEMAIN                           /* because of X/GEM SRTL       */
#endif /* FLEXOS */

#if MS_C | TURBO_C | HIGH_C                   /* ANSI compilers              */
#define ANSI 1
#define _(params) params                      /* parameter checking          */
#else
#define ANSI 0
#define _(params) ()                          /* no parameter checking       */
#define const
#define volatile
#define size_t UINT
#endif

#if DR_C | LASER_C | LATTICE_C | MW_C | HIGH_C
#define cdecl
#define pascal
#endif

#define CONST    const
#define VOLATILE volatile
#define CDECL    cdecl

#if !HIGH_C
#define PASCAL   pascal
#endif

#define SIZE_T   size_t

#ifndef VOID
typedef void VOID;
#endif

/*****************************************************************************/
/* OPERATING SYSTEM DEPENDENT DEFINITIONS                                    */
/*****************************************************************************/

#if GEM & GEM1
#define appl_bvset(bvdisk, bvhard)
#define appl_yield() evnt_timer (0, 0)
#define menu_unregister(mid)
#define scrp_clear()
#define xgrf_stepcalc(orgw, orgh, xc, yc, w, h, pcx, pcy, pcnt, pxstep, pystep)
#define xgrf_2box(xc, yc, w, h, corners, cnt, xstep, ystep, doubled)
#endif /* GEM1 */

#if GEM & GEM1
#define shel_rdef(lpcmd, lpdir)
#define shel_wdef(lpcmd, lpdir)
#endif /* GEM1 | XGEM */

#if GEM & (GEM1 | GEM2)
#define menu_click(click, setit)
#endif /* GEM1 | GEM2 */

#if GEM & (GEM2 | GEM3 | XGEM)
#define fsel_exinput(pipath, pisel, pbutton, plabel)\
        fsel_input  (pipath, pisel, pbutton)
#define wind_new()
#endif /* GEM2 | GEM3 | XGEM */

/*****************************************************************************/
/* MISCELLANEOUS DEFINITIONS                                                 */
/*****************************************************************************/

#define FALSE   0                             /* Function FALSE value        */
#define TRUE    1                             /* Function TRUE  value        */
#define FAILURE (-1)                          /* Function failure return val */
#define SUCCESS 0                             /* Function success return val */
#define FOREVER for (;;)                      /* Infinite loop declaration   */
#define EOS     '\0'                          /* End of string value         */
#ifndef NULL
#define NULL    0                             /* Null character value        */
#endif
#define NULLPTR 0L                            /* Null pointer value          */

#ifndef EOF
#define EOF     (-1)                          /* EOF value                   */
#endif

#endif /* __PORTAB__ */
