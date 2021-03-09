#######################################################################
# POGMAP             RANJITH GOPALANKUTTY                15-01-2019   #
# POGMAP is a quick python program to compare SRPOG and SRMAP files   # 
# and report missing planners from SRMAP if the activation date       #
# is in the past.                                                     # 
#######################################################################

import sys 
import time
import datetime
import os
import glob 
from   pathlib import Path 
import re
import binascii
import binhex
import decimal
import threading
import struct
from   functools import partial
from   multiprocessing import Pool
from   multiprocessing.dummy import Pool as ThreadPool  
 
StringValue = ""
StoreList = [] 
SRPOGFileName = ""
SRMAPFileName = ""
Fixedpath = ""
FullPath = ""
SRMAPList = []

# Class contains few functions to process a keyed record
class PureRead:         

    def FormatRecord(Field):
        i = 0
        global StringValue
        StringValue = ""
        while i < len(Field):
            StringValue = StringValue + format(Field[i],'02x')
            i = i + 1
        return StringValue

    def SRMAPRead():
        global SRMAPFileName
        global SRPOGFileName
        global FixedPath         
        srmapcount = 0       
        FixedPath = "C:/SRP06/"
        FullPath = FixedPath + SRMAPFileName
        try:
            
            f = open(FullPath,"rb")      
            MAPNumberOfSectors = ((os.stat(FullPath).st_size) / 512 ) - 1
            Exist = False
            while (srmapcount <= MAPNumberOfSectors):
                SRMAPSector = f.read(512)[4:512]               
                srmapcount = srmapcount + 1
                #  We dont want the first sector as it tells the file attributes
                if srmapcount > 1:
                        MAPIterator = int(508 / 18)
                        MAPRecordPerSector = 0
                        while MAPRecordPerSector < MAPIterator:
                            MAPR = MAPRecordPerSector * 18
                            MAPS = MAPR + 18  
                            MAPRecord = SRMAPSector[MAPR:MAPS]
                            SRMAPDB   = str(struct.unpack("<i",MAPRecord[0:4])).strip('(,)')
                            if SRMAPDB != '0':
                                SRMAPList.append(SRMAPDB)  
                            MAPRecordPerSector = MAPRecordPerSector + 1
            
        except Exception as e:
            SRMAPErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(SRMAPErrorString)
            POGMAP.writelines(SRMAPErrorString)

# Below function will help to read the keyed file in sector by sector
# then each sector can be processed further            
    def SRPOGRead(StoreList):
        global SRMAPFileName
        global SRPOGFileName
        global FixedPath
        global Missing        
        CurrentDate = datetime.datetime.now().strftime("%Y%m%d")
            
 
        for i in range(len(StoreList)):
            SRPOGFileName = StoreList[i]     # Getting the SRPOG file value from first 13 bytes
            SRMAPFileName = StoreList[i] 
            SRPOGFileName = StoreList[i][:13]     # Getting the SRPOG file value from first 13 bytes
            SRMAPFileName = StoreList[i][-13:]    # Getting the SRMAP file value from second 13 byte        
            FixedPath = "C:/SRP06/"
            FullPath = FixedPath + SRPOGFileName
            PureRead.SRMAPRead()
            try:
                f = open(FullPath,"rb") 
                POGMAP.writelines("Now Checking for " + SRPOGFileName[5:9] + "\n")
                print("Now Checking for " + SRPOGFileName[5:9] + "\n")
                POGNumberOfSectors = ((os.stat(FullPath).st_size) / 512 ) - 1
                found = False
                srpogcount = 0    
                while srpogcount <= POGNumberOfSectors:
                    SRPOGSector = f.read(512)[4:512]               
                    srpogcount = srpogcount + 1
                    #  We dont want the first sector as it tells the file attributes
                    if srpogcount > 1:                    
                        POGIterator = int(508 / 101)
                        POGRecordPerSector = 0
                        while POGRecordPerSector < POGIterator:
                            R = POGRecordPerSector * 101
                            S = R + 101  
                            Record = SRPOGSector[R:S]
                            # Below line of code tells how to process little ending value from 4690
                            SRPOGPOGDB            =  str(struct.unpack("<i",Record[0:4])).strip('(,)')
                            if SRPOGPOGDB != '0':
                                # Below line of code tells how to process little ending value from 4690
                                SRPOGPOGID            =  str(struct.unpack("<i",Record[4:8])).strip('(,)')
                                SRPOGACTDATE          =  Record[8:12]                                      
                                SRPOGDEACTDATE        =  Record[12:16]                                                   
                                PureRead.FormatRecord(SRPOGACTDATE)
                                SRPOGACTDATE     =  StringValue
                                PureRead.FormatRecord(SRPOGDEACTDATE)
                                SRPOGDEACTDATE   =   StringValue                    
                                POGRecordPerSector = POGRecordPerSector + 1 
                                # Now we need to find if this planner present in SRMAP 
                                # file and based on that further processing has to be done
                                if (CurrentDate > SRPOGACTDATE) & (CurrentDate < SRPOGDEACTDATE):
                                    try:
                                         b=SRMAPList.index(SRPOGPOGDB)
                                    except ValueError:
                                         POGMAP.writelines("Missing " + SRPOGPOGDB + " in SRMAP with activation date "  + str(SRPOGACTDATE)  + " and deactivation date " + SRPOGDEACTDATE + "\n")
                                    else:
                                        pass                                    
                                else:
                                    pass
                            else:
                                # As soon finding a null record , stop reading that sector and move on
                                # This will make the keyed reader much faster
                                POGRecordPerSector = POGIterator   
                #Need to clear the list for next store so that list is tiny and sorting is faster
                SRMAPList.clear()              
            except Exception as e:
                POGErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
                print(ErrorString)
                POGMAP.writelines(POGErrorString)       
        
         
# Below function is to just add the pair of SRPOG and SRMAP file for comparison purpose
# Not used in this code as its just to show how to process a keyed record
    def ReadFileList():
        for root, dirs, files in os.walk("C:/SRP06/"):  
            for filename in files:
                if filename[-3:] == '002':
                    pass
                elif filename[-3:] == '001':
                    # Now check if it has got matching SRMAP
                    MapFileName = filename[:9] + ".002"
                    if MapFileName in files:                        
                        StoreString = filename  + MapFileName
                        StoreList.append(StoreString)
                    else:
                        pass
                else:
                    pass               
        PureRead.SRPOGRead(StoreList)
        
#MainLine code starts from here , then class call begins
TotalPlanner = 0
Missing = 0
start = time.time()
POGMAP = open("C:/SRP06/FutureExpired.txt","w+")

PureRead.ReadFileList() 
end = time.time()
TotalTime = end - start
POGMAP.writelines(str(TotalTime))
POGMAP.close()