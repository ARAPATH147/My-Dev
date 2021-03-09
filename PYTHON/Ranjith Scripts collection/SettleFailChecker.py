
###############################################################################
#   SettleFailChecker     Ranjith Gopalankutty               26-05-2018       #
#    Below piece of code reads through Advaudit files and find if any         #
#    settlement fails
###############################################################################

import sys 
import time
import datetime
import re
import csv
import os
import glob 
from pathlib import Path


 
# Function will read through AdvAudit files from the latest date
# and read through all valid 58 string and finds if any settlement failure
def ReadAdvAudit():      
    NumFolders = 0
    FileCount = 0
    FiftySixCount   = 0
    Invalid   = 0   

    
    for root, dirs, files in os.walk('//centre1.uk.boots.com/user/shared/Boots The Chemists/Is & T/Business Systems/S_Epos/Projects/Service Problems/'):
        TotalFolders = dirs
        NumFolders = len(TotalFolders) 
        i = 0
        while i <= NumFolders:
        #while i == 0:
            FilePath = TotalFolders[i]
            result = bool('Jun'in TotalFolders[i])
            if result == True:

            #FilePath = '28-Jun-2018'
                Fullpath = "//centre1.uk.boots.com/user/shared/Boots The Chemists/Is & T/Business Systems/S_Epos/Projects/Service Problems/" + FilePath + "/"
                FileLine = "Settlement issues for the date - " + FilePath + '\n'
                OutPut.writelines(FileLine)
                print("Checking Files for day ", FilePath)
                for file in os.listdir(Fullpath):
                   
                    Finalpath = Fullpath + file    
                    with open(Finalpath,encoding="Latin-1") as  auto: 
                        FileCount = FileCount + 1                 
                        for line in auto:           
                            FiftySix =  bool('UNPROCESSED SETTLEMENTS'in line)
                            if FiftySix  == True:   
                                FiftySixCount = FiftySixCount + 1
                                Invalid = Invalid + 1
                                OutPut.writelines(line)
            i = i + 1
 
    StatusLine = "I have read total " + str(NumFolders) + " days data " + str(FileCount) + " files and Found " + str(FiftySixCount) + " :58: strings " + " on those found " + str(Invalid) + "settlement issues"
    print(StatusLine) 
    OutPut.writelines(StatusLine)
 
# Function identifies the latest folder to open from T drive location
def FindFolderName():
    global FolderName        
    FolderName = datetime.datetime.today().strftime('%d-%b-%Y')       
        
        
###############################################################################
#                                                                             # 
#                     Start of Mainline Code                                  #
#                                                                             #  
###############################################################################

start = time.time()
OutPut = open("c:/temp/SettleFail.txt","w+")
#Function calls starts
FindFolderName()
ReadAdvAudit()
end = time.time()



print("I have taken total" ,(end - start) , "to execute")
    
