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
    
    def FormatRecord(Field):
        i = 0
        global StringValue
        StringValue = ""
        while i < len(Field):
            StringValue = StringValue + format(Field[i],'02x')
            i = i + 1
        return StringValue
            
# Below function will help to read the keyed file in sector by sector
# then each sector can be processed further            
    def FirstBlock(filename):
        global Missing       
        FixedPath = "C:/phf/"
        FullPath = FixedPath + filename
        try:
            f = open(FullPath,"rb")     
            Block = f.read(512)
            FileCreationDate = str(ord(Block[33:34])) + "/" + str(ord(Block[32:33])) + "/"  #+ str(ord(Block[30:32]))
            BlockSize        = struct.unpack("L",Block[42:46])[0]
            RecordSize       = struct.unpack("<H",Block[46:48])[0]
            FileCapacity     = (BlockSize - 1) * int(508/RecordSize)
            LongestChain     = str(struct.unpack("L",Block[60:64])[0])
            TotalWrites      = str(struct.unpack("L",Block[76:80])[0])
            TotalDeletes     = str(struct.unpack("L",Block[88:92])[0])
            CurrentRecords  =  int(TotalWrites) - int(TotalDeletes)
            PercentageFull  =  (CurrentRecords/FileCapacity) * 100
            StatString = filename[5:9] + "," + str(FileCapacity) + "," + LongestChain + "," + str(CurrentRecords) + "," + str(PercentageFull) + "\n"
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

