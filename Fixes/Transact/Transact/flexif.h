/******************************************************************************
FLEXIF.H - User function number definitions and SVC calls prototyped. 
*******************************************************************************
Copyright (c) 1985,1990  Digital Research Inc. 
All rights reserved. The Software Code contained in this listing is copyrighted
and may be used and copied only under terms of the Digital  Research  Inc.  End
User License Agreement.  This code may be used only by the registered user, and
may not be resold or transfered  without the consent of Digital  Research  Inc.
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
-------	-------	----	------------------------------------------------------
22Mar90 MA		Added prototypes that are compatible with PORTLIB
			for both High C and Turbo C.
900212	RFW	----	Added s_gcopy() prototype to include an option 
			parameter also removed s_gsx() its not supported.
890915	ldt	----	Added IAPX286 define if no IAPX is defined.
890828  cpg             Added S_OVER?LAY() prototypes.  Made 'far' 286 specific.
890321	aeb		Added parentheses to ambiguous #define arguments.
890125	whf		Handles new typedefs
05Jan89 reb     	Created.
******************************************************************************/
#if ( !IAPX186 && !IAPX286 && !IAPX386 )	/* The default condition */
#define IAPX286     1
#endif

#define __flexif_included

#ifndef __portab_included
#include "portab.h"
#endif

#ifndef __flextab_included
#include "flextab.h"
#endif

/****************************************/
/*                                      */
/* FlexOS independent of processor call */
/*                                      */
/****************************************/

#ifndef PORTLIB
EXTERN	RFAR LONG _osif();
#else
EXTERN	LONG __osif(UWORD func, LONG pblk);
#endif

/**********************************/
/*                                */
/* Meta-Ware specific definitions */
/*                                */
/**********************************/
#if __HIGHC__
#define blkfill(where,what,how_many) (_fill_char(where,how_many,what))
#define ZERO_PARMS (blkfill(&pblock,NULL,sizeof(pblock)))
#define SWICAST	(far LONG(*)())

#ifndef far

#if (IAPX186|IAPX286)		/* For segmented applications... */
#define far _far
#else
#define far
#endif

#endif	/* far */

#endif	/* __HIGHC__ */

/***************************/
/*                         */
/*  User Function Numbers  */
/*                         */
/***************************/
#define	F_GET		0
#define	F_SET		1
#define	F_LOOKUP	2
#define	F_CREATE	3
#define	F_DELETE	4
#define	F_OPEN		5
#define	F_CLOSE 	6
#define	F_READ		7
#define	F_WRITE 	8
#define	F_SPECIAL	9
#define	F_RENAME	10
#define	F_DEFINE	11
#define	F_DEVLOCK	12
#define	F_INSTALL	13
#define	F_LOCK		14
#define	F_COPY		15
#define	F_ALTER 	16
#define	F_XLAT		17
#define	F_RWAIT 	18
#define	F_KCTRL 	19
#define	F_ORDER 	20
#define	F_KEYPUT	21
#define	F_GIVE		22
#define	F_BWAIT 	23
#define	F_TIMER 	24
#define	F_EXIT		25
#define	F_ABORT 	26
#define	F_CANCEL	27
#define	F_WAIT		28
#define	F_STATUS	29
#define	F_RETURN	30
#define	F_EXCEPTION	31
#define	F_ENABLE	32
#define	F_DISABLE	33
#define	F_SWIRET	34
#define	F_MALLOC	35
#define	F_MFREE 	36
#define	F_OVERLAY	37
#define	F_COMMAND	38
#define	F_CONTROL	39
/* Reserved  		40 */
#define	F_SEEK		41


/****************************************/
/*                                      */
/*  FlexOS Supervisor Calls prototyped  */
/*                                      */
/****************************************/

#ifndef PORTLIB

/* FLEXLIB has only BIG model entry points.  Thus, the following
 * prototypes require the use of FAR pointers if other memory
 * models are used.
 */
RFAR LONG e_bwait(far LONG (*swi)(), UWORD clicks, LONG fnum, LONG mask,
	LONG state);
RFAR LONG e_command(far LONG (*swi)(), far LONG *pid, UWORD flags, far BYTE *command,
	far BYTE *buffer, LONG bufsiz, far PINFO *procinfo);
RFAR LONG e_control(far LONG (*swi)(), BYTE flags,UWORD option, LONG pid, 
	far BYTE *buffer, LONG bufsiz, far BYTE *target, far LONG *tpid);
RFAR LONG e_lock(far LONG (*swi)(), UWORD flags, LONG fnum, LONG offset,
	LONG nbytes);
RFAR LONG e_read(far LONG (*swi)(), UWORD flags, LONG fnum, far BYTE *buffer,
	LONG bufsiz, LONG offset);
RFAR LONG e_rwait(far LONG (*swi)(), UWORD flags, LONG fnum, far RECT *region);
RFAR LONG e_special(far LONG (*swi)(), BYTE func, UWORD flags, LONG fnum,
	far BYTE *databuf, LONG dbufsiz, far BYTE *parmbuf, LONG pbufsiz);
RFAR LONG e_termevent(far LONG (*swi)(), LONG pid);
RFAR LONG e_timer(far LONG (*swi)(), UWORD flags, LONG time);
RFAR LONG e_write(far LONG (*swi)(), UWORD flags, LONG fnum, far BYTE *buffer,
	LONG bufsiz, LONG offset);
RFAR LONG s_abort(LONG pid);
RFAR LONG s_alter(UWORD flags, LONG fnum, far FRAME *dframe, far RECT *drect,
	far BYTE *alterb);
RFAR LONG s_bwait(UWORD clicks, LONG fnum, LONG mask, LONG state);
RFAR LONG s_cancel(LONG events);
RFAR LONG s_close(UWORD flags, LONG fnum);
RFAR LONG s_command(UWORD flags, far BYTE *name, far BYTE *cmdline, LONG cmdlen,
	far PINFO *procinfo);
RFAR LONG s_control(BYTE flags, UWORD option, LONG pid, far BYTE *buffer, LONG bufsiz,
	far BYTE *target, far LONG *tpid);
RFAR LONG s_copy(UWORD flags, LONG fnum, far FRAME *dframe, far RECT *drect,
	far FRAME *sframe, far RECT *srect);
	/* Please note that the prototype for s_gcopy() uses UWORD for	*/
	/* the dframe, drect, sframe, and srect intentionally because of*/
	/* the differing subfunctions supported. RFW			*/
RFAR LONG s_gcopy(UBYTE option, UWORD flags, LONG fnum, far UWORD *dframe, 
	far UWORD *drect, far UWORD *sframe, far UWORD *srect);
RFAR LONG s_create(BYTE option, UWORD flags, far BYTE *name, UWORD rec_size,
	UWORD security, LONG size);
RFAR LONG s_define(UWORD flags, far BYTE *lname, far BYTE *prefix, LONG psize);
RFAR LONG s_delete(UWORD flags, far BYTE *name);
RFAR LONG s_devlock(UWORD option, LONG fnum);
RFAR LONG s_disable(void);
RFAR LONG s_enable(void);
RFAR LONG s_exception(WORD number, far LONG (*isr)());
RFAR LONG s_exit(LONG flag);
RFAR LONG s_get(BYTE table, LONG id, far VOID *buffer, LONG bufsiz);
RFAR LONG s_give(LONG fnum);
RFAR LONG s_install(UBYTE option, UWORD flags, far BYTE *devname, far BYTE *parm);
RFAR LONG s_kctrl(LONG fnum, BYTE nranges, UWORD begend,...);
RFAR LONG s_lock(UWORD flags, LONG fnum, LONG offset, LONG nbytes);
RFAR LONG s_lookup(BYTE table, UWORD flags, far BYTE *name, far BYTE *buffer,
	LONG bufsiz, LONG itemsiz, LONG key);
RFAR LONG s_malloc(BYTE option, far MPB *mpbptr);
RFAR LONG s_mfree(far BYTE *address);
RFAR LONG s_mctrl(LONG fnum, far RECT *region);
RFAR LONG s_open(UWORD flags, far BYTE *name);
RFAR LONG s_order(UWORD order, LONG fnum);

