#
# BlockAnalyzer      Ranjith Gopalankutty         19-09-2018
# This program is written to analyze BLOCKITEM run status
# and update the status on success failures and update a 
# common log based on need. Also this program will create a
# text file , that will be used in NFM plan to send as email
#

import sys 
import time
import datetime
import os
import glob 
from pathlib import Path
import getpass
import re

# Below lines of code helps with Assignements and helps the function
# inside the code to use it whenever its required
 
EstateMessage = " "
FailCount = 0
Now = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
NumberOfitems = 0 
ROI = False 
StoreCount = 0
SuccessCount = 0
TotalNumberOfStores = 0
Unblocked = False
UnBlockedMessage = " "
UK = False
username = getpass.getuser()

# Functions defined below
def CallOfError(Message):
    MasterLog.write(Message + Now + '\n')
    quit()

# Function to create missing directory, highly unlikely that it gets called :-)
def CreateDirectory(Name):
    print("Creating missing directories")

    if (Name == "Item"):
        os.makedirs("C:/ITEM")
    elif (Name == "Input"):
        os.makedirs("C:/ITEM/INPUT")
    elif (Name == "OUTPUT"):
        os.makedirs("C:/ITEM/OUTPUT")
    elif (Name == "Log"):
        os.makedirs("C:/ITEM/LOG")
    else:
        print("Invalid directory is passed")     

# Function to delete the log files as we dont need to keep it
def Clearit():
    
    for file in os.listdir("c:/ITEM/OUTPUT/"):
        os.remove("c:/item/output/" + file)

# text required for mailing is created below
def MailIt():
    global EstateMessage
    global UnBlockedMessage
    global username
    MailLog.write("Hi All," + '\n')   

    if Unblocked == True:
        UnBlockedMessage = "An Item Unblock "
    else:
        UnBlockedMessage = "An item block "

    if UK and ROI == True:
        EstateMessage = "was performed on both UK and ROI stores using id "
    elif UK == True and ROI == False:
        EstateMessage = "was performed only on UK stores using id "
    elif UK == False and ROI == True:
        EstateMessage = "was performed only on ROI stores using id "
    
    if username == "CENTDCZC5350NHW$":
        username = "NFM Server"
    MasterLog.write("Progam Executed on " + Now + '\n' )
    MailLog.write(UnBlockedMessage + EstateMessage + username + " on " + Now +   '\n') 
    MasterLog.write(UnBlockedMessage + EstateMessage + username + " on " + Now +   '\n') 
    MailLog.write("Program executed on " + str(StoreCount) + " stores and Number of items blocked are " + str(NumberOfitems) + '\n')
    MasterLog.write("Program executed on  " + str(StoreCount) + " stores and Number of items blocked are " + str(NumberOfitems) + '\n')
    MailLog.write("Program is successful on " + str(SuccessCount) + " Stores and failed on " + str(FailCount) + " stores" + '\n')
    MasterLog.write("Program is successful on " + str(SuccessCount) + " Stores and failed on " + str(FailCount) + " stores" + '\n')
    MailLog.write("Thanks," + '\n')
    MailLog.write("Ranjith Gopalankutty")         

# To check if it was all stores or just UK only or ROI only
def BrexIt():
    global UK
    global ROI
    StoreList = os.listdir("c:/ITEM/OUTPUT/")
    result = bool("store0006"in StoreList)
    if ("STORE0006" or "STORE0023" or "STORE0158" or "STORE1200")  in StoreList:
        UK = True
    if ("STORE1700" or "STORE1705" or "STORE1710" or "STORE2700")  in StoreList:
        ROI = True       
        
           
# This Function looks for weather it was block or unblock execution by checking the output log
def SmackIt():
    Unblocked = False
    UnblockedString = "Unblocked"
    Filepath  = "c:/ITEM/OUTPUT/" 
    for file in os.listdir(Filepath):
        Fullpath = Filepath + file
        with open(Fullpath,encoding="Latin-1") as f:
            for line in f:
                if UnblockedString in line:
                    Unblocked = True
        if file[5:9] == "0006" or file[5:9] == "1700" or file[5:9] == "0023" or file[5:9] == "1705":
             return

# Main function to analyze BLOCKITM results
def WhackIt():
    SuccessMessage = "Completed Successfully"
    FailureMessage = "either IDF or IRF read issues please check the status"
    global StoreCount
    global FailCount 
    global SuccessCount
    UNBLOCK = 0
    Filepath  = "c:/ITEM/OUTPUT/"     
    for file in os.listdir(Filepath): 
        StoreCount =  StoreCount + 1
        Fullpath = Filepath + file      
        with open(Fullpath,encoding="Latin-1") as f:         
            for line in f:     
                if SuccessMessage in line:
                    SuccessCount = SuccessCount + 1
                
    FailCount = StoreCount - SuccessCount            
# Get total number of items count   
def BlockIt():
    global NumberOfitems
    with open("c:/ITEM/INPUT/BLOCKITM.DAT",encoding="Latin-1") as f:         
            for line in f:    
                 NumberOfitems = NumberOfitems + 1

# Ensure all directories are in place and create missing ones
def CheckIt():
    print("Checking if directory structure looks allright")
    try:
       Result = os.path.isdir("c:/item")
       if Result == False:             
           print("Item directory is missing")
           CreateDirectory("Item")

       Result = os.path.isdir("c:/item/input")
       if Result == True:   
           if not os.listdir("c:/item/input"):
               print("Directory is empty")
           else:
               NumberofItems = os.path.getsize("c:/item/input/BLOCKITM.DAT")
               NumberofItems = NumberofItems / 9
       else:
           print("Input directory is missing")
           CreateDirectory("Input")

       Result = os.path.isdir("c:/item/Log")
       if Result == False:     
        
           CreateDirectory("Log")          
        
       Result = os.path.isdir("c:/item/OUTPUT")
       if Result == True:
           if len(os.listdir("C:/ITEM/OUTPUT/")) == 0:
                 CallOfError("No Logs to analyze ")
       else:
           CreateDirectory("OUTPUT")
    except Exception as e:

        MasterLog.write(e)

# Mainline code starts from here
print("BlockAnalyzer starting")
MasterLog = open("C:/ITEM/LOG/masterlog.txt","a")
MailLog   = open("C:/ITEM/LOG/email.txt","w+")
CheckIt()
BlockIt()
WhackIt()
SmackIt() 
BrexIt() 
MailIt()
Clearit()
print("Completed successfully")