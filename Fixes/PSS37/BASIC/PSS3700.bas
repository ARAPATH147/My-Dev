\******************************************************************************
\******************************************************************************
\***
\***   $Workfile:   PSS3700.bas  $
\***
\***   $Revision:   1.23  $
\***
\******************************************************************************
\******************************************************************************
\***
\***   $Log:   V:/Archive/Basarch/PSS3700.bav  $
\***   
\***      Rev 1.23   12 Jan 2009 12:06:42   stuart.highley
\***   Changes for +UODs (A9B)
\***   
\***      Rev 1.22   06 Oct 2008 09:40:32   Peter.Sserunkuma
\***   A9A. Relinked to include changes made to
\***   the PLLDB functions
\***   
\***      Rev 1.21   02 Jul 2007 12:44:44   greenfield.brian
\***   Changes for A7C Recalls.
\***   
\***      Rev 1.20   16 Feb 2007 10:28:48   paul.bowers
\***   NB A7B defect resolution 
\***   for ASN processing
\***
\***      Rev 1.18   18 Jul 2006 12:08:24   charles.skadorwa
\***   PDT Connection Checker. PSS38 can now
\***   instruct PSS37 to end if it detects that a
\***   PDT is not connected after 5 minutes. It
\***   sends the status "@@".
\***
\***      Rev 1.17   01 Mar 2005 11:33:48   dev07ps
\***   Upwards TSF Fix.
\***   Fix to Report & Session numbers.
\***   Relinked with latest PLLOL object.
\***   Removal of LSS specific code.
\***   Fix to Stock Counting - moved to new program PDTSC.286 to solve PDT
\***   timeout when > 40 counts.
\***
\***
\***      Rev 1.16   12 Jan 2005 12:20:42   dev88ps
\***   Corrected so that old and new price checks
\***   work in this release. Changes could be
\***   removed in the future to get rid of the old
\***   price check code. For A5A
\***
\***      Rev 1.15   24 Dec 2004 13:04:00   dev07ps
\***   A5A Shelf Monitor Project
\***
\***
\***      Rev 1.14   20 Apr 2004 13:50:44   devjsps
\***   Changes to Goods Out functionality
\***   as part of A4D package upgrade
\***
\***      Rev 1.13   16 Dec 2003 09:03:34   devjsps
\***   Program updated to add Pharmacy Smartscript
\***   Stock count changes and Credit Claim changes.
\***   David Artiss has also added Stock takings changes
\***   to PSB3704.BAS
\***
\***      Rev 1.12   30 Jul 2003 14:27:48   dev88ps
\***   Changes made to accomodate new Stock
\***   Count facility on the PDT. New module
\***   PSS3705 created.
\***
\***      Rev 1.11   09 Jul 2003 11:18:18   dev38ps
\***   Reduced file allocations
\***
\***      Rev 1.11   09 Jul 2003 11:04:20   dev38ps
\***   Ensure LOCCNT is opened whether it's a normal stocktake or BOL one.
\***
\***      Rev 1.9   29 Oct 2002 09:09:24   dev88ps
\***   Changes to accomodate live stock counting
\***   in the LSS only (store 2999.) Changes are
\***   not visible in any other store.
\***
\***      Rev 1.8   12 Oct 2001 12:03:42   DEV38PS
\***   Added LDT audit file processing
\***
\***      Rev 1.7   05 Feb 1998 11:57:42   DEV45PS
\***
\***
\***      Rev 1.6   25 Sep 1997 13:22:02   DEV45PS
\***   Increased processing speed of data from stocktake PDT. Also
\***   now sends acknowledgement back to PDT after transmission.
\***
\***      Rev 1.5   10 Sep 1997 11:44:20   DEV45PS
\***   Changes For Stocktake System - Processes transmissions from
\***   stocktake PDT's
\***
\***      Rev 1.4   21 Aug 1996 14:25:44   DEVDSPS
\***   ECC Enhancements
\***
\***      Rev 1.3   21 May 1996 09:58:20   DEV20PS
\***   Gap Monitor Changes
\***
\***      Rev 1.2   28 Jul 1995 13:22:24   DEVDSPS
\***   CSR PHASE 2 - disallow PDT linking when PHASE 2.
\***
\***      Rev 1.1   07 Oct 1994 15:51:14   DEVSPPS
\***   Fixed PDT Support hang
\***
\***   REVISION 1.2  David Smallwood    16th February 1995
\***
\***   Disallow linking to CSR application if CSR conversion has either
\***   started or finished. Also bypass processing of CSR item file at
\***   startup if conversion completed.
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***           PROGRAM  :  PSS3700                                          ***
\***                                                                        ***
\***           AUTHOR   :  S.Wright  (Pseudocode / BASIC code)              ***
\***                                                                        ***
\***           DATE     :  23rd May 1989                                    ***
\***                                                                        ***
\***                                                                        ***
\***           Current version letter : O                                   ***
\***                                                                        ***
\***           Date of last amendment : 17th May 1994                       ***
\***                                                                        ***
\*** NOTE: IN PARAGRAPH CREATE.AUDIT.FILE REMEMBER TO CHANGE PROGRAM        ***
\*** VERSION NUMBER AND DATE OF CREATION AS THIS IS WRITTEN TO THE AUDIT    ***
\*** FILE. THIS ALSO REQUIRES CHANGING IN MODULE 1 IN CREATE.AUDIT.FILE     ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   O V E R V I E W                                                      ***
\***                                                                        ***
\***   PSS37 - L.D.T. / P.D.T. Support Program.                             ***
\***                                                                        ***
\***   PSS37 is designed to run concurrently with PSS38. PSS38 handles      ***
\***   all asyncronous communications with a connected LDT/PDT. All data    ***
\***   sent by the LDT/PDT is passed to PSS37 via PSS38 by means of a       ***
\***   'pipe'.                                                              ***
\***                                                                        ***
\***   PSS37 validates the data sent to ensure the data has been sent in    ***
\***   the correct sequence, has a valid format and is meaningful.          ***
\***   There are basically two processes PSS37 performs to either an LDT    ***
\***   or PDT;                                                              ***
\***                                                                        ***
\***   i)  Reception of data from connected device to provide stock         ***
\***        updating information usually onto the STKMQ.                    ***
\***   ii) Transfer of data to the connected device to provide a basis for  ***
\***        the user to capture stock information.                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   A M E N D M E N T S                                                  ***
\***                                                                        ***
\***   Version B.   S.Wright.                            21st August 1989   ***
\***   Compare only the leftmost six characters of the IRF and IDF Boots    ***
\***   codes.                                                               ***
\***                                                                        ***
\***   Version C.   S.Wright.                          6th September 1989   ***
\***   Alter program so text strings sent to the PDT can be converted, to   ***
\***   allow for the differences in the PDT character set and the 4680      ***
\***   character set. Alter program to allow for a log on time less than    ***
\***   the current time.                                                    ***
\***                                                                        ***
\***   Version D.   Janet Lawrence                     11th July 1990       ***
\***   EPOS-CSR link II.                                                    ***
\***   The PDT now runs three applications - EPSOM, CSR and PRICE CHECK.    ***
\***   Both PSS37 and PSS38 are being altered to cope with the two extra    ***
\***   applications, also to monitor two ports.  Consequently a parameter   ***
\***   will be passed to the program to indicate the port being monitored.  ***
\***   NB.  Because the original program referred to "lists" this has been  ***
\***   left to mean EPSOM lists; CSR lists will be "CSR lists".             ***
\***                                                                        ***
\***   Version D.   Paul Bowers                        1st July 1991        ***
\***   EPOS-CSR Link II.                                                    ***
\***   Comments:                                                            ***
\***   To include code to perform an auto shut down after 10pm daily to     ***
\***   prevent files being left open during the night causing access        ***
\***   conflicts in the batch suites.                                       ***
\***                                                                        ***
\***   Version E.   Paul Bowers                        2nd October 1991     ***
\***   EPOS-CSR Link II.                                                    ***
\***   To include processing for the new CSRAF CSR Audit File               ***
\***                                                                        ***
\***   Version F.  SSJ/P Bowers/S Wright               30th October 1991    ***
\***   Performance improvements to idle state, reduce processor usage from  ***
\***   11% to 0% !!! Yes really.                                            ***
\***   Correct bug in b/g message display.                                  ***
\***   Remove Ev. 66s.                                                      ***
\***                                                                        ***
\***   Version G.  Paul Bowers                          8th January 1992    ***
\***   CSR Change to remove update of WEEK 4 SALES                          ***
\***                                                                        ***
\***   Version H.  David Smallwood                     17th March 1992      ***
\***   A new application for Directs Receiving is being introduced.  This   ***
\***   application uses a new piece of hardware, the Laser Data Terminal    ***
\***   (LDT).  When orders are requested from the LDT PSS37 will process    ***
\***   new data files to format the order information for transmitting back ***
\***   to the LDT.  The new data files are the Direct Supplier File         ***
\***   (DIRSUP) and the Direct Orders File (DIRORD).  When receipts are     ***
\***   passed back to the controller PSS37 will write receipts information  ***
\***   to the Stock Movement Queue for processing by Stock Support.         ***
\***                                                                        ***
\***   Version I.  Les Cook                            30th October 1992    ***
\***   Streamlined code as per new Programming guidelines.                  ***
\***   Checks made to ensure printer is switched                            ***
\***   on and online before CHKBF.BIN is created during PRICE CHECK appl.   ***
\***   A relevant message is sent to the PDT if it is not.                  ***
\***   At end of each application link, PSS37 & PSS38 chain onto themselves ***
\***   to solve memory problems.                                            ***
\***   Fix to problem 761.                                                  ***
\***                                                                        ***
\***   Version J.  Les Cook                            23rd November 1992   ***
\***   LDTBF file now created by PSS37 for use by PSS78 (LDT Reporting      ***
\***   Program). Event 6 messages not logged now for read errors on the     ***
\***   CITEM or IDF.                                                        ***
\***   Fix to problem B??? - if wrong store on LDT it now says "LDT" rather ***
\***   than "PDT".                                                          ***
\***   Also, all checks to the printer and SEL from version I are deleted.  ***
\***   CSR lines linked for the first time which have an allocation record  ***
\***   on the Initial Display Stock Outstanding File (IDSOF) will have sales***
\***   figures on the CIMF adjusted accordingly and the matching IDSOF      ***
\***   record deleted.                                                      ***
\***                                                                        ***
\***   Version K.  Les Cook / Steven Goulding          13th January 1993    ***
\***   Interim version to fix problem where STKMQ is being corrupted.       ***
\***   STKMQ is now opened with BUFFSIZE 512 and READONLY. The STKMQ is now ***
\***   to opened, written & closed at list level for type 12 & 13 txns      ***
\***                                                                        ***
\***   Version L.  Les Cook                                25th March 1993  ***
\***   Extra data states created to accomodate UOD Receiving using and LDT. ***
\***   These states are a - f (NOTE!!!! lower case letters)                 ***
\***   When a successful transmission of UOD data has occurred, type        ***
\***   21 and 23 records are written to the STKMQ and UODBF, and PSS45 is   ***
\***   kicked off as a background task to produce the Missing UOD Report    ***
\***   and the Picking Check Report (if necessary).                         ***
\***                                                                        ***
\***   Version L (Supplemental).  Steven Goulding           24th June 1993  ***
\***   Changes to problems identified with reporting two byte file reporting***
\***   numbers and addition of UOD report number to access conflict within  ***
\***   ERROR.DETECTED routine.                                              ***
\***                                                                        ***
\***   Version L (Supplemental).  Michael J. Kelsall        14th July 1993  ***
\***   Changes to validate CSR header record transmission before data is    ***
\***   written to the CSRBF to correct problem within system.               ***
\***                                                                        ***
\***   Version M Michael J. Kelsall                        30th Sept 1993   ***
\***   Addition of code to handle the LDT transmissions concerned with the  ***
\***   Returns/Automatic Credit Claiming System. The main functionality     ***
\***   will be present in an additional module (four).                      ***
\***   Inclusion of change to fix to BOOTS problem 1060.                    ***
\***                                                                        ***
\***   Version M (Supplemental).  David Smallwood         7th January 1994  ***
\***   If failure to open CCUOD/read CCUOD header record then log error     ***
\***   and goto program exit.  Also fix CCDMY error reporting.  Now report  ***
\***   2 byte file number.                                                  ***
\***                                                                        ***
\***   Version N  Michael J. Kelsall                      23rd March 1994   ***
\***   RETURNS/AUTOMATIC CREDIT CLAIMING SYSTEM MODIFICATIONS.              ***
\***   Various fixes and changes to LDT/PSS37 interface for Returns;        ***
\***   - Change to Returns data state validation to allow a UOD trailer to  ***
\***     follow a UOD header with no item level records present             ***
\***   - Change to LDTAF duration to seconds as opposed to minutes.         ***
\***   - Four digit quantity being supplied from LDT as opposed to three.   ***
\***   - Change to CSRAF to add port letter to every output line except '-' ***
\***                                                                        ***
\***   Version O  Steven Goulding                         2nd June 1994     ***
\***   Move write to audit files after PSS37 has acknowledged a logon to    ***
\***   PDT via PSS38 - to reduce double logon failures                      ***
\***                                                                        ***
\***   Version O (Supplemental)   Michael J. Kelsall     15th June 1994     ***
\***   Change to Data state S valid data state options to prevent PSS37     ***
\***   logging abort at end of LDT program load (even though load is        ***
\***   successful).                                                         ***
\***                                                                        ***
\***                                                                        ***
\***   REVISION 1.1      ROBERT COWEY / STEVE WRIGHT      6TH OCTOBER 1994  ***
\***   Removed version letters from included code.                          ***
\***                                                                        ***
\***   REVISION 1.2      David Smallwood                 17th November 1994 ***
\***   Do not allow CSR link if PHASE2 CSR has been activated. Also bypass  ***
\***   processing of CSR item file at startup if conversion activated.      ***
\***                                                                        ***
\***   REVISION 1.3      Stuart Highley                   10th April 1996   ***
\***   Redirect gapped items (distinguished from price checked items due to ***
\***   the price of the item being entered as 1p at the PDT) to a new file, ***
\***   the Gapped Item Buffer File. Also, start the Gap Report application  ***
\***   (PSS47).                                                             ***
\***                                                                        ***
\***   REVISION 1.4      David Smallwood                  21st May 1996     ***
\***   Include file processing for CCLAM.  Module 4 is being changed to     ***
\***   transmit despatched UODs from CCLAM to the LDT.  This change is part ***
\***   of ECC enhancements and will help prevent duplicate claims returning ***
\***   to the centre.                                                       ***
\***                                                                        ***
\***   REVISION 1.5      Nik Sen                          30th June 1997    ***
\***   Added new data states p,q,r and new files SXTCF, SXTMP, STKBF, STKTK ***
\***   to support stocktaking PDT's.                                        ***
\***                                                                        ***
\***   REVISION 1.6            Nik Sen                 10th December 1997   ***
\***   Changed length of file received from PDT to include an extra digit   ***
\***   for the location.                                                    ***
\***                                                                        ***
\***   REVISION 1.7      David Artiss                   29th January 2001   ***
\***   Added new file STLDT.                                                ***
\***                                                                        ***
\***   REVISION 1.8      Brian Greenfield               17th October 2002   ***
\***   Added LSSST and IMSTC for LSS stock counts.                          ***
\***                                                                        ***
\***   REVISION 1.9      David Artiss                      7th March 2003   ***
\***   Added new processing to allow stocktake PDTs with other stores       ***
\***   data to be transmitted and sent straight to the appropriate          ***
\***   mainframe file.                                                      ***
\***                                                                        ***
\***   REVISION 2.0      David Artiss                       27th May 2003   ***
\***   During the testing of version 1.9 of this code there were problems   ***
\***   with the fact that the code kept running out of session numbers to   ***
\***   allocate. This I got around by allocating certain files only when    ***
\***   entering the appropriate module and then deallocating on exit.       ***
\***   However, this problem has persisted in the live environment. To      ***
\***   this end I have improved this by allocating/deallocating more files  ***
\***   as well as the fact that I've commented out reference to STKCF       ***
\***   which doesn't seem to be used any more.                              ***
\***                                                                        ***
\***   *** !!!WARNING!!!  ***                                               ***
\***                                                                        ***
\***   There are still potential problems due to the large number of        ***
\***   files that this program uses.                                        ***
\***   Module 1 contains a lot of code for the old CSR transmissions        ***
\***   system which is no longer used. In fact now that the PDT6100 has     ***
\***   rolled out the function is not even accessable anymore. This code    ***
\***   should be removed and this will also free up a lot of extra files.   ***
\***                                                                        ***
\***   REVISION 2.1      Brian Greenfield                  17th July 2003   ***
\***   Changes made to add three new data states s, t, AND u to accomodate  ***
\***   new store stock counts. A new module of PSS3705 has been created.    ***
\***                                                                        ***
\***   REVISION 2.2      Julia Stones                  21st October 2003    ***
\***   Changes made to add four new data states w, x, y AND z to accomodate ***
\***   new Pharmacy stock counts. The main bulk of the code to process the  ***
\***   data in the four new data states has been added to module PSS3705.   ***
\***                                                                        ***
\***   REVISION 2.3     Julia Stones                   13th November 2003   ***
\***   Changes made to change existing returns data state g to accomodate   ***
\***   new Other Credit Claiming.  The main bulk of the code to process the ***
\***   data for Other Credit Claiming has been added to module PSS3704      ***
\***                                                                        ***
\***   REVISION 2.4    Julia Stones                   12th March 2004       ***
\***   Other Credit Claim is being changed so that items not on file are    ***
\***   allowed as part of the claim (items not on file were being rejected  ***
\***   from the claim and added to the rejection file).  Also instead of    ***
\***   PSS37 creating the rejection report, the information is now written  ***
\***   out to a new credit claim rejection file CCREJ.  If any items have   ***
\***   been rejected the PSS25 is started via ADXSTART                      ***
\***   Goods out has been changed to be in line with Other Credit claiming  ***
\***   Items that are for invalid Business Centres are rejected from the    ***
\***   claims and added to the rejection file                               ***
\***   Items where the item qty x the item value is > £9,999.99 will have   ***
\***   the record split into individual items.                              ***
\***   If a new flag on SOFTS record 43 shows that all rejections are req   ***
\***   then if the item business centre does not match the claim business   ***
\***   centre this item will be removed from the claim and a record added   ***
\***   to the rejection file CCREJ                                          ***
\***   For both Other Credit Claiming and goods out - the rejecting of items***
\***   not on file has been commented out (in case this becomes a later     ***
\***   requirement as part of all items in all stores)                      ***
\***                                                                        ***
\***   REVISION 2.5    Charles Skadorwa             25th October 2004       ***
\***   Shelf Monitor Project changes.                                       ***
\***   Record size for Price Check item increased as PDT now transmits      ***
\***   Shelf and Fill quantity data.                                        ***
\***   IMPORTANT: Update CREATE.AUDIT.FILE                                  ***
\***                                                                        ***
\***   REVISION 2.6    Brian Greenfield             11th January 2005       ***
\***   Fixes to allow old record type P and new record type P to both work. ***
\***   The old record type P can be removed next time this module is        ***
\***   altered.                                                             ***
\***                                                                        ***
\***   REVISION 2.7    Charles Skadorwa             4th February 2005       ***
\***   Fix file reporting issue. When opening files you pass the file       ***
\***   reporting number to SB.FILE.UTILS function. When closing, you pass   ***
\***   the session number. This has not been adhered to which results in    ***
\***   the wrong file being reported. The SB.FILE.UTILS should also be      ***
\***   called to remove an entry from the session number table, otherwise   ***
\***   it fills up and causes PSS37 to become unstable.                     ***
\***                                                                        ***
\***   Removed LSS 2999 processing as redundant now.                        ***
\***                                                                        ***
\***   SB.FILE.UTILS now called when closing the TSF. The Session Number    ***
\***   Table was getting filled up because a new entry was added each time  ***
\***   the TSF was re-opened.                                               ***
\***                                                                        ***
\***   REVISION 2.8    Charles Skadorwa             3rd July 2006           ***
\***   This program will now shutdown if an "@@" request is received from   ***
\***   PSS38. This will prevent PSS37 taking up any resources if a PDT is   ***
\***   not connected.                                                       ***
\***                                                                        ***
\***   REVISION 2.9    Neil Bennett                22nd December 2006       ***
\***   Add ASN processing support.                                          ***
\***   Remove CSR processing support.                                       ***
\***                                                                        ***
\***   REVISION 2.10   Brian Greenfield            11th May 2007            ***
\***   Added Recalls functionality for A7C.                                 ***
\***   Added new states 1, 2, 3, 4, 5, & 6.                                 ***
\***                                                                        ***
\***   REVISION 2.11   Peter Sserunkuma            25th Sep 2008            ***
\***   Relinked to include changes made to the PLLDB function as part of    ***
\***   the work to allow the counting of Multi-sited items.                 ***
\***                                                                        ***
\***   REVISION 2.12   Stuart Highley              13th Nov 2008            ***
\***   Positive UOD receiving.                                              ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   P R O G R A M   S T R U C T U R E                                    ***
\***                                                                        ***
\*** - Global definitions                                                   ***
\*** - Variable declarations                                                ***
\*** - External function declarations                                       ***
\*** - Internal function declarations (all prefixed 'FN.')                  ***
\*** - Mainline code                                                        ***
\*** - Mainline code subroutines                                            ***
\*** - Low level subroutines (all passed vars and routines prefixed 'SB.')  ***
\*** - Error handling (IF END processing and all general error trapping)    ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   G L O B A L   D E F I N I T I O N S                                  ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

%INCLUDE PSBF06G.J86                                                    ! ILC

%INCLUDE ASYNCNUB.J86                                                   ! ILC
%INCLUDE EPSOMDEC.J86                                                   ! ILC
%INCLUDE PDTWFDEC.J86                                                   ! ILC
%INCLUDE PIPEONUB.J86                                                   ! ILC
%INCLUDE PLDTNUMA.J86                                                   ! ILC
%INCLUDE SITEMDEC.J86                                                   ! ILC
%INCLUDE CBDEC.J86                                                      !2.9NWB

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   V A R I A B L E   D E C L A R A T I O N S                            ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

INTEGER*1                                                               \
        CURR.SESS.NUM%,                                                 \
        MATCH.DELIMITER1,                                               \ 2.4JAS
        MATCH.DELIMITER2,                                               \ 2.4JAS
        MATCH.DELIMITER3,                                               \ 2.4JAS
        RES.POS%,                                                       \
        SB.EVENT.NO%,                                                   \
        SB.FILE.SESS.NUM%,                                              \
        STORE.NUMBER.INCORRECT,                                         \ 1.9DA
        RETURNS.LOGON.VALID                                             ! MMJK

INTEGER*1 GLOBAL                                                        \
        STOCKTAKING.ALTERNATIVE.STORE                                   ! 1.9DA

INTEGER*2                                                               \
        ADX.FUNCTION%,                                                  \ ILC
        ADX.PARM.1%,                                                    \ ILC
        ASN.RCD.CNT%,                                                   \ 2.9NWB
        CB.MANUAL.COUNT%,                                               \ 2.9NWB
        CB.RECORD.COUNT%,                                               \ 2.9NWB
        DATA.LENGTH%,                                                   \
        INACTIVITY.SHUTDOWN%,                                           \ DSW
        INDX%,                                                          \ HDS
        LOG.ON.DISABLE%,                                                \ ILC
        MAN.REC.CNT%,                                                   \ 2.9NWB
        MESSAGE.NO%,                                                    \
        MESSAGE.NUMBER%,                                                \ ILC
        NUMBER.OF.RECORDS%,                                             \ NMJK
        POSITION%,                                                      \
        REP%,                                                           \ DSW
        SB.FILE.REP.NUM%,                                               \
        SB.INTEGER%,                                                    \
        SECTOR.COUNT%,                                                  \
        TEMP.EVENT%                                                     ! CSW

INTEGER*4                                                               \
        ADX.RET.CODE%,                                                  \
\       CSRWF.EXISTS%,                                                  \ DJAL!2.9NWB
        I%,                                                             \
        NOW%,                                                           \ DSW
        START.TIME%                                                     ! NMJK



REAL                                                                    \
        TIME.DIFFERENCE

STRING                                                                  \
        ADX.PARM.2$,                                                    \ ILC
        APPL.STATUS$,                                                   \
        CB.FN$,                                                         \ 2.9NWB
        CB.TN$,                                                         \ 2.9NWB
        CB.OPEN.FLAG$,                                                  \ 2.9NWB
\       CITEM.RECORD$,                                                  \ DJAL!2.9NWB
\       CITEM.SECTOR.ALTERED$,                                          \ DJAL!2.9NWB
        CR$,                                                            \
        CRLF$,                                                          \
        CURRENT.KEY$,                                                   \
        DEVICE$,                                                        \ LLC
\       END.OF.CITEM$,                                                  \ DJAL!2.9NWB
        ENQ$,                                                           \
        FILLER$,                                                        \ DJAL
        INVOK.OPEN.FLAG$,                                               \ DJAL
        LAST.SHUTDOWN.DATE$,                                            \ DPAB
        LF$,                                                            \
        LOCATION$,                                                      \ DSW
        LOG.STATE$,                                                     \
        NAK$,                                                           \
        NAK.LINE.1$,                                                    \ DSW
        NAK.LINE.2$,                                                    \ DSW
        NAK.LINE.3$,                                                    \ DSW
        PLDT.RECORD$,                                                   \ HDS
        PIPE.IN$,                                                       \
        QUIT.FLAG$,                                                     \
        REJECTION.TYPE$,                                                \ 2.4JAS
        SB.ACTION$,                                                     \
        SB.ERRF$,                                                       \
        SB.ERRL$,                                                       \
        SB.ERRS$,                                                       \
        SB.MESSAGE$,                                                    \
        SB.STRING$,                                                     \
        SB.UNIQUE$,                                                     \
        STKTK.OPEN.FLAG$,                                               \ 1.5
        STORE.CLOSE.FLAG$,                                              \ 2.3JAS
        SUCCESS.FLAG$,                                                  \ DJAL
        TEMP.STATE$,                                                    \
        TIDY.FLAG$,                                                     \
        UNIQUE.2$                                                       !

STRING                                                                  \
        APPL.TABLE$(1),                                                 \ HDS
        LOGON.TAB$(1)                                                   ! DJAL


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   C O M M O N   D A T A   I N C L U D E                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

%INCLUDE PSS37G.J86                                                    ! NMJK

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   S U B R O U T I N E   D E C L A R A T I O N                          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

SUB PSS3701 EXTERNAL
END SUB

SUB PSS3702 EXTERNAL                                                    ! HDS
END SUB                                                                 ! HDS

SUB PSS3703 EXTERNAL                                                    ! LLC
END SUB                                                                 ! LLC

SUB PSS3704 EXTERNAL                                                    ! MMJK
END SUB                                                                 ! MMJK

SUB PSS3705 EXTERNAL                                                    ! 2.1BG
END SUB                                                                 ! 2.1BG

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   E X T E R N A L   F U N C T I O N   D E C L A R A T I O N S          ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

%INCLUDE PSBF06E.J86                                                    ! ILC
%INCLUDE CHKBFEXT.J86                                                   ! ILC
%INCLUDE GAPBFEXT.J86                                                  ! 1.3 !2.5CS !2.6BG
%INCLUDE PLLOLEXT.J86                                                   !2.5CS
%INCLUDE PLLDBEXT.J86                                                   !2.5CS
\%INCLUDE CSRBFEXT.J86                                                  ! ILC !2.9NWB
%INCLUDE IEFEXT.J86                                                     ! ILC
%INCLUDE PDTWFEXT.J86                                                   ! ILC
\%INCLUDE CIMFEXT.J86                                                   ! ILC !2.9NWB
\%INCLUDE CITEMEXT.J86                                                  ! ILC !2.9NWB
\%INCLUDE CSRWFEXT.J86                                                  ! ILC !2.9NWB
%INCLUDE FPFEXT.J86                                                     ! ILC
%INCLUDE ONORDEXT.J86                                                   ! ILC
%INCLUDE DIROREXT.J86                                                   ! ILC
%INCLUDE DIRSUEXT.J86                                                   ! ILC
%INCLUDE DIRWFEXT.J86                                                   ! ILC
%INCLUDE LDTCFEXT.J86                                                   ! ILC
%INCLUDE DRSMQEXT.J86                                                   ! ILC
%INCLUDE SITEMEXT.J86                                                   ! ILC
%INCLUDE IDSOFEXT.J86                                                   ! JLC
!%INCLUDE STOCKEXT.J86                                                  !1.8BG !2.1BG !2.5CS
!%INCLUDE IMSTCEXT.J86                                                  !1.8BG !2.1BG !2.5CS
!%INCLUDE LSSSTEXT.J86                                                  !1.8BG !2.7CS
%INCLUDE TSFEXT.J86                                                     !2.3JAS
%INCLUDE CBEXT.J86                                                      !2.9NWB

   FUNCTION ADXSTART(NAME$, PARM$, MESS$) EXTERNAL                            !2.9NWB
      INTEGER*2 ADXSTART                                                      !2.9NWB
      STRING    NAME$, PARM$, MESS$                                           !2.9NWB
   END FUNCTION                                                               !2.9NWB

\*******************************************************************************
\***
\***   SUB ASYNC.ERROR
\***
\***      STANDARD.ERROR.DETECTED is called to log the error.  If an
\***      error is detected in this section of code it is logged from
\***      ERROR.DETECTED.IN.ASYNC.
\***
\*******************************************************************************

    SUB ASYNC.ERROR(RETRY.FLAG%,OVERLAY.STRING$)

       STRING OVERLAY.STRING$
       INTEGER*2 RETRY.FLAG%

       RETRY.FLAG% = 0
       ON ERROR GOTO ERROR.DETECTED.IN.ASYNC

       EXIT SUB

\***---------------------------------------------------------------------------
\***   ERROR.DETECTED.IN.ASYNC:
\***
\***      The standard error detected function is called to log the error.
\***
\***---------------------------------------------------------------------------
     ERROR.DETECTED.IN.ASYNC:

       EXIT SUB

   END SUB


\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   I N T E R N A L   F U N C T I O N   D E C L A R A T I O N S          ***
\***                                                                        ***
\***   - FN.VALIDATE.STATE                                                  ***
\***   - FN.TRANSLATE.TEXT                                                  ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   Function : FN.VALIDATE.STATE( C.STATE$, L.STATE$ )
\***
\***   Purpose  : Validate received record type C.STATE$, given L.STATE$ was
\***              the last state received
\***
\***   Output   : = 0 for invalid (out of sequence)
\***              = 1 for valid (in sequence)
\***
\******************************************************************************

   FUNCTION FN.VALIDATE.STATE(C.STATE$, L.STATE$)
      STRING     C.STATE$, L.STATE$
      INTEGER*1  FN.VALIDATE.STATE
        EXP.STATES$ = V.TAB$(ASC(L.STATE$) - ASC("1") + 1 ) ! 2.10BG Changed from "A" to "1" to allow earlier Data States
        FN.VALIDATE.STATE = MATCH(C.STATE$, EXP.STATES$, 1)
   END FUNCTION

