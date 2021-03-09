\*****************************************************************************
\***                                                                         *
\***   4680 BASIC LIST                                                       *
\***                                                                         *
\***   Copyright (c) 2010 Stuart Highley                                     *
\***   All rights reserved                                                   *
\***                                                                         *
\***   Provides a big string array of up to about 256,000,000 elements.      *
\***   Currently only one list per program though.                           *
\***                                                                         *
\***                                                                         *
\***                                                                         *
\***   Reference : LISTLIB.BAS                                               *
\***                                                                         *
\***   Version A      Stuart Highley       28th October 2010                 *
\***                                                                         *
\***   Version B      Tittoo Thomas        05th August  2011                 *
\***      Cloned LISTLIB to support INTEGER*1, INTEGER*2 and INTEGER*4       *
\***                                                                         *
\*****************************************************************************

STRING TEMP$(1)
INTEGER*4 PTRS%(1)
INTEGER*4 CURRENT.PTRS%
INTEGER*4 PTR.TO.TEMP%
INTEGER*4 ELEMENTS.PER.ARRAY%

%INCLUDE I1LISTD.J86                                                     !BTT
%INCLUDE I2LISTD.J86                                                     !BTT
%INCLUDE I4LISTD.J86                                                     !BTT

%INCLUDE I1LISTF.J86                                                     !BTT
%INCLUDE I2LISTF.J86                                                     !BTT
%INCLUDE I4LISTF.J86                                                     !BTT

\*****************************************************************************
\***                                                                         *
\***    CREATE.NEW.ARRAY                                                     *
\***    Resets TEMP$ so BASIC thinks there's no array allocated, DIMs a      *
\***    new array, then saves the address of the array in an array of        *
\***    pointers.                                                            *
\***                                                                         *
\*****************************************************************************

SUB CREATE.NEW.ARRAY(ELE%)

    INTEGER*4 ELE%
    INTEGER*4 TEMP%
    INTEGER*4 POKE.ADR%

    !Fool BASIC into thinking it hasn't allocated the TEMP$ array
    POKE PTR.TO.TEMP%, 0
    POKE PTR.TO.TEMP% + 1, 0
    POKE PTR.TO.TEMP% + 2, 0
    POKE PTR.TO.TEMP% + 3, 0

    !Allocate an array
    DIM TEMP$(ELE%)

    !Save away the address of the array
    POKE.ADR% = VARPTR(TEMP%)
    POKE POKE.ADR%, PEEK(PTR.TO.TEMP%)
    POKE POKE.ADR% + 1, PEEK(PTR.TO.TEMP% + 1)
    POKE POKE.ADR% + 2, PEEK(PTR.TO.TEMP% + 2)
    POKE POKE.ADR% + 3, PEEK(PTR.TO.TEMP% + 3)
    PTRS%(CURRENT.PTRS%) = TEMP%
    CURRENT.PTRS% = CURRENT.PTRS% + 1

END SUB

\*****************************************************************************
\***                                                                         *
\***    LIST.DIM                                                             *
\***                                                                         *
\***    Dimension the list up to 256,000,000 elements (16,000 * 16,000)      *
\***    if you have the RAM available!                                       *
\***                                                                         *
\*****************************************************************************

FUNCTION LIST.DIM(NEW.ELEMENTS%) PUBLIC

    INTEGER*4 A%
    INTEGER*4 NEW.ELEMENTS%
    INTEGER*4 ARRAY%
    INTEGER*4 PEEK.ADR%

    !Set up constants
    PTR.TO.TEMP% = VARPTR(TEMP$)
    ELEMENTS.PER.ARRAY% = 16000

    !Clear out any existing arrays
    FOR A% = 0 TO (CURRENT.PTRS% - 1)
        ARRAY% = PTRS%(A%)
        IF ARRAY% <> 0 THEN BEGIN
            PEEK.ADR% = VARPTR(ARRAY%)
            POKE PTR.TO.TEMP%, PEEK(PEEK.ADR%)
            POKE PTR.TO.TEMP% + 1, PEEK(PEEK.ADR% + 1)
            POKE PTR.TO.TEMP% + 2, PEEK(PEEK.ADR% + 2)
            POKE PTR.TO.TEMP% + 3, PEEK(PEEK.ADR% + 3)
            DIM TEMP$(0)
        ENDIF
    NEXT A%

    !Dimension an integer array to hold the pointers to the string arrays
    CURRENT.PTRS% = 0
    DIM PTRS%(NEW.ELEMENTS% / ELEMENTS.PER.ARRAY%)

    !Create as many string arrays as required
    WHILE NEW.ELEMENTS% > ELEMENTS.PER.ARRAY%
        CALL CREATE.NEW.ARRAY(ELEMENTS.PER.ARRAY%)
        NEW.ELEMENTS% = NEW.ELEMENTS% - ELEMENTS.PER.ARRAY%
    WEND
    CALL CREATE.NEW.ARRAY(NEW.ELEMENTS%)

END FUNCTION

\*****************************************************************************
\***                                                                         *
\***    LIST.SET                                                             *
\***                                                                         *
\***    Sets an element in the list to a given string.                       *
\***                                                                         *
\*****************************************************************************

SUB LIST.SET(ELEMENT%, DATA$) PUBLIC

    INTEGER*4 PEEK.ADR%
    INTEGER*4 ARRAY%
    INTEGER*4 ELEMENT%
    STRING DATA$

    !Set up TEMP$ to point to the right array
    ARRAY% = PTRS%(ELEMENT% / ELEMENTS.PER.ARRAY%)
    PEEK.ADR% = VARPTR(ARRAY%)
    POKE PTR.TO.TEMP%, PEEK(PEEK.ADR%)
    POKE PTR.TO.TEMP% + 1, PEEK(PEEK.ADR% + 1)
    POKE PTR.TO.TEMP% + 2, PEEK(PEEK.ADR% + 2)
    POKE PTR.TO.TEMP% + 3, PEEK(PEEK.ADR% + 3)

    !Update the element in the array
    TEMP$(MOD(ELEMENT%, ELEMENTS.PER.ARRAY%)) = DATA$

END SUB

\*****************************************************************************
\***                                                                         *
\***    LIST.GET                                                             *
\***                                                                         *
\***    Gets the string from a given element in the list.                    *
\***                                                                         *
\*****************************************************************************

SUB LIST.GET(ELEMENT%, RETURNED.DATA$) PUBLIC

    INTEGER*4 PEEK.ADR%
    INTEGER*4 ARRAY%
    INTEGER*4 ELEMENT%
    STRING RETURNED.DATA$

    !Set up TEMP$ to point to the right array
    ARRAY% = PTRS%(ELEMENT% / ELEMENTS.PER.ARRAY%)
    PEEK.ADR% = VARPTR(ARRAY%)
    POKE PTR.TO.TEMP%, PEEK(PEEK.ADR%)
    POKE PTR.TO.TEMP% + 1, PEEK(PEEK.ADR% + 1)
    POKE PTR.TO.TEMP% + 2, PEEK(PEEK.ADR% + 2)
    POKE PTR.TO.TEMP% + 3, PEEK(PEEK.ADR% + 3)

    !Get the element from the array
    RETURNED.DATA$ = TEMP$(MOD(ELEMENT%, ELEMENTS.PER.ARRAY%))

END SUB

