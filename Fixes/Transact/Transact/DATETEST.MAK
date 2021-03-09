OBJS=dateconv.obj datetest.obj
LOBJS=dateconv.obj,datetest.obj

datetest.286 : $(OBJS)

dateconv.obj : dateconv.c dateconv.h
datetest.obj : datetest.c dateconv.h makefile
