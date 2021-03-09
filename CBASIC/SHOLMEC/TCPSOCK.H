// :H1 TCPSOCK: START OF SPECIFICATIONS
//---------------------------------------------------------------------
//
//  Module Name: tcpsock.h
//
//  Description: prototype for socket calls
//
//  Security Classification:
//
//        IBM CONFIDENTIAL (IBM CONFIDENTIAL-RESTRICTED
//          when aggregated with all OCO source modules
//          for this product)
//
//        OCO Source Materials
//
//        Product-id (C) Copyright IBM Corp. 1994
//
//        The source code for this program is not published
//          or otherwise divested of its trade secrets,
//          irrespective of what has been deposited with the
//          U. S. Copyright Office.
//
//
//  Status: 4690 OS Version 1 Release 1
//
//  Initial Author: Linda Meckley
//
//  Function: prototypes for socket calls defined in one place.
//            should be used by all modules that make uses the
//            socket library.
//  Notes:
//
//  Dependencies:
//
//  Restrictions:
//
//  Compiler: Metaware HighC Version 1.7
//
// END-OF-SPECIFICATIONS ----------------------------------------------

#ifndef __tcp_sock_h
#define __tcp_sock_h

// :H1 USER-DEFINED INCLUDE FILES.  SELECTIVITY USED WHERE POSSIBLE.
// END OF USER-DEFINED INCLUDE FILE DEFINITIONS.

// :H1 #DEFINES FOR GLOBAL CONSTANTS.
// END OF GLOBAL CONSTANT DEFINITIONS.

// :H1 GLOBAL TYPE DEFINITIONS.
// END OF GLOBAL TYPE DEFINITIONS.

// :H1 GLOBAL VARIABLE DECLARATIONS.
// END OF GLOBAL VARIABLE DEFINITATIONS.

extern int accept(int, struct sockaddr *, int *);
extern int bind(int, struct sockaddr *, int);
extern int connect(int, struct sockaddr *, int);
extern int listen(int, int);
extern int recv(int, char *, int, int);
extern int recvfrom(int, char *, int, int, struct sockaddr *, int *);
extern int select(int *, int, int, int, long);
extern int send(int, char *, int, int);
extern int sendto(int, char*, int, int, struct sockaddr *, int);
extern int setsockopt(int, int, int, char *, int);
extern int getsockopt(int, int, int, char *, int *);
extern int socket(int, int, int);
extern int ioctl(int, int, char *, int);
extern u_short BSWAP(u_short);
extern void cleanupsockets(void);
extern int soclose(int);
extern int sock_init(void);
extern int port_cancel(int);

extern int tcperrno;

#endif //  __tcp_sock_h
