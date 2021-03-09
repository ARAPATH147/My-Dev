transact.286: transact.inp osfunc.obj   sockserv.obj \
              trxutil.obj  trxdeal.obj  main.obj     trxbase.obj  transact.obj \
              trxpog.obj   trxGIA.obj   GIAfiles.obj DALfiles.obj trxDAL.obj   \
              af.obj       ccfiles.obj  iudf.obj     trxbase2.obj   \
              idf.obj      irf.obj      isf.obj      nvurl.obj    pgf.obj      prtctl.obj   \
              prtlist.obj  rfhist.obj   srfiles.obj  bcsmf.obj    rfscf.obj    invok.obj    \
              clfiles.obj  trxuod.obj   trxrcall.obj phf.obj      seldesc.obj  trxfile.obj  \
              affcf.obj    podok.obj    podlog.obj   ealterms.obj MBfiles.obj \
              adxhsisl.l86 flexlib.l86 hcbc.l86

#UPDATE#
trxgia.obj:     transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxbase.h \
                trxutil.h osfunc.h sockserv.h irf.h idf.h rfscf.h trxGIA.h \
                GIAfiles.h
giafiles.obj:   transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
                GIAfiles.h
mbfiles.obj:    transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
                MBfiles.h
iduf.obj:       transact.h rfs.h rfsfile.h rfglobal.h trxfile.h iduf.h
dalfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h \
		DALfiles.h osfunc.h
trxdal.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxbase.h \
		trxutil.h osfunc.h sockserv.h irf.h idf.h rfscf.h trxDAL.h \
		DALfiles.h
osfunc.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h sockserv.h \
		osfunc.h trxutil.h rfscf.h
sockserv.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		sockserv.h trxutil.h MBfiles.h
trxutil.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		trxutil.h sockserv.h irf.h idf.h srfiles.h isf.h rfscf.h \
		rfhist.h pgf.h af.h
trxdeal.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxbase.h \
		trxutil.h osfunc.h sockserv.h prtctl.h srfiles.h idf.h irf.h \
		rfscf.h ccfiles.h invok.h
main.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		trxutil.h sockserv.h prtctl.h prtlist.h srfiles.h idf.h irf.h \
		isf.h rfscf.h ccfiles.h invok.h ealterms.h af.h affcf.h \
		bcsmf.h clfiles.h iudf.h MBfiles.h nvurl.h pgf.h phf.h \
		rfhist.h seldesc.h GIAfiles.h podlog.h podok.h DALfiles.h
trxbase.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		prtctl.h prtlist.h srfiles.h idf.h irf.h isf.h rfhist.h \
		bcsmf.h pgf.h invok.h rfscf.h af.h ccfiles.h clfiles.h \
		trxrcall.h seldesc.h phf.h iudf.h trxutil.h podlog.h podok.h
transact.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h \
		srfiles.h osfunc.h trxutil.h trxpog.h trxbase.h idf.h isf.h \
		rfhist.h irf.h rfscf.h invok.h af.h affcf.h trxuod.h \
		trxdeal.h trxrcall.h sockserv.h clfiles.h trxGIA.h podok.h \
		podlog.h trxDAL.h transmsg.h trxbase2.h
trxpog.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxutil.h \
		osfunc.h srfiles.h rfscf.h idf.h trxbase.h
trxGIA.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxbase.h \
		trxutil.h osfunc.h sockserv.h irf.h idf.h rfscf.h trxGIA.h \
		GIAfiles.h
GIAfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		GIAfiles.h
DALfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h \
		DALfiles.h osfunc.h
trxDAL.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxbase.h \
		trxutil.h osfunc.h sockserv.h irf.h idf.h rfscf.h trxDAL.h \
		DALfiles.h
af.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h af.h
ccfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		ccfiles.h
iudf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h iudf.h
trxbase2.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		prtctl.h prtlist.h rfscf.h trxutil.h
idf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h idf.h
irf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxutil.h \
		irf.h
isf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h isf.h
nvurl.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h nvurl.h
pgf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h pgf.h
prtctl.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h prtctl.h
prtlist.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h prtlist.h
rfhist.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h rfhist.h \
		rfscf.h
srfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h srfiles.h
bcsmf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h bcsmf.h
rfscf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		trxutil.h rfscf.h
invok.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h invok.h
clfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h clfiles.h
trxuod.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h irf.h \
		idf.h osfunc.h rfscf.h trxutil.h ccfiles.h trxbase.h
trxrcall.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h \
		trxbase.h trxutil.h osfunc.h sockserv.h prtctl.h srfiles.h \
		idf.h irf.h rfscf.h ccfiles.h invok.h
phf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h phf.h
seldesc.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h seldesc.h
trxfile.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		srfiles.h sockserv.h ccfiles.h bcsmf.h pgf.h idf.h isf.h \
		irf.h prtlist.h prtctl.h invok.h rfhist.h clfiles.h trxutil.h \
		af.h rfscf.h GIAfiles.h podok.h podlog.h
affcf.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxutil.h \
		affcf.h ealterms.h osfunc.h
podok.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxutil.h \
		podok.h
podlog.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h trxutil.h \
		podlog.h
ealterms.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h \
		ealterms.h
MBfiles.obj:	transact.h rfs.h rfsfile.h rfglobal.h trxfile.h osfunc.h \
		MBfiles.h
#ENDUPDATE#

