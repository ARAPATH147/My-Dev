// -------------------------------------------------------------------------
// Version 1.0 Paul Bowers                                       24 May 2007
//
// define recall globals exclusivly to TRANS02 and not TRANSACT/RFS
// as the global array stack is full. 
// There is no reason to define these files globally as all I/O to 
// them will be managed by TRANS02 functions
//--------------------------------------------------------------------------

// recall file globals

RCINDX_REC rcindxrec;                                           // PAB 24-5-2007 Recalls
RECOK_REC recokrec;                                             // PAB 24-5-2007 Recalls
RCSPI_REC rcspirec;                                             // PAB 24-5-2007 Recalls
RECALL_REC recallrec;                                           // PAB 24-5-2007 Recalls
IMSTC_REC imstcrec;

int  gRecallFilesAvailable = 0;

