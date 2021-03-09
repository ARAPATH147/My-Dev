\********************************************************************
\***      Space and Range Planner Common Functions  (SRPFUN)
\***      Version A           Neil Bennett          24.08.2006
\***
\....................................................................
\***
\***    GET.CATID Construct a 4 byte integer Category ID from 3
\***              hierarchical (I4) keys supplied.
\***
\********************************************************************

   FUNCTION GET.CATID%(lev1%,lev2%,lev3%) PUBLIC

    INTEGER*4 GET.CATID%
    INTEGER*4 lev1%, lev2%, lev3%
    STRING    work$

    IF (lev1% < 0) OR (lev1% >   20)                                \
    OR (lev2% < 0) OR (lev2% > 9999)                                \
    OR (lev3% < 0) OR (lev3% > 9999) THEN BEGIN
       work$ = "-1"
    ENDIF ELSE BEGIN
       work$ = RIGHT$("00"   + STR$(lev1%), 2)                      \
             + RIGHT$("0000" + STR$(lev2%), 4)                      \
             + RIGHT$("0000" + STR$(lev3%), 4)
    ENDIF

    GET.CATID% = VAL(work$)

   END FUNCTION

\********************************************************************