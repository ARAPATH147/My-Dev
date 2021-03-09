/******************************************************************************
FLEXDEF.H - SVC flags, error codes, modes, and system definitions for FlexOS  *
*******************************************************************************
Copyright (c) 1985,1991  Digital Research Inc. 
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
11Oct91	ldt		Added A_LOGMNT flag to compliment the A_PHYSMNT flag
			and added an explanation about the use of them and how
			they need not be used.
04Oct91	rfb	5166	Moved A_PHYSMNT flag definition to this file from
			trnfrsys.c.
04Sep91 ldt		Fixed O_RETURN and O_APID to be LONG values.
03Jun91 NZ	4734	Add new error codes for 0x4196 and 0x4197.
05Dec90	glp		Added A_NONBLOCK.
15Oct90 ldt		E_INSWI was incorrectly defined.
10Aug90 ldt		Fixed O_RETURN value (was 1, now 0)
29Mar90 MA		Added E_SIGNAL for POSIX library.
03Mar90 ldt		Added definitions to coordinate with PGR guide.
21Sep89 ldt	----	Parts of the orginal Flextab.h
******************************************************************************/
#define __flexdef_included

	/* Define standard I/O values */

#define	STDIN		(LONG)0
#define	STDOUT		(LONG)1
#define	STDERR		(LONG)2


	/* MODE Field */

#define M_ASYNC		0x01	/* Asynchronous function		     */
#define	M_UADDR		0x02	/* Addresses are in user space		     */
#define M_COMMAND	0x04	/* Originated by COMMAND RM		     */
#define M_FE		0x08	/* Originated by Front End		     */


	/* Function Field (Special) */

#define A_DATABUF	0x40	/* Write (send buffer) (0=Read(receive buffer)) */ 
#define A_PARMBUF	0x80    /* Write (send buffer) (0=Read(receive buffer)) */


	/* OPTION Fields */

/* CREATE */
#define	O_FILE		0x00	/* CREATE - Disk or Message Pipe	     */
#define O_DIR		0x01	/* CREATE - Directory			     */
#define O_VCON		0x02	/* CREATE - Virtual Console		     */

/* CONTROL */
#define O_LOAD		0x01	/* Load program for control	*/
#define O_REMOVE	0x02	/* Remove program */
#define O_READCODE	0x03	/* Read target code memory	*/
#define O_READATA	0x04	/* Read target data memory	*/
#define O_WRITECODE	0x05	/* Write target code memory */
#define O_WRITEDATA	0x06	/* Write target data memory	*/
#define O_READREG	0x07	/* Read target registers	*/
#define O_WRITEREG	0x08	/* Write target registers	*/
#define O_START		0x09	/* Start executing	*/
#define O_TRACE		0x0A	/* Trace a single instruction	*/
#define O_HALT		0x0B	/* Force a halt	*/
#define O_ALLON		0x0C	/* All exception traps on	*/
#define O_ALLOFF	0x0D	/* All exception traps off	*/
#define O_TRAPON	0x0E	/* Select exception trap on	*/
#define O_TRAPOFF	0x0F	/* Select exception trap off	*/
#define O_CHKBUF	0x10	/* Range check target buffer	*/

/* SWIRET */
#define O_RETURN	(LONG)0   /* Return to main program */
#define O_APID		(LONG)1    /* Assume pid from main program */

/* INSTALL */
#define O_REMUNIT	0x00	/* Remove previously installed driver unit */
#define O_LOADDEV	0x01	/* Load device driver from disk */
#define O_ADDUNIT	0x02	/* Add unit to existing device driver */
#define O_LINKUNIT	0x03	/* Link a subdriver to a device driver */

/* MALLOC */
#define O_GROWHEAP	0x00	/* Expand an existing heap */
#define O_NEWHEAP	0x01	/* Allocate a new heap */ 


	/* FLAGS Field (attributes) */

#define A_OPENMSK (A_SET+A_EXEC+A_WRITE+A_READ+A_SHARE+A_SHRO+A_SHFP+A_REDUCE)

		/* bit 0 */
#define A_CHARPL	0x0001	/* Character Plane - ALTER,COPY		     */
#define A_PCLOSE	0x0001	/* Partial Close - CLOSE		     */
#define A_BSCREEN	0x0001	/* Bit Map Screen - VCCREATE		     */
#define A_SYSTEM	0x0001	/* Set System Level (0=Process) - DEFINE     */
#define A_MCTRL		0x0001	/* Mouse Control (0=KB control) - KCTRL      */
#define A_FLUSH		0x0001	/* Flush before or after I/O - READ,WRITE    */
#define A_ABSOLUTE	0x0001	/* Absolute (0=Relative) - TIMER	     */
#define A_REPLACE	0x0001	/* Replace (0=Add) - XLAT		     */
#define A_EXIT		0x0001	/* Wait for exit (0=entry) - RWAIT	     */
#define A_SET		0x0001	/* Set - CREATE,INSTALL,OPEN		     */
#define A_MOUSE		0x0001	/* Mouse control - KCTRL		     */
#define A_HIDDEN	0x0001	/* Lookup Hidden Files			     */
#define A_REG		0x0001  /* 386 register set - CONTROL		     */

		/* bit 1 */

#define A_ATTRPL	0x0002	/* Attribute Plane - ALTER,COPY		     */
#define A_BADTRK	0x0002  /* Mark track as bad - SPECIAL		     */
#define A_EXEC		0x0002	/* Execute Privs - OPEN, CREATE		     */
#define A_BBORDER	0x0002	/* Bit map Borders - VCCREATE		     */
#define A_CLIP		0x0002	/* Clip to current window - RWAIT	     */
#define A_RETURN	0x0002	/* Return (0=Set) - DEFINE		     */
#define A_DELIM		0x0002	/* Read until Delimiter - READ		     */
#define A_TRUNCATE	0x0002	/* Truncate File - WRITE		     */
#define A_SYSFILE	0x0002	/* Lookup System Files			     */
				/* LOCK - See A_LCKMSK			     */

		/* bit 0 and 1 */

#define	A_UNLOCK	0x0000	/* Unlock - LOCK			     */
#define A_EXLOCK	0x0001	/* Exlusive Lock - LOCK			     */
#define	A_EWLOCK	0x0002	/* Exclusive Write Lock	- LOCK		     */
#define	A_SWLOCK	0x0003	/* Shared Write Lock -LOCK		     */
#define A_LCKMSK	0x0003	/* Lock Mask - LOCK			     */

#define A_PROLOCK	0x0000  /* Lock for process - DEVLOCK		     */
#define A_FAMLOCK	0x0001  /* Lock for family - DEVLOCK		     */
#define A_UNLKDEV	0x0002  /* Unlock device - DEVLOCK		     */

		/* bit 2 */
#define A_EXTPL		0x0004	/* Extension Plane - ALTER,COPY		     */
#define A_WRITE		0x0004	/* Write - CREATE,INSTALL,OPEN		     */
#define A_SIZE		0x0004  /* Size Spec (0=same as parent) - VCCREATE   */
#define A_NODESCT	0x0004	/* Non-Destructive - READ		     */
#define A_VOLUME	0x0004	/* Lookup Volume Label			     */
#define A_GEMRECT	0x0004	/* Use GEM RECT instead of Flex RECT - RWAIT */
#define A_READPHYS_V	0x0004	/* Verify media (0=read media) SPECIAL	     */
#define A_USECSHN	0x0004  /* Use C,H, S, and N fields - SPECIAL	     */

		/* bit 3 */
#define A_READ		0x0008	/* Read - CREATE,INSTALL,OPEN		     */
#define A_DELSC		0x0008	/* Remove Parent Screen - VCCREATE	     */
#define A_PREINIT	0x0008	/* Preinitialized - READ		     */
#define A_INCLDIR	0x0008	/* Lookup DIR files			     */
#define A_USEHEAD	0x0008  /* Use Head, Cylinder, etc. fields - SPECIAL */

		/* bit 4 */
