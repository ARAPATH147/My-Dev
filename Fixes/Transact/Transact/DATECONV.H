//--------------------------------------------------------------------------//
//                                                                          //
//  Program   : dateconv.h                                                  //
//  Desc.     : Gregorian / Julian format date conversion routines headers  //
//  Author    : Steve Wright                                                //
//  Date      : 15th November 1998                                          //
//                                                                          //
//--------------------------------------------------------------------------//
// Version 1.00 - 15/11/1998 - Steve Wright
// Initial implementation
//--------------------------------------------------------------------------//

#ifndef DATECONV
#define DATECONV

#include <flexif.h>

typedef struct B_DATE_struct {
    WORD wDOW;  // 0-6 for the day of week
    WORD wDay;
    WORD wMonth;
    WORD wYear;
} B_DATE;

typedef struct B_TIME_struct {
    WORD wHour;
    WORD wMin;
    WORD wSec;
} B_TIME;

DOUBLE   ConvGJ(LONG day, LONG month, LONG year);
void     ConvJG(DOUBLE jd, LONG *day, LONG *month, LONG *year);
WORD     ConvDOW(DOUBLE jd);

#endif
