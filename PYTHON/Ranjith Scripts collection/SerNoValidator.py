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

# All the global variable and list declarations are here

Valid_Entry = []
Read_Serial_Number = [] 
FinalList = []
MisFile = open("C:/TEMP/MisMatch.TXT","w+")

# Went with list option as its much faster comparing with 
# sequential file read and write. 

def FinalDo():
     
    MisFile.writelines("****************************************************************************\n")
    MisFile.writelines("*           Report of PED serial Numbers mismatches                        *\n" )
    MisFile.writelines("****************************************************************************\n")
    MisFile.writelines("Below Entries shows whats in stores and not in config server [4 Digit Store Number + 10 Digit PED Serial Numbers]\n")
    MisFile.writelines([c for c in Valid_Entry if c not in Read_Serial_Number])  
    MisFile.write("Below entries displays whats in Config Server and Not present in Stores [4 Digit Store Number + 10 Digit PED Serial Numbers]\n")
    MisFile.writelines([c for c in Read_Serial_Number if c not in Valid_Entry])
  
# Function will read through AdvAudit files from the latest date
# and read through all valid 75 string. One final list will be created

def ReadToday():     
    
    Filepath  = "c:/Q09/"
    print("Reading upto date AdvAudit Files")
    for file in os.listdir(Filepath):        
        Fullpath = Filepath + file      
        with open(Fullpath,encoding="Latin-1") as f: 
            I = 0
            for line in f:           
                SeventyFive =  bool('Response :75:'in line)
                if SeventyFive == True:                    
                    NewLine = re.split("[:,]",line)
                    if (NewLine[12] != "" and NewLine[12] != '0'): 
                        I = I + 1
                        LineEntry  = file[5:9]+NewLine[12] + '\n'
                        PEDEntry   = file[5:9] + "," + NewLine[12] + "\n"
                        ValidPED.writelines(PEDEntry)
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

def ReadYesterday():

    #Filepath  = "//centre1.uk.boots.com/user/shared/Boots The Chemists/Is & T/Business Systems/S_Epos/Projects/Service Problems/" + FolderName + "/"
    Filepath  = "//centre1.uk.boots.com/user/shared/Boots The Chemists/Is & T/Business Systems/S_Epos/Projects/Service Problems/TEMP/"
    print("Checking  AdvAudit from yesterday to get early morning resets before 2.20am" )
    EntryCheck = datetime.datetime.today().strftime('%y%m%d')
    for file in os.listdir(Filepath):        
        Fullpath = Filepath + file 
       
        with open(Fullpath,encoding="Latin-1") as f: 
            I = 0
            for line in f:           
                SeventyFive =  bool('Response :75:'in line)
                if SeventyFive == True:                    
                    NewLine = re.split("[:,]",line)
                    if (NewLine[12] != "" and NewLine[12] != '0'):
                        if NewLine[0][1:7] == EntryCheck:
                            LineEntry  = file[0:4]+NewLine[12] + '\n'  
                            if LineEntry in Valid_Entry:
                                position = Valid_Entry.index(LineEntry)
                                Valid_Entry[position] = LineEntry
                            else:
                                Valid_Entry.append(LineEntry) 
                                
# Function identifies the latest folder to open from T drive location
def FindFolderName():
    global FolderName
    print("Identifying the AdvAudit folder Name to read")    
    FolderName = datetime.datetime.today().strftime('%d-%b-%Y')      

# Function to read Config Server file and format it correctly
# formatted entries will be added to List  Read_Serial_Number
def ReadSerialNumber(): 
    
    line = file.readline()    
    while True:
        line = file.readline()         
        length = len(line) 
        if (length == 16):      
            StoreNumber = line[0:4]
            PedNumber = line[5:16]
            OutputLine = StoreNumber + PedNumber         
            Read_Serial_Number.append(OutputLine)
            #rk.writelines(OutputLine)
        TotalLength = len(StoreNumber) + len(line)
    
        if (TotalLength <= 16) and (TotalLength >= 14) and (line != '\n') and (line != ""):
                result = bool('NULL'in line)
                if (result == False) :
                    TotalLine = StoreNumber + line
                    laslength = len(TotalLine)
                    if (laslength <=16) and (laslength >=10):                  
                        Read_Serial_Number.append(TotalLine)
                        #rk.writelines(TotalLine)
        if ("" == line):
            print ("PED Serial Number has been formatted correctly")
            break 

    Read_Serial_Number.sort()                                                         

###############################################################################
#                                                                             # 
#                     Start of Mainline Code                                  #
#                                                                             #  
###############################################################################

start = time.time()
#file = open("C:/temp/serialnumbers.txt","r")
rk = open("C:/temp/rkd.txt","w+")
ValidPED = open("C:/temp/StorePed.txt","w+")
print("Config Server serial number file opened")
print("Ignoring the first line to move over title")   

#Function calls starts
#ReadSerialNumber()
#FindFolderName()
ReadYesterday()
ReadToday() 
#FinalDo()
#end = time.time()
#my_file = Path("c:/temp/StoreNumbers.txt")
print("Now Deciding if needs to check against deployment stores")

if my_file.is_file():
    Store = open("C:/temp/storenumbers.txt","r")
    print("File exists , triggering JustCompare")
    os.system("python C:\Dev\Python\JustCompare\JustCompare\JustCompare.py") 
    print("JustCompare is finished, please check output files")
else:
    print("No Store Number file in c:/temp location not triggering JustCompare")  
  
print("I have taken total" ,(end - start) , "to execute")
MisFile.close()

