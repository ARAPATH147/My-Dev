/******************************************************************************
FLEXTAB.H - SVC tables and operating system definitions for FlexOS            *
*******************************************************************************
Copyright (c) 1985,1991  Digital Research Inc. 
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
-------	-------	----	-------------------------------------------------------
22Oct91	AEB	5245	Changed comment for KPUT -- length field is number of
			characters, not (# of characters * 2).
11Oct91 ldt		Reversed SYSTEM and SYSTAB table fields version and
			release.
30Sep91	rfb		Added LI_F_POSTLINKED to the SRTL struct to indicate
			that the SRTL is postlinked.
19Jul91	ldt		Added PROCDEF, SYSDEF and SYSTEM table names to be
			compatible with Programmer's Guide.
18Jul91 ldt	4690    Changed DVS_* values to 0-4, PTB_* values to 0-15 and 
			PTS_* values to 0-7.
06Jun91 ldt		Added MX_SECUR flag and mxowner field to both the
			PIPE and SMTABLE. Also changed fields r1 and r2 in
			SMTABLE to recsize and flags.
04Jun91	NZ	4636	Change the PROST_* values.
03Jun91 NZ		Add MSC compatibility.
23Apr91 ldt		Added pp_flags to the pipe table.
05Dec90	glp		Updated PIPE table to agree with ES on NON-BLOCKED
			PIPE I/O.  Also added the user and group fields
			as someone had not done previously.
22Aug90 ldt		Changed DSKFNAM structure to use UBYTES for attributes.
22Mar90 MA		Changed use of 'far' to be compatible with both
			Turbo C and High C.
03Mar90 ldt		Added definitions to coordinate with PGR guide.
900215	RFW	----	Addtion if KJ addition to VDI_PRN table to allow 
			v_opnwk() to pass an output filename of 88 bytes.
891108	AeB	---- 	Added fill-style-index to LINEATTR structure --- used 
			for user defined pattern fill.
891106	ldt	----	Added MOUSTBL from char console file CVMTBL.H, the
			SRTL table and SYSDEF table.
891024  RFW	---- 	Added VCWM_UNDERL attribute moved table defines to 
			a central location. 
890921	ldt	----	Placed error codes and reserved names in FLEXDEF.H 
890908  cpg             Added structures LD_STRUCT2 and LD_STRUCT3, and made
                          'far' 286 specific.
890723	ldt	----	Added many defines and missing tables.
890321	aeb		Added parentheses to ambiguous #define arguments.
890125  whf		Use typedefs, not #defines, for Flexview ease of use
890118	reb		Added flexvdi.h
880105	reb     	Created.
******************************************************************************/
#if ( !IAPX186 && !IAPX286 && !IAPX386 )	/* The default condition */
#define IAPX286     1
#endif

#define __flextab_included

#ifndef __flexdef_included
#include "flexdef.h"
#endif

#ifdef __HIGHC__
#ifndef far

#if (IAPX286|IAPX186)
#define far  _far
#else
#define far
#endif

#endif	/* far */
#endif	/* __HIGHC__ */

/*
	Operating System Definitions
*/ 
/****************************************************************/
/*								*/	
/*	Driver types						*/
/*								*/
/****************************************************************/ 
#define	DVR_TIME	0x01	/* Timer Driver			*/
#define	DVR_PIPE	0x11	/* Pipe Driver			*/
#define	DVR_SHMEM	0x12	/* Shared Memory Driver		*/
#define	DVR_DISK	0x21	/* Disk Driver			*/
#define	DVR_CON		0x31	/* Console Driver (old)		*/
#define DVR_CVDI	0x38	/* Console VDI Driver		*/
/****************************************************************/
/* Network Driver types range from 0x61 - 0x6F			*/
/* See FLEXNET.H						*/
/****************************************************************/
#define	DVR_PRN		0x71	/* Printer Driver (old)		*/
#define DVR_SER		0x72	/* Serial Driver		*/
#define DVR_PRTVDI	0x78	/* Printer Graphics Driver	*/
#define DVR_METAVDI	0x79	/* MetaFile Graphics Driver(old)*/
#define DVR_PLOTVDI	0x7a	/* Plotter VDI Driver		*/
#define DVR_CAMVDI	0x7b	/* Camera VDI Driver		*/
#define DVR_SCANVDI	0x7c	/* Scanner VDI Driver		*/
#define	DVR_NETMAN	0x7d	/* Network Resource Manager	*/
#define DVR_CLOCK	0x7e	/* DOS Clock Driver Emulator	*/
#define DVR_NULL	0x7f	/* Null Device			*/ 
#define	DVR_PORT	0x81	/* Port Driver			*/
#define DVR_MOUSE	0x82	/* mouse VDI subdriver		*/
#define DVR_KB		0x83	/* keyboard VDI subdriver	*/
#define	DVR_SCRVDI	0x38	/* Screen VDI subdriver		*/
#define	DVR_TEXT	0x85	/* Text Subdriver (old)		*/
#define	DVR_OEM		0x90	/* 1st OEM Driver		*/
/****************************************************************/
/*								*/	
/*	System information table types				*/
/*								*/
/****************************************************************/
#define	T_PROC		0x00		/* Process table		*/
#define	T_ENV		0x01		/* Environment table		*/
#define	T_TD		0x02		/* Time and date table		*/
#define	T_MEM		0x03		/* Memory table			*/
#define	T_PIPE		0x10		/* Pipe table			*/
#define	T_SHMEM		0x11		/* Shared memory table		*/
#define	T_FILE		0x20		/* Disk File table		*/
#define	T_DISK		DVR_DISK	/* Disk table			*/
#define	T_CON		0x30		/* Screen table			*/
#define	T_PCON		DVR_CON		/* Physical Console table	*/
#define	T_VCON		0x32		/* Virtual Console table	*/
#define	T_MOUSE		0x33		/* Mouse table			*/
#define T_TOP		0x34		/* Top Border (no table)	*/
#define T_BOTTOM	0x35		/* Bottom Border (no table)	*/
#define T_LEFT		0x36		/* Left Border (no table)	*/
#define T_RIGHT		0x37		/* Right Border (no table)	*/
#define	T_VIRCON	0x38		/* vircon table number		*/
#define	T_SYS		0x40		/* System table			*/
#define	T_FNUM		0x41		/* File number table		*/
#define	T_SDEF		0x42		/* System Defines table		*/
#define	T_PDEF		0x43		/* Process Defines table	*/
#define	T_CMDENV	0x44		/* Command Environment table	*/
#define	T_DEV		0x45		/* Device table			*/
#define	T_PNAME		0x46		/* Pathname table		*/
#define T_SRTL		0x47		/* SRTL table			*/
#define T_PRTVDI	0x78	/* vdi printer driver and table number  */
/****************************************************************/
/* Network table types range from 0x60 - 0x6F			*/
/* See FLEXNET.H						*/
/****************************************************************/
#define T_SER		DVR_SER		/* Serial table			*/
#define	T_PRN		DVR_PRN		/* Printer table		*/
#define T_CLOCK		DVR_CLOCK	/* DOS Clock Driver table	*/
#define T_NULL		DVR_NULL	/* Null Device table		*/
#define	T_PORT		DVR_PORT	/* Port table			*/

/* Note:  Tables from 0x82 -> 0xff are special tables */


/* Miscellaneous buffer values sizes */ 
#define	SERSIZ		8	/* Size of serialization sequence	    */
#define	NAME1		10	/* Size of a name field			    */
#define	NAMEFN		12	/* Size of a filename 12345678.123          */
#define	NAME2		14	/* Size of a name field			    */
#define	NAME3		18	/* Size of a name field			    */
#define	MOUSIZ		16	/* Size of mouse data and mask fields	    */
#define	GBUFSIZ		128	/* Size of general buffer		    */
#define	VOLMAX		11	/* volume label name length		    */
#define	MAXNODE		65	/* Maximum length of a node name	    */
#define	DEVMAX	MAXNODE+15	/* device name (with node::) 		    */
#define	PATHMAX		128	/* path spec buffer size: see GBUFSIZ	    */
#define	FILMAX		10	/* filename buffer size: see NAME1	    */
#define MAXEXT		3	/* Maximum length of an extension	    */
#define	EXTMAX		5	/* file extension buffer size		    */
#define	SPECMAX		DEVMAX+PATHMAX+FILMAX+EXTMAX-3	  /* full spec size */


/****************************************************************/
/****************************************************************/
/*								*/
/*   FlexOS Table Structures.					*/
/*								*/
/****************************************************************/
/****************************************************************/

typedef	struct	_cmdenv
{
	UBYTE	cmd_file[GBUFSIZ];
	UBYTE	cmd_tail[GBUFSIZ];
}CMDENV;
#define CMDSIZE		(LONG)sizeof(CMDENV)

typedef	struct	_console
{
	UWORD	cn_tahead;
	UWORD	cn_smode;
	UWORD	cn_kmode;
	UWORD	cn_currow;
	UWORD	cn_curcol;
	UWORD	cn_nrows;
	UWORD	cn_ncols;
	UBYTE	cn_vcnum;
	UBYTE	cn_type;
	UBYTE	cn_name[NAME1];
}CONSOLE;
#define CONSIZE		(LONG)sizeof(CONSOLE)

						 /* console smode bit values */
#define	CSM_DUMB	0x0001		       /* disables ANSI/VT52 support */
#define	CSM_SXB		0x0002			   /* sixteen bit characters */
#define	CSM_CRLF	0x0004			    /* convert lf's to cr/lf */
						 /* console kmode bit values */
#define	CKM_NAB		0x0001			  /* disable aborts (CTRL-C) */
#define	CKM_NSC		0x0002		     /* disable scroll (CTRLs S & Q) */
#define	CKM_NXL		0x0004				   /* no translation */
#define	CKM_NV52	0x0008				 /* no VT52 decoding */
#define	CKM_SXB		0x0010			   /* sixteen bit characters */
#define	CKM_NEC		0x0020					  /* no echo */
#define	CKM_NCZ		0x0040				     /* no control z */
#define	CKM_TOG		0x0080				/* toggle characters */
#define	CKM_CRLF	0x0100			 /* convert CR or LF to CRLF */
#define	CKM_NCRE	0x0200			               /* No CR echo */
#define	CKM_NDE		0x0400			        /* No delimiter echo */

						  /* subset of console table */
typedef    struct _minicon
{
    UWORD	mc_tahead;		   /* # of keys in type-ahead buffer */
    UWORD	mc_sc;					      /* Screen Mode */
    UWORD	mc_kb;					    /* Keyboard Mode */
    UWORD	mc_currow;				      /* current row */
    UWORD	mc_curcol;
}MINICON;
#define MCONSIZE	(LONG)sizeof(MINICON)

typedef struct	_device
{
	ULONG	dev_lookid;
	UBYTE	dev_name[NAME1];
	UWORD	dev_type;
	UWORD	dev_access;
	UWORD	dev_instat;
	UWORD	dev_owner;
}DEVICE;
#define DEVSIZE		(LONG)sizeof(DEVICE)
						 /* device access bit values */
#define	DVA_SET		0x0001				   /* delete allowed */
#define	DVA_WRT		0x0004				    /* write allowed */
#define	DVA_RD		0x0008				     /* read allowed */
#define	DVA_SHR		0x0010			    /* shared access allowed */
#define	DVA_RMVD	0x0020				 /* removable device */
#define	DVA_DISK	0x0021				      /* Disk Driver */
#define	DVA_DLCK	0x0040				  /* DEVLOCK allowed */
#define	DVA_SHRO	0x0080			       /* shared access only */
#define DVA_PART	0x0100			/* device partitions allowed */
#define DVA_VRFY	0x0200			       /* Verify disk writes */

/* device installation status bit values */

#define DVS_NOTINST	0x0000	/* not installed */ 
#define DVS_NEEDSD	0x0001	/* needs a subdriver */
#define DVS_RMOWN	0x0002	/* owned by a resource manager */
#define DVS_DVROWN	0x0003 	/* owned by another driver */
#define DVS_OPT		0x0004	/* optional subdriver */	

/**/
typedef  struct	_dskfnam                             /* Short Diskfile table */
{
    ULONG	dn_key;
    UBYTE	dn_name[NAMEFN];
    UBYTE	dn_atts1;			     /* Disk file attributes */
    UBYTE	dn_atts2;			     /* Disk file attributes */
}DSKFNAM;
#define MDSKFSIZE	(LONG)sizeof(DSKFNAM)

/**/

typedef struct	_diskfile                                  /* Diskfile table */
{
    ULONG	df_key;
    UBYTE	df_name[NAME3];
    UBYTE	df_attr1;
    UBYTE	df_attr2;
    UWORD	df_recsize;
    UBYTE	df_user;
    UBYTE	df_group;
    UWORD	df_protect;
    UWORD	df_res0;
    UWORD	df_res1;
    UWORD	df_res2;
    ULONG	df_size;
    UWORD	df_modyear;
    UBYTE	df_modmonth;
    UBYTE	df_modday;
    UBYTE	df_modhr;
    UBYTE	df_modmin;
    UBYTE	df_modsec;
    UBYTE	df_res3;
}DISKFILE;
#define DSKFSIZE	(LONG)sizeof(DISKFILE)

