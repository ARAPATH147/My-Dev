
#######################################################################
#  Sanjeev       Ranjith Gopalankutty             1-December-2018     #
#  This program is written to get some data for Sanjeev, who is a     #
#  project manager                                                    #
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
i = 0
PlannerDetails = " "
 
 

class PureRead:        
                         
    def rkd(StoreNumber):
        global PlannerDetails        
       
        try:
            NewStoreNumber = "0000" + str(StoreNumber)
            NewStoreNumber = NewStoreNumber[-4:]
            Count = 0
            with open("c:/temp/rkd.txt",encoding="Latin-1") as f:
                for line in f:
                    if line[0:4] == NewStoreNumber:
                        Count = Count + 1
                PlannerDetails.Tills = Count
                PlannerDetails.PED = Count
                WriteString =  str(NewStoreNumber) + "," + str(Count) + '\n'
                temp.writelines(WriteString)

        except Exception as e:
            POGErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(POGErrorString)
            
             
  
    # Process the daily excel file        
    def ReadInput():

        global i
        global  PlannerDetails
        i = 0
        ExcelFile = "c:/temp/PCI Audit Store Info-May19.xlsx"
        print("Reading Sanjeev's input file ")
        PlannerDetails = pd.read_excel(ExcelFile) 
        while i < len(PlannerDetails):
            PureRead.rkd(PlannerDetails.StoreNumber[i])
            i = i + 1
          
            
                          
         
                                
#MainLine code starts from here 
 
start = time.time() 
temp = open("c:/temp/storecount.txt","w+")
PureRead.ReadInput()
end = time.time() 
