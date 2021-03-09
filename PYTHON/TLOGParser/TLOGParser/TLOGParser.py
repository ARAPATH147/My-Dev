#######################################################################
#  TLOGParser      Ranjith Gopalankutty             17-09-2019        #
#  This program is written to parse the TLOG file in windows          #
#  environement so that, it comes to a readable format. Can add       #
#  more features in the future                                        #
#######################################################################

import sys 
import time
import datetime
import os
import glob 
from pathlib import Path 
import re
import binascii
import binhex
import decimal
import threading
import struct
 
       
  
def ReadTlog():
 
    try:
        FileLocation = input("Please enter the file location :")
        with open(FileLocation,encoding="Latin") as f:
            for line in f:
                print (line)
        
    except Exception as e:
             print('{c} - {m}'.format(c = type(e).__name__, m = str(e)))
         
#MainLine code starts from here 
start = time.time()
ReadTlog()
end = time.time()
print("I have taken total" ,(end - start) , "to execute")
