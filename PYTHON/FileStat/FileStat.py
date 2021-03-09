#######################################################################
# FileStat            RANJITH GOPALANKUTTY               20-05-2019   #
#   This program is written to read a keyed file first block using    #
#   python; program will helps to get the status of a file - useful   #
#   when we want to read 1000s of files and produce the stat.         # 
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

# Class contains few functions to process a keyed record
class FileStat:     
            
# Below function will help to read the keyed file in sector by sector
# then each sector can be processed further            
    def FirstBlock(filename):
        global Missing       
        FixedPath = "C:/phf/"
        FullPath = FixedPath + filename
        try:
            with open(FullPath,encoding="Latin-1") as f: 
                for line in f:
                    if line[0:24] == "Total file size (blocks)":
                        BlockSize = line[30:38]
                        DateOfCreation = line[70:78]
                    elif line[0:19] == "Randomizing divisor":
                        RandomizingDivisor = line[30:38]
                    elif line[0:17] == "Distribution type":
                        LongestChain = line[76:79]
                StatString = filename[5:9] + "," + BlockSize + ","  + DateOfCreation + "," + RandomizingDivisor + "," + LongestChain + "\n"
                FileStatus.writelines(StatString)

                     
        except Exception as e:
            ErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(ErrorString)
            FileStatus.writelines(ErrorString)                 
    
# Below function is to just add the pair of SRPOG and SRMAP file for comparison purpose
# Not used in this code as its just to show how to process a keyed record
    def GetAllFiles():
        for root, dirs, files in os.walk("C:/PHF/"):  
            for filename in files:               
                FileStat.FirstBlock(filename)
        
#MainLine code starts from here , then class call begins 
start = time.time()
FileStatus = open("C:/temp/FileStat.txt","w+")
FileStat.GetAllFiles()
end = time.time()
TotalTime = end - start
FileStatus.writelines(str(TotalTime))
FileStatus.close()

