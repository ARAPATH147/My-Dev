//--------------------------------------------------------------------------//
//                                                                          //
//  Program   : dateconv.c                                                  //
//  Desc.     : Gregorian / Julian format date conversion routines          //
//  Author    : Steve Wright                                                //
//  Date      : 15th November 1998                                          //
//                                                                          //
//--------------------------------------------------------------------------//
// Version 1.00 - 15/11/1998 - Steve Wright
// Initial implementation
//--------------------------------------------------------------------------//

#include "transact.h" //SDH 19-May-2006

#include <stdio.h>
#include <math.h>
#include <flexif.h>
#include "DateConv.h"

// Convert Gregorian date to Julian day (MJD)
DOUBLE ConvGJ(LONG d, LONG m, LONG y) {

    LONG f, z;
    z = y + (LONG)floor((m - 14L) / 12L);
    f = (LONG)floor((979L * (m - 12L * (LONG)floor((m - 14L) / 12L)) - 2918L) / 32L);
    return d + f + 365.0 * z + (z / 4L) - (z / 100L) + (z / 400L) + 1721118.5;

}

// Convert Julian day (MJD) to Gregorian date
void ConvJG(DOUBLE jd, LONG *day, LONG *month, LONG *year)
{

   LONG d, m, y, a, b, z, c, h;

   z = (LONG)(jd - 1721118.5);
   h = 100 * z - 25;
   a = (LONG)floor(h / 3652425L);
   b = a - (LONG)floor(a / 4);
   y = (LONG)floor((100 * b + h) / 36525L);
   c = b + z - 365 * y - (LONG)floor(y / 4);
   m = (LONG)floor((5 * c + 456) / 153);
   d = c - (LONG)floor((153 * m - 457) / 5);
   if (m > 12L) {
      y += 1L;
      m -= 12L;
   }

   *day = d;
   *month = m;
   *year = y;

}

// Return day of week (0=Sunday, 6=Saturday) given Julian day (MJD)
WORD ConvDOW(DOUBLE jd)
{
   LONG temp;
   temp = (LONG)floor(jd + 1.5) % 7L;
   return (WORD)temp;
}
