#ifndef SRFILES_H
#define SRFILES_H

#include "trxfile.h"

typedef struct {
   BYTE abSRDELTASerial[4];
   BYTE abSRMAPSerial[4];
   BYTE abSRDELTADate[4];           //YYYYMMDD (packed)
   BYTE abSRMAPDate[4];             //YYYYMMDD (packed)
   BYTE cReload;                    //Y/N
   BYTE cSRP10;                     //SEXY
   BYTE cSRP05;                     //SEXY
   BYTE cSRP06;                     //SEXY
   BYTE cSRP07;                     //SEXY
   BYTE cSRP04;                     //SEXY
   BYTE cSRP10rc;                   //0=ok
                                    //1=record count mismatch
                                    //2=missing trailer
                                    //3=out of seq. serial num
                                    //4=old file previous processed
                                    //5=delta file sent when initial load was expected
                                    //6=input file not found
                                    //7=run not required, awaiting previous application
   BYTE cSRP05rc;                   //As above
   BYTE cSRP06rc;                   //As above
   BYTE cSRP07rc;                   //As above
   BYTE cSRP04rc;                   //As above
   BYTE abSRDELTAFailedSerial[4];
   BYTE abSRDELTAFailedDate[4];     //YYYYMMDD (packed)
   BYTE abSRMAPFailedSerial[4];
   BYTE abSRMAPFailedDate[4];       //YYYYMMDD (packed)
   BYTE abSRP10Date[4];             //YYYYMMDD (packed)
   BYTE abSRP05Date[4];             //YYYYMMDD (packed)
   BYTE abSRP06Date[4];             //YYYYMMDD (packed)
   BYTE abSRP07Date[4];             //YYYYMMDD (packed)
   BYTE abSRP04Date[4];             //YYYYMMDD (packed)
   WORD wRetainDays;
   BYTE cSRP19;                     //SEXY
   BYTE cSRP19rc;                   //As above (0=ok...)
   BYTE abSRP19Date[4];             //YYYYMMDD (packed)
   BYTE SRDELTARecCount[4];
   BYTE SRMAPRecCount[4];
   BYTE bFiller;
} POGOK_REC;

typedef struct {                                        //CSk 12-03-2012 SFA
    ULONG ulPOGDB;    // Key = POGDB + ubRecChain       //CSk 12-03-2012 SFA
    UBYTE ubRecChain;          //Base 0                 //CSk 12-03-2012 SFA
    BYTE FragmentStartDate[4]; //YYYYMMDD (packed)      //CSk 12-03-2012 SFA
    BYTE FragmentEndDate[4];   //YYYYMMDD (packed)      //CSk 12-03-2012 SFA
    UBYTE ubRepeatCnt;                                  //CSk 12-03-2012 SFA
    BYTE  bFiller[4];                                   //CSk 12-03-2012 SFA
} SRMAP_REC;                                            //CSk 12-03-2012 SFA

typedef struct {
    ULONG ulKey;
    ULONG ulID;
    BYTE abActiveDate[4];           //YYYYMMDD (packed)
    BYTE abDeactiveDate[4];         //YYYYMMDD (packed)
    BYTE abDesc[30];
    BYTE abSRCATDesc[30];
    UBYTE ubModuleCount;
    ULONG ulSRCATKey;
    UBYTE ubKeyHierachy;            //1, 2 or 3
    UBYTE ubLiveRepeatCount;
    BYTE abDateRepeatCount[4];      //YYYYMMDD (packed)
    UBYTE ubPendingRepeatCount;
    LONG lCat1ID;
    LONG lCat2ID;
    LONG lCat3ID;
    BYTE bFiller;
} SRPOG_REC;

typedef struct {
    UBYTE ubShelfNum;
    UBYTE ubFacings;
    BYTE abItemCode[3];
    UWORD uwMDQ;                        //Min Display Qty
    UWORD uwPSC;                        //Physical Shelf Capacity
} ShelfItem;
#define SRMOD_NUM_ITEMS 50
typedef struct {
    ULONG ulKey;
    UBYTE ubModSeq;                     //Base 0
    UBYTE ubRecChain;                   //Base 0                                //SDH 20-May-2009 Model Day
    BYTE abModDesc[30];                                                         //SDH 20-May-2009 Model Day
    ShelfItem aShelfItem[SRMOD_NUM_ITEMS];
    UWORD uwShelfCount;
    UWORD uwItemCoun;
    BYTE abFiller[18];
} SRMOD_REC;

typedef struct {
    ULONG ulPOGKey;
    UBYTE ubModSeq;
    UBYTE ubRepeatCnt;                                                          //SDH 20-May-2009 Model Day
    BYTE bCoreFlag;                 //Core=Y, NonCore=N
} SritmModuleKey;                                                               //SDH 20-May-2009 Model Day
#define SRITM_NUM_MODS  16
typedef struct {
    BYTE abItemCode[3];
    UBYTE ubRecChain;               //Base 0
    UBYTE ubModuleCount;
    UWORD uwCoreItemCount;
    UWORD uwNonCoreItemCount;
    SritmModuleKey aModuleKey[SRITM_NUM_MODS];                                  //SDH 20-May-2009 Model Day
    BYTE abFiller[6];
} SRITML_REC, SRITMP_REC;

