#include "transact.h"

#include "trxfile.h"
#include "srfiles.h"

FILE_CTRL pogok;
FILE_CTRL srpog;
FILE_CTRL srmap;                               //CSk 12-03-2012 SFA
FILE_CTRL srmod;
FILE_CTRL sritml;
FILE_CTRL sritmp;
FILE_CTRL srpogif;
FILE_CTRL srpogil;
FILE_CTRL srpogip;
FILE_CTRL srcat;
FILE_CTRL srsdf;
FILE_CTRL srsxf;
POGOK_REC pogokrec;
SRPOG_REC srpogrec;
SRMAP_REC srmaprec;                            //CSk 12-03-2012 SFA
SRMOD_REC srmodrec;
SRITML_REC sritmlrec;
SRITMP_REC sritmprec;
SRPOGIF_REC srpogifrec;
SRPOGIL_REC srpogilrec;
SRPOGIP_REC srpogiprec;
SRCAT_REC srcatrec;
SRSDF_REC srsdfrec;
SRSXF_REC srsxfrec;

void PogokSet(void) {
    pogok.sessions   = 0;
    pogok.fnum       = -1L;
    pogok.pbFileName = "POGOK";
    pogok.wOpenFlags = A_READ | A_SHARE;
    pogok.wReportNum = 718;
    pogok.wRecLth    = 80L;
    pogok.wKeyLth    = 0;
    pogok.pBuffer    = &pogokrec;
}
URC PogokOpen(void) {return direct_open(&pogok, TRUE);}
URC PogokClose(WORD type) {return close_file(type, &pogok);}
LONG PogokRead(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&pogok, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG PogokWrite(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&pogok, lRecNum, lLineNumber, LOG_ALL);}

void SrpogSet(void) {
    srpog.sessions   = 0;
    srpog.fnum       = -1L;
    srpog.pbFileName = "SRPOG";
    srpog.wOpenFlags = A_READ | A_SHARE;
    srpog.wReportNum = 719;
    srpog.wRecLth    = 101L;
    srpog.wKeyLth    = 4;
    srpog.pBuffer    = &srpogrec;
}
URC SrpogOpen(void) {return keyed_open(&srpog, TRUE);}
URC SrpogClose(WORD type) {return close_file(type, &srpog);}
LONG SrpogRead(LONG lLineNumber) {return ReadKeyed(&srpog, lLineNumber, LOG_CRITICAL);}
LONG SrpogWrite(LONG lLineNumber) {return WriteKeyed(&srpog, lLineNumber, LOG_ALL);}

void SrmapSet(void) {                                                                         //CSk 12-03-2012 SFA
    srmap.sessions   = 0;                                                                     //CSk 12-03-2012 SFA
    srmap.fnum       = -1L;                                                                   //CSk 12-03-2012 SFA
    srmap.pbFileName = "SRMAP";                                                               //CSk 12-03-2012 SFA
    srmap.wOpenFlags = A_READ | A_SHARE;                                                      //CSk 12-03-2012 SFA
    srmap.wReportNum = 720;                                                                   //CSk 12-03-2012 SFA
    srmap.wRecLth    = 18L;                                                                   //CSk 12-03-2012 SFA
    srmap.wKeyLth    = 5;                                                                     //CSk 12-03-2012 SFA
    srmap.pBuffer    = &srmaprec;                                                             //CSk 12-03-2012 SFA
}                                                                                             //CSk 12-03-2012 SFA
URC SrmapOpen(void) {return keyed_open(&srmap, TRUE);}                                        //CSk 12-03-2012 SFA
URC SrmapClose(WORD type) {return close_file(type, &srmap);}                                  //CSk 12-03-2012 SFA
LONG SrmapRead(LONG lLineNumber) {return ReadKeyed(&srmap, lLineNumber, LOG_CRITICAL);}       //CSk 12-03-2012 SFA
LONG SrmapWrite(LONG lLineNumber) {return WriteKeyed(&srmap, lLineNumber, LOG_ALL);}          //CSk 12-03-2012 SFA

void SrmodSet(void) {
    srmod.sessions   = 0;
    srmod.fnum       = -1L;
    srmod.pbFileName = "SRMOD";
    srmod.wOpenFlags = A_READ | A_SHARE;
    srmod.wReportNum = 719;
    srmod.wRecLth    = 508L;
    srmod.wKeyLth    = 6;
    srmod.pBuffer    = &srmodrec;
}
URC SrmodOpen(void) {return keyed_open(&srmod, TRUE);}
URC SrmodClose(WORD type) {return close_file(type, &srmod);}
LONG SrmodRead(LONG lLineNumber) {return ReadKeyed(&srmod, lLineNumber, LOG_CRITICAL);}
LONG SrmodWrite(LONG lLineNumber) {return WriteKeyed(&srmod, lLineNumber, LOG_ALL);}

void SritmlSet(void) {
    sritml.sessions   = 0;
    sritml.fnum       = -1L;
    sritml.pbFileName = "SRITML";
    sritml.wOpenFlags = A_READ | A_SHARE;
    sritml.wReportNum = 721;
    sritml.wRecLth    = 127L;
    sritml.wKeyLth    = 4;
    sritml.pBuffer    = &sritmlrec;
}
URC SritmlOpen(void) {return keyed_open(&sritml, TRUE);}
URC SritmlClose(WORD type) {return close_file(type, &sritml);}
LONG SritmlRead(LONG lLineNumber) {return ReadKeyed(&sritml, lLineNumber, LOG_CRITICAL);}
LONG SritmlWrite(LONG lLineNumber) {return WriteKeyed(&sritml, lLineNumber, LOG_ALL);}

