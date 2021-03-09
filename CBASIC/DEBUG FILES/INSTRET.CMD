/*----------------------------------------------------------------------
//
//  Module Name: INSTRET.CMD
//
//  Description: Install the Application Debugger on Retail Workbench/2.
//
//  Product Classification:
//    Licensed Materials - Property of IBM
//    This product contains "Restricted Materials of IBM"
//    5799-DWR (C) Copyright IBM Corp. 1992
//    All rights reserved.
//    US Government Users Restricted Rights - Use, duplication or
//    disclosure restricted by GSA ADP Schedule Contract with IBM
//    Corp.
//
//  Status: New
//
//  Initial Author: George L. Havens
//
//  Function: This procedure will install the OS/2 component of the Application
//            Debugger.
//
//  Dependencies: None.
//
//  Restrictions: None.
//
//  Compiler: REXX
//
// Change Activity -----------------------------------------------------
//
// $MOD(module) COMP(component) PROD(product): Description...
//
// FLAG  REASON  VERS  DATE  WHO   DESCRIPTION
// ---- -------- ---- ------ ---   -----------
//               V100 921023 GLH : Initial level
// END-OF-SPECIFICATIONS -----------------------------------------------*/

'@echo off'

arg option

if option = '' then do
'START PMREXX A:\INSTRET PMREXX'
exit
end
else if option = 'TRACE' then do
'START PMREXX /T A:\INSTRET PMREXX'
exit
end

/* initialize to no exit message */
exit_msg = 0

/* load REXX system functions */
call RxFuncAdd 'SysLoadFuncs', 'RexxUtil', 'SysLoadFuncs'
call SysLoadFuncs

/* display installation logo */
action = RxMessageBox('IBM 4680/4690 Application Debugger', 'Installation', 'OKCANCEL')
if action = 2 then exit

/* get bin directory */
retailbin = SysIni('USER', 'RETAILWB', 'BIN')
retailbin = strip(retailbin,,'00'x)

if retailbin = 'ERROR:' then do
   exit_msg = 1

   say 'Retail Workbench/2 has not been installed. Please specify drive and directory to copy executable files.'
   parse pull retailbin
end

/* get doc directory */
retaildoc = SysIni('USER', 'RETAILWB', 'DOC')
retaildoc = strip(retaildoc,,'00'x)

if retailbin = 'ERROR:' then do
   exit_msg = 1
   say 'Retail Workbench/2 has not been installed. Please specify drive and directory to copy documentation files.'
   parse pull retaildoc
end

/* get free disk space */
parse value SysDriveInfo(left(retailbin,1)) with info_drive free rest

/* if not enough disk space */
if free < 1500000 then do
   action = RxMessageBox('Not enough disk space. Free disk space and retry.', 'Installation', 'OK','ERROR')
   signal error
end                                                                  /* @P2A */

/* display insert diskette 2 */
action = RxMessageBox('Insert IBM 4680/4690 Application Debugger Diskette #2', 'Installation', 'OKCANCEL')
if action = 2 then exit

/* copy EXE files */
'COPY A:\*.EXE' retailbin '>nul'

result = SysFileTree(retailbin'\DEBUGDMP.EXE', file, 'F')
if result <> 0 | file.0 <> 1 then do
   action = RxMessageBox('Error creating DEBUGDMP.EXE file.', 'Installation', 'OK','ERROR')
   signal error
end

result = SysFileTree(retailbin'\DEBUGCDV.EXE', file, 'F')
if result <> 0 | file.0 <> 1 then do
   action = RxMessageBox('Error creating DEBUGCDV.EXE file.', 'Installation', 'OK','ERROR')
   signal error
end

result = SysFileTree(retailbin'\DEBUGFRM.EXE', file, 'F')
if result <> 0 | file.0 <> 1 then do
   action = RxMessageBox('Error creating DEBUGFRM.EXE file.', 'Installation', 'OK','ERROR')
   signal error
end

result = SysFileTree(retailbin'\DBINFO.EXE', file, 'F')
if result <> 0 | file.0 <> 1 then do
   action = RxMessageBox('Error creating DBINFO.EXE file.', 'Installation', 'OK','ERROR')
   signal error
end

result = SysFileTree(retailbin'\DBGINFO.EXE', file, 'F')
if result <> 0 | file.0 <> 1 then do
   action = RxMessageBox('Error creating DBGINFO.EXE file.', 'Installation', 'OK','ERROR')
   signal error
end

/* copy INF file */
'COPY A:\*.INF' retaildoc '>nul'

result = SysFileTree(retaildoc'\DEBUGUSE.INF', file, 'F')
if result <> 0 | file.0 <> 1 then do
   action = RxMessageBox('Error creating DEBUGUSE.INF file.', 'Installation', 'OK','ERROR')
   signal error
end

/* create view for User's Guide */
result = SysCreateObject('WPProgram', '4680/4690 Application Debugger User''s Guide', '<RETAILWB>', 'EXENAME=VIEW.EXE;PARAMETERS='retaildoc'\DEBUGUSE.INF;OBJECTID=<DEBUGGUIDE>')
if result = 0 then do
   result = SysCreateObject('WPProgram', '4680/4690 Application Debugger User''s Guide', '<WP_DESKTOP>', 'EXENAME=VIEW.EXE;PARAMETERS='retaildoc'\DEBUGUSE.INF;OBJECTID=<DEBUGGUIDE>')
   if result = 0 then do
       action = RxMessageBox('Error creating User''s Guide object.', 'Installation', 'OK','ERROR')
       signal error
   end
end

action = RxMessageBox('    Installation complete.', 'Installation', 'OK', 'INFORMATION');
if exit_msg == 1 then do
   say ''
   say 'This window can be closed after reading messages.'
end

exit

/* error processing **/
ERROR:

action = RxMessageBox('    Installation failed.', 'Installation', 'OK','ERROR')
if exit_msg == 1 then do
   say ''
   say 'This window can be closed after reading messages.'
end

exit
