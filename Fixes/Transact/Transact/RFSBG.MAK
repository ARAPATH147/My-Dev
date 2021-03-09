OBJS  = rfsbg.obj rfs.obj adxsrvfn.obj dateconv.obj c4680if.obj dateconv.obj wrap.obj
LOBJS = rfsbg.obj,rfs.obj,adxsrvfn.obj,c4680if.obj,dateconv.obj,
LOBJS2= wrap.obj

INILIBS = l:adxhsisl.l86[s],l:flexlib.l86[s],l:hcbc.l86[s]
ADDSTACK = 2048

#INICFLAGS = -Hpro=rf.pro -Hlist=$*
INICFLAGS = -Hpro=rfdbg.pro -Hlist=$* -g

rfsbg.286 : $(OBJS)

wrap.obj : wrap.c wrap.h
dateconv.obj : dateconv.c
rfs.obj : rfs.c adxsrvfn.h rfsfile.h rfs.h
c4680if.obj : c4680if.c
adxsrvfn.obj : adxsrvfn.c adxsrvst.h rfs.h
rfsbg.obj : rfsbg.c adxsrvfn.h adxsrvst.h rfsfile.h rfs.h wrap.h rfsbg.mak
