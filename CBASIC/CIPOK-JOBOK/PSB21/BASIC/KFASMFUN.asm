;******************************************************************************
;******************************************************************************
;***
;***              KEYED FILE ASSEMBLER FUNCTIONS (KFASMFUN)
;***
;******************************************************************************  
;******************************************************************************  

;******************************************************************************  
;***
;***   Version A.              Mark Walker                         5th Feb 1998
;***   Initial version. This includes INITKF, READKF and TERMKF routines.  
;***
;***   Version B.              Mark Walker                        19th Feb 1998
;***   Fixed subtle bug which can cause 'FirstTime' flag to be reset. 
;***
;***   Version C.              Mark Walker                        28th Oct 1998
;***   Added a check for return string being null on entry to READKF. 
;***
;***   Version D.              Mark Walker                         9th Sep 2013
;***   Changed FlexOS includes from ASM to A86 to prevent compilation.
;***
;***   Version E.              Mark Walker                        28th Jan 2014
;***   - Added new WRITEKF routine.
;***   - Modified READKF and TERMKF routines to perform disk writes if the
;***     read buffer has been updated.
;***
;***   Version F.              Mark Walker                        21st May 2014
;***   - Extended the READKF interface to include a mode parameter. If this is
;***     set to 1, the pattern string will be treated as a bit mask.
;***
;******************************************************************************  

       .MODEL large
       .386

       INCLUDE flexdef.a86                                                      ;DMW
       INCLUDE flextab.a86                                                      ;DMW
       INCLUDE flexif.a86                                                       ;DMW

;      Constants for size of memory block used for reading
       BLOCK_SIZE_LOW  equ 0000h
       BLOCK_SIZE_HIGH equ 0001h 

MPB_STR     STRUC
       start       DW 2    DUP (?)
       min         DW 2    DUP (?)
       max         DW 2    DUP (?)
MPB_STR     ENDS

DSeg    SEGMENT PARA PUBLIC 'DATA' USE16
       ReadOffset              DW 2    DUP (0)
       ParameterBlock          DW 14   DUP (0)
       FileID                  DW 2    DUP (0)
       ReadBuffer              DW 2    DUP (0)
       ReadBufsiz              DW 2    DUP (0)                                  ;EMW
       PatternString           DB 64   DUP (0)
       FileName                DB 64   DUP (0)
       KeyedFileString         DB 'FSFACADX'
       RecordLength            DW 0
       KeyLength               DW 0
       PatternLength           DW 0
       RecordsPerSector        DW 0
       SectorsPerBlock         DW 0
       PatternOffset           DW 0
       Mode                    DW 0                                             ;FMW
       SectorCount             DW 0
       RecordCount             DW 0
       FirstTime               DW 0
       AllRecords              DW 0
       WriteRequired           DW 0                                             ;EMW
       RecordPointer           DW 0
       SectorPointer           DW 0
       rc                      DW 2    DUP (0)
       mpb                     MPB_STR  ?

DSeg    ENDS

CSeg    SEGMENT PARA PUBLIC 'CODE' USE16
              ASSUME cs:CSeg, ds:DSeg

;****************************************************************************** 
;****************************************************************************** 
;***
;***   FUNCTION        :       INITKF
;***
;****************************************************************************** 
;***
;***   Opens a keyed file, allocates memory for reading and initialises
;***   variables required by the READKF function.
;***
;******************************************************************************  
;***
;***   BASIC function call usage:
;***
;***   FID% = INITKF(FILE.NAME$,       File name                       ASC
;***                 OPTIONS%)         Open options (e.g. read only)   INT 2
;***
;***   Return code:                                                    INT 4
;***   FID% > 0                        File identification number      
;***   FID% <= 0                       Error code                      
;***
;******************************************************************************  
;******************************************************************************  

INITKF PROC

       PUBLIC INITKF
                          
       push bp
       push ds
       mov bp, sp

       mov ax, DSeg
       mov ds, ax

;      Clear file name storage 
       mov ax, SEG FileName
       mov es, ax
       xor ax, ax
       mov di, OFFSET FileName
       mov cx, 64
       rep stosb

;      Parameter 1 - File name
       push ds
       lds si, 8[bp]
       mov di, OFFSET FileName
       mov ax, SEG FileName
       mov es, ax
       mov cx, ds:[si]
       add si, 2
       cld
       rep movsb
       pop ds

;      Parameter 2 - Options
       mov ax, 12[bp]
       mov dx, ax

;       Parameter block for SVC OPEN is as follows:                             ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |      0        |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                               0                              |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                            Filename                          |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW

;      Setup parameter block for file open
       xor ax, ax
       mov [ParameterBlock], ax
       mov ax, dx
       mov 2[ParameterBlock], ax
       xor ax, ax
       mov 4[ParameterBlock], ax
       mov 6[ParameterBlock], ax
       mov ax, OFFSET FileName
       mov 8[ParameterBlock], ax
       mov ax, SEG FileName
       mov 10[ParameterBlock], ax
        
;      Open file
       mov cx, F_OPEN
       mov ax, OFFSET ParameterBlock
       mov bx, SEG ParameterBlock
       int OSINT

;      Store the file ID
       mov [FileID], ax
       mov 2[FileID], bx

;      Check whether open error occurred
       cmp bx, 0
       jg AllocMem
       jmp ExitINITKF

AllocMem:
;       Parameter block for SVC MALLOC is as follows:                           ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |      0        |   Options     |             0                |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                               0                              |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                               0                              |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 12 |                            MPBPTR                            |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 16 |                      12 (size of MPBPTR)                     |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW

;      Setup parameter block for memory allocation
       xor ax, ax
       mov [mpb.start], ax
       mov 2[mpb.start], ax
       mov ax, BLOCK_SIZE_LOW
       mov [mpb.min], ax
       mov [mpb.max], ax
       mov ax, BLOCK_SIZE_HIGH
       mov 2[mpb.min], ax
       mov 2[mpb.max], ax
       
       xor al, al
       mov [ParameterBlock], ax
       mov al, O_NEWHEAP
       mov 1[ParameterBlock], ax
       xor ax, ax
       mov 2[ParameterBlock], ax
       mov 4[ParameterBlock], ax
       mov 6[ParameterBlock], ax
       mov 8[ParameterBlock], ax
       mov 10[ParameterBlock], ax
       mov bx, OFFSET mpb
       mov 12[ParameterBlock], bx
       mov bx, SEG mpb
       mov 14[ParameterBlock], bx
       mov ax, 12
       mov 16[ParameterBlock], ax
       xor ax, ax
       mov 18[ParameterBlock], ax
       mov 20[ParameterBlock], ax
       mov 22[ParameterBlock], ax
       mov 24[ParameterBlock], ax
       mov 26[ParameterBlock], ax

;      Allocate memory block 
       mov cx, F_MALLOC
       mov ax, OFFSET ParameterBlock
       mov bx, ds
       int OSINT

;      Check whether memory allocation failed
       cmp ax, 0
       jne ExitINITKF
       cmp bx, 0
       jne ExitINITKF

       mov ax, [mpb.start]
       mov [ReadBuffer], ax
       mov ax, 2[mpb.start]
       mov 2[ReadBuffer], ax

;       Parameter block for SVC READ is as follows:                             ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |  Sync/Async   |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                              SWI                             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                              FNUM                            |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 12 |                             BUFFER                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 16 |                             BUFSIZ                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 20 |                             OFFSET                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 24 |                           Delimiters                         |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW

;      Setup parameter block to read first sector
       xor ax, ax
       mov [ParameterBlock], ax
       mov 2[ParameterBlock], ax
       mov 4[ParameterBlock], ax
       mov 6[ParameterBlock], ax
       mov ax, [FileID]
       mov 8[ParameterBlock], ax
       mov ax, 2[FileID]
       mov 10[ParameterBlock], ax
       mov ax, [ReadBuffer]
       mov 12[ParameterBlock], ax
       mov ax, 2[ReadBuffer]
       mov 14[ParameterBlock], ax
       mov ax, 512                     ; 512 bytes required 
       mov 16[ParameterBlock], ax      ;
       xor ax, ax                      ;
       mov 18[ParameterBlock], ax      ;
       mov 20[ParameterBlock], ax
       mov 22[ParameterBlock], ax
       mov 24[ParameterBlock], ax
       mov 26[ParameterBlock], ax

;      Read first sector of file
       mov cx, F_READ
       mov ax, OFFSET ParameterBlock
       mov bx, ds
       int OSINT

;      Check whether 512 bytes read 
       cmp ax, 512
       jnz NotKeyed

;      Check whether this is a keyed file 
;      (i.e. contains 'FSFACADX' starting at byte 160 in first sector) 
       mov di, [ReadBuffer]
       mov es, 2[ReadBuffer]
       add di, 160
       mov si, OFFSET KeyedFileString
       mov cx, 8
       cld
       repe cmpsb
       je Keyed

NotKeyed:

;       Parameter block for SVC CLOSE is as follows:                            ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |      0        |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                               0                              |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                             FNUM                             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW

;      Setup parameter block to close file
       xor ax, ax
       mov [ParameterBlock], ax
       mov 2[ParameterBlock], ax
       mov 4[ParameterBlock], ax
       mov 6[ParameterBlock], ax
       mov ax, [FileID] 
       mov 8[ParameterBlock], ax
       mov ax, 2[FileID]
       mov 10[ParameterBlock], ax

;      Close file
       mov cx, F_CLOSE
       mov ax, OFFSET ParameterBlock
       mov bx, SEG ParameterBlock
       int OSINT

;      Set return code = -1 (not a keyed file) 
       mov ax, 0FFFFh
       mov bx, 0FFFFh
       jmp ExitINITKF

Keyed: 
;      Get record length (byte 46 from first sector)
       mov di, [ReadBuffer]
       add di, 46
       mov ax, 2[ReadBuffer]
       mov es, ax
       mov bx, es:[di]
       mov [RecordLength], bx
       xor ax, ax
       mov 2[RecordLength], ax

;      Get maximum number of records per sector
       mov dx, 0
       mov ax, 508
       div bx
       mov [RecordsPerSector], ax

;      Get key length (byte 54 from first sector)
       mov di, [ReadBuffer]
       add di, 54
       mov ax, 2[ReadBuffer]
       mov es, ax
       mov ax, es:[di]
       mov [KeyLength], ax
       mov ax, 0
       mov 2[KeyLength], ax

;      Initialise block pointer to ignore first sector
       mov [ReadOffset], 512
       mov 2[ReadOffset], 0

;      Initialise flags
       mov BYTE PTR [FirstTime], 0
       mov BYTE PTR [AllRecords], 0
       mov BYTE PTR [WriteRequired], 0                                          ;EMW

;      Return the file ID 
       mov ax, [FileID]
       mov bx, 2[FileID]

ExitINITKF:
       pop ds
       pop bp

       retf

INITKF ENDP

;****************************************************************************** 
;****************************************************************************** 
;***
;***   FUNCTION        :       TERMKF
;***
;****************************************************************************** 
;***
;***   Commit any outstanding disk writes due to updates to the read buffer,
;***   close file and deallocate memory.
;***
;******************************************************************************   
;***
;***   BASIC function call usage:
;***
;***   RC% = TERMKF(FID%)              File identification no          INT 4
;***
;***   Return code:                                                    INT 4
;***   RC% = 0                         Successful
;***   RC% < 0                         Error code
;***
;****************************************************************************** 
;******************************************************************************  

TERMKF PROC

       PUBLIC TERMKF

       push bp
       push ds
       mov bp, sp

       mov ax, DSeg
       mov ds, ax

;      ------------------------------------------------------------------------ ;EMW
;      As we are now allowing disk writes, we must check whether there are any  ;EMW
;      updates to the current read buffer that we have not yet committed to     ;EMW
;      disk. This is necessary to cater for the scenario where the file         ;EMW
;      processing has been deliberately terminated before the end of the file   ;EMW
;      has been reached.                                                        ;EMW
;      ------------------------------------------------------------------------ ;EMW
                                                                                ;EMW
;      Check the 'write required' flag                                          ;EMW
       mov ax, [WriteRequired]                                                  ;EMW
       cmp ax, 0                                                                ;EMW
       jz CloseFile                                                             ;EMW
                                                                                ;EMW
;       Parameter block for SVC WRITE is as follows:                            ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |  Sync/Async   |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                              SWI                             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                              FNUM                            |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 12 |                             BUFFER                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 16 |                             BUFSIZ                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 20 |                             OFFSET                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 24 |                           Delimiters                         |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
                                                                                ;EMW
;      Setup parameter block to write file block                                ;EMW
       xor ax, ax                                                               ;EMW
       mov [ParameterBlock], ax                                                 ;EMW
       mov 2[ParameterBlock], ax                                                ;EMW
       mov 4[ParameterBlock], ax                                                ;EMW
       mov 6[ParameterBlock], ax                                                ;EMW
       mov ax, [FileID]                                                         ;EMW
       mov 8[ParameterBlock], ax                                                ;EMW
       mov ax, 2[FileID]                                                        ;EMW
       mov 10[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadBuffer]                                                     ;EMW
       mov 12[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadBuffer]                                                    ;EMW
       mov 14[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadBufsiz]                                                     ;EMW
       mov 16[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadBufsiz]                                                    ;EMW
       mov 18[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadOffset]                                                     ;EMW
       mov 20[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadOffset]                                                    ;EMW
       mov 22[ParameterBlock], ax                                               ;EMW
       xor ax, ax                                                               ;EMW
       mov 24[ParameterBlock], ax                                               ;EMW
       mov 26[ParameterBlock], ax                                               ;EMW
                                                                                ;EMW
