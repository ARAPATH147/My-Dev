###############################################################################
#   SerNoValidator        Ranjith Gopalankutty               25-05-2018       #
#    This python script will compare the config server serial numbers         #
#    against the store AdvAudit numbers and will report the differences       #
#    Written for validation during PED Serial Number validation project       #
###############################################################################

import sys 
import time
import datetime
import re
import csv
import os
import glob 
from pathlib import Path
#import msvcrt, os, sys
#msvcrt.setmode(sys.stdout.fileno(), os.O_BINARY)

Valid_Entry = []
FinalList = []

def ProcessRkd():
    windows_line_ending = '\r\n'
    linux_line_ending = '\n'
    
    filename = "C:/temp/rkd.txt"
    with open(filename, 'rb') as f:
        content = f.read().decode('utf-8')
        

    with open("c:/temp/just.txt", 'wb') as d:       
        FinalList = content.replace(windows_line_ending,linux_line_ending)
        d.write(FinalList.encode('utf-8')) 
       

    with open ("c:/temp/just.txt",'rb') as e:
        for line in e:
            
            StoreNumber = line[0:4]
            if StoreNumber != PrevStore:

                PrevStore = StoreNumber
                PedString = line[4:16]
                TotalString = TotalString + PedString

            elif:
                TotalString = TotalString + line[4:16]
            
        #StoresString = "Boots" + ","+ StoreNumber+ ","+StoreNumber+"%"+ PEDString           



def CreateOneFile():      
    
    #Filepath = "//centre1.uk.boots.com/user/shared/Boots The Chemists/Is & T/Business Systems/S_Epos/Projects/Service Problems/" + FolderName + "/"  
    Filepath  = "c:/Q09/"
    print("Reading the AdvAudit Files")
    for file in os.listdir(Filepath):        
        Fullpath = Filepath + file      
        with open(Fullpath,encoding="Latin-1") as f: 
            I = 0
            for line in f:           
                SeventyFive =  bool('Response :75:'in line)
                if SeventyFive == True:                    
                    NewLine = re.split("[:,]",line)
                    if (NewLine[12] != "" and NewLine[12] != '0'):           
                        LineEntry  = file[5:9]+NewLine[12] + '\n'    
                        # Below set of code helps to overwrite existing entries
                        # during reboot scenarios of Tills, latest 75 value 
                        # will be taken
                        if LineEntry in Valid_Entry:
                            position = Valid_Entry.index(LineEntry)
                            Valid_Entry[position] = LineEntry
                        else:
                            Valid_Entry.append(LineEntry)                            
    Valid_Entry.sort()  
    for x in range(len(Valid_Entry)):
        rk.writelines(Valid_Entry[x])	

start = time.time()
#file = open("C:/temp/serialnumbers.txt","r") 
 
print("Config Server serial number file opened")
print("Ignoring the first line to move over title")   

#Function calls starts
#CreateOneFile() 
ProcessRkd()