#define A_NOPROC	0x0010	/* No process - COMMAND			     */
#define A_SHARE		0x0010	/* Shared - CREATE,INSTALL,OPEN		     */
#define A_DELINCL	0x0010	/* Include Delimiter - READ		     */
#define A_EXCLNORM	0x0010	/* Don't Lookup Normal files		     */
#define A_CONFLICT	0x0010  /* Return error on conflict - DEVLOCK,LOCK   */

		/* bit 5 */
#define A_CHAIN		0x0020	/* Chain (0=procedure) - COMMAND	     */
#define A_SHRO		0x0020	/* Allow R/O shared (0=Allow R/W) - CREATE,  */
				/*	OPEN				     */
#define A_REMOVE	0x0020	/* Removeable Driver - INSTALL		     */
#define A_EDIT		0x0020	/* Edited - READ			     */

		/* bit 6 */
#define A_SHFP		0x0040	/* Share FP with Family (0=unique) - OPEN,   */
				/*	CREATE				     */
#define A_DEVLOCK	0x0040	/* Device Locks Allowed - INSTALL	     */
#define	A_NONBLOCK	0x0040	/* Non-Blocked I/O. - READ,WRITE (PIPE RM only) */

		/* bit 7 */
#define A_NOEXCL	0x0080	/* No Exclusive Access - INSTALL	     */
#define A_ZFILL		0x0080	/* Zero Fill Space - CREATE, WRITE	     */
#define A_REDUCE	0x0080	/* Allow Reduced Access - OPEN		     */
#define	A_NEWFMLY	0x0080	/* Create a new family - COMMAND	     */

		/* bit 8 */
#define A_PART		0x0100	/* Partitions enabled - INSTALL		     */
#define A_TEMP		0x0100	/* Delete on Last Close - CREATE, VCCREATE   */
				/* READ, WRITE, SEEK, LOCK - see A_OFFMSK    */
#define A_SUBPROC	0x0100	/* Spawn a subprocess - COMMAND		     */

		/* bit 9 */
#define A_VERIFY	0x0200	/* Verify Writes on this media - INSTALL     */
#define	A_CONTIG	0x0200	/* Contiguous File - CREATE		     */
				/* READ, WRITE, SEEK, LOCK - see A_OFFMSK    */
#define A_FORK		0x0200	/* Spawn a FORK process - COMMAND	     */

/*****************************************************************************
*									     *
*  04Oct91 SPR 4166 -- Moved A_PHYSMNT definition to here from trnfrsys.c    *
*  The A_PHSYMNT and A_LOGMNT flags for the open svc are included here for   *
*  for compatibility with pre-FlexOS 2.0 applications.  These flags force    *
*  the disk resource manager to remount devices considered removable media.  *
*  If another process has the removable device open at the time one of these *
*  flags are used, then the disk resource manager ignores the flag and no    *
*  remount occurs. 							     *
*									     *
*  These flags should not be used for any new application development as the *
*  disk resource manager handles remounts on devices that support open door  *
*  detection.								     *
*									     *
******************************************************************************/
#define A_LOGMNT	0x0100	/* Force logical remount on device - OPEN    */
#define A_PHYSMNT	0x0200	/* Force physical remount on device - OPEN   */

		/* bit 8 and 9 */
#define A_OFFMSK	0x0300	/* Offset Mask - READ, WRITE, SEEK, LOCK     */
#define A_BOFOFF	0x0000	/* Relative to Beginning of File	     */
#define A_FPOFF		0x0100	/* Relative to File Pointer		     */
#define A_EOFOFF	0x0200	/* Relative to End of File		     */

		/* bit 10 */
#define	A_DELETE	0x0400	/* Delete File if exists - CREATE	     */
#define A_SUBFUNC	0x0400  /* Use primary subfunction number, 0 use     */
				/* secondary subfunction number - SPECIAL    */

		/* bit 11 */
#define A_OEM		0x0800	/* OEM specific bit everywhere - for internal*/
				/*	use only, must be 0 from user entry  */
