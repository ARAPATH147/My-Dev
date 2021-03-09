\************************************************************************
\***
\***    PHKEEP            Ranjith Gopalankutty          05/04/2017  
\***                                                                
\***    There was a defect with program SRP10. It supposed to house 
\***    keep the expired planner records from SRPOG,SRMOD,SRSXF     
\***    and SRPDF files. SRPOG is the master planner file SRMOD     
\***    is module,SRSXF is notch data and SRPDF is the planner      
\***    descriptor file.  SRMOD,SRPDF and SRSXF files depends on    
\***    the master planner file SRPOG to build the keys for them.   
\***                                                                
\***    The defect was that, SRP10 program was housekeeping only    
\***    records from SRPOG and leaving behind other 3 files.Over    
\***    time. Records will be housekept from SRPOG and untouching   
\***    other files. Records will get piled up and causing file     
\***    full issues. This resulted in new planner update not        
\***    able to add in to planner files.Defect has been found       
\***    and fixed by AppsMgmt. change is live in all stores,but     
\***    one issue found afterwards that program was not able to     
\***    delete the historical expired  records from planner files.  
\***                                                                
\***    This program will housekeep the data from 3 planner files   
\***    SRMOD,SRSXF and SRPDF and will recreate the planner files   
\***    fresh using only live records.                              
\***                                                                
\***    Program can be executed via command prompt using NFM or     
\***    in a package.
\***  
\***    A                RANJITH GOPALANKUTTY               12-07-2017
\***    After the meeting with Inctactix it came to our notice that   
\***    there is a big mis match between the number of planners in    
\***    in inctactix and controller. The reason for that is in the    
\***    difference between the housekeeping logic, inctactis never    
\***    considers a planner (POGID) as deactive and set the maximum   
\***    end date (2199 year)instead it creates new fragments(POGDB)   
\***    planner(POGID)and creates end date for it, which is mostly    
\***    2 weeks. but in SRPOG file we have end date only for POGID    
\***    and not POGDB, so when housekeeping logic runs, its never     
\***    able to delete the expired records, so amended the code that  
\***    instead of SRPOG, application takes SRMAP as the master file  
\***    and started towards deletion to all4 files SRPOG,SRMOD,SRSXF  
\***    and SRPDF.        
\***    
\***    B                RANJITH GOPALANKUTTY               23-11-2017
\***    During AppsMgmt testing found that certain stores have more 
\***    10000 records in SRMOD files and array would not handle it 
\***    so increasing the array count from current 10000 to maximum
\***    Also relinked the code with 17D file functions.
\***                                            
\***********************************************************************
\***
\***    Function globals
\***
\***********************************************************************


%INCLUDE BEMFDEC.J86  
%INCLUDE BOOTSDEC.J86 
%INCLUDE POGDEDEC.J86 
%INCLUDE POGOKDEC.J86 
%INCLUDE PSBF01G.J86  
%INCLUDE PSBF20G.J86 
%INCLUDE SRMODDEC.J86 
%INCLUDE SRPDFDEC.J86 
%INCLUDE SRPOGDEC.J86 
%INCLUDE SRSXFDEC.J86
%INCLUDE SRMAPDEC.J86                                                   !ARG


\***********************************************************************
\*
\*    Global variable definitions
\*
\***********************************************************************

STRING     GLOBAL        \
     FILE.OPERATION$,    \
     PINFO.TABLE$,       \
     PASSED.STRING$,     \
     SVC.PARAM$          

INTEGER*4  GLOBAL        \
   ADX.FUNCTION%,        \
   ADX.PARAM%,           \
   CURRENT.REPORT.NUM%,  \
   EVENT.NO%,            \
   PASSED.INTEGER%,      \
   RC%
   
\***********************************************************************
\*
\*    Local Variables
\*
\***********************************************************************

STRING                    \
     ADX.COMMAND$,        \
     COMM.TAIL$,          \
     CURRENT.CODE$,       \
     FLAG$,               \
     FUNCTION.FLAG$,      \
     OK.STRING$,          \
     PROG.NAME$,          \
     PHKEEP.OK.NAME$,     \
     PHKEEP.LOG.NAME$,    \
     RCD$,                \
     SEC$,                \
     SPACE$,              \
     SRMAP.ARRAY$(1),     \
     SRMOD.ARR$(1),       \
     SRMAP.FLAG$,         \
     SRMAP.STRING$,       \
     SRMOD.DIR.NAME$,     \
     SRMOD.NEW.FILE$,     \
     SRPDF.ARRAY$(1),     \
     SRPDF.DIR.NAME$,     \
     SRPDF.KEY.NAME$,     \
     SRMOD.KEY.NAME$,     \
     SRPOG.ARRAY$(1),     \
     SRPOG.DIR.NAME$,     \
     SRPOG.FLAG$,         \
     SRPOG.KEY.NAME$,     \
     SRPOG.TEMP.NAME$,    \
     SRSXF.FLAG$,         \
     SRPOG.STRING$,       \
     SRMOD.STRING$,       \
     SRMOD.FLAG$,         \
     SRSXF.STRING$,       \
     SRSXF.DIR.NAME$,     \
     SRSXF.KEY.NAME$,     \
     SRPDF.STRING$,       \
     SRPDF.FLAG$,         \
     TEMP.FORM$,          \
     VAR.STRING.1$,       \
     VAR.STRING.2$
                           
INTEGER*4                 \
     ADX.INTEGER%,        \
     ARR.COUNT%,          \
     I%,                  \
     EOF%,                \
     ERROR.CNT%,          \
     K%,                  \
     FOUND%,              \
     FOUND1%,             \
     J%,                  \
     MESSAGE.NO%,         \
     NOFILE.SRMOD.COUNT%, \
     NOFILE.SRPDF.COUNT%, \
     NOFILE.SRSXF.COUNT%, \
     P%,                  \
     PHKEEP.OK.SESS%,     \
     PHKEEP.SESS.NUM%,    \
     POGDB%,              \
     RPS%,                \
     RC1%,                \
     SRMAP%,              \
     SRMOD%,              \
     SRMOD.DIR.SESS%,     \
     SRPDF%,              \
     SRPOG%,              \
     SRSXF%,              \
     SRPOG.COUNT%,        \
     SRPDF.DIR.SESS%,     \
     SRPOG.DIR.SESS%,     \
     SRMOD.COUNT%,        \
     SRPDF.COUNT%,        \
     SRSXF.COUNT%,        \
     SRSXF.DIR.SESS%,     \
     TEMP.ARR%(1),         \
     TEMP.COUNT%  



\***********************************************************************
\*
\*    Included code defining external Boots functions
\*
\***********************************************************************
    
%INCLUDE BEMFEXT.J86
%INCLUDE BASROUT.J86
%INCLUDE BOOTSEXT.J86
%INCLUDE BTCMEM.J86
%INCLUDE POGDEEXT.J86
%INCLUDE POGOKEXT.J86
%INCLUDE PSBF01E.J86 
%INCLUDE PSBF20E.J86 
%INCLUDE PSBF24E.J86
%INCLUDE SRPEXT.J86
%INCLUDE SRPDFEXT.J86
%INCLUDE SRPOGEXT.J86
%INCLUDE SRMODEXT.J86
%INCLUDE SRSXFEXT.J86
%INCLUDE SRMAPEXT.J86                                                   !ARG

\***********************************************************************
\*
\*    INITIALISATION : This Sub-routine does all the initial processing
\*                     before starting the main process
\*
\***********************************************************************

FUNCTION FLEXOSSVC(COMMAND%, PARAM$) EXTERNAL                           
                                                                        
      STRING PARAM$                                                     
      INTEGER*2 COMMAND%, FLEXOSSVC                                     
                                                                        
END FUNCTION                                                            

\**********************************************************************
\***
\***    ADXSERVE
\***    IBM routine to request Application Services.
\***
\**********************************************************************

SUB ADXSERVE (RET, FUNC, PARM1, PARM2$) EXTERNAL

    INTEGER*4  RET
    INTEGER*2  FUNC,PARM1
    STRING     PARM2$

END SUB

\**********************************************************************
\***
\***    ADXCOPYF
\***    IBM routine to request Application Services.
\***
\**********************************************************************

SUB ADXCOPYF (RETC,INFILE,OUTFILE,OPT0,OPT1) EXTERNAL

    INTEGER*4 RETC
    STRING    INFILE,OUTFILE
    INTEGER*2 OPT0,OPT1

END SUB 

\***********************************************************************
\*                                                                     *
\*          S T A R T   O F   M A I N L I N E   C O D E                *
\*                                                                     *
\***********************************************************************
\***********************************************************************

ON ERROR GOTO ERROR.DETECTED

    PRINT  TIME.STAMP$(2)  + "-Program started"         

    GOSUB INITIALISATION

    PRINT #PHKEEP.SESS.NUM% ; TIME.STAMP$(2)  + "-Program started" 
    GOSUB MAIN.PROCESSING
    GOSUB TERMINATION
 
STOP.PROGRAM:
    STOP

\***********************************************************************
\*
\*    INITIALISATION : This Sub-routine does all the initial processing
\*                     before starting the main process
\*
\***********************************************************************
INITIALISATION:

    GOSUB INITIAL.CHECKS          
    GOSUB INITIALISE.VARIABLES    
    GOSUB ALLOCATE.SESSION.NUMBERS
    GOSUB OPEN.FILES              
    GOSUB CREATE.FILES            

RETURN

\***********************************************************************
\*
\*    INITIALISE.VARIABLES :   
\*                
\*
\***********************************************************************
INITIALISE.VARIABLES:

   CALL SRMAP.SET                                                       !ARG
   CALL SRPOG.SET
   CALL SRMOD.SET
   CALL SRMAP.SET
   CALL SRPDF.SET
   CALL SRSXF.SET
   
   ERROR.CNT% = 0
   
   !ARR.COUNT% = 10000                                                  !BRG
    ARR.COUNT% = 16000                                                  !BRG
   
   DIM SRMAP.ARRAY$(ARR.COUNT%)                                         !ARG
   
   DIM SRPOG.ARRAY$(ARR.COUNT%) 
    
   DIM SRMOD.ARR$(ARR.COUNT%)      

   DIM SRPDF.ARRAY$(ARR.COUNT%)  
   
   DIM TEMP.ARR%(ARR.COUNT%)
   
     
   FOUND% = 0
   
   PHKEEP.LOG.NAME$ = "ADXLXACN::W:/PHKEEP.LOG"
   PHKEEP.SESS.NUM% = 456
   
   PHKEEP.OK.NAME$  = "ADXLXACN::W:/PHKEEP.ERR"
   PHKEEP.OK.SESS%  = 457   
   
   SRPOG.DIR.NAME$  = "ADXLXACN::W:\SRPOG.DIR"
   SRPOG.KEY.NAME$  = "ADXLXACN::W:\SRPOG.KEY"
   SRMOD.DIR.NAME$  = "ADXLXACN::W:\SRMOD.DIR"
   SRMOD.KEY.NAME$  = "ADXLXACN::W:\SRMOD.KEY"
   SRPDF.DIR.NAME$  = "ADXLXACN::W:\SRPDF.DIR"
   SRPDF.KEY.NAME$  = "ADXLXACN::W:\SRPDF.KEY"
   SRSXF.DIR.NAME$  = "ADXLXACN::W:\SRSXF.DIR"
   SRSXF.KEY.NAME$  = "ADXLXACN::W:\SRSXF.KEY"
   
   SPACE$ = " "   
   
   CREATE PHKEEP.LOG.NAME$ AS PHKEEP.SESS.NUM%   
   
   CREATE PHKEEP.OK.NAME$ AS PHKEEP.OK.SESS%
    
   SRMAP.FLAG$ = ""
   SRPOG.FLAG$ = ""
   SRMOD.FLAG$ = ""
   SRSXF.FLAG$ = ""
   SRPDF.FLAG$ = ""
   
RETURN 

\***********************************************************************
\*
\*    ALLOCATE.SESSION.NUMBERS:   
\*                
\*
\***********************************************************************
ALLOCATE.SESSION.NUMBERS:

    FUNCTION.FLAG$ = "O"  

    PASSED.INTEGER% = SRMAP.REPORT.NUM%                                 !ARG
    PASSED.STRING$  = SRMAP.FILE.NAME$                                  !ARG
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     !ARG
    SRMAP.SESS.NUM% = F20.INTEGER.FILE.NO% 	                            !ARG
    
    PASSED.INTEGER% = SRPOG.REPORT.NUM%                                 
    PASSED.STRING$  = SRPOG.FILE.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRPOG.SESS.NUM% = F20.INTEGER.FILE.NO%                              

    PASSED.INTEGER% = SRMOD.REPORT.NUM%                                 
    PASSED.STRING$  = SRMOD.FILE.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRMOD.SESS.NUM% = F20.INTEGER.FILE.NO%   

    PASSED.INTEGER% = SRSXF.REPORT.NUM%                                 
    PASSED.STRING$  = SRSXF.FILE.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRSXF.SESS.NUM% = F20.INTEGER.FILE.NO%                              

    PASSED.INTEGER% = SRPDF.REPORT.NUM%                                 
    PASSED.STRING$  = SRPDF.FILE.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRPDF.SESS.NUM% = F20.INTEGER.FILE.NO%                              

    PASSED.INTEGER% = PHKEEP.SESS.NUM%                                  
    PASSED.STRING$  = PHKEEP.LOG.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    PHKEEP.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = PHKEEP.OK.SESS%                                   
    PASSED.STRING$  = PHKEEP.OK.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    PHKEEP.OK.SESS% = F20.INTEGER.FILE.NO% 

    PASSED.INTEGER% = 457                                               
    PASSED.STRING$  = SRPOG.DIR.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRPOG.DIR.SESS% = F20.INTEGER.FILE.NO% 	

    PASSED.INTEGER% = 458                                               
    PASSED.STRING$  = SRMOD.DIR.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRMOD.DIR.SESS% = F20.INTEGER.FILE.NO%                              

    PASSED.INTEGER% = 459                                               
    PASSED.STRING$  = SRPDF.DIR.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRPDF.DIR.SESS% = F20.INTEGER.FILE.NO%                              

    PASSED.INTEGER% = 460                                               
    PASSED.STRING$  = SRSXF.DIR.NAME$                                   
    GOSUB CALL.F20.SESS.NUM.UTILITY                                     
    SRSXF.DIR.SESS% = F20.INTEGER.FILE.NO%                              

RETURN

\***********************************************************************
\*
\*    CALL.F20.SESS.NUM.UTILITY:  
\*                
\*
\***********************************************************************
CALL.F20.SESS.NUM.UTILITY: 
        
   FILE.OPERATION$ = FUNCTION.FLAG$
   CURRENT.REPORT.NUM% = PASSED.INTEGER%
   RC% = SESS.NUM.UTILITY (FUNCTION.FLAG$, PASSED.INTEGER%,       \
                                 PASSED.STRING$)
   IF RC% <> 0 THEN GOTO ERROR.DETECTED

RETURN

\***********************************************************************
\*
\*    CREATE.FILES:  
\*             
\***********************************************************************
CREATE.FILES:

   PRINT  TIME.STAMP$(2)  + "-Creating Direct And Log Files" 
    
   
   CREATE POSFILE SRMOD.DIR.NAME$ AS SRMOD.DIR.SESS% BUFFSIZE 32256 

   CREATE POSFILE SRPDF.DIR.NAME$ AS SRPDF.DIR.SESS% BUFFSIZE 32256 

   CREATE POSFILE SRSXF.DIR.NAME$ AS SRSXF.DIR.SESS% BUFFSIZE 32256 

RETURN

\***********************************************************************
\*
\*    OPEN.FILES:  
\*              
\*
\***********************************************************************
OPEN.FILES:

   IF END #PHKEEP.SESS.NUM% THEN PHKEEP.END
   OPEN PHKEEP.LOG.NAME$ AS PHKEEP.SESS.NUM% APPEND    

RETURN

PHKEEP.END:
    
   PRINT  TIME.STAMP$(2)  + "- No PHKEEP Log File"  
STOP 
 
\***********************************************************************
\*
\*    MAIN.PROCESSING
\*
\***********************************************************************
MAIN.PROCESSING:   

   GOSUB SEQUENTIAL.READ.SRPOG
   GOSUB HOUSEKEEP.SRPOG

   GOSUB SEQUENTIAL.READ.SRMOD
  
   GOSUB SEQUENTIAL.READ.SRSXF
  
   GOSUB SEQUENTIAL.READ.SRPDF 
  
   GOSUB COMPARE.SRMOD
   GOSUB COMPARE.SRPDF    
   GOSUB CREATE.SRMOD.KEY
   GOSUB CREATE.SRSXF.KEY
   GOSUB CREATE.SRPDF.KEY
  
   GOSUB STOP.BACKGROUNDS
   GOSUB BACKUP.ORIGINAL 
   GOSUB DELETE.EXISTING 
   GOSUB COPY.NEW        
   GOSUB START.BACKGROUNDS   

RETURN

\***********************************************************************
\*
\*    INITIAL.CHECKS:  
\*               
\*
\***********************************************************************
INITIAL.CHECKS:

   PRINT  TIME.STAMP$(2)  + "-Checking the existence of Files"    
   
   CALL OSSHELL ("IF EXIST W:/SRPOG.DIR DEL W:/SRPOG.DIR")
   CALL OSSHELL ("IF EXIST W:/SRMOD.DIR DEL W:/SRMOD.DIR")
   CALL OSSHELL ("IF EXIST W:/SRSXF.DIR DEL W:/SRSXF.DIR")
   CALL OSSHELL ("IF EXIST W:/SRPDF.DIR DEL W:/SRPDF.DIR")
   CALL OSSHELL ("IF EXIST W:/SRPOG.KEY DEL W:/SRPOG.KEY")
   CALL OSSHELL ("IF EXIST W:/SRMOD.KEY DEL W:/SRMOD.KEY")
   CALL OSSHELL ("IF EXIST W:/SRSXF.KEY DEL W:/SRSXF.KEY")
   CALL OSSHELL ("IF EXIST W:/SRPDF.KEY DEL W:/SRPDF.KEY")
   CALL OSSHELL ("IF EXIST W:/PHKEEP.OK DEL W:/PHKEEP.OK")
   CALL OSSHELL ("IF EXIST W:/PHKEEP.ERR DEL W:/PHKEEP.ERR")
   CALL OSSHELL ("IF EXIST W:/PHKEEP.LOG DEL W:/PHKEEP.LOG")
   
   PRINT  TIME.STAMP$(2)  + "-Checking the existence of D: drive SR files"
      
   CALL OSSHELL ("IF EXIST D:/SRPOG.DAT DEL D:/SRPOG.DAT")
   CALL OSSHELL ("IF EXIST D:/SRMOD.DAT DEL D:/SRMOD.DAT")
   CALL OSSHELL ("IF EXIST D:/SRSXF.DAT DEL D:/SRSXF.DAT")
   CALL OSSHELL ("IF EXIST D:/SRPDF.DAT DEL D:/SRPDF.DAT")
    
  RETURN       

\***********************************************************************
\*
\*   SEQUENTIAL.READ.SRPOG 
\*               
\*
\***********************************************************************
SEQUENTIAL.READ.SRPOG:

   PRINT  TIME.STAMP$(2)  + "-Sequential Reading SRPOG"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Sequential Reading SRPOG"
   
   FILE.OPERATION$ = "O"     
   OPEN SRPOG.FILE.NAME$ DIRECT RECL 512 AS SRPOG.SESS.NUM% NOWRITE NODEL     
   OPEN SRMAP.FILE.NAME$ KEYED RECL SRMAP.RECL% AS SRMAP.SESS.NUM%      !ARG  
   
   
   EOF%   = 0
   I%     = 0
   J%     = 1
   SRPOG% = 0
   P%     = 0
   ! RPS indicates record per sector, as it is block read, will filtering
   ! the number of records per each sector. same is obtained using the 
   ! record size against total block size.

   RPS%   = 4                   

   SRPOG.FLAG$ = "S" 
   SRPOG.COUNT% = 0   
   
     WHILE NOT EOF%
   
         I% = I% +1 
         IF END #SRPOG.SESS.NUM% THEN SRPOG.END 
         READ FORM "T5,C508"; #SRPOG.SESS.NUM%,I%; SEC$      

         FOR J% = 1 TO RPS%

             SRPOG.STRING$ = MID$(SEC$, ((J%-1)* SRPOG.RECL%) +1,      \
             SRPOG.RECL%)

             IF GETN4(SRPOG.STRING$,0) <> 0 THEN BEGIN

                 SRPOG% = SRPOG% + 1
                 SRPOG.POGDB% = GETN4(SRPOG.STRING$,0)
                 SRMAP.POGDB% = SRPOG.POGDB%                            !ARG
                 SRMAP.CHAIN.SEQ% = 0                                   !ARG

             ! Check if there is a proper match in SRMAP file as its    !ARG
             ! been proved that, everday SRMAP file is read to create   !ARG
             ! SRITEMP/SRITEML files so if any POGDB is missing from    !ARG
             ! SRMAP, the planer wont be visible from next day after    !ARG
             ! POGOK run. So any POGDB not present in SRMAP ignore it   !ARG
             ! while writing to SRPOG.DIR file.

                 RC% = READ.SRMAP                                       !ARG

                 IF RC% = 0 THEN BEGIN                                  !ARG
                     SRPOG.COUNT% = SRPOG.COUNT% + 1                    !ARG
                     SRPOG.ARRAY$(SRPOG.COUNT%) = SRPOG.STRING$         !ARG
                 ENDIF ELSE BEGIN 
                      P% = P% + 1
                      TEMP.ARR%(P%) = SRPOG.POGDB%
                 ENDIF     
                 
             ENDIF ELSE BEGIN

                 J% = RPS%
             ENDIF

         NEXT J%

     WEND

SRPOG.END: 
    
   
   EOF% = -1
   CLOSE SRPOG.SESS.NUM%

RETURN

\***********************************************************************
\*
\*   HOUSEKEEP.SRPOG
\*               
\*
\***********************************************************************
HOUSEKEEP.SRPOG:

   PRINT  TIME.STAMP$(2)  + "-Housekeeping SRPOG by checking the existence of planner in SRMAP"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Housekeeping SRPOG by checking against SRMAP"
   
   FILE.OPERATION$ = "O"  
   OPEN SRPOG.FILE.NAME$ KEYED RECL SRPOG.RECL% AS SRPOG.SESS.NUM%
   I% = 1 

    
   FOR I% = 1 TO P%
   
   
    SRPOG.POGDB% = TEMP.ARR%(I%)
    RC% = READ.SRPOG
    IF RC% = 0 THEN BEGIN
        RC% = DELREC.SRPOG
    ENDIF

   NEXT I%
   
   CLOSE SRPOG.SESS.NUM%
 RETURN

\***********************************************************************
\*
\*    SEQUENTIAL.READ.SRMOD
\*                
\*
\***********************************************************************
SEQUENTIAL.READ.SRMOD:                                                  

   PRINT  TIME.STAMP$(2)  + "-Sequential Reading SRMOD"               
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  +                         \
   "-Sequential Reading SRMOD"             
  

   FILE.OPERATION$ = "O"  
   OPEN SRMOD.FILE.NAME$ DIRECT RECL 512 AS SRMOD.SESS.NUM% BUFFSIZE    \
                                                 32767 NOWRITE NODEL  
    
   I%          = 0
   J%          = 1
   RPS%        = 1                        
   EOF%        = 0 
   SRMOD%      = 0
   SRMOD.FLAG$ = "S"
   
   WHILE NOT EOF%
   
      I% = I% +1
      IF END #SRMOD.SESS.NUM% THEN SRMOD.END
      READ FORM "T5,C508"; #SRMOD.SESS.NUM%,I%; SEC$
                                                                        
      FOR J% = 1 TO RPS%                                                
         SRMOD.STRING$ = SEC$                                           
         SRMOD.STRING$ = MID$(SEC$, 1,508)                              
         IF GETN4(SRMOD.STRING$,0) <> 0 THEN BEGIN                      
            SRMOD% = SRMOD% + 1                                         
            SRMOD.ARR$(SRMOD%) = SRMOD.STRING$                          
                                                                        
         ENDIF ELSE BEGIN

             J% = RPS%

         ENDIF

      NEXT J%
   WEND

SRMOD.END:    
  
   EOF% = -1
   CLOSE SRMOD.SESS.NUM%

RETURN

\***********************************************************************
\*
\*    SEQUENTIAL.READ.SRSXF 
\*                
\*
\***********************************************************************
SEQUENTIAL.READ.SRSXF:   

   PRINT  TIME.STAMP$(2)  +                                             \
   "-Sequential Reading SRSXF and filtering with SRPOG"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2) +                           \
               "-Sequential Reading SRSXF and filtering with SRPOG"

   FILE.OPERATION$ = "O"  
   OPEN SRSXF.FILE.NAME$ DIRECT RECL 512 AS SRSXF.SESS.NUM% BUFFSIZE    \
                                                 32767 NOWRITE NODEL 
    
   I%          = 0
   J%          = 1
   RPS%        = 8                        
   EOF%        = 0 
   SRSXF%      = 0
   
   SRSXF.FLAG$ = "S"
   
   WHILE NOT EOF%
   
      I% = I% +1
      IF END #SRSXF.SESS.NUM% THEN SRSXF.END
      READ FORM "T5,C508"; #SRSXF.SESS.NUM%,I%; SEC$
      
      FOR J% = 1 TO RPS%

         SRSXF.STRING$ = MID$(SEC$, ((J%-1)* SRSXF.RECL%) +1,          \
                                             SRSXF.RECL%) 
         IF GETN4(SRSXF.STRING$,0) <> 0 THEN BEGIN                       

            SRSXF% = SRSXF% + 1

         ! AS SRSXF file will generally have anything between 10000 to 
         ! 90000 records based on the number of planners. It is 
         ! impossible to do an array sort so filtering the records
         ! and writing it individually to the SRSXF file

            K% = 1

             WHILE K% <= SRPOG%

                 IF STR$(GETN4(SRSXF.STRING$,0)) =                       \
                                STR$(GETN4(SRPOG.ARRAY$(K%),0)) THEN     \
                                                         BEGIN

                     SRSXF.COUNT% = SRSXF.COUNT% + 1

                     GOSUB CREATE.SRSXF.DIR

                 ENDIF

                     K% = K% + 1
             WEND

         ENDIF ELSE BEGIN 

             J% = RPS%

         ENDIF

      NEXT J%

   WEND