RFAR LONG s_overlay(LONG fnum, LONG DFAR *codeadr, LONG DFAR *dataadr, LONG offset);
RFAR LONG s_over1lay( LONG, LONG DFAR *, LONG DFAR *, LONG );
RFAR LONG s_over2lay( LONG, LD_STRUCT2 DFAR *, LONG );
RFAR LONG s_over3lay( LONG, LD_STRUCT3 DFAR *, LONG );

RFAR LONG s_rdelim(UWORD flags, LONG fnum, far BYTE *buffer, LONG bufsiz,
	LONG offset, far UWORD *delims);
RFAR LONG s_read(UWORD flags, LONG fnum, far BYTE *buffer, LONG bufsiz,
	LONG offset);
RFAR LONG s_rename(UWORD flags, far BYTE *name, far BYTE *newname);
RFAR LONG s_return(LONG emask);
RFAR LONG s_rwait(UWORD flags, LONG fnum, far RECT *region);
RFAR LONG s_seek(UWORD flags, LONG fnum, LONG offset);
RFAR LONG s_set(BYTE table, LONG id, far VOID *buffer, LONG bufsiz);
RFAR LONG s_special(BYTE func, UWORD flags, LONG fnum, far BYTE *databuf,
	LONG dbufsiz, far BYTE *parmbuf, LONG pbufsiz);
RFAR LONG s_status(void);
RFAR LONG s_swiret(LONG option);
RFAR LONG s_timer(UWORD flags, LONG time);
RFAR LONG s_vccreate(UWORD flags, LONG pfnum, WORD rows, WORD columns,
	BYTE top, BYTE bottom, BYTE left, BYTE right);
RFAR LONG s_wait(LONG events);
RFAR LONG s_write(UWORD flags, LONG fnum, far BYTE *buffer, LONG bufsiz,
	LONG offset);
RFAR LONG s_xlat(UWORD flags, far BYTE *buffer, LONG bufsiz);
RFAR LONG s_getfield(BYTE table, UWORD flags, LONG id, far VOID *buffer,
	LONG bufsiz);
RFAR LONG s_setfield(BYTE table, UWORD flags, LONG id, far VOID *buffer,
	LONG bufsiz);
RFAR LONG s_givefield(UWORD flags, LONG id);

#else

/* Prototypes for PORTLIB's SVC entry points do not require FAR pointers.
 * Instead, PORTLIB has separate entry points for each memory model.
 */
LONG e_bwait(LONG (*swi)(), UWORD clicks, LONG fnum, LONG mask,
	LONG state);
LONG e_command(LONG (*swi)(), LONG *pid, UWORD flags, BYTE *command,
	BYTE *buffer, LONG bufsiz, PINFO *procinfo);
LONG e_control(LONG (*swi)(), BYTE flags,UWORD option, LONG pid, 
	BYTE *buffer, LONG bufsiz, BYTE *target, LONG *tpid);
LONG e_lock(LONG (*swi)(), UWORD flags, LONG fnum, LONG offset,
	LONG nbytes);
LONG e_read(LONG (*swi)(), UWORD flags, LONG fnum, BYTE *buffer,
	LONG bufsiz, LONG offset);
LONG e_rwait(LONG (*swi)(), UWORD flags, LONG fnum, RECT *region);
LONG e_special(LONG (*swi)(), BYTE func, UWORD flags, LONG fnum,
	BYTE *databuf, LONG dbufsiz, BYTE *parmbuf, LONG pbufsiz);
LONG e_termevent(LONG (*swi)(), LONG pid);
LONG e_timer(LONG (*swi)(), UWORD flags, LONG time);
LONG e_write(LONG (*swi)(), UWORD flags, LONG fnum, BYTE *buffer,
	LONG bufsiz, LONG offset);
LONG s_abort(LONG pid);
LONG s_alter(UWORD flags, LONG fnum, FRAME *dframe, RECT *drect,
	BYTE *alterb);
LONG s_bwait(UWORD clicks, LONG fnum, LONG mask, LONG state);
LONG s_cancel(LONG events);
LONG s_close(UWORD flags, LONG fnum);
LONG s_command(UWORD flags, BYTE *name, BYTE *cmdline, LONG cmdlen,
	PINFO *procinfo);