/* file attributes - 1 */

#define	FA_READ		0x01	/* read/only			*/
#define	FA_HIDE		0x02	/* hidden file			*/
#define	FA_SYS		0x04	/* system file			*/
#define	FA_VOL		0x08	/* volume entry			*/
#define	FA_DIR		0x10	/* directory entry		*/
#define	FA_ARCH		0x20	/* archive			*/
#define	FA_PROT		0x40	/* protection enable		*/

/* file attributes - 2 */

#define FA2_SEC		0x01	/* security enabled (label only)*/
#define FA2_CASE	0x02	/* uppercase/lowercase names 	*/
				/* 0 = lowercase only		*/
/* file security */

#define	FS_OD	0x0001		/* owner delete/set		*/
#define	FS_OE	0x0002		/* owner execute		*/
#define	FS_OW	0x0004		/* owner write			*/
#define	FS_OR	0x0008		/* owner read			*/
#define	FS_GD	0x0010		/* group delete/set		*/
#define	FS_GE	0x0020		/* group execute		*/
#define	FS_GW	0x0040		/* group write			*/
#define	FS_GR	0x0080		/* group read			*/
#define	FS_WD	0x0100		/* world delete/set		*/
#define	FS_WE	0x0200		/* world execute		*/
#define	FS_WW	0x0400		/* world write			*/
#define	FS_WR	0x0800		/* world read			*/
#define FS_NO	0x0FFF		/* No security			*/

/**/

typedef struct _dsk_buf
{
	BYTE	name[NAME1];
	WORD	type;
	WORD	ioptions;
	UWORD	status;
	WORD	lrfid;
	WORD	lrnid;
	LONG	lrpid;
	ULONG	free;
	ULONG	size;
	UWORD	sectsize;
	UWORD	firstsect;
	ULONG	nsectors;
	UWORD	sects_tract;
	UWORD	sects_block;
	UBYTE	nfats;
	UBYTE	fatid;
	UWORD	nfsects;
	UWORD	dirsize;
	BYTE	nheads;
	BYTE	format;
	LONG	hidden;
	ULONG	syssize;
	BYTE	laflag;
	BYTE	lamode;
	BYTE	lauser;
	BYTE	lagroup;
	BYTE	label[NAME2];
}DSK_BUF;
#define DISKSIZE	(LONG)sizeof(DSK_BUF)

					       /* disk table type bit values */
#define	DTP_RMV		0x0001				  /* removable media */
					    /* disk table ioption bit values */
#define	DIO_URST	0x0001			     /* user raw set allowed */
#define	DIO_URWR	0x0004			   /* user raw write allowed */
#define	DIO_URRD	0x0008			    /* user raw read allowed */
#define	DIO_RDD		0x0020			  /* removable device driver */
#define	DIO_DLK		0x0040				 /* DEVLOCKs allowed */
#define	DIO_PDD		0x0100			  /* Partitioned disk driver */
#define	DIO_VER		0x0200				   /* Verify control */
					     /* disk table status bit values */
#define	DST_DLP		0x0001			   /* disk locked to process */
#define	DST_DLF		0x0002			    /* disk locked to family */
#define	DST_DOE		0x0004	      /* disk opened exclusively as a device */
#define	DST_IUP		0x0008		   /* disk in use by another process */
#define	DST_IUF		0x0010	 /* disk in use by processes in other family */
#define	DST_AFS		0x0020		   /* disk activated for file system */
#define	DST_FOP		0x0040			       /* files open on disk */
						  /* disk table format codes */
#define	DFM_RAW		(BYTE)0			/* RAW (unknown) file system */
#define	DFM_DOS1	(BYTE)1		  /* DOS (1.5 FAT bytes) file system */
#define	DFM_DOS2	(BYTE)2		    /* DOS (2 FAT bytes) file system */
					      /* disk table label flag codes */
#define	DLF_NOL		(BYTE)0				  /* no label exists */
#define	DLF_LEX		(BYTE)1				     /* label exists */
#define	DLF_DER		(BYTE)2		/* return device error on label read */
					 /* disk table label mode bit values */
#define	DLM_FSE		(BYTE)0x01		    /* file security enabled */
#define	DLM_CSE		(BYTE)0x02		/* lower & upper case enable */

typedef	struct	_environ
{
	LONG	en_stdin;
	LONG	en_stdout;
	LONG	en_stderr;
	LONG	en_overlay;
	UWORD	en_security;
	UWORD	en_rsvd;
	UBYTE	en_user;
	UBYTE	en_group;
	UWORD	en_fid;
	ULONG	en_pid;
	UWORD	en_rnid;
	UWORD	en_rfid;
	ULONG	en_rpid;
}ENVIRON;
#define ENVSIZE		(LONG)sizeof(ENVIRON)

typedef	struct	_filenum
{
	LONG	fn_filenum;
	UBYTE	fn_access;
	UBYTE	fn_table;

}FILENUM;
#define FNUMSIZE	(LONG)sizeof(FILENUM)

					  /* filenum table access bit values */
#define	FNA_SET		0x0001			       /* set access allowed */
#define	FNA_EXC		0x0002			   /* execute access allowed */
#define	FNA_WRT		0x0004			     /* write access allowed */
#define	FNA_RD		0x0008			      /* read access allowed */

typedef struct _memory
{
	LONG	m_free;
	LONG	m_total;
	LONG	m_system;
}MEMORY;
#define MEMSIZE		(LONG)sizeof(MEMORY)

typedef struct  _ovload0
{
        BYTE	far *codeaddr;
        BYTE	far *dataaddr;
}LD_STRUCT2;

typedef struct  _ovload1
{
	BYTE	far *codeaddr;
	BYTE	far *dataaddr;
	LONG	resvd3;
	LONG	resvd2;
	LONG	resvd1;
}LD_STRUCT3;

typedef struct _pathname
{
	UBYTE   pname[PATHMAX];
}PATHNAME;
#define PATHSIZE	(LONG)sizeof(PATHNAME)

typedef	struct	_pconsole
{
	UBYTE	pc_name[NAME1];
	UBYTE	pc_nvc;
	UBYTE	pc_cid;
	UWORD	pc_rows;
	UWORD	pc_cols;
	UWORD	pc_crows;
	UWORD	pc_ccols;
	UBYTE	pc_flags;
	UBYTE	pc_planes;
	UBYTE	pc_attrp;
	UBYTE	pc_extp;
	UWORD	pc_country;
	UBYTE	pc_nfkeys;
	UBYTE	pc_buttons;
	ULONG	pc_serial;
}PCONSOLE;
#define PCONSIZE	(LONG)sizeof(PCONSOLE)

/* pconsole table flag bit values */

#define	PCF_GRP		0x01				 /* graphics console */
#define	PCF_NNK		0x02			      /* no numerical keypad */
#define	PCF_MSS		0x04			   /* mouse support provided */
#define	PCF_CLR		0x08				    /* color console */
#define PCF_MMV		0x10		 /* Memory-mapped video (0 = serial) */
#define PCF_CGR		0x20 /* Currently in graphics mode (0 = character) */

/* pconsole table plane bit values */

#define	PCP_CHAR	0x01			/* character plane supported */
#define	PCP_ATTR	0x02			/* attribute plane supported */
#define	PCP_EXTN	0x04			/* extension plane supported */

/* pconsole table attribute plane bit values */

#define PCA_INT		0x08			    /* intensity ON; 0 = OFF */
#define PCA_BLNK	0x80		 	        /* blink ON; 0 = OFF */

/* pconsole table extension plane bit values */

#define PCE_2CELL	0x01		     /* two-cell characters; 0 = one */
#define PCE_2ND		0x02		  /* 2nd part of two-cell character; */
					  /* (0 = 1st part of one-cell char) */


typedef  struct _port_table
{
        WORD    pt_type;
        WORD    pt_state;
	BYTE	pt_baud;
	BYTE	pt_mode;
	BYTE	pt_control;
	BYTE	pt_resvd1;
}PORTINFO ;
#define PORTSIZE	(LONG)sizeof(PORTINFO)

/* port table type bit values */

#define PTT_UNDEFINED	0x0001	/* undefined				*/
#define PTT_SERIAL		0x0002	/* standard serial driver	*/
#define PTT_CHAR		0x0004	/* character I/O device		*/
#define PTT_PARALLEL	0x0008	/* standard parallel driver		*/

/* port table state bit values */

#define PTS_RTS			0	/* transmit enable		*/
#define PTS_CHREC		1	/* character recieved		*/
#define PTS_DSRCH		2	/* change in DSR or DCD		*/
#define PTS_PARERR		3	/* parity error			*/
#define PTS_OVRERR		4	/* overrun error		*/
#define PTS_FRMERR		5	/* framing error		*/
#define PTS_CD			6	/* carrier present (Data Carrier Detect)*/
#define PTS_DSR			7	/* Data Set Ready (DSR) active	*/

/* port table baud rate bit values */

#define PTB_50			0	/* 50 baud		*/
#define PTB_75			1	/* 75 baud		*/
#define PTB_110			2	/* 110 baud		*/
#define PTB_134_5		3	/* 134.5 baud	*/
#define PTB_150			4	/* 150 baud		*/
#define PTB_300			5	/* 300 baud		*/
#define PTB_600			6	/* 600 baud		*/
#define PTB_1200		7	/* 1200 baud	*/
#define PTB_1800		8	/* 1800 baud	*/
#define PTB_2000		9	/* 2000 baud	*/
#define PTB_2400		10	/* 2400 baud	*/
#define PTB_3600		11	/* 3600 baud	*/
#define PTB_4800		12	/* 4800 baud	*/
#define PTB_7200		13	/* 7200 baud	*/
#define PTB_9600		14	/* 9600 baud	*/
#define PTB_19200		15	/* 19200 baud	*/

/* port table mode bit values (word length, stop bits, parity) */

/* word length */

#define PTM_L5			0x0000	/* 5 bits */
#define PTM_L6			0x0001	/* 6 bits */
#define PTM_L7			0x0002	/* 7 bits */
#define PTM_L8			0x0003	/* 8 bits */

/* stop bits */

#define PTM_XSTOP		0x0000	/* none */
#define PTM_1STOP		0x0001	/* 1 stop bit */
#define PTM_12STOP		0x0002	/* 1.5 stop bits */
#define PTM_2STOP		0x0003	/* 2 stop bits */

/* parity */

#define PTM_NOPAR		0x0000	/* none */
#define PTM_ODDPAR		0x0001	/* odd */
#define PTM_PEVEN		0x0003	/* even */

/* port table control bit values */

#define PTC_TXENAB	0x0001	/* enable character transmission	*/
#define PTC_DRT		0x0002	/* Force Data Transmission Ready (DTR) low */
#define PTC_RXENAB	0x0004	/* enable character reception		*/
#define PTC_BRK		0x0008	/* force break signal 			*/
#define PTC_RESERR	0x0010	/* reset error 				*/
#define PTC_RTS		0x0020	/* force Request to Send (RTS) low	*/


typedef  struct _prninfo
{
        LONG    pr_status;
	BYTE	pr_mode;
	BYTE	pr_paper;
        WORD    pr_width;
	BYTE	pr_legmode;
	BYTE	pr_onepage;
	BYTE	pr_lpi;
	BYTE	pr_length;
        BYTE    pr_name[16];
}PRNINFO ;
#define PRNSIZE		(LONG)sizeof(PRNINFO)

/* printer table status bit values */

#define PRS_OFF		0x0001	/* off line 				*/
#define PRS_OUT		0x0002	/* out of paper 			*/
#define PRS_SLCERR	0x0004	/* select error				*/
#define PRS_INIERR	0x0008	/* initialization error		*/
#define PRS_MODERR	0x0010	/* illegal mode requested	*/
#define PRS_FRMERR	0x0020	/* framing error			*/
#define PRS_BUFFUL	0x0040	/* internal buffer full		*/
#define PRS_XONWT	0x0080	/* waiting for XON			*/

/* printer table mode bit values */

#define PRM_BOLD	0x0001	/* boldface				*/
#define PRM_GRA		0x0002	/* graphics				*/
#define PRM_ITL		0x0004	/* italics				*/
#define PRM_SUB		0x0008	/* subscript				*/
#define PRM_SUP 	0x0010	/* superscript				*/
#define PRM_COND	0x0020	/* condensed				*/
#define PRM_ELON	0x0040	/* elongated				*/
#define PRM_LTR		0x0080	/* letter quality			*/

/* printer table paper type bit values */

#define PRPT_WID	0x0001	/* wide paper				*/
#define PRPT_LTR	0x0002	/* letterhead				*/
#define PRPT_LBL	0x0004	/* labels				*/

/* printer table leg.mode bit values */

#define PRLM_BOLD	0x0001	/* boldface				*/
#define PRLM_GRA	0x0002	/* graphics				*/
#define PRLM_ITL	0x0004	/* italics				*/
#define PRLM_SUB	0x0008	/* subscript				*/
#define PRLM_SUP 	0x0010	/* superscript				*/
#define PRLM_COND	0x0020	/* condensed				*/
#define PRLM_ELON	0x0040	/* elongated				*/
#define PRLM_LTR	0x0080	/* letter quality			*/