SRSXF.END:      
    
   EOF% = -1
   CLOSE SRSXF.SESS.NUM%

RETURN 

\***********************************************************************
\*
\*    SEQUENTIAL.READ.SRPDF 
\*              
\*
\***********************************************************************
SEQUENTIAL.READ.SRPDF:   

   PRINT  TIME.STAMP$(2)  + "-Sequential Reading SRPDF"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Sequential Reading SRPDF"

   FILE.OPERATION$ = "O" 
   OPEN SRPDF.FILE.NAME$ DIRECT RECL 512 AS SRPDF.SESS.NUM% NOWRITE NODEL
    
   I%     = 0
   J%     = 1
   RPS%   = 3                        
   EOF%   = 0 
   SRPDF% = 0
   
   SRPDF.FLAG$ = "S"
   
   WHILE NOT EOF%
   
      I% = I% +1
      IF END #SRPDF.SESS.NUM% THEN SRPDF.END
      READ FORM "T5,C508"; #SRPDF.SESS.NUM%,I%; SEC$
      
      FOR J% = 1 TO RPS%

         SRPDF.STRING$ = SEC$
         SRPDF.STRING$ = MID$(SEC$, ((J%-1)* SRPDF.RECL%) +1, SRPDF.RECL%)

         IF GETN4(SRPDF.STRING$,0) <> 0 THEN BEGIN 

            SRPDF% = SRPDF% + 1
            SRPDF.ARRAY$(SRPDF%) = SRPDF.STRING$

         ENDIF  ELSE J% = RPS%

      NEXT J%

   WEND

SRPDF.END:     

   EOF% = -1
   CLOSE SRPDF.SESS.NUM%

RETURN 

\***********************************************************************
\*
\*    WRITE.PHKEEP.OK  
\*                
\*
\***********************************************************************
WRITE.PHKEEP.OK:
    
   OPEN PHKEEP.OK.NAME$ AS PHKEEP.OK.SESS% APPEND

   OK.STRING$ = SRPOG.FLAG$ + SRMOD.FLAG$ + SRSXF.FLAG$ + SRSXF.FLAG$

   WRITE #PHKEEP.OK.SESS% ; OK.STRING$
   
   CLOSE PHKEEP.OK.SESS%

RETURN


\***********************************************************************
\*
\*    COMPARE.SRMOD   
\*                
\*
\***********************************************************************
COMPARE.SRMOD:

     PRINT  TIME.STAMP$(2)  + "-Comparing SRMOD records against SRPOG"
     PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  +                       \
                           "-Comparing SRMOD records against SRPOG"
    
     I%           = 1
     J%           = 1 
     TEMP.COUNT%  = 0
     SRMOD.COUNT% = 0      

     FOR J% = 1 TO SRMOD% 

         FOUND% = 0        
         I% = 1
 
         WHILE FOUND% = 0 AND I% <= SRPOG.COUNT% 

             IF STR$(GETN4(SRMOD.ARR$(J%),0))                           \
              = STR$(GETN4(SRPOG.ARRAY$(I%),0)) THEN BEGIN

                 SRMOD.COUNT% = SRMOD.COUNT% +1

                 GOSUB CREATE.SRMOD.DIR 

                 FOUND% = -1

                 I% = SRPOG%

             ENDIF

                 I% = I% + 1
         WEND

     NEXT J%     

RETURN 

\***********************************************************************
\*
\*    COMPARE.SRPDF  
\*                
\*
\***********************************************************************
COMPARE.SRPDF:    

   PRINT  TIME.STAMP$(2)  + "-Comparing SRPDF records against SRPOG"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                             \
                        + "-Comparing SRPDF records against SRPOG"

   I% = 1
   J% = 1    
   SRPDF.COUNT% = 0

   FOR J% = 1 TO SRPDF%        

        FOUND% = 0        
        I% = 1

        WHILE FOUND% = 0 AND I% <= SRPOG.COUNT%

            IF STR$(GETN4(SRPDF.ARRAY$(J%),0))                          \
            = STR$(GETN4(SRPOG.ARRAY$(I%),0)) THEN BEGIN

                SRPDF.COUNT%  =  SRPDF.COUNT% + 1

                GOSUB CREATE.SRPDF.DIR
                FOUND% = -1 

                I% = SRPOG%

            ENDIF    

                I% = I% + 1
        WEND

   NEXT J%  

RETURN
 

\***********************************************************************
\*
\*    CREATE.SRMOD.DIR  
\*                
\*
\***********************************************************************
CREATE.SRMOD.DIR: 

   WRITE FORM "C508" ;#SRMOD.DIR.SESS% ; SRMOD.ARR$(J%)  
   
RETURN

\***********************************************************************
\*
\*    CREATE.SRSXF.DIR
\*                
\*
\***********************************************************************
CREATE.SRSXF.DIR:

    WRITE FORM "C63" ;#SRSXF.DIR.SESS% ; SRSXF.STRING$	

RETURN  

\***********************************************************************
\*
\*    CREATE.SRPDF.DIR  
\*               
\*
\***********************************************************************
CREATE.SRPDF.DIR:  

    WRITE FORM "C169" ;#SRPDF.DIR.SESS% ; SRPDF.ARRAY$(J%)   

RETURN

\***********************************************************************
\*
\*    REKEY.SRPOG  
\*               
\*
\***********************************************************************
  
  REKEY.SRPOG:   
      
    
    SRPOG.TEMP.NAME$ = "ADXLXACN::D:\ADX_UDT3\SRPOG.TST"

    PROG.NAME$ = "C:\ADX_SPGM\COMMAND.286"
    COMM.TAIL$ = "-C TURBO.286 REKEY"     + " "                         \
                   +  SRPOG.FILE.NAME$    + " "                         \
                   +  SRPOG.TEMP.NAME$    + " "                         \
                   + "-R:" + STR$(6000)   + " "                         \
                   +  SPACE$ +                                          \
                   + "-O"  +  " "         + " "                         \
                   + "-Z"  +  " "         + " "                         \
                   + "-D:" + STR$(3)      + " "                         !

    SVC.PARAM$   = STRING$(28,"0")
    GOSUB EXECUTE.SVC

    SRPOG.FLAG$ = "E"

