
# ESPChecker      Ranjith Gopalankutty         21-09-2018
# This program is written for Gary wilkinson to check result
# status and send mail to all  users on daily basis.
#
# Version B       Gary Wilkinson               27-09-2018
# Modified to do the whole check process rather than just
# generating the email.

import sys 
import time
import datetime as dt
import os
import glob 
from pathlib import Path
import getpass
import re

# Below lines of code helps with Assignements and helps the function
# inside the code to use it whenever its required
 
Now = dt.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
Yesterday = dt.datetime.now() - dt.timedelta(days=1)
Yesterday = Yesterday.replace(hour=21, minute=59, second=00, microsecond=00)

# Generate report to email
def MailIt():  
    MailLog.write("Hi All," + '\n' )
    MailLog.write("ESPCheck program has run successfully around " + Now + "\n")
    MailLog.write("Found " + str(len(resultslist))  + " failures for today: " + '\n')
    if len(resultslist) > 0:
        for x in resultslist:  
            MailLog.write(x + '\n')      
    MailLog.write("Thanks," + '\n')
    MailLog.write("EPOS Applications Management")

                     
# Check for failures
def CheckIt():
    for filename in glob.glob('c:\\espcheck\\data\\*'):
        with open(filename, encoding="Latin-1") as h:
            for fileline in h:
                if ' GMT ' in fileline and fileline[:1] == "#":
                    date_ran = dt.datetime.strptime(fileline[5:].strip('\n'), '%b %d %H:%M:%S %Z %Y')                   
                    if date_ran < Yesterday:
                        resultslist.append(os.path.basename(filename).strip(".txt") + " " + str(date_ran))
        os.remove(filename)
        
# Mainline code starts from here
print("EspChecker starting")
MailLog = open("c:/ESPCHECK/espmail.txt","w+")                             
resultslist = []
CheckIt()
MailIt()
print("Completed successfully")
MailLog.close()