typedef struct {
    BYTE abDesc[50];                //50 bytes legacy.  desc is actually 30 bytes on other files
    ULONG ulPOGLiveIndexPtr;
    ULONG ulPOGPendingIndexPtr;
    UBYTE ubFamilyType;             //1=CORE,2=Gondola-end,3=Mid gondola,4=PSDU
    UBYTE ubKeyHierachy;
    ULONG ulSRCATKey;
} SRPOGIF_REC;

typedef struct {
    ULONG ulPOGKey;
    ULONG ulNextPOGIRec;            //Next planner in family (or 0xFFFFFFFF for end of chain)
    ULONG ulPOGId;                  //Non-unique
    BYTE abDesc[30];
    BYTE abActiveDate[4];           //YYYYMMDD (packed)
    BYTE abDeactiveDate[4];         //YYYYMMDD (packed)
    UBYTE ubModuleCount;
    ULONG ulSRCATKey;
    BYTE abFiller1[4];
    UBYTE ubSRMAPRepeatCnt;
    BYTE bCoreFlag;                 //Y=Core,N=NonCore
    BYTE abFiller2[3];
} SRPOGIL_REC, SRPOGIP_REC;

typedef struct {
    ULONG ulKey;
    BYTE abDesc[30];
    UBYTE ubKeyHierachy;
    BYTE bCoreFlag;                 //Y=Core,N=NonCore
    BYTE abFiller[28];
} SRCAT_REC;

typedef struct {
    ULONG ulKey;
    BYTE abDesc[30];
} SRSDF_REC;

typedef struct {
    ULONG ulPOGDB;
    UBYTE ubModSeq;
    UBYTE ubShelfNum;
    UBYTE ubNotchNum;
    ULONG ulShelfDescKey;
    BYTE abShelfDesc[30];
    BYTE abFiller[2];
} SRSXF_REC;

extern FILE_CTRL pogok;
extern FILE_CTRL srmap;                                       //CSk 12-03-2012 SFA
extern FILE_CTRL srpog;
extern FILE_CTRL srmod;
extern FILE_CTRL sritml;
extern FILE_CTRL sritmp;
extern FILE_CTRL srpogif;
extern FILE_CTRL srpogil;
extern FILE_CTRL srpogip;
extern FILE_CTRL srcat;
extern FILE_CTRL srsxf;
extern POGOK_REC pogokrec;
extern SRMAP_REC srmaprec;                                    //CSk 12-03-2012 SFA
extern SRPOG_REC srpogrec;
extern SRMOD_REC srmodrec;
extern SRITML_REC sritmlrec;
extern SRITMP_REC sritmprec;
extern SRPOGIF_REC srpogifrec;
extern SRPOGIL_REC srpogilrec;
extern SRPOGIP_REC srpogiprec;
extern SRCAT_REC srcatrec;
extern SRSDF_REC srsdfrec;
extern SRSXF_REC srsxfrec;

void PogokSet(void);
URC PogokOpen(void);
URC PogokClose(WORD type);
LONG PogokRead(LONG lRecNum, LONG lLineNumber);

void SrmapSet(void);                                     //CSk 12-03-2012 SFA
URC SrmapOpen(void);                                     //CSk 12-03-2012 SFA
URC SrmapClose(WORD type);                               //CSk 12-03-2012 SFA
LONG SrmapRead(LONG lLineNum);                           //CSk 12-03-2012 SFA

void SrpogSet(void);
URC SrpogOpen(void);
URC SrpogClose(WORD type);
LONG SrpogRead(LONG lLineNum);

void SrmodSet(void);
URC SrmodOpen(void);
URC SrmodClose(WORD type);
LONG SrmodRead(LONG lLineNum);

void SritmlSet(void);
URC SritmlOpen(void);
URC SritmlClose(WORD type);
LONG SritmlRead(LONG lLineNum);

void SritmpSet(void);
URC SritmpOpen(void);
URC SritmpClose(WORD type);
LONG SritmpRead(LONG lLineNum);

void SrpogifSet(void);
URC SrpogifOpen(void);
URC SrpogifClose(WORD type);
LONG SrpogifRead(LONG lRecNum, LONG lLineNum);

void SrpogilSet(void);
URC SrpogilOpen(void);
URC SrpogilClose(WORD type);
LONG SrpogilRead(LONG lRecNum, LONG lLineNum);

void SrpogipSet(void);
URC SrpogipOpen(void);
URC SrpogipClose(WORD type);
LONG SrpogipRead(LONG lRecNum, LONG lLineNum);

void SrcatSet(void);
URC SrcatOpen(void);
URC SrcatClose(WORD type);
LONG SrcatRead(LONG lRecNum, LONG lLineNumber);

void SrsxfSet(void);
URC SrsxfOpen(void);
URC SrsxfClose(WORD type);
LONG SrsxfRead(LONG lLineNum);

#endif

