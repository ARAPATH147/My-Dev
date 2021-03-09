#ifndef OSFUNC_H
#define OSFUNC_H

LONG start_background_app(BYTE *prog, BYTE *pparms, BYTE *message);
void background_msg(BYTE *text);
void disp_msg(BYTE *text);
void log_event(UBYTE event, UBYTE sev, BYTE *unique);
void log_event101(LONG rc, WORD file_rep, WORD line_no);
void restrict_file (BYTE *filename);                                    //SDH 09-02-2005
UBYTE establish_start_method();
void * AllocateBuffer ( long BufferSize );
void FreeBuffer ( void * AllocatedBuffer );
void ExitApplication ( void );
void CtlBrkHandler ( void );
long TermEvent ( void );
void ChainApplication ( void ) ;
long DivideExceptEvent ( void );
long ProtExceptEvent ( void );
int get_node_ID(BYTE *node);


#endif

