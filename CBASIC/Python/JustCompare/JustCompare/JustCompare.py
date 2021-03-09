###############################################################################
#   JustCompare      Ranjith Gopalankutty             13-06-2018              #
#     This Python script will compare the missing serial number store         #
#     against configuration server and report if any of the stores are        #
#     part of deployment , if so write to deploy.txt                          #
###############################################################################
import sys 
import time
import datetime
import re
import csv
import os
import glob 
from pathlib import Path

StoreNumbers = []
Mismatch     = []
OldOutLis = []

def Difference():
    Final.writelines("Below stores have got missing Serial Numbers \n")
    #Final.writelines([c + "\n" for c in StoreNumbers if c in Mismatch])
    Final.writelines([c for c in Mismatch if c in OldOutLis])
	

def ReadOld():

    OldLine = Old.readline()
    OldLine = Old.readline()
    while True:
        OldLine = Old.readline()         
        length = len(OldLine) 
        if (length == 37):      
            StoreNumber = OldLine[0:4]
            PedNumber = OldLine[26:37]
            OldOutLine = StoreNumber + PedNumber         
            OldOutLis.append(OldOutLine)
        OldLength = len(StoreNumber) + len(OldLine)
    
        if (OldLength <= 16) and (OldLength >= 14) and ( OldLine != '\n') and (OldLine != ""):
                result = bool('NULL'in OldLine)
                if (result == False) :
                    OldLine = StoreNumber + OldLine
                    Oldlaslength = len(OldLine)      
                    if (Oldlaslength <=16) and (Oldlaslength >=10):                  
                        OldOutLis.append(OldLine)
        if ("" == OldLine):
            print ("Old File has been formatted correctly")
            break 

    OldOutLis.sort()
	


def ReadStoreNumber():
    
    while True:
         StoreNum = Store.readline()
         if len(StoreNum) == 2:
             StoreNum = "000"+StoreNum
         elif len(StoreNum) == 3:
                StoreNum = "00"+StoreNum
         elif len(StoreNum) == 4:
                 StoreNum = "0" + StoreNum

             
         StoreNumbers.append(StoreNum[0:4])
         
         if ("" == StoreNum):     
             break  


def ReadMismatch():
   
         Line = Mis.readline()
         Line = Mis.readline()
         Line = Mis.readline()
         Line = Mis.readline()
         while True:
             Line = Mis.readline()
              
             if Line[0:5] == "Below":
                 break
             else:
                 Mismatch.append(Line)

Store = open("c:/temp/storenumbers.txt","r")
Mis  =  open("c:/temp/mismatch.txt","r")
Final = open("c:/temp/deploy.txt","w+")
ReadMismatch()
ReadStoreNumber()

OldFile = Path("c:/temp/old.txt")

if OldFile.is_file():
    print("old file exists , formatting")
    Old = open("c:/temp/old.txt" ,"r")
    ReadOld()
else:
    print("No old file to format , going ahead with other processing")

Difference()
    
