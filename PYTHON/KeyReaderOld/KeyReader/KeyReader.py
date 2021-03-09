#######################################################################
#  KeyReader       Ranjith Gopalankutty             1-December-2018   #
#  This program is written to read any keyed file from 4690           #                                                    
#  controller in windows. There are vaious ways to use the program    #
#                                                                     #
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
import pandas as pd
import numpy
from   functools import partial
from   multiprocessing import Pool
from   multiprocessing.dummy import Pool as ThreadPool 

Missing = 0
TotalPlanner = 0
StoreList = [] 
DataList  = []
class PureRead:        
                         
    def SRPOGRead(StoreNumber):  
        global Missing
        global TotalPlanner
        FixedPath = "c:/srpog/"
        FileName = "0000" + str(StoreNumber)
        FullPath = FixedPath + "STORE" + FileName[-4:]
        try:
            f = open(FullPath,"rb")
            print("Now Checking for " + str(StoreNumber) + "\n")
            POGNumberOfSectors = ((os.stat(FullPath).st_size) / 512 ) - 1                 
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
                            StoreList.append(str(SRPOGPOGDB)) 
                            POGRecordPerSector = POGRecordPerSector + 1
                        else:
                         # As soon finding a null record , stop reading that sector and move on
                         # This will make the keyed reader much faster
                            POGRecordPerSector = POGIterator                        
                 
            #Now check for the match and report accordingly
            for k in range(len(DataList)):                
                try:
                    b=StoreList.index(DataList[k])
                except ValueError:
                    Result.writelines("Store  " + str(FileName[-4:]) + " has missing planner " +  DataList[k]  + "\n")
                    Missing = Missing + 1
                else:
                    pass  
           # Dont we need to clear both list??
            StoreList.clear()
            DataList.clear()
        except Exception as e:
            POGErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(POGErrorString)
            #Result.writelines(POGErrorString)
            # We need to clear the datalist in case of file mising as well
            DataList.clear()
  
    # Process the daily excel file        
    def ReadInput():

        global StoreList
        global TotalPlanner
        ExcelFile = "c:/temp/planner.xlsx"
        print("Reading Excel file ")
        PlannerDetails = pd.read_excel(ExcelFile)  
        i = 0
        TotalPlanner = len(PlannerDetails)
        StoreNumber = PlannerDetails.StoreNo[i]
        while i < len(PlannerDetails): 
            
            if PlannerDetails.StoreNo[i] == StoreNumber:            
                DataList.append(str(PlannerDetails.POGDBKey[i]))                
                i = i + 1                 
            else:
                PureRead.SRPOGRead(StoreNumber)                 
                StoreNumber = PlannerDetails.StoreNo[i]
        #PureRead.SRPOGRead(StoreNumber)                 
         
                                
#MainLine code starts from here 
TotalPlanner = 0 
start = time.time()
Result = open("C:/KeyReader/Results.txt","w+")
PureRead.ReadInput()
end = time.time()
FinalString = "Checked "  + str(TotalPlanner) + " planners and found " + str(Missing) + " Missing planners" + "\n" 
TotalTime = end - start
Result.writelines(FinalString) 
Result.writelines(str(TotalTime))