typedef	struct	_process
{
	ULONG	p_pid;
	UWORD	p_fid;
	UBYTE	p_cid;
	UBYTE	p_vcid;
	UBYTE	p_name[NAME1];
	UBYTE	p_state;
	UBYTE	p_prior;
	ULONG	p_maxmem;
	UWORD	p_flags;
	UBYTE	p_cuser;
	UBYTE	p_cgroup;
	ULONG	p_parent;
	ULONG	p_events;
	ULONG	p_code;
	ULONG	p_csize;
	ULONG	p_data;
	ULONG	p_dsize;
	ULONG	p_heap;
	ULONG	p_hsize;
}PROCESS;
#define PROCSIZE	(LONG)sizeof(PROCESS)


/* process table state bit values */

#define PROST_RUN	0x0000	/* run			*/
#define PROST_WAIT	0x0001	/* waiting		*/
#define PROST_TERM	0x0002	/* terminating	*/
#define PROST_SHAR	0x0003	/* shared		*/

/* process table flag bit values */

#define	PCF_SYS		0x0001				   /* system process */
#define	PCF_LKM		0x0002				 /* locked in memory */
#define	PCF_SWI		0x0004			   /* running in SWI context */
#define	PCF_SUP		0x0008			     /* originally superuser */


typedef  struct  _pipetab
{
        LONG    pp_key;					   /* Key for lookup */
        BYTE    pp_name[NAME1];					/* File name */
        WORD    pp_size;				      /* Buffer size */
        WORD    pp_recsize;				      /* Record size */
        WORD    pp_security;				    /* File security */
	BYTE	pp_user;			      /* Owner's user number */
	BYTE	pp_group;			     /* Owner's group number */
	WORD	pp_flags;			       /* Flags for the pipe */
	LONG	pp_res2;					 /* reserved */
	LONG	pp_res3;					 /* reserved */
	LONG	pp_cfilesize;				/* Current file size */
	LONG	pp_mxowner;			    /* ID of semaphore owner */
}PIPETAB ;
#define PIPESIZE	(LONG)sizeof(PIPETAB)

/* Pipe and Shared Memory table flag bit values */
#ifndef MX_SECUR
#define MX_SECUR	0x01			/* Enable semaphore security */
#endif

typedef	struct	_vconsole
{
	ULONG	vc_lookid;
	UWORD	vc_mode;
	UBYTE	vc_vcnum;
	UBYTE	vc_type;
	WORD	vc_viewrow;
	WORD	vc_viewcol;
	WORD	vc_nrow;
	WORD	vc_ncol;
	WORD	vc_posrow;
	WORD	vc_poscol;
	WORD	vc_rows;
	WORD	vc_cols;
	BYTE	vc_top;
	BYTE	vc_bottom;
	BYTE	vc_left;
	BYTE	vc_right;
}VCONSOLE;
#define VCONSIZE	(LONG)sizeof(VCONSOLE)

/* virtual console table mode bit values */

#define	VCM_FZB		0x0001				    /* freeze border */
#define	VCM_AVC		0x0002			   /* allow auto view change */
#define	VCM_KCE		0x0004		     /* keep cursor on edge (on AVC) */
#define	VCM_AVO		0x0008		       /* auto view change on output */


/* virtual console type bit values */

#define	VCT_GRP		0x01		/* graphics vcon LARRY TOWNER */
#define VCT_NNK		0x02		/* no numeric keypad						*/
#define VCT_MSS		0x04		/* mouse support; 0 = no mouse				*/
#define VCT_CLR		0x08		/* color; 0 = monochrome					*/
#define VCT_MMV		0x10		/* memory-mapped video; 0 = serial device	*/

#define	VCT_CGM		0x10		/* Currently in graphics mode */
#define VCT_CGR		0x20		/* currently in graphics mode; 0 = char		*/


#define PL_NPLANE	3
typedef	struct	_fframe
{
    UBYTE	far *fr_pl[PL_NPLANE];	/* char, attrib and extension planes */
    UWORD	fr_nrow;
    UWORD	fr_ncol;
    UWORD	fr_use;
}FFRAME;
#define FRAMSIZE	(LONG)sizeof(FFRAME)

#define PL_USEC		0x01			       /* Plane 0 usage mask */
#define PL_USEA		0x02			       /* Plane 1 usage mask */
#define PL_USEE		0x04			       /* Plane 2 usage mask */
#define PL_USEALL	0x07			    /* All Planes usage mask */

#define PL_CHAR		0		     /* Character Plane Index, Shift */
#define PL_ATTR		1		     /* Attribute Place Index, Shift */
#define PL_EXTN		2		     /* Extension Plane Index, Shift */


typedef	struct	_rect		/******************************************/
{				/***					***/
    WORD	r_row;		/***   The actual FLexos implementation ***/
    WORD	r_col;		/***   uses col & row in different order***/
    WORD	r_nrow;		/***   then GEM. This is the the Flex286***/
    WORD	r_ncol;		/***   Affected are BWAIT, KCTRL calls. ***/
}RECT;				/***					***/
				/******************************************/
#define FRECTSIZE	(LONG)sizeof(RECT)

/*		
   command process info
*/

typedef struct _pinfo
{
	BYTE	pi_pname[NAME1];
	BYTE	pi_prior;
	BYTE	pi_rsvd1;
	LONG	pi_maxmem;
	LONG	pi_addmem;
}PINFO;
#define PINFSIZE	(LONG)sizeof(PINFO)

				      /* memory parameter block for S_MALLOC */
typedef	struct	_mpb
{
	ULONG	mp_start;
	ULONG	mp_min;
	ULONG	mp_max;
}MPB;
#define MPBSIZE		(LONG)sizeof(MPB)


/* */
typedef	struct _smtable                           /* Shared memory get table */
{
    LONG	key;                                 /* key field for lookup */
    BYTE	name[NAME1];                                 /* SM file name */
    UWORD	size;                             /* user/system buffer size */
    UWORD	recsize;                                      /* record size */
    UWORD	security;                              /* file security word */
    BYTE	user;                                  /* file owner user ID */
    BYTE	group;                                /* file owner group ID */
    UWORD	flags;                                /* flags for semaphore */
    BYTE	far *ubuffer;               /* ptr to user accessable buffer */
    BYTE	far *sbuffer;             /* ptr to system accessable buffer */
    LONG	res1;						 /* reserved */
    LONG	mxowner;			    /* ID of semaphore owner */
}SMTABLE;    
#define SMSIZE		(LONG)sizeof(SMTABLE)

/* Pipe and Shared Memory table flag bit values */
#ifndef MX_SECUR
#define MX_SECUR	0x01			/* Enable semaphore security */
#endif

/*
  Software Run-Time Library (SRTL) table
*/

typedef struct _srtltable
{
    LONG	key;				     /* key field for lookup */
    BYTE	name[8];				   /* SRTL file name */
    WORD	reserved;				   /* Reserved Field */
    WORD	use_count;	  /* Current number of users using the table */
    WORD	maj_version;	    /* Major number specified in SRTL header */
    WORD	min_version;	    /* Minor number specified in SRTL header */
    BYTE	flag1;					  /* SRTL Attributes */ 
    BYTE	flag2;					  /*        "        */
    BYTE	flag3;					  /*        "        */
    BYTE	flag4;					  /*        "        */
    LONG	size;		       /* Number of bytes this SRTL occupies */
}SRTL;
#define SRTLSIZE	(LONG)sizeof(SRTL)

#define LI_F_MODEL	0x01		     /* flag2 bit 1 - New model SRTL */
#define LI_F_286	0x01		   /* flag3 bit 1 - Code type is 286 */
#define LI_F_DUP_MAIN	0x10  /* flag3 bit 4 - Allow dup defs of main module */
#define LI_F_DS_STACK	0x20       /* flag3 bit 5 - Stack appears at low end */
			       /* of the data segment in small memory module */
/* 30Sep91 rfb -- Added LI_F_POSTLINKED */
#define LI_F_POSTLINKED	0x80	/* Flag[2] bit 7 - SRTL is postlinked */

/*
  System definition name table for process and system level logical names 
*/ 
typedef	struct	_define
{
	ULONG	def_key;			    /* key field for lookup */
	UBYTE	def_lname[NAME1];		       /* Logic name string */
	UBYTE	def_prefix[GBUFSIZ];     /* Prefix name substitution string */
}DEFINE;
#define DEFSIZE		(LONG)sizeof(DEFINE)

typedef	struct	_procdef	
{
	ULONG	def_key;			    /* key field for lookup */
	UBYTE	def_lname[NAME1];		       /* Logic name string */
	UBYTE	def_prefix[GBUFSIZ];     /* Prefix name substitution string */
}PROCDEF;
#define PDEFSIZE	(LONG)sizeof(PROCDEF)

typedef	struct	_sysdef
{
	ULONG	def_key;			    /* key field for lookup */
	UBYTE	def_lname[NAME1];		       /* Logic name string */
	UBYTE	def_prefix[GBUFSIZ];     /* Prefix name substitution string */
}SYSDEF;
#define SDEFSIZE	(LONG)sizeof(SYSDEF)

typedef  struct _systab
{
        BYTE    sys_cpu;
        BYTE    sys_ostype;
        BYTE    sys_release;
        BYTE    sys_version;
        BYTE    sys_serial[8];
        LONG    sys_idlecnt;
	LONG	sys_curfile;
	LONG	sys_maxfile;
	LONG	sys_filespertab;
}SYSTAB ;
#define SYSIZE		(LONG)sizeof(SYSTAB)

typedef  struct _system
{
        BYTE    sys_cpu;
        BYTE    sys_ostype;
        BYTE    sys_release;
        BYTE    sys_version;
        BYTE    sys_serial[8];
        LONG    sys_idlecnt;
	LONG	sys_curfile;
	LONG	sys_maxfile;
	LONG	sys_filespertab;
}SYSTEM ;
#define SYSTEMSIZE		(LONG)sizeof(SYSTEM)

/**/
typedef struct _timedate
{
    WORD	td_year;
    BYTE	td_month;
    BYTE	td_day;
    LONG	td_time;
    WORD	td_timezone;
    BYTE	td_weekday;
    BYTE	td_reserved;
}TIMEDATE;
#define TIMESIZE	(LONG)sizeof(TIMEDATE)
/**/


struct PARAMBLOCK
    {
        BYTE    _pmode;
        BYTE    _poption;
        WORD    _pflags;
#if __MSC__
        LONG	(far *_pswi)();
#else
        LONG    far (*_pswi)();
#endif
        union	
	{
		LONG	_long ;
		UWORD	_word[2] ;
		VOID	far *_ptr ;
	}	_p1;
        union 
	  {
                LONG    _long;
                UWORD	_word[2];
		VOID	far *_ptr ;
          }     _p2;
        union	
	{
		LONG	_long ;
		UWORD	_word[2] ;
		VOID	far *_ptr ;
	}	_p3;
        union	
	{
		LONG	_long ;
		UWORD	_word[2] ;
		VOID	far *_ptr ;
	}	_p4;
        union	
	{
		LONG	_long ;
		UWORD	_word[2] ;
		VOID	far *_ptr ;
	}	_p5;
    } ;


/******************************************************************************
*
* This section is intended for the use of FlexOS application writers to be
* included in their application programs.
*
* It is assumed that PORTAB.H has been included before this file.
*
******************************************************************************/
#ifndef GMODE
#define GMODE	1	/* If 1, using Graphics Operation Mode, or	     */
#endif			/*	Alternate Character Operations Mode.	     */
			/* If 0, using Character Operations Mode.	     */
 
		/* NOTE: Character Ops Mode is compatible with the   */
		/*	FlexOS 1.x console system		     */

/******************************************************************************
* RECTSIZE data structure
*
* The RECTSIZE data structure describes the height and width of an
* object.  The format of the two words depends on the current operations
* mode in VC_SMODE field.
*
*	Graphics Ops Mode or Alternate Char Ops Mode:
*	+---------------+---------------+
*	| number cols(X)| number rows(Y)|
*	+---------------+---------------+
*
*	Character Ops Mode:
*	+---------------+---------------+
*	| number rows	| number columns|
*	+---------------+---------------+
*
******************************************************************************/

typedef	struct _rectsize
{
#if GMODE == 1
    WORD rs_ncols;	/* Number of Columns (X) */
    WORD rs_nrows;	/* Number of Rows (Y) */
#else
    WORD rs_nrows;	/* Number of Rows (Y) */
    WORD rs_ncols;	/* Number of Columns (X) */
#endif	/* End of GMODE == 1 */
}RECTSIZE;

/******************************************************************************
* POSITION data structure
*
*	The POSITION data structure describes a current location on a
*	two dimensional object like the screen.  The format depends on the
*	current operations mode in the VC_SMODE field.
*
*	Graphics Ops Mode or Alternate Char Ops Mode:
*	+---------------+---------------+
*	| column(X)	| row(Y)	|
*	+---------------+---------------+
*
*	Character Ops Mode:
*	+---------------+---------------+
*	| row		| column	|
*	+---------------+---------------+
*
******************************************************************************/

