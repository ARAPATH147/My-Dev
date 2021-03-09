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

StoreList = [] 
class PureRead:        
                   
    # Below function will help to read the keyed file in sector by sector
    # then each sector can be processed further  
       
     
        
    def KeyRead(StoreDB):
        global Missing
        StoreNo = StoreDB.split(",")[0]
        POGDBKEY = StoreDB.split(",")[1]
        count = 0
        FixedPath = "C:/SRPOG/"
        FileName = "0000" + str(StoreNo)
        FullPath = FixedPath + "STORE" + FileName[-4:]
        try:
            f = open(FullPath,"rb") 
            Status =   "Checking for planner key " + str(POGDBKEY) + " in store " + FullPath[-4:]  + "\n"
            print(Status)
            NumberOfSectors = ((os.stat(FullPath).st_size) / 512 ) - 1
            found = False
            while count <= NumberOfSectors and found == False:
                byte = f.read(512)[4:508]               
                NewByte = repr(byte).replace('\\x', '') 
                count = count + 1
                if count > 1:
                    ValueTest = NewByte[2:6]
                    if ValueTest != "0000":
                        if  NewByte[1:5] !="" :                         
                            Iterator = int(508 / 101)
                            i = 0
                            while i < Iterator:
                                R = i * 101
                                S = R + 101
                                EachRecord = byte[R:S]                             
                                Value = str(struct.unpack("<i",EachRecord[0:4]))
                                POGDB = re.findall(r'\d+', Value)
                                POGDBKEY = str(POGDBKEY)
                                if POGDBKEY in POGDB:
                                    found = True                                  
                                    break
                                length = len(POGDB[0])

                                if POGDB[0] == "000000" or POGDB[0] == " " or len(POGDB[0]) == 1:
                                    i = Iterator
                                    break                                                  
                                i = i + 1 
            if found == False:
                WriteString = "Store " + str(StoreNo) + " has missing planner  " +  str(POGDBKEY) + "\n"
                Result.writelines(WriteString)
                Missing = Missing + 1
        except Exception as e:
                ErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
                Result.writelines(ErrorString)

        return Missing

    def testfunction(StoreNo):
        PureRead.KeyRead(StoreNo,PlannerDetails.POGDBKey[i])
    # Process the daily excel file        
    def ReadInput():

        global StoreList
        global TotalPlanner
        ExcelFile = "c:/temp/planner.xlsx"
        PlannerDetails = pd.read_excel(ExcelFile)  
        i = 0
        TotalPlanner = len(PlannerDetails)
        while i < len(PlannerDetails):                       
            # Call the keyed file read by passing store number and POGDBKEY
            #result = pool.starmap(partial(PureRead.KeyRead,PlannerDetails.StoreNo [i]),PlannerDetails.POGDBKey[i]) 
            FunctionString = str(PlannerDetails.StoreNo[i]) + ","+str(PlannerDetails.POGDBKey[i])
            StoreList.append(FunctionString)
            i = i + 1
            PureRead.KeyRead(FunctionString)             

         
                                
#MainLine code starts from here 
TotalPlanner = 0
Missing = 0
start = time.time()
Result = open("C:/KeyReader/Results.txt","w+")
PureRead.ReadInput()
end = time.time()
FinalString = "Checked "  + str(TotalPlanner) + " planners and found " + str(Missing) + " Missing planners" + "\n" 
TotalTime = end - start
Result.writelines(FinalString) 
Result.writelines(str(TotalTime))