\******************************************************************************
\***
\***   Function : FN.TRANSLATE.TEXT( TEXT$ )
\***
\***   Purpose  : Translate passed text from 4680 character set to PDT
\***              character set
\***
\***   Output   : = string
\***
\******************************************************************************

   FUNCTION FN.TRANSLATE.TEXT( TEXT$ )                                  ! CSW
      STRING  FN.TRANSLATE.TEXT, TEXT$                                  ! CSW
      FN.TRANSLATE.TEXT = TRANSLATE$(TEXT$, TRANS.FROM$, TRANS.TO$)     ! CSW
   END FUNCTION                                                         ! CSW

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   M A I N L I N E   C O D E                                            ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   PROGRAM.START:
\***
\***      handle all errors in ERROR.DETECTED
\***
\***      execute all SET included code
\***
\***      display 'start-up message'
\***      initialise the program / allocate session numbers
\***
\***      set RECEIVE.STATE$ to "A"
\***      while RECEIVE.STATE$ does not equal "Z"
\***
\***      WAITING:
\***         display 'waiting' message
\***
\***         wait until able to read data from PSS38
\***              or until the LDT pipe has become active
\***
\***         if the LDT pipe has data then read pipe and send a
\***            message to PSS38 to load program into LDT
\***            GOTO waiting
\***         endif
\***
\***         read data from PSS38 (via pipe)
\***         work out the data type of the received data ( "A" - "Z")
\***
\***         check the data is expected
\***
\***         if data is not expected then set STATE$ to "*" (out of sequence)
\***
\***         if the data type is "A" - "o" then call the appropriate
\***            subroutine ;
\***            TIDY.UP,                        A
\***            RECEIVED.LOG.ON,                B
\***            RECEIVED.EPSOM.FILE.HEADER,     C ***
\***            RECEIVED.EPSOM.LIST.HEADER,     D ***
\***            RECEIVED.EPSOM.LIST.COUNT,      E ***
\***            RECEIVED.EPSOM.LIST.TRAILER,    F ***
\***            RECEIVED.EPSOM.FILE.TRAILER,    G ***
\***            RECEIVED.EPSOM.LIST.REQUEST     H ***
\***            RECEIVED.EPSOM.EOT              I ***
\*** >>> Following states removed and Replaced  ver 2.9NWB
\*** >>>        RECEIVED.CSR.LIST.HEADER        J *
\*** >>>        RECEIVED.CSR.LIST.RECORD        K *
\*** >>>        RECEIVED.CSR.LIST.TRAILER       L *
\*** >>>        RECEIVED.CSR.TABLE.REQUEST      M *
\*** >>>        RECEIVED.CSR.EOT                N *
\*** >>> End of Replaced states                 ver 2.9NWB
\***            RECEIVED.ASN.CARTON.SOT         J                       ! 2.9NWB
\***            RECEIVED.ASN.CARTON.AUTO.RCD    K                       ! 2.9NWB
\***            RECEIVED.ASN.CARTON.MAN.RCD     L                       ! 2.9NWB
\***            ????????                        M                       ! 2.9NWB
\***            RECEIVED.ASN.CARTON.EOT         N                       ! 2.9NWB
\***            RECEIVED.PCHECK.HEADER          O *
\***            RECEIVED.PCHECK.RECORD          P *
\***            RECEIVED.PCHECK.TRAILER         Q *
\***            RECEIVED.CHECK.VERSION.REQUEST  R **
\***            RECEIVED.PROGRAM.TRAILER        S **
\***            RECEIVED.DIRECT.ORDERS.REQUEST  T **
\***            RECEIVED.DIRECT.FILE.HEADER     U **
\***            RECEIVED.DIRECT.ORDERS.HEADER   V **
\***            RECEIVED.DIRECT.ORDERS.DETAIL   W **
\***            RECEIVED.DIRECT.ORDERS.TRAILER  X **
\***            RECEIVED.DIRECT.FILE.TRAILER    Y **
\***            RECEIVED.DIRECT.EOT             Z **
\***            RECEIVED.UOD.FILE.HEADER        a **
\***            RECEIVED.UOD.HEADER             b **
\***            RECEIVED.UOD.DETAIL             c **
\***            RECEIVED.UOD.TRAILER            d **
\***            RECEIVED.UOD.FILE.TRAILER       e **
\***            RECEIVED.UOD.EOT                f **
\***            RECEIVED.RETURNS.FILE.ID        g ****
\***            RECEIVED.RETURNS.FILE.HDR       h ****
\***            RECEIVED.RETURNS.UOD.HDR        i ****
\***            RECEIVED.RETURNS.UOD.DETAIL     j ****
\***            RECEIVED.RETURNS.UOD.TRAILER    k ****
\***            RECEIVED.RETURNS.FILE.TRAILER   l ****
\***            RECEIVED.RETURNS.FILE.REQUEST   m ****
\***            RECEIVED.RETURNS.FILE.REC.OK    n ****
\***            RECEIVED.RETURNS.EOT            o ****
\***            RECEIVED.STOCKTAKE.HEADER       p ****
\***            RECEIVED.STOCKTAKE.DETAIL       q ****
\***            RECEIVED.STOCKTAKE.TRAILER      r ****
\***            RECEIVED.STOCKCOUNT.HEADER      s *****
\***            RECEIVED.STOCKCOUNT.DETAIL      t *****
\***            RECEIVED.STOCKCOUNT.TRAILER     u *****
\***            RECEIVED.STOCKCOUNT.EOT         v *****
\***            RECEIVED.PHARMACY.COUNT.HEADER  w *****
\***            RECEIVED.PHARMACY.COUNT.DETAIL  x *****
\***            RECEIVED.PHARMACY.COUNT.TRAILER y *****
\***            RECEIVED.PHARMACY.COUNT.EOT     Z *****
\***         endif
\***         *      These modules are all in module 01
\***         **     These modules are all in module 02
\***         ***    These modules are all in module 03
\***         ****   These modules are all in module 04
\***         *****  These modules are all in module 05
\***
\***         if the STATE$ is "*" then
\***            log an error
\***            wait until PDT times out (a set time)
\***            reset STATE$ to "A" (waiting for connection)
\***         endif
\***
\***      wend
\***
\***   PROGRAM.EXIT:
\***      GOSUB SHUTDOWN to close all open files and de-allocate session no.s
\***
\***   PROGRAM.QUIT:
\***   STOP
\***
\******************************************************************************

   PROGRAM.START:

      ON ASYNC ERROR CALL ASYNC.ERROR

      ON ERROR GOTO ERROR.DETECTED

      BATCH.SCREEN.FLAG$ = "B"
      OPERATOR.ID$ = "99999999"
      MODULE.NUMBER$ = "PSS3700"

      DIM PLLDB.TABLE$(3000)                                            !2.5CS
      DIM EAN.TABLE$(1000)                                              !2.5CS

      GOSUB PORT.SETUP                                                  ! DSW

      %INCLUDE ASYNCSEB.J86                                             ! ILC
      %INCLUDE PIPEISEB.J86                                             ! JLC
      %INCLUDE PIPEOSEB.J86                                             ! JLC
      %INCLUDE PLDTSETA.J86                                             ! JLC
      CALL BCSMF.SET                                                    ! ILC
      CALL CCUOD.SET                                                    ! MMJK
      CALL CCLAM.SET                                                    ! 1.4
      CALL CCITM.SET                                                    ! MMJK
      CALL CCTRL.SET                                                    ! MMJK
      CALL CCDMY.SET                                                    ! MMJK
      CALL CCTMP.SET                                                    ! MMJK
      CALL CCBUF.SET                                                    ! MMJK
      CALL CCUPF.SET                                                    ! NMJK
      CALL CCWKF.SET                                                    ! MMJK
      CALL CHKBF.SET                                                    ! ILC
      CALL GAPBF.SET                                                    ! 1.3  !2.5CS !2.6BG
      CALL PLLOL.SET                                                    !2.5CS
      CALL PLLDB.SET                                                    !2.5CS
\     CALL CIMF.SET                                                     ! ILC !2.9NWB
\     CALL CITEM.SET                                                    ! ILC !2.9NWB
\     CALL CSR.SET                                                      ! ILC !2.9NWB
\     CALL CSRBF.SET                                                    ! ILC !2.9NWB
\     CALL CSRWF.SET                                                    ! ILC !2.9NWB
      CALL EPSOM.SET                                                    ! ILC
      CALL FPF.SET                                                      ! ILC
      CALL IDF.SET                                                      ! ILC
      CALL IEF.SET                                                      ! ILC
      CALL INVOK.SET                                                    ! ILC
      CALL IRF.SET                                                      ! ILC
      CALL LDTAF.SET                                                    ! MMJK
      CALL ONORD.SET                                                    ! ILC
      CALL PCHK.SET                                                     ! ILC
      CALL PDTWF.SET                                                    ! ILC
      CALL PIITM.SET                                                    ! ILC
      CALL PILST.SET                                                    ! ILC
      CALL SOFTS.SET                                                    ! MMJK
      CALL STKMQ.SET                                                    ! ILC
      CALL UNITS.SET                                                    ! ILC
      CALL DIRORD.SET                                                   ! ILC
      CALL DIRSUP.SET                                                   ! ILC
      CALL DIRWF.SET                                                    ! ILC
      CALL DIREC.SET                                                    ! ILC
      CALL LDTCF.SET                                                    ! ILC
      CALL DRSMQ.SET                                                    ! ILC
      CALL LDTBF.SET                                                    ! JLC
      CALL IDSOF.SET                                                    ! JLC
      CALL UOD.SET                                                      ! LLC
      CALL UODBF.SET                                                    ! LLC
      CALL UODTF.SET                                                    ! LLC
      CALL STKBF.SET                                                    ! 1.5
      CALL SXTCF.SET                                                    ! 1.5
      CALL SXTMP.SET                                                    ! 1.5
      CALL STKTK.SET                                                    ! 1.5
      CALL STLDT.SET                                                    ! 1.7
      CALL STOCK.SET                                                    !1.8BG
      CALL IMSTC.SET                                                    !1.8BG
