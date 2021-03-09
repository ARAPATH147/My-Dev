OBJS  = rfsstat.obj
LOBJS = rfsstat.obj

INILIBS = l:flexlib.l86[s],l:hcbc.l86[s]
ADDSTACK = 2048

#INICFLAGS = -Hpro=rf.pro -Hlist=$*
INICFLAGS = -Hpro=rfdbg.pro -Hlist=$* -g

rfsstat.286 : $(OBJS)

rfsstat.obj : rfsstat.c rfsfile.h rfs.h rfsstat.mak
