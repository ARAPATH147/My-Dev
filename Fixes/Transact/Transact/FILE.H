#if !defined FILE_H
#define FILE_H 1

// 4690 file functions
typedef struct C4690_File_Control {
   LONG fnum;
   UBYTE present;
   UBYTE sessions;
   BYTE* pbFileName;                            // SDH 26-11-04 OSSR WAN
   WORD  wOpenFlags;                            // SDH 26-11-04 OSSR WAN
   WORD  wReportNum;                            // SDH 26-11-04 OSSR WAN
   void* pBuffer;                               // SDH 26-11-04 OSSR WAN
   WORD  wRecLth;                               // SDH 26-11-04 OSSR WAN
   WORD  wKeyLth;                               // SDH 26-11-04 OSSR WAN
} FILE_CTRL;

#endif

