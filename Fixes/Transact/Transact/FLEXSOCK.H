/* 
prepartory defines so that we can use the IBM 
supplied header files 
*/
typedef unsigned long u_long;
typedef unsigned short u_short;
typedef unsigned short ushort;
typedef unsigned int u_int;
typedef unsigned char u_char;
typedef long off_t;
typedef char * caddr_t;

typedef char *caddr_t;
typedef long daddr_t;
typedef unsigned (*FUNC)();

#define NIL ((char *) 0)


#include <errno.h>
#include <tcpip/in.h>
#include <tcpip/ioctl.h>
#include <tcpip/socket.h>
#include <tcpip/nerrno.h>
#include <tcpip/tcpsock.h>