!     CALL LSSST.SET                                                    !1.8BG !2.7CS
!     CALL STKCF.SET                                                    ! 1.9DA
      CALL STKMF.SET                                                    ! 1.9DA
      CALL STKRC.SET                                                    ! 1.9DA
      CALL XGCF.SET                                                     ! 1.9DA
      CALL STKEX.SET                                                    ! 1.9DA
      CALL STKIF.SET                                                    ! 1.9DA
      CALL LOCCNT.SET                                                   ! 1.9DA
      CALL STKIG.SET                                                    ! 1.9DA
      CALL STKTF.SET                                                    ! 1.9DA
      CALL STKDC.SET                                                    ! 1.9DA
      CALL SSPSCTRL.SET                                                 ! 2.2JAS
      CALL BTCS.SET                                                     ! 2.2JAS
      CALL TSF.SET                                                      ! 2.3JAS
      CALL SOPTS.SET                                                    ! 2.3JAS
      CALL LOCAL.SET                                                    ! 2.3JAS
      CALL CCREJ.SET                                                    ! 2.4JAS
      CALL CB.SET                                                       !2.9NWB
      CALL REWKF.SET                                                    ! 2.10BG
      CALL RB.SET                                                       ! 2.10BG
      CALL RECALLS.SET                                                  ! 2.10BG
      CALL DELVINDX.SET                                                 ! 2.12SH
      CALL AF.SET                                                       ! 2.12SH
      CALL UODOT.SET                                                    ! 2.12SH
      CALL UB.SET                                                       ! 2.12SH
      
      SB.MESSAGE$ = "PDT Support - Started"                             ! DSW
      GOSUB SB.BG.MESSAGE

      GOSUB SETUP

      LDTAF.LINK.TYPE% = 0                                              ! MMJK
      GOSUB LOG.TO.LDTAF.FILE                                           ! MMJK

      RECEIVE.STATE$ = "A"
      WHILE RECEIVE.STATE$ <> "?"                                       !HDS

      WAITING:

         IF MATCH(RECEIVE.STATE$, "AHNRTlm", 1) = 0 THEN BEGIN          ! MMJK
            NOW% = FN.SECONDS(TIME$)                                    ! DSW
            IF DATE$ <> LAST.ACTIVE.DATE$                               \ DSW
            OR (NOW% - LAST.ACTIVE%) > INACTIVITY.SHUTDOWN% THEN BEGIN  ! DSW
               GOSUB INACTIVITY.SHUTDOWN                                ! DSW
            ENDIF                                                       ! DSW
         ENDIF ELSE BEGIN                                               ! DPAB
            IF VAL(LEFT$(TIME$,2)) >= 22 THEN BEGIN                     ! DPAB
               IF DATE$ <> LAST.SHUTDOWN.DATE$ THEN BEGIN               ! DPAB
                  LAST.SHUTDOWN.DATE$ = DATE$                           ! DPAB
                  GOSUB INACTIVITY.SHUTDOWN                             ! DPAB
               ENDIF                                                    ! DPAB
            ENDIF                                                       ! DPAB
         ENDIF                                                          ! DSW

         SB.MESSAGE$ = "PDT Support - Data state : " + RECEIVE.STATE$   ! DSW
         GOSUB SB.BG.MESSAGE                                            ! DSW

         WAIT PIPEO.SESS.NUM%, PLDT.SESS.NUM% ; 60000                   ! HDS
         TEMP.EVENT% = EVENT%                                           ! FSSJ
         IF TEMP.EVENT% = 0 THEN GOTO WAITING                           ! FSSJ

         IF TEMP.EVENT% = PLDT.SESS.NUM% THEN BEGIN                     ! HDS
            IF END# PLDT.SESS.NUM% THEN READ.ERROR                      ! HDS
            CURR.SESS.NUM% = PLDT.SESS.NUM%                             ! HDS
            READ# PLDT.SESS.NUM%; PLDT.RECORD$                          ! HDS
            IF LEFT$(PLDT.RECORD$,4) <> "PLDT" THEN BEGIN               ! HDS
               GOTO WAITING                                             ! HDS
            ENDIF                                                       ! HDS
            GOSUB REQUEST.PROGRAM.LOAD                                  ! HDS
            GOTO WAITING                                                ! HDS
         ENDIF                                                          ! HDS

         IF END# PIPEO.SESS.NUM% THEN READ.ERROR
         CURR.SESS.NUM% = PIPEO.SESS.NUM%
         READ# PIPEO.SESS.NUM%; PIPE.IN$

         IF LEFT$(PIPE.IN$, 1) <> "R"                                   \
         OR LEN(PIPE.IN$) < 2 THEN GOTO WAITING
         DATA.IN$ = RIGHT$(PIPE.IN$, LEN(PIPE.IN$) - 1)
         GOSUB DETERMINE.DATA.TYPE
         LOG.STATE$ = STATE$
         
         IF STATE$ = "@" THEN BEGIN                                     !2.8CS
             GOSUB SHUTDOWN                                             !2.8CS
             GOTO PROGRAM.QUIT                                          !2.8CS
         ENDIF                                                          !2.8CS

         IF MATCH(STATE$,                                               \ MMJK
            "123456789:;<=ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 1) > 0  \ 2.12SH
            THEN BEGIN                                                  ! LLC
            IF FN.VALIDATE.STATE(STATE$, RECEIVE.STATE$) = 0 THEN BEGIN
               STATE$ = "*"
            ENDIF
         ENDIF
         
         RECEIVE.STATE$ = STATE$

         IF MATCH(RECEIVE.STATE$,                                       \ LLC
            "123456789:;<=ABCDEFGHIJKLMNOPQRSTUVWXYZabcdef", 1) > 0     \ 2.12SH
            THEN BEGIN                                                  ! LLC
            ON ((ASC(RECEIVE.STATE$)) - ASC("1") + 1) GOSUB             \
               CALL.OTHER.MODULE5,                                      \ 2.10BG  State 1
               CALL.OTHER.MODULE5,                                      \ 2.10BG  State 2
               CALL.OTHER.MODULE5,                                      \ 2.10BG  State 3
               CALL.OTHER.MODULE5,                                      \ 2.10BG  State 4
               CALL.OTHER.MODULE5,                                      \ 2.10BG  State 5
               CALL.OTHER.MODULE5,                                      \ 2.10BG  State 6
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State 7
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State 8
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State 9
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State :
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State ;
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State <
               CALL.OTHER.MODULE5,                                      \ 2.12SH  State =
               DUMMY.ROUTINE,                                           \ 2.10BG  State > !DO NOT USE
               DUMMY.ROUTINE,                                           \ 2.10BG  State ? !DO NOT USE
               DUMMY.ROUTINE,                                           \ 2.10BG  State @ !DO NOT USE
               TIDY.UP,                                                 \
               RECEIVED.LOG.ON,                                         \
               CALL.OTHER.MODULE3,                                      \ LLC
               CALL.OTHER.MODULE3,                                      \ LLC
               CALL.OTHER.MODULE3,                                      \ LLC
               CALL.OTHER.MODULE3,                                      \ LLC
               CALL.OTHER.MODULE3,                                      \ LLC
               CALL.OTHER.MODULE3,                                      \ LLC
               CALL.OTHER.MODULE3,                                      \ LLC
\              CSR.LIST.HEADER.RECEIVED,                                \ EPAB
\              WRITE.DATA.TO.CSR.BUFFER,                                \ DJAL
\              WRITE.DATA.TO.CSR.BUFFER,                                \ DJAL
\              CALL.OTHER.MODULE,                                       \ DJAL
\              RECEIVED.CSR.EOT,                                        \ DJAL
               ASN.CARTON.HEADER.RECEIVED,                              \ 2.9NWB
               AUTOMATIC.CARTON.BOOK.IN,                                \ 2.9NWB
               MANUAL.CARTON.BOOK.IN,                                   \ 2.9NWB
               DUMMY.ROUTINE,                                           \ 2.9NWB
               ASN.CARTON.EOT.RECEIVED,                                 \ 2.9NWB
               CALL.OTHER.MODULE,                                       \ DJAL
               CALL.OTHER.MODULE,                                       \ DJAL
               CALL.OTHER.MODULE,                                       \ DJAL
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ HDS
               CALL.OTHER.MODULE2,                                      \ LLC
               DUMMY.ROUTINE,                                           \ LLC State [ !DO NOT USE
               DUMMY.ROUTINE,                                           \ LLC State \ !DO NOT USE
               DUMMY.ROUTINE,                                           \ LLC State ] !DO NOT USE
               DUMMY.ROUTINE,                                           \ LLC State ^ !DO NOT USE
               DUMMY.ROUTINE,                                           \ LLC State _ !DO NOT USE
               DUMMY.ROUTINE,                                           \ LLC State . !DO NOT USE
               CALL.OTHER.MODULE2,                                      \ LLC
               CALL.OTHER.MODULE2,                                      \ LLC
               CALL.OTHER.MODULE2,                                      \ LLC
               CALL.OTHER.MODULE2,                                      \ LLC
               CALL.OTHER.MODULE2,                                      \ LLC
               CALL.OTHER.MODULE2                                       ! LLC
               !Moved last 16 from here to the next section to allow the new first 16 ! 2.10BG
               
         ENDIF ELSE BEGIN                                               ! 2.2JAS
            IF MATCH(RECEIVE.STATE$,                                    \ 2.2JAS
               "ghijklmnopqrstuvwxyz", 1) > 0                           \ 2.2JAS ! 2.10BG
               THEN BEGIN                                               ! 2.2JAS
                ON ((ASC(RECEIVE.STATE$)) - ASC("g") + 1) GOSUB         \ 2.2JAS ! 2.10BG
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ MMJK
                  CALL.OTHER.MODULE4,                                   \ 1.5
                  CALL.OTHER.MODULE4,                                   \ 1.5
                  CALL.OTHER.MODULE4,                                   \ 1.5
                  CALL.OTHER.MODULE4,                                   \ 1.5
                  CALL.OTHER.MODULE5,                                   \ 2.1BG
                  CALL.OTHER.MODULE5,                                   \ 2.1BG
                  CALL.OTHER.MODULE5,                                   \ 2.1BG
                  CALL.OTHER.MODULE5,                                   \ 2.1BG ! 2.2JAS
                  CALL.OTHER.MODULE5,                                   \ 2.2JAS
                  CALL.OTHER.MODULE5,                                   \ 2.2JAS
                  CALL.OTHER.MODULE5,                                   \ 2.2JAS
                  CALL.OTHER.MODULE5                                    ! 2.2JAS
            ENDIF                                                       ! 2.2JAS
         ENDIF                                                          ! 2.2JAS

         IF RECEIVE.STATE$ = "*"  THEN BEGIN
            GOSUB SB.LOG.PDT.ERROR
            IF CURR.TERMINAL$ <> "??????" THEN BEGIN                    ! LLC
               CSR.AUDIT.DATA$ = DEVICE$ +                              \ NMJK
                   "Link session ABORTED by Application"                \ NMJK
                   + " program, " + DEVICE$ + "number " + CURR.TERMINAL$! LLC
               GOSUB LOG.TO.AUDIT.FILE                                  ! EPAB
               CSR.AUDIT.DATA$ = "Message sent to " + DEVICE$ + "[" +   \ LLC
                  NAK.LINE.1$ + " " + NAK.LINE.2$ + " " +               \ EPAB
                  NAK.LINE.3$ + "]"                                     ! EPAB
               GOSUB LOG.ABORT.TO.AUDIT.FILE                            ! EPAB
            ENDIF                                                       ! EPAB
            GOSUB TIDY.UP
            RECEIVE.STATE$ = "A"                                        ! ILC
         ENDIF

      WEND

   PROGRAM.EXIT:
   
      IF RE.CHAIN THEN BEGIN                                            ! ILC
         PIPE.OUT$ = "QR"                                               ! ILC
         GOSUB SEND.TO.PSS38                                            ! ILC
         CALL ADXSERCL                                                  ! ILC
         CHAIN "ADX_UPGM:PSS37.286", MONITORED.PORT$                    ! ILC
      ENDIF ELSE BEGIN                                                  ! ILC
         PIPE.OUT$ = "Q"                                                ! ILC
         GOSUB SEND.TO.PSS38                                            ! ILC
      ENDIF                                                             ! ILC

      GOSUB SHUTDOWN
      
   PROGRAM.QUIT:
   STOP

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***   S U B R O U T I N E S                                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\*****************************************************************************
\***   DUMMY.ROUTINE:
\***
\***   to allow the use of lower case DATA.STATEs - there are 6 non-alpha
\***   ASCII characters between Z and a.  The MATCH statement must allow
\***   for these characters, so they call this dummy routine.
\***
\*****************************************************************************

DUMMY.ROUTINE:

RETURN

\******************************************************************************
\***
\***   PORT.SETUP:
\***
\***      determine the port being monitored by interrogating the command tail
\***
\***   RETURN
\***
\******************************************************************************

PORT.SETUP:

   TRUE = -1                                                            ! ILC
   FALSE = 0                                                            ! ILC

   WARM.START = FALSE                                                   ! ILC
   RE.CHAIN = FALSE                                                     ! ILC
   IF LEFT$(COMMAND$,3) = "PI:" THEN BEGIN                              ! ILC
      USE MONITORED.PORT$                                               ! ILC
      WARM.START = TRUE                                                 ! ILC
   ENDIF ELSE BEGIN                                                     ! ILC
      IF MATCH("BACKGRND",COMMAND$,1) > 0 THEN BEGIN                    ! DSW
         IF LEN(COMMAND$) > MATCH("BACKGRND",COMMAND$,1) + 8 THEN BEGIN ! DSW
            MONITORED.PORT$ = MID$(COMMAND$,MATCH("BACKGRND",COMMAND$,1) + 9,1)
         ENDIF ELSE BEGIN                                               ! DSW
            MONITORED.PORT$ = " "                                       ! DSW
         ENDIF                                                          ! DSW
      ENDIF ELSE BEGIN                                                  ! DSW
         SB.EVENT.NO% = 73                                              ! DSW
         GOSUB SB.LOG.AN.EVENT                                          ! DSW
         GOTO PROGRAM.EXIT                                              ! DSW
      ENDIF                                                             ! DSW
   ENDIF                                                                ! ILC

   IF MATCH(MONITORED.PORT$,"AB",1) = 0 THEN BEGIN                      ! DSW
      SB.MESSAGE$ = "INVALID PORT NAME"                                 ! DSW
      GOSUB SB.BG.MESSAGE                                               ! DSW
      SB.UNIQUE$ = MONITORED.PORT$                                      ! DSW
      SB.EVENT.NO% = 74                                                 ! DSW
      GOSUB SB.LOG.AN.EVENT                                             ! DSW
      GOTO PROGRAM.EXIT                                                 ! DSW
   ENDIF                                                                ! DSW

RETURN


\******************************************************************************
\***
\***   SETUP:
\***
\***      set-up general variables
\***      set-up link/pdt control characters
\***      dimension and set-up received data state validation tables
\***      dimension general arrays
\***      set-up all session numbers required in program
\***      set-up constant values
\***      open communication pipes PSS37 and PSS38
\***      gosub ZEROISE.CITEM (because, following a reboot, it is possible
\***      that the ON.ORDER.IN.THIS.PDT field could still be set.)
\***
\***   RETURN
\***
\******************************************************************************

   SETUP:

      CALL ADXSERVE(ADX.RET.CODE%, 4, 0, APPL.STATUS$)
      IF ADX.RET.CODE% <> 0 THEN BEGIN
         SB.EVENT.NO% = 23
         SB.UNIQUE$ = FN.Z.PACK(STR$(ADX.RET.CODE%),5) + "04   "
         SB.MESSAGE$ = "ADXSERVE FUNCTION 4 FAILURE"
         GOSUB SB.LOG.AN.EVENT
      ENDIF

      STORE.NUMBER$ = LEFT$(APPL.STATUS$,4)
      SAVED.STORE.NUMBER$ = STORE.NUMBER$
      DATE.TODAY$ = DATE$
      TIDY.FLAG$ = "N"
      PDT.ACTION$ = "INACTIVE"
      NUMBER.OF.RECORDS% = 0                                            ! MMJK
      LOG.ON.TIME$ = "000000"
      PK2$ = PACK$("0000")
      PK4$ = PACK$("00000000")
      LAST.ACTIVE.DATE$ = DATE$                                         ! DSW
      LAST.ACTIVE% = 0                                                  ! DSW
      LAST.STOCKTAKE.DATE$ = DATE$                                      ! 1.5
      LAST.STOCKTAKE% = FN.SECONDS(TIME$)                               ! 1.5
      NAK.LINE.1$ = ""                                                  ! DSW
      NAK.LINE.2$ = ""                                                  ! DSW
      NAK.LINE.3$ = ""                                                  ! DSW
      PREV.LOGGED.STATE$ = ""                                           ! JLC
      LDTBF.HEADER.WRITTEN$ = "N"                                       ! JLC

      SOH$ = CHR$(01h)
      STX$ = CHR$(02h)
      ENQ$ = CHR$(05h)
      CR$  = CHR$(0Dh)
      LF$  = CHR$(0Ah)
      ACK$ = CHR$(06h)
      NAK$ = CHR$(15h)
      CRLF$ = CR$ + LF$

      ! 4680 character set --> MSI character set translation table
      !
      ! character 04h on the PDT displays a pound sign
      ! character 03h displays a backslash
      !
      TRANS.FROM$ = "œ"       + "\"       + "$"                         ! CSW
      TRANS.TO$   = CHR$(03h) + CHR$(04h) + CHR$(03h)                   ! CSW

      DIM V.TAB$(74)                                                    ! 1.5 ! 2.1BG !2.2JAS ! 2.10BG
      FOR I% = 1 TO 74 : READ V.TAB$(I%) : NEXT I%                      ! 1.5 ! 2.1BG !2.2JAS ! 2.10BG

!------------------------------------------------------------------------------
! DATA - V.TAB$() - received state, validation table
!
! state   description                           next valid state
!------------------------------------------------------------------------------
! 1     = Recall File Request                   2                           ! 2.10BG
! 2     = Recall File Received OK               6                           ! 2.10BG
! 3     = Recall Header                         4,5                         ! 2.10BG
! 4     = Recall Detail                         4,5                         ! 2.10BG
! 5     = Recall Trailer                        3,6                         ! 2.10BG
! 6     = Recall EOT                            -                           ! 2.10BG
! 7     = +UOD sign on received                 8                           ! 2.12SH
! 8     = +UOD expected received okay received  -                           ! 2.12SH
! 9     = +UOD booking header received          : <                         ! 2.12SH
! :     = +UOD booking detail receivd           : ; <                       ! 2.12SH
! ;     = +UOD booking batch trailer received   : <                         ! 2.12SH
! <     = +UOD booking session trailer received =                           ! 2.12SH
! =     = +UOD EOT                              -                           ! 2.10BG
! >     = Dummy (ASCII 062)                     a                           ! 2.10BG
! ?     = Dummy (ASCII 063)                     a                           ! 2.10BG
! @     = Dummy (ASCII 064)                     a                           ! 2.10BG
! A     = waiting for log-on                    B
! B     = log-on record received                3,C,H,J,M,O,R,U,a,g,p,s,w     1.5 ! 2.1BG !2.2JAS ! 2.10BG
! C     = EPSOM file header record received     D
! D     = EPSOM list header record received     E
! E     = EPSOM list count record received      E,F
! F     = EPSOM list trailer record received    D,G
! G     = EPSOM file trailer record received    H,I
! H     = EPSOM list request record received    G
! I     = EPSOM EOT received                    -
! J     = ASN Carton header received            K,L                           !2.9NWB
! K     = ASN Automatic Carton Book In          K,L,N,R                       !2.9NWB
! L     = ASN Manual Carton Book In             K,L,N,R                       !2.9NWB
! M     = ASN                                                                 !2.9NWB
! N     = ASN Carton EOT received               -                             !2.9NWB
! O     = Price check header received           P
! P     = Price check record received           P,Q
! Q     = Price check trailer received          -
! R     = Check/Request program version         1,S,T,Z,f,m,v,z          2.1BG !2.2JAS ! 2.10BG
! S     = Program trailer                       T,Z,o,f                 OMJK
! T     = Directs order request                 Y
! U     = Directs file header                   V
! V     = Directs order header                  W,X
! W     = Directs order detail                  W,X
! X     = Directs order trailer                 V,Y
! Y     = Directs file trailer                  R,T,Z
! Z     = Directs EOT                           -
! [     = Dummy (ASCII 091)                     a
! \     = Dummy (ASCII 092)                     a
! ]     = Dummy (ASCII 093)                     a
! ^     = Dummy (ASCII 094)                     a
! _     = Dummy (ASCII 095)                     a
! `     = Dummy (ASCII 096)                     a
! a     = UOD File Header                       b
! b     = UOD Header                            c,d
! c     = UOD Detail                            c,d
! d     = UOD Trailer                           b,e
! e     = UOD File Trailer                      R
! f     = UOD EOT                               -
! g     = RETURNS File ID                       h,R
! h     = RETURNS File Header                   i,l                      NMJK
! i     = RETURNS UOD Header                    j,k
! j     = RETURNS Item Detail                   j,k
! k     = RETURNS UOD Trailer                   i,l
! l     = RETURNS File Trailer                  o
! m     = RETURNS File Request                  n
! n     = RETURNS File Received OK              o
! o     = RETURNS EOT                           -
! p     = STOCKTAKE File Header                 q                        1.5
! q     = STOCKTAKE Item Detail                 q,r                      1.5
! r     = STOCKTAKE File Trailer                -                        1.5
! s     = STOCKCOUNT File Header                t                        2.1BG
! t     = STOCKCOUNT Detail                     t,u                      2.1BG
! u     = STOCKCOUNT File Trailer               R                        2.1BG
! v     = STOCKCOUNT EOT                        -                        2.1BG
! w     = PHARMACY File Header                  x                        2.2JAS
! x     = PHARMACY Item Detail                  x,y                      2.2JAS
! y     = PHARMACY File Trailer                 z                        2.2JAS
! z     = PHARMACY EOT                          -                        2.2JAS
!
!------------------------------------------------------------------------------
DATA     "2","6","45","45","36","-","8","-",":<",":;<",":<","=","-","a", \ 2.12SH
         "a","a",                                                        \ 2.12SH
         "B", "379CHJMORUagpsw", "D", "E", "EF", "DG", "HI", "G", "-",   \ 2.12SH
         "KL", "KLNR", "KLNR", "-", "-", "P", "PQ", "-",                 \ DSW!2.9NWB
         "1SNTZfmvz", "TZof", "Y", "V","WX","WX","VY","RTZ","-",         \ OMJK !2.1BG !2.2JAS ! 2.10BG
         "a","a","a","a","a","a",                                        \ LLC
         "b","cd","cd","be","R","-",                                     \ OMJK
         "hR","il","jk","jk","il","o","n","o","-","q","qr","-",          \ 1.5
         "t","tu","R", "-",                                              \! 2.1BG !2.2JAS
         "x","xy","z","-"                                                ! 2.2JAS

      DIM LOGON.TAB$(12)                                                 ! 2.12SH
      FOR I% = 1 TO 12 : READ LOGON.TAB$(I%) : NEXT I%                   ! 2.12SH

!------------------------------------------------------------------------------
! DATA - LOGON.TAB$() - expected states depending upon application number
!
! application no.    description                next valid state
!------------------------------------------------------------------------------
!       01           EPSOM                      B,C,H
!       02           ASN                        B,J                           !2.9NWB
!       03           Price check                B,O
!       27 (=4)                                 B                             !2.9NWB
!       05           DIRECTS                    B,R,U
!       06           UOD                        B,a
!       07           RETURNS                    B,R,g
!       08           STOCKTAKE                  B,p
!       09           STOCKCOUNT                 B,s     2.1BG
!       10           PHARMACY                   B,w     2.2JAS
!       11           RECALLS                    B,1,3                         !2.10BG
!       12           +UOD                       B,
!                                               B,7,9                         !2.12SH
! NOTE : log-on is allowed at any point in case of previous failure
!
!------------------------------------------------------------------------------
DATA     "BCH", "BJ", "BO", "B", "BRU", "Ba" , "BRg" , "Bp" , "Bs", "Bw", "B13", "B79" !2.12SH

      REC.MAX% = 67                                                     ! 2.12SH
      DIM REC.CHECK$(REC.MAX%)

      ! Log-on record (B)
      !
      ! SOH + nnnn + nnnnnn + nn + ENQ$
      !
      REC.CHECK$(1) = "B"+SOH$ + "N"+CHR$(4) + "N"+CHR$(6) +            \ DJAL
                      "N"+CHR$(2) + "B"+ENQ$                            ! DJAL

      ! File header record (C)
      !
      ! aa + nn + nn + nnnnnn
      !
      REC.CHECK$(2) = "A"+CHR$(2) + "N"+CHR$(2) + "N"+CHR$(2) +         \
                      "N"+CHR$(6)

      ! List header record (D)
      !
      ! aa + nnnn + aaaaa
      !
      REC.CHECK$(3) = "A"+CHR$(2) + "N"+CHR$(4) + "A"+CHR$(5)

      ! List item record (E)
      !
      ! aa + nnnnnnn + aaaa + aaaa + aaaaaa + aaaa + nnnnnn + aaaaaaaaa
      !
      REC.CHECK$(4) = "A"+CHR$(2) + "N"+CHR$(7) + "A"+CHR$(4) +         \
                      "A"+CHR$(4) + "A"+CHR$(6) + "A"+CHR$(4) +         \
                      "N"+CHR$(6) + "A"+CHR$(9)

      ! List trailer record (F)
      !
      ! aa + nnnn + nnn
      !
      REC.CHECK$(5) = "A"+CHR$(2) + "N"+CHR$(4) + "N"+CHR$(3)

      ! File trailer record (G)
      !
      ! aa + nn + nnnnnn + nn
      !
      REC.CHECK$(6) = "A"+CHR$(2) + "N"+CHR$(2) + "N"+CHR$(6) +         \
                      "N"+CHR$(2)

      ! List request record (H)
      !
      ! SOH + nn + nnnnnn
      !
      REC.CHECK$(7) = "B"+SOH$ + "N"+CHR$(2) + "N"+CHR$(6)

      ! EPSOM EOT record (I)
      !
      ! aa + nnnnnn
      !
      REC.CHECK$(8) = "A"+CHR$(2) + "N"+CHR$(6)                         ! DJAL

\     ! CSR list header received (J)                                          !2.9NWB
\     !                                                                       !2.9NWB
\     ! aa + n + nn + nnnnnn + nnnnnn                                         !2.9NWB
\     !                                                                       !2.9NWB
\     REC.CHECK$(9) = "A"+CHR$(2) + "N"+CHR$(1) + "N"+CHR$(2) +        \ DJAL !2.9NWB
\                     "N"+CHR$(6) + "N"+CHR$(4)                        ! DJAL !2.9NWB

\     ! CSR list record received (K)                                          !2.9NWB
\     !                                                                       !2.9NWB
\     ! XU: aa + nn + nnn + nnn                                               !2.9NWB
\     ! XM: aa + nnnnnnn + nnn + a + aaa + aaa                                !2.9NWB
\     ! XO: aa + nnnnnnn + nnn                                                !2.9NWB
\     !                                                                       !2.9NWB
\     REC.CHECK$(10) = "A"+CHR$(2) + "N"+CHR$(2) + "N"+CHR$(3) +       \ DJAL !2.9NWB
\                      "N"+CHR$(3)                                     ! DJAL !2.9NWB
\     REC.CHECK$(11) = "A"+CHR$(2) + "N"+CHR$(7) + "N"+CHR$(3) +       \ DJAL !2.9NWB
\                      "A"+CHR$(1) + "A"+CHR$(3) + "A"+CHR$(3)         ! DJAL !2.9NWB
\     REC.CHECK$(12) = "A"+CHR$(2) + "N"+CHR$(7) + "N"+CHR$(3)         ! DJAL !2.9NWB

\     ! CSR list trailer received (L)                                         !2.9NWB
\     !                                                                       !2.9NWB
\     ! aa + nnn                                                              !2.9NWB
\     !                                                                       !2.9NWB
\     REC.CHECK$(13) = "A"+CHR$(2) + "N"+CHR$(3)                       ! DJAL !2.9NWB

\     ! CSR table request received (M)                                        !2.9NWB
\     !                                                                       !2.9NWB
\     ! aa                                                                    !2.9NWB
\     !                                                                       !2.9NWB
\     REC.CHECK$(14) = "A"+CHR$(2)                                     ! DJAL !2.9NWB

\     ! CSR EOT received (N)                                                  !2.9NWB
\     !                                                                       !2.9NWB
\     ! aa + nnnnnn + nnn                                                     !2.9NWB
\     !                                                                       !2.9NWB
\     REC.CHECK$(15) = "A"+CHR$(2) + "N"+CHR$(6) + "N"+CHR$(3)         ! DJAL !2.9NWB

      ! ASN Carton Header Received (J)                                        !2.9NWB
      !                                                                       !2.9NWB
      ! aa                                                                    !2.9NWB
      !                                                                       !2.9NWB
      REC.CHECK$(9)  = "A"+CHR$(2)                                            !2.9NWB

      ! ASN Automatic Carton Book in (K)                                      !2.9NWB
      !                                                                       !2.9NWB
      ! aa + aaaaaaaaaaaaaa                                                   !2.9NWB
      !                                                                       !2.9NWB
      REC.CHECK$(10)  = "A"+CHR$(2) + "A"+CHR$(14)                            !2.9NWB

      ! ASN Manual Carton Book in (L)                                         !2.9NWB
      !                                                                       !2.9NWB
      ! XM: aa + aaaaaaaaaaaaaa                                               !2.9NWB
      ! XU: aa + aaaaaaaaaaaaa + aaaa                                         !2.9NWB
      ! XO: aa + aaaaa + aaaa                                                 !2.9NWB
      !                                                                       !2.9NWB
      REC.CHECK$(11)  = "A"+CHR$(2) + "A"+CHR$(14)                            !2.9NWB
      REC.CHECK$(12)  = "A"+CHR$(2) + "A"+CHR$(13) + "A"+CHR$(4)              !2.9NWB
      REC.CHECK$(13)  = "A"+CHR$(2) + "A"+CHR$(5) + "A"+CHR$(4)               !2.9NWB

      ! ASN (M)                                                               !2.9NWB
      !                                                                       !2.9NWB
      ! aa                                                                    !2.9NWB
      !                                                                       !2.9NWB
      REC.CHECK$(14)  = "A"+CHR$(2)                                           !2.9NWB

      ! ASN Carton EOT (N)                                                    !2.9NWB
      !                                                                       !2.9NWB
      ! aa + aaaaa                                                            !2.9NWB
      !                                                                       !2.9NWB
      REC.CHECK$(15)  = "A"+CHR$(2) + "A"+CHR$(5)                             !2.9NWB

      ! Price Check header received (O)
      !
      ! aa + nnnnnn
      !
      REC.CHECK$(16) = "A"+CHR$(2) + "N"+CHR$(6)                       ! DJAL

      ! Price check record received (P)
      !
      ! aa + nnnnnnnnnnnnn + aaaaaa
      !
      REC.CHECK$(17) = "A"+CHR$(2) + "N"+CHR$(13) + "A"+CHR$(6)        ! DJAL

      ! Price Check trailer received (Q)
      !
      ! aa + nnnnnn + nnn
      !
      REC.CHECK$(18) = "A"+CHR$(2) + "N"+CHR$(6) + "N"+CHR$(3)        ! DJAL

      ! Check/Request version of program (R)
      !
      ! aa + nnn + nnnnnn
      !
      REC.CHECK$(19) = "A"+CHR$(3) + "N"+CHR$(2) + "N"+CHR$(6)         ! HDS

      ! Program trailer (S)
      !
      ! aa + nnn + nnnnnn + nnnnnn
      !
      REC.CHECK$(20) = "A"+CHR$(5) + "N"+CHR$(6) + "N"+CHR$(5)         ! HDS

      ! Direct order request (T)
      !
      !           aa + nn + nnnnnn + variable portion
      !
      !           The direct order request record is variable length.
      !           PSS37 module 2 sets up the validation string after
      !           determining the length of the LDT record.
      !

      ! Direct file header (U)
      !
      ! aa + nn + nnnnnn + nn
      !
      REC.CHECK$(22) = "A"+CHR$(2) + "N"+CHR$(2) + "N"+CHR$(6) +      \ HDS
                       "N"+CHR$(2)                                    ! HDS

      ! Direct order header (V)
      !
      ! aa + nnnnnn + nnnn + a + nnnn + nnnn + nnnn
      !
      REC.CHECK$(23) = "A"+CHR$(2) + "N"+CHR$(6) + "N"+CHR$(4) +      \ HDS
                       "A"+CHR$(1) + "N"+CHR$(4) + "N"+CHR$(4) +      \ HDS
                       "N"+CHR$(4)                                    ! HDS


      ! Direct order detail (W)
      !
      ! aa + nn + a + nnnnnnnnnnnnn + nnnn + nnnn + nnnn
      !
      REC.CHECK$(24) = "A"+CHR$(2) + "N"+CHR$(2) + "A"+CHR$(1) +      \ HDS
                       "A"+CHR$(13) + "N"+CHR$(4) + "N"+CHR$(4) +     \ HDS
                       "N"+CHR$(4)                                    ! HDS

      ! Direct order trailer (X)
      !
      ! aa
      !
      REC.CHECK$(25) = "A"+CHR$(2)                                    ! HDS

      ! Direct file trailer (Y)
      !
      ! aa + nn + nnnnnn + nn
      !
      REC.CHECK$(26) = "A"+CHR$(2) + "N"+CHR$(2) + "N"+CHR$(6) +      \ HDS
                       "N"+CHR$(2)                                    ! HDS

      ! DIRECT EOT record (Z)
      !
      ! aa
      !
      REC.CHECK$(27) = "A"+CHR$(2)                                    ! HDS

      ! UOD File Header (a)                                           ! LLC
      !
      ! aa + nn + nnnnnn + nnnn
      !
      REC.CHECK$(28) = "A" +CHR$(2) + "N" +CHR$(2) + "N" +CHR$(6) +   \ LLC
                       "N" +CHR$(4)                                   ! LLC

      ! UOD Header (b)                                                ! LLC
      !
      ! aa + nnnnnnnnnnnnnn + a + nnnn                                ! LLC
      !
      REC.CHECK$(29) = "A" +CHR$(2) + "N" +CHR$(14) + "A" +CHR$(1) +  \ LLC
                       "N" +CHR$(4)                                   ! LLC

      ! UOD Detail (c)                                                ! LLC
      !
      ! aa + a + nnnnnnnnnnnnn + nnnn                                 ! LLC
      !
      REC.CHECK$(30) = "A" +CHR$(2) + "A" +CHR$(1) + "N" +CHR$(13) +  \ LLC
                       "N" +CHR$(4)                                   ! LLC

      ! UOD Trailer (d)                                               ! LLC
      !
      ! aa                                                            ! LLC
      !
      REC.CHECK$(31) = "A" +CHR$(2)                                   ! LLC

      ! UOD File Trailer (e)                                          ! LLC
      !
      ! aa + nn + nnnnnn + nnnn                                       ! LLC
      !
      REC.CHECK$(32) = "A" +CHR$(2) + "N" +CHR$(2) + "N" +CHR$(6) +   \ LLC
                       "N" +CHR$(4)                                   ! LLC

      ! UOD EOT (f)                                                   ! LLC
      !
      ! aa                                                            ! LLC
      !
      REC.CHECK$(33) = "A" +CHR$(2)                                   ! LLC

      ! RETURNS FILE ID (g)                                           ! MMJK
      !                                                               ! MMJK
      ! aa + nnnnnnnn + nnnnnnnnnnnn + a                              ! MMJK ! 2.3JAS
      !                                                               ! MMJK
      REC.CHECK$(34) = "A"+CHR$(2) + "N"+CHR$(8) + "N"+CHR$(12) +     \ MMJK ! 2.3JAS
                       "A"+CHR$(1)                                    ! 2.3JAS

      ! RETURNS FILE HEADER (h)                                       ! MMJK
      !                                                               ! MMJK
      ! aa + nnnn                                                     ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(35) = "A"+CHR$(2) + "N"+CHR$(4)                      ! MMJK

      ! RETURNS UOD HEADER (i)                                        ! MMJK
      !                                                               ! MMJK
      ! aa + nnnnnnnn + nnnnnnnnnnnnnn + a + a + nnnn + nnnn + a      ! MMJK
      !    + nnnnnn + nnnnnn + nnnnnn + a + a + aaaaaaaa              ! MMJK
      !    + aaaaaaaaaaaaaaa + aaaaaaaaaaaaaaa + n + n + aaaaaaaa     ! MMJK
      !    + nn + nnnn + n + a + n + n                                ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(36) = "A"+CHR$(2) + "N"+CHR$(8) + "N"+CHR$(14) +     \ MMJK
                       "A"+CHR$(1) + "A"+CHR$(1) + "N"+CHR$(4) +      \ MMJK
                       "N"+CHR$(4) + "A"+CHR$(1) + "N"+CHR$(6) +      \ MMJK
                       "N"+CHR$(6) + "N"+CHR$(6) + "A"+CHR$(1) +      \ MMJK
                       "A"+CHR$(1) + "A"+CHR$(1) + "A"+CHR$(8) +      \ MMJK
                       "A"+CHR$(15) + "A"+CHR$(15) + "N"+CHR$(1) +    \ MMJK
                       "N"+CHR$(1) + "A"+CHR$(8) + "N"+CHR$(2) +      \ MMJK
                       "N"+CHR$(4) + "N"+CHR$(1) + "A"+CHR$(1) +      \ MMJK
                       "N"+CHR$(1) + "N"+CHR$(1)                      ! MMJK

      ! RETURNS ITEM DETAIL (j)                                       ! MMJK
      !                                                               ! MMJK
      ! aa + a + a + nnnnnnnnnnnnn + nnnn                             ! NMJK
      !                                                               ! MMJK
      REC.CHECK$(37) = "A"+CHR$(2) + "A"+CHR$(1) + "A"+CHR$(1) +      \ MMJK
                       "N"+CHR$(13) + "N"+CHR$(4)                     ! NMJK

      ! RETURNS UOD TRAILER (k)                                       ! MMJK
      !                                                               ! MMJK
      ! aa                                                            ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(38) = "A"+CHR$(2)                                    ! MMJK

      ! RETURNS FILE TRAILER (l)                                      ! MMJK
      !                                                               ! MMJK
      ! aa + nnnnnnnnnnnn                                             ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(39) = "A"+CHR$(2) + "N"+CHR$(12)                     ! MMJK

      ! RETURNS FILE REQUEST (m)                                      ! MMJK
      !                                                               ! MMJK
      ! aa + a                                                        ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(40) = "A"+CHR$(2) + "A"+CHR$(1)                      ! MMJK

      ! RETURNS FILE RECEIVED OK (n)                                  ! MMJK
      !                                                               ! MMJK
      ! aa                                                            ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(41) = "A"+CHR$(2)                                    ! MMJK

      ! RETURNS EOT (o)                                               ! MMJK
      !                                                               ! MMJK
      ! aa                                                            ! MMJK
      !                                                               ! MMJK
      REC.CHECK$(42) = "A"+CHR$(2)                                    ! MMJK

      ! STOCKTAKE HEADER (p)                                          ! 1.5
      !                                                               ! 1.5
      ! aa + aaaaaaaaaaaa                                             ! 1.5
      !                                                               ! 1.5
      REC.CHECK$(43) = "A"+CHR$(2) + "A"+CHR$(12)                     ! 1.5

      ! STOCKTAKE DETAIL (q)                                          ! 1.5
      !                                                               ! 1.5
      ! aa + aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa                          ! 1.6
      !                                                               ! 1.5
      REC.CHECK$(44) = "A"+CHR$(2) + "A"+CHR$(30)                     ! 1.6

      ! STOCKTAKE TRAILER (r)                                         ! 1.5
      !                                                               ! 1.5
      ! aa + aaaaaaaaaaaa                                             ! 1.5
      !                                                               ! 1.5
      REC.CHECK$(45) = "A"+CHR$(2) + "A"+CHR$(12)                     ! 1.5

      ! STOCKCOUNT HEADER (s)                                         ! 2.1BG
      !                                                               ! 2.1BG
      ! aa + aaaaaaaaaaaaa + aaaa                                     ! 2.1BG
      !                                                               ! 2.1BG
      REC.CHECK$(46) = "A"+CHR$(2) + "A"+CHR$(13) + "A"+CHR$(4)       ! 2.1BG

      ! STOCKCOUNT DETAIL (t)                                         ! 2.1BG
      !                                                               ! 2.1BG
      ! aa + aaaaaaaaaaaaa + aaaa                                     ! 2.1BG
      !                                                               ! 2.1BG
      REC.CHECK$(47) = "A"+CHR$(2) + "A"+CHR$(13) + "A"+CHR$(4)       ! 2.1BG

      ! STOCKCOUNT TRAILER (u)                                        ! 2.1BG
      !                                                               ! 2.1BG
      ! aa + aaaaaaaaaaaaa + aaaa                                     ! 2.1BG
      !                                                               ! 2.1BG
      REC.CHECK$(48) = "A"+CHR$(2) + "A"+CHR$(13) + "A"+CHR$(4)       ! 2.1BG

      ! STOCKCOUNT EOT (v)                                            ! 2.1BG
      !                                                               ! 2.1BG
      ! aa                                                            ! 2.1BG
      !                                                               ! 2.1BG
      REC.CHECK$(49) = "A"+CHR$(2)                                    ! 2.1BG

      ! PHARMACY HEADER (w)                                           ! 2.2JAS
      !                                                               ! 2.2JAS
      ! aa + aaaa + aaaa + aaaaaa + aaaaaa + a + a                    ! 2.2JAS
      !                                                               ! 2.2JAS
      REC.CHECK$(50) = "A"+CHR$(2) + "A"+CHR$(4) + "A"+CHR$(4) +      \ 2.2JAS
                       "A"+CHR$(6) + "A"+CHR$(6) + "A"+CHR$(1) +      \ 2.2JAS
                       "A"+CHR$(1)                                    ! 2.2JAS

      ! PHARMACY DETAIL (x)                                           ! 2.2JAS
      !                                                               ! 2.2JAS
      ! aa + aaaaaaaaaaaaa + a + aaaaaa + aaaa                        ! 2.2JAS
      !                                                               ! 2.2JAS
      REC.CHECK$(51) = "A"+CHR$(2) + "A"+CHR$(13) + "A"+CHR$(1) +     \ 2.2JAS
                       "A"+CHR$(6) + "A"+CHR$(4)                      ! 2.2JAS

      ! PHARMACY TRAILER (y)                                          ! 2.2JAS
      !                                                               ! 2.2JAS
      ! aa + aaaa                                                     ! 2.2JAS
      !                                                               ! 2.2JAS
      REC.CHECK$(52) = "A"+CHR$(2) + "A"+CHR$(4)                      ! 2.2JAS

      ! PHARMACY EOT (z)                                              ! 2.2JAS
      !                                                               ! 2.2JAS
      ! aa                                                            ! 2.2JAS
      !                                                               ! 2.2JAS
      REC.CHECK$(53) = "A"+CHR$(2)                                    ! 2.2JAS

      ! Price check record received (P)                                                     ! 2.5CS !2.6BG
      ! This is a duplicate of data state P because need two record layouts for the                 !2.6BG
      ! two versions. The original Data State P (type 17) can be replaced with this                 !2.6BG
      ! long term.                                                                                  !2.6BG
      !                                                                                             !2.6BG
      ! aa + nnnnnnnnnnnnn + aaaaaa + aaa + aaa                                             ! 2.5CS !2.6BG
      !                                                                                             !2.6BG
      REC.CHECK$(54) = "A"+CHR$(2) + "N"+CHR$(13) + "A"+CHR$(6) + "A"+CHR$(3) + "A"+CHR$(3) ! 2.5CS !2.6BG
      
      ! RECALL REQUEST (1)                                            ! 2.10BG
      !                                                               ! 2.10BG
      ! aa + aaaaaaaa                                                 ! 2.10BG
      !                                                               ! 2.10BG
      REC.CHECK$(55) = "A"+CHR$(2) + "A"+CHR$(8)                      ! 2.10BG
      
      ! RECALL RECEIVED OK (2)                                        ! 2.10BG
      !                                                               ! 2.10BG
      ! aa                                                            ! 2.10BG
      !                                                               ! 2.10BG
      REC.CHECK$(56) = "A"+CHR$(2)                                    ! 2.10BG
      
      ! RECALL HEADER (3)                                             ! 2.10BG
      !                                                               ! 2.10BG
      ! aa + aaaaaaaa + aaaaaaaaaaaaaa + a                            ! 2.10BG
      !                                                               ! 2.10BG
      REC.CHECK$(57) = "A"+CHR$(2) + "A"+CHR$(8) + "A"+CHR$(14) + \   ! 2.10BG
                       "A"+CHR$(1)                                    ! 2.10BG
      
      ! RECALL DETAIL (4)                                             ! 2.10BG
      !                                                               ! 2.10BG
      ! aa + aaaaaaa + aaaa                                           ! 2.10BG
      !                                                               ! 2.10BG
      REC.CHECK$(58) = "A"+CHR$(2) + "A"+CHR$(7) + "A"+CHR$(4)        ! 2.10BG
      
      ! RECALL TRAILER (5)                                            ! 2.10BG
      !                                                               ! 2.10BG
      ! aa + aaaaa + aaaa                                             ! 2.10BG
      !                                                               ! 2.10BG
      REC.CHECK$(59) = "A"+CHR$(2) + "A"+CHR$(5) + "A"+CHR$(4)        ! 2.10BG
      
      ! RECALL EOT (6)                                                ! 2.10BG
      !                                                               ! 2.10BG
      ! aa + A                                                        ! 2.10BG
      !                                                               ! 2.10BG
      REC.CHECK$(60) = "A"+CHR$(2) + "A"+CHR$(1)                      ! 2.10BG

      ! +UOD recs 
      REC.CHECK$(61) = "A"+CHR$(2) + "N"+CHR$(6)                !"TA" ! 2.12SH
      REC.CHECK$(62) = "A"+CHR$(2)                              !"TF" ! 2.12SH
      REC.CHECK$(63) = "A"+CHR$(2) + "N"+CHR$(3)                !"TI" ! 2.12SH
      REC.CHECK$(64) = "A"+CHR$(2) + "N"+CHR$(28) + "A"+CHR$(1) !"TJ" ! 2.12SH
      REC.CHECK$(65) = "A"+CHR$(2) + "N"+CHR$(8)                !"TK" ! 2.12SH
      REC.CHECK$(66) = "A"+CHR$(2) + "N"+CHR$(8) + "A"+CHR$(1) +\     ! 2.12SH
                       "N"+CHR$(5)                              !"TL" ! 2.12SH
      REC.CHECK$(67) = "A"+CHR$(2)                              !"TZ" ! 2.12SH

      DIM REQ.LIST.STORE$(50),          \ List of requested lists
          REQ.LIST.DATA$(50),           \ Data associated with above lists
          LIST.TRANSMIT$(500),          \ Lists to be transmitted to PDT
          BC.CHECK%(26),                \ Business Centre 'done' flags
          BAR.CODES$(2)                 ! Temp. Bar code store

      GOSUB LOAD.APPLICATION.TABLE                                      ! HDS

      SB.ACTION$ = "O"

      CSR.AUDIT.FILE$ = "CSRAF"                                         ! EPAB
      CSR.AUDIT.SESS.NUM% = 255                                         ! EPAB

      SB.INTEGER% = CSR.AUDIT.SESS.NUM%                                 ! EPAB
      SB.STRING$ = CSR.AUDIT.FILE$                                      ! EPAB
      GOSUB SB.FILE.UTILS                                               ! EPAB
      CSR.AUDIT.SESS.NUM% = SB.FILE.SESS.NUM%                           ! EPAB

      SB.INTEGER% = GAPBF.REPORT.NUM%                                   ! 1.3 !2.5CS !2.6BG
      SB.STRING$ = GAPBF.FILE.NAME$                                     ! 1.3 !2.5CS !2.6BG
      GOSUB SB.FILE.UTILS                                               ! 1.3 !2.5CS !2.6BG
      GAPBF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.3 !2.5CS !2.6BG

      SB.INTEGER% = PLLOL.REPORT.NUM%                                   !2.5CS
      SB.STRING$ = PLLOL.FILE.NAME$                                     !2.5CS
      GOSUB SB.FILE.UTILS                                               !2.5CS
      PLLOL.SESS.NUM% = SB.FILE.SESS.NUM%                               !2.5CS

      SB.INTEGER% = PLLDB.REPORT.NUM%                                   !2.5CS
      SB.STRING$ = PLLDB.FILE.NAME$                                     !2.5CS
      GOSUB SB.FILE.UTILS                                               !2.5CS
      PLLDB.SESS.NUM% = SB.FILE.SESS.NUM%                               !2.5CS

      SB.INTEGER% = BCSMF.REPORT.NUM%
      SB.STRING$ = BCSMF.FILE.NAME$
      GOSUB SB.FILE.UTILS
      BCSMF.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = CCUOD.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCUOD.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCUOD.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CCLAM.REPORT.NUM%                                   ! 1.4
      SB.STRING$ = CCLAM.FILE.NAME$                                     ! 1.4
      GOSUB SB.FILE.UTILS                                               ! 1.4
      CCLAM.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.4

      SB.INTEGER% = EPSOM.REPORT.NUM%                                   ! DJAL
      SB.STRING$ = EPSOM.FILE.NAME$                                     ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
      EPSOM.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DJAL

      SB.INTEGER% = CCITM.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCITM.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCITM.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CCTRL.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCTRL.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCTRL.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CCDMY.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCDMY.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCDMY.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CCTMP.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCTMP.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCTMP.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CCBUF.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCBUF.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCBUF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CCUPF.REPORT.NUM%                                   ! NMJK
      SB.STRING$ = CCUPF.FILE.NAME$                                     ! NMJK
      GOSUB SB.FILE.UTILS                                               ! NMJK
      CCUPF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! NMJK

      SB.INTEGER% = CCWKF.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = CCWKF.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      CCWKF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = LDTAF.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = LDTAF.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      LDTAF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = CHKBF.REPORT.NUM%                                   ! DSW
      SB.STRING$ = CHKBF.FILE.NAME$                                     ! DSW
      GOSUB SB.FILE.UTILS                                               ! DSW
      CHKBF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DSW

\     SB.INTEGER% = CSR.REPORT.NUM%                                     ! DJAL!2.9NWB
\     SB.STRING$ = CSR.FILE.NAME$                                       ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
\     CSR.SESS.NUM% = SB.FILE.SESS.NUM%                                 ! DJAL!2.9NWB

\     SB.INTEGER% = CITEM.REPORT.NUM%                                   ! DJAL!2.9NWB
\     SB.STRING$ = CITEM.FILE.NAME$                                     ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
\     CITEM.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DJAL!2.9NWB

\     SB.INTEGER% = CSRBF.REPORT.NUM%                                   ! DJAL!2.9NWB
\     SB.STRING$ = CSRBF.FILE.NAME$                                     ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
\     CSRBF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DJAL!2.9NWB

\     SB.INTEGER% = CSRWF.REPORT.NUM%                                   ! DJAL!2.9NWB
\     SB.STRING$ = CSRWF.FILE.NAME$                                     ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
\     CSRWF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DJAL!2.9NWB

!     SB.INTEGER% = FPF.REPORT.NUM%                                     ! DJAL
!     SB.STRING$ = FPF.FILE.NAME$                                       ! DJAL
!     GOSUB SB.FILE.UTILS                                               ! DJAL
!     FPF.SESS.NUM% = SB.FILE.SESS.NUM%                                 ! DJAL

      SB.INTEGER% = IDF.REPORT.NUM%
      SB.STRING$ = IDF.FILE.NAME$
      GOSUB SB.FILE.UTILS
      IDF.SESS.NUM% = SB.FILE.SESS.NUM%


      SB.INTEGER% = IEF.REPORT.NUM%
      SB.STRING$ = IEF.FILE.NAME$
      GOSUB SB.FILE.UTILS
      IEF.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = IRF.REPORT.NUM%
      SB.STRING$ = IRF.FILE.NAME$
      GOSUB SB.FILE.UTILS
      IRF.SESS.NUM% = SB.FILE.SESS.NUM%

!     SB.INTEGER% = ONORD.REPORT.NUM%                                   ! DJAL
!     SB.STRING$ = ONORD.FILE.NAME$                                     ! DJAL
!     GOSUB SB.FILE.UTILS                                               ! DJAL
!     ONORD.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DJAL

      SB.INTEGER% = PCHK.REPORT.NUM%                                    ! DJAL
      SB.STRING$ = PCHK.FILE.NAME$                                      ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
      PCHK.SESS.NUM% = SB.FILE.SESS.NUM%                                ! DJAL

      SB.INTEGER% = PDTWF.REPORT.NUM%
      SB.STRING$ = PDTWF.FILE.NAME$
      GOSUB SB.FILE.UTILS
      PDTWF.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = PIITM.REPORT.NUM%
      SB.STRING$ = PIITM.FILE.NAME$
      GOSUB SB.FILE.UTILS
      PIITM.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = PILST.REPORT.NUM%
      SB.STRING$ = PILST.FILE.NAME$
      GOSUB SB.FILE.UTILS
      PILST.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = PIPEI.REPORT.NUM%
      SB.STRING$ = PIPEI.FILE.NAME$
      GOSUB SB.FILE.UTILS
      PIPEI.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = PIPEO.REPORT.NUM%
      SB.STRING$ = PIPEO.FILE.NAME$
      GOSUB SB.FILE.UTILS
      PIPEO.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = SOFTS.REPORT.NUM%                                   ! MMJK
      SB.STRING$ = SOFTS.FILE.NAME$                                     ! MMJK
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SOFTS.SESS.NUM% = SB.FILE.SESS.NUM%                               ! MMJK

      SB.INTEGER% = STKMQ.REPORT.NUM%
      SB.STRING$ = STKMQ.FILE.NAME$
      GOSUB SB.FILE.UTILS
      STKMQ.SESS.NUM% = SB.FILE.SESS.NUM%

      SB.INTEGER% = UNITS.REPORT.NUM%                                   ! DJAL
      SB.STRING$ = UNITS.FILE.NAME$                                     ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
      UNITS.SESS.NUM% = SB.FILE.SESS.NUM%                               ! DJAL

      SB.INTEGER% = DIRORD.REPORT.NUM%                                  ! HDS
      SB.STRING$ = DIRORD.FILE.NAME$                                    ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      DIRORD.SESS.NUM% = SB.FILE.SESS.NUM%                              ! HDS

      SB.INTEGER% = DIRSUP.REPORT.NUM%                                  ! HDS
      SB.STRING$ = DIRSUP.FILE.NAME$                                    ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      DIRSUP.SESS.NUM% = SB.FILE.SESS.NUM%                              ! HDS

      SB.INTEGER% = DIRWF.REPORT.NUM%                                   ! HDS
      SB.STRING$ = DIRWF.FILE.NAME$                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      DIRWF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! HDS

      SB.INTEGER% = DIREC.REPORT.NUM%                                   ! HDS
      SB.STRING$ = DIREC.FILE.NAME$                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      DIREC.SESS.NUM% = SB.FILE.SESS.NUM%                               ! HDS

      SB.INTEGER% = LDTCF.REPORT.NUM%                                   ! HDS
      SB.STRING$ = LDTCF.FILE.NAME$                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      LDTCF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! HDS

      SB.INTEGER% = PLDT.REPORT.NUM%                                    ! HDS
      SB.STRING$ = PLDT.FILE.NAME$                                      ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      PLDT.SESS.NUM% = SB.FILE.SESS.NUM%                                ! HDS

      SB.INTEGER% = DRSMQ.REPORT.NUM%                                   ! HDS
      SB.STRING$ = DRSMQ.FILE.NAME$                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      DRSMQ.SESS.NUM% = SB.FILE.SESS.NUM%                               ! HDS

      SB.INTEGER% = LDTBF.REPORT.NUM%                                   ! JLC
      SB.STRING$ = LDTBF.FILE.NAME$                                     ! JLC
      GOSUB SB.FILE.UTILS                                               ! JLC
      LDTBF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! JLC

!     SB.INTEGER% = IDSOF.REPORT.NUM%                                   ! JLC
!     SB.STRING$ = IDSOF.FILE.NAME$                                     ! JLC
!     GOSUB SB.FILE.UTILS                                               ! JLC
!     IDSOF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! JLC

      SB.INTEGER% = UOD.REPORT.NUM%                                     ! LLC
      SB.STRING$ = UOD.FILE.NAME$                                       ! LLC
      GOSUB SB.FILE.UTILS                                               ! LLC
      UOD.SESS.NUM% = SB.FILE.SESS.NUM%                                 ! LLC

      SB.INTEGER% = UODBF.REPORT.NUM%                                   ! LLC
      SB.STRING$ = UODBF.FILE.NAME$                                     ! LLC
      GOSUB SB.FILE.UTILS                                               ! LLC
      UODBF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! LLC

      SB.INTEGER% = UODTF.REPORT.NUM%                                   ! LLC
      SB.STRING$ = UODTF.FILE.NAME$                                     ! LLC
      GOSUB SB.FILE.UTILS                                               ! LLC
      UODTF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! LLC

      SB.INTEGER% = SXTCF.REPORT.NUM%                                   ! 1.5
      SB.STRING$ = SXTCF.FILE.NAME$                                     ! 1.5
      GOSUB SB.FILE.UTILS                                               ! 1.5
      SXTCF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.5

      SB.INTEGER% = STKBF.REPORT.NUM%                                   ! 1.5
      SB.STRING$ = STKBF.FILE.NAME$                                     ! 1.5
      GOSUB SB.FILE.UTILS                                               ! 1.5
      STKBF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.5

      SB.INTEGER% = SXTMP.REPORT.NUM%                                   ! 1.5
      SB.STRING$ = SXTMP.FILE.NAME$                                     ! 1.5
      GOSUB SB.FILE.UTILS                                               ! 1.5
      SXTMP.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.5

      SB.INTEGER% = STKTK.REPORT.NUM%                                   ! 1.5
      SB.STRING$ = STKTK.FILE.NAME$                                     ! 1.5
      GOSUB SB.FILE.UTILS                                               ! 1.5
      STKTK.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.5

      SB.INTEGER% = STLDT.REPORT.NUM%                                   ! 1.7
      SB.STRING$ = STLDT.FILE.NAME$                                     ! 1.7
      GOSUB SB.FILE.UTILS                                               ! 1.7
      STLDT.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.7

!      LSSST.FILE.NAME$ = "C:\LSSST" + MONITORED.PORT$ + ".BIN"          !1.8BG !2.7CS
!      SB.INTEGER% = LSSST.REPORT.NUM%                                   !1.8BG !2.7CS
!      SB.STRING$ = LSSST.FILE.NAME$                                     !1.8BG !2.7CS
!      GOSUB SB.FILE.UTILS                                               !1.8BG !2.7CS
!      LSSST.SESS.NUM% = SB.FILE.SESS.NUM%                               !1.8BG !2.7CS

      SB.INTEGER% = STOCK.REPORT.NUM%                                   !1.8BG
      SB.STRING$ = STOCK.FILE.NAME$                                     !1.8BG
      GOSUB SB.FILE.UTILS                                               !1.8BG
      STOCK.SESS.NUM% = SB.FILE.SESS.NUM%                               !1.8BG

      SB.INTEGER% = IMSTC.REPORT.NUM%                                   !1.8BG
      SB.STRING$ = IMSTC.FILE.NAME$                                     !1.8BG
      GOSUB SB.FILE.UTILS                                               !1.8BG
      IMSTC.SESS.NUM% = SB.FILE.SESS.NUM%                               !1.8BG

      SB.INTEGER% = CB.REPORT.NUM%                                      ! 2.9NWB
      SB.STRING$ = CB.FILE.NAME$                                        ! 2.9NWB
      GOSUB SB.FILE.UTILS                                               ! 2.9NWB
      CB.SESS.NUM% = SB.FILE.SESS.NUM%                                  ! 2.9NWB
      
      SB.INTEGER% = RB.REPORT.NUM%                                      ! 2.10BG
      SB.STRING$ = RB.FILE.NAME$                                        ! 2.10BG
      GOSUB SB.FILE.UTILS                                               ! 2.10BG
      RB.SESS.NUM% = SB.FILE.SESS.NUM%                                  ! 2.10BG
      
      SB.INTEGER% = REWKF.REPORT.NUM%                                   ! 2.10BG
      SB.STRING$ = REWKF.FILE.NAME$                                     ! 2.10BG
      GOSUB SB.FILE.UTILS                                               ! 2.10BG
      REWKF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.10BG
      
      SB.INTEGER% = RECALLS.REPORT.NUM%                                 ! 2.10BG
      SB.STRING$ = RECALLS.FILE.NAME$                                   ! 2.10BG
      GOSUB SB.FILE.UTILS                                               ! 2.10BG
      RECALLS.SESS.NUM% = SB.FILE.SESS.NUM%                             ! 2.10BG

      SB.INTEGER% = DELVINDX.REPORT.NUM%                                ! 2.12SH
      SB.STRING$ = DELVINDX.FILE.NAME$                                  ! 2.12SH
      GOSUB SB.FILE.UTILS                                               ! 2.12SH
      DELVINDX.SESS.NUM% = SB.FILE.SESS.NUM%                            ! 2.12SH

      SB.INTEGER% = AF.REPORT.NUM%                                      ! 2.12SH
      SB.STRING$ = AF.FILE.NAME$                                        ! 2.12SH
      GOSUB SB.FILE.UTILS                                               ! 2.12SH
      AF.SESS.NUM% = SB.FILE.SESS.NUM%                                  ! 2.12SH

      SB.INTEGER% = UODOT.REPORT.NUM%                                   ! 2.12SH
      SB.STRING$ = UODOT.FILE.NAME$                                     ! 2.12SH
      GOSUB SB.FILE.UTILS                                               ! 2.12SH
      UODOT.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.12SH

      SB.INTEGER% = UB.REPORT.NUM%                                      ! 2.12SH
      SB.STRING$ = UB.FILE.NAME$                                        ! 2.12SH
      GOSUB SB.FILE.UTILS                                               ! 2.12SH
      UB.SESS.NUM% = SB.FILE.SESS.NUM%                                  ! 2.12SH
      
      UB.OPEN.FLAG$ = "N"                                               ! 2.12SH
      AF.OPEN.FLAG$ = "N"                                               ! 2.12SH
      DELVINDX.OPEN.FLAG$ = "N"                                         ! 2.12SH
      UODOT.OPEN.FLAG$ = "N"                                            ! 2.12SH
      RECALLS.OPEN.FLAG$ = "N"                                          ! 2.10BG
      REWKF.OPEN.FLAG$ = "N"                                            ! 2.10BG
      RB.OPEN.FLAG$ = "N"                                               ! 2.10BG
      CB.OPEN.FLAG$ = "N"                                               ! 2.9NWB
      CCBUF.OPEN.FLAG$ = "N"                                            ! MMJK
      CCDMY.OPEN.FLAG$ = "N"                                            ! MMJK
      CCITM.OPEN.FLAG$ = "N"                                            ! MMJK
      CCLAM.OPEN.FLAG$ = "N"                                            ! 1.4
      CCREJ.OPEN.FLAG$ = "N"                                            ! 2.4JAS
      CCTMP.OPEN.FLAG$ = "N"                                            ! MMJK
      CCTRL.OPEN.FLAG$ = "N"                                            ! MMJK
      CCUOD.OPEN.FLAG$ = "N"                                            ! MMJK
      CCUPF.OPEN.FLAG$ = "N"                                            ! NMJK
      CCWKF.OPEN.FLAG$ = "N"                                            ! MMJK
      CHKBF.OPEN.FLAG$ = "N"                                            ! DSW
\     CITEM.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB
      CSR.AUDIT.OPEN.FLAG$ = "N"                                        ! EPAB
\     CSR.OPEN.FLAG$ = "N"                                              ! DSW !2.9NWB
\     CSRBF.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB
\     CSRWF.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB
      DIREC.OPEN.FLAG$ = "N"                                            ! HDS
      DIRORD.OPEN.FLAG$ = "N"                                           ! HDS
      DIRSUP.OPEN.FLAG$ = "N"                                           ! HDS
      DIRWF.OPEN.FLAG$ = "N"                                            ! HDS
      DRSMQ.OPEN.FLAG$ = "N"                                            ! HDS
!     FPF.OPEN.FLAG$ = "N"                                              ! DSW
      GAPBF.OPEN.FLAG$ = "N"                                            ! 1.3 !2.5CS !2.6BG
      IDF.OPEN.FLAG$ = "N"                                              ! DSW
!     IDSOF.OPEN.FLAG$ = "N"                                            ! JLC
      LDTAF.OPEN.FLAG$ = "N"                                            ! MMJK
      LDTBF.OPEN.FLAG$ = "N"                                            ! JLC
      LDTCF.OPEN.FLAG$ = "N"                                            ! HDS
      LOCCNT.OPEN.FLAG$ = "N"                                           ! 1.9DA
!     LSSST.OPEN.FLAG$ = "N"                                            !1.8BG !2.7CS
!     ONORD.OPEN.FLAG$ = "N"                                            ! DSW
      PCHK.OPEN.FLAG$ = "N"                                             ! DSW
      PLLDB.OPEN.FLAG$ = "N"                                            ! 2.5CS
      PLLOL.OPEN.FLAG$ = "N"                                            ! 2.5CS
      SOFTS.OPEN.FLAG$ = "N"                                            ! MMJK
      SOPTS.OPEN.FLAG$ = "N"                                            ! 2.3JAS
!     STKCF.OPEN.FLAG$ = "N"                                            ! 1.9DA
      STKDC.OPEN.FLAG$ = "N"                                            ! 1.9DA
      STKEX.OPEN.FLAG$ = "N"                                            ! 1.9DA
      STKIF.OPEN.FLAG$ = "N"                                            ! 1.9DA
      STKMF.OPEN.FLAG$ = "N"                                            ! 1.9DA
      STKRC.OPEN.FLAG$ = "N"                                            ! 1.9DA
      STKTF.OPEN.FLAG$ = "N"                                            ! 1.9DA
      TSF.OPEN.FLAG$ = "N"                                              ! 2.3JAS
      UNITS.OPEN.FLAG$ = "N"                                            ! DSW
      UOD.OPEN.FLAG$ = "N"                                              ! LLC
      UODBF.OPEN.FLAG$ = "N"                                            ! LLC
      UODTF.OPEN.FLAG$ = "N"                                            ! LLC
      XGCF.OPEN.FLAG$ = "N"                                             ! 1.9DA

      TIMEOUT.VALUE% = 2                ! Seconds delay to cause PDT timeout
      INT.DELAY% = 10                   ! Seconds delay between PSS38 checks
      INACTIVITY.SHUTDOWN% = VAL(RIGHT$("37INACT:0030", 4))             ! DSW
                                        ! Seconds inactivity before shutdown
                                        ! Load module is editable
      LOG.ON.DISABLE% = 8               ! Min. Seconds between logons   ! MDS
      STKMQ.RECORD.DELIMITER$ = CHR$(34)!
      STKMQ.FIELD.DELIMITER$ = CHR$(59) ! STKMQ delimiters
      STKMQ.ENDREC.MARKER$ = CRLF$      !
      CCTMP.RECORD.DELIMITER$ = CHR$(34)                                ! MMJK
      CCTMP.FIELD.DELIMITER$ = CHR$(59)                                 ! MMJK
      CCTMP.ENDREC.MARKER$ = CRLF$                                      ! MMJK
      CCREJ.ENDREC.MARKER$ = CRLF$                                      ! 2.4JAS

      CURR.SESS.NUM% = PIPEI.SESS.NUM%
      OPEN PIPEI.FILE.NAME$ AS PIPEI.SESS.NUM% BUFFSIZE 518
      CURR.SESS.NUM% = PIPEO.SESS.NUM%
      OPEN PIPEO.FILE.NAME$ AS PIPEO.SESS.NUM% BUFFSIZE 518
      CURR.SESS.NUM% = PLDT.SESS.NUM%                                   ! HDS
      OPEN PLDT.FILE.NAME$ AS PLDT.SESS.NUM% BUFFSIZE 518               ! HDS

                                                          ! 1.2


\     IF NOT WARM.START THEN BEGIN                                      ! ILC !2.9NWB
\        ALLOW.CSR.PROCESSING = TRUE                                    ! 1.2 !2.9NWB
\        CURR.SESS.NUM% = SOFTS.SESS.NUM%                               ! 1.2 !2.9NWB
\        IF END #SOFTS.SESS.NUM% THEN OPEN.ERROR                        ! 1.2 !2.9NWB
\        OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL%                  \ 1.2 !2.9NWB
\             AS SOFTS.SESS.NUM% NOWRITE NODEL                          ! 1.2 !2.9NWB
\        SOFTS.OPEN.FLAG$ = "Y"                                         ! 1.2 !2.9NWB
\        SOFTS.REC.NUM% = 6                                             ! 1.2 !2.9NWB
\        IF READ.SOFTS = 0 THEN BEGIN                                   ! 1.2 !2.9NWB
\           IF (LEFT$(SOFTS.RECORD$,11) = "CSR PHASE 2") THEN BEGIN ! 1.2     !2.9NWB
\              ALLOW.CSR.PROCESSING = FALSE                             ! 1.2 !2.9NWB
\           ENDIF                                                       ! 1.2 !2.9NWB
\        ENDIF ELSE ALLOW.CSR.PROCESSING = FALSE                        ! 1.2 !2.9NWB
\        CLOSE SOFTS.SESS.NUM%                                          ! 1.2 !2.9NWB
\        SOFTS.OPEN.FLAG$ = "N"                                         ! 1.2 !2.9NWB

\        IF (NOT ALLOW.CSR.PROCESSING) THEN BEGIN                       ! 1.2 !2.9NWB
\           GOTO BYPASS.CSR.PROCESSING                                  ! 1.2 !2.9NWB
\        ENDIF                                                                !2.9NWB

\        LOCATION$ = "STARTUP"                                                !2.9NWB
\        GOSUB STAGGER.PORT                                             ! DSW !2.9NWB
\        CURR.SESS.NUM% = CSR.SESS.NUM%                                 ! DSW !2.9NWB
\        IF END# CSR.SESS.NUM% THEN OPEN.ERROR                          ! DSW !2.9NWB
\        OPEN CSR.FILE.NAME$ AS CSR.SESS.NUM% LOCKED                    ! DSW !2.9NWB
\        CSR.OPEN.FLAG$ = "Y"                                           ! DSW !2.9NWB
\        CSRWF.EXISTS% = SIZE(CSRWF.FILE.NAME$)                         ! DSW !2.9NWB
\        IF CSRWF.EXISTS% > 0 THEN BEGIN                                ! DSW !2.9NWB
\           SB.MESSAGE$ = "PDT Support - Processing CSR work file"      ! DSW !2.9NWB
\           GOSUB SB.BG.MESSAGE                                         ! DSW !2.9NWB
\           PROCESS.CSR.WORKFILE$ = "Y"                                 ! DSW !2.9NWB
\           GOSUB ALLOCATE.MODULE.1                                     !2.0DA!2.9NWB
\           CALL PSS3701                                                ! DSW !2.9NWB
\           GOSUB DEALLOCATE.MODULE.1                                   !2.0DA!2.9NWB
\           IF CITEM.OPEN.FLAG$ = "Y" THEN BEGIN                        ! DSW !2.9NWB
\              CLOSE CITEM.SESS.NUM%                                    ! DSW !2.9NWB
\              CITEM.OPEN.FLAG$ = "N"                                   ! DSW !2.9NWB
\           ENDIF                                                       ! DSW !2.9NWB
\        ENDIF                                                          ! DSW !2.9NWB

\        GOSUB ZEROISE.CITEM                                            ! FSW !2.9NWB
\     ENDIF                                                                   !2.9NWB

\ BYPASS.CSR.PROCESSING:                                                !1.2  !2.9NWB

\     IF CSR.OPEN.FLAG$ = "Y" THEN BEGIN                                ! DSW !2.9NWB
\        CLOSE CSR.SESS.NUM%                                            ! DSW !2.9NWB
\        CSR.OPEN.FLAG$ = "N"                                           ! DSW !2.9NWB
\     ENDIF                                                             ! DSW !2.9NWB

   IGNORE.LOCKED.FILE:                                                  ! DSW

      CURR.SESS.NUM% = SOFTS.SESS.NUM%                                  ! 2.9NWB
      IF END #SOFTS.SESS.NUM% THEN OPEN.ERROR                           ! 2.9NWB
      OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL%                     \ 2.9NWB
           AS SOFTS.SESS.NUM% NOWRITE NODEL                             ! 2.9NWB
      SOFTS.OPEN.FLAG$ = "Y"                                            ! 2.9NWB
      SOFTS.REC.NUM% = 50                                               ! 2.9NWB
      ASN.ACTIVE% = FALSE                                               ! 2.9NWB
      IF READ.SOFTS = 0 THEN BEGIN                                      ! 2.9NWB
         IF MATCH("INACTIVE", SOFTS.RECORD$, 1) = 0 THEN BEGIN          ! 2.9NWB
            ASN.ACTIVE% = TRUE                                          ! 2.9NWB
         ENDIF                                                          ! 2.9NWB
      ENDIF                                                             ! 2.9NWB
      CLOSE SOFTS.SESS.NUM%                                             ! 2.9NWB
      SOFTS.OPEN.FLAG$ = "N"                                            ! 2.9NWB

      CURR.SESS.NUM% = DRSMQ.SESS.NUM%                                  ! HDS
      IF END# DRSMQ.SESS.NUM% THEN DRSMQ.NOT.FOUND                      ! HDS
      OPEN DRSMQ.FILE.NAME$ AS DRSMQ.SESS.NUM% NODEL                    ! HDS
      DRSMQ.OPEN.FLAG$ = "Y"                                            ! HDS

   DRSMQ.CONTINUE:                                                      ! HDS

      CLOSE DRSMQ.SESS.NUM%                                             ! HDS
      DRSMQ.OPEN.FLAG$ = "N"                                            ! HDS

      LOCATION$ = "RUNNING"                                             ! DSW
      SB.MESSAGE$ = "PDT Support - Waiting for PSS38"                   ! DSW
      GOSUB SB.BG.MESSAGE                                               ! DSW

      CR$ = CHR$(13)                                                    !1.9DA

      DIM BUSINESS.CENTRES$(26)                                         !1.9DA
      DIM CONCEPT.GROUPS$(99)                                           !1.9DA
      DIM PRODUCT.GROUPS$(1000)                                         !1.9DA

   RETURN

ALLOCATE.MODULE.1:

      SB.ACTION$ = "O"                                                  !1.9DA

\     SB.INTEGER% = CIMF.REPORT.NUM%                                    !DJAL !2.9NWB
\     SB.STRING$ = CIMF.FILE.NAME$                                      !DJAL !2.9NWB
\     GOSUB SB.FILE.UTILS                                               !DJAL !2.9NWB
\     CIMF.SESS.NUM% = SB.FILE.SESS.NUM%                                !DJAL !2.9NWB

\     SB.INTEGER% = FPF.REPORT.NUM%                                     !2.0DA!2.9NWB
\     SB.STRING$ = FPF.FILE.NAME$                                       !2.0DA!2.9NWB
\     GOSUB SB.FILE.UTILS                                               !2.0DA!2.9NWB
\     FPF.SESS.NUM% = SB.FILE.SESS.NUM%                                 !2.0DA!2.9NWB

\     SB.INTEGER% = ONORD.REPORT.NUM%                                   !2.0DA!2.9NWB
\     SB.STRING$ = ONORD.FILE.NAME$                                     !2.0DA!2.9NWB
\     GOSUB SB.FILE.UTILS                                               !2.0DA!2.9NWB
\     ONORD.SESS.NUM% = SB.FILE.SESS.NUM%                               !2.0DA!2.9NWB

\     SB.INTEGER% = IDSOF.REPORT.NUM%                                   !2.0DA!2.9NWB
\     SB.STRING$ = IDSOF.FILE.NAME$                                     !2.0DA!2.9NWB
\     GOSUB SB.FILE.UTILS                                               !2.0DA!2.9NWB
\     IDSOF.SESS.NUM% = SB.FILE.SESS.NUM%                               !2.0DA!2.9NWB

\     CIMF.OPEN.FLAG$ = "N"                                             ! DSW !2.9NWB
\     FPF.OPEN.FLAG$ = "N"                                              !2.0DA!2.9NWB
\     ONORD.OPEN.FLAG$ = "N"                                            !2.0DA!2.9NWB
\     IDSOF.OPEN.FLAG$ = "N"                                            !2.0DA!2.9NWB

RETURN

DEALLOCATE.MODULE.1:

      SB.ACTION$ = "C"                                                  !1.9DA
      SB.STRING$ = ""                                                   !1.9DA

\     IF CIMF.OPEN.FLAG$ = "Y" THEN BEGIN                               !1.9DA!2.9NWB
\        CLOSE CIMF.SESS.NUM%                                           !1.9DA!2.9NWB
\        SB.INTEGER% = CIMF.SESS.NUM%                                   !2.7CS!2.9NWB
\        GOSUB SB.FILE.UTILS                                            !2.7CS!2.9NWB
\        CIMF.OPEN.FLAG$ = "N"                                          !1.9DA!2.9NWB
\     ENDIF                                                             !1.9DA!2.9NWB

\     IF FPF.OPEN.FLAG$ = "Y" THEN BEGIN                                !2.0DA!2.9NWB
\        CLOSE FPF.SESS.NUM%                                            !2.0DA!2.9NWB
\        SB.INTEGER% = FPF.SESS.NUM%                                    !2.7CS!2.9NWB
\        GOSUB SB.FILE.UTILS                                            !2.7CS!2.9NWB
\        FPF.OPEN.FLAG$ = "N"                                           !2.0DA!2.9NWB
\     ENDIF                                                             !2.0DA!2.9NWB

\     IF ONORD.OPEN.FLAG$ = "Y" THEN BEGIN                              !2.0DA!2.9NWB
\        CLOSE ONORD.SESS.NUM%                                          !2.0DA!2.9NWB
\        SB.INTEGER% = ONORD.SESS.NUM%                                  !2.7CS!2.9NWB
\        GOSUB SB.FILE.UTILS                                            !2.7CS!2.9NWB
\        ONORD.OPEN.FLAG$ = "N"                                         !2.0DA!2.9NWB
\     ENDIF                                                             !2.0DA!2.9NWB

\     IF IDSOF.OPEN.FLAG$ = "Y" THEN BEGIN                              !2.0DA!2.9NWB
\        CLOSE IDSOF.SESS.NUM%                                          !2.0DA!2.9NWB
\        SB.INTEGER% = IDSOF.SESS.NUM%                                  !2.7CS!2.9NWB
\        GOSUB SB.FILE.UTILS                                            !2.7CS!2.9NWB
\        IDSOF.OPEN.FLAG$ = "N"                                         !2.0DA!2.9NWB
\     ENDIF                                                             !2.0DA!2.9NWB

\     SB.INTEGER% = CIMF.SESS.NUM%                                      ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB

\     SB.INTEGER% = FPF.SESS.NUM%                                       !2.0DA!2.9NWB
\     GOSUB SB.FILE.UTILS                                               !2.0DA!2.9NWB

\     SB.INTEGER% = ONORD.SESS.NUM%                                     !2.0DA!2.9NWB
\     GOSUB SB.FILE.UTILS                                               !2.0DA!2.9NWB

\     SB.INTEGER% = IDSOF.SESS.NUM%                                     !2.0DA!2.9NWB
\     GOSUB SB.FILE.UTILS                                               !2.0DA!2.9NWB

RETURN

ALLOCATE.MODULE.3:


RETURN

DEALLOCATE.MODULE.3:


RETURN

ALLOCATE.MODULE.4:

      SB.ACTION$ = "O"                                                  ! 1.9DA

!     SB.INTEGER% = STKCF.SESS.NUM%                                     ! 1.9DA
!     SB.STRING$ = STKCF.FILE.NAME$                                     ! 1.9DA
!     GOSUB SB.FILE.UTILS                                               ! 1.9DA
!     STKCF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

      IF APPLICATION.NO$ = "08" THEN BEGIN                              ! 2.3JAS Stocktake application

         SB.INTEGER% = STKMF.REPORT.NUM%                                   ! 1.9DA  ! 2.7CS
         SB.STRING$ = STKMF.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKMF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

         SB.INTEGER% = STKRC.REPORT.NUM%                                   ! 1.9DA  ! 2.7CS
         SB.STRING$ = STKRC.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKRC.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

         SB.INTEGER% = XGCF.REPORT.NUM%                                    ! 1.9DA  ! 2.7CS
         SB.STRING$ = XGCF.FILE.NAME$                                      ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         XGCF.SESS.NUM% = SB.FILE.SESS.NUM%                                ! 1.9DA

         SB.INTEGER% = STKEX.REPORT.NUM%                                   ! 1.9DA  ! 2.7CS
         SB.STRING$ = STKEX.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKEX.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

         SB.INTEGER% = STKIF.REPORT.NUM%                                   ! 1.9DA   ! 2.7CS
         SB.STRING$ = STKIF.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKIF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

         SB.INTEGER% = LOCCNT.REPORT.NUM%                                  ! 1.9DA  ! 2.7CS
         SB.STRING$ = LOCCNT.FILE.NAME$                                    ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         LOCCNT.SESS.NUM% = SB.FILE.SESS.NUM%                              ! 1.9DA

         SB.INTEGER% = STKIG.REPORT.NUM%                                   ! 1.9DA  ! 2.7CS
         SB.STRING$ = STKIG.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKIG.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

         SB.INTEGER% = STKTF.REPORT.NUM%                                   ! 1.9DA  ! 2.7CS
         SB.STRING$ = STKTF.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKTF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

         SB.INTEGER% = STKDC.REPORT.NUM%                                   ! 1.9DA  ! 2.7CS
         SB.STRING$ = STKDC.FILE.NAME$                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA
         STKDC.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 1.9DA

!     STKCF.OPEN.FLAG$ = "N"                                            ! 1.9DA
         STKMF.OPEN.FLAG$ = "N"                                            ! 1.9DA

   ENDIF ELSE BEGIN                                                     ! 2.3JAS Returns application

      SB.INTEGER% = SOPTS.REPORT.NUM%                                   ! 2.3JAS  ! 2.7CS
      SB.STRING$ = SOPTS.FILE.NAME$                                     ! 2.3JAS
      GOSUB SB.FILE.UTILS                                               ! 2.3JAS
      SOPTS.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.3JAS

      SB.INTEGER% = LOCAL.REPORT.NUM%                                   ! 2.3JAS  ! 2.7CS
      SB.STRING$ = LOCAL.FILE.NAME$                                     ! 2.3JAS
      GOSUB SB.FILE.UTILS                                               ! 2.3JAS
      LOCAL.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.3JAS

      SB.INTEGER% = CCREJ.REPORT.NUM%                                   ! 2.4JAS  ! 2.7CS
      SB.STRING$ = CCREJ.FILE.NAME$                                     ! 2.4JAS
      GOSUB SB.FILE.UTILS                                               ! 2.4JAS
      CCREJ.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.4JAS

   ENDIF                                                                ! 2.3JAS

RETURN

DEALLOCATE.MODULE.4:

      SB.ACTION$ = "C"                                                  ! 1.9DA
      SB.STRING$ = ""                                                   ! 1.9DA

!     IF STKCF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
!        CLOSE STKCF.SESS.NUM%                                          ! 1.9DA
!        STKCF.OPEN.FLAG$ = "N"                                         ! 1.9DA
!     ENDIF                                                             ! 1.9DA

      IF SOPTS.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 2.3JAS
         CLOSE SOPTS.SESS.NUM%                                          ! 2.3JAS
         SOPTS.OPEN.FLAG$ = "N"                                         ! 2.3JAS
      ENDIF                                                             ! 2.3JAS

      IF LOCAL.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 2.3JAS
         CLOSE LOCAL.SESS.NUM%                                          ! 2.3JAS
         LOCAL.OPEN.FLAG$ = "N"                                         ! 2.3JAS
      ENDIF                                                             ! 2.3JAS

      IF CCREJ.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 2.4JAS
         CLOSE CCREJ.SESS.NUM%                                          ! 2.4JAS
         CCREJ.OPEN.FLAG$ = "N"                                         ! 2.4JAS
      ENDIF                                                             ! 2.4JAS

      IF STKMF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
         CLOSE STKMF.SESS.NUM%                                          ! 1.9DA
         STKMF.OPEN.FLAG$ = "N"                                         ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF STKRC.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
         CLOSE STKRC.SESS.NUM%                                          ! 1.9DA
         STKRC.OPEN.FLAG$ = "N"                                         ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF XGCF.OPEN.FLAG$ = "Y" THEN BEGIN                               ! 1.9DA
         CLOSE XGCF.SESS.NUM%                                           ! 1.9DA
         XGCF.OPEN.FLAG$ = "N"                                          ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF STKEX.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
         CLOSE STKEX.SESS.NUM%                                          ! 1.9DA
         STKEX.OPEN.FLAG$ = "N"                                         ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF STKIF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
         CLOSE STKIF.SESS.NUM%                                          ! 1.9DA
         STKIF.OPEN.FLAG$ = "N"                                         ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF LOCCNT.OPEN.FLAG$ = "Y" THEN BEGIN                             ! 1.9DA
         CLOSE LOCCNT.SESS.NUM%                                         ! 1.9DA
         LOCCNT.OPEN.FLAG$ = "N"                                        ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF STKTF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
         CLOSE STKTF.SESS.NUM%                                          ! 1.9DA
         STKTF.OPEN.FLAG$ = "N"                                         ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF STKDC.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA
         CLOSE STKDC.SESS.NUM%                                          ! 1.9DA
         STKDC.OPEN.FLAG$ = "N"                                         ! 1.9DA
      ENDIF                                                             ! 1.9DA

      CLOSE IRF.SESS.NUM%                                               ! 1.9D
      CLOSE IDF.SESS.NUM%                                               ! 1.9D

!     SB.INTEGER% = STKCF.SESS.NUM%                                     ! 1.9DA
!     GOSUB SB.FILE.UTILS                                               ! 1.9DA

      IF APPLICATION.NO$ = "08" THEN BEGIN                              ! 2.3JAS Stocktake application

         SB.INTEGER% = STKMF.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = STKRC.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = XGCF.SESS.NUM%                                      ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = STKEX.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = STKIF.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = LOCCNT.SESS.NUM%                                    ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = STKIG.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = STKTF.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

         SB.INTEGER% = STKDC.SESS.NUM%                                     ! 1.9DA
         GOSUB SB.FILE.UTILS                                               ! 1.9DA

      ENDIF ELSE BEGIN                                                     ! 2.3JAS Returns application                                                                ! 2.3JAS

         SB.INTEGER% = SOPTS.SESS.NUM%                                     ! 2.3JAS
         GOSUB SB.FILE.UTILS                                               ! 2.3JAS

         SB.INTEGER% = LOCAL.SESS.NUM%                                     ! 2.3JAS
         GOSUB SB.FILE.UTILS                                               ! 2.3JAS

         SB.INTEGER% = CCREJ.SESS.NUM%                                     ! 2.4JAS
         GOSUB SB.FILE.UTILS                                               ! 2.4JAS

      ENDIF                                                                ! 2.3JAS

RETURN

ALLOCATE.MODULE.5:                                                      ! 2.1BG

      SB.ACTION$ = "O"                                                  ! 2.1BG

!     SB.INTEGER% = STKCF.SESS.NUM%                                     ! 2.1BG
!     SB.STRING$ = STKCF.FILE.NAME$                                     ! 2.1BG
!     GOSUB SB.FILE.UTILS                                               ! 2.1BG
!     STKCF.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.1BG

      SB.INTEGER% = SSPSCTRL.REPORT.NUM%                                ! 2.2JAS  ! 2.7CS
      SB.STRING$ = SSPSCTRL.FILE.NAME$                                  ! 2.2JAS
      GOSUB SB.FILE.UTILS                                               ! 2.2JAS
      SSPSCTRL.SESS.NUM% = SB.FILE.SESS.NUM%                            ! 2.2JAS

      SB.INTEGER% = BTCS.REPORT.NUM%                                    ! 2.2JAS  ! 2.7CS
      SB.STRING$ = BTCS.FILE.NAME$                                      ! 2.2JAS
      GOSUB SB.FILE.UTILS                                               ! 2.2JAS
      BTCS.SESS.NUM% = SB.FILE.SESS.NUM%                                ! 2.2JAS

      SB.INTEGER% = PRINT.REPORT.NUM%                                   ! 2.2JAS  ! 2.7CS
      SB.STRING$ = PRINT.FILE.NAME$                                     ! 2.2JAS
      GOSUB SB.FILE.UTILS                                               ! 2.2JAS
      PRINT.SESS.NUM% = SB.FILE.SESS.NUM%                               ! 2.2JAS

RETURN                                                                  ! 2.1BG

DEALLOCATE.MODULE.5:                                                    ! 2.1BG

      SB.ACTION$ = "C"                                                  ! 2.1BG
      SB.STRING$ = ""                                                   ! 2.1BG

!     IF STKCF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 2.1BG
!        CLOSE STKCF.SESS.NUM%                                          ! 2.1BG
!        STKCF.OPEN.FLAG$ = "N"                                         ! 2.1BG
!     ENDIF                                                             ! 2.1BG

      SB.INTEGER% = SSPSCTRL.SESS.NUM%                                  ! 2.2JAS
      GOSUB SB.FILE.UTILS                                               ! 2.2JAS

      SB.INTEGER% = BTCS.SESS.NUM%                                      ! 2.2JAS
      GOSUB SB.FILE.UTILS                                               ! 2.2JAS

      SB.INTEGER% = PRINT.SESS.NUM%                                     ! 2.2JAS
      GOSUB SB.FILE.UTILS                                               ! 2.2JAS

RETURN                                                                  ! 2.1BG

\******************************************************************************
\***
\***   DETERMINE.DATA.TYPE:
\***
\***      set RECEIVE.STATE$ depending on record received (DATA.IN$),
\***      decode is follows ;
\***      1st char = SOH char. and record length = 14           - state "B"
\***      1st char = SOH char. and record length <> 14          - state "H"
\***      1st 2 chars = "CH"                                    - state "C"
\***      1st 2 chars = "LH"                                    - state "D"
\***      1st 2 chars = "LI"                                    - state "E"
\***      1st 2 chars = "LT"                                    - state "F"
\***      1st 2 chars = "CT"                                    - state "G"
\***      1st 2 chars = "CZ"                                    - state "I"
\***      1st 2 chars = "XH"                                    - state "K"   !2.9NWB
\***      1st 2 chars = "XM"                                    - state "L"   !2.9NWB
\***      1st 2 chars = "XU"                                    - state "L"   !2.9NWB
\***      1st 2 chars = "XO"                                    - state "L"   !2.9NWB
\***      1st 2 chars = "XR"                                    - state "N"   !2.9NWB
\***      1st 2 chars = "XT"                                    - state "J"   !2.9NWB
\***      1st 2 chars = "XZ"                                    - state "M"   !2.9NWB
\***      1st 2 chars = "PH"                                    - state "O"
\***      1st 2 chars = "PC"                                    - state "P"
\***      1st 2 chars = "PT"                                    - state "Q"
\***      1st 2 chars = "CV"                                    - state "R"
\***      1st 2 chars = "PZ"                                    - state "S"
\***      1st 2 chars = "OR"                                    - state "T"
\***      1st 2 chars = "FH"                                    - state "U"
\***      1st 2 chars = "OH"                                    - state "V"
\***      1st 2 chars = "OD"                                    - state "W"
\***      1st 2 chars = "OT"                                    - state "X"
\***      1st 2 chars = "FT"                                    - state "Y"
\***      1st 2 chars = "OZ"                                    - state "Z"
\***      1st 2 chars = "UE"                                    - state "a"
\***      1st 2 chars = "UH"                                    - state "b"
\***      1st 2 chars = "UD"                                    - state "c"
\***      1st 2 chars = "UT"                                    - state "d"
\***      1st 2 chars = "UR"                                    - state "e"
\***      1st 2 chars = "UZ"                                    - state "f"
\***      1st 2 chars = "RI"                                    - state "g"
\***      1st 2 chars = "RE"                                    - state "h"
\***      1st 2 chars = "RH"                                    - state "i"
\***      1st 2 chars = "RD"                                    - state "j"
\***      1st 2 chars = "RT"                                    - state "k"
\***      1st 2 chars = "RR"                                    - state "l"
\***      1st 2 chars = "RQ"                                    - state "m"
\***      1st 2 chars = "RO"                                    - state "n"
\***      1st 2 chars = "RZ"                                    - state "o"
\***      1st 2 chars = "SH"                                    - state "p"
\***      1st 2 chars = "SR"                                    - state "q"
\***      1st 2 chars = "ST"                                    - state "r"
\***      1st 2 chars = "IH"                                    - state "s"
\***      1st 2 chars = "IR"                                    - state "t"
\***      1st 2 chars = "IT"                                    - state "u"
\***      1st 2 chars = "IZ"                                    - state "v"
\***      1st 2 chars = "AH"                                    - state "w"
\***      1st 2 chars = "AR"                                    - state "x"
\***      1st 2 chars = "AT"                                    - state "y"
\***      1st 2 chars = "AZ"                                    - state "z"
\***      1st 2 chars = "TA"                                    - state "7"
\***      1st 2 chars = "TF"                                    - state "8"
\***      1st 2 chars = "TI"                                    - state "9"
\***      1st 2 chars = "TJ"                                    - state ":"
\***      1st 2 chars = "TK"                                    - state ";"
\***      1st 2 chars = "TL"                                    - state "<"
\***      1st 2 chars = "@@"                                    - state "@"
\***      any record that does not satisfy any of the above conditions will
\***      be given a state of "*" (invalid record)
\***
\***   RETURN
\***
\******************************************************************************

   DETERMINE.DATA.TYPE:

      DATA.LENGTH% = LEN(DATA.IN$)

      IF LEFT$(DATA.IN$,1) = SOH$ THEN BEGIN
         IF DATA.LENGTH% = 14 THEN                                      \ DJAL
            STATE$ = "B"                                                \
         ELSE                                                           \
            STATE$ = "H"
         RETURN
      ENDIF

      IF LEFT$(DATA.IN$,2) = "CH" THEN BEGIN
         STATE$ = "C"
         RETURN
      ENDIF

      IF LEFT$(DATA.IN$,2) = "LH" THEN BEGIN
         STATE$ = "D"
         RETURN
      ENDIF

      IF LEFT$(DATA.IN$,2) = "LI" THEN BEGIN
         STATE$ = "E"
         RETURN
      ENDIF

      IF LEFT$(DATA.IN$,2) = "LT" THEN BEGIN
         STATE$ = "F"
         RETURN
      ENDIF

      IF LEFT$(DATA.IN$,2) = "CT" THEN BEGIN
         STATE$ = "G"
         RETURN
      ENDIF

      IF LEFT$(DATA.IN$,2) = "CZ" THEN BEGIN                            ! DJAL
         STATE$ = "I"                                                   ! DJAL
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XH" THEN BEGIN                            ! DJAL
         STATE$ = "K"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XM" THEN BEGIN                            ! DJAL
         STATE$ = "L"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XU" THEN BEGIN                            ! DJAL
         STATE$ = "L"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XO" THEN BEGIN                            ! DJAL
         STATE$ = "L"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XR" THEN BEGIN                            ! DJAL
         STATE$ = "N"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XT" THEN BEGIN                            ! DJAL
         STATE$ = "J"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "XZ" THEN BEGIN                            ! DJAL
         STATE$ = "M"                                                   ! DJAL!2.9NWB
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "PH" THEN BEGIN                            ! DJAL
         STATE$ = "O"                                                   ! DJAL
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "PC" THEN BEGIN                            ! DJAL
         STATE$ = "P"                                                   ! DJAL
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "PT" THEN BEGIN                            ! DJAL
         STATE$ = "Q"                                                   ! DJAL
         RETURN                                                         ! DJAL
      ENDIF                                                             ! DJAL

      IF LEFT$(DATA.IN$,2) = "CV" THEN BEGIN                            ! HDS
         STATE$ = "R"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "PZ" THEN BEGIN                            ! HDS
         STATE$ = "S"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "OR" THEN BEGIN                            ! HDS
         STATE$ = "T"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "FH" THEN BEGIN                            ! HDS
         STATE$ = "U"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "OH" THEN BEGIN                            ! HDS
         STATE$ = "V"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "OD" THEN BEGIN                            ! HDS
         STATE$ = "W"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "OT" THEN BEGIN                            ! HDS
         STATE$ = "X"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "FT" THEN BEGIN                            ! HDS
         STATE$ = "Y"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "OZ" THEN BEGIN                            ! HDS
         STATE$ = "Z"                                                   ! HDS
         RETURN                                                         ! HDS
      ENDIF                                                             ! HDS

      IF LEFT$(DATA.IN$,2) = "UE" THEN BEGIN                            ! LLC
         STATE$ = "a"         ! lower case !!!!!!                       ! LLC
         RETURN                                                         ! LLC
      ENDIF                                                             ! LLC

      IF LEFT$(DATA.IN$,2) = "UH" THEN BEGIN                            ! LLC
         STATE$ = "b"         ! lower case !!!!!!                       ! LLC
         RETURN                                                         ! LLC
      ENDIF                                                             ! LLC

      IF LEFT$(DATA.IN$,2) = "UD" THEN BEGIN                            ! LLC
         STATE$ = "c"         ! lower case !!!!!!                       ! LLC
         RETURN                                                         ! LLC
      ENDIF                                                             ! LLC

      IF LEFT$(DATA.IN$,2) = "UT" THEN BEGIN                            ! LLC
         STATE$ = "d"         ! lower case !!!!!!                       ! LLC
         RETURN                                                         ! LLC
      ENDIF                                                             ! LLC

      IF LEFT$(DATA.IN$,2) = "UR" THEN BEGIN                            ! LLC
         STATE$ = "e"         ! lower case !!!!!!                       ! LLC
         RETURN                                                         ! LLC
      ENDIF                                                             ! LLC

      IF LEFT$(DATA.IN$,2) = "UZ" THEN BEGIN                            ! LLC
         STATE$ = "f"         ! lower case !!!!!!                       ! LLC
         RETURN                                                         ! LLC
      ENDIF                                                             ! LLC

      IF LEFT$(DATA.IN$,2) = "RI" THEN BEGIN                            ! MMJK
         STATE$ = "g"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RE" THEN BEGIN                            ! MMJK
         STATE$ = "h"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RH" THEN BEGIN                            ! MMJK
         STATE$ = "i"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RD" THEN BEGIN                            ! MMJK
         STATE$ = "j"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RT" THEN BEGIN                            ! MMJK
         STATE$ = "k"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RR" THEN BEGIN                            ! MMJK
         STATE$ = "l"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RQ" THEN BEGIN                            ! MMJK
         STATE$ = "m"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RO" THEN BEGIN                            ! MMJK
         STATE$ = "n"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "RZ" THEN BEGIN                            ! MMJK
         STATE$ = "o"         ! lower case !!!!!!                       ! MMJK
         RETURN                                                         ! MMJK
      ENDIF                                                             ! MMJK

      IF LEFT$(DATA.IN$,2) = "SH" THEN BEGIN                            ! 1.5
         STATE$ = "p"         ! lower case !!!!!!                       ! 1.5
         RETURN                                                         ! 1.5
      ENDIF                                                             ! 1.5

      IF LEFT$(DATA.IN$,2) = "SR" THEN BEGIN                            ! 1.5
         STATE$ = "q"         ! lower case !!!!!!                       ! 1.5
         RETURN                                                         ! 1.5
      ENDIF                                                             ! 1.5

      IF LEFT$(DATA.IN$,2) = "ST" THEN BEGIN                            ! 1.5
         STATE$ = "r"         ! lower case !!!!!!                       ! 1.5
         RETURN                                                         ! 1.5
      ENDIF                                                             ! 1.5

      IF LEFT$(DATA.IN$,2) = "IH" THEN BEGIN                            ! 2.1BG
         STATE$ = "s"         ! lower case !!!!!!                       ! 2.1BG
         RETURN                                                         ! 2.1BG
      ENDIF                                                             ! 2.1BG

      IF LEFT$(DATA.IN$,2) = "IR" THEN BEGIN                            ! 2.1BG
         STATE$ = "t"         ! lower case !!!!!!                       ! 2.1BG
         RETURN                                                         ! 2.1BG
      ENDIF                                                             ! 2.1BG

      IF LEFT$(DATA.IN$,2) = "IT" THEN BEGIN                            ! 2.1BG
         STATE$ = "u"         ! lower case !!!!!!                       ! 2.1BG
         RETURN                                                         ! 2.1BG
      ENDIF                                                             ! 2.1BG

      IF LEFT$(DATA.IN$,2) = "IZ" THEN BEGIN                            ! 2.1BG
         STATE$ = "v"         ! lower case !!!!!!                       ! 2.1BG
         RETURN                                                         ! 2.1BG
      ENDIF                                                             ! 2.1BG

      IF LEFT$(DATA.IN$,2) = "AH" THEN BEGIN                            ! 2.2JAS
         STATE$ = "w"         ! lower case !!!!!!                       ! 2.2JAS
         RETURN                                                         ! 2.2JAS
      ENDIF                                                             ! 2.2JAS

      IF LEFT$(DATA.IN$,2) = "AR" THEN BEGIN                            ! 2.2JAS
         STATE$ = "x"         ! lower case !!!!!!                       ! 2.2JAS
         RETURN                                                         ! 2.2JAS
      ENDIF                                                             ! 2.2JAS

      IF LEFT$(DATA.IN$,2) = "AT" THEN BEGIN                            ! 2.2JAS
         STATE$ = "y"         ! lower case !!!!!!                       ! 2.2JAS
         RETURN                                                         ! 2.2JAS
      ENDIF                                                             ! 2.2JAS

      IF LEFT$(DATA.IN$,2) = "AZ" THEN BEGIN                            ! 2.2JAS
         STATE$ = "z"         ! lower case !!!!!!                       ! 2.2JAS
         RETURN                                                         ! 2.2JAS
      ENDIF                                                             ! 2.2JAS
      
      IF LEFT$(DATA.IN$,2) = "YR" THEN BEGIN                            ! 2.10BG
         STATE$ = "1"                                                   ! 2.10BG
         RETURN                                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF LEFT$(DATA.IN$,2) = "YO" THEN BEGIN                            ! 2.10BG
         STATE$ = "2"                                                   ! 2.10BG
         RETURN                                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF LEFT$(DATA.IN$,2) = "YH" THEN BEGIN                            ! 2.10BG
         STATE$ = "3"                                                   ! 2.10BG
         RETURN                                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF LEFT$(DATA.IN$,2) = "YD" THEN BEGIN                            ! 2.10BG
         STATE$ = "4"                                                   ! 2.10BG
         RETURN                                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF LEFT$(DATA.IN$,2) = "YT" THEN BEGIN                            ! 2.10BG
         STATE$ = "5"                                                   ! 2.10BG
         RETURN                                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF LEFT$(DATA.IN$,2) = "YZ" THEN BEGIN                            ! 2.10BG
         STATE$ = "6"                                                   ! 2.10BG
         RETURN                                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG

      IF LEFT$(DATA.IN$,2) = "TA" THEN BEGIN                            ! 2.12SH
         STATE$ = "7"                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH

      IF LEFT$(DATA.IN$,2) = "TF" THEN BEGIN                            ! 2.12SH
         STATE$ = "8"                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH

      IF LEFT$(DATA.IN$,2) = "TI" THEN BEGIN                            ! 2.12SH
         STATE$ = "9"                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH

      IF LEFT$(DATA.IN$,2) = "TJ" THEN BEGIN                            ! 2.12SH
         STATE$ = ":"                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH

      IF LEFT$(DATA.IN$,2) = "TK" THEN BEGIN                            ! 2.12SH
         STATE$ = ";"                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH

      IF LEFT$(DATA.IN$,2) = "TL" THEN BEGIN                            ! 2.12SH
         STATE$ = "<"                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH

      IF LEFT$(DATA.IN$,2) = "TZ" THEN BEGIN                            ! 2.12SH
         STATE$ = "="                                                   ! 2.12SH
         RETURN                                                         ! 2.12SH
      ENDIF                                                             ! 2.12SH
      
      IF LEFT$(DATA.IN$,2) = "@@" THEN BEGIN                            ! 2.8CS
         STATE$ = "@"         ! Signal from PSS38 to end                ! 2.8CS
         RETURN                                                         ! 2.8CS
      ENDIF                                                             ! 2.8CS

      STATE$ = "*"                                                      ! HDS

RETURN

\******************************************************************************
\***
\***   LOG.TO.AUDIT.FILE
\***
\******************************************************************************

   LOG.TO.AUDIT.FILE:

        IF CSR.AUDIT.OPEN.FLAG$ = "N" THEN BEGIN                        ! EPAB
           IF END #CSR.AUDIT.SESS.NUM% THEN CREATE.AUDIT.FILE           ! EPAB
           OPEN CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM% APPEND           ! EPAB
           CSR.AUDIT.OPEN.FLAG$ = "Y"                                   ! EPAB
        ENDIF                                                           ! EPAB

        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$           ! EPAB
        IF LEFT$(APPL$,4) = "LOAD" THEN BEGIN                           ! ILC
           CLOSE CSR.AUDIT.SESS.NUM%                                    ! ILC
           CSR.AUDIT.OPEN.FLAG$ = "N"                                   ! ILC
        ENDIF                                                           ! ILC

        RETURN

   CREATE.AUDIT.FILE:                                                   ! EPAB

        CREATE CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM%                   ! EPAB
        CSR.AUDIT.OPEN.FLAG$ = "Y"                                      ! EPAB
        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;                          \ EPAB
        "                          LDT/PDT Tracking Log"                ! LLC
        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;" "                       ! EPAB
        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"File Creation Date " +   \ EPAB
          LEFT$(DATE$,2) + "/" + MID$(DATE$,3,2) + "/" + RIGHT$(DATE$,2)! EPAB
        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;"Currently executing " +  \ EPAB
\         "PSS37 Version 24 (A5B), Created on 4th Jan 2005"             ! 1.2 ! 2.2JAS  ! 2.7CS
\         "PSS37 Version 25 (A7B), Created on 22nd Dec 2006"            ! 2.9NWB
          "PSS37 Version 26 (A7C), Created on 11th May 2007"            ! 1.5BG
        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;" "                       ! EPAB
        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;" "                       ! EPAB

        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$           ! EPAB

        RETURN

\******************************************************************************
\***
\***   LOG.TO.LDTAF.FILE
\***
\******************************************************************************

   LOG.TO.LDTAF.FILE:                                                   ! MMJK
                                                                        ! MMJK
        CURRENT.KEY$ = ""                                               ! MMJK
        CURR.SESS.NUM% = LDTAF.SESS.NUM%                                ! MMJK
        IF LDTAF.OPEN.FLAG$ <> "Y" THEN BEGIN                           ! MMJK
           IF END #LDTAF.SESS.NUM% THEN CREATE.LDTAF.FILE               ! MMJK
           OPEN LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM% APPEND              ! MMJK
           LDTAF.OPEN.FLAG$ = "Y"                                       ! MMJK
        ENDIF                                                           ! MMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                   \ MMJK
          IF WRITE.LDTAF THEN GOTO WRITE.ERROR                          ! MMJK
        LDTAF.OPEN.FLAG$ = "N"                                          ! MMJK
        CLOSE LDTAF.SESS.NUM%                                           ! MMJK
                                                                        ! MMJK
   RETURN                                                               ! MMJK
                                                                        ! MMJK
   CREATE.LDTAF.FILE:                                                   ! MMJK
                                                                        ! MMJK
        IF END #LDTAF.SESS.NUM% THEN CREATE.ERROR                       ! MMJK
          CREATE POSFILE LDTAF.FILE.NAME$ AS LDTAF.SESS.NUM%            \ MMJK
            MIRRORED ATCLOSE                                            ! MMJK
        LDTAF.OPEN.FLAG$ = "Y"                                          ! MMJK
        IF LDTAF.LINK.TYPE% <> 0 THEN                                   \ MMJK
          IF WRITE.LDTAF THEN GOTO WRITE.ERROR                          ! MMJK
        LDTAF.OPEN.FLAG$ = "N"                                          ! MMJK
        CLOSE LDTAF.SESS.NUM%                                           ! MMJK
                                                                        ! MMJK
   RETURN                                                               ! MMJK

\*****************************************************************************
\***
\***   ASN.CARTON.HEADER.RECEIVED:
\***
\*****************************************************************************

ASN.CARTON.HEADER.RECEIVED:                                                   !2.9NWB

   IF CB.OPEN.FLAG$ = "Y" THEN BEGIN                                          !2.9NWB
      DELETE CB.SESS.NUM%                                                     !2.9NWB
      CB.OPEN.FLAG$ = "N"                                                     !2.9NWB
   ENDIF                                                                      !2.9NWB

   CB.FN$ = CB.FILE.NAME$ + TIME$                                             !2.9NWB
   CB.TN$ = CB.FN$ + ".TMP"                                                   !2.9NWB
   IF END #CB.SESS.NUM% THEN CREATE.ERROR                                     !2.9NWB
   CREATE CB.TN$ AS CB.SESS.NUM%                                              !2.9NWB
   CB.OPEN.FLAG$ = "Y"                                                        !2.9NWB

   ASN.RCD.CNT%   = 1                           ! 1st rcd rcvd                !2.9NWB

RETURN                                                                        !2.9NWB

\*****************************************************************************
\***
\***   AUTOMATIC.CARTON.BOOK.IN:
\***
\*****************************************************************************

AUTOMATIC.CARTON.BOOK.IN:                                                     !2.9NWB

   IF CB.OPEN.FLAG$ = "N" THEN BEGIN                                          !2.9NWB
      GOSUB ASN.CARTON.HEADER.RECEIVED                                        !2.9NWB
   ENDIF ELSE BEGIN                                                           !2.9NWB
      ASN.RCD.CNT%   = ASN.RCD.CNT% +1          ! Increment                   !2.9NWB
   ENDIF                                                                      !2.9NWB

   CB.REC.TYPE$       = "C"                                                   !2.9NWB
   CB.CARTON.BARCODE$ = MID$(DATA.IN$,  3, 14)                                !2.9NWB
   CB.REPORT.RQD$     = "Y"                                                   !2.9NWB

   GOSUB WRITE.DATA.TO.CB.FILE                                                !2.9NWB

RETURN                                                                        !2.9NWB

\*****************************************************************************
\***
\***   MANUAL.CARTON.BOOK.IN:
\***
\*****************************************************************************

MANUAL.CARTON.BOOK.IN:                                                        !2.9NWB

   IF CB.OPEN.FLAG$ = "N" THEN BEGIN                                          !2.9NWB
      GOSUB ASN.CARTON.HEADER.RECEIVED                                        !2.9NWB
   ENDIF ELSE BEGIN                                                           !2.9NWB
      ASN.RCD.CNT%   = ASN.RCD.CNT% +1          ! Increment                   !2.9NWB
   ENDIF                                                                      !2.9NWB

   IF LEFT$(DATA.IN$, 2) = "XM" THEN BEGIN                                    !2.9NWB
      CB.REC.TYPE$        = "H"                                               !2.9NWB
      CB.CARTON.BARCODE$  = MID$(DATA.IN$,  3, 14)                            !2.9NWB
      IF MAN.REC.CNT% <> 0 THEN BEGIN                                         !2.9NWB
         ! Log to Audit File                                                  !2.9NWB
         CSR.AUDIT.DATA$ = "ASN Processing - Audit/No Carton - "              \2.9NWB
                         + "Out of Sequence Header Received. "                !2.9NWB
         GOSUB LOG.TO.AUDIT.FILE                                              !2.9NWB
      ENDIF                                                                   !2.9NWB
      MAN.REC.CNT%        = 1                                                 !2.9NWB
   ENDIF ELSE IF LEFT$(DATA.IN$, 2) = "XU" THEN BEGIN                         !2.9NWB
      CB.REC.TYPE$        = "D"                                               !2.9NWB
      CB.ITEM.BARCODE$    = MID$(DATA.IN$,  3, 13)                            !2.9NWB
      CB.ITEM.QUANTITY$   = MID$(DATA.IN$, 16,  4)                            !2.9NWB
      MAN.REC.CNT%        = MAN.REC.CNT% +1                                   !2.9NWB
   ENDIF ELSE IF LEFT$(DATA.IN$, 2) = "XO" THEN BEGIN                         !2.9NWB
      CB.REC.TYPE$        = "T"                                               !2.9NWB
      CB.ITEM.COUNT$      = MID$(DATA.IN$,  3, 5)                             !2.9NWB
      CB.MANUAL.COUNT%    = VAL(MID$(DATA.IN$,  8, 4))                        !2.9NWB
      MAN.REC.CNT%        = MAN.REC.CNT% +1                                   !2.9NWB
      IF MAN.REC.CNT% <> CB.MANUAL.COUNT% THEN BEGIN                          !2.9NWB
         ! Log to Audit File                                                  !2.9NWB
         CSR.AUDIT.DATA$ = "ASN Processing - Audit/No Carton - "              \2.9NWB
                         + "Incorrect Number of Records Received. "           \2.9NWB
                         + STR$(MAN.REC.CNT%) + " Received, "                 \2.9NWB
                         + STR$(CB.MANUAL.COUNT%) + " Expected."              !2.9NWB
         GOSUB LOG.TO.AUDIT.FILE                                              !2.9NWB
      ENDIF                                                                   !2.9NWB
      MAN.REC.CNT%        = 0                   ! Reset                       !2.9NWB
   ENDIF                                                                      !2.9NWB

   GOSUB WRITE.DATA.TO.CB.FILE                                                !2.9NWB

RETURN                                                                        !2.9NWB

\*****************************************************************************
\***
\***   WRITE.DATA.TO.CB.FILE:
\***
\*****************************************************************************

WRITE.DATA.TO.CB.FILE:                                                        !2.9NWB

   RC% = WRITE.CB                                                             !2.9NWB
   IF RC% <> 0 THEN BEGIN                                                     !2.9NWB
      GOSUB WRITE.ERROR                                                       !2.9NWB
   ENDIF                                                                      !2.9NWB

RETURN                                                                        !2.9NWB

\*****************************************************************************
\***
\***   ASN.CARTON.EOT.RECEIVED:
\***
\*****************************************************************************

ASN.CARTON.EOT.RECEIVED:                                                      !2.9NWB

   CB.RECORD.COUNT% = VAL(MID$(DATA.IN$,  3,  5))                             !2.9NWB

   ASN.RCD.CNT%   = ASN.RCD.CNT% +1             ! Last rcd rcvd               !2.9NWB

   IF CB.RECORD.COUNT% <> ASN.RCD.CNT% THEN BEGIN                             !2.9NWB
      ! Log to Audit File                                                     !2.9NWB
      CSR.AUDIT.DATA$ = "ASN Processing - PDT Transmission - "                \2.9NWB
                      + "Incorrect Number of Records Received. "              \2.9NWB
                      + STR$(ASN.RCD.CNT%) + " Received, "                    \2.9NWB
                      + STR$(CB.RECORD.COUNT%) + " Expected."                 !2.9NWB
      GOSUB LOG.TO.AUDIT.FILE                                                 !2.9NWB
   ENDIF                                                                      !2.9NWB

   CLOSE CB.SESS.NUM%                                                         !2.9NWB
   RC% = RENAME(CB.FN$, CB.TN$)                                               !2.9NWB
   WHILE RC% <> -1                                                            !2.9NWB
      CB.FN$ = CB.FILE.NAME$ + TIME$                                          !2.9NWB
      RC% = RENAME(CB.FN$, CB.TN$)                                            !2.9NWB
   WEND                                                                       !2.9NWB
   CB.OPEN.FLAG$ = "N"                                                        !2.9NWB

   GOSUB START.PSD62                                                          !2.9NWB

   RE.CHAIN = TRUE                                                            !2.9NWB
   RECEIVE.STATE$ = "?"                                                       !2.9NWB

RETURN                                                                        !2.9NWB

\*****************************************************************************
\***
\***   START.PSD62:
\***
\*****************************************************************************

START.PSD62:                                                                  !2.9NWB

   ADX.RET.CODE% = ADXSTART("ADX_UPGM:PSD62.286",                             \2.9NWB
                            CB.FN$,                                           \2.9NWB
                            "Carton Book In Batch Program")                   !2.9NWB

RETURN                                                                        !2.9NWB

\*****************************************************************************
\***
\***   CSR.LIST.HEADER.RECEIVED:   (removed v2.9)
\***
\*****************************************************************************

\  CSR.LIST.HEADER.RECEIVED:

\       IF (FN.VALIDATE.DATA( DATA.IN$ , 9 ) = 0) OR                    \ LMJK!2.9NWB
\          (CSRBF.OPEN.FLAG$ = "N") THEN BEGIN                          ! LMJK!2.9NWB
\         RECEIVE.STATE$ = "*"                                          ! LMJK!2.9NWB
\         RETURN                                                        ! LMJK!2.9NWB
\       ENDIF                                                           ! LMJK!2.9NWB

\       IF CSR.AUDIT.OPEN.FLAG$ = "N" THEN BEGIN                        ! EPAB!2.9NWB
\          IF END #CSR.AUDIT.SESS.NUM% THEN AUDIT.FAIL                  ! EPAB!2.9NWB
\          OPEN CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM% APPEND           ! EPAB!2.9NWB
\          CSR.AUDIT.OPEN.FLAG$ = "Y"                                   ! EPAB!2.9NWB
\       ENDIF                                                           ! EPAB!2.9NWB

\       IF MID$(DATA.IN$,6,6) <> "000000" THEN BEGIN                    ! EPAB!2.9NWB
\       CSR.AUDIT.DATA$ = "Received Unit " +                            \ EPAB!2.9NWB
\          MID$(DATA.IN$,4,2) + " Frequency " + MID$(DATA.IN$,3,1) +    \ EPAB!2.9NWB
\          " Counted on " + MID$(DATA.IN$,6,2) + "/" +                  \ EPAB!2.9NWB
\          MID$(DATA.IN$,8,2) + "/" + MID$(DATA.IN$,10,2) + " at " +    \ EPAB!2.9NWB
\          MID$(DATA.IN$,12,2) + ":" + MID$(DATA.IN$,14,2)              ! EPAB!2.9NWB
\       ENDIF ELSE BEGIN                                                ! EPAB!2.9NWB
\       CSR.AUDIT.DATA$ = "Received Unit " +                            \ EPAB!2.9NWB
\          MID$(DATA.IN$,4,2) + " Frequency " + MID$(DATA.IN$,3,1)      ! EPAB!2.9NWB
\       ENDIF                                                           ! EPAB!2.9NWB
\       CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] PDT No." +    \ EPAB!2.9NWB
\                         CURR.TERMINAL$ + " " + CSR.AUDIT.DATA$        ! EPAB!2.9NWB
\       PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$           ! EPAB!2.9NWB

\  AUDIT.FAIL:                                                          ! EPAB!2.9NWB

\       GOSUB WRITE.DATA.TO.CSR.BUFFER                                  ! EPAB!2.9NWB

\ RETURN                                                                      !2.9NWB


\******************************************************************************
\***
\***   LOG.ABORT.TO.AUDIT.FILE
\***
\******************************************************************************

   LOG.ABORT.TO.AUDIT.FILE:

        IF CSR.AUDIT.OPEN.FLAG$ = "N" THEN BEGIN                        ! EPAB
           OPEN CSR.AUDIT.FILE$ AS CSR.AUDIT.SESS.NUM% APPEND           ! EPAB
           CSR.AUDIT.OPEN.FLAG$ = "Y"                                   ! EPAB
        ENDIF                                                           ! EPAB

        PRINT USING "&"; #CSR.AUDIT.SESS.NUM%;CSR.AUDIT.DATA$           ! EPAB
        CLOSE CSR.AUDIT.SESS.NUM%                                       ! EPAB
        CSR.AUDIT.OPEN.FLAG$ = "N"                                      ! EPAB

  RETURN


\******************************************************************************
\***
\***   TIDY.UP
\***
\***      if PDT currently being 'held' then release it
\***
\***      if tidy then RETURN
\***
\***      If stocktake file open then close it                         1.5
\***
\***      if a CSR workfile exists, then process it
\***
\***      clear / tidy-up session variables
\***
\***   RETURN
\***
\******************************************************************************

   TIDY.UP:

      IF HOLD.FLAG$ = "Y" THEN GOSUB RELEASE.PDT                        ! DSW

      IF TIDY.FLAG$ = "Y" THEN RETURN

      GOSUB CLOSE.OPEN.FILES

      IF STKTK.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.5
          CLOSE STKTK.SESS.NUM%                                         ! 1.5
          STKTK.OPEN.FLAG$ = "N"                                        ! 1.5
      ENDIF                                                             ! 1.5

\     PROCESS.CSR.WORKFILE$ = "N"                                       !2.9NWB

      CURR.TERMINAL$ = "??????"
      CURR.LIST$ = "0000"
      TRAILER.LIST$ = "0000"
      LIST.COUNT% = 0
      ITEM.COUNT% = 0
      FILE.HEADER.LISTS% = 0
      FILE.TRAILER.LISTS% = 0
      LIST.TRAILER.ITEMS% = 0
      LIST.BC$ = ""

      TIDY.FLAG$ = "Y"

      GOSUB SEND.XON                                                    ! DSW

   RETURN

\******************************************************************************
\***
\***   ZEROISE.CITEM:
\***
\***      all "on order for this PDT" figures in CSRITEM are set to 0
\***
\***   RETURN
\***
\******************************************************************************

\  ZEROISE.CITEM:                                                       ! DJAL!2.9NWB

\     SB.MESSAGE$ = "PDT Support - Processing item file"                ! DSW !2.9NWB
\     GOSUB SB.BG.MESSAGE                                               ! DSW !2.9NWB

\     CURR.SESS.NUM% = CITEM.SESS.NUM%                                  ! DSW !2.9NWB
\     IF END #CITEM.SESS.NUM% THEN OPEN.ERROR                           ! DJAL!2.9NWB
\     OPEN CITEM.FILE.NAME$ DIRECT RECL 512 AS CITEM.SESS.NUM% NODEL    ! DJAL!2.9NWB
\     CITEM.OPEN.FLAG$ = "Y"                                            ! DSW !2.9NWB

\     END.OF.CITEM$ = "N"                                               ! DJAL!2.9NWB

\     SECTOR.COUNT% = 2                                                 ! DJAL!2.9NWB
\     CURRENT.KEY$ ="SECTOR"                                            ! DJAL!2.9NWB
\     IF END #CITEM.SESS.NUM% THEN READ.ERROR                           ! DJAL!2.9NWB
\     READ FORM "C4,C508"; #CITEM.SESS.NUM%, SECTOR.COUNT%;             \ DJAL!2.9NWB
\        FILLER$, CITEM.RECORD$                                         ! DJAL!2.9NWB

\     WHILE END.OF.CITEM$ = "N"                                         ! DJAL!2.9NWB
\        POSITION% = 1                                                  ! DJAL!2.9NWB
\        CITEM.SECTOR.ALTERED$ = "N"                                    ! DJAL!2.9NWB
\        WHILE POSITION% < 448                                          \ DSW !2.9NWB
\        AND MID$(CITEM.RECORD$,POSITION%,4) <> PK4$                    ! DSW !2.9NWB
\           IF MID$(CITEM.RECORD$,POSITION% + 14,2) <> PK2$ THEN BEGIN  ! DSW !2.9NWB
\              CITEM.SECTOR.ALTERED$ = "Y"                              ! DSW !2.9NWB
\              CITEM.RECORD$ = LEFT$(CITEM.RECORD$,POSITION% + 13) +    \ DSW !2.9NWB
\                              PK2$ +                                   \ DSW !2.9NWB
\                              RIGHT$(CITEM.RECORD$,                    \ DSW !2.9NWB
\                                LEN(CITEM.RECORD$) - POSITION% - 15)   ! DSW !2.9NWB
\           ENDIF                                                       ! DSW !2.9NWB
\           POSITION% = POSITION% + 64                                  ! DJAL!2.9NWB
\        WEND                                                           ! DJAL!2.9NWB
\        IF CITEM.SECTOR.ALTERED$ = "Y" THEN BEGIN                      ! DSW !2.9NWB
\           IF END #CITEM.SESS.NUM% THEN WRITE.ERROR                    ! DSW !2.9NWB
\           WRITE FORM "C4,C508"; #CITEM.SESS.NUM%, SECTOR.COUNT%;      \ DSW !2.9NWB
\                      FILLER$, CITEM.RECORD$                           ! DSW !2.9NWB
\        ENDIF                                                          ! DSW !2.9NWB
\        SECTOR.COUNT% = SECTOR.COUNT% + 1                              ! DJAL!2.9NWB
\        IF END #CITEM.SESS.NUM% THEN END.OF.CITEM                      ! DJAL!2.9NWB
\        READ FORM "C4,C508"; #CITEM.SESS.NUM%, SECTOR.COUNT%;          \ DJAL!2.9NWB
\                  FILLER$,CITEM.RECORD$                                ! DJAL!2.9NWB
\        GOTO ZEROISE.CITEM.CONTINUE                                    ! DJAL!2.9NWB
\  END.OF.CITEM:                                                        ! DJAL!2.9NWB
\        END.OF.CITEM$ = "Y"                                            ! DJAL!2.9NWB
\  ZEROISE.CITEM.CONTINUE:                                              ! DJAL!2.9NWB
\     WEND                                                              ! DJAL!2.9NWB

\     IF CITEM.OPEN.FLAG$ = "Y" THEN BEGIN                              ! DSW !2.9NWB
\        CLOSE CITEM.SESS.NUM%                                          ! DSW !2.9NWB
\        CITEM.OPEN.FLAG$ = "N"                                         ! DSW !2.9NWB
\     ENDIF                                                             ! DSW !2.9NWB

\  RETURN                                                               ! DJAL!2.9NWB

\******************************************************************************
\***
\***   CLOSE.OPEN.FILES:
\***
\***      close any open files, except pipe sessions
\***
\***   RETURN
\***
\******************************************************************************

   CLOSE.OPEN.FILES:

      CLOSE BCSMF.SESS.NUM%                                             ! DSW
      BCSMF.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE CHKBF.SESS.NUM%                                             ! DSW
      CHKBF.OPEN.FLAG$ = "N"                                            ! DSW

\     IF CIMF.OPEN.FLAG$ = "Y" THEN BEGIN                               ! 1.9D!2.9NWB
\        CLOSE CIMF.SESS.NUM%                                           ! DSW !2.9NWB
\        CIMF.OPEN.FLAG$ = "N"                                          ! DSW !2.9NWB
\     ENDIF                                                             ! 1.9D!2.9NWB

\     CLOSE CITEM.SESS.NUM%                                             ! DSW !2.9NWB
\     CITEM.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB

\     CLOSE CSRBF.SESS.NUM%                                             ! DSW !2.9NWB
\     CSRBF.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB

\     CLOSE CSRWF.SESS.NUM%                                             ! DSW !2.9NWB
\     CSRWF.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB

!     CLOSE FPF.SESS.NUM%                                               ! DSW
!     FPF.OPEN.FLAG$ = "N"                                              ! DSW

      CLOSE IDF.SESS.NUM%                                               ! DSW
      IDF.OPEN.FLAG$ = "N"                                              ! DSW

!     CLOSE INVOK.SESS.NUM%                                             ! DSW
!     INVOK.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE IEF.SESS.NUM%                                               ! DSW
      IEF.OPEN.FLAG$ = "N"                                              ! DSW

      CLOSE IRF.SESS.NUM%                                               ! DSW
      IRF.OPEN.FLAG$ = "N"                                              ! DSW

!     CLOSE ONORD.SESS.NUM%                                             ! DSW
!     ONORD.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE PDTWF.SESS.NUM%                                             ! DSW
      PDTWF.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE PIITM.SESS.NUM%                                             ! DSW
      PIITM.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE PILST.SESS.NUM%                                             ! DSW
      PILST.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE STKMQ.SESS.NUM%                                             ! DSW
      STKMQ.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE UNITS.SESS.NUM%                                             ! DSW
      UNITS.OPEN.FLAG$ = "N"                                            ! DSW

      CLOSE EPSOM.SESS.NUM%                                             ! DSW
      EPSOM.OPEN.FLAG$ = "N"                                            ! DSW

\     CLOSE CSR.SESS.NUM%                                               ! DSW !2.9NWB
\     CSR.OPEN.FLAG$ = "N"                                              ! DSW !2.9NWB

      CLOSE PCHK.SESS.NUM%                                              ! DSW
      PCHK.OPEN.FLAG$ = "N"                                             ! DSW

      CLOSE DIRORD.SESS.NUM%                                            ! HDS
      DIRORD.OPEN.FLAG$ = "N"                                           ! HDS

      CLOSE DIRSUP.SESS.NUM%                                            ! HDS
      DIRSUP.OPEN.FLAG$ = "N"                                           ! HDS

      CLOSE DIRWF.SESS.NUM%                                             ! HDS
      DIRWF.OPEN.FLAG$ = "N"                                            ! HDS

      CLOSE DIREC.SESS.NUM%                                             ! HDS
      DIREC.OPEN.FLAG$ = "N"                                            ! HDS

      CLOSE LDTCF.SESS.NUM%                                             ! HDS
      LDTCF.OPEN.FLAG$ = "N"                                            ! HDS

      IF APPL$ = "DIRECT" THEN BEGIN                                    ! JLC
         GOSUB RECREATE.DRSMQ                                           ! JLC
         CLOSE DRSMQ.SESS.NUM%                                          ! HDS
         DRSMQ.OPEN.FLAG$ = "N"                                         ! HDS
      ENDIF                                                             ! JLC

      CLOSE LDTBF.SESS.NUM%                                             ! JLC
      LDTBF.OPEN.FLAG$ = "N"                                            ! JLC

!     CLOSE IDSOF.SESS.NUM%                                             ! JLC
!     IDSOF.OPEN.FLAG$ = "N"                                            ! JLC

      CLOSE UOD.SESS.NUM%                                               ! LLC
      UOD.OPEN.FLAG$ = "N"                                              ! LLC

      CLOSE UODTF.SESS.NUM%                                             ! LLC
      UODTF.OPEN.FLAG$ = "N"                                            ! LLC

      CLOSE UODBF.SESS.NUM%                                             ! LLC
      UODBF.OPEN.FLAG$ = "N"                                            ! LLC

      IF CSR.AUDIT.OPEN.FLAG$ = "Y" THEN BEGIN                          ! EPAB
         CLOSE CSR.AUDIT.SESS.NUM%                                      ! EPAB
         CSR.AUDIT.OPEN.FLAG$ = "N"                                     ! EPAB
      ENDIF                                                             ! EPAB

      CLOSE CCUOD.SESS.NUM%                                             ! MMJK
      CCUOD.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE CCLAM.SESS.NUM%                                             ! 1.4
      CCLAM.OPEN.FLAG$ = "N"                                            ! 1.4

      CLOSE CCITM.SESS.NUM%                                             ! MMJK
      CCITM.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE CCTRL.SESS.NUM%                                             ! MMJK
      CCTRL.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE CCDMY.SESS.NUM%                                             ! MMJK
      CCDMY.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE CCTMP.SESS.NUM%                                             ! MMJK
      CCTMP.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE CCBUF.SESS.NUM%                                             ! MMJK
      CCBUF.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE CCUPF.SESS.NUM%                                             ! NMJK
      CCUPF.OPEN.FLAG$ = "N"                                            ! NMJK

      CLOSE CCWKF.SESS.NUM%                                             ! MMJK
      CCWKF.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE LDTAF.SESS.NUM%                                             ! MMJK
      LDTAF.OPEN.FLAG$ = "N"                                            ! MMJK

      CLOSE SOFTS.SESS.NUM%                                             ! MMJK
      SOFTS.OPEN.FLAG$ = "N"                                            ! MMJK

      IF GAPBF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 1.9DA !2.5CS !2.6BG
         CLOSE GAPBF.SESS.NUM%                                          ! 1.9DA !2.5CS !2.6BG
         GAPBF.OPEN.FLAG$ = "N"                                         ! 1.9DA !2.5CS !2.6BG
      ENDIF                                                             ! 1.9DA !2.5CS !2.6BG

      IF PLLOL.OPEN.FLAG$ = "Y" THEN BEGIN                              !2.5CS
         CLOSE PLLOL.SESS.NUM%                                          !2.5CS
         PLLOL.OPEN.FLAG$ = "N"                                         !2.5CS
      ENDIF                                                             !2.5CS

      IF PLLDB.OPEN.FLAG$ = "Y" THEN BEGIN                              !2.5CS
         CLOSE PLLDB.SESS.NUM%                                          !2.5CS
         PLLDB.OPEN.FLAG$ = "N"                                         !2.5CS
      ENDIF                                                             !2.5CS

      IF TSF.OPEN.FLAG$ = "Y" THEN BEGIN                                ! 2.3JAS
         CLOSE TSF.SESS.NUM%                                            ! 2.3JAS
         TSF.OPEN.FLAG$ = "N"                                           ! 2.3JAS
      ENDIF                                                             ! 2.3JAS

      IF SOPTS.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 2.3JAS
         CLOSE SOPTS.SESS.NUM%                                          ! 2.3JAS
         SOPTS.OPEN.FLAG$ = "N"                                         ! 2.3JAS
      ENDIF                                                             ! 2.3JAS

      IF CB.OPEN.FLAG$ = "Y" THEN BEGIN                                 ! 2.9NWB
         CLOSE CB.SESS.NUM%                                             ! 2.9NWB
         CB.OPEN.FLAG$ = "N"                                            ! 2.9NWB
      ENDIF                                                             ! 2.9NWB
      
      IF RB.OPEN.FLAG$ = "Y" THEN BEGIN                                 ! 2.10BG
         CLOSE RB.SESS.NUM%                                             ! 2.10BG
         RB.OPEN.FLAG$ = "N"                                            ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF REWKF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! 2.10BG
         CLOSE REWKF.SESS.NUM%                                          ! 2.10BG
         REWKF.OPEN.FLAG$ = "N"                                         ! 2.10BG
      ENDIF                                                             ! 2.10BG
      
      IF RECALLS.OPEN.FLAG$ = "Y" THEN BEGIN                            ! 2.10BG
         CLOSE RECALLS.SESS.NUM%                                        ! 2.10BG
         RECALLS.OPEN.FLAG$ = "N"                                       ! 2.10BG
      ENDIF                                                             ! 2.10BG

      IF DELVINDX.OPEN.FLAG$ = "Y" THEN BEGIN                           ! 2.12SH
         CLOSE DELVINDX.SESS.NUM%                                       ! 2.12SH
         DELVINDX.OPEN.FLAG$ = "N"                                      ! 2.12SH
      ENDIF                                                             ! 2.12SH

      ALL.FILES.CLOSED$ = "Y"                                           ! DSW

   RETURN

\******************************************************************************
\***
\***   INACTIVITY.SHUTDOWN:
\***
\***      release PDT
\***      close ALL files
\***      Read stocktake control file
\***      If stocktake not in progress Then
\***         Receive.State = "A"
\***      Else
\***         If last stocktake data movement was today And
\***            happened more than Inactivity.Shutdown ago Then
\***               Receive.State = "A"
\***         Else
\***            If last stocktake data movement was before today And
\***               it is more than Inactivity.Shutdown after midnight Then
\***                  Receive.State = "A"
\***
\***
\***   RETURN
\***
\******************************************************************************

INACTIVITY.SHUTDOWN:                                                    ! DSW

      SB.MESSAGE$ = "PDT Support - Inactivity shutdown."                ! DSW
      GOSUB SB.BG.MESSAGE                                               ! DSW
      IF HOLD.FLAG$ = "Y" THEN GOSUB RELEASE.PDT                        ! DSW
      GOSUB CLOSE.OPEN.FILES                                            ! DSW




      IF END# SXTCF.SESS.NUM% THEN OPEN.ERROR                           ! 1.5
      CURR.SESS.NUM% = SXTCF.SESS.NUM%                                  ! 1.5
      OPEN SXTCF.FILE.NAME$ DIRECT RECL SXTCF.RECL% AS                  \ 1.5
           SXTCF.SESS.NUM% NODEL                                        ! 1.5
      RC% = READ.SXTCF                                                  ! 1.5
      IF RC% <> 0 THEN GOTO READ.ERROR                                  ! 1.5
      CLOSE SXTCF.SESS.NUM%                                             ! 1.5
      IF SXTCF.STOCKTAKE.IN.PROGRESS$ <> "Y" THEN BEGIN                 ! 1.5
           RECEIVE.STATE$ = "A"                                         ! 1.5
      ENDIF ELSE BEGIN                                                  ! 1.5
           NOW% = FN.SECONDS(TIME$)                                     ! 1.5
           IF LAST.STOCKTAKE.DATE$ = DATE$ THEN BEGIN                   ! 1.5
              IF (NOW% - LAST.STOCKTAKE%) > INACTIVITY.SHUTDOWN% THEN   \ 1.5
                  RECEIVE.STATE$ = "A"                                  ! 1.5
           ENDIF ELSE BEGIN                                             ! 1.5
              IF NOW% > INACTIVITY.SHUTDOWN% THEN RECEIVE.STATE$ = "A"  ! 1.5
           ENDIF                                                        ! 1.5
       ENDIF                                                            ! 1.5


RETURN

\*****************************************************************************
\***
\***   RECREATE.DRSMQ:
\***
\***     To solve potential problems with transmission failure followed by
\***     an LDT reboot. i.e. making sure there is no data in the DRSMQ.
\***
\*****************************************************************************

   RECREATE.DRSMQ:                                                      ! JLC

      CURR.SESS.NUM% = DRSMQ.SESS.NUM%                                  ! JLC

      IF DRSMQ.OPEN.FLAG$ = "Y" THEN BEGIN                              ! JLC
         CLOSE DRSMQ.SESS.NUM%                                          ! JLC
         DRSMQ.OPEN.FLAG$ = "N"                                         ! JLC
      ENDIF                                                             ! JLC

      IF END #DRSMQ.SESS.NUM% THEN CREATE.ERROR                         ! JLC
      CREATE POSFILE DRSMQ.FILE.NAME$ AS DRSMQ.SESS.NUM%                \ JLC
             BUFFSIZE 10240 LOCKED                                      ! JLC
      DRSMQ.OPEN.FLAG$ = "Y"                                            ! JLC

   RETURN

\******************************************************************************
\***
\***   REQUEST.PROGRAM.LOAD:
\***
\***      display message "PDT Support - LDT Program request received"
\***      GOSUB SB.BG.MESSAGE
\***
\***      send an "E" to PSS38 to begin program load
\***
\***   RETURN
\***
\******************************************************************************

   REQUEST.PROGRAM.LOAD:                                                ! HDS

      SB.MESSAGE$ = "PDT Support - LDT Program request received"        ! HDS
      GOSUB SB.BG.MESSAGE                                               ! HDS

      PIPE.OUT$ = "E"                                                   ! HDS
      GOSUB SEND.TO.PSS38                                               ! HDS

   RETURN                                                               ! HDS

\******************************************************************************
\***
\***   RECEIVED.LOG.ON:                                         STATE : B
\***
\***      if received data is not valid then set the RECEIVE.STATE$ to "*" and
\***      return
\***
\***      GOSUB TIDY.UP
\***
\***      if the received store number is correct and this PDT has not sent
\***                two logons close together then
\***         gosub LOCK.APPLICATION.FILE
\***         if successful
\***            transmit a positive log-on acknowledgment
\***         else
\***            transmit a negative log-on acknowledgement
\***         endif
\***      else
\***         transmit a negative log-on acknowledgment
\***         set RECEIVE.STATE$ to "*"
\***         return
\***      endif
\***
\***      set up CURR.TERMINAL$ as PDT terminal number
\***
\***   RETURN
\***
\******************************************************************************

   RECEIVED.LOG.ON:

      SB.MESSAGE$ = "PDT Support - Log-on received"                     ! DJAL
      GOSUB SB.BG.MESSAGE

      IF FN.VALIDATE.DATA(DATA.IN$, 1) = 0 THEN BEGIN
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF

      GOSUB TIDY.UP
      DATE.TODAY$ = DATE$
      CURR.TERMINAL$ = MID$(DATA.IN$,6,6)
      TIDY.FLAG$ = "N"
      APPL$ = APPL.TABLE$(VAL(MID$(DATA.IN$,12,2)))                     ! HDS
      APPLICATION.NO$ = MID$(DATA.IN$,12,2)                             ! ILC

      STORE.NUMBER.INCORRECT = FALSE                                    ! 1.9DA
      STOCKTAKING.ALTERNATIVE.STORE = FALSE                             ! 1.9DA

      TIME.DIFFERENCE = FN.SECONDS(TIME$) - FN.SECONDS(LOG.ON.TIME$)
      IF MID$(DATA.IN$, 2, 4) = SAVED.STORE.NUMBER$ THEN BEGIN          ! 1.9DA
         IF LOG.ON.DATE$ <> DATE$                                       \ CSW
         OR TIME.DIFFERENCE > LOG.ON.DISABLE%                           \ ILC
         OR TIME.DIFFERENCE < 0 THEN BEGIN                              ! CSW
            NAK.LINE.1$ = ""                                            ! DSW
            NAK.LINE.2$ = ""                                            ! DSW
            NAK.LINE.3$ = ""                                            ! DSW
            GOSUB LOCK.APPLICATION.FILE                                 ! DJAL
            IF SUCCESS.FLAG$ = "Y" THEN BEGIN                           ! DJAL
               GOSUB TRANSMIT.POSITIVE.LOG.ON
               GOSUB SET.UP.AUDIT.FILE                                  ! OSMG
               LOG.ON.DATE$ = DATE$
               LOG.ON.TIME$ = TIME$
            ENDIF ELSE BEGIN                                            ! DJAL
               GOSUB TRANSMIT.NEGATIVE.LOG.ON                           ! DJAL
               GOSUB SET.UP.AUDIT.FILE                                  ! OSMG
               RECEIVE.STATE$ = "*"                                     ! DJAL
               LOG.ON.DATE$ = DATE$                                     ! ILC
               LOG.ON.TIME$ = TIME$                                     ! ILC
               RETURN                                                   ! DJAL
            ENDIF                                                       ! DJAL
         ENDIF ELSE RECEIVE.STATE$ = "*"                                ! LMJK
      ENDIF ELSE BEGIN
         STORE.NUMBER.INCORRECT = TRUE                                  ! 1.9DA
         IF APPLICATION.NO$ = "08" THEN BEGIN                           ! 1.9DA
            CURR.SESS.NUM% = SXTCF.SESS.NUM%                            ! 1.9DA
            IF END #SXTCF.SESS.NUM% THEN OPEN.ERROR                     ! 1.9DA
            OPEN SXTCF.FILE.NAME$ DIRECT RECL SXTCF.RECL% AS            \ 1.9DA
                 SXTCF.SESS.NUM% NODEL                                  ! 1.9DA
            RC% = READ.SXTCF                                            ! 1.9DA
            IF RC% <> 0 THEN GOTO READ.ERROR                            ! 1.9DA
            CLOSE SXTCF.SESS.NUM%                                       ! 1.9DA
            IF SXTCF.STOCKTAKE.IN.PROGRESS$ = "N" THEN BEGIN            ! 1.9DA
               STORE.NUMBER.INCORRECT = FALSE                           ! 1.9DA
               STOCKTAKING.ALTERNATIVE.STORE = TRUE                     ! 1.9DA
               STORE.NUMBER$ = MID$(DATA.IN$, 2, 4)                     ! 1.9DA
               NAK.LINE.1$ = ""                                         ! 1.9DA
               NAK.LINE.2$ = ""                                         ! 1.9DA
               NAK.LINE.3$ = ""                                         ! 1.9DA
               GOSUB LOCK.APPLICATION.FILE                              ! 1.9DA
               IF SUCCESS.FLAG$ = "Y" THEN BEGIN                        ! 1.9DA
                  GOSUB TRANSMIT.POSITIVE.LOG.ON                        ! 1.9DA
                  GOSUB SET.UP.AUDIT.FILE                               ! 1.9DA
                  LOG.ON.DATE$ = DATE$                                  ! 1.9DA
                  LOG.ON.TIME$ = TIME$                                  ! 1.9DA
               ENDIF ELSE BEGIN                                         ! 1.9DA
                  GOSUB TRANSMIT.NEGATIVE.LOG.ON                        ! 1.9DA
                  GOSUB SET.UP.AUDIT.FILE                               ! 1.9DA
                  RECEIVE.STATE$ = "*"                                  ! 1.9DA
                  LOG.ON.DATE$ = DATE$                                  ! 1.9DA
                  LOG.ON.TIME$ = TIME$                                  ! 1.9DA
               ENDIF                                                    ! 1.9DA
               RETURN                                                   ! 1.9DA
            ENDIF ELSE BEGIN                                            ! 1.9DA
               NAK.LINE.1$ = "A stocktake is"                           ! 1.9DA
               NAK.LINE.2$ = "already in"                               ! 1.9DA
               NAK.LINE.3$ = "progress."                                ! 1.9DA
               GOSUB TRANSMIT.NEGATIVE.LOG.ON                           ! 1.9DA
               GOSUB SET.UP.AUDIT.FILE                                  ! 1.9DA
               RECEIVE.STATE$ = "*"                                     ! 1.9DA
               RETURN                                                   ! 1.9DA
            ENDIF                                                       ! 1.9DA
         ENDIF                                                          ! 1.9DA
      ENDIF                                                             ! 1.9DA

      IF STORE.NUMBER.INCORRECT = TRUE THEN BEGIN                       ! 1.9DA
         NAK.LINE.1$ = "The store number"                               ! DSW
         NAK.LINE.2$ = "is incorrect in"                                ! DSW
         IF APPL$ = "DIRECT" OR APPL$ = "UOD" OR APPL$ = "RETURNS"      \ MMJK
          THEN NAK.LINE.3$ = "this LDT." ELSE NAK.LINE.3$ = "this PDT." ! MMJK
         GOSUB TRANSMIT.NEGATIVE.LOG.ON
         GOSUB SET.UP.AUDIT.FILE                                        ! OSMG
         RECEIVE.STATE$ = "*"
         RETURN
      ENDIF                                                             ! 1.9DA

   RETURN

\******************************************************************************
\***
\***   SET UP AUDIT FILE
\***
\******************************************************************************

   SET.UP.AUDIT.FILE:                                                   ! OSMG

      TEMP.TIME$ = TIME$                                                ! NMJK
      START.TIME% = (VAL(LEFT$(TEMP.TIME$,2))*3600) +                   \ NMJK
                    (VAL(MID$(TEMP.TIME$,3,2))*60) +                    \ NMJK
                     VAL(RIGHT$(TEMP.TIME$,2))                          ! NMJK
      LDTAF.DURATION% = START.TIME%                                     ! MMJK
      CSR.AUDIT.DATA$ = STRING$(80,"-")                                 ! EPAB
      GOSUB LOG.TO.AUDIT.FILE                                           ! EPAB
      IF APPL$ = "UOD" OR APPL$ = "DIRECT" OR APPL$ = "RETURNS"         \ MMJK
         THEN DEVICE$ = "LDT " ELSE DEVICE$ = "PDT "                    ! MMJK
      CSR.AUDIT.DATA$ = "Log on received from " + DEVICE$ +             \ LLC
         CURR.TERMINAL$ +                                               \ EPAB
         " at " + LEFT$(TIME$,2) + ":" + MID$(TIME$,3,2) + " on " +     \ EPAB
         LEFT$(DATE$,2) + "/" + MID$(DATE$,3,2) + "/" + RIGHT$(DATE$,2) \ EPAB
         + " for application " + APPL$                                  ! EPAB
      GOSUB LOG.TO.AUDIT.FILE                                           ! EPAB

   RETURN                                                               ! OSMG

\******************************************************************************
\***
\***   LOAD.APPLICATION.TABLE:
\***
\***       dimension application table for 27 entries
\***
\***       load table entries
\***
\***   RETURN
\***
\******************************************************************************

   LOAD.APPLICATION.TABLE:                                              ! HDS

       DIM APPL.TABLE$(27)                                              ! HDS

       APPL.TABLE$(1) = "EPSOM"                                         ! HDS
\      APPL.TABLE$(2) = "CSR"                                           ! HDS !2.9NWB
       APPL.TABLE$(2) = "ASN"                                           ! 2.9NWB
       APPL.TABLE$(3) = "PRICE CHECK"                                   ! HDS
       APPL.TABLE$(4) = ""                                              ! HDS
       APPL.TABLE$(5) = "DIRECT"                                        ! HDS
       APPL.TABLE$(6) = "UOD"                                           ! LLC
       APPL.TABLE$(7) = "RETURNS"                                       ! MMJK
       APPL.TABLE$(8) = "STOCKTAKE"                                     ! 1.5
       APPL.TABLE$(9) = "STOCKCOUNT"                                    ! 2,1BG
       APPL.TABLE$(10) = "PHARMACY"                                     ! 2.2JAS
       APPL.TABLE$(11) = "RECALLS"                                      ! 2.10BG
       APPL.TABLE$(12) = "+UOD"                                         ! 2.12SH
       FOR INDX% = 13 TO 26                                             ! 2.12SH
           APPL.TABLE$(INDX%) = ""                                      ! HDS
       NEXT INDX%                                                       ! HDS
       APPL.TABLE$(27) = "LOAD UNITS"                                   ! HDS

   RETURN                                                               ! HDS


\******************************************************************************
\***
\***   CALL.OTHER.MODULE:                               STATES : J - Q
\***
\***      call module PSS3701 which contains the processing instructions
\***      for these record types
\***
\***   RETURN
\***
\******************************************************************************

   CALL.OTHER.MODULE:                                                   ! DJAL

      GOSUB ALLOCATE.MODULE.1                                           ! 1.9DA

      CALL PSS3701                                                      ! DJAL
      TIDY.FLAG$ = "N"                                                  ! DJAL

      GOSUB DEALLOCATE.MODULE.1                                         ! 1.9DA

   RETURN                                                               ! DJAL

\******************************************************************************
\***
\***   CALL.OTHER.MODULE2:                              STATES : R - f
\***
\***      call module PSS3702 which contains the processing instructions
\***      for these record types
\***
\***   RETURN
\***
\******************************************************************************

   CALL.OTHER.MODULE2:                                                  ! HDS

    IF RECEIVE.STATE$ = "*" THEN BEGIN                                  ! HDS
       RETURN                                                           ! HDS
    ENDIF                                                               ! HDS

    CALL PSS3702                                                        ! HDS
    TIDY.FLAG$ = "N"                                                    ! HDS

   RETURN                                                               ! HDS

\*****************************************************************************
\***
\***   CALL.OTHER.MODULE3:                            Data States C - I
\***
\***       EPSOM routines
\***
\*****************************************************************************

CALL.OTHER.MODULE3:

   IF RECEIVE.STATE$ = "*" THEN BEGIN                                   ! LLC
      RETURN                                                            ! LLC
   ENDIF                                                                ! LLC

   GOSUB ALLOCATE.MODULE.3                                              ! 1.9DA

   CALL PSS3703                                                         ! LLC
   TIDY.FLAG$ = "N"                                                     ! LLC

   GOSUB DEALLOCATE.MODULE.3                                            ! 1.9DA

RETURN

\*****************************************************************************
\***
\***   CALL.OTHER.MODULE4:                            Data States g - o
\***
\***       RETURNS routines
\***
\*****************************************************************************

CALL.OTHER.MODULE4:

   IF RECEIVE.STATE$ = "*" THEN BEGIN                                   ! MMJK
      RETURN                                                            ! MMJK
   ENDIF                                                                ! MMJK

   GOSUB ALLOCATE.MODULE.4                                              ! 1.9DA

   CALL PSS3704                                                         ! MMJK
   TIDY.FLAG$ = "N"                                                     ! MMJK

   GOSUB DEALLOCATE.MODULE.4                                            ! 1.9DA

RETURN


\*****************************************************************************
\***
\***   CALL.OTHER.MODULE5:                            Data States s - z
\***
\***       STOCKCOUNT routines
\***
\*****************************************************************************

CALL.OTHER.MODULE5:                                                     ! 2.1BG

   IF RECEIVE.STATE$ = "*" THEN BEGIN                                   ! 2.1BG
      RETURN                                                            ! 2.1BG
   ENDIF                                                                ! 2.1BG
   
   GOSUB ALLOCATE.MODULE.5                                              ! 2.1BG
   
   CALL PSS3705                                                         ! 2.1BG
   TIDY.FLAG$ = "N"                                                     ! 2.1BG

   GOSUB DEALLOCATE.MODULE.5                                            ! 2.1BG

RETURN                                                                  ! 2.1BG


\*****************************************************************************
\*****************************************************************************
\***
\***   C S R    S U B R O U T I N E S
\***
\*****************************************************************************
\*****************************************************************************

\******************************************************************************
\***
\***   WRITE.DATA.TO.CSR.BUFFER: (removed v2.9)
\***
\***     display message to background screen
\***     unconditionally write the data received from the PDT to
\***        the buffer (checking of data will all be done in
\***        module 01)
\***
\***   RETURN
\***
\******************************************************************************

\  WRITE.DATA.TO.CSR.BUFFER:                                            ! DJAL!2.9NWB

\     CSRBF.DATA$ = DATA.IN$                                            ! DJAL!2.9NWB
\     LAST.ACTIVE.DATE$ = DATE$                                         ! DSW !2.9NWB
\     LAST.ACTIVE% = FN.SECONDS(TIME$)                                  ! DSW !2.9NWB

\     RC% = WRITE.CSRBF                                                 ! ILC !2.9NWB
\     IF RC% = 1 THEN GOTO WRITE.ERROR                                  ! ILC !2.9NWB

\  RETURN                                                               ! DJAL!2.9NWB

\******************************************************************************
\***
\***   RECEIVED.CSR.EOT: (removed v2.9)
\***
\***     gosub WRITE.DATA.TO.CSR.BUFFER
\***     close the buffer
\***     gosub CALL.OTHER.MODULE
\***
\***   RETURN
\***
\******************************************************************************

\  RECEIVED.CSR.EOT:                                                    ! DJAL!2.9NWB

\     IF APPLICATION.NO$ <> "02" THEN BEGIN                             ! DSW !2.9NWB
\        RECEIVE.STATE$ = "A"                                           ! DSW !2.9NWB
\        RETURN                                                         ! DSW !2.9NWB
\     ENDIF                                                             ! DSW !2.9NWB

\     LAST.ACTIVE.DATE$ = DATE$                                         ! DSW !2.9NWB
\     LAST.ACTIVE% = FN.SECONDS(TIME$)                                  ! DSW !2.9NWB

\     IF FIRST.EOT.FOR.THIS.PDT$ = "N" THEN BEGIN                       ! DSW !2.9NWB
\        GOSUB CALL.OTHER.MODULE                                        ! JLC !2.9NWB
\        START.TIME% = ( (VAL(LEFT$(TIME$,2))*3600) +                   \ NMJK!2.9NWB
\                        (VAL(MID$(TIME$,3,2))*60) +                    \ NMJK!2.9NWB
\                         VAL(MID$(TIME$,3,2)) ) - START.TIME%          ! NMJK!2.9NWB
\        CSR.AUDIT.DATA$ = "Link Duration was " + STR$(START.TIME%)     \ EPAB!2.9NWB
\           + " seconds "                                               ! EPAB!2.9NWB
\        CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +          \ EPAB!2.9NWB
\                          CSR.AUDIT.DATA$                              ! EPAB!2.9NWB
\        GOSUB LOG.ABORT.TO.AUDIT.FILE                                  ! EPAB!2.9NWB
\        RE.CHAIN = TRUE                                                ! ILC !2.9NWB
\        RECEIVE.STATE$ = "?"                                           ! ILC !2.9NWB
\        RETURN                                                         ! DSW !2.9NWB
\     ENDIF                                                             ! DSW !2.9NWB

\     CSR.AUDIT.DATA$ = "Received last List from PDT " +                \ EPAB!2.9NWB
\         CURR.TERMINAL$ + ", PDT contained " + MID$(DATA.IN$,9,3) +    \ EPAB!2.9NWB
\         " lists "                                                     ! EPAB!2.9NWB
\     CSR.AUDIT.DATA$ = "[PORT " + MONITORED.PORT$ + "] " +             \ EPAB!2.9NWB
\                       CSR.AUDIT.DATA$                                 ! EPAB!2.9NWB
\     GOSUB LOG.TO.AUDIT.FILE                                           ! EPAB!2.9NWB
\     GOSUB WRITE.DATA.TO.CSR.BUFFER                                    ! DJAL!2.9NWB

\     CLOSE CSRBF.SESS.NUM%                                             ! DJAL!2.9NWB
\     CSRBF.OPEN.FLAG$ = "N"                                            ! DJAL!2.9NWB

\     GOSUB HOLD.PDT                                                    ! DJAL!2.9NWB
\     GOSUB CALL.OTHER.MODULE                                           ! JLC !2.9NWB

\  RETURN                                                               ! DJAL!2.9NWB

\******************************************************************************
\******************************************************************************
\***
\***   P D T    S U B R O U T I N E S
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   LOCK.APPLICATION.FILE:
\***
\***      wait for allocated time slot (to circumvent file LOCKing problem)
\***      attempt to lock a file specific to the application, indicated
\***      by the application number in the logon record
\***
\***      Initialise CREDIT.CLAIM.FLAG$ = "NN" and STORE.CLOSE.FLAG$ = "O"
\***
\***      if this is the CSR application and the deliveries logging suite
\***      has not yet run then the PDT logon will fail
\***
\***      if the application is RETURNS then perform validation of log on
\***      attempt, if conditions are not valid then set flag to indicate.
\***      Check SOFTS record 43, if this shows that CREDIT CLAIMING IS ACTIVE
\***      and which rejctions to be reported set to ALL set CREDIT.CLAIM.FLAG$ = "YY"
\***      else set CREDIT.CLAIM.FLAG$ = "YN".  If LEFT$(CREDIT.CLAIM.FLAG$,1) = "Y" then check
\***      Store open status by checking the 9999 key record on the TSF
\***      If the TSF.INDICATO% = -1 then set the STORE.CLOSE.FLAG$ = "C" (Store
\***      closed)
\***
\***      If the Price Check application, then PSS37 checks that the
\***      printer and labeller are switched on.
\***
\***      If the Stocktake application then check for stocktake in progress.
\***
\***   RETURN
\***
\***   LOCKED.FILE
\***
\***      file was locked - attempt unsuccessful
\***
\***   RETURN
\***
\******************************************************************************

   LOCK.APPLICATION.FILE:                                               ! DJAL

      GOSUB STAGGER.PORT                                                ! DSW

      CREDIT.CLAIM.FLAG$ = "NN"                                         ! 2.3JAS 2.4JAS
      STORE.CLOSE.FLAG$  = "O"                                          ! 2.3JAS

      IF APPLICATION.NO$ = "01" THEN BEGIN                              ! epsom
        CURR.SESS.NUM% = EPSOM.SESS.NUM%                                ! DSW
        IF END #EPSOM.SESS.NUM% THEN OPEN.ERROR                         ! DSW
        OPEN EPSOM.FILE.NAME$ AS EPSOM.SESS.NUM% LOCKED                 ! DSW
        EPSOM.OPEN.FLAG$ = "Y"                                          ! DSW
        ALL.FILES.CLOSED$ = "N"                                         ! DSW
      ENDIF ELSE IF APPLICATION.NO$ = "02" THEN BEGIN                   ! DSW
\       CURR.SESS.NUM% = CSR.SESS.NUM%                                  ! DSW !2.9NWB
\       IF END #CSR.SESS.NUM% THEN OPEN.ERROR                           ! DSW !2.9NWB
\       OPEN CSR.FILE.NAME$ AS CSR.SESS.NUM% LOCKED                     ! DSW !2.9NWB
\       CSR.OPEN.FLAG$ = "Y"                                            ! DSW !2.9NWB
\       ALL.FILES.CLOSED$ = "N"                                         ! DSW !2.9NWB
        IF (NOT ASN.ACTIVE%) THEN BEGIN                                 ! 2.9NWB
           RECEIVE.STATE$ = "*"                                         ! 2.9NWB
           SUCCESS.FLAG$ = "N"                                          ! 2.9NWB
           NAK.LINE.1$ = "Carton Delivery"                              ! 2.9NWB
           NAK.LINE.2$ = "System is"                                    ! 2.9NWB
           NAK.LINE.3$ = "Inactive."                                    ! 2.9NWB
           RETURN                                                       ! 2.9NWB
        ENDIF                                                           ! 2.9NWB
      ENDIF ELSE IF APPLICATION.NO$ = "03" THEN BEGIN                   ! pchk
        CURR.SESS.NUM% = PCHK.SESS.NUM%                                 ! DSW
        IF END #PCHK.SESS.NUM% THEN OPEN.ERROR                          ! DSW
        OPEN PCHK.FILE.NAME$ DIRECT RECL 1 AS PCHK.SESS.NUM% LOCKED     ! DSW
        PCHK.OPEN.FLAG$ = "Y"                                           ! DSW
        ALL.FILES.CLOSED$ = "N"                                         ! DSW
      ENDIF ELSE IF APPLICATION.NO$ = "05" THEN BEGIN                   ! HDS
        CURR.SESS.NUM% = DIREC.SESS.NUM%                                ! HDS
        IF END #DIREC.SESS.NUM% THEN OPEN.ERROR                         ! HDS
        OPEN DIREC.FILE.NAME$ AS DIREC.SESS.NUM% LOCKED                 ! HDS
        DIREC.OPEN.FLAG$ = "Y"                                          ! HDS
        ALL.FILES.CLOSED$ = "N"                                         ! HDS
      ENDIF ELSE IF APPLICATION.NO$ = "06" THEN BEGIN                   ! LLC
        CURR.SESS.NUM% = UOD.SESS.NUM%                                  ! LLC
        IF END #UOD.SESS.NUM% THEN OPEN.ERROR                           ! LLC
        OPEN UOD.FILE.NAME$ AS UOD.SESS.NUM% LOCKED                     ! LLC
        UOD.OPEN.FLAG$ = "Y"                                            ! LLC
        ALL.FILES.CLOSED$ = "N"                                         ! LLC
      ENDIF ELSE IF APPLICATION.NO$ = "07" THEN BEGIN                   ! MMJK
        CURR.SESS.NUM% = CCDMY.SESS.NUM%                                ! MMJK
        IF END #CCDMY.SESS.NUM% THEN OPEN.ERROR                         ! MMJK
        OPEN CCDMY.FILE.NAME$ AS CCDMY.SESS.NUM% LOCKED                 ! MMJK
        CCDMY.OPEN.FLAG$ = "Y"                                          ! MMJK
        ALL.FILES.CLOSED$ = "N"                                         ! MMJK
      ENDIF ELSE IF APPLICATION.NO$ = "08" THEN BEGIN                   ! 1.5
        CURR.SESS.NUM% = STKTK.SESS.NUM%                                ! 1.5
        IF END #STKTK.SESS.NUM% THEN OPEN.ERROR                         ! 1.5
        OPEN STKTK.FILE.NAME$ AS STKTK.SESS.NUM% LOCKED                 ! 1.5
        STKTK.OPEN.FLAG$ = "Y"                                          ! 1.5
      ENDIF ELSE IF APPLICATION.NO$ = "09" THEN BEGIN                   ! 2.1BG
        SUCCESS.FLAG$ = "Y"                                             ! 2.1BG
      ENDIF ELSE IF APPLICATION.NO$ = "10" THEN BEGIN                   ! 2.2JAS
        SUCCESS.FLAG$ = "Y"                                             ! 2.2JAS
      ENDIF ELSE IF APPLICATION.NO$ = "11" THEN BEGIN                   ! 2.10BG
        SUCCESS.FLAG$ = "Y"                                             ! 2.10BG
      ENDIF ELSE IF APPLICATION.NO$ = "12" THEN BEGIN                   ! 2.12SH
        SUCCESS.FLAG$ = "Y"                                             ! 2.12SH
      ENDIF ELSE BEGIN                                                  ! 1.5
        SUCCESS.FLAG$ = "N"                                             ! DSW
        RECEIVE.STATE$ = "*"                                            ! DSW
        RETURN                                                          ! DSW
      ENDIF

\     IF APPLICATION.NO$ = "02" OR                                      \ 1.2 !2.9NWB
\        APPLICATION.NO$ = "04" THEN BEGIN                              ! DSW !2.9NWB

\               SB.INTEGER% = INVOK.REPORT.NUM%                         !1.9DA!2.9NWB
\               SB.STRING$ = INVOK.FILE.NAME$                           !1.9DA!2.9NWB
\               GOSUB SB.FILE.UTILS                                     !1.9DA!2.9NWB
\               INVOK.SESS.NUM% = SB.FILE.SESS.NUM%                     !1.9DA!2.9NWB

\               CURR.SESS.NUM% = INVOK.SESS.NUM%                        ! DSW !2.9NWB
\               IF END #INVOK.SESS.NUM% THEN OPEN.ERROR                 ! DSW !2.9NWB
\               OPEN INVOK.FILE.NAME$ DIRECT RECL 80 AS INVOK.SESS.NUM% \ DSW !2.9NWB
\                    NOWRITE NODEL                                      ! DSW !2.9NWB
\               INVOK.OPEN.FLAG$ = "Y"                                  ! DSW !2.9NWB
\               RC% = READ.INVOK                                        ! ILC !2.9NWB
\               IF RC% = 1 THEN GOTO READ.ERROR                         ! ILC !2.9NWB
\               CLOSE INVOK.SESS.NUM%                                   ! DSW !2.9NWB
\               INVOK.OPEN.FLAG$ = "N"                                  ! DSW !2.9NWB

\        ALLOW.CSR.PROCESSING = TRUE                                    ! 1.2 !2.9NWB

\        IF INVOK.CSR.CONVERSION.STATUS.FLAG$ = "S" OR                  \ 1.2 !2.9NWB
\           INVOK.CSR.CONVERSION.STATUS.FLAG$ = "C" THEN BEGIN          ! 1.2 !2.9NWB
\           ALLOW.CSR.PROCESSING = FALSE                                ! 1.2 !2.9NWB
\        ENDIF ELSE BEGIN                                               ! 1.2 !2.9NWB
\           IF INVOK.CSR.CONVERSION.STATUS.FLAG$ = "X" THEN BEGIN       ! 1.2 !2.9NWB
\              CURR.SESS.NUM% = SOFTS.SESS.NUM%                         ! 1.2 !2.9NWB
\              IF END #SOFTS.SESS.NUM% THEN OPEN.ERROR                  ! 1.2 !2.9NWB
\              OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL%            \ 1.2 !2.9NWB
\                   AS SOFTS.SESS.NUM% NOWRITE NODEL                    ! 1.2 !2.9NWB
\              SOFTS.OPEN.FLAG$ = "Y"                                   ! 1.2 !2.9NWB
\              SOFTS.REC.NUM% = 6                                       ! 1.2 !2.9NWB
\              IF READ.SOFTS = 0 THEN BEGIN                             ! 1.2 !2.9NWB
\                 IF (LEFT$(SOFTS.RECORD$,11) = "CSR PHASE 2") THEN BEGIN !1.2!2.9NWB
\                    ALLOW.CSR.PROCESSING = FALSE                       ! 1.2 !2.9NWB
\                 ENDIF                                                 ! 1.2 !2.9NWB
\              ENDIF ELSE ALLOW.CSR.PROCESSING = FALSE                  ! 1.2 !2.9NWB
\              CLOSE SOFTS.SESS.NUM%                                    ! 1.2 !2.9NWB
\              SOFTS.OPEN.FLAG$ = "N"                                   ! 1.2 !2.9NWB
\            ENDIF                                                      ! 1.2 !2.9NWB
\         ENDIF                                                         ! 1.2 !2.9NWB

\        IF (NOT ALLOW.CSR.PROCESSING) THEN BEGIN                       ! 1.2 !2.9NWB
\            NAK.LINE.1$ = "Link Inactive."                             ! 1.2 !2.9NWB
\            NAK.LINE.2$ = "Press ENTER and"                            ! 1.2 !2.9NWB
\            NAK.LINE.3$ = "FUNC Q to quit."                            ! 1.2 !2.9NWB
\            SUCCESS.FLAG$ = "N"                                        ! 1.2 !2.9NWB
\            RECEIVE.STATE$ = "*"                                       ! 1.2 !2.9NWB
\            RETURN                                                     ! 1.2 !2.9NWB
\        ENDIF                                                          ! 1.2 !2.9NWB


\        IF APPLICATION.NO$ <> "04" THEN BEGIN                          ! 1.2 !2.9NWB
\               IF INVOK.CSR.DELIVERY.DATE$ = DATE$ THEN BEGIN          ! DSW !2.9NWB
\                     GOSUB CHECK.FOR.WORK.FILE                         ! DSW !2.9NWB
\                     FIRST.CSR.LIST.FOR.PDT$ = "Y"                     ! DSW !2.9NWB
\                     FIRST.EOT.FOR.THIS.PDT$ = "Y"                     ! DSW !2.9NWB
\                     GOSUB CREATE.BUFFER                               ! DSW !2.9NWB
\                  ENDIF ELSE BEGIN                                     ! DSW !2.9NWB
\                     SUCCESS.FLAG$ = "N"                               ! DSW !2.9NWB
\                     NAK.LINE.1$ = "Link unavailable"                  ! DSW !2.9NWB
\                     NAK.LINE.2$ = "due to delivery"                   ! DSW !2.9NWB
\                     NAK.LINE.3$ = "logging failure"                   ! DSW !2.9NWB
\                     RECEIVE.STATE$ = "*"                              ! DSW !2.9NWB
\                     RETURN                                            ! DSW !2.9NWB
\                  ENDIF                                                ! DSW !2.9NWB
\        ENDIF                                                          ! DSW !2.9NWB

\        SB.INTEGER% = INVOK.SESS.NUM%                                  !1.9DA!2.9NWB
\        GOSUB SB.FILE.UTILS                                            !1.9DA!2.9NWB

\     ENDIF                                                                   !2.9NWB

      IF APPLICATION.NO$ = "07" THEN BEGIN                              ! MMJK
        RETURNS.LOGON.VALID = TRUE                                      ! MMJK
        CURR.SESS.NUM% = SOFTS.SESS.NUM%                                ! MMJK
        IF END #SOFTS.SESS.NUM% THEN SOFTS.FILE.ABSENT                  ! MMJK
        OPEN SOFTS.FILE.NAME$ DIRECT RECL SOFTS.RECL%                   \ MMJK
           AS SOFTS.SESS.NUM% NOWRITE NODEL                             ! MMJK
        SOFTS.OPEN.FLAG$ = "Y"                                          ! MMJK
        GOTO CHECK.SOFTS.RECORD                                         ! MMJK
        SOFTS.FILE.ABSENT:                                              ! MMJK
        RETURNS.LOGON.VALID = FALSE                                     ! MMJK
        CHECK.SOFTS.RECORD:                                             ! MMJK
        IF RETURNS.LOGON.VALID THEN BEGIN                               ! MMJK
          SOFTS.REC.NUM% = 5                                            ! MMJK
          IF READ.SOFTS = 0 THEN BEGIN                                  ! MMJK
            IF (SOFTS.RECORD$<>"CHILLED FOODS IS ACTIVE") AND           \ MMJK
            (SOFTS.RECORD$<>"RETURNS IS ACTIVE") THEN BEGIN             ! MMJK
              NAK.LINE.1$ = "Returns/Chilled"                           ! MMJK
              NAK.LINE.2$ = "Food system is"                            ! MMJK
              NAK.LINE.3$ = "inactive"                                  ! MMJK
              RETURNS.LOGON.VALID = FALSE                               ! MMJK
            ENDIF                                                       ! MMJK
          ENDIF ELSE RETURNS.LOGON.VALID = FALSE                        ! MMJK
        ENDIF                                                           ! MMJK
        IF RETURNS.LOGON.VALID THEN BEGIN                               ! 2.3JAS
           SOFTS.REC.NUM% = 43                                          ! 2.3JAS
           IF READ.SOFTS = 0 THEN BEGIN                                 ! 2.3JAS
              IF LEFT$(SOFTS.RECORD$,22) = "CREDIT CLAIM IS ACTIVE" THEN BEGIN ! 2.3JAS
                 CREDIT.CLAIM.FLAG$ = "YN"                              ! 2.3JAS 2.4JAS

                 MATCH.DELIMITER1 = MATCH(",",SOFTS.RECORD$,26)                           !2.4JAS
                 MATCH.DELIMITER2 = MATCH(",",SOFTS.RECORD$,(MATCH.DELIMITER1 + 1))       !2.4JAS
                 MATCH.DELIMITER3 = MATCH(",",SOFTS.RECORD$,(MATCH.DELIMITER2 + 1))       !2.4JAS
                 REJECTION.TYPE$  = MID$(SOFTS.RECORD$,(MATCH.DELIMITER2 + 1), \          !2.4JAS
                                        (MATCH.DELIMITER3 - (MATCH.DELIMITER2 +1)))       !2.4JAS
                 IF REJECTION.TYPE$ = "ALL" THEN BEGIN                  ! 2.4JAS
                    CREDIT.CLAIM.FLAG$ = "YY"                           ! 2.4JAS
                 ENDIF                                                  ! 2.4JAS

                 SB.ACTION$ = "O"                                       ! 2.3JAS

                 SB.INTEGER% = TSF.SESS.NUM%                            ! 2.3JAS
                 SB.STRING$ = TSF.FILE.NAME$                            ! 2.3JAS
                 GOSUB SB.FILE.UTILS                                    ! 2.3JAS
                 TSF.SESS.NUM% = SB.FILE.SESS.NUM%                      ! 2.3JAS

                 CURR.SESS.NUM% = TSF.SESS.NUM%                         ! 2.3JAS

                 IF END #TSF.SESS.NUM% THEN TSF.FILE.ABSENT             ! 2.3JAS
                 OPEN TSF.FILE.NAME$ KEYED RECL TSF.RECL%               \ 2.3JAS
                   AS TSF.SESS.NUM% NOWRITE NODEL                       ! 2.3JAS
                 TSF.OPEN.FLAG$ = "Y"                                   ! 2.3JAS
                 GOTO CHECK.TSF.RECORD                                  ! 2.3JAS
                 TSF.FILE.ABSENT:                                       ! 2.3JAS
                 RETURNS.LOGON.VALID = FALSE                            ! 2.3JAS
                 CHECK.TSF.RECORD:                                      ! 2.3JAS
                 IF RETURNS.LOGON.VALID THEN BEGIN                      ! 2.3JAS
                    TSF.TERM.STORE$ EQ PACK$("9999")                    ! 2.3JAS
                    IF READ.TSF = 0 THEN BEGIN                          ! 2.3JAS
                       IF TSF.INDICAT0% EQ -1 THEN BEGIN                ! 2.3JAS
                          STORE.CLOSE.FLAG$ = "C"                       ! 2.3JAS
                       ENDIF                                            ! 2.3JAS
                    ENDIF ELSE BEGIN                                    ! 2.3JAS
                       RETURNS.LOGON.VALID = FALSE                      ! 2.3JAS
                    ENDIF                                               ! 2.3JAS
                 ENDIF                                                  ! 2.3JAS

                 SB.ACTION$ = "C"                                       ! 2.3JAS
                 SB.STRING$ = ""                                        ! 2.3JAS

                 IF TSF.OPEN.FLAG$ = "Y" THEN BEGIN                     ! 2.3JAS
                    GOSUB SB.FILE.UTILS    ! CURR.SESS.NUM% set above   ! 2.7CS
                    CLOSE TSF.SESS.NUM%                                 ! 2.3JAS
                    TSF.OPEN.FLAG$ = "N"                                ! 2.3JAS
                 ENDIF                                                  ! 2.3JAS
              ENDIF                                                     ! 2.3JAS
           ENDIF ELSE BEGIN                                             ! 2.3JAS
              RETURNS.LOGON.VALID = FALSE                               ! 2.3JAS
           ENDIF                                                        ! 2.3JAS
        ENDIF                                                           ! 2.3JAS
        IF RETURNS.LOGON.VALID THEN BEGIN                               ! MMJK
          CURR.SESS.NUM% = CCUOD.SESS.NUM%                              ! MMJK
          IF END #CCUOD.SESS.NUM% THEN OPEN.ERROR                       ! MDS
          OPEN CCUOD.FILE.NAME$ KEYED RECL CCUOD.RECL%                  \ MMJK
             AS CCUOD.SESS.NUM%                                         ! MMJK
          CCUOD.OPEN.FLAG$ = "Y"                                        ! MMJK
          GOTO CHECK.CCUOD.HDR.RECORD                                   ! MMJK
          CCUOD.FILE.ABSENT:                                            ! MMJK
          RETURNS.LOGON.VALID = FALSE                                   ! MMJK
          CHECK.CCUOD.HDR.RECORD:                                       ! MMJK
          CCUOD.UOD.NUM$ = PACK$(STRING$(7,"??"))                       ! MMJK
          CURRENT.KEY$ = CCUOD.UOD.NUM$                                 ! MDS
          IF READ.CCUOD = 1 THEN GOTO READ.ERROR                        ! MDS
          IF (CURR.TERMINAL$<>UNPACK$(CCUOD.LDT.NUM$)) AND              \ MMJK
             (UNPACK$(CCUOD.LDT.NUM$)<>"000000") THEN BEGIN             ! MMJK
              IF SOFTS.RECORD$="CHILLED FOODS IS ACTIVE" THEN BEGIN     ! MMJK
                NAK.LINE.1$ = "Chilled Food"                            ! MMJK
                NAK.LINE.2$ = "system in use"                           ! MMJK
                NAK.LINE.3$ = "Please try later"                        ! MMJK
              ENDIF ELSE BEGIN                                          ! MMJK
                NAK.LINE.1$ = "Returns system"                          ! MMJK
                NAK.LINE.2$ = "in use"                                  ! MMJK
                NAK.LINE.3$ = "Please try later"                        ! MMJK
              ENDIF                                                     ! MMJK
              RETURNS.LOGON.VALID = FALSE                               ! MMJK
          ENDIF                                                         ! MMJK
        ENDIF                                                           ! MMJK
        IF (NOT RETURNS.LOGON.VALID) THEN BEGIN                         ! MMJK
          SUCCESS.FLAG$ = "N"                                           ! MMJK
          RECEIVE.STATE$ = "*"                                          ! MMJK
          RETURN                                                        ! MMJK
        ENDIF                                                           ! MMJK
      ENDIF                                                             ! MMJK

      IF APPLICATION.NO$ = "08" AND                                     \ 1.9DA
         STOCKTAKING.ALTERNATIVE.STORE = FALSE THEN BEGIN               ! 1.9DA
        IF END# SXTCF.SESS.NUM% THEN OPEN.ERROR                         ! 1.5
        CURR.SESS.NUM% = SXTCF.SESS.NUM%                                ! 1.5
        OPEN SXTCF.FILE.NAME$ DIRECT RECL SXTCF.RECL%                   \ 1.5
               AS SXTCF.SESS.NUM% NOWRITE NODEL                         ! 1.5

        CURR.SESS.NUM% = SXTCF.SESS.NUM%                                ! 1.5
        RC% = READ.SXTCF                                                ! 1.5
        IF RC% <> 0 THEN GOTO READ.ERROR                                ! 1.5
        CLOSE SXTCF.SESS.NUM%                                           ! 1.5

        IF SXTCF.STOCKTAKE.IN.PROGRESS$ <> "Y" THEN BEGIN               ! 1.5
               NAK.LINE.1$ = "Stocktake not"                            ! 1.5
               NAK.LINE.2$ = "in progress."                             ! 1.5
               NAK.LINE.3$ = "Log-on not valid."                        ! 1.5
               SUCCESS.FLAG$ = "N"                                      ! 1.5
               RECEIVE.STATE$ = "*"                                     ! 1.5
               RETURN                                                   ! 1.5
        ENDIF                                                           ! 1.5

      ENDIF                                                             ! 1.5

      SUCCESS.FLAG$ = "Y"                                               ! DJAL
      EXP.STATES$ = LOGON.TAB$(VAL(APPLICATION.NO$))                    ! DJAL

   RETURN                                                               ! DJAL

LOCKED.FILE:                                                            ! DJAL

      NAK.LINE.1$ = "Application is"                                    ! DSW
      NAK.LINE.2$ = "already in use."                                   ! DSW
      NAK.LINE.3$ = "Please try later"                                  ! DSW
      SUCCESS.FLAG$ = "N"                                               ! DSW

   RETURN                                                               ! DJAL

\******************************************************************************
\***
\***   STAGGER.PORT:
\***
\***      if the other port calls this routine at exactly the same time as
\***      this port then ensure ONE port has to wait before return
\***      N.B This is a temperary workround to the file-lock problem (when
\***      two applications simultaneously attempt to open a file, BOTH
\***      applications get the file !)
\***
\***   RETURN
\***
\******************************************************************************

   STAGGER.PORT:                                                        ! DSW


      WHILE MOD(VAL(TIME$), 2) <> (ASC(MONITORED.PORT$)-ASC("A"))       ! DSW
        WAIT ; 200                                                      ! DSW
      WEND                                                              ! DSW


   RETURN                                                               ! DSW
\******************************************************************************
\***
\***   CHECK.FOR.WORK.FILE:
\***
\***      hold PDT
\***      process CSR work file if it exists and is to be processed
\***      release PDT
\***
\***   RETURN
\***
\******************************************************************************

\  CHECK.FOR.WORK.FILE:                                                 ! DSW !2.9NWB

\     GOSUB HOLD.PDT                                                    ! DSW !2.9NWB
\     CURR.SESS.NUM% = CSRWF.SESS.NUM%                                  ! DSW !2.9NWB
\     IF END# CSRWF.SESS.NUM% THEN NO.CSRWF                             ! DSW !2.9NWB
\     OPEN CSRWF.FILE.NAME$ AS CSRWF.SESS.NUM%                          ! DSW !2.9NWB
\     CSRWF.OPEN.FLAG$ = "Y"                                            ! DSW !2.9NWB
\     CLOSE CSRWF.SESS.NUM%                                             ! DSW !2.9NWB
\     CSRWF.OPEN.FLAG$ = "N"                                            ! DSW !2.9NWB
\     SB.MESSAGE$ = "PDT Support - Processing CSR work file"            ! DSW !2.9NWB
\     GOSUB SB.BG.MESSAGE                                               ! DSW !2.9NWB
\     PROCESS.CSR.WORKFILE$ = "Y"                                       ! DSW !2.9NWB
\     GOSUB ALLOCATE.MODULE.1                                           !2.0DA!2.9NWB
\     CALL PSS3701                                                      ! DSW !2.9NWB
\     GOSUB DEALLOCATE.MODULE.1                                         !2.0DA!2.9NWB
\  NO.CSRWF:                                                            ! DSW !2.9NWB
\     GOSUB RELEASE.PDT                                                 ! DSW !2.9NWB

\  RETURN                                                               ! DSW !2.9NWB

\******************************************************************************
\***
\***   TRANSMIT.POSITIVE.LOG.ON:
\***
\***   If application transmitted is not Returns/Credit Claiming then the pipe out
\***      variable must not contain the credit claim flag or Store close flag
\***   If application transmitted is credit claiming then the pipe out
\***      variable must contain the credit claim flag and store close flag
\***
\***      send a positive log-on acknowledgment to PSS38 to indicate a
\***      successful log-on
\***
\***   RETURN
\***
\******************************************************************************

   TRANSMIT.POSITIVE.LOG.ON:

      T.DATE$ = DATE$
      T.TIME$ = TIME$

      ! If Application "07" (is not Returns/credit claim transmission)
      IF APPLICATION.NO$ <> "07" THEN BEGIN                              ! 2.3JAS
         PIPE.OUT$ = "L" +                                                 \
                     SOH$ +                                                \
                     FN.Z.PACK(STORE.NUMBER$, 4) +                         \
                     FN.Z.PACK(CURR.TERMINAL$, 6) +                        \
                     APPLICATION.NO$ +                                     \ DJAL
                     RIGHT$(T.DATE$,2)+MID$(T.DATE$,3,2)+LEFT$(T.DATE$,2) +\
                     LEFT$(T.TIME$,4) +                                    \
                     ACK$                                                  ! DJAL
      ENDIF ELSE BEGIN                                                     ! 2.3JAS
         ! Application is Returns/Credit claim transmission
         PIPE.OUT$ = "L" +                                                 \! 2.3JAS
                     SOH$ +                                                \! 2.3JAS
                     FN.Z.PACK(STORE.NUMBER$, 4) +                         \! 2.3JAS
                     FN.Z.PACK(CURR.TERMINAL$, 6) +                        \! 2.3JAS
                     APPLICATION.NO$ +                                     \! 2.3JAS
                     RIGHT$(T.DATE$,2)+MID$(T.DATE$,3,2)+LEFT$(T.DATE$,2) +\! 2.3JAS
                     LEFT$(T.TIME$,4) +                                    \! 2.3JAS
                     LEFT$(CREDIT.CLAIM.FLAG$,1) +                         \! 2.3JAS 2.4JAS
                     STORE.CLOSE.FLAG$ +                                   \! 2.3JAS
                     ACK$                                                  !  2.3JAS
      ENDIF                                                                !  2.3JAS

      GOSUB SEND.TO.PSS38

   RETURN

\******************************************************************************
\***
\***   CREATE.BUFFER: (removed v2.9)
\***
\***      if a buffer already exists then close it
\***      create a new buffer
\***
\***   RETURN
\***
\******************************************************************************

\  CREATE.BUFFER:                                                       ! DJAL!2.9NWB

\     CURR.SESS.NUM% = CSRBF.SESS.NUM%                                  ! DJAL!2.9NWB

\     IF CSRBF.OPEN.FLAG$ = "Y" THEN BEGIN                              ! DSW !2.9NWB
\        IF END #CSRBF.SESS.NUM% THEN DELETE.ERROR                      ! DSW !2.9NWB
\        DELETE CSRBF.SESS.NUM%                                         ! DSW !2.9NWB
\        CSRBF.OPEN.FLAG$ = "N"                                         ! DSW !2.9NWB
\     ENDIF                                                             ! DSW !2.9NWB

\     IF END #CSRBF.SESS.NUM% THEN OPEN.ERROR                           ! DJAL!2.9NWB
\     CREATE CSRBF.FILE.NAME$ AS CSRBF.SESS.NUM%                        ! DJAL!2.9NWB
\     CSRBF.OPEN.FLAG$ = "Y"                                            ! DJAL!2.9NWB

\  RETURN                                                                     !2.9NWB

\******************************************************************************
\***
\***   TRANSMIT.NEGATIVE.LOG.ON:
\***
\***      send a negative log-on acknowledgment to PSS38 to indicate an
\***      unsuccessful log-on
\***
\***   RETURN
\***
\******************************************************************************

   TRANSMIT.NEGATIVE.LOG.ON:

      PIPE.OUT$ = "L" +                                                 \
                  SOH$ +                                                \
                  FN.Z.PACK(STORE.NUMBER$,4) +                          \
                  FN.Z.PACK(CURR.TERMINAL$,6) +                         \
                  APPLICATION.NO$ +                                     \ DJAL
                  NAK$ +                                                \ DSW
                  LEFT$(NAK.LINE.1$ + "                ", 16) +         \ DSW
                  LEFT$(NAK.LINE.2$ + "                ", 16) +         \ DSW
                  LEFT$(NAK.LINE.3$ + "                ", 16)           ! DSW

      GOSUB SEND.TO.PSS38

   RETURN

\******************************************************************************
\***
\***   HOLD.PDT:
\***
\***      send a hold PDT command to PSS38
\***
\***   RETURN
\***
\******************************************************************************

   HOLD.PDT:

      PIPE.OUT$ = "HY"
      GOSUB SEND.TO.PSS38
      HOLD.FLAG$ = "Y"

   RETURN

\******************************************************************************
\***
\***   RELEASE.PDT:
\***
\***      send a release PDT command to PSS38
\***
\***   RETURN
\***
\******************************************************************************

   RELEASE.PDT:

      PIPE.OUT$ = "HN"
      GOSUB SEND.TO.PSS38
      HOLD.FLAG$ = "N"

   RETURN

\******************************************************************************
\***
\***   SEND.XON:
\***
\***      send an XON to the PDT (via PSS38), just in case the PDT has hung
\***
\***   RETURN
\***
\******************************************************************************

   SEND.XON:                                                            ! DSW

      PIPE.OUT$ = "XY"                                                  ! DSW
      GOSUB SEND.TO.PSS38                                               ! DSW

   RETURN                                                               ! DSW

\******************************************************************************
\***
\***   SEND.TO.PSS38:
\***
\***      transmit data to PSS38 (data contained in PIPE.OUT$)
\***
\***   RETURN
\***
\******************************************************************************

   SEND.TO.PSS38:

      IF END# PIPEI.SESS.NUM% THEN WRITE.ERROR
      CURR.SESS.NUM% = PIPEI.SESS.NUM%
      WRITE# PIPEI.SESS.NUM%; PIPE.OUT$

   RETURN

\******************************************************************************
\******************************************************************************
\***
\***   S H U T D O W N
\***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   SHUTDOWN:
\***
\***      stop PSS38
\***      close PSS37 - PSS38 communications
\***      de-allocate all used file session numbers
\***      display 'ended' message
\***
\***   RETURN
\***
\******************************************************************************

   SHUTDOWN:
   
      QUIT.FLAG$ = "Y"

      PIPE.OUT$ = "Q"
      GOSUB SEND.TO.PSS38

      CLOSE PIPEI.SESS.NUM%
      CLOSE PIPEO.SESS.NUM%
      CLOSE PLDT.SESS.NUM%                                              ! HDS

      SB.ACTION$ = "C"
      SB.STRING$ = ""

      SB.INTEGER% = BCSMF.SESS.NUM%
      GOSUB SB.FILE.UTILS
\     SB.INTEGER% = CITEM.SESS.NUM%                                     ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
\     SB.INTEGER% = CSR.SESS.NUM%                                       ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
\     SB.INTEGER% = CSRWF.SESS.NUM%                                     ! DJAL!2.9NWB
\     GOSUB SB.FILE.UTILS                                               ! DJAL!2.9NWB
!     SB.INTEGER% = FPF.SESS.NUM%                                       ! DJAL
!     GOSUB SB.FILE.UTILS                                               ! DJAL
      SB.INTEGER% = IDF.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = IEF.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = IRF.SESS.NUM%
      GOSUB SB.FILE.UTILS
!     SB.INTEGER% = ONORD.SESS.NUM%                                     ! DJAL
!     GOSUB SB.FILE.UTILS                                               ! DJAL
      SB.INTEGER% = PCHK.SESS.NUM%                                      ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
      SB.INTEGER% = PDTWF.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = PIITM.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = PILST.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = PIPEI.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = PIPEO.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = STKMQ.SESS.NUM%
      GOSUB SB.FILE.UTILS
      SB.INTEGER% = UNITS.SESS.NUM%                                     ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
      SB.INTEGER% = DIRORD.SESS.NUM%                                    ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = DIRSUP.SESS.NUM%                                    ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = DIRWF.SESS.NUM%                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = DIREC.SESS.NUM%                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = LDTCF.SESS.NUM%                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = PLDT.SESS.NUM%                                      ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = DRSMQ.SESS.NUM%                                     ! HDS
      GOSUB SB.FILE.UTILS                                               ! HDS
      SB.INTEGER% = EPSOM.SESS.NUM%                                     ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
      SB.INTEGER% = LDTBF.SESS.NUM%                                     ! JLC
      GOSUB SB.FILE.UTILS                                               ! JLC
      SB.INTEGER% = IDSOF.SESS.NUM%                                     ! JLC
      GOSUB SB.FILE.UTILS                                               ! JLC
      SB.INTEGER% = CCUOD.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = CCLAM.SESS.NUM%                                     ! 1.4   ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! 1.4
      SB.INTEGER% = CCITM.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = CCTRL.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = CCDMY.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = CCTMP.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = CCBUF.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = CCUPF.SESS.NUM%                                     ! NMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! NMJK
      SB.INTEGER% = CCWKF.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = LDTAF.SESS.NUM%                                     ! MMJK  ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! MMJK
      SB.INTEGER% = SXTCF.SESS.NUM%                                     ! 1.5   ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! 1.5
      SB.INTEGER% = SXTMP.SESS.NUM%                                     ! 1.5   ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! 1.5
      SB.INTEGER% = STKBF.SESS.NUM%                                     ! 1.5   ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! 1.5
      SB.INTEGER% = STKTK.SESS.NUM%                                     ! 1.5   ! 2.7CS
      GOSUB SB.FILE.UTILS                                               ! 1.5
!      SB.INTEGER% = LSSST.SESS.NUM%                                     !1.8BG  ! 2.7CS
!      GOSUB SB.FILE.UTILS                                               !1.8BG  ! 2.7CS
      SB.INTEGER% = GAPBF.SESS.NUM%                                     ! DJAL !2.5CS !2.6BG
      GOSUB SB.FILE.UTILS                                               ! DJAL !2.5CS !2.6BG
      SB.INTEGER% = PLLOL.SESS.NUM%                                     !2.5CS
      GOSUB SB.FILE.UTILS                                               !2.5CS
      SB.INTEGER% = PLLDB.SESS.NUM%                                     !2.5CS
      GOSUB SB.FILE.UTILS                                               !2.5CS
      SB.INTEGER% = CB.SESS.NUM%                                        ! 2.9NWB
      GOSUB SB.FILE.UTILS                                               ! 2.9NWB
      SB.INTEGER% = RB.SESS.NUM%                                        ! 2.10BG
      GOSUB SB.FILE.UTILS                                               ! 2.10BG
      SB.INTEGER% = REWKF.SESS.NUM%                                     ! 2.10BG
      GOSUB SB.FILE.UTILS                                               ! 2.10BG
      SB.INTEGER% = RECALLS.SESS.NUM%                                   ! 2.10BG
      GOSUB SB.FILE.UTILS                                               ! 2.10BG
      SB.INTEGER% = DELVINDX.SESS.NUM%
      
      SB.MESSAGE$ = "PDT Support - Ended"                               ! DJAL
      GOSUB SB.BG.MESSAGE

   RETURN

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***   L O W   L E V E L   S U B R O U T I N E S                            ***
\***                                                                        ***
\***                                                                        ***
\***   - SB.FILE.UTILS                                                      ***
\***   - SB.BG.MESSAGE                                                      ***
\***   - SB.LOG.AN.EVENT                                                    ***
\***   - SB.FORMAT.ERROR.DATA                                               ***
\***   - SB.LOG.PDT.ERROR                                                   ***
\***   - SB.FILE.OPEN.ERROR                                                 ***
\***   - SB.FILE.READ.ERROR                                                 ***
\***   - SB.FILE.WRITE.ERROR                                                ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   Subroutine : SB.FILE.UTILS
\***
\***   Purpose    : Allocate / report / de-allocate a file session number
\***
\***   Parameters : 2 or 3 (depending on action)
\***
\***      SB.ACTION$  = "O" for allocate file session number
\***                    "R" for report file session number
\***                    "C" for de-allocate file session number
\***      SB.INTEGER% = file reporting number for action "O" or
\***                  = file session number for actions "R" or "C"
\***
\***      SB.STRING$  = logical file name for action "O" or
\***                    null ("") for actions "R" and "C"
\***
\***   Output     : 1 or 2 (depending on action)
\***      SB.FILE.SESS.NUM% = file session number for action "O" or
\***                          undefined for action "C"
\***      or
\***      SB.FILE.REP.NUM%  = file reporting number for action "R" or
\***                          undefined for action "C"
\***
\***   Error action : log event 48 and end program
\***
\******************************************************************************

   SB.FILE.UTILS:

      RC% = SESS.NUM.UTILITY(SB.ACTION$,                                \
                             SB.INTEGER%,                               \
                             SB.STRING$ )

      IF RC% <> 0 THEN BEGIN
         SB.EVENT.NO% = 48
         SB.UNIQUE$ = FN.Z.PACK(STR$(F20.INTEGER.FILE.NO%), 10)
         SB.MESSAGE$ = "SESSION NUMBER ALLOCATION ROUTINE FAILED"
         GOSUB SB.LOG.AN.EVENT
         GOTO PROGRAM.EXIT
      ENDIF

      IF SB.ACTION$ = "O" THEN                                          \
         SB.FILE.SESS.NUM% = F20.INTEGER.FILE.NO%
      IF SB.ACTION$ = "R" THEN                                          \
         SB.FILE.REP.NUM% = F20.INTEGER.FILE.NO%

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.BG.MESSAGE
\***
\***   Purpose    : Display a message to the background screen
\***
\***   Parameters : 1
\***
\***      SB.MESSAGE$ = message to be displayed (message will be truncated to
\***                    46 characters if the message is longer than 46 chars)
\***                    Minus the port letter.
\***
\***   Output     : 1
\***      SB.MESSAGE$ = null
\***
\***   Error action : log an event 23 and end program
\***
\******************************************************************************

   SB.BG.MESSAGE:

      IF SB.MESSAGE$ = LAST.MESSAGE$ THEN RETURN
      LAST.MESSAGE$ = SB.MESSAGE$                                       ! FPAB

      SB.MESSAGE$ = MONITORED.PORT$ + ": " + SB.MESSAGE$                ! DJAL
      SB.MESSAGE$ = LEFT$(SB.MESSAGE$ + STRING$(46," "),46)             ! FPAB
      CALL ADXSERVE( ADX.RET.CODE%, 26, 0, SB.MESSAGE$)

      IF ADX.RET.CODE% <> 0 THEN BEGIN
         SB.EVENT.NO% = 23
         SB.UNIQUE$ = FN.Z.PACK(STR$(ADX.RET.CODE%),5) + "04   "
         SB.MESSAGE$ = ""
         GOSUB SB.LOG.AN.EVENT
      ENDIF

      SB.MESSAGE$ = ""

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.LOG.AN.EVENT
\***
\***   Purpose    : General routine to log an event using passed data. If
\***                program has been started manually for a re-run then also
\***                display a message on the background screen.
\***                The event will be preceded by one indicating the port
\***                being monitored by the program in error.
\***
\***   Parameters : 2
\***
\***      SB.EVENT.NO% = number of event to be logged
\***      SB.UNIQUE$   = 10 byte block of data unique to event
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.LOG.AN.EVENT:

      MESSAGE.NO% = 0
      UNIQUE.2$ = ""

      PORT.STRING$ = "PORT : " + MONITORED.PORT$ + "  "                 ! DJAL
      PORT.EVENT% = 75                                                  ! DJAL

      RC% = APPLICATION.LOG(MESSAGE.NO%,                                \ ILC
                            PORT.STRING$,                               \ DJAL
                            UNIQUE.2$,                                  \ DJAL
                            PORT.EVENT% )                               ! DJAL

      RC% = APPLICATION.LOG(MESSAGE.NO%,                                \ ILC
                            SB.UNIQUE$,                                 \
                            UNIQUE.2$,                                  \
                            SB.EVENT.NO% )

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FORMAT.ERROR.DATA
\***
\***   Purpose    : General routine to format the common error reporting
\***                data
\***
\***   Parameters : 0
\***
\***   Output     :
\***      SB.ERRS$   = ERRN converted to a 4 byte string
\***      SB.ERRL$   = ERRL zero packed up to 6 bytes
\***      SB.ERRF$   = ERRF converted to a reporting number (1 byte)
\***
\***   Error action : if hex conversion or string conversion fails then the
\***                  program ends
\***
\******************************************************************************

   SB.FORMAT.ERROR.DATA:

      RC% = CONV.TO.HEX( ERRN )
      IF RC% <> 0 THEN                                                  \
         GOTO PROGRAM.QUIT

      RC% = CONV.TO.STRING(0,                                           \
                           ERRN )
      IF RC% <> 0 THEN                                                  \
         GOTO PROGRAM.QUIT
      SB.ERRS$ = F17.RETURNED.STRING$

      SB.ERRL$ = FN.Z.PACK(STR$(ERRL), 6)
      SB.ACTION$ = "R" : SB.INTEGER% = ERRF% : SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      SB.ERRF$ = CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +                       \MDS
                 CHR$(SHIFT(SB.FILE.REP.NUM%,0))                         !MDS

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.LOG.PDT.ERROR
\***
\***   Purpose    : Standard PDT error event logging
\***
\***   Parameters : none
\***      routine picks up current / received state and data length etc.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.LOG.PDT.ERROR:

      IF EXP.STATES$ = "B" AND                                          \ JLC
         PREV.LOGGED.STATE$ <> LOG.STATE$ + EXP.STATES$ THEN BEGIN      ! JLC
         RETURN                                                         ! JLC
      ENDIF                                                             ! JLC
      PREV.LOGGED.STATE$ = LOG.STATE$ + EXP.STATES$                     ! JLC

      IF FN.Z.PACK(CURR.TERMINAL$,6) <> "??????" THEN BEGIN             ! JLC
         SB.EVENT.NO% = 62
         SB.UNIQUE$ = LOG.STATE$ +                                      \
                      LEFT$(EXP.STATES$+"   ",3) +                      \
                      FN.Z.PACK(CURR.TERMINAL$,6)
         GOSUB SB.LOG.AN.EVENT
      ENDIF                                                             ! JLC

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FILE.OPEN.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to open a file.
\***
\***   Parameters : 1
\***
\***      CURR.SESS.NUM% = file session number of the file that caused the
\***                       error, this is used to look-up the file reporting
\***                       number that is logged in the event's unique data.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.OPEN.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      SB.EVENT.NO% = 106                                              ! LSMG
      SB.UNIQUE$ = "O" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ LSMG
                   CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +                  \ LSMG
                   PACK$(STRING$(16,"0"))                             ! LSMG
      GOSUB SB.LOG.AN.EVENT

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FILE.READ.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to read a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***      CURRENT.KEY$    = value of key used to try to read from file.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.READ.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      IF CURRENT.KEY$ <> "SECTOR" AND                                   \ MDS
         CURRENT.KEY$ <> PACK$(STRING$(7,"??")) THEN BEGIN              ! MDS
         SB.EVENT.NO% = 6
         SB.UNIQUE$ = "R" +                                             \
         CHR$(SB.FILE.REP.NUM%) +                                       \
         FN.Z.PACK(CURRENT.KEY$,8)
      ENDIF ELSE BEGIN
          SB.EVENT.NO% = 106                                            ! LSMG
          SB.UNIQUE$ = "R" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +          \ LSMG
                       CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +                \ LSMG
                       CURRENT.KEY$                                     ! LSMG
      ENDIF                                                             ! LSMG
      GOSUB SB.LOG.AN.EVENT

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FILE.WRITE.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to write a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***      CURRENT.KEY$    = value of key used to try to write to file.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.WRITE.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
      IF CURRENT.KEY$ <> "SECTOR" THEN BEGIN                            ! DSW
         SB.EVENT.NO% = 6
         SB.UNIQUE$ = "W" + CHR$(SB.FILE.REP.NUM%) +                    \
         FN.Z.PACK(CURRENT.KEY$,8)
      ENDIF ELSE BEGIN                                                  ! DSW
          SB.EVENT.NO% = 106                                            ! LSMG
          SB.UNIQUE$ = "W" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +          \ LSMG
                       CHR$(SHIFT(SB.FILE.REP.NUM%,0)) +                \ LSMG
                       FN.Z.PACK(CURRENT.KEY$,8)                        ! LSMG
      ENDIF                                                             ! DSW
      GOSUB SB.LOG.AN.EVENT

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FILE.CREATE.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to create a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.CREATE.ERROR:

      SB.ACTION$ = "R"
      SB.INTEGER% = CURR.SESS.NUM%
      SB.STRING$ = ""
      GOSUB SB.FILE.UTILS
        SB.EVENT.NO% = 106                                              ! LSMG
        SB.UNIQUE$ = "C" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ LSMG
                      CHR$(SHIFT(SB.FILE.REP.NUM%,0))                   ! LSMG
      GOSUB SB.LOG.AN.EVENT

   RETURN

\******************************************************************************
\***
\***   Subroutine : SB.FILE.DELETE.ERROR
\***
\***   Purpose    : Log an event 6 with unique data indicating an error has
\***                occurred whilst attempting to delete a file.
\***
\***   Parameters : 2
\***
\***      CURR.SESS.NUM%  = file session number of the file that caused the
\***                        error, this is used to look-up the file reporting
\***                        number that is logged in the event's unique data.
\***
\***   Output     : none
\***
\***   Error action : none
\***
\******************************************************************************

   SB.FILE.DELETE.ERROR:                                                ! DJAL

      SB.ACTION$ = "R"                                                  ! DJAL
      SB.INTEGER% = CURR.SESS.NUM%                                      ! DJAL
      SB.STRING$ = ""                                                   ! DJAL
      GOSUB SB.FILE.UTILS                                               ! DJAL
        SB.EVENT.NO% = 106                                              ! LSMG
        SB.UNIQUE$ = "D" + CHR$(SHIFT(SB.FILE.REP.NUM%,8)) +            \ LSMG
                     CHR$(SHIFT(SB.FILE.REP.NUM%,0))                    ! LSMG
      GOSUB SB.LOG.AN.EVENT                                             ! DJAL

   RETURN                                                               ! DJAL

\******************************************************************************
\******************************************************************************
\***                                                                        ***
\***                                                                        ***
\***   E R R O R   H A N D L I N G                                          ***
\***                                                                        ***
\***                                                                        ***
\******************************************************************************
\******************************************************************************

\******************************************************************************
\***
\***   OPEN.ERROR:
\***
\***      log an event 6 (open error)
\***      set RECEIVE.STATE$ to "*"
\***
\***   GOTO PROGRAM.EXIT (no return is possible)
\***
\******************************************************************************

   OPEN.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.OPEN.ERROR
      RECEIVE.STATE$ = "*"

   GOTO PROGRAM.EXIT

\******************************************************************************
\***
\***   READ.ERROR:
\***
\***      log an event 6 (read error)
\***      set RECEIVE.STATE$ to "*"
\***
\***   GOTO PROGRAM.EXIT (no return is possible)
\***
\******************************************************************************

   READ.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.READ.ERROR
      RECEIVE.STATE$ = "*"


   GOTO PROGRAM.EXIT

\******************************************************************************
\***
\***   WRITE.ERROR:
\***
\***      log an event 6 (write error)
\***      set RECEIVE.STATE$ to "*"
\***
\***   GOTO PROGRAM.EXIT (no return is possible)
\***
\******************************************************************************

   WRITE.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.WRITE.ERROR
      RECEIVE.STATE$ = "*"

   GOTO PROGRAM.EXIT

\******************************************************************************
\**
\***   CREATE.ERROR:
\***
\***      log an event 6 (create error)
\***      set RECEIVE.STATE$ to "*"
\***
\***   GOTO PROGRAM.EXIT (no return is possible)
\***
\******************************************************************************

   CREATE.ERROR:

      TEMP.STATE$ = RECEIVE.STATE$
      GOSUB SB.FILE.CREATE.ERROR
      RECEIVE.STATE$ = "*"

   GOTO PROGRAM.EXIT

\******************************************************************************
\***
\***   DELETE.ERROR:
\***
\***      log an event 6 (delete error)
\***      set RECEIVE.STATE$ to "*"
\***
\***   GOTO PROGRAM.EXIT (no return is possible)
\***
\******************************************************************************

   DELETE.ERROR:                                                        ! DJAL

      TEMP.STATE$ = RECEIVE.STATE$                                      ! DJAL
      GOSUB SB.FILE.DELETE.ERROR                                        ! DJAL
      RECEIVE.STATE$ = "*"                                              ! DJAL

   GOTO PROGRAM.EXIT                                                    ! DJAL



\******************************************************************************
\***
\***   DRSMQ.NOT.FOUND:
\***
\***      create DRSMQ
\***
\***   GOTO DRSMQ.CONTINUE:
\***
\******************************************************************************

   DRSMQ.NOT.FOUND:                                                     ! HDS

      IF END #DRSMQ.SESS.NUM% THEN CREATE.ERROR                         ! HDS

      CREATE POSFILE DRSMQ.FILE.NAME$ AS DRSMQ.SESS.NUM%                \ HDS
             BUFFSIZE 10240 LOCKED
      DRSMQ.OPEN.FLAG$ = "Y"

      GOTO DRSMQ.CONTINUE


\******************************************************************************
\***
\***   ERROR.DETECTED:
\***
\***      if an error has occurred after a 'fatal' error then quit program
\***
\***      increment ERROR.COUNT%
\***      NOTE : all returns from error detected should decrement the
\***             variable ERROR.COUNT% and exit using the RESUME command
\***      if ERROR.COUNT% > 1 then end program (error within error detected)
\***
\***      if the CSR workfile does not exist then resume
\***
\***      set-up common error reporting information
\***
\***      if the error is an access conflict on CSR, EPSOM or PCHK files then
\***          resume at FILE.CONFLICT
\***
\***      if unable to open PSS38 communication pipes then
\***         wait 3 seconds then retry
\***      endif
\***
\***      if an access conflict occurs on a session then retry
\***
\***      if error is invalid characters in VAL() then
\***         store RECEIVE.STATE$
\***         set RECEIVE.STATE$ to "*"
\***         use stored receive state variable to decide where to resume
\***      endif
\***
\***      if error is out of memory then log an event, using ADXERROR and
\***      quit program
\***
\***      log an event 1
\***
\***   resume PROGRAM.EXIT
\***
\******************************************************************************

ERROR.DETECTED:

   IF QUIT.FLAG$ = "Y" THEN RESUME PROGRAM.QUIT

   IF ERR = "CU" THEN RESUME                                            ! DSW

   ERROR.COUNT% = ERROR.COUNT% + 1
   IF ERROR.COUNT% > 1 THEN RESUME PROGRAM.QUIT

   GOSUB SB.FORMAT.ERROR.DATA

   IF ERR = "OE" AND ERRF% = 0 THEN BEGIN                               ! DSW
      ERROR.COUNT% = ERROR.COUNT% - 1                                   ! DSW
\     CSRWF.EXISTS% = 0                                                 ! DSW !2.9NWB
      RESUME                                                            ! DSW
   ENDIF                                                                ! DSW

   IF (ERRN AND 0000FFFFh) = 0000400Ch THEN BEGIN                       ! DSW
      IF SB.FILE.REP.NUM% = EPSOM.REPORT.NUM%                           \ MDS
\     OR SB.FILE.REP.NUM% = CSR.REPORT.NUM%                             \ MDS !2.9NWB
      OR SB.FILE.REP.NUM% = PCHK.REPORT.NUM%                            \ MDS
      OR SB.FILE.REP.NUM% = DIREC.REPORT.NUM%                           \ MDS
      OR SB.FILE.REP.NUM% = UOD.REPORT.NUM%                             \ MDS
      OR SB.FILE.REP.NUM% = CCDMY.REPORT.NUM%                           \ MDS
      OR SB.FILE.REP.NUM% = INVOK.REPORT.NUM%                           \ 1.5
      OR SB.FILE.REP.NUM% = STKTK.REPORT.NUM% THEN BEGIN                ! 1.5
         ERROR.COUNT% = ERROR.COUNT% - 1                                ! DSW
         IF LOCATION$ <> "STARTUP" THEN BEGIN                           ! DSW
            RESUME LOCKED.FILE                                          ! DSW
         ENDIF ELSE BEGIN                                               ! DSW
            RESUME IGNORE.LOCKED.FILE                                   ! DSW
         ENDIF                                                          ! DSW
      ENDIF                                                             ! DSW
   ENDIF                                                                ! DSW

   IF ERR = "OE"                                                        \
   AND (CURR.SESS.NUM% = PIPEI.SESS.NUM%                                \
   OR CURR.SESS.NUM% = PIPEO.SESS.NUM%                                  \
   OR CURR.SESS.NUM% = PLDT.SESS.NUM%) THEN BEGIN                       ! HDS
      ERROR.COUNT% = ERROR.COUNT% - 1
      WAIT ; 3 * 1000
      RESUME RETRY
   ENDIF

   IF (ERRN AND 0000FFFFh) = 400Ch THEN BEGIN
      ERROR.COUNT% = ERROR.COUNT% - 1
      RESUME RETRY
   ENDIF

   IF ERR = "IH" AND ERRN = 00000094h THEN BEGIN
      TEMP.STATE$ = RECEIVE.STATE$
      RES.POS% = MATCH(TEMP.STATE$, "CEFG", 1)
      IF RES.POS% = 0 THEN BEGIN
         RESUME PROGRAM.EXIT
      ENDIF ELSE BEGIN
         RESUME PROGRAM.QUIT
      ENDIF
   ENDIF

   SB.EVENT.NO% = 101
   SB.UNIQUE$ = SB.ERRS$ + SB.ERRF$ +                                   \MDS
                PACK$(RIGHT$(STRING$(8,"0")+SB.ERRL$,8))                !MDS
   GOSUB SB.LOG.AN.EVENT

   ERROR.COUNT% = ERROR.COUNT% - 1
RESUME PROGRAM.EXIT

END