void SritmpSet(void) {
    sritmp.sessions   = 0;
    sritmp.fnum       = -1L;
    sritmp.pbFileName = "SRITMP";
    sritmp.wOpenFlags = A_READ | A_SHARE;
    sritmp.wReportNum = 722;
    sritmp.wRecLth    = 127L;
    sritmp.wKeyLth    = 4;
    sritmp.pBuffer    = &sritmprec;
}
URC SritmpOpen(void) {return keyed_open(&sritmp, TRUE);}
URC SritmpClose(WORD type) {return close_file(type, &sritmp);}
LONG SritmpRead(LONG lLineNumber) {return ReadKeyed(&sritmp, lLineNumber, LOG_CRITICAL);}
LONG SritmpWrite(LONG lLineNumber) {return WriteKeyed(&sritmp, lLineNumber, LOG_ALL);}

void SrpogifSet(void) {
    srpogif.sessions   = 0;
    srpogif.fnum       = -1L;
    srpogif.pbFileName = "SRPOGIF";
    srpogif.wOpenFlags = A_READ | A_SHARE;
    srpogif.wReportNum = 723;
    srpogif.wRecLth    = 64L;
    srpogif.wKeyLth    = 0;
    srpogif.pBuffer    = &srpogifrec;
}
URC SrpogifOpen(void) {return direct_open(&srpogif, TRUE);}
URC SrpogifClose(WORD type) {return close_file(type, &srpogif);}
LONG SrpogifRead(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srpogif, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG SrpogifWrite(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srpogif, lRecNum, lLineNumber, LOG_ALL);}

void SrpogilSet(void) {
    srpogil.sessions   = 0;
    srpogil.fnum       = -1L;
    srpogil.pbFileName = "SRPOGIL";
    srpogil.wOpenFlags = A_READ | A_SHARE;
    srpogil.wReportNum = 724;
    srpogil.wRecLth    = 64L;
    srpogil.wKeyLth    = 0;
    srpogil.pBuffer    = &srpogilrec;
}
URC SrpogilOpen(void) {return direct_open(&srpogil, TRUE);}
URC SrpogilClose(WORD type) {return close_file(type, &srpogil);}
LONG SrpogilRead(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srpogil, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG SrpogilWrite(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srpogil, lRecNum, lLineNumber, LOG_ALL);}

void SrpogipSet(void) {
    srpogip.sessions   = 0;
    srpogip.fnum       = -1L;
    srpogip.pbFileName = "SRPOGIP";
    srpogip.wOpenFlags = A_READ | A_SHARE;
    srpogip.wReportNum = 725;
    srpogip.wRecLth    = 64L;
    srpogip.wKeyLth    = 0;
    srpogip.pBuffer    = &srpogiprec;
}
URC SrpogipOpen(void) {return direct_open(&srpogip, TRUE);}
URC SrpogipClose(WORD type) {return close_file(type, &srpogip);}
LONG SrpogipRead(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srpogip, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG SrpogipWrite(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srpogip, lRecNum, lLineNumber, LOG_ALL);}

void SrcatSet(void) {
    srcat.sessions   = 0;
    srcat.fnum       = -1L;
    srcat.pbFileName = "SRCAT";
    srcat.wOpenFlags = A_READ | A_SHARE;
    srcat.wReportNum = 726;
    srcat.wRecLth    = 64L;
    srcat.wKeyLth    = 0;
    srcat.pBuffer    = &srcatrec;
}
URC SrcatOpen(void) {return direct_open(&srcat, TRUE);}
URC SrcatClose(WORD type) {return close_file(type, &srcat);}
LONG SrcatRead(LONG lRecNum, LONG lLineNumber) {return ReadDirect(&srcat, lRecNum, lLineNumber, LOG_CRITICAL);}
LONG SrcatWrite(LONG lRecNum, LONG lLineNumber) {return WriteDirect(&srcat, lRecNum, lLineNumber, LOG_ALL);}

void SrsdfSet(void) {
    srsdf.sessions   = 0;
    srsdf.fnum       = -1L;
    srsdf.pbFileName = "SRSDF";
    srsdf.wOpenFlags = A_READ | A_SHARE;
    srsdf.wReportNum = 729;
    srsdf.wRecLth    = 34L;
    srsdf.wKeyLth    = 4;
    srsdf.pBuffer    = &srsdfrec;
}
URC SrsdfOpen(void) {return keyed_open(&srsdf, TRUE);}
URC SrsdfClose(WORD type) {return close_file(type, &srsdf);}
LONG SrsdfRead(LONG lLineNumber) {return ReadKeyed(&srsdf, lLineNumber, LOG_CRITICAL);}
LONG SrsdfWrite(LONG lLineNumber) {return WriteKeyed(&srsdf, lLineNumber, LOG_ALL);}

void SrsxfSet(void) {
    srsxf.sessions   = 0;
    srsxf.fnum       = -1L;
    srsxf.pbFileName = "SRSXF";
    srsxf.wOpenFlags = A_READ | A_SHARE;
    srsxf.wReportNum = 730;
    srsxf.wRecLth    = 63L;
    srsxf.wKeyLth    = 6;
    srsxf.pBuffer    = &srsxfrec;
}
URC SrsxfOpen(void) {return keyed_open(&srsxf, TRUE);}
URC SrsxfClose(WORD type) {return close_file(type, &srsxf);}
LONG SrsxfRead(LONG lLineNumber) {return ReadKeyed(&srsxf, lLineNumber, LOG_CRITICAL);}
LONG SrsxfWrite(LONG lLineNumber) {return WriteKeyed(&srsxf, lLineNumber, LOG_ALL);}