RETURN


RETURN

\***********************************************************************
\*
\*    CREATE.SRMOD.KEY  
\*               
\*
\***********************************************************************
CREATE.SRMOD.KEY:

     PRINT  TIME.STAMP$(2)  + "-Creating SRMOD Key file from direct file"
     PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                            \
                     + "-Creating SRMOD Key file from direct file"   
     
     CLOSE SRMOD.DIR.SESS%

     PROG.NAME$ = "C:\ADX_SPGM\COMMAND.286"
     COMM.TAIL$ = "-C TURBO.286 KEYUP"     + " "                         \
                   +  SRMOD.DIR.NAME$     + " "                         \
                   +  SRMOD.KEY.NAME$     + " "                         \
                   + "-k:" + STR$(6)      + " "                         \
                   + "-l:" + STR$(508)    + " "                         \
                   + "-r:" + STR$(8000)   + " "                         \
                   + "-o"  +  " "         + " "                         \
                   + "-d:" + STR$(3)      + " "                         !

     SVC.PARAM$   = STRING$(28,"0")
     GOSUB EXECUTE.SVC


RETURN
\***********************************************************************
\*                
\*    CREATE.SRSXF.KEY  
\*                
\*
\***********************************************************************
CREATE.SRSXF.KEY:

     PRINT  TIME.STAMP$(2)  + "-Creating SRSXF Key file from direct file"
     PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                           \
                  + "-Creating SRSXF Key file from direct file" 

     CLOSE SRSXF.DIR.SESS%

     PROG.NAME$ = "C:\ADX_SPGM\COMMAND.286"
     COMM.TAIL$ = "-C TURBO.286 KEYUP"     + " "                        \
                   +  SRSXF.DIR.NAME$     + " "                         \
                   +  SRSXF.KEY.NAME$     + " "                         \
                   + "-k:" + STR$(6)      + " "                         \
                   + "-l:" + STR$(63)     + " "                         \
                   + "-r:" + STR$(60000)  + " "                         \
                   + "-o"  +  " "         + " "                         \
                   + "-d:" + STR$(3)      + " "                         !

     SVC.PARAM$   = STRING$(28,"0")
     GOSUB EXECUTE.SVC

RETURN

\***********************************************************************
\*
\*    CREATE.SRPDF.KEY 
\*               
\*
\***********************************************************************
CREATE.SRPDF.KEY:

     PRINT  TIME.STAMP$(2)  + "-Creating SRPDF Key file from direct file"
     PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                           \
               + "-Creating SRPDF Key file from direct file"
     
     CLOSE SRPDF.DIR.SESS%

     PROG.NAME$ = "C:\ADX_SPGM\COMMAND.286"
     COMM.TAIL$ = "-C TURBO.286 KEYUP"     + " "                        \
                   +  SRPDF.DIR.NAME$    + " "                          \
                   +  SRPDF.KEY.NAME$    + " "                          \
                   + "-k:" + STR$(4)     + " "                          \
                   + "-l:" + STR$(169)   + " "                          \
                   + "-r:" + STR$(6000)  + " "                          \
                   + "-o"  +  " "        + " "                          \
                   + "-d:" + STR$(3)     + " "                          !

     SVC.PARAM$   = STRING$(28,"0")
     GOSUB EXECUTE.SVC
    
RETURN

\*****************************************************************************
\***    SUBROUTINE : EXECUTE.SVC                                             *
\***    DETAILS    : EXECUTES APPLICATION ASYNC                              *
\*****************************************************************************

EXECUTE.SVC:

     PINFO.TABLE$  = "DURATION  " + CHR$(200) + STRING$(9,CHR$(00))
     PROG.NAME$   = PROG.NAME$ + CHR$(00)
     COMM.TAIL$   = COMM.TAIL$ + CHR$(00)
     CALL PUTN4(SVC.PARAM$,  0, 00000000H)
     CALL PUTN4(SVC.PARAM$,  4, 00000000H)
     CALL PUTN4(SVC.PARAM$,  8, SADD(PROG.NAME$) + 2)
     CALL PUTN4(SVC.PARAM$, 12, SADD(COMM.TAIL$) + 2)
     CALL PUTN4(SVC.PARAM$, 16, LEN(COMM.TAIL$) - 1)
     CALL PUTN4(SVC.PARAM$, 20, SADD(PINFO.TABLE$) + 2)
     CALL PUTN4(SVC.PARAM$, 24, 00000000H)
     CALL FLEXOSSVC(38, SVC.PARAM$)

RETURN
\***********************************************************************
\*
\*    STOP.BACKGROUNDS 
\*                
\*
\***********************************************************************
STOP.BACKGROUNDS:
 
     PRINT  TIME.STAMP$(2)  + "-Stopping BGMON Application"
     PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                             \
                               + "-Stopping BGMON Application"   
   
     CALL OSSHELL("C:\ADX_UPGM\BGSTOP" + SPACE$ + "ADX_UPGM:BGMON.286")   
   
     PRINT  TIME.STAMP$(2)  + "-Stopping TRANSACT Application"
     PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                            \
                       + "-Stopping TRANSACT Application" 
   
     CALL OSSHELL("C:\ADX_UPGM\BGSTOP" + SPACE$ + "ADX_UPGM:TRANSACT.286")  
 
RETURN

\***********************************************************************
\*
\*    BACKUP.ORIGINAL
\*                
\*
\***********************************************************************
BACKUP.ORIGINAL:

   PRINT  TIME.STAMP$(2)  + "-Creating backups"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Creating backups"    

   CALL ADXCOPYF(RC%,"ADXLXACN::D:\ADX_UDT3\SRPOG.DAT",                 \
                     "ADXLXACN::D:\SRPOG.DAT",0,0)   

   IF RC% = 0 THEN BEGIN

       PRINT  TIME.STAMP$(2)  + "-Backing up of SRPOG Successful"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
                     + "-Backing up of SRPOG Successful"

   ENDIF

   CALL ADXCOPYF(RC%,"ADXLXACN::D:\ADX_UDT3\SRMOD.DAT",                 \
                     "ADXLXACN::D:\SRMOD.DAT",0,0)

   IF RC% = 0 THEN BEGIN

       PRINT  TIME.STAMP$(2)  + "-Backing up of SRMOD Successful"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
             + "-Backing up of SRMOD Successful"

   ENDIF   


   CALL ADXCOPYF(RC%,"ADXLXACN::D:\ADX_UDT3\SRSXF.DAT",                 \
                     "ADXLXACN::D:\SRSXF.DAT",0,0) 

   IF RC% = 0 THEN BEGIN

       PRINT  TIME.STAMP$(2)  + "-Backing up of SRSXF Successful"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
                              + "-Backing up of SRSXF Successful"

   ENDIF

   CALL ADXCOPYF(RC%,"ADXLXACN::D:\ADX_UDT3\SRPDF.DAT",                 \
                     "ADXLXACN::D:\SRPDF.DAT",0,0)   

   IF RC% = 0 THEN BEGIN

       PRINT  TIME.STAMP$(2)  + "-Backing up of SRPDF Successful"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
                        + "-Backing up of SRPDF Successful"

   ENDIF