typedef	struct _position
{
#if GMODE == 1
    WORD pos_col;	/* Column (X) */
    WORD pos_row;	/* Row (Y) */
#else
    WORD pos_row;	/* Row (Y) */
    WORD pos_col;	/* Column (X) */
#endif	/* End of GMODE == 1 */
}POSITION;

/******************************************************************************
* RECTANGLE data structure
*
*	The RECTANGLE data structure describes a rectangular region. The
*	format depends on the current operations mode in the VC_SMODE field.
*
*	Graphics Ops Mode or Alternate Char Ops Mode:
*	+---------------+---------------+---------------+---------------+
*	| ul column(X)	| ul row(Y)	| lr column(X)	| lr row(Y)	|
*	+---------------+---------------+---------------+---------------+
*		(ul = upper left, lr = lower right)
*	Character Ops Mode:
*	+---------------+---------------+---------------+---------------+
*	| ul row	| ul column	| number rows	| number cols	|
*	+---------------+---------------+---------------+---------------+
*
******************************************************************************/

typedef	struct _rectangle
{
    POSITION	r_ul;		/* Upper Left Corner */
#if GMODE == 1
    POSITION	r_lr;		/* Lower Right Corner */
#else
    RECTSIZE	r_size;		/* Size of rectangle */
#endif	/* End of GMODE == 1 */
}RECTANGLE;

#define BYTEORDER	0x0102	/* byte order field value		     */

/******************************************************************************
*	WRITING MODES
*******************************************************************************/

/* Writing modes used with COPY RASTER OPAQUE */

#define	WM_BLANK	0		/* Erase everything */
#define	WM_AND		1		/* Set only where both are set */
#define	WM_NAND		2		/* Set only where dest. is zero */
#define	WM_REPLACE	3		/* Replace write mode */
#define	WM_ERASE	4		/* Erase mode */
#define	WM_NOTHING	5		/* Doesn't change destination */
#define	WM_XOR		6		/* Exclusive OR write mode */
#define	WM_TRANSPARENT	7		/* Transparent write mode */
#define	WM_NOT_SORD	8		/* D' = !(S | D) */
#define	WM_NT_SXORD	9		/* D' = !(S x D) */
#define	WM_DFLIP	10		/* D' = !D */
#define	WM_S_OR_NOTD	11		/* D' = S | !D */
#define	WM_REVERSE	12		/* D' = !S */
#define	WM_RVRSTRANS	4		/* Reverse Transparent write mode */
#define	WM_NOTS_ORD	13		/* D' = !S | D */
#define	WM_NT_SAND	14		/* D' = !(S & D) */
#define	WM_SET		15		/* D' = 1 */

/* Standard writing modes, used with all VDI calls except for
   COPY RASTER OPAQUE.
*/

#define	SWM_REPLACE	1		/* Replace Mode */
#define	SWM_TRANSPARENT	2		/* Transparent Mode */
#define	SWM_XOR		3		/* Exclusive OR Mode */
#define	SWM_RVRSTRANS	4		/* Reverse Transparent Mode */

/******************************************************************************
*	Line End Styles
*******************************************************************************/

#define	LE_SQUARED	0		/* Squared off line end */
#define	LE_ARROW	1		/* Arrow line end */
#define	LE_ROUNDED	2		/* Rounded line end */

/******************************************************************************
*	Text Special Effects
*******************************************************************************/

#define	TX_THICK	1		/* Thick characters */
#define	TX_LIGHT	2		/* "Gray" characters */
#define	TX_SKEW		4		/* Skewed, or italic, characters */
#define	TX_ULINE	8		/* Underlined characters */
#define	TX_OUTLINE	16		/* Outlined characters */
#define	TX_SHADOW	32		/* Shadowed characters */

/******************************************************************************
* VDI WRITE Subfunction Numbers, Data Buffer Formats
*
*	A Write buffer (in Graphics Operations Mode) consists of
*	the Byte Order WORD followed by a series of Write Commands.
*	All information in the WRITE buffer consists of 16-bit entities or
*	structures consisting of 16-bit entities.
*
*	Each Write Command starts with the Write Function Number followed by
*	the Length of the command.  The length is the number of WORDs following
*	the length field before reaching the next command or end of buffer.
*
*	Each Write Command has its own format.
*
*	In the following definitions, the comments describe the contents of
*	the command.  Typically, the command number is followed by attributes,
*	then data.  The attributes are data structures described below.
*
*	Data can be a structure or a list of items defined as follows:
*
*	<Points>	[Number of Points],[POSITION 1], ..., [POSITION N]
*	<String>	[Number of Chars],[16-bit CHAR 1], ..., [CHAR N]
*	<Point-Text>	[Number of Chars],[POSITION 1],[CHAR 1],...,
*				[POSITION N],[CHAR N]
*
******************************************************************************/

	/* Write Commands */

#define WR_PLINE	1	/* Polyline	LINEATTR,<Points>	     */
#define WR_PMARK	2	/* Polymarker	MARKATTR,<Points>	     */
#define WR_TEXT		3	/* Text		TEXTATTR,<String>	     */
#define WR_FILL		4	/* Fill		FILLATTR,<Points>	     */
#define WR_BOX		5	/* Box		LINEATTR,RECTANGLE	     */
#define WR_BAR		6	/* Bar		FILLATTR,RECTANGLE	     */
#define WR_ARC		7	/* Arc		LINEATTR,ELARC		     */
#define WR_PIE		8	/* Pie		FILLATTR,ELARC		     */
#define WR_RBOX		9	/* Rounded Box	LINEATTR,RECTANGLE	     */
#define WR_RBAR		10	/* Rounded Bar	FILLATTR,RECTANGLE	     */
#define WR_ESCTEXT	11	/* Escaped Text	TEXTATTR,<Point-Text>	     */
#define WR_CONFILL	12	/* Contour Fill	FILLATTR,flags,POSITION	     */

#define WR_CLIP		32	/* Clip Region	RECTANGLE		     */
#define WR_CLEAR	33	/* Clear Device (none)			     */

#define WR_NSYSCLIP	64	/* New Sysclip	RECTANGLE		     */
#define WR_ASYSCLIP	65	/* Add Sysclip	RECTANGLE		     */

#define WR_FORM		128	/* Form Advance	(none)			     */
#define WR_OWINDOW	129	/* Output Window	RECTANGLE	     */
#define WR_CLRDLIST	130	/* Clear Display List	(none)		     */

/******************************************************************************
* LINEATTR - Line Attributes Data Structure
******************************************************************************/

typedef	struct _lineattr
{
    UWORD	la_length;	/* # Words of Line Attributes that follow    */
    UWORD	la_locolor;	/* Color Index				     */
    UWORD	la_hicolor;	/* Color Index				     */
    UWORD	la_wrmode;	/* Writing Mode				     */
    UWORD	la_pattern;	/* Line Pattern				     */
    WORD	la_width;	/* Line Width				     */
    UWORD	la_bstyle;	/* Beginning Line Style			     */
    UWORD	la_estyle;	/* Ending Line Style			     */
    UWORD	la_sindex;	/* Fill Style Index			     */
}LINEATTR;
#define LATR_LEN	(sizeof(LINEATTR)/2) - 1	/* la_length	     */

/******************************************************************************
* FILLATTR - Fill Attributes Data Structure
******************************************************************************/

typedef	struct _fillattr
{
    UWORD	fa_length;	/* # Words of Fill Attributes that follow    */
    UWORD	fa_locolor;	/* Color Index				     */
    UWORD	fa_hicolor;	/* Color Index				     */
    UWORD	fa_wrmode;	/* Writing Mode				     */
    UWORD	fa_sindex;	/* Fill Style Index			     */
    UWORD	fa_style;	/* Fill Style				     */
    UWORD	fa_flags;	/* Fill Flags				     */
}FILLATTR;
#define FATR_LEN	(sizeof(FILLATTR)/2) - 1	/* fa_length	     */

	/* Fill Flags (fa_flags) Bit Definitions */

#define	FAF_PVIS	0x0001	/* 0: 1 = Visible Perimeter, 0 = Invisible   */

/******************************************************************************
* MARKATTR - Marker Attributes Data Structure
******************************************************************************/

typedef	struct _markattr
{
    UWORD	ma_length;	/* # Words of Fill Attributes that follow    */
    UWORD	ma_locolor;	/* Color Index				     */
    UWORD	ma_hicolor;	/* Color Index				     */
    UWORD	ma_wrmode;	/* Writing Mode				     */
    UWORD	ma_type;	/* Marker Type				     */
    UWORD	ma_height;	/* Marker Height			     */
}MARKATTR;
#define MATR_LEN	(sizeof(MARKATTR)/2) - 1	/* ma_length	     */

/******************************************************************************
* TEXTATTR - Text Attributes Data Structure
******************************************************************************/

typedef	struct _textattr
{
    UWORD	ta_length;	/* # Words of Text Attributes that follow    */
    UWORD	ta_locolor;	/* Color Index				     */
    UWORD	ta_hicolor;	/* Color Index				     */
    UWORD	ta_wrmode;	/* Writing Mode				     */
    UWORD	ta_face;	/* Font Face				     */
    UWORD	ta_rotate;	/* Text Rotation			     */
    UWORD	ta_halign;	/* Horizontal Alignment			     */
    UWORD	ta_valign;	/* Vertical Alignment			     */
    UWORD	ta_height;	/* Character Height			     */
    UWORD	ta_effects;	/* Text Effects				     */
    UWORD	ta_flags;	/* Text Flags				     */
    POSITION	ta_pos;		/* Alignment Point			     */
    UWORD	ta_pixlen;	/* Length of justified string in pixels      */
}TEXTATTR;
#define TATR_LEN	(sizeof(TEXTATTR)/2) - 1	/* ta_length	     */

	/* Text Flags (ta_flags) Bit Definitions */

#define TAF_JUSTIFY	0x0001	/* 0: 1 = Justify Text, 0 = No Justify	     */
#define TAF_WORDSPACE	0x0002	/* 1: 1 = Modify Word Spacing, 0 = Don't     */
#define TAF_CHARSPACE	0x0004	/* 2: 1 = Modify Char Spacing, 0 = Don't     */
#define TAF_ISPOINTS	0x0008	/* 3: 1 = ta_height ='s Font point size      */
				/*    0 = ta_height ='s height in pixels     */

/******************************************************************************
* ELARC Data Structure
******************************************************************************/

typedef	struct _elarc
{
    UWORD	el_bangle;	/* Beginning Angle			     */
    UWORD	el_eangle;	/* End Angle				     */
    POSITION	el_center;	/* Center Position			     */
    UWORD	el_xradius;	/* X radius				     */
    UWORD	el_yradius;	/* Y radius				     */
}ELARC;

/******************************************************************************
* VDI SPECIAL Subfunction Numbers, Flags, Parm and Data Buffer Formats
******************************************************************************/

#define DSEND	64	/* Data Buffer is a "send" buffer */
#define PSEND	128	/* Parm Buffer is a "send" buffer */

/******************************************************************************
* UDPAT		Set User-defined Pattern
*
*	parmblk.option	VS_UDPAT	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDUDPAT structure (send)
*	parmblk.dbufsiz	sizeof(VSUDPAT)
*	parmblk.parmbuf	Address of bit pattern (plane 1 ... plane n). (send)
*	parmblk.pbufsiz	# bytes of planes information
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_UDPAT	(0 + DSEND + PSEND)	/* Sub Function Number	     */

typedef	struct _vsdudpat	/* Data Buffer Structure     */
{
    UWORD	dud_byteorder;	/* byte order				     */
    UWORD	dud_planes;	/* # of planes				     */
    UWORD	dud_blcolor;	/* background color			     */
    UWORD	dud_bhcolor;	/* background color			     */
    UWORD	dud_flcolor;	/* foreground color			     */
    UWORD	dud_fhcolor;	/* foreground color			     */
    UWORD	dud_pwidth;	/* pattern width			     */
    UWORD	dud_pheight;	/* pattern height			     */
}VSDUDPAT;

/******************************************************************************
* GETPIX	Get Pixel Value
*
*	parmblk.option	VS_GETPIX	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDGETPIX structure (send)
*	parmblk.dbufsiz	sizeof(VSDGETPIX)
*	parmblk.parmbuf	Address of VSPGETPIX structure (receive)
*	parmblk.pbufsiz	sizeof(VSPGETPIX)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_GETPIX	(1 + DSEND)	/* Sub Function Number */

typedef	struct _vsdgetpix	/* Data Buffer Structure */
{
    UWORD	dgp_byteorder;	/* Byte Order				     */
    POSITION	dgp_position;	/* Position of Pixel to examine		     */
}VSDGETPIX;

typedef	struct _vspgetpix	/* Parm Buffer Structure */
{
    UWORD	dgp_pel;	/* Pixel Value				     */
    UWORD	dgp_lcolor;	/* Color Index				     */
    UWORD	dgp_hcolor;	/* Color Index				     */
}VSPGETPIX;

