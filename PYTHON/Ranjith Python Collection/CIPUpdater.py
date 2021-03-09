###############################################################################
#  CipUpdater        Ranjith Gopalankutty               14-06-2019            #
#  This script will read CIP status from all stores output files and update   #
#  DaveBase. So for checking the CIP status or producing any sort of          # 
#  reporting purpose, one doesn't need to log on to store conrollers          #  
###############################################################################

import sys 
import time
import datetime
import os
import glob 
from   pathlib import Path 
import pyodbc


StoreList      = []
Cursor         = ""


def processcip():

    Filepath = "C:/GETCIP/"
    with open(FullPath,encoding="Latin-1") as f1:
        for line in f1:
                ItemList.append(line[2:9])
        # Now need to update this  to the database as a string
        try:
            global cursor
            SqlCommand  = 'UPDATE APPS_EPOS_POC.dbo.CIP VALUES (?,?,?)'
            FieldValues =  [StoreNumber,CIPList,DateTimeStamp]            
            cursor.execute(SqlCommand,FieldValues) 
            cursor.commit()
        except Exception as e:
            CIPErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)
   
def ConnectDaveBase():
    
    global cursor

    try:
        conn = pyodbc.connect('Driver={SQL Server};'
                               'Server=UKC1CENTPV;'
                               'Database=APPS_EPOS_POC;'                                  
                               'Trusted_Connection=yes;')
        cursor = conn.cursor()

    except Exception as e:
        CIPErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
        print(CIPErrorString)
###############################################################################
#                                                                             #
#                     Start of Mainline Code                                  #
#                                                                             #
###############################################################################
start = time.time()
ConnectDaveBase() 
end = time.time()
TotalTime = end - start 
 
 
