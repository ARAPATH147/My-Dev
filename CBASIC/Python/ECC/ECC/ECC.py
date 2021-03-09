import sys
import time
import datetime
import re
import csv
import os
import glob

#Global Declarations
#String
PosDate = '17/04/2018'
DqDate = '180417'

#List
PosRecords = list()
DqRecords = list()

#Array
PosTotals = [0,0,0,0,0,0,0,0,0,0,0]
DqTotals = [0,0,0]

#Functions
def left(s, amount):
    return s[:amount]

def right(s, amount):
    return s[-amount:]

def mid(s, offset, amount):
    return s[offset:offset+amount]

def CnvInt(s):
    try:
        ret = int(s)
    except ValueError:
        ret = float(s)
    return ret

def ProcessPosRaw(PosRaw):

    for PosRecord in PosRaw:

        if PosRecord in ['\n', '\r\n']:
            pass
        elif PosRecord.startswith('Retail - Transaction Detail Report'):
            pass
        else:
            RecordSplit = PosRecord.split(",")
            if PosDate == left(RecordSplit[23],10):
                PosRecords.append(PosRecord)
                PosTotals[0] += 1

                if RecordSplit[22].startswith('Transactions routed to WorldPaySnk'):
                    Route = 3
                elif RecordSplit[22].startswith('Transactions routed to AmexSnk'):
                    Route = 5
                elif RecordSplit[22].startswith('Transactions routed to ParkSnk'):
                    Route = 7
                else:
                    Route = 9

                if CnvInt(RecordSplit[32]) >= 0:
                    PosTotals[1] += CnvInt(RecordSplit[32].replace(".",""))
                    PosTotals[Route] += CnvInt(RecordSplit[32].replace(".",""))
                else:
                    PosTotals[2] += CnvInt(RecordSplit[32].replace(".",""))
                    PosTotals[Route + 1] += CnvInt(RecordSplit[32].replace(".",""))

def ProcessDqRaw(DqRaw):

    for DqRecord in DqRaw:
        CleanDqRecord = DqRecord[1:-3]
        DqSplitRecord = CleanDqRecord.split("&CRTLF;")
        SplitSubRecord = DqSplitRecord[1].split(",")
        if SplitSubRecord[0] == '0':
            if SplitSubRecord[8] == DqDate:
                if SplitSubRecord[3] == '6':
                    DqRecords.append(CleanDqRecord)
                    for i in range(len(DqSplitRecord)):
                        SplitSubRecord = DqSplitRecord[i].split(",")
                        if SplitSubRecord[0] == '61':
                            DqTotals[0] += 1
                            if SplitSubRecord[4] == '':
                                DqTotals[1] += CnvInt(SplitSubRecord[3])
                            else:
                                DqTotals[2] += CnvInt(SplitSubRecord[3]) * -1

def PosVsDq():
    for PosRecord in PosRecords:
        RecordSplit = PosRecord.split(",")

        PosStore   = left(RecordSplit[24],4)
        PosTill    = right(RecordSplit[24],4)
        PosTranNum = right(RecordSplit[25],4)
        PosEnPan   = RecordSplit[26]
        PosAmount  = CnvInt(RecordSplit[32].replace(".",""))

        Match = 0

        for DqRecord in DqRecords:
            DqSplitRecord = DqRecord.split("&CRTLF;")
            SplitSubRecord = DqSplitRecord[1].split(",")
            Dq0Split =DqSplitRecord[0].split(",")

            DqStore = Dq0Split[2]
            DqTill = '{:04d}'.format(CnvInt(SplitSubRecord[2]))
            DqTransNum = '{:04d}'.format(CnvInt(SplitSubRecord[1]))

            for i in range(len(DqSplitRecord)):
                SplitSubRecord = DqSplitRecord[i].split(",")
                if SplitSubRecord[0] == '61':

                    DqEnPan = SplitSubRecord[5]

                    if SplitSubRecord[4] == '':
                        DqAmount = CnvInt(SplitSubRecord[3])
                    else:
                        DqAmount = CnvInt(SplitSubRecord[3]) * -1

                    if (PosStore == DqStore) and (PosTill == DqTill) and (PosTranNum == DqTransNum) and (PosEnPan == DqEnPan) and (PosAmount == DqAmount):
                        Match = -1


        if Match == 0:
            print('Pos :',PosStore, PosTill, PosTranNum, PosEnPan, PosAmount)
        else:
            pass


def DqVsPos():

    for DqRecord in DqRecords:
        DqSplitRecord = DqRecord.split("&CRTLF;")
        SplitSubRecord = DqSplitRecord[1].split(",")
        Dq0Split =DqSplitRecord[0].split(",")

        DqStore = Dq0Split[2]
        DqTill = '{:04d}'.format(CnvInt(SplitSubRecord[2]))
        DqTransNum = '{:04d}'.format(CnvInt(SplitSubRecord[1]))
        DqTime = SplitSubRecord[9]

        Count61 = 0

        for i in range(len(DqSplitRecord)):
            SplitSubRecord = DqSplitRecord[i].split(",")
            if SplitSubRecord[0] == '61':
                Count61 += 1
                DqEnPan = SplitSubRecord[5]

                if SplitSubRecord[4] == '':
                    DqAmount = CnvInt(SplitSubRecord[3])
                else:
                    DqAmount = CnvInt(SplitSubRecord[3]) * -1

        Match = 0

        for PosRecord in PosRecords:
            RecordSplit = PosRecord.split(",")

            PosStore   = left(RecordSplit[24],4)
            PosTill    = right(RecordSplit[24],4)
            PosTranNum = right(RecordSplit[25],4)
            PosEnPan   = RecordSplit[26]
            PosAmount  = CnvInt(RecordSplit[32].replace(".",""))

            if (DqStore == PosStore) and (DqTill == PosTill) and (DqTransNum == PosTranNum) and (DqEnPan == PosEnPan) and (DqAmount == PosAmount):
                Match = -1

        if Match == 0:
            print('DQ :',DqTime,DqStore,DqTill,DqTransNum,DqEnPan,DqAmount)
        else:
            pass

        if Count61 > 1:
            print ('Multiple 61 - Not handled.', DqRecord)


#Main Code
PosFile = open("C:/Users/ranjith.gopalankutty/Desktop/0158/test.TXT","r")
DqFile = open("C:/Users/ranjith.gopalankutty/Desktop/0158/0158.TXT","r")
print("test")


PosRaw = PosFile.readlines()
DqRaw = DqFile.readlines()

PosFile.close()
DqFile.close()

ProcessPosRaw(PosRaw)
ProcessDqRaw(DqRaw)

print ('Pos totals:', PosTotals)
print ('Dq2 totals:', DqTotals)

if PosTotals[0] > DqTotals[0]:
    PosVsDq()
elif DqTotals[0] > PosTotals[0]:
    DqVsPos()
else:
    pass

print ('Fin')