/******************************************************************************
* LWIDTH	Get Line Widths
*
*	parmblk.option	VS_LWIDTH	
*	parmblk.flags	0
*	parmblk.databuf	(LONG)BYTEORDER
*	parmblk.dbufsiz	NULL
*	parmblk.parmbuf	Address of line width buffer (UWORD) (receive)
*	parmblk.pbufsiz	(# of line widths expected) * 2
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_LWIDTH	2	/* Sub Function Number */

/******************************************************************************
* MSIZE	Get Marker Sizes
*
*	parmblk.option	VS_MSIZE	
*	parmblk.flags	0
*	parmblk.databuf	(LONG)BYTEORDER
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	Address of marker size buffer (UWORD) (receive)
*	parmblk.pbufsiz	(# of marker sizes expected) * 2
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_MSIZE	3	/* Sub Function Number */

/******************************************************************************
* VDI INQUIRE	Provides functionality for VDI functions which preform an 
*		action as well as return information back to the application.
*		These function mainly deal with Graphic Text and Font functions.
*
*	parmblk.option	VS_INQUIRE	
*	parmblk.flags	0
*	parmblk.databuf	Address of Inquire command buffer similar Graphic 
*			Command buffer(send)
*	parmblk.dbufsiz	Number of bytes in command buffer.
*	parmblk.parmbuf	Address of returned information  (receive)
*	parmblk.pbufsiz	size of return information 
*
*	Return:	0L - Success
*		Error Code
*-------------------------------------------------------------------------------
* VDI INQUIRE Data Buffer Format
*
*	A Inquire buffer (in Graphics Operations Mode) consists of
*	the Byte Order WORD followed by a series of Inquire Commands.
*	All information in the INQUIRE buffer consists of 16-bit entities or
*	structures consisting of 16-bit entities.
*
*	Each Inquire Command starts with the Inquire Function Number followed by
*	the Length of the command.  The length is the number of WORDs following
*	the length field before reaching the next command or end of buffer.
*
*	Each Inquire Command has its own format.
*
*	In the following definitions, the comments describe the contents of
*	the command.  Typically, the command number is followed by attributes,
*	then data.  The attributes are the TEXTATTR structure described above.
*
*	Data can be a structure or a list of items defined as follows:
*
*	<String>	[Number of Chars],[16-bit CHAR 1], ..., [CHAR N]
* 	   or		no data at all.
*
******************************************************************************/

#define VS_INQUIRE	(4 + DSEND)	/* Sub Function Number */

/* VS_INQUIRE subfunctions definitions and structures */
				/*      Inputs		  Outputs        */
#define IQ_JUSTIFIED	1	/* <TEXTATTR>,<STRING> <VSJWIDTH*strlen> */

typedef WORD	VSJWIDTH;	/* Width for each character in string */


#define IQ_QJUSTIFIED	2	/* <TEXTATTR>,<STRING> <VSQJWIDTH*strlen> */

typedef struct
{
    POSITION qj_xyoff;		/* x,y position of left base of character */
} VSQJWIDTH;

#define IQ_EXTENT	3	/* <TEXTATTR>,<STRING>  <VSEXTENT> 	  */

typedef struct
{
    POSITION	ext_1delta;	/* delta x,y for point1 of string */
    POSITION	ext_2delta;	/* delta x,y for point2 of string */
    POSITION	ext_3delta;	/* delta x,y for point3 of string */
    POSITION	ext_4delta;	/* delta x,y for point4 of string */
} VSEXTENT;

#define IQ_WIDTH	4	/* <TEXTATTR>,<STRING> <VSWIDTH*strlen>   */

typedef struct
{
    WORD	wid_width;	/* cell width of character */
    WORD	wid_ldelta;	/* left character alignment delta */
    WORD	wid_rdelta;	/* right character alignment delta */
} VSWIDTH;

#define IQ_FONTINFO	5	/* <TEXTATTR>		<VSFONT> 	  */

typedef struct
{
    WORD	fnt_minADE;	/* Minimum ASCII Decimal Equivalent char */
    WORD	fnt_maxADE;	/* Maximum ASCII Decimal Equivalent char */
    WORD	fnt_maxwidth;	/* Maximum character cell width */
    WORD	fnt_bottom;	/* Bottom line distance */
    WORD	fnt_descent;	/* Descent line distance */
    WORD	fnt_half;	/* Half line distance */
    WORD	fnt_ascent;	/* Ascent line distance */
    WORD	fnt_top;	/* Top line distance */
    WORD	fnt_xeffects;	/* Special effects delta-X */
    WORD	fnt_loffset;	/* Left offset */
    WORD	fnt_roffset;	/* Right offset */
} VSFONT;

#define IQ_FONTSELECT	6	/* <TEXTATTR>		<VSFNTSELECT>   */

typedef struct
{
    WORD	fsel_index;	/* Selected font face index */
    WORD	fsel_height;	/* Selected font height */
    WORD	fsel_point;	/* Selected font point size */
    RECTSIZE	fsel_chsize;	/* Selected font character size */
    RECTSIZE	fsel_cesize;	/* Selected font character cell size */
} VSFNTSELECT;

#define IQ_FONTNAME	7	/* <TEXTATTR>,WORDitem_number <VSFONTNAME>	*/

typedef struct
{
    WORD	fnt_id;		/* Font Face ID	*/
    WORD	fnt_effects;	/* Effects supported by FACE */
    BYTE	fnt_name[16];	/* Font name */
} VSFNAME;
/******************************************************************************
* FHNDLER	Load/Unload fonts
*
*	parmblk.option	VS_FHNDLER
*	parmblk.flags	See Flag Definition Below
*	parmblk.databuf	0L
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	Long containing the number of addtional font faces. (load fonts)
*		0L - Success (unloading fonts)
*		Error Code
******************************************************************************/

#define VS_FHNDLER	5 		/* Sub Function Number */

	/* Flag Definitions */
#define FH_LOAD		0x0000
#define FH_UNLOAD	0x0001 		/* Load fonts = 0 unload = 1*/

/******************************************************************************
*DEVFONTS	Devivce specific font info 
*
*	parmblk.option	VS_DEV_FONTS
*	parmblk.flags	0
*	parmblk.databuf	VSDEVFONT plus Optional 256 byte mapping array.(send)
*	parmblk.dbufsiz	sizeof(VSDEVFONT + Optional 256 byte array).
*	parmblk.parmbuf	Address of device dependent string. (send)
*			<font name>;<font filename> optional see dev_mapflag 
*	parmblk.pbufsiz	sizeof (parmblk.parmbuf<string>)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_DEV_FONT	(7 + DSEND + PSEND)	/* Sub Function Number */

typedef struct 
{
    WORD	dev_byteorder;		/* Byte order (0x0102) */
    WORD	dev_flags;		/* bit flags, see defs below */
    WORD	dev_face;		/* font face identifier */
    WORD	dev_points;		/* font points size */
    WORD	dev_attrs;		/* font special effects */
    WORD	dev_mapflag;		/* mapping flag see defs below */
} VSDEVFONT;

	/* dev_flags definitions */
#define DFLG_FRST	0x0001;		/* Set to indicate first call */
#define DFLG_LST	0x0002;		/* Set to indicate last call */
#define DFLG_NDWNLD	0x0080;		/* Set to suppress downloading */

	/* dev_mapflag definitions */

#define  DMF_STDMP	0x0000;    /* Standard device driver char set mapping */ 
#define  DMF_NMP	0x0001;    /* Do not perform any character mapping */
#define  DMF_MAP	0xFFFF;	   /* Use mapping table (after VSDEVFONTS) */ 

/******************************************************************************
* COLOR Set Color Index
*
*	parmblk.option	VS_COLOR	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSCOLOR structure (send)
*	parmblk.dbufsiz	sizeof(VSCOLOR)
*	parmblk.parmbuf	Address of VSCOLOR structure (receive)
*	parmblk.pbufsiz	sizeof(VSCOLOR)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_COLOR	(9 + DSEND)	/* Sub Function Number */

typedef	struct _vscolor				/* Data Buffer Structure */
{
    UWORD	clr_byteorder;	/* byte_order				     */
    UWORD	clr_flags;	/* see clr_flags definition below	     */
    UWORD	clr_loindex;	/* color index				     */
    UWORD	clr_hiindex;	/* color index				     */
    UWORD	clr_red;	/* actual red (requested red on send)	     */
    UWORD	clr_green;	/* actual green (requested green on send)    */
    UWORD	clr_blue;	/* actual blue (requested blue on send)	     */
}VSCOLOR;

	/* clr_flags field definitions	*/

#define CLRF_SET	0x0001	/* 1 = set color index			     */

/******************************************************************************
* CSFONT	Get Character Resolution in System Font Sizes.
*
*	parmblk.option	VS_CSFONT	
*	parmblk.flags	0
*	parmblk.databuf	(LONG)BYTEORDER
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	Address of array of VSPCSFONT structures (recieve)
*	parmblk.pbufsiz	(# VSPCSFONTs expected) * sizeof(VSPCSFONT)
*
*	Return:	0L - Success
*		Error Code
*
* NOTE: Returns array of VSPCSFONT structure providing the Character mode 
*	resolutions supported by the driver.
******************************************************************************/

#define VS_CSFONT	10	/* Sub Function Number */

typedef	struct _vspcsfont	/* Parm Buffer Structure */
{
    WORD	pcf_fontnum;	/* Font Number				     */
    RECTSIZE	pcf_physsize;	/* Character Dimensions if on physical device*/
}VSPCSFONT;

/******************************************************************************
* RES	Get Graphic Resolutions Supported by driver.
*
*	parmblk.option	VS_RES	
*	parmblk.flags	0
*	parmblk.databuf	(LONG)BYTEORDER
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	Address of Array of VSPRES structures (receive)
*	parmblk.pbufsiz	(# of VSPRESs expected) * sizeof(VSPRES)
*
*	Return:	0L - Success
*		Error Code
*
* NOTE: Returns array of VSPRES structure to provide information on different
* 	Graphics mode resolutions supported by driver.
******************************************************************************/

#define VS_RES		11	/* Sub Function Number */

typedef	struct _vspres
{
    WORD	prs_resnum;	/* Resolution Number			     */
    RECTSIZE	prs_physsize;	/* Pixels dimensions if on physical device   */
    WORD	prs_colors;	/* Number of Colors supported		     */
}VSPRES;

/******************************************************************************
* REDRAW Wait for Redraw
*
*	parmblk.option	VS_REDRAW	
*	parmblk.flags	0
*	parmblk.databuf	(LONG) BYTEORDER
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	Address of RECTANGLE structure (receive)
*	parmblk.pbufsiz	sizeof(RECTANGLE)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_REDRAW	16	/* Sub Function Number */

/******************************************************************************
* SOUND	Perform Sound
*
*	parmblk.option	VS_SOUND	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDSOUND structure (send)
*	parmblk.dbufsiz	sizeof(VSDSOUND)
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_SOUND	(17 + DSEND)	/* Sub Function Number */

typedef	struct _vsdsound	/* Data Buffer Structure */
{
    UWORD	snd_byteorder;	/* Byte Order				     */
    UWORD	snd_frequency;
    UWORD	snd_duration;
}VSDSOUND;

/******************************************************************************
* MFORM	Change Mouse Form
*
*	parmblk.option	VS_MFORM	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDMFORM structure (send)
*	parmblk.dbufsiz	sizeof(VSDMFORM)
*	parmblk.parmbuf	Address of mouse form and mask data (send)
*	parmblk.pbufsiz	size of mouse form and mask data
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_MFORM	(18 + DSEND + PSEND)	/* Sub Function Number */

typedef	struct _vsdmform	/* Data Buffer Structure */
{
    UWORD	dmf_byteorder;	/* Byte Order				     */
    UWORD	dmf_mlcolor;	/* mask color				     */
    UWORD	dmf_mhcolor;	/* mask color				     */
    UWORD	dmf_dlcolor;	/* data color				     */
    UWORD	dmf_dhcolor;	/* data color				     */
    UWORD	dmf_pwidth;	/* pattern width			     */
    UWORD	dmf_pheight;	/* pattern height			     */
    POSITION	dmf_hotspot;	/* x and y of hot spot pixel		     */
}VSDMFORM;

typedef	struct _vspmret		/* Parameter Buffer Struct   */
{
    POSITION	pmr_mouse;	/* Current Mouse Position		     */
    UWORD	pmr_bstate;	/* Current Button State			     */
    UWORD	pmr_kstate;	/* Current State (Alt,CTRL,Left/Right shift) */
}VSPMRET;

/******************************************************************************
* BWAIT	Button Wait - Wait for the specified buttons (dbw_mask) to reach the 
*	specified state (dbw_state) within the requested number of click times
*	(dbw_clicks). The current mouse position, button state, and keystate 
*	(Alt Ctrl Left shift and Right shift) are returned in the parm buffer.
*
*	parmblk.option	VS_BWAIT	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDBWAIT structure (send)
*	parmblk.dbufsiz	sizeof(VSDBWAIT)
*	parmblk.parmbuf	Address of VSPMRET structure (receive)
*	parmblk.pbufsiz	sizeof (VSPMRET)
*
*	Return:	Number of Clicks
*		Error Code
******************************************************************************/