#define A_NOCACHE	0x0800	/* Force write to disk (Flexos 2.2 or + )    */
				/* CREATE, WRITE			     */

		/* bit 12 */
#define A_SECURITY	0x1000	/* Use specified Security Word - CREATE	     */

		/* bit 13 */
#define A_FORCE		0x2000	/* Force Case - COMMAND, CREATE, DELETE	     */
				/*	INSTALL, LOOKUP, OPEN, RENAME	     */

		/* bit 14 */
#define A_LIT		0x4000	/* Literal Name - COMMAND,CREATE,DELETE,     */
				/*	INSTALL,LOOKUP,OPEN,RENAME,	     */
				/*	Reserved for DEFINE		     */

		/* bit 15 */
#define A_UADDR		0x8000	/* Use to tell drivers that buffer is a	     */
				/*	user address			     */


	/* Note:  Tables from 0x82 -> 0xff are special tables */

/****************************************************************/
/*								*/
/*  Error Definitions						*/
/*								*/
/*	All error codes are negative numbers (LONGS).          	*/
/*	Error Code Format :					*/
/*								*/
/*      31|30   24|23    16|15             0 (bits)		*/
/*	+-+-------+--------+----------------+			*/
/*	|1|   0   | module |   Error Code   |			*/
/*	+-+-------+--------+----------------+			*/
/*	(high byte)		(low word)			*/
/*								*/
/*	Error Code	16 bit number as described below	*/
/*			(NOTE: these are all cast as WORD)	*/
/*	Module		8 bit number indicating source Module	*/
/*	High Byte	high bit is 1, low 7 bits are reserved	*/
/*								*/
/*			00H - Kernel & Supervisor		*/
/*			10H - Pipe System			*/
/*			20H - Disk System			*/
/*			21H-2FH - Disk drivers			*/
/*			30H - Console System			*/
/*			31H-3FH - Console drivers		*/
/*			40H - Command				*/
/*			50H - Extension				*/
/*			51H-5FH - OEM Ext. Drivers		*/
/*			60H - Network System			*/
/*			61H-6FH - Network Drivers		*/
/*			70H - Misc. Resource Manager		*/
/*			71H-7FH - Misc. drivers			*/
/*			81H - Port Driver			*/
/*			Special driver ID's begin at 82H.	*/
/*			Driver ID's cannot have a zero in the	*/
/*			low nibble of ID byte.			*/
/*								*/
/*	Resource Managers return errors that they generated	*/
/*	by ORing the RM number with the Error Code.		*/
/*	ie.							*/
/*		return( EM_CON | E_EXISTS );			*/ 
/*	  (The Console RM is noting that a file already exists)	*/
/*								*/
/*	ALSO:  DON'T OR Module Number with E_SUCCESS...		*/
/*								*/
/****************************************************************/

/****************************************************************/
/*								*/	
/*	Module numbers, used to identify the source of the	*/
/*	error.							*/
/*								*/
/****************************************************************/

#define EM_KERN	0x80000000L	/* Kernel			*/
#define	EM_SUP	0x80000000L	/* Internal Supervisor Module	*/
#define EM_EMU	0x80020000L	/* Emulation module(DAE,CPM,...)*/
#define EM_PIPE	0x80100000L	/* Pipe Resource Manager	*/
#define EM_DISK	0x80200000L	/* Disk Resource Manager	*/
#define EM_CON	0x80300000L	/* Console Resource Manager	*/
#define EM_LOAD	0x80400000L	/* Command (Load) Resource Mgr.	*/
#define EM_EXT	0x80500000L	/* Extension Resource Manager	*/
#define EM_NET	0x80600000L	/* Networ Resource Manager	*/
#define	EM_MSC	0x80700000L	/* Misc. Driver Resoure Mgr.	*/

