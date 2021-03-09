/* IBM COPYRIGHT 1989 */
#ifndef _TIMEVAL
#define _TIMEVAL
struct timeval {
   unsigned long tv_sec;
   unsigned long tv_usec;
};

struct timezone {
  unsigned short tmz;
};
#endif

struct hostent * gethostbyaddr();
struct hostent * gethostent() ;
struct hostent * gethostbyname();

u_long inet_addr();
struct in_addr inet_makeaddr();
u_long inet_network();
char * inet_ntoa();
u_long inet_lnaof();
u_long inet_netof();

struct netent * getnetbyname();
struct netent * getnetent();
struct protoent * getprotobyname();
struct servent * getservbyport();
struct servent * getservbyname();
struct servent * getservent();
struct netent * getnetbyname();
struct netent * getnetbyaddr();

char * rindex();
#define SIGALRM 0
u_long _getlong();
char * p_type();
char * p_class();
char *p_time();
char * hostalias();
