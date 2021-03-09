#if ! _time_defs_included
#define Local_time_zone 8
#if __HIGHC__
#define CLK_TCK 100		/* obsolescent. */
#endif
#define CLOCKS_PER_SEC 100	/* new ANSI name. */

/* Two bytes for the year, one each for month, day, hour, minute, and second. */
#define time_t double
#define clock_t long

struct tm {
	   int tm_sec;	  /*seconds after the minute- 0..59*/
	   int tm_min;	  /*minutes after the hour- 0..59*/
	   int tm_hour;   /*hours since midnight- 0..23*/
	   int tm_mday;   /*day of the month- 1..31*/
	   int tm_mon;	  /*month of the year- 0..11*/
	   int tm_year;   /*years since 1900*/
	   int tm_wday;   /*days since Sunday- 0..6*/
	   int tm_yday;   /*day of the year- 0..365*/
	   int tm_isdst;  /*daylight savings time- boolean (0..1)*/
	   };
extern clock_t clock(void); /* Time of day in 100ths of a second. */
extern time_t time(time_t *timer);
extern char *asctime(const struct tm *timeptr);
extern char *ctime(const time_t *timer);
extern double difftime(time_t time2, time_t time1);
extern struct tm *gmtime(const time_t *timer);
extern struct tm *localtime(const time_t *timer);
extern int localtimezone();
#define _time_defs_included 1
#endif
