#######################################################################
# SRP06              RANJITH GOPALANKUTTY                15-01-2019   #
# This program is written to read a keyed file using python  which    # 
# is incredibly easy once we understand the exact field type          # 
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
class PureRead:         

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
    def SRPOGRead(StoreList):
        global Missing
        count = 0
        SRPOGFileName = StoreList[0][:13]     # Getting the SRPOG file value from first 13 bytes
        SRMAPFileName = StoreList[0][-13:]    # Getting the SRMAP file value from second 13 byt
        count = 0
        FixedPath = "C:/SRP06/"
        FullPath = FixedPath + SRPOGFileName
        try:
            f = open(FullPath,"rb")      
            NumberOfSectors = ((os.stat(FullPath).st_size) / 512 ) - 1
            found = False
            while count <= NumberOfSectors:
                Sector = f.read(512)[4:512]               
                count = count + 1
                #  We dont want the first sector as it tells the file attributes
                if count > 1:                    
                        Iterator = int(508 / 101)
                        RecordPerSector = 0
                        while RecordPerSector < Iterator:
                            R = RecordPerSector * 101
                            S = R + 101  
                            Record = Sector[R:S]
                            # Below line of code tells how to process little ending value from 4690
                            SRPOGPOGDB            =  str(struct.unpack("<i",Record[0:4])).strip('(,)')
                            if SRPOGPOGDB != '0':
                                # Below line of code tells how to process little ending value from 4690
                                SRPOGPOGID            =  str(struct.unpack("<i",Record[4:8])).strip('(,)')
                                SRPOGACTDATE          =  Record[8:12]                                      
                                SRPOGDEACTDATE        =  Record[12:16]
                                SRPOGDESCRIPTION      =  Record[16:46].decode("utf-8")
                                SRPOGPLANNERFAMILY    =  Record[46:76].decode("utf-8")
                                # Below line of code is to just convert a byte of hex data in to decimal
                                SRPOGMODULECOUNT      =  ord(Record[76:77])
                                # Below line of code is to unpack a PD value which is not little endian
                                SRPOGCATDBKEY         =  str(struct.unpack("i",Record[77:81])).strip('(,)')
                                SRPOGKEYLEVEL         =  Record[81:82]
                                SRPOGLIVERPTCNT       =  Record[82:83]
                                SRPOGDATERPTCNT       =  Record[83:87]
                                SRPOGPENDRPTCNT       =  Record[87:88]
                                # Below lines of code is to unpack a PD value which is not little endian
                                SRPOGCAT1ID           =  str(struct.unpack("i",Record[88:92])).strip('(,)')
                                SRPOGCAT2ID           =  str(struct.unpack("i",Record[92:96])).strip('(,)')
                                SRPOGCAT3ID           =  str(struct.unpack("i",Record[96:100])).strip('(,)')
                                SRPOGFILLER           =  Record[100:101].decode("utf-8")

                            # All Below functions calls are to convert UPD values in to normal integer 
                            # As using binary read method in python, each byte is taken as hex even 
                            # even if  its integer, so its converting back to original value                            
                                PureRead.FormatRecord(SRPOGACTDATE)
                                SRPOGACTDATE     =  StringValue
                                PureRead.FormatRecord(SRPOGDEACTDATE)
                                SRPOGDEACTDATE   =   StringValue                    
                                PureRead.FormatRecord(SRPOGKEYLEVEL)
                                SRPOGKEYLEVEL    = StringValue
                                PureRead.FormatRecord(SRPOGLIVERPTCNT)
                                SRPOGLIVERPTCNT  = StringValue
                                PureRead.FormatRecord(SRPOGDATERPTCNT)
                                SRPOGDATERPTCNT  = StringValue
                                PureRead.FormatRecord(SRPOGPENDRPTCNT)
                                SRPOGPENDRPTCNT  = StringValue
                                
                                #Now Lets Make the Complete String out of it
                                CompleteString = SRPOGPOGDB + "," + SRPOGPOGID + "," + SRPOGACTDATE + ", "                           + \
                                                 SRPOGDEACTDATE + "," + str(SRPOGDESCRIPTION) + "," + str(SRPOGPLANNERFAMILY)  + "," + \
                                                 str(SRPOGMODULECOUNT) + "," + SRPOGCATDBKEY + "," + SRPOGKEYLEVEL + ", "                 + \
                                                 SRPOGLIVERPTCNT + "," +  SRPOGDATERPTCNT + "," + SRPOGPENDRPTCNT + ","              + \
                                                 str(SRPOGCAT1ID) + "," + str(SRPOGCAT2ID) + "," + str(SRPOGCAT3ID) + "," + str(SRPOGFILLER) + "\n"
                                SRP06.writelines(CompleteString)
                                RecordPerSector = RecordPerSector + 1 
                            else:
                                # As soon finding a null record , stop reading that sector and move on
                                # This will make the keyed reader much faster
                                RecordPerSector = Iterator                               
                            
        except Exception as e:
            ErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(ErrorString)
            SRP06.writelines(ErrorString)                 
    
# Below function is to just add the pair of SRPOG and SRMAP file for comparison purpose
# Not used in this code as its just to show how to process a keyed record
    def ReadSRPOG():
        for root, dirs, files in os.walk("C:/SRP06/"):  
            for filename in files:
                if filename[-3:] == '002':
                    pass
                elif filename[-3:] == '001':
                    # Now check if it has got matching SRMAP
                    MapFileName = filename[:9] + ".002"
                    if MapFileName in files:
                        print ("File exists, lets add it to the List")
                        StoreString = filename  + MapFileName
                        StoreList.append(StoreString)
                else:
                    pass               
        PureRead.SRPOGRead(StoreList)
        
#MainLine code starts from here , then class call begins
TotalPlanner = 0
Missing = 0
start = time.time()
SRP06 = open("C:/SRP06/SRP06.txt","w+")
PureRead.ReadSRPOG()
end = time.time()
TotalTime = end - start
SRP06.writelines(str(TotalTime))
SRP06.close()