#define VS_BWAIT	(19 + DSEND)	/* Sub Function Number */

typedef	struct _vsdbwait	/* Data Buffer Structure */
{
    UWORD	dbw_byteorder;
    UWORD	dbw_clicks;
    UWORD	dbw_mask;
    UWORD	dbw_state;
}VSDBWAIT;

/******************************************************************************
* RWAIT	Rectangle Wait - Wait until the mouse moves into or exits from the
*		specified rectangle.  The current mouse position, button state
*		and keystate (Alt Ctrl Left shift, and Right shift) are returned
*		in the parm buffer.
*
*	parmblk.option	VS_RWAIT	
*	parmblk.flags	See Flags Definition Below
*	parmblk.databuf	Address of VSDRWAIT structure (send)
*	parmblk.dbufsiz	sizeof(VSDRWAIT)
*	parmblk.parmbuf	Address of VSPMRET (receive)
*	parmblk.pbufsiz	sizeof(VSPMRET)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_RWAIT	(20 + DSEND)	/* Sub Function Number */

typedef	struct _vsdrwait	/* Data Buffer Structure */
{
    UWORD	drw_byteorder;	/* Byte Order				     */
    RECTANGLE	drw_rect;	/* Area of Virtual Console to watch	     */
}VSDRWAIT;

	/* Flags Definition */

#define RW_EXIT		0x0001	/* 1 = Return on Exit, 0 = Return on Entry   */
#define RW_CLIP		0x0002	/* 1 = Clip to current window, 0 = no clip   */

/******************************************************************************
* GIVE	Give the Keyboard or Mouse to specified Virtual Console
*
*	parmblk.option	VS_GIVE	
*	parmblk.flags	See Flags Definition Below
*	parmblk.databuf	0L
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_GIVE		21	/* Sub Function Number */

	/* Flags Definitions */

#define	GVE_KB		0x0001	/* 1=give keyboard, 0=don't give keyboard    */
#define GVE_MOUSE	0x0002	/* 1=give mouse, 0=don't give mouse	     */
#define	GVE_PARENT	0x0004	/* 1=give to parent, 0=give to self	     */

/******************************************************************************
* KCTRL	Specify Keyboard Control Character Ranges
*
*	parmblk.option	VS_KCTRL	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDKCTRL structure (send)
*	parmblk.dbufsiz	sizeof(VSDKCTRL)
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_KCTRL	(22 + DSEND)	/* Sub Function Number */

typedef	struct _vsdkctrl	/* Data Buffer Structure */
{
    UWORD	dkc_byteorder;	/* Byte Order				     */
    UWORD	dkc_1start;	/* Start of Range 1 (0xffff = No Range)	     */
    UWORD	dkc_1end;	/* End of Range 1			     */
    UWORD	dkc_2start;	/* Start of Range 2 (0xffff = No Range)	     */
    UWORD	dkc_2end;	/* End of Range 2			     */
    UWORD	dkc_3start;	/* Start of Range 3 (0xffff = No Range)	     */
    UWORD	dkc_3end;	/* End of Range 3			     */
    UWORD	dkc_4start;	/* Start of Range 4 (0xffff = No Range)	     */
    UWORD	dkc_4end;	/* End of Range 4			     */
}VSDKCTRL;

/******************************************************************************
* MCTRL	Set Mouse Control Area
*
*	parmblk.option	VS_MCTRL	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDMCTRL structure (send)
*	parmblk.dbufsiz	sizeof(VSDMCTRL)
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_MCTRL	(23 + DSEND)	/* Sub Function Number */

typedef	struct _vsdmctrl	/* Data Buffer Structure */
{
    UWORD	dmc_byteorder;	/* Byte Order				     */
    RECTANGLE	dmc_rect;	/* Control Area				     */
}VSDMCTRL;

/******************************************************************************
* ORDER	Change the layer order of windows on the screen
*
*	parmblk.option	VS_ORDER	
*	parmblk.flags	order number
*	parmblk.databuf 0L
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_ORDER	24	/* Sub Function Number */

/******************************************************************************
* GKMAP	Obtain Current Keyboard Mapping
*
*	parmblk.option	VS_GKMAP	
*	parmblk.flags	0
*	parmblk.databuf	(LONG)BYTEORDER
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	Address of KBMAP structure (receive)
*	parmblk.pbufsiz	sizeof(KBMAP)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_GKMAP	26		/* Sub Function Number */

typedef	struct _kbmap	/* Parm Buffer Structure */
{
    UWORD	kbm_country;	/* Country Code				     */
    UWORD	kbm_char[GBUFSIZ]; /* Character Values			     */
}KBMAP;

/******************************************************************************
* SKMAP	Set Keyboard Map
*
*	parmblk.option	VS_SKMAP	
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDSKMAP structure (send)
*	parmblk.dbufsiz	sizeof(VSDSKMAP)
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_SKMAP	(27 + DSEND)		/* Sub Function Number */

typedef	struct _vsdskmap	/* Data Buffer Structure */
{
    UWORD	dkbm_byteorder;	/* Byte Order				     */
    KBMAP	dkbm_kbmap;	/* Keyboard Map				     */
}VSDSKMAP;

/******************************************************************************
* XRECT	Specify XOR Rectangle to Place on Screen (independent of layering)
*
*	parmblk.option	VS_XRECT
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDXRECT structure
*	parmblk.dbufsiz	sizeof(VSDXRECT)
*	parmblk.parmbuf	0L
*	parmblk.pbufsiz	0L
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_XRECT	28 + DSEND	/* Sub Function Number */

typedef	struct _vsdxrect	/* Data Buffer Structure */
{
    UWORD	dxr_byteorder;	/* Byte Order				     */
    RECTANGLE	dxr_rect;	/* Line Rectangle to draw		     */
}VSDXRECT;

/******************************************************************************
* KPUT	Put Characters into Virtual Console Keyboard Buffer
*
*	parmblk.option	VS_KPUT	
*	parmblk.flags	0
*	parmblk.databuf	(LONG) BYTEORDER
*	parmblk.dbufsiz	0L
*	parmblk.parmbuf	Address of 16-bit Character buffer (send)
*	parmblk.pbufsiz	(# of characters)		(changed 911022)
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_KPUT		(29 + PSEND)	/* Sub Function Number */

/******************************************************************************
* IMAGE	Process a bit image file
*
*	parmblk.option	VS_IMAGE
*	parmblk.flags	0
*	parmblk.databuf	Address of VSDIMAGE structure
*	parmblk.dbufsiz	sizeof(VSDIMAGE)
*	parmblk.parmbuf	Address of NULL-terminated filename
*	parmblk.pbufsiz	Length of filename with NULL
*
*	Return:	0L - Success
*		Error Code
******************************************************************************/

#define VS_IMAGE	(16 + DSEND + PSEND)	/* Sub Function Number */

typedef	struct _vsdimage	/* Data Buffer Structure */
{
	WORD	dim_byteorder;		/* 0x0102			*/
	WORD	dim_aspect;		/* aspect ratio flag:		*/
					/* 0:ignore,1:honor aspect ratio*/
	WORD	dim_x_scale;		/* scaling for X axis		*/
					/* 0:fractional, 1:integer	*/
	WORD	dim_y_scale;		/* scaling for Y axis		*/
					/* 0:fractional, 1:integer	*/
	WORD	dim_h_align;		/* horizontal alignment:	*/
					/* 0:left, 1:center, 2:right	*/
	WORD	dim_v_align;		/* vertical alignment:		*/
					/* 0:top, 1:middle, 2:bottom	*/
	WORD	dim_rotate;		/* rotation in 1/10 degrees	*/
	WORD	dim_blcolor;		/* low background color		*/
	WORD	dim_bhcolor;		/* high background color	*/
	WORD	dim_flcolor;		/* low foreground color		*/
	WORD	dim_fhcolor;		/* high foreground color	*/
	WORD	dim_ulx;		/* upper left x location	*/
	WORD	dim_uly;		/* upper left y location	*/
	WORD	dim_lrx;		/* lower right x location	*/
	WORD	dim_lry;		/* lower right y location	*/
}VSDIMAGE;

#define BI_ASPECT	0x0001		/* Honor the aspect ratio (0=ignore)*/
#define BI_XSCALE	0x0001		/* Integer (0=fraction) */
#define BI_YSCALE	0x0001		/* Integer (0=fraction) */
#define BI_HALIGN_L	0x0000		/* Horizontal align left */
#define BI_HALIGN_C	0x0001		/* Horizontal align center */
#define BI_HALIGN_R	0x0002		/* Horizontal align right */
#define BI_VALIGN_L	0x0000		/* Vertical align left */
#define BI_VALIGN_C	0x0001		/* Vertical align center */
#define BI_VALIGN_R	0x0002		/* Vertical align right */

/******************************************************************************
* VDI COPY Subfunction Numbers, Flags, Parm and Data Buffer Formats
******************************************************************************/

/******************************************************************************
* CHAR - Copy character rectangle.
*
*	parmblk.option	CPY_CHAR
*	parmblk.flags	See Flags Definition below
*	parmblk.dframe	Address of FRAME describing Destination
*			(FRAME *)0 indicates Virtual Console is Destination
*	parmblk.drect	Address of RECTANGLE in Destination
*	parmblk.sframe	Address of FRAME describing Source
*			(FRAME *)0 indicates Virtual Console is Source
*	parmblk.srect	Address of RECTANGLE in Source
*
*	Return:	0L - Success
*		Error Code
*
******************************************************************************/

#define CPY_CHAR	0

typedef	struct _frame
{
    BYTE	far *fr_char;	/* Address of Character Plane		     */
				/* If Alt. Char Op mode, this is a UWORD ptr.*/
				/*   Char and Attr in each word (ala IBM)    */
    BYTE	far *fr_attr;	/* Address of Attribute Plane		     */
				/* If Alt. Char Mode, this is not used	     */
    BYTE	far *fr_ext;	/* Address of Attribute Plane		     */
    RECTSIZE	fr_size;	/* Dimensions of Frame			     */
    UWORD	fr_use;		/* Usage Flags				     */
}FRAME;

	/* fr_use bit Definitions */

#define FRU_CHAR	1	/* 1=fr_char is Array, 0=fr_char is 1 char   */
#define FRU_ATTR	2	/* 1=fr_attr is Array, 0=fr_attr is 1 char   */
#define FRU_EXT		4	/* 1=fr_ext is Array, 0=fr_ext is 1 char     */

	/* Flags Definitions */

#define CCF_CHAR	1	/* 1=Copy Char Plane, 0=Ignore Char Plane    */
#define CCF_ATTR	2	/* 1=Copy Attr Plane, 0=Ignore Attr Plane    */
#define CCF_EXT		4	/* 1=Copy Ext Plane, 0=Ignore Ext Plane	     */

typedef	struct _mfdb
{
    WORD *m_raster;
    WORD  m_frmwidth;
    WORD  m_frmheight;
    WORD  m_width;
    WORD  m_format;
    WORD  m_planes;
    WORD  m_dummy[3];
}MFDB;

/******************************************************************************
* OPAQUE - Copy Raster Opaque.
*
*	parmblk.option	CPY_OPAQUE
*	parmblk.flags	Writing Mode
*	parmblk.dframe	Address of MFDB describing Destination
*	parmblk.drect	Address of RECTANGLE in Destination
*	parmblk.sframe	Address of MFDB describing Source
*	parmblk.srect	Address of RECTANGLE in Source
*
*	Return:	0L - Success
*		Error Code
*
******************************************************************************/

#define CPY_OPAQUE	1

/******************************************************************************
* XFORM - Transform Form.
*
*	parmblk.option	CPY_XFORM
*	parmblk.flags	0
*	parmblk.dframe	Address of MFDB describing Destination
*	parmblk.drect	0L
*	parmblk.sframe	Address of MFDB describing Source
*	parmblk.srect	0L
*
*	Return:	0L - Success
*		Error Code
*
******************************************************************************/

#define CPY_XFORM	2

/******************************************************************************
* XPARENT - Copy Pixels Transparent.
*
*	parmblk.option	CPY_XPARENT
*	parmblk.flags	Writing Mode
*	parmblk.dframe	Address of MFDB describing Destination
*	parmblk.drect	Address of CXDRECT in Destination
*	parmblk.sframe	Address of MFDB describing Source
*	parmblk.srect	Address of RECTANGLE in Source
*
*	Return:	0L - Success
*		Error Code
*
******************************************************************************/

#define CPY_XPARENT	3

typedef	struct _cxdrect
{
    RECTANGLE	cxd_drect;	/* Destination Rectangle		     */
    UWORD	cxd_flcolor;	/* Foreground Color Index		     */
    UWORD	cxd_fhcolor;	/* Foreground Color Index		     */
    UWORD	cxd_blcolor;	/* Background Color Index		     */
    UWORD	cxd_bhcolor;	/* Background Color Index		     */
}CXDRECT;

/******************************************************************************
* WINDOW - Copy Pixels Opaque and change Windowing to match.
*
*	parmblk.option	CPY_WINDOW
*	parmblk.flags	writing mode
*	parmblk.dframe	Address of MFDB describing Destination
*	parmblk.drect	Address of CWDRECT in Destination
*	parmblk.sframe	Address of MFDB describing Source
*	parmblk.srect	Address of RECTANGLE in Source
*
*	Return:	0L - Success
*		Error Code
*
******************************************************************************/

