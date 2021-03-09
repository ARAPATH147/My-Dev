#ifndef WRAP_H
#define WRAP_H

#define TEMP_BUFF_SZ 64

typedef struct{
   BYTE *rbuf;       // pointer to input text buffer
   WORD rbuf_sz;     // size of input text buffer
   WORD rbuf_os;     // input text buffer - next byte to process
   BYTE *wbuf;       // pointer to output text buffer
   WORD wbuf_sz;     // size of output text buffer
   WORD wbuf_os;     // output text buffer - next free byte
   WORD wbuf_ll;     // output buffer line length (wrap at this character)
   BYTE *tbuf;       // temporary word buffer
   WORD tbuf_os;     // temp word buffer - next free byte
} FTEXT;

BOOLEAN get_word( FTEXT *ftb, WORD *word_size );
WORD format_text( BYTE *rbuf, WORD rbuf_sz,
                  BYTE *wbuf, WORD wbuf_sz,
                  WORD wbuf_ll );
void translate_text( BYTE *buf, WORD buf_sz );

#endif