;      Write file block                                                         ;EMW
       mov cx, F_WRITE                                                          ;EMW
       mov ax, OFFSET ParameterBlock                                            ;EMW
       mov bx, ds                                                               ;EMW
       int OSINT                                                                ;EMW
                                                                                ;EMW
;      Check for file write errors                                              ;EMW
       cmp bx, 0                                                                ;EMW
       jl ExitTERMKF                                                            ;EMW
                                                                                ;EMW
;      Reset the 'write required' flag                                          ;EMW
       mov BYTE PTR [WriteRequired], 0                                          ;EMW
                                                                                ;EMW
CloseFile:                                                                      ;EMW
;      Parameter 1 - File ID
       mov bx, 8[bp]
       mov cx, 10[bp]

;       Parameter block for SVC CLOSE is as follows:                            ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |      0        |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                               0                              |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                             FNUM                             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW

;      Setup parameter block to close file
       xor ax, ax
       mov [ParameterBlock], ax
       mov 2[ParameterBlock], ax
       mov 4[ParameterBlock], ax
       mov 6[ParameterBlock], ax
       mov ax, bx
       mov 8[ParameterBlock], ax
       mov ax, cx
       mov 10[ParameterBlock], ax

;      Close file
       mov cx, F_CLOSE
       mov ax, OFFSET ParameterBlock
       mov bx, SEG ParameterBlock
       int OSINT

;      Deallocate memory block
       mov cx, F_MFREE
       mov ax, [ReadBuffer]
       mov bx, 2[ReadBuffer]
       int OSINT

ExitTERMKF:                                                                     ;EMW
       pop ds
       pop bp

       retf

TERMKF ENDP

;****************************************************************************** 
;****************************************************************************** 
;***
;***   FUNCTION        :       READKF
;***
;****************************************************************************** 
;***
;***   Reads the first occurence of a record in a keyed file that contains
;***   a particular pattern string at a specified offset within the record.
;***
;***   Subsequent calls to the function will result in the next record being 
;***   returned that matches the specified pattern criteria.
;***
;***   Note that if the offset specified is ZERO, the pattern string parameter
;***   is ignored and ALL records within the file will be processed.
;***
;***   If the mode parameter is set to 1, the pattern string will be treated    ;FMW
;***   as a bit mask. The pattern string will be ANDed with the corresponding   ;FMW
;***   bytes in the record at the specified offset, and the record will be      ;FMW
;***   returned if the value of the masked record bytes exactly matches the     ;FMW
;***   pattern mask.                                                            ;FMW
;***                                                                            ;FMW
;***   The matching record strings are returned in the RECORD$ parameter.
;***
;***   The length of RECORD$ determines how many bytes of the file are
;***   returned. For example, you may only require the keys being returned or
;***   the whole record. Varying the length of RECORD$ on entry to the function
;***   will enable you to do either of these.
;***
;******************************************************************************
;***
;***   BASIC function call :
;***
;***   RC% = READKF(RECORD$,           n-byte return string            ASC
;***                PATTERN$,          n-byte pattern/mask string      ASC      ;FMW
;***                OFFSET%,           pattern offset                  INT 4    ;FMW
;***                MODE%)             mode                            INT 2    ;FMW
;***                                   possible values are:                     ;FMW
;***                                      0 = pattern is a value                ;FMW
;***                                      1 = pattern is a bit mask             ;FMW
;***
;***   Return code:                                                    INT 4
;***   RC% = 0                         read successful
;***   RC% = -1                        end of file
;***   RC% = -2                        null return string specified
;***   RC% < -2                        error code
;***
;***   Record (or part record) returned in RECORD$                     ASC
;***
;******************************************************************************  
;******************************************************************************  

READKF PROC

       PUBLIC READKF

       push bp
       push ds
       mov bp, sp

       mov ax, DSeg
       mov ds, ax

;      Test whether this is the first execution
       cmp BYTE PTR [FirstTime], 0
       jz GetReadParams                                                         ;EMW
       mov cx, [SectorCount]
       push cx
       mov cx, [RecordCount]
       push cx

;      Continue from next record position 
       jmp ContinueREADKF

GetReadParams:                                                                  ;EMW
;      Set flag to prevent further executions 
       mov BYTE PTR [FirstTime], 1

