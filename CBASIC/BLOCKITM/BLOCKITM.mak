BLOCKITM.286: BLOCKITM.INP     \
           BLOCKITM.OBJ   \
           BEMFFUN.OBJ   \
           IDFFUN.OBJ    \
           IEFFUN.OBJ    \
           IRFFUN.OBJ    \
           FUNLIB.L86    \
           ADXACRCL.L86  \
           ADXADMBL.L86  \
           SB286TVL.L86  \
           SB286L.L86
           
#UPDATE#
IDFFUN.obj:	IDFDEC.J86
IEFFUN.obj:	IEFDEC.J86
BLOCKITM.obj:	IDFDEC.J86 IEFDEC.J86 IRFDEC.J86 PSBF01G.J86 PSBF11G.J86 \
		PSBF20G.J86 ADXSERVE.J86 ERRNH.J86 IDFEXT.J86 IEFEXT.J86 \
		IRFEXT.J86 PSBF01E.J86 PSBF11E.J86 PSBF20E.J86 PSBF24E.J86 \
		PSBF30E.J86
#ENDUPDATE#
