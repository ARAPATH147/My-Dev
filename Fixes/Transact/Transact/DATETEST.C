#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <string.h>
#include <flexif.h>
#include "DateConv.h"

main(void)
{

   DOUBLE julian_day;
   LONG d, m, y;
   TIMEDATE now;
   BYTE dow[7][4] = {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

   printf("Leap year : No\n");
   d = 1L;
   m = 3L;
   y = 1900L;
   julian_day = ConvGJ( d, m, y );
   printf( "%02ld/%02ld/%04ld -> %f\n", d, m, y, julian_day );
   julian_day-=1.0;
   d=m=y=0;
   ConvJG( julian_day, &d, &m, &y );
   printf( "%02ld/%02ld/%04ld <- %f, day=%d\n\n",
           d, m, y, julian_day, ConvDOW(julian_day) );

   printf("Leap year : Yes\n");
   d = 1L;
   m = 3L;
   y = 2000L;
   julian_day = ConvGJ( d, m, y );
   printf( "%02ld/%02ld/%04ld -> %f\n", d, m, y, julian_day );
   julian_day-=1.0;
   d=m=y=0L;
   ConvJG( julian_day, &d, &m, &y );
   printf( "%02ld/%02ld/%04ld <- %f, day=%d\n\n",
           d, m, y, julian_day, ConvDOW(julian_day) );

   printf("Date increment / decrement\n");
   d = 29L;
   m = 3L;
   y = 1967L;
   julian_day = ConvGJ( d, m, y );
   printf( "%02ld/%02ld/%04ld -> %f\n", d, m, y, julian_day );
   julian_day+=1000000.0;
   d=m=y=0L;
   ConvJG( julian_day, &d, &m, &y );
   printf( "%02ld/%02ld/%04ld <- %f, day=%d\n\n",
           d, m, y, julian_day, ConvDOW(julian_day) );

   julian_day = ConvGJ( d, m, y );
   printf( "%02ld/%02ld/%04ld -> %f\n", d, m, y, julian_day );
   julian_day-=1000000.0;
   d=m=y=0L;
   ConvJG( julian_day, &d, &m, &y );
   printf( "%02ld/%02ld/%04ld <- %f, day=%d\n\n",
           d, m, y, julian_day, ConvDOW(julian_day) );


   printf("Today\n");
   s_get( T_TD, 0L, (void *)&now, TIMESIZE );
   d = now.td_day;
   m = now.td_month;
   y = now.td_year;
   julian_day = ConvGJ( d, m, y );
   printf( "%02ld/%02ld/%04ld -> %f\n", d, m, y, julian_day );
   d=m=y=0L;
   ConvJG( julian_day, &d, &m, &y );
   printf( "%02ld/%02ld/%04ld <- %f, day=%s\n\n",
           d, m, y, julian_day, dow[ConvDOW(julian_day)] );

}