;      Initialise the read buffer size                                          ;EMW
       mov ax, BLOCK_SIZE_LOW                                                   ;EMW
       mov [ReadBufsiz], ax                                                     ;EMW
       mov ax, BLOCK_SIZE_HIGH                                                  ;EMW
       mov 2[ReadBufsiz], ax                                                    ;EMW

;      Parameter 1 - Return string
       mov ax, 8[bp]
       mov bx, 10[bp]

;      Ensure address of return string is not NULL 
       cmp ax, 0
       jnz GetPatternOffset
       cmp bx, 0
       jz  ReadReturnStringNull                                                 ;EMW

GetPatternOffset:
;      Parameter 3 - Pattern offset within record
       mov ax, 16[bp]

;      Check whether all records required i.e. pattern offset = 0
       cmp ax, 0
       jg StorePatternOffset

;      Set flag to get ALL records
       mov BYTE PTR [AllRecords], 1
       xor ax, ax
       mov [PatternOffset], ax
       jmp ReadBlock                                                            ;EMW

StorePatternOffset:
;      Store pattern offset
       dec ax
       mov [PatternOffset], ax

;      Parameter 2 - Pattern string to search for
       push ds
       lds si, 12[bp]
       mov di, OFFSET PatternString
       mov ax, SEG PatternString
       mov es, ax
       mov cx, ds:[si]
       push cx
       add si, 2
       cld
       rep movsb
       pop cx
       pop ds
       mov [PatternLength], cx

;      Parameter 4 - Mode                                                       ;FMW
       mov ax, 20[bp]                                                           ;FMW
       mov [Mode], ax                                                           ;FMW

;      Read the first file block                                                ;EMW
       jmp ReadBlock                                                            ;EMW

NextBlock:
;      Check the 'write required' flag                                          ;EMW
       mov ax, [WriteRequired]                                                  ;EMW
       cmp ax, 0                                                                ;EMW
       jz IncrementBlock                                                        ;EMW
                                                                                ;EMW
WriteBlock:                                                                     ;EMW
;       Parameter block for SVC WRITE is as follows:                            ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |  Sync/Async   |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                              SWI                             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                              FNUM                            |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 12 |                             BUFFER                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 16 |                             BUFSIZ                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 20 |                             OFFSET                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 24 |                           Delimiters                         |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
                                                                                ;EMW
;      Setup parameter block to write file block                                ;EMW
       xor ax, ax                                                               ;EMW
       mov [ParameterBlock], ax                                                 ;EMW
       mov 2[ParameterBlock], ax                                                ;EMW
       mov 4[ParameterBlock], ax                                                ;EMW
       mov 6[ParameterBlock], ax                                                ;EMW
       mov ax, [FileID]                                                         ;EMW
       mov 8[ParameterBlock], ax                                                ;EMW
       mov ax, 2[FileID]                                                        ;EMW
       mov 10[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadBuffer]                                                     ;EMW
       mov 12[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadBuffer]                                                    ;EMW
       mov 14[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadBufsiz]                                                     ;EMW
       mov 16[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadBufsiz]                                                    ;EMW
       mov 18[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadOffset]                                                     ;EMW
       mov 20[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadOffset]                                                    ;EMW
       mov 22[ParameterBlock], ax                                               ;EMW
       xor ax, ax                                                               ;EMW
       mov 24[ParameterBlock], ax                                               ;EMW
       mov 26[ParameterBlock], ax                                               ;EMW
                                                                                ;EMW
;      Write file block                                                         ;EMW
       mov cx, F_WRITE                                                          ;EMW
       mov ax, OFFSET ParameterBlock                                            ;EMW
       mov bx, ds                                                               ;EMW
       int OSINT                                                                ;EMW
                                                                                ;EMW
;      Check for file write errors                                              ;EMW
       cmp bx, 0                                                                ;EMW
       jl ExitREADKF                                                            ;EMW
                                                                                ;EMW
;      Reset the 'write required' flag                                          ;EMW
       mov BYTE PTR [WriteRequired], 0                                          ;EMW
                                                                                ;EMW
IncrementBlock:                                                                 ;EMW
;      Increment read offset to get next file block                             ;EMW
       mov ax, 2[ReadOffset]                                                    ;EMW
       inc ax                                                                   ;EMW
       mov 2[ReadOffset], ax                                                    ;EMW
                                                                                ;EMW
