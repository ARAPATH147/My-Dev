###############################################################################
#                                                                             #
#  RADAR                  RANJITH GOPALANKUTTY                MARCH-31-2019   #
#  Radar, is a new program written to receive real time event data from all   #
#  stores, process it, and update it in Dave-Base; so, this going to be a     #
#  critical application. RADAR, will have 3 classes as below                  #
#         InitEverything = To initialize all files & parameters               #
#         AcceptStores   = Will accept connections from all stores            #
#         ProcessData    = This module will update the received data into     #
#                          database table                                     #
#                                                                             #
###############################################################################

import sys
import time
import datetime
import os
import glob 
from   pathlib import Path 
import re
import decimal
from   threading import *
import struct
import socket
from   functools import partial
from   multiprocessing import Pool
from   multiprocessing.dummy import Pool as ThreadPool 
import socketserver
import select
import queue
from   multiprocessing import Process
import pyodbc

#########################################################################
#     All Major variables,lists and declarations happens below          #
#########################################################################
RadarConfig        = "c:/temp/RadarConfig.txt"
RadarError         = "c:/temp/RadarError.txt"
NumberOfSockets    = 0
PortToBindTo       = 0
host               = ""
StoreIpList        = []
ConnectionList     = []
address            = ""
ConnectionAccepted = 0
EventDataSize      = 1024 
MasterQueue        = queue.Queue()
StoreNumber        = '0000'        
NodeID             = "XX"    
EventDate          = '2020/01/01' 
EventTime          = '0000' 
EventTerminal      = '0000' 
EventSource        = '000' 
EventGroup         = "X" 
EventMessage       = '000'
EventSeverity      = '00' 
EventNumber        = '00' 
EventBucket        = '00'  
EventFormat        = '00'
EventCount         = '0000'
cursor             = ""

##########################################################################
#                     ProcessData                                        #
#          Below class and methods are to split the data                 #
##########################################################################

class ProcessData:

    def InsertData():
        global StoreNumber         
        global NodeID      
        global EventDate      
        global EventTime      
        global EventTerminal  
        global EventSource    
        global EventGroup     
        global EventMessage  
        global EventSeverity  
        global EventNumber    
        global EventBucket     
        global EventFormat   
        global EventCount
        
        try:
            global cursor
            SqlCommand = 'INSERT INTO APPS_EPOS_POC.dbo.RealTimeEvent VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)'  
            FieldValues =  [StoreNumber,NodeID,EventDate,EventTime,EventTerminal,EventSource,EventGroup,  \
                           EventMessage,EventSeverity,EventNumber,EventBucket,EventFormat,EventCount]      
            cursor.execute(SqlCommand,FieldValues) 
            cursor.commit()
        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)

    def SplitData(EventData):
        global StoreNumber         
        global NodeID      
        global EventDate      
        global EventTime      
        global EventTerminal  
        global EventSource    
        global EventGroup     
        global EventMessage  
        global EventSeverity  
        global EventNumber    
        global EventBucket     
        global EventFormat   
        global EventCount    
        # Split the data from the string and get each column value
        # This will be passed as row to the table update function

        try:
            SplitEvent      = EventData.split(",")
            StoreNumber     = SplitEvent[4]      
            NodeID          = SplitEvent[10] 
            EventDate       = SplitEvent[8]
            EventDate       = EventDate[0:4] + "/" + EventDate[4:6] + "/" + EventDate[6:8]    # To get the right date format
            EventDate       = datetime.datetime.strptime(EventDate, "%Y/%m/%d")               # To convert to a date object
            EventTime       = SplitEvent[9]            
            EventTime       = EventTime[0:2] + ":" + EventTime[2:4]                           # To match with table structure
            EventTerminal   = SplitEvent[11]
            EventSource     = SplitEvent[12]
            EventGroup      = SplitEvent[13]
            EventMessage    = SplitEvent[14]
            EventSeverity   = SplitEvent[15]
            EventNumber     = SplitEvent[16]
            EventBucket     = SplitEvent[18]
            EventFormat     = SplitEvent[19]
            EventCount      = SplitEvent[20][:4]
            ProcessData.InsertData()
         
        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)
