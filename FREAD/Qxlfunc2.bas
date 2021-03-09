\/* TIME STAMP BLOCK ************************************************
\*PROM  PVEJ QXLFUNC2 B86      02/09/98 07:51:39 PIDBLD   BVEJ IR35895
\*RTIME PVEJ QXLFUNC2 B86      02/09/98 07:51:39 PIDBLD   BVEJ IR35895
\*PROM  BVEJ QXLFUNC2 B86      08/21/97 18:19:12 PIDBLD   MVEJ IR35895
\*RTIME BVEJ QXLFUNC2 B86      08/21/97 18:19:12 PIDBLD   MVEJ IR35895
\*LIB   MVEJ QXLFUNC2 B86      07/11/97 16:36:26 EDJRAL   *    IR35895
\*PROM  PVEJ QXLFUNC2 B86      04/02/97 08:06:43 PIDBLD   BVEJ IR34352
\*RTIME PVEJ QXLFUNC2 B86      04/02/97 08:06:43 PIDBLD   BVEJ IR34352
\*PROM  BVEJ QXLFUNC2 B86      04/01/97 16:55:47 PIDBLD   MVEJ IR34352
\*RTIME BVEJ QXLFUNC2 B86      04/01/97 16:55:47 PIDBLD   MVEJ IR34352
\*LIB   MVEJ QXLFUNC2 B86      03/27/97 12:45:44 EDJRAL   *    IR34352
\*PROM  BVEJ QXLFUNC2 B86      03/10/97 17:19:24 PIDBLD3  MVEJ IR34352
\*RTIME BVEJ QXLFUNC2 B86      03/10/97 17:19:24 PIDBLD3  MVEJ IR34352
\*LIB   MVEJ QXLFUNC2 B86      03/04/97 13:55:41 EDJRAL   *    IR34352
\*LIB   MVEJ QXLFUNC2 B86      02/14/97 15:37:49 EDJRAL   *    REASONAR
\** END OF TIME STAMP BLOCK ****************************************/
!***********************************************************************
\/*      COPYRIGHT: THIS MODULE IS "RESTRICTED MATERIALS OF EDJ       */
\/*                 ENTERPRISES, INC" (C) COPYRIGHT EDJ ENTERPRISES,  */
\/*                 1994,1997 ALL RIGHTS RESERVED LICENSED            */
\/*                 MATERIALS - PROPERTY OF EDJ ENTERPRISES, INC      */
\/*                                                                   */
\/*                 5799-QXL THIS MODULE IS "RESTRICTED               */
\/*                 MATERIALS OF IBM" (C) COPYRIGHT IBM CORP          */
\/*                 1996,1997 ALL RIGHTS RESERVED LICENSED            */
\/*                 MATERIALS                                         */
\/*                                                                   */
!***********************************************************************
!routines that use basrout, and EDJ C routines and nothing else
!***********************************************************************
!DEV IR35895:
!970616 To improve performance and handle larger data base,
!       the C routines that look at the directory for the journal
!       data base have been re-written
!***********************************************************************

%INCLUDE BASROUT.J86

!***************************
!AIR35895
!* 970616 Start
!***************************
%INCLUDE QXLCROUT.J86
!function edjdir (file.name$,dtbl.buffer$,name.length) external
!        integer*4       edjdir
!        string          file.name$                      ! file name to start
!        string          dtbl.buffer$                    ! must be initialized
!        integer*2       name.length                     ! returned name length
!end function
!function edjdirz (file.name$,dtbl.buffer$) external     ! routine to return
!        integer*4       edjdirz
!        string          file.name$                      ! file name to start
!        string          dtbl.buffer$                    ! must be initialized
!end function
!* 970616 end
!EIR35895
!*****************************************************************
!
!     FileCreateDate
!
!        Return date file was created in 8 byte string: YYYYMMDD
!        Return Null String if File does not exist
!
!*****************************************************************
FUNCTION FileCreateDate(FileName) PUBLIC
         String FileCreateDate, FileName
         String dtbl.buffer$
         Integer*4 retcode
         String new.year$, new.month$, new.day$


FileCreateDate = ""

dtbl.buffer$ = STRING$(48,chr$(0))
retcode = SRCHDIR(FileName,dtbl.buffer$)
IF retcode = 80204010H THEN EXIT FUNCTION ! File not found