ReadBlock:                                                                      ;EMW
;       Parameter block for SVC READ is as follows:                             ;EMW
;      +-------------------------------------------------------------------+    ;EMW
;      |  0 |  Sync/Async   |   Options     |            Flags             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  4 |                              SWI                             |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      |  8 |                              FNUM                            |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 12 |                             BUFFER                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 16 |                             BUFSIZ                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 20 |                             OFFSET                           |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW
;      | 24 |                           Delimiters                         |    ;EMW
;      +----+--------------------------------------------------------------+    ;EMW

;      Setup parameter block to read file block
       xor ax, ax
       mov [ParameterBlock], ax
       mov 2[ParameterBlock], ax
       mov 4[ParameterBlock], ax
       mov 6[ParameterBlock], ax
       mov ax, [FileID]
       mov 8[ParameterBlock], ax
       mov ax, 2[FileID]
       mov 10[ParameterBlock], ax
       mov ax, [ReadBuffer]
       mov 12[ParameterBlock], ax
       mov ax, 2[ReadBuffer]
       mov 14[ParameterBlock], ax
       mov ax, [ReadBufsiz]                                                     ;EMW
       mov 16[ParameterBlock], ax                                               ;EMW
       mov ax, 2[ReadBufsiz]                                                    ;EMW
       mov 18[ParameterBlock], ax                                               ;EMW
       mov ax, [ReadOffset]
       mov 20[ParameterBlock], ax
       mov ax, 2[ReadOffset]
       mov 22[ParameterBlock], ax
       xor ax, ax
       mov 24[ParameterBlock], ax
       mov 26[ParameterBlock], ax
        
;      Read file block
       mov cx, F_READ
       mov ax, OFFSET ParameterBlock
       mov bx, ds
       int OSINT

;      ------------------------------------------------------------------------ ;EMW
;      At this point, the BX:AX registers will either hold the error code       ;EMW
;      or the number of bytes returned by the file read.                        ;EMW
;      Note that as we are now writing modified file blocks, we must take       ;EMW
;      into account the fact that the last file block will almost certainly     ;EMW
;      be smaller than the rest!                                                ;EMW
;      ------------------------------------------------------------------------ ;EMW

;      Check whether end of file has been reached
       cmp ax, 4003h
       je Eof

;      Check for other file read errors                                         ;EMW
       cmp bx, 0
       jge ReadSuccessful
       jmp ExitREADKF

Eof:
;      Set return code = -1 (end of file reached)
       mov ax, 0FFFFh
       mov bx, 0FFFFh
       jmp ExitREADKF

ReadReturnStringNull:                                                           ;EMW
;      Set return code = -2 (null return string)
       mov ax, 0FFFEh
       mov bx, 0FFFFh
       jmp ExitREADKF

ReadSuccessful:
;      Set the read buffer size (as it could be smaller than the maximum)       ;EMW
       mov [ReadBufsiz], ax                                                     ;EMW
       mov 2[ReadBufsiz], bx                                                    ;EMW

;      Calculate number of sectors in block
       push dx
       mov dx, bx
       mov bx, 512
       div bx
       pop dx
       mov cx, ax

;      Set sector count to number of sectors in block 
       mov [SectorsPerBlock], cx

NextSector:
;      Save sector count on stack
       push cx

;      Calculate the position of the next sector within the block 
       mov bx, [SectorsPerBlock]
       sub bx, cx
       mov ax, 512
       mul bx
       mov [SectorPointer], ax

;      Set record count to number of records per sector
       mov cx, [RecordsPerSector]

NextRecord:
;      Save record count on stack
       push cx

;      Calculate the position of the next record within the sector 
       mov bx, [RecordsPerSector]
       sub bx, cx
       mov ax, [RecordLength]
       mul bx
       mov bx, [SectorPointer]
       add ax, bx
       add ax, 4

;      Store record pointer (dx) 
       mov dx, ax 

       jmp CheckNullKey                                                         ;FMW
                                                                                ;FMW
;CheckNullKey:                                                                  ;FMW
;      Check whether current record has a null key                              ;FMW
;      mov di, [ReadBuffer]                                                     ;FMW
;      mov es, 2[ReadBuffer]                                                    ;FMW
;      add di, dx                                                               ;FMW
;      mov cx, [KeyLength]                                                      ;FMW
;      mov al, 0                                                                ;FMW
;      cld                                                                      ;FMW
;      repe scasb                                                               ;FMW
;      je FoundNullKey                                                          ;FMW
;                                                                               ;FMW
;CheckPattern:                                                                  ;FMW
;      Check whether ALL records are required                                   ;FMW
;      cmp BYTE PTR [AllRecords], 1                                             ;FMW
;      jz FoundMatch                                                            ;FMW
                                                                                ;FMW
;      Compare pattern string with record string                                ;FMW
;      mov di, [ReadBuffer]                                                     ;FMW
;      mov es, 2[ReadBuffer]                                                    ;FMW
;      add di, dx                                                               ;FMW
;      add di, [PatternOffset]                                                  ;FMW
;      mov si, OFFSET PatternString                                             ;FMW
;      mov cx, [PatternLength]                                                  ;FMW
;      cld                                                                      ;FMW
;      repe cmpsb                                                               ;FMW
;      je FoundMatch                                                            ;FMW

;+++++++++++++++++++++++++++++++++++++++++++++++++++
;>>>>> Entry point for repeated function calls <<<<<
;+++++++++++++++++++++++++++++++++++++++++++++++++++
ContinueREADKF:
;      Recover record count from stack
       pop cx

;      Get next record
       Loop NextRecord

;      Recover sector count from stack
       pop cx

;      Get next sector
       Loop NextSector

;      Get next block
       jmp NextBlock

CheckNullKey:                                                                   ;FMW
;      Check whether current record has a null key                              ;FMW
       mov di, [ReadBuffer]                                                     ;FMW
       mov es, 2[ReadBuffer]                                                    ;FMW
       add di, dx                                                               ;FMW
       mov cx, [KeyLength]                                                      ;FMW
       mov al, 0                                                                ;FMW
       cld                                                                      ;FMW
       repe scasb                                                               ;FMW
       je FoundNullKey                                                          ;FMW
                                                                                ;FMW
CheckPattern:                                                                   ;FMW
;      Check whether ALL records are required                                   ;FMW
       cmp BYTE PTR [AllRecords], 1                                             ;FMW
       jz FoundMatch                                                            ;FMW
                                                                                ;FMW
;      Point to pattern string and record string                                ;FMW
       mov di, [ReadBuffer]                                                     ;FMW
       mov es, 2[ReadBuffer]                                                    ;FMW
       add di, dx                                                               ;FMW
       add di, [PatternOffset]                                                  ;FMW
       mov si, OFFSET PatternString                                             ;FMW
                                                                                ;FMW
;      Get pattern length                                                       ;FMW
       mov cx, [PatternLength]                                                  ;FMW
                                                                                ;FMW
;      Check whether 'pattern value' mode specified                             ;FMW
       cmp BYTE PTR [Mode], 0                                                   ;FMW
       jz ComparePatternValue                                                   ;FMW
                                                                                ;FMW
ComparePatternBitmap:                                                           ;FMW
;      Get next pattern byte and record byte                                    ;FMW
       mov al, es:[di]                                                          ;FMW
       mov bl, ds:[si]                                                          ;FMW
                                                                                ;FMW
;      Check for a bitmap pattern match                                         ;FMW
       and al, bl                                                               ;FMW
       cmp al, bl                                                               ;FMW
       jnz ContinueREADKF                                                       ;FMW
                                                                                ;FMW
;      Move to next pattern and record string byte                              ;FMW
       inc di                                                                   ;FMW
       inc si                                                                   ;FMW
                                                                                ;FMW
;      Check next pattern bitmap                                                ;FMW
       Loop ComparePatternBitmap                                                ;FMW
                                                                                ;FMW
       jmp FoundMatch                                                           ;FMW
                                                                                ;FMW
ComparePatternValue:                                                            ;FMW
;      Compare pattern value with record string                                 ;FMW
       cld                                                                      ;FMW
       repe cmpsb                                                               ;FMW
       je FoundMatch                                                            ;FMW
                                                                                ;FMW
       jmp ContinueREADKF                                                       ;FMW

FoundNullKey:
;      Reset record count to force end of sector 
       pop cx
       mov cx, 1
       push cx
       
       jmp ContinueREADKF

FoundMatch:
;      Recover record count from stack and save in memory
       pop cx
       mov [RecordCount], cx

;      Recover sector count from stack and save in memory                       ;EMW
       pop cx
       mov [SectorCount], cx

;      Save record pointer in memory                                            ;EMW
       mov [RecordPointer], dx                                                  ;EMW

;      Put matched record in the returned record string
       mov di, 8[bp]
       mov ax, 10[bp]
       mov es, ax 
       mov si, [ReadBuffer]
       mov ax, 2[ReadBuffer]
       add si, dx
       mov ds, ax
       mov cx, es:[di]
       add di, 2
       cld
       rep movsb

;      Set return code = 0 (successful)
       xor ax, ax
       mov bx, 0