##########################################################################
#                  AcceptStores                                          #
# Below class is to, handle all connections coming asynchronously from   #
# clients at random times, so the loop will run for ever once started.   #
# Once connection is accepted, it will add an entry in to the list then  #
# connection will be handled using a thread.                             #
##########################################################################    
             
class AcceptStores:

    def StreamData(CurrentThread,ThreadName):
        global ConnectionAccepted

        try:
            print("Listening in " + ThreadName)
            while True:    
                EventData = CurrentThread.recv(EventDataSize).decode('Latin-1') 
                if len(EventData) > 0:                                           
                    print(EventData)
                    ProcessData.SplitData(EventData)
                else:
                    break
            CurrentThread.close()
            ConnectionAccepted = ConnectionAccepted - 1
        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)
    def AddConnectionDetailsToArray(ConnectionDetails):
        ConnectionList.append(address)

    def JoinStore():
        global host
        global address
        global ConnectionAccepted
        global EventDataSize
        global MessageStarter
        global MessageEnder
        global CompleteString
        global LetterCounter
        global FullRecord
        global PartialRecord
        global StartRecord
        global EndRecord
        
        try:
            print("RADAR started, scanning and accepting connections :")             
            RadarSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            RadarSocket.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR,1)
            RadarSocket.bind((host, PortToBindTo))
            RadarSocket.listen(NumberOfSockets)
            # Ensure ports are not exhausted
       
            if ConnectionAccepted < NumberOfSockets:
                while True: 
                    try:
                        connection, address = RadarSocket.accept()
                        #RadarSocket.setblocking(0)              # Prevents time outs
                        StoreIpList.append(address)
                        ConnectionList.append(connection)
                        ConnectionAccepted = ConnectionAccepted + 1
                        print("Accepted connection from IP: " + str(address[0]) + "\n")
                        testfile.writelines("Accepted connection from IP: " + str(address[0]) + "\n")
                        ThreadName = "Newthread " + str(ConnectionAccepted)
                        p = Process(target = AcceptStores.StreamData(connection,ThreadName))
                        p.start()
                        p.join()

                    #Now we need to add this connection details to an array to keep     
                        IPAddress = address[0]           
                        AcceptStores.AddConnectionDetailsToArray(IPAddress)
                    except Exception as e:
                        RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
                        print(RadarErrorString)                                                                                                            
            else:
                print("No more ports to connect")

        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)
#########################################################################
#                     InitEverything                                    #
#     Class to initialize and get all configurations, so in future      #
#     any changes to configuration, text file can take care of it       #
#########################################################################
class InitEverything:
    
    # Function to read configuration file and get details of port number 
    # and number of sockets.
    def ReadConfigFile():
        global RadarConfig
        global PortToBindTo
        global NumberOfSockets
        global MessageStarter      

        try:
            with open(RadarConfig,encoding="Latin-1") as RadarConfig:
                for line in RadarConfig:
                    if line[0:11] == "PORT_NUMBER":
                        PortToBindTo = int(line[14:18])
                    elif line[0:17] == "NUMBER_OF_SOCKETS":
                        NumberOfSockets = int(line[20:24])
                    elif line[0:15] == "MESSAGE_STARTER":
                        MessageStarter = line[18:20]                 
                        
        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)
   #Establish connection with the database
    def ConnectDb():
        global cursor

        try:
            conn = pyodbc.connect('Driver={SQL Server};'
                                  'Server=UKC1CENTPV;'
                                  'Database=APPS_EPOS_POC;'
                                  
                                  'Trusted_Connection=yes;')

            cursor = conn.cursor()
        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)
         
#########################################################################
#           Mainline code starts from here                              #
#########################################################################

# Program started, lets open the configuration file and initialize the 
# variables.
testfile = open("c:/temp/test.txt","w+")
InitEverything.ReadConfigFile()
InitEverything.ConnectDb()
AcceptStores.JoinStore()

