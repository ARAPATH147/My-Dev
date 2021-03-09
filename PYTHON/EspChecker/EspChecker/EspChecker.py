
# ESPChecker      Ranjith Gopalankutty         21-09-2018
# This program is written for Gary wilkinson to check result
# status and send mail to all  users on daily basis.

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
NumberOfStores = 0 
ROI = False 
StoreCount = 0
SuccessCount = 0
TotalNumberOfStores = 0

# text required for mailing is created below
def MailIt():
   
    MailLog.write("Hi All, " + Now + '\n' )
    MailLog.write("ESPCheck program has run successfuly around " + Now + "\n")
    MailLog.write("Found " + str(NumberOfStores)  + " failures for today " + '\n')
    MailLog.write("Thanks," + '\n')
    MailLog.write("Gary Wilkinson")
	
# Get total number of items count   
def BlockIt():
    global NumberOfStores
    with open("C:/ESPCHECK/results.txt",encoding="Latin-1") as f:         
            for line in f:    
                 NumberOfStores = NumberOfStores + 1

# Mainline code starts from here
print("EspChecker starting")
ResultLog = open("C:/ESPCHECK/results.txt","r")
MailLog = open("c:/ESPCHECK/espmail.txt","w+") 
BlockIt()
MailIt()
print("Completed successfully")
