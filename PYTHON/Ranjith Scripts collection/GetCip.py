###############################################################################
#    GetCip        Ranjith Gopalankutty               05-02-2019              #
#    This python is snapped it up for proceeing the items which has CIP flag  #
#    turned on in IDF and ensure it has got matching Reversal or H record in  #
#    in CIPPMR file, else it will be an issue                                 #
###############################################################################

import sys 
import time
import datetime
import os
import glob 
from   pathlib import Path 
import re
import pandas as pd
import numpy

# I dont know , why i need so many lists but let it be.

StoreList      = []
ItemList       = []
PMRList        = []
UniqueItemList = []
StoreCountList = []
CIPOTLIST      = []

def AnalyzeFurther():

    FileName = "c:/temp/getcip.txt"
    with open(FileName,encoding="Latin-1") as f3:
        for line in f3:
            UniqueItemList.append(line[20:27])    
            StoreCountList.append(line[6:10])
        ItemCount =  pd.Series(UniqueItemList).value_counts().to_dict()
        StoreCount = pd.Series(StoreCountList).value_counts().to_dict()
        print("Now sorting item codes against store counts")
        ItemAnalytics.writelines("******************************************************************************" + "\n")
        ItemAnalytics.writelines("*                     Item Level count report                                *" + "\n")
        ItemAnalytics.writelines("******************************************************************************" + "\n")
        for element in ItemCount:
            ItemAnalytics.writelines("item Code " + str(element) + " Has CIP reversal missing in " + str(ItemCount[element]) + " stores" +"\n")
        print("Now checking number of items affected per store" + "\n")
        StoreAnalytics.writelines("******************************************************************************" + "\n")
        StoreAnalytics.writelines("*                     Store Level count report                               *" + "\n")
        StoreAnalytics.writelines("******************************************************************************" + "\n")
        for element in StoreCount:
            StoreAnalytics.writelines("Store " + str(element) + " Has " + str(StoreCount[element]) + " items in cip without matching R record" +"\n")
 
def processcip():

    Filepath = "C:/GETCIP/"
    for i in range(len(StoreList)):
        print("now checking for ", StoreList[i])
        #Process the store IDF list and add to a 
        LogFileName = StoreList[i] + ".LOG"
        FullPath = Filepath + LogFileName
        with open(FullPath,encoding="Latin-1") as f1:
            for line in f1:
                ItemList.append(line[2:9])
        #Now wack the current stoes PMR file in to a list
        PMRFileName = StoreList[i] + ".PMR"
        FullPath = Filepath + PMRFileName
        with open(FullPath,encoding="Latin-1") as f2:
            for line in f2:
                PMRList.append(line[:8])
       # Now lets do the comparison we are looking for R record let me tell you 
       # but we will be happy with H as well :-).

        for j in range(len(ItemList)):
            FirstTry = "R"+ ItemList[j]
            try:
                b=PMRList.index(FirstTry)
            except ValueError:
                SecondTry = "H" + ItemList[j]
                if SecondTry in ItemList:
                    pass
                else:
                    file.writelines("Store " + PMRFileName[5:9] + " has item "  + ItemList[j] + " without matching reversal record" +"\n")
               
            else:
                pass
    # Need to call CIPOT processing as well only if 
    # there are add request for that day else ignore that store for that day.

        CIPOTFileName = StoreList[i] + ".OT"
        FullPath = Filepath + CIPOTFileName
        AddRecordCount = 0                                   # initialize it before each 
        try:
            with open(FullPath,encoding="Latin-1") as f4:
                for line in f4:                 
                    if line[:1] == "A":
                        AddRecordCount = AddRecordCount + 1
                        CIPOTLIST.append(line[:8])
                #Now check if you need to a comparison against CIPPMR list based on the count
                if AddRecordCount > 0:
                    for k in range(len(CIPOTLIST)):
                        if ((CIPOTLIST[k][-7:]) in ItemList):

                            try:
                                Rtype = "R" + CIPOTLIST[k][-7:]
                                b=PMRList.index(Rtype)
                            except ValueError:
                                Htype = "H" + CIPOTLIST[k][-7:]
                                if Htype in PMRList:
                                    pass
                                else:
                                    AddAnalytics.writelines("Store " + str(PMRFileName[5:9]) + " received Add request for item "  + str(CIPOTLIST[k][-7:]) + " without matching reversal or held record in PMR" +"\n")
                            else:
                                pass
                        else:
                            #mark down hasn't applied may be due to various factors such as planner leaver
                            # date hasn't met and is in pending list until then , so need to worry about
                            # such items
                            pass
            #Now clear both list before next set of store
            PMRList.clear()
            ItemList.clear()
            CIPOTLIST.clear()
        except Exception as e:
            ErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            file.writelines(ErrorString)
            PMRList.clear()
            ItemList.clear()
    file.close()
     
def Getcip():
    for root, dirs, files in os.walk("C:/GETCIP/"):  
        for filename in files:
            if filename[-3:] == 'LOG' or filename[-3:] == 'log':
                # now ensure a matching PMR file exist
                PMRFile = filename[:9] + ".PMR"
                if PMRFile in files:
                    StoreList.append(filename[:9])
                else:
                    pass             
            else:
                pass
    processcip()
###############################################################################
#                                                                             #
#                     Start of Mainline Code                                  #
#                                                                             #
###############################################################################
start = time.time()
file = open("C:/temp/GETCIP.txt","w+") 
AddAnalytics  = open("c:/temp/AddAnalysis.txt","w+")
Getcip() 
ItemAnalytics = open("c:/temp/ItemAnalysis.txt","w+")
StoreAnalytics= open("c:/temp/StoreAnalysis.txt","w+")
AnalyzeFurther()
end = time.time()
TotalTime = end - start 
ItemAnalytics.close()
StoreAnalytics.close()
 