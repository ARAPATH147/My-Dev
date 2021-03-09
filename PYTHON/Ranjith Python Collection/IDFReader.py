
#######################################################################
#  IDFReader       Ranjith Gopalankutty            18-December-2018   #
#  This program is written to read any keyed file from 4690           #                                                    
#  controller in windows. There are vaious ways to use the program    #
#                                                                     #
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
import pandas as pd
import numpy 
 
class IDFRead: 
    
    def ProcessRecord(Record):
        IDFITEMCODE         =Record[3:4]
        IDFITEMCODE = hex(IDFITEMCODE)
        
        IDFFIRSTBARCODE        = str(struct.unpack("<i",Record[4:6]))  
        IDFSECONDBARCODE       = str(struct.unpack("<i",Record[10:6])) 
        #IDFNOOFBARCODES        = str(struct.unpack("<i",Record[16:2])) 
        #IDFPRODUCTGRP          = str(struct.unpack("<i",Record[18:3]))
        #IDFSTNDRDDESC          = str(struct.unpack("<i",Record[21:24]))
        #IDFSTATUS              = str(struct.unpack("<i",Record[45:1]))
        #IDFINTRO.DATE          = str(struct.unpack("<i",Record[46:3]))
        #IDFBSNS.CNTR           = str(struct.unpack("<i",Record[49:1]))
        #IDFFILLER              = str(struct.unpack("<i",Record[50:1]))
        #IDFBITFLAGS1           = str(struct.unpack("<i",Record[51:1]))
        #IDFBITFLAGS2           = str(struct.unpack("<i",Record[52:1]))
        #IDFPARENTCODE          = str(struct.unpack("<i",Record[53:4]))
        #IDFDATEOFLASTSALE      = str(struct.unpack("<i",Record[57:3]))
                   
    # Below function will help to read the keyed file in sector by sector
    # then each sector can be processed further   
    def KeyRead():     
        count = 0
        FileName = "C:/temp/IDF.BIN"  
        try:
            f = open(FileName,"rb")           
            NumberOfSectors = ((os.stat(FileName).st_size) / 512 )           
            while count <= NumberOfSectors:                
                byte = f.read(512)[-508:]                
                if count > 0:
                    IDFRead.ProcessRecord(byte)                    
                    print(count)
                    if count == NumberOfSectors:
                        break
                count = count + 1
        except Exception as e:
                ErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
                print(ErrorString)
                
#MainLine code starts from here 
start = time.time() 
IDFRead.KeyRead()
end = time.time()
print (" i have taken " + str(end - start) )

 