#define CPY_WINDOW	4

typedef	struct _cwdrect
{
    RECTANGLE	cwd_drect;	/* Destination Rectangle		     */
    RECTANGLE	cwd_wview;	/* New Window View			     */
    POSITION	cwd_wpos;	/* New Window Position on Parent	     */
}CWDRECT;

/******************************************************************************
* ALTER SVC - Alter a character rectangle.
*
*	parmblk.flags	See Flags Definition below
*	parmblk.dframe	Address of FRAME describing Destination
*			(FRAME *)0 indicates Virtual Console is Destination
*	parmblk.drect	Address of RECTANGLE in Destination
*	parmblk.alterb	char,attr and extension plane AND and XOR bytes
*
*	Return:	0L - Success
*		Error Code
*
******************************************************************************/

	/* Flags Definitions */

#define ALTF_CHAR	1	/* 1=Copy Char Plane, 0=Ignore Char Plane    */
#define ALTF_ATTR	2	/* 1=Copy Attr Plane, 0=Ignore Attr Plane    */
#define ALTF_EXT	4	/* 1=Copy Ext Plane, 0=Ignore Ext Plane	     */

	/* Alterb Definition - Part of PBLK */
typedef	struct _alterb
{
#if GMODE
    UWORD	alb_caand;	/* Char/Attr Plane AND			     */
    UWORD	alb_caxor;	/* Char/Attr Plane XOR			     */
#else
    BYTE	alb_cand;	/* Character Plane AND			     */
    BYTE	alb_cxor;	/* Character Plane XOR			     */
    BYTE	alb_aand;	/* Attribute Plane AND			     */
    BYTE	alb_axor;	/* Attribute Plane XOR			     */
#endif
    BYTE	alb_eand;	/* Extension Plane AND			     */
    BYTE	alb_exor;	/* Extension Plane XOR			     */
}ALTERB;

/******************************************************************************
*	VIRCON Table Data Structure and Offsets
*
*	The GET and SET SVC's specify a buffer, buflen and flags field which
*	contain the initial offset.  The offset specifies which of the following
*	fields is the first field to be included in the S_GETFIELD or S_SETFIELD
*	operation.  The buflen field is the number of bytes
*	of information that is to be included and therefore specifies the
*	number of fields to include following the first one.
*
*	The defines associated with each field indicate their starting offset.
*****************************************************************************/


typedef	struct _vircon
{
/* 0x00 - 0 */

#define VC_KEY		0
    LONG	vc_key;		/* Lookup Key Field			-G   */
#define VC_PID		4
    LONG	vc_pid;		/* Process ID of owner			-G   */
#define VC_CID		8
    WORD	vc_cid;		/* Physical Console ID			-G   */
#define VC_VCNUM	10
    WORD	vc_vcnum;	/* Virtual Console Number		-G   */
#define VC_PARENT	12
    WORD	vc_parent;	/* Parent VC Number			-G   */
#define VC_TYPE		14
    UWORD	vc_type;	/* VC Type		   		-G   */

	/* VC_TYPE bit field definitions */
#define VCT_GRAPH	0x0001	/* 0: Graphics Support			     */
#define VCT_NONUM	0x0002	/* 1: No Numeric Keypad Support		     */
#define VCT_MOUSE	0x0004	/* 2: Pointer Support			     */
#define VCT_COLOR	0x0008	/* 3: Color Support			     */
#define VCT_MEMMAP	0x0010	/* 4: Memory Mapped Video		     */
#define VCT_TABLET	0x0024	/* 5: Tablet Support - Pointer On if Tablet  */
#define VCT_KEYBOARD	0x0040  /* 6: Keyboard Supported 		     */
#define VCT_SOUND	0x0080	/* 7: Sound Supported			     */

/* 0x10 - 16 */

#define VC_WNAME	16
    BYTE	vc_wname[16];	/* Window Name				-GS   */

/* 0x20 - 32 */

#define VC_SIZE		32
    RECTSIZE	vc_size;	/* Virtual Console Size			-GS  */
#define VC_1GFLAGS	36
    UWORD	vc_1gflags;	/* 1st Graphic Flags			-G   */

	/* VC_1GFLAGS bit field definitions */
#define VC1_NOPSIMAGE	0x0001	/* 0: No Precisely Scaled Images	     */
#define VC1_SCOLOR	0x0002	/* 1: Set Color Representation		     */
#define VC1_FILLED	0x0004	/* 2: Filled Area Support		     */
#define VC1_CELLARRAY	0x0008	/* 3: Cell Array Support		     */
#define VC1_LOOKUP	0x0010	/* 4: Color Lookup Support		     */
#define VC1_RSCALE	0x0020	/* 5: Raster Scaling			     */
#define VC1_CONTOUR	0x0040	/* 6: Contour Fill Support		     */
#define VC1_TXTALIGN	0x0080	/* 7: Text Alignment Support		     */
#define VC1_INKING	0x0100	/* 8: Inking 				     */
#define VC1_WSTYLE	0x0200	/* 9: Wide Line Styles			     */
#define VC1_WWMODE	0x0400	/* 10: Wide Line Writing Modes		     */
#define VC1_LRUBBER	0x0800	/* 11: Rubber Band Lines		     */
#define VC1_RRUBBER	0x1000	/* 12: Rubber Band Rectangles		     */
#define VC1_ROTATE	0x2000	/* 13: 90,180,270 Bit Image Rotation	     */
#define VC1_DLFONT	0x4000	/* 14: Downloadable Fonts		     */

#define VC_2GFLAGS	38
    UWORD	vc_2gflags;	/* 2nd Graphic Flags			-G   */

	/* VC_2GFLAGS bit field definitions */

#define VC2_UDFILL	0x0001	/* 0: Var Sized User Defined Fill Patterns   */
#define VC2_BCKSTR	0x4000	/* 14: Backing Store for Graphic Consoles    */
#define VC2_PIXALG	0x8000	/* 15: Pixel Alignment on Virtual Consoles   */

#define VC_UCLIP	40
    RECTANGLE	vc_uclip;	/* User Clip Region			-G  */

/* 0x30 - 48 */

#define VC_PIXSIZE	48
    RECTSIZE	vc_pixsize;	/* Pixel Size in Microns		-G   */
#define VC_NPATT	52
    WORD	vc_npatt;	/* # Fill Patterns			-G   */
#define VC_HATCH	54
    WORD	vc_hatch;	/* # Hatch Patterns			-G   */
#define VC_LTYPES	56
    WORD	vc_ltypes;	/* # Line Types				-G   */
#define VC_LWIDTHS	58
    WORD	vc_lwidths;	/* # Line Widths			-G   */
#define VC_MTYPES	60
    WORD	vc_mtypes;	/* # Marker Types			-G   */
#define VC_MSIZES	62
    WORD	vc_msizes;	/* # Marker Sizes			-G   */

/* 0x40 - 64 */

#define VC_NFCLR	64
    WORD	vc_nfclr;	/* # Foreground Colors			-G   */
#define VC_NBCLR	66
    WORD	vc_nbclr;	/* # Background Colors			-G   */
#define VC_WRMODES	68
    WORD	vc_wrmodes;	/* # Writing Modes			-G   */
#define VC_NPLANES	70
    WORD	vc_nplanes;	/* # Planes				-GS  */
#define VC_DEVMEM	72
    LONG	vc_devmem;	/* Available Device Mem			-G   */
#define VC_PERFORM	76
    WORD	vc_perform;	/* # 16x16 rasters/sec			-G   */
#define VC_XPUNIT	78
    WORD	vc_xpunit;	/* Extended Pixel Units			-G   */

/* 0x50 - 80 */

#define VC_XPSIZE	80
    RECTSIZE	vc_xpsize;	/* Extended Pixel Size			-G   */
#define VC_PIXINCH	84
    RECTSIZE	vc_pixinch;	/* Pixels per inch			-G   */
#define VC_NRES		88
    WORD	vc_nres;	/* # Resolutions			-G   */
#define VC_RESTYPE	90
    WORD	vc_restype;	/* Resolution Type			-GS  */
#define VC_RESSIZE	92
    RECTSIZE	vc_ressize;	/* Screen Resolution			-G   */

/* 0x60 - 96 */

#define VC_FEXT		96
    BYTE	vc_fext[4];	/* Font File Extension			-GS  */
#define VC_NSFONTS	100
    WORD	vc_nsfont;	/* # System Fonts			-G   */
#define VC_SYSFONT	102
    WORD	vc_sysfont;	/* Current System Font			-GS  */
#define VC_CHRES	104
    RECTSIZE	vc_chres;	/* Char Resolution			-G   */
#define VC_CHSIZE	108
    RECTSIZE	vc_chsize;	/* Character Pixel Size			-G   */

/* 0x70 - 112 */

#define VC_FLCOLOR	112
    WORD	vc_flcolor;	/* Foreground Color low word		-GS  */
#define VC_FHCOLOR	114	/* Foreground Color high word		-GS  */
    WORD	vc_fhcolor;
#define VC_BLCOLOR	116
    WORD	vc_blcolor;	/* Background Color low word		-GS  */
#define VC_BHCOLOR	118
    WORD	vc_bhcolor;	/* Background Color high word		-GS  */

#define VC_SMODE	120
    WORD	vc_smode;	/* Screen Operations Mode		-GS  */

	/* VC_SMODE bit field definitions */

#define VCSM_NOESC	0x0001	/* 0: Disable Escape Sequences		     */
#define VCSM_16BIT	0x0002	/* 1: 16-Bit Character Mode		     */
#define VCSM_CRLF	0x0004	/* 2: Convert LF to CRLF		     */
#define VCSM_ALT	0x0008	/* 3: Alternate Character Operations Mode    */
#define VCSM_GRAPH	0x0010	/* 4: Graphics Operation Mode		     */
#define VCSM_GEMCHAR	0x0020	/* 5: GEM Character Set			     */
#define VCSM_BACKGD	0x0040	/* 6: Background Virtual Console	     */
#define VCSM_REDRAW	0x0080	/* 7: Redraws Required			     */

#define VC_EFFECTS	122
    WORD	vc_effects;	/* Text Effects Supported		-G   */

	/* VC_EFFECTS bit field definitions */

#define VCE_THICKEN	0x0001	/* 0: Thicken Text Font			     */
#define VCE_INTENSE	0x0002	/* 1: Light Intensity Text Font		     */
#define VCE_SKEWED	0x0004	/* 2: Skewed Text Font 			     */
#define VCE_ULINE	0x0008	/* 3: Underlined Text Font		     */
#define VCE_OUTLINE	0x0010	/* 4: Outlined Text Font		     */
#define VCE_SHADOW	0x0020	/* 5: Shadowed Text Font		     */

#define VC_1RSRVD	124
    WORD	vc_1rsrvd[2];	/* Reserved				-I   */

/* 0x80 - 128 */

#define VC_BPATT	128
    WORD	vc_bpatt;	/* Background Pattern			-GS  */
#define VC_BHATCH	130
    WORD	vc_bhatch;	/* Background Hatch			-GS  */
#define VC_SNDMODE	132
    WORD	vc_sndmode;	/* Sound Mode				-GS  */

	/* VC_SNDMODE bit field definitions */

#define VCSD_DISABLE	0x0001	/* 0: Tone Generation Disabled		     */

#define VC_MODE		134
    WORD	vc_mode;	/* Window Mode				-GS  */

	/* VC_MODE bit field definitions */

#define	VCWM_FREEZE	0x0001		/* 0: Freeze borders		     */
#define	VCWM_TRACKING	0x0002		/* 1: Cursor tracking		     */
#define	VCWM_EDGE	0x0004		/* 2: Edge cursor when tracking	     */
#define	VCWM_OUTPUT	0x0008		/* 3: Tracking on output	     */
#define	VCWM_INSERT	0x0010		/* 4: Insert character		     */
#define	VCWM_DELETE	0x0020		/* 5: Delete character		     */
#define	VCWM_REVERSE	0x0040		/* 6: Reverse video		     */
#define	VCWM_WRAP	0x0080		/* 7: Character wrap		     */
#define	VCWM_CURSOR	0x0100		/* 8: Cursor off		     */
#define VCWM_BLOCK	0x0200		/* 9: Block Cursor		     */
#define VCWM_UNDERL	0x0400		/* 10: Underline		     */

#define VC_WVIEW	136
    RECTANGLE	vc_wview;	/* Window View				-GS  */

/* 0x90 - 144 */

#define VC_WPOS		144
    POSITION	vc_wpos;	/* Position on parent			-GS  */
#define VC_TOP		148
    WORD	vc_top;		/* Top Border Height			-GS  */
#define VC_BOTTOM	150
    WORD	vc_bottom;	/* Bottom Border Height			-GS  */
#define VC_LEFT		152
    WORD	vc_left;	/* Left Border Width			-GS  */
#define VC_RIGHT	154
    WORD	vc_right;	/* Right Border Width			-GS  */
#define VC_COUNTRY	156
    WORD	vc_country;	/* Country Code				-GS  */
#define VC_NFKEYS	158
    WORD	vc_nfkeys;	/* # Function Keys			-G   */

/* 0xA0 -160 */

#define VC_TRES		160
    RECTSIZE	vc_tres;	/* Tablet Resolution			-GS  */
#define VC_TORIGIN	164
    POSITION	vc_torigin;	/* Tablet Origin			-GS  */
#define VC_TDIM		168
    RECTSIZE	vc_tdim;	/* Tablet Dimensions			-G   */
#define VC_TALIGN	172
    POSITION	vc_talign;	/* Tablet Align Point			-GS  */

/* 0xB0 - 176 */

#define VC_KBMODE	176
    WORD	vc_kbmode;	/* Keyboard Mode			-GS  */

	/* VC_KBMODE bit field definitions */

#define VCKM_NABORT	0x0001	/* 0: Disable CTRL-C			     */
#define VCKM_NSTOP	0x0002	/* 1: Disable CTRL-S, CTRL-Q		     */
#define VCKM_NXLAT	0x0004	/* 2: Disable keybaord translation	     */
#define VCKM_NESC	0x0008	/* 3: Disable Escape Sequence Decode	     */
#define VCKM_16BIT	0x0010	/* 4: 16-bit Character Set		     */
#define VCKM_NECHO	0x0020	/* 5: Disable Echo			     */
#define VCKM_NEOF	0x0040	/* 6: Disable CTRL-Z			     */
#define VCKM_TOGGLE	0x0080	/* 7: Enable Toggle Characters		     */
#define VCKM_CRLF	0x0100	/* 8: Convert CR or LF to CR,LF		     */
#define VCKM_NCR	0x0200	/* 9: Do not echo CR			     */
#define VCKM_NDELIM	0x0400	/* 10: Do not echo CR on any delimiters	     */
#define VCKM_GEMCHAR	0x0800	/* 11: GEM Character Set		     */

#define VC_MFLAGS	178
    WORD	vcb_mflags;	/* Mouse Flags				-G   */

	/* VC_MFLAGS bit field definitions */

#define VCMF_VARSIZE	0x0001	/* 0: Variable Sized Mouse Form		     */

#define VC_CURSOR	180
    POSITION	vc_cursor;	/* KB Cursor Position			-GS  */
#define VC_TAHEAD	184
    WORD	vc_tahead;	/* KB Type Ahead			-G   */
#define VC_KBSTATE	186
    WORD	vc_kbstate;	/* KB State Key Status			-G   */

	/* VC_KBSTATE bit field definitions */

#define VCKS_RSHIFT	0x0001	/* 0: Right Shift Key Down		     */
#define VCKS_LSHIFT	0x0002	/* 1: Left Shift Key Down		     */
#define VCKS_CONTROL	0x0004	/* 2: Control Key Down			     */
#define VCKS_ALT	0x0008	/* 3: ALT Key Down			     */

#define VC_MPOS 	188
    POSITION	vc_mpos;	/* Mouse Position			-GS  */

/* 0xC0 - 192 */

#define VC_BSTATE	192
    WORD	vcb_bstate;	/* Mouse Button State			-G   */
#define VC_MMODE	194
    WORD	vcb_mmode;	/* Mouse Mode				-GS  */

	/* VC_MMODE bit field definitions */

#define VCMM_MOUSE	0x0001	/* 0: Mouse Form Enabled		     */
#define VCMM_XORRECT	0x0002	/* 1: XOR Rectangle Enabled		     */

#define VC_NBUTT	196
    WORD	vcb_nbutt;	/* # Mouse Buttons			-G   */
#define VC_CLICKS	198
    WORD	vcb_clicks;	/* Click Interval			-GS  */
#define VC_CDEV		200
    BYTE	vc_cdev[NAME1];	/* Phys Device Name			-G   */
#define VC_2RSRVD	210
    WORD	vc_2rsrvd[3];	/* Reserved				-I   */

/* 0xd8 - 216 = Total Length */

}VIRCON;	/* End of VIRCON Definition */
#define VIRCSIZE	(LONG)sizeof(VIRCON)