#define	ED_TIME	0x80010000L	/* Timer driver			*/
#define	ED_PIPE	0x80110000L	/* Pipe driver			*/
#define	ED_DISK	0x80210000L	/* Disk driver			*/
#define	ED_CON	0x80310000L	/* Console driver		*/
#define	ED_PRN	0x80710000L	/* Printer driver		*/
#define	ED_SER	0x80720000L	/* Serial driver		*/
#define	ED_CLOCK 0x807e0000L	/* DOS clock driver emulator	*/
#define	ED_NULL	0x807f0000L	/* Null device driver		*/
#define	ED_PORT	0x80810000L	/* Port driver			*/

/****************************************************************/
/*								*/
/*  Common Error Definitions - Range 0x4000 - 0x407F		*/
/*								*/
/*  These error codes are shared by more than one system module.*/
/*								*/
/****************************************************************/

#define	E_SUCCESS	0x0L	/* No Error				*/

#define	E_ACCESS	0x4001	/* Cannot access file due to ownership.	*/
#define	E_CANCEL	0x4002	/* Event Cancelled			*/
#define	E_EOF		0x4003	/* End of File				*/
#define	E_EXISTS	0x4004	/* File already exists			*/
				/* INSTALL - Device already exists	*/
#define	E_DEVICE	0x4005	/* Device does not match.		*/
				/* RENAME on different devices.		*/
#define	E_DEVLOCK	0x4006	/* Device is LOCKED			*/
#define	E_FILENUM	0x4007	/* Bad File Number			*/
#define	E_FUNCNUM	0x4008	/* Bad function number			*/
#define	E_IMPLEMENT	0x4009	/* This function not implemented	*/
#define	E_INFOTYPE	0x400A	/* Illegal Infotype for this file	*/
#define	E_INIT		0x400B	/* Error on Initialization of Driver	*/
#define	E_CONFLICT	0x400C	/* Cannot access file due to current usage */
				/* DELETE of open file.			*/
				/* INSTALL- Replace Driver in use	*/
#define	E_MEMORY	0x400D	/* Not enough memory available		*/
#define	E_MISMATCH	0x400E	/* Function Mismatch.  Attempt to perform */
				/*    a function on a file that does not*/
				/*    support the function.		*/
				/* INSTALL- Subdrive type mismatch	*/
#define	E_NAME		0x400F	/* Illegal file name specified		*/
#define	E_NO_FILE	0x4010	/* File Not Found.			*/
				/* CREATE- Device or Directory		*/
				/*    does not exist			*/
#define	E_PARAM		0x4011	/* Illegal Parameter specified		*/
				/* EXCEPTION- Illegal number		*/
#define	E_RECSIZE	0x4012	/* Record Size does not match request.	*/
#define	E_SUBDEV	0x4013	/* INSTALL - Sub-drive required		*/
#define	E_FLAG		0x4014	/* Bad Flag Number			*/

#define	E_NOMEM		0x4015	/* Non-existant memory			*/
#define	E_MBOUND	0x4016	/* Memory Bound error			*/
#define E_EBOUNDEX	E_MBOUND
#define	E_ILLINS	0x4017	/* Illegal instruction			*/
#define	E_DIVZERO	0x4018	/* Divide by zero			*/
#define	E_BOUND		0x4019	/* Bound exception			*/
#define	E_OFLOW		0x401A	/* Overflow exception			*/
#define	E_PRIV		0x401B	/* Privilege violation			*/
#define	E_TRACE		0x401C	/* Trace				*/
#define	E_BRKPT		0x401D	/* Breakpoint				*/
#define	E_FLOAT		0x401E	/* Floating point exception		*/
#define	E_STACK		0x401F	/* Stack fault				*/
#define	E_NOTON286	0x4020	/* Exception not caught by 286		*/
#define E_EM1		0x4021	/* emulated instruction group 1		*/
#define E_PFAULT	0x4022	/* 386 Page Fault 			*/


/****************************************************************/
/*								*/
/*  Supervisor - Range 0x4080 - 0x40FF				*/
/*								*/
/****************************************************************/
#define	E_ASYNC		0x4080	/* Asynchronous I/O not supported on	*/
				/*   function.				*/
#define	E_LOAD		0x4082	/* Bad Load Format			*/
#define	E_LOOP		0x4083	/* Infinite recursion (99 times) on	*/
				/*    Prefix Substitution.		*/
				/* INSTALL- Subdrive type mismatch	*/