RETURN

\***********************************************************************
\*
\*    DELETE.EXISTING 
\*              
\*
\***********************************************************************
DELETE.EXISTING:    

   PRINT  TIME.STAMP$(2)  + "-Deleting SRMOD original"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                            \
                                     + "-Deleting SRMOD original"    
  
   OPEN SRMOD.FILE.NAME$ AS SRMOD.SESS.NUM%

   DELETE SRMOD.SESS.NUM%
   
   PRINT  TIME.STAMP$(2)  + "-Deleting SRPDF original"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  +                         \
                                         "-Deleting SRPDF original"    
  
   OPEN SRPDF.FILE.NAME$ AS SRPDF.SESS.NUM%

   DELETE SRPDF.SESS.NUM%
   
   PRINT  TIME.STAMP$(2)  + "-Deleting SRSXF original"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  +                          \
                                     "-Deleting SRSXF original"    
  
   OPEN SRSXF.FILE.NAME$ AS SRSXF.SESS.NUM%

   DELETE SRSXF.SESS.NUM%  

RETURN

\***********************************************************************
\*
\*    COPY.NEW 
\*             
\*
\***********************************************************************
COPY.NEW:    

   PRINT  TIME.STAMP$(2)  + "-Rekeying SRPOG"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Rekeying SRPOG"    
   
   GOSUB REKEY.SRPOG                                                    ! ARG

   CALL ADXCOPYF(RC%,"ADXLXACN::W:\SRMOD.KEY",                         \
                     "ADXLXACN::D:\ADX_UDT3\SRMOD.DAT",0,0)   

   IF RC% = 0 THEN BEGIN

       PRINT  TIME.STAMP$(2)  + "- New SRMOD has been copied successfuly"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
                + "-New SRMOD has been copied successfuly"
       SRMOD.FLAG$ = "E"

   ENDIF
   
   CALL ADXCOPYF(RC%,"ADXLXACN::W:\SRSXF.KEY",                          \
                     "ADXLXACN::D:\ADX_UDT3\SRSXF.DAT",0,0)   

   IF RC% = 0 THEN BEGIN

       PRINT  TIME.STAMP$(2)  + "- New SRSXF has been copied successfuly"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
                      + "-New SRSXF has been copied successfuly"
       SRSXF.FLAG$ = "E"

   ENDIF   

   CALL ADXCOPYF(RC%,"ADXLXACN::W:\SRPDF.KEY",                          \
                     "ADXLXACN::D:\ADX_UDT3\SRPDF.DAT",0,0) 

   IF RC% = 0 THEN BEGIN
   
       PRINT  TIME.STAMP$(2)  + "- New SRPDF has been copied successfuly"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
                      + "-New SRPDF has been copied successfuly"

       SRPDF.FLAG$ = "E"

   ENDIF


RETURN


\***********************************************************************
\*
\*    START.BACKGROUNDS 
\*                
\*
\***********************************************************************
START.BACKGROUNDS:
 
   PRINT  TIME.STAMP$(2)  + "-Starting BGMON Application"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                             \
                               + "-Starting BGMON Application"   
   
   CALL OSSHELL("C:\ADX_UPGM\BGSTART" + SPACE$ + "ADX_UPGM:BGMON.286")   
   
   PRINT  TIME.STAMP$(2)  + "-Starting TRANSACT Application"
   PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                             \
                                   + "-Starting TRANSACT Application"    
   
   CALL OSSHELL("C:\ADX_UPGM\BGSTART" + SPACE$                          \
                                           + "ADX_UPGM:TRANSACT.286")    
 
RETURN

\***********************************************************************
\*
\*    TERMINATION   
\*               
\*
\***********************************************************************
TERMINATION:

   GOSUB WRITE.PHKEEP.OK     
   
   IF ERROR.CNT% = 0 THEN BEGIN  

       PRINT  TIME.STAMP$(2)  + "-Renaming error file to OK file"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
        + "-Renaming error file to OK file"    

       RC% = RENAME("W:/PHKEEP.OK","W:/PHKEEP.ERR")

 
       PRINT  TIME.STAMP$(2)  + "-Read " +  STR$(SRPOG%) +              \
                          " SRPOG records"  + \
                          + " Written " + STR$(SRPOG.COUNT%) +          \
                            " In to New file"

       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Read " +           \
                                                  STR$(SRPOG%)      +   \
         " SRPOG records" + " Written " + STR$(SRPOG.COUNT%)            \
                                        + " In to New file"


       PRINT  TIME.STAMP$(2)  + "-Read " +  STR$(SRMOD%) +              \
                          " SRMOD records"  + \
                          + " Written " + STR$(SRMOD.COUNT%) +          \
                            " In to New file"

       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Read " +           \
                                                  STR$(SRMOD%)      +   \
         " SRMOD records" + " Written " + STR$(SRMOD.COUNT%)            \
                                        + " In to New file"

       PRINT  TIME.STAMP$(2)  + "-Read " +  STR$(SRSXF%)                \
                                        +  " SRSXF records"  +          \
                         + " Written " + STR$(SRSXF.COUNT%)             \
                         + " In to New file"

       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Read "             \
                                              +  STR$(SRSXF%) +         \
        " SRSXF records" + " Written " + STR$(SRSXF.COUNT%)             \
                                    + " In to New file"

       PRINT  TIME.STAMP$(2)  + "-Read " +  STR$(SRPDF%)                \
                        + " SRPDF records"  + \
                        + " Written " + STR$(SRPDF.COUNT%)              \
                        + " In to New file"

       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)  + "-Read " +           \
                         STR$(SRPDF%) +  \
       " SRPDF records" + " Written " + STR$(SRPDF.COUNT%)              \
       + " In to New file"

       PRINT  TIME.STAMP$(2)  + "- Program completed successfully"
       PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                         \
              + "- Program completed successfuly"

   ENDIF


   GOSUB CLOSE.FILES
   GOSUB HOUSEKEEP.WDRIVE
   GOSUB DEALLOCATE.SESS.NUM
     