LONG s_control(BYTE flags, UWORD option, LONG pid, BYTE *buffer, LONG bufsiz,
	BYTE *target, LONG *tpid);
LONG s_copy(UWORD flags, LONG fnum, FRAME *dframe, RECT *drect,
	FRAME *sframe, RECT *srect);
	/* Please note that the prototype for s_gcopy() uses UWORD for	*/
	/* the dframe, drect, sframe, and srect intentionally because of*/
	/* the differing subfunctions supported. RFW			*/
LONG s_gcopy(UBYTE option, UWORD flags, LONG fnum, UWORD *dframe, 
	UWORD *drect, UWORD *sframe, UWORD *srect);
LONG s_create(BYTE option, UWORD flags, BYTE *name, UWORD rec_size,
	UWORD security, LONG size);
LONG s_define(UWORD flags, BYTE *lname, BYTE *prefix, LONG psize);
LONG s_delete(UWORD flags, BYTE *name);
LONG s_devlock(UWORD option, LONG fnum);
LONG s_disable(void);
LONG s_enable(void);
LONG s_exception(WORD number, LONG (*isr)());
LONG s_exit(LONG flag);
LONG s_get(BYTE table, LONG id, VOID *buffer, LONG bufsiz);
LONG s_give(LONG fnum);
LONG s_install(UBYTE option, UWORD flags, BYTE *devname, BYTE *parm);
LONG s_kctrl(LONG fnum, BYTE nranges, UWORD begend,...);
LONG s_lock(UWORD flags, LONG fnum, LONG offset, LONG nbytes);
LONG s_lookup(BYTE table, UWORD flags, BYTE *name, BYTE *buffer,
	LONG bufsiz, LONG itemsiz, LONG key);
LONG s_malloc(BYTE option, MPB *mpbptr);
LONG s_mfree(BYTE *address);
LONG s_mctrl(LONG fnum, RECT *region);
LONG s_open(UWORD flags, BYTE *name);
LONG s_order(UWORD order, LONG fnum);

LONG s_overlay(LONG fnum, LONG *codeadr, LONG *dataadr, LONG offset);
LONG s_over1lay( LONG, LONG *, LONG *, LONG );
LONG s_over2lay( LONG, LD_STRUCT2 *, LONG );
LONG s_over3lay( LONG, LD_STRUCT3 *, LONG );

LONG s_rdelim(UWORD flags, LONG fnum, BYTE *buffer, LONG bufsiz,
	LONG offset, UWORD *delims);
LONG s_read(UWORD flags, LONG fnum, BYTE *buffer, LONG bufsiz,
	LONG offset);
LONG s_rename(UWORD flags, BYTE *name, BYTE *newname);
LONG s_return(LONG emask);
LONG s_rwait(UWORD flags, LONG fnum, RECT *region);
LONG s_seek(UWORD flags, LONG fnum, LONG offset);
LONG s_set(BYTE table, LONG id, VOID *buffer, LONG bufsiz);
LONG s_special(BYTE func, UWORD flags, LONG fnum, BYTE *databuf,
	LONG dbufsiz, BYTE *parmbuf, LONG pbufsiz);
LONG s_status(void);
LONG s_swiret(LONG option);
LONG s_timer(UWORD flags, LONG time);
LONG s_vccreate(UWORD flags, LONG pfnum, WORD rows, WORD columns,
	BYTE top, BYTE bottom, BYTE left, BYTE right);
LONG s_wait(LONG events);
LONG s_write(UWORD flags, LONG fnum, BYTE *buffer, LONG bufsiz,
	LONG offset);
LONG s_xlat(UWORD flags, BYTE *buffer, LONG bufsiz);
LONG s_getfield(BYTE table, UWORD flags, LONG id, VOID *buffer,
	LONG bufsiz);
LONG s_setfield(BYTE table, UWORD flags, LONG id, VOID *buffer,
	LONG bufsiz);
LONG s_givefield(UWORD flags, LONG id);

#endif	/* PORTLIB */

/* ------------------------- End of FLEXIF.H ---------------- */
