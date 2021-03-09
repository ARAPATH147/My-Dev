
#######################################################################
#  NewFinder       Ranjith Gopalankutty             1-December-2018   #
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
ItemList = [] 
DataList  = []
class PureRead:        
                         
    def SRPOGRead(StoreNumber):  
        global Missing
        global TotalPlanner
        FixedPath = "c:/getcip/"
        FileName = "0000" + str(StoreNumber) 
        FullPath = FixedPath + "STORE" + FileName[-4:] + ".log"
        print("now Checking for " + str(StoreNumber))
        try:
            f = open(FullPath,"r")            
            with open(FullPath,encoding="Latin-1") as f1:
                for line in f1:
                    ItemList.append(line[2:9])
            for k in range(len(DataList)):                 
                if((DataList[k][0:7]) in ItemList):
                    pass
                else:
                    Result.writelines("Store " + str(StoreNumber) + " has item code " + str(DataList[k][0:7]) + " as new item" + "\n")


           # Dont we need to clear both list??
            ItemList.clear()
            DataList.clear()
        except Exception as e:
            POGErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(POGErrorString)
            #Result.writelines(POGErrorString)
            # We need to clear the datalist in case of file mising as well
            DataList.clear()
            ItemList.clear()
  
    # Process the daily excel file        
    def ReadInput():

        global StoreList
        global TotalPlanner
        ExcelFile = "c:/temp/Andy.xlsx"
        print("Reading Excel file ")
        PlannerDetails = pd.read_excel(ExcelFile)  
        i = 0
        TotalPlanner = len(PlannerDetails)
        StoreNumber = PlannerDetails.StoreNo[i]
        while i < len(PlannerDetails): 
            
            if PlannerDetails.StoreNo[i] == StoreNumber:            
                DataList.append(str(PlannerDetails.ItemCode[i]))                
                i = i + 1                 
            else:
                PureRead.SRPOGRead(StoreNumber)                 
                StoreNumber = PlannerDetails.StoreNo[i]
        PureRead.SRPOGRead(StoreNumber)                 
         
                                
#MainLine code starts from here 
TotalPlanner = 0 
start = time.time()
Result = open("C:/temp/New.txt","w+")
PureRead.ReadInput()
end = time.time()
FinalString = "Checked "  + str(TotalPlanner) + " planners and found " + str(Missing) + " Missing planners" + "\n" 
TotalTime = end - start