RETURN

\***********************************************************************
\*
\*    HOUSEKEEP.WDRIVE  
\*               
\*
\*********************************************************************** 
HOUSEKEEP.WDRIVE:

   CALL OSSHELL ("IF EXIST W:/SRPOG.DIR DEL W:/SRPOG.DIR")
   CALL OSSHELL ("IF EXIST W:/SRMOD.DIR DEL W:/SRMOD.DIR")
   CALL OSSHELL ("IF EXIST W:/SRSXF.DIR DEL W:/SRSXF.DIR")
   CALL OSSHELL ("IF EXIST W:/SRPDF.DIR DEL W:/SRPDF.DIR")
   CALL OSSHELL ("IF EXIST W:/SRPOG.KEY DEL W:/SRPOG.KEY")
   CALL OSSHELL ("IF EXIST W:/SRMOD.KEY DEL W:/SRMOD.KEY")
   CALL OSSHELL ("IF EXIST W:/SRSXF.KEY DEL W:/SRSXF.KEY")
   CALL OSSHELL ("IF EXIST W:/SRPDF.KEY DEL W:/SRPDF.KEY")


RETURN

\***********************************************************************
\*
\*    ERROR.LOG  
\*               
\*
\***********************************************************************
ERROR.LOG: 
        
         VAR.STRING.1$ = FILE.OPERATION$                     +          \
                CHR$(SHIFT(CURRENT.REPORT.NUM%, 8) AND 0FFH) +          \
                CHR$(CURRENT.REPORT.NUM% AND 0FFH)           +          \
                CURRENT.CODE$
         VAR.STRING.2$ = "PHKEEP"
         MESSAGE.NO%   = 0
         EVENT.NO%     = 106

         RC% = APPLICATION.LOG(MESSAGE.NO%,VAR.STRING.1$,               \
                      VAR.STRING.2$,EVENT.NO%)

         GOSUB WRITE.PHKEEP.OK

         GOTO END.PROGRAM

RETURN

\***********************************************************************
\*
\*    CLOSE.FILES:
\*                
\*
\***********************************************************************

CLOSE.FILES:
   
   CLOSE PHKEEP.SESS.NUM%
   
RETURN  
\***********************************************************************
\*
\*    DEALLOCATE.SESS.NUM  
\*                
\*
\***********************************************************************
DEALLOCATE.SESS.NUM: 

    FUNCTION.FLAG$ = "C"   
    
    PASSED.INTEGER% = SRPOG.REPORT.NUM%                                
    PASSED.STRING$  = SRPOG.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRPOG.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = SRMOD.REPORT.NUM%                                
    PASSED.STRING$  = SRMOD.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRMOD.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = SRSXF.REPORT.NUM%                                
    PASSED.STRING$  = SRSXF.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRSXF.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = SRPDF.REPORT.NUM%                                
    PASSED.STRING$  = SRPDF.FILE.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRPDF.SESS.NUM% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = PHKEEP.SESS.NUM%                                 
    PASSED.STRING$  = PHKEEP.LOG.NAME$                                 
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    PHKEEP.SESS.NUM% = F20.INTEGER.FILE.NO%                            

    PASSED.INTEGER% = PHKEEP.OK.SESS%                                  
    PASSED.STRING$  = PHKEEP.OK.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    PHKEEP.OK.SESS% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = 458                                              
    PASSED.STRING$  = SRMOD.DIR.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRMOD.DIR.SESS% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = 459                                              
    PASSED.STRING$  = SRPDF.DIR.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRPDF.DIR.SESS% = F20.INTEGER.FILE.NO%                             

    PASSED.INTEGER% = 460                                              
    PASSED.STRING$  = SRSXF.DIR.NAME$                                  
    GOSUB CALL.F20.SESS.NUM.UTILITY                                    
    SRSXF.DIR.SESS% = F20.INTEGER.FILE.NO%                             


RETURN

\***********************************************************************
\*
\*    ERROR.DETECTED  
\*              
\*
\***********************************************************************
ERROR.DETECTED:     
   

     ERROR.CNT% = ERROR.CNT% + 1
	 
	 IF ERR = "OE" AND ERRF% = SRMAP.SESS.NUM% THEN BEGIN
         PRINT  TIME.STAMP$(2)  + "-NO SRMAP File" +                    \
                                        " program can't be continued"
         PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                       \
                + "-NO SRMAP File, program can't be continued"
         SRMAP.FLAG$ = "X" 
         GOSUB ERROR.LOG
     ENDIF

     IF ERR = "OE" AND ERRF% = SRPOG.SESS.NUM% THEN BEGIN
         PRINT  TIME.STAMP$(2)  + "-NO SRPOG File" +                    \
                                        " program can't be continued"
         PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                       \
                + "-NO SRPOG File, program can't be continued"
         SRPOG.FLAG$ = "X" 
         GOSUB ERROR.LOG
     ENDIF
   
     IF ERR = "OE" AND ERRF% = SRMOD.SESS.NUM% THEN BEGIN
         PRINT  TIME.STAMP$(2)  + "-NO SRMOD File" +                    \
                                    ",program can't be continued"
         PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                       \
                + "-NO SRMOD File, program can't be continued"
         SRMOD.FLAG$ = "X" 
         GOSUB ERROR.LOG
     ENDIF
   
     IF ERR = "OE" AND ERRF% = SRSXF.SESS.NUM% THEN BEGIN
         PRINT  TIME.STAMP$(2)  + "-NO SRSXF File," +                   \
                                       "program can't be continued"
         PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                       \
              + "-NO SRSXF File, program can't be continued"
         SRSXF.FLAG$ = "X" 
         GOSUB ERROR.LOG
     ENDIF
   
     IF ERR = "OE" AND ERRF% = SRPDF.SESS.NUM% THEN BEGIN
         PRINT  TIME.STAMP$(2)  + "-NO SRPDF File," +                   \
                                        "program can't be continued"
         PRINT  #PHKEEP.SESS.NUM%; TIME.STAMP$(2)                       \
               + "-NO SRPDF File, program can't be continued"
         SRPDF.FLAG$ = "X" 
         GOSUB ERROR.LOG
     ENDIF    
   
     GOSUB ERROR.LOG
   
RETURN  

\***********************************************************************
\*
\*    END.PROGRAM 
\*              
\*
\***********************************************************************

   END.PROGRAM:

   END 
       