ExitREADKF:
       pop ds
       pop bp

       retf

READKF ENDP

;****************************************************************************** ;EMW
;****************************************************************************** ;EMW
;***                                                                            ;EMW
;***   FUNCTION        :       WRITEKF                                          ;EMW
;***                                                                            ;EMW
;****************************************************************************** ;EMW
;***                                                                            ;EMW
;***   Updates the most recent record string returned by READKF.                ;EMW
;***                                                                            ;EMW
;****************************************************************************** ;EMW
;***                                                                            ;EMW
;***   BASIC function call :                                                    ;EMW
;***                                                                            ;EMW
;***   Updates the read buffer for the most recent string returned by function  ;EMW
;***   READKF, and marks the file block as requiring a disk write. Note that    ;EMW
;***   the disk write will only be performed when the file block has been       ;EMW
;***   fully processed, for efficiency reasons.                                 ;EMW
;***                                                                            ;EMW
;***   It is IMPORTANT that the length of the RECORD$ parameter is              ;EMW
;***   unchanged. In addition, amending the key field is not allowed.           ;EMW
;***                                                                            ;EMW
;***   RC% = WRITEKF(RECORD$)           n-byte return string           ASC      ;EMW
;***                                                                            ;EMW
;***   Return code:                                                    INT 4    ;EMW
;***   RC% = 0                         write successful                         ;EMW
;***   RC% = -2                        null update string specified             ;EMW
;***   RC% < -2                        error code                               ;EMW
;***                                                                            ;EMW
;***   Record (or part record) to update in RECORD$                    ASC      ;EMW
;***                                                                            ;EMW
;****************************************************************************** ;EMW
;****************************************************************************** ;EMW
                                                                                ;EMW
WRITEKF PROC                                                                    ;EMW
                                                                                ;EMW
       PUBLIC WRITEKF                                                           ;EMW
                                                                                ;EMW
       push bp                                                                  ;EMW
       push ds                                                                  ;EMW
       mov bp, sp                                                               ;EMW
                                                                                ;EMW
       mov ax, DSeg                                                             ;EMW
       mov ds, ax                                                               ;EMW
                                                                                ;EMW
GetWriteParams:                                                                 ;EMW
;      Parameter 1 - Return string                                              ;EMW
       mov ax, 8[bp]                                                            ;EMW
       mov bx, 10[bp]                                                           ;EMW
                                                                                ;EMW
;      Ensure address of return string is not NULL                              ;EMW
       cmp ax, 0                                                                ;EMW
       jnz UpdateBuffer                                                         ;EMW
       cmp bx, 0                                                                ;EMW
       jz  WriteReturnStringNull                                                ;EMW
                                                                                ;EMW
       jmp UpdateBuffer                                                         ;EMW
                                                                                ;EMW
WriteReturnStringNull:                                                          ;EMW
;      Set return code = -2 (null return string)                                ;EMW
       mov ax, 0FFFEh                                                           ;EMW
       mov bx, 0FFFFh                                                           ;EMW
       jmp ExitWRITEKF                                                          ;EMW
                                                                                ;EMW
UpdateBuffer:                                                                   ;EMW
;      Set the 'write required' flag                                            ;EMW
       mov BYTE PTR [WriteRequired], 1                                          ;EMW
                                                                                ;EMW
;      Restore record pointer from memory                                       ;EMW
       mov dx, [RecordPointer]                                                  ;EMW
                                                                                ;EMW
;      Put amended record into the read buffer                                  ;EMW
       mov di, [ReadBuffer]                                                     ;EMW
       mov ax, 2[ReadBuffer]                                                    ;EMW
       mov es, ax                                                               ;EMW
       add di, dx                                                               ;EMW
       mov si, 8[bp]                                                            ;EMW
       mov ax, 10[bp]                                                           ;EMW
       mov ds, ax                                                               ;EMW
       mov cx, ds:[si]                                                          ;EMW
       add si, 2                                                                ;EMW
       cld                                                                      ;EMW
       rep movsb                                                                ;EMW
                                                                                ;EMW
;      Set return code = 0 (successful)                                         ;EMW
       xor ax, ax                                                               ;EMW
       mov bx, 0                                                                ;EMW
                                                                                ;EMW
ExitWRITEKF:                                                                    ;EMW
       pop ds                                                                   ;EMW
       pop bp                                                                   ;EMW
                                                                                ;EMW
       retf                                                                     ;EMW
                                                                                ;EMW
WRITEKF ENDP                                                                    ;EMW

CSeg   ENDS

END

