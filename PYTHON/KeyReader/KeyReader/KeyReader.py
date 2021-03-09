#######################################################################
#  KeyReader       Ranjith Gopalankutty             1-December-218    #
#  This program is written to read any keyed file from 4690           #                                                    
#  controller in windows. There are vaious ways to use the program    #
#                                                                     #
#######################################################################

import sys 
import time
import datetime
import os
import glob 
from pathlib import Path 
import re
import binascii
import binhex
import decimal
import threading
import struct
import pandas as pd
import numpy 

class PureRead:        
                   
    # Below function will help to read the keyed file in sector by sector
    # then each sector can be processed further   
    def KeyRead(StoreNo,POGDBKEY):
        count = 0
        FixedPath = "C:/SRPOG/"
        FileName = "0000" + str(StoreNo)
        FullPath = FixedPath + "STORE" + FileName[-4:]
        try:
            f = open(FullPath,"rb") 
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
                print(StoreNo,POGDBKEY)
        except Exception as e:
             print('{c} - {m}'.format(c = type(e).__name__, m = str(e)))
    
    # Process the daily excel file        
    def ReadInput():
        ExcelFile = "c:/temp/planner.xlsx"
        PlannerDetails = pd.read_excel(ExcelFile)  
        i = 0
        while i < len(PlannerDetails):
            # Call the keyed file read by passing store number and POGDBKEY
            PureRead.KeyRead(PlannerDetails.StoreNo [i], PlannerDetails.POGDBKey[i])
            i = i + 1      
                                 
#MainLine code starts from here 
start = time.time()
PureRead.ReadInput()
end = time.time()
print("I have taken total" ,(end - start) , "to execute")