#define	E_FULL		0x4084	/* File number table full		*/
#define	E_DEFINE	0x4085	/* DEFINE - Illegal name		*/

#define E_UNIT		0x4086	/* Too Many Driver Units		*/
#define E_UNWANTED	0x4087	/* Driver does not need Subdriver	*/
#define E_DVRTYPE	0x4088	/* Driver returns bad Driver type	*/
#define	E_LSTACK	0x4089	/* No load stack in cmd header		*/
#define E_FILETABLE	0x408A  /* Cannot override maximum number of	*/
				/* open files set at system generation	*/
/****************************************************************/
/*								*/
/*  Memory Error Definitions - Range 0x4100 - 0x417F		*/
/*								*/
/****************************************************************/
#define	E_POOL		0x4100	/* Out of memory pool			*/
#define	E_BADADD	0x4101	/* Bad address specified to free	*/

/****************************************************************/
/*								*/
/*  Kernel Error Definitions - Range 0x4180 - 0x41FF		*/
/*								*/
/****************************************************************/
#define	E_OVERRUN	0x4180	/* Flag already set			*/
#define	E_FORCE		0x4181	/* Return code of process being aborted	*/
#define	E_PDNAME	0x4182	/* Process ID not found on abort	*/
#define	E_PROCINFO	0x4183	/* COMMAND - no procinfo specified	*/
#define	E_LOADTYPE	0x4184	/* COMMAND - invalid loadtype		*/
#define	E_ADDRESS	0x4185	/* CONTROL - invalid memory access	*/
#define	E_EMASK		0x4186	/* Invalid event mask			*/
#define	E_COMPLETE	0x4187	/* Event has not completed		*/
#define	E_SRTL		0x4188	/* The required SRTL could no be found	*/
#define	E_ABORT		0x4189	/* Process cannot be terminated		*/
#define	E_CTLC		0x418A	/* Ctrl-C abort				*/
#define E_GO		0x418B	/* Slave process running		*/
#define E_INSWI		0x418C	/* Not in swi context; not a swi process*/
#define E_UNDERRUN	0x418D	/* Flag already pending			*/ 
#define	E_SRTLLEVEL	0x418E	/* Too many nested SRTLs		*/
#define E_SRTLHEADER	0x418F	/* 1). Header says SRTLs required, but
				    no SRTL names were specified.	*/
#define E_LOADHEADER	0x4190	/* 1). Illegal group. Loader only supports
				   CODE, DATA, and STACK groups.	*/
#define E_BADFIXUP	0x4191	/* 1). Corrupted fixup record.
				    a). Illegal group.
				    b). Offset > than 0x0f.
				    c). Fixup to memory group that does
				    not exist.
				    d). Fixup past the end of a memory
				    group.				*/
#define E_BADSRTLFIXUP	0x4192	/* 1). A non code to code fixup was
				   found(error for Type 1 SRTLs only).
				   2). Fixup to a SRTL when no SRTLs
				   are indicated in the command header.
				   3). Attempt to fixup to Type 2
				   SRTL data.				*/
#define E_MISSINGFIXUP	0x4193	/* 1). EOF reached before end of fixup
				   record found.			*/
#define	E_ADDRESSLIMIT	0x4194	/* 1). Could not get addressibility
				    a). Could not expand LDT
				     1). LDT limit hit.
				     2). Out of memory			*/
#define	E_LDTFULL	0x4195	/* 1). Reached physical limit. Really a 
				   FlexOS limit as we only support 8191
				   entries(depends on MTBL size).	*/
#define E_SRTLUSAGE	0x4196L	/* 1). An overlay is trying to use a SRTL
				   that was not specificied in the load
				   file of the process.			*/
#define E_SRTL_NOMATCH	0x4197L /* The version number of the SRTL found 
				   does not match the version number of
				   the requested SRTL.			*/

/****************************************************************/
/*								*/
/*  Disk Error Definitions - Range 0x4300 - 0x437F		*/
/*								*/
/****************************************************************/
#define	E_SPACE		0x4300	/* No block or directory entries	*/
				/* available				*/
#define E_MEDIACHANGE	0x4301	/* Media change occured			*/
#define E_MEDCHGERR	0x4302	/* Detected media change after a write	*/
#define E_PATH		0x4303	/* Bad path				*/
#define E_DEVCONFLICT	0x4304	/* Devices locked exclusively		*/
#define E_RANGE         0x4305	/* Address out of range			*/
#define E_READONLY	0x4306	/* Rename or delete on R/O file		*/
#define E_DIRNOTEMPTY	0x4307	/* Delete of non-empty directory	*/
#define E_BADOFFSET	0x4308	/* Badoffset in read, write or seek	*/
#define E_CORRUPT	0x4309	/* Corrupted Fat			*/
#define E_PENDLK        0x430A	/* Cannot unlock a pending lock		*/
#define E_RAWMEDIA	0x430B	/* Media does not support file system.	*/
				/* Not DOS media.			*/
#define	E_FILECLOSED	0x430C	/* File was closed before async lock	*/
				/* could be completed.			*/
#define	E_LOCK		0x430D	/* Lock access conflict			*/
#define	E_FATERR	0x430E	/* Error while reading the FAT		*/
#define E_OUT_OF_DRIVE_SLOTS 0x4021 /* Out of drive slots		*/

/****************************************************************/
/*								*/
/*  Network Error Definitions - Range 0x4380 - 0x44FF		*/
/*  See FLEXNET.H for error code descriptions			*/
/*								*/
/****************************************************************/

/****************************************************************/
/*								*/
/*  IBM PC machine emulation Errors - Range 0x4500 - 0x45ff	*/
/*								*/
/****************************************************************/
#define	E_VIDEO		0x4500	/* Attempt to do direct IO through
				   driver that does not support it.
				   DOS application error.		   */
#define	E_ROS10		0x4501	/* Attempt to do ROS(int 10) IO through
				   driver that does not support it.
				   DOS application error.		   */
#define	E_ROS16		0x4502	/* Attempt to do ROS(int 16) keyboard
				   input through driver that does not
				   support PC-DOS applications.
				   DOS application error.		   */

/****************************************************************/
/*								*/
/*  POSIX library signal return codes -  Range 0x4600 - 0x46ff	*/
/*  Low byte of return code is the signal number that caused	*/
/*  the process to terminate (00 - ff).				*/
/*								*/
/****************************************************************/

#define	E_SIGNAL		0x4600L

/****************************************************************/
/*								*/
/*  Note the order and definition of the following 16 error	*/
/*  codes must not change.  These error are to be returned to 	*/
/*  the appropriate resource manager.				b*/
/*								*/
/****************************************************************/

#define	E_WPROT		0x0000	/*  write protect violation     */
#define	E_UNITNO	0x0001	/*  illegal unitnumber		*/
#define	E_READY		0x0002	/*  drive not ready		*/
#define	E_INVCMD	0x0003	/*  invalid command issued	*/
#define	E_CRC		0x0004	/*  crc error on i/o		*/
#define	E_BADPB		0x0005	/*  bad parameter block		*/
#define	E_SEEK		0x0006	/*  seek operation failed	*/
#define	E_UNKNOWNMEDIA	0x0007	/*  unknown media present	*/
#define	E_SEC_NOTFOUND	0x0008	/*  req'd sector not found	*/
#define	E_DKATTACH	0x0009	/*  attchmt didn't respond	*/
#define	E_WRITEFAULT	0x000A	/*  write fault			*/
#define	E_READFAULT	0x000B	/*  read fault			*/
#define	E_GENERAL	0x000C	/*  general failure		*/
#define	E_MISADDR	0x000D	/*  missing address mark	*/ 
#define	E_NEWMEDIA	0x000E	/*  new media detected		*/
#define	E_DOOROPEN	0x000F	/*  door open			*/

/*---------------------------End of FLEXDEF.H------------------------*/