new.year$ = STR$(cextract2(dtbl.buffer$,41))
new.year$ = RIGHT$("0000"+new.year$,4)
new.month$ = STR$(ASC(MID$(dtbl.buffer$,43,1)))
new.month$ = RIGHT$("00"+new.month$,2)
new.day$ = STR$(ASC(MID$(dtbl.buffer$,44,1)))
new.day$ = RIGHT$("00"+new.day$,2)

FileCreateDate = new.year$ + new.month$ + new.day$

FEND

!*****************************************************************
!
!     ?Exist
!            Returns -1 if file exists
!                     0 if file does not exist
!
!*****************************************************************
FUNCTION ?Exist(FileName) PUBLIC
         Integer*1 ?Exist
         String    FileName
         Integer*4 FileSize
         Integer*2 ErrorF%
         String    ErrorCode,dtbl.buffer$
         Integer*4 ErrorN
         integer*4 RC


         ?Exist = 0
         dtbl.buffer$ = string$(48,chr$(0))
         RC = srchdir (FileName,dtbl.buffer$)
         IF RC > 0 THEN ?Exist = -1
         EXIT FUNCTION

FEND


FUNCTION EscTime         PUBLIC
         STRING EscTime

         EscTime = RIGHT$(STR$(timedate(1)),6)

FEND

!AIR35895
!************************************************************************
!
!970616 This routine has been changed to use new EDJDIR routines
!       It will only handle the amount of data that can fit in one
!       32,000 byte buffer
!
!************************************************************************
SUB file.list.nz$(file.name$, file.list$, name.count, name.length) PUBLIC
        STRING file.name$, file.list$
        INTEGER*4 name.count, nkey, temp.key
        INTEGER*2 name.length


        file.list$ = STRING$(32000,chr$(0))
        !********************************************************
        !**  Only read one buffer full of data
        !********************************************************
        nkey = 0
        temp.key = varptr(nkey)
        name.count = EDJDIR(file.name$,file.list$,name.length,temp.key,1)

        IF name.count < 0 THEN \
          name.count = LEN(file.list$)/name.length
END SUB

!************************************************************************
!
!970616 This routine has been changed to use new EDJDIR routines
!       It will only handle the amount of data that can fit in one
!       32,000 byte buffer
!
!************************************************************************
SUB file.list.z$(file.name$, file.list$, name.count) PUBLIC
        STRING file.name$, file.list$
        INTEGER*4 name.count, nkey, temp.key
        INTEGER*2 name.length


        file.list$ = STRING$(32000,chr$(0))
        !********************************************************
        !**  Only read one buffer full of data
        !********************************************************
        nkey = 0
        temp.key = varptr(nkey)
        name.count = EDJDIR(file.name$,file.list$,name.length,temp.key,0)

        IF name.count < 0 THEN \
          name.count = LEN(file.list$)/name.length
END SUB
!EIR35895
FUNCTION dated.list$(file.name$) PUBLIC
        STRING   dated.list$, file.name$

        STRING dtbl.buffer$,new.name$,temp.list$, new.year$, new.month$, new.day$
        INTEGER*4 retcode
        INTEGER*2 i

        dtbl.buffer$ = STRING$(48,chr$(0))
        temp.list$ = ""
        retcode = -1
        WHILE retcode <> 0
                retcode = SRCHDIR(file.name$,dtbl.buffer$)
                IF retcode = 80204010H THEN retcode = 0    ! Directory not found
                IF retcode <> 0 THEN BEGIN
                        new.name$ = MID$(dtbl.buffer$,5,18)
                        IF LEFT$(new.name$,1) = CHR$(0) THEN retcode = 0
                ENDIF
                IF retcode <> 0 THEN BEGIN
                        IF LEFT$(new.name$,1) <> "." THEN BEGIN
                                new.year$ = STR$(cextract2(dtbl.buffer$,41))
                                new.year$ = RIGHT$("0000"+new.year$,4)
                                new.month$ = STR$(ASC(MID$(dtbl.buffer$,43,1)))
                                new.month$ = RIGHT$("00"+new.month$,2)
                                new.day$ = STR$(ASC(MID$(dtbl.buffer$,44,1)))
                                new.day$ = RIGHT$("00"+new.day$,2)
                                i = MATCH(CHR$(0),new.name$,1)
                                IF i = 0 THEN i = 19
                                new.name$ = LEFT$(new.name$,i-1)
                                temp.list$ = temp.list$ + new.name$ + CHR$(0) \
                                             + new.year$ + new.month$ + new.day$
                        ENDIF
                ENDIF
        WEND
        dated.list$ = temp.list$
FEND