/******************************************************************************
*	VDIPRN Table Data Structure and Offsets
*
*	The GET and SET SVC's specify a buffer, buflen and flags field contain
*       the initial offset.  The offset specifies which of the following
*	fields is the first field to be included in the S_GETFIELD or S_SETFIELD
*	operation.  The buflen field is the number of bytes
*	of information that is to be included and therefore specifies the
*	number of fields to include following the first one.
*
*	The defines associated with each field indicate their starting offset.
*****************************************************************************/


typedef	struct _vdiprn
{
/* 0x00 - 0 */

#define VP_1STATUS	0
    UWORD	vp_1status;	/* 1st Printer Status Word		-G   */
#define VP_2STATUS	2
    UWORD	vp_2status;	/* 2nd Printer Status Word		-G   */
#define VP_GHEIGHT	4
    UWORD	vp_gheight;	/* G Height				-G   */
#define VP_GSLICES	6
    UWORD	vp_gslices;	/* G Slices				-G   */
#define VP_AHEIGHT	8
    UWORD	vp_aheight;	/* A Height				-G   */
#define VP_ASLICES	10
    WORD	vp_vcnum;	/* A Slices				-G   */
#define VP_FACTOR	12
    WORD	vp_factor;	/* Factor				-G   */
#define VP_PSINDEX	14
    UWORD	vp_psindex;	/* Page Size Index	   		-G   */

/* 0x10 - 16 */

#define VP_NAME	16
    BYTE	vp_name[16];	/* Brand Name				-GS   */

/* 0x20 - 32 */

#define VP_SIZE		32
    RECTSIZE	vp_size;	/* Paper Size				-GS  */
#define VP_1GFLAGS	36
    UWORD	vp_1gflags;	/* 1st Graphic Flags			-G   */

	/* VP_1GFLAGS bit field definitions */
#define VP1_NOPSIMAGE	0x0001	/* 0: No Precisely Scaled Images	     */
#define VP1_SCOLOR	0x0002	/* 1: Set Color Representation		     */
#define VP1_FILLED	0x0004	/* 2: Filled Area Support		     */
#define VP1_CELLARRAY	0x0008	/* 3: Cell Array Support		     */
#define VP1_LOOKUP	0x0010	/* 4: Color Lookup Support		     */
#define VP1_RSCALE	0x0020	/* 5: Raster Scaling			     */
#define VP1_CONTOUR	0x0040	/* 6: Contour Fill Support		     */
#define VP1_TXTALIGN	0x0080	/* 7: Text Alignment Support		     */
#define VP1_INKING	0x0100	/* 8: Inking 				     */
#define VP1_WSTYLE	0x0200	/* 9: Wide Line Styles			     */
#define VP1_WWMODE	0x0400	/* 10: Wide Line Writing Modes		     */
#define VP1_LRUBBER	0x0800	/* 11: Rubber Band Lines		     */
#define VP1_RRUBBER	0x1000	/* 12: Rubber Band Rectangles		     */
#define VP1_ROTATE	0x2000	/* 13: 90,180,270 Bit Image Rotation	     */
#define VP1_DLFONT	0x4000	/* 14: Downloadable Fonts		     */
#define VP1_LANDSCAPE	0x8000	/* 15: Supports Landscape		     */

#define VP_2GFLAGS	38
    UWORD	vp_2gflags;	/* 2nd Graphic Flags			-G   */

	/* VP_2GFLAGS bit field definitions */

#define VP2_UDFILL	0x0001	/* 0: Var Sized User Defined Fill Patterns   */

#define VP_UCLIP	40
    RECTANGLE	vp_uclip;	/* User Clip Region			-G   */

/* 0x30 - 48 */

#define VP_PIXSIZE	48
    RECTSIZE	vp_pixsize;	/* Pixel Size in Microns		-G   */
#define VP_NPATT	52
    WORD	vp_npatt;	/* # Fill Patterns			-G   */
#define VP_HATCH	54
    WORD	vp_hatch;	/* # Hatch Patterns			-G   */
#define VP_LTYPES	56
    WORD	vp_ltypes;	/* # Line Types				-G   */
#define VP_LWIDTHS	58
    WORD	vp_lwidths;	/* # Line Widths			-G   */
#define VP_MTYPES	60
    WORD	vp_mtypes;	/* # Marker Types			-G   */
#define VP_MSIZES	62
    WORD	vp_msizes;	/* # Marker Sizes			-G   */

/* 0x40 - 64 */

#define VP_NFCLR	64
    WORD	vp_nfclr;	/* # Foreground Colors			-G   */
#define VP_NBCLR	66
    WORD	vp_nbclr;	/* # Background Colors			-G   */
#define VP_WRMODES	68
    WORD	vp_wrmodes;	/* # Writing Modes			-G   */
#define VP_NPLANES	70
    WORD	vp_nplanes;	/* # Planes				-GS  */
#define VP_DEVMEM	72
    LONG	vp_devmem;	/* Available Device Mem			-G   */
#define VP_PERFORM	76
    WORD	vp_perform;	/* # 16x16 rasters/sec			-G   */
#define VP_XPUNIT	78
    WORD	vp_xpunit;	/* Extended Pixel Units			-G   */

/* 0x50 - 80 */

#define VP_XPSIZE	80
    RECTSIZE	vp_xpsize;	/* Extended Pixel Size			-G   */
#define VP_PIXINCH	84
    RECTSIZE	vp_pixinch;	/* Pixels per inch			-G   */
#define VP_NRES		88
    WORD	vp_nres;	/* # Resolutions			-G   */
#define VP_RESTYPE	90
    WORD	vp_restype;	/* Resolution Type			-GS  */
#define VP_RESSIZE	92
    RECTSIZE	vp_ressize;	/* Printer Resolution			-G   */

/* 0x60 - 96 */

#define VP_FEXT		96
    BYTE	vp_fext[4];	/* Font File Extension			-G   */
#define VP_NSFONTS	100
    WORD	vp_nsfont;	/* # Char System Fonts			-G   */
#define VP_SYSFONT	102
    WORD	vp_sysfont;	/* Current Char System Font		-GS  */
#define VP_CHRES	104
    RECTSIZE	vp_chres;	/* Char Resolution			-G   */
#define VP_CHSIZE	108
    RECTSIZE	vp_chsize;	/* Character Pixel Size			-G   */

/* 0x70 - 112 */

#define VP_FLCOLOR	112
    WORD	vp_flcolor;	/* Foreground Char Color low word	-GS  */
#define VP_FHCOLOR	114
    WORD	vp_fhcolor;	/* Foreground Char Color high word	-GS  */
#define VP_BLCOLOR	116
    WORD	vp_blcolor;	/* Background Char Color low word	-GS  */
#define VP_BHCOLOR	118
    WORD	vp_bhcolor;	/* Background Char Color high word	-GS  */
#define VP_PMODE	120
    WORD	vp_pmode;	/* Printer Operations Mode		-GS  */

	/* VP_PMODE bit field definitions */

#define VPSM_NOESC	0x0001	/* 0: Disable Escape Sequences		     */
#define VPSM_16BIT	0x0002	/* 1: 16-Bit Character Mode		     */
#define VPSM_CRLF	0x0004	/* 2: Convert LF to CRLF		     */
#define VPSM_ALT	0x0008	/* 3: Alternate Character Operations Mode    */
#define VPSM_GRAPH	0x0010	/* 4: Graphics Operation Mode		     */
#define VPSM_GEMCHAR	0x0020	/* 5: GEM Character Set			     */

#define VP_EFFECTS	122
    WORD	vp_effects;	/* Text Effects Supported		-S   */

	/* VP_EFFECTS bit field definitions */

#define VPE_THICKEN	0x0001	/* 0: Thicken Text Font			     */
#define VPE_INTENSE	0x0002	/* 1: Light Intensity Text Font		     */
#define VPE_SKEWED	0x0004	/* 2: Skewed Text Font 			     */
#define VPE_ULINE	0x0008	/* 3: Underlined Text Font		     */
#define VPE_OUTLINE	0x0010	/* 4: Outlined Text Font		     */
#define VPE_SHADOW	0x0020	/* 5: Shadowed Text Font		     */

#define VP_1RSRVD	124
    WORD	vp_1rsrvd[2];	/* Reserved				-I   */

/* 0x80 - 128 */

#define VP_TRAY		128
    WORD	vp_tray;	/* Tray					-GS  */
#define VP_COPIES	130
    WORD	vp_copies;	/* Copies				-GS  */
#define VP_ORIENT	132
    WORD	vp_orient;	/* Orient				-GS  */
#define VP_2RSRVD	134
    WORD	vp_2rsrvd;	/* Reserved				-I   */
/* 900213 RFW --- 891208 KJ addition to VDI_PRN table */
#define VP_OUTPUTFILENAME 136
    BYTE	vp_outputfilename[88];  /* work_in[12] from v_opnwk 	-GS  */

/* 136 + 88 = 224 - 0xE0  Total Length */

}VDIPRN;	/* End of VDI_PRN Definition */


#ifndef __flexif_included
#include "flexif.h"
#endif

/*---------------------------End of FLEXTAB.H------------------------*/
