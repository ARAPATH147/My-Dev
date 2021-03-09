###############################################################################
#                                                                             #
# RADAR                  RANJITH GOPALANKUTTY                MARCH-31-2019    #
# Radar, is a new program written to receive real time event data from all    #
# stores, process it, and update it in Dave-Base; so, this going to be a      #
# critical application. RADAR, will have 4 classes as below                   #
#         InitEverything = To initialize all files & parameters               #
#         AcceptStores   = Will accept connections from all stores            #  
#         ProcessData    = This class will have the data to process data      #
#                          received from all strores                          #
#         UpdateData     = This module will update the received data into     #
#                          database table                                     #
#                                                                             #
###############################################################################

import sys
from _socket import socket 
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
import socket
from   functools import partial
from   multiprocessing import Pool
from   multiprocessing.dummy import Pool as ThreadPool  


########################################################################
#     All Major variables,lists and declarations happens below         #
########################################################################
RadarConfig  = "c:/temp/RadarConfig.txt"
RadarError   = "c:/temp/RadarError.txt"
NumberOfSockets = 0
PortToBindTo = 0
host = ""
StoreIpList = []
ConnectionList = []
address = ""
ConnectionAccepted = 0
EventDataSize = 256

#########################################################################
#                  AcceptStores                                         #
# Below class is to, handle all connections coming asynchronously from  #
# clients at random times, so the loop will run for ever once started.  #
# Once connection is accepted, it will add an entry in to the list then #
# connection will be handled using a thread.                            #
#########################################################################
 
class AcceptStores:

    def AddConnectionDetailsToArray(ConnectionDetails):
        ConnectionList.append(address)

    def JoinStore():
        global host
        global address
        global ConnectionAccepted
        global EventDataSize
        print("Opening connection")             
        RadarSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        RadarSocket.bind((host, PortToBindTo))
        RadarSocket.listen(NumberOfSockets)
        # Ensure ports are not exhausted
        if ConnectionAccepted < NumberOfSockets:

            while True: 
                 
                connection, address = RadarSocket.accept()                    
                ConnectionAccepted = ConnectionAccepted + 1
                print("Connection from: " + str(address[0]) + "\n")
                #Now we need to add this connection details to an array to keep     
                IPAddress = address[0]           
                AcceptStores.AddConnectionDetailsToArray(IPAddress)
                while True:
                    EventData = connection.recv(EventDataSize).decode('Latin-1')
                    if len(EventData) > 0:
                        # DEC sometimes combine multiple strings together, So we need a method to 
                        # combine the exact strings and pass it accordingly.
                        print(EventData)
                        
        else:
            print("No more ports to connect")

########################################################################
#                     InitEverything                                   #
#     Class to initialize and get all configurations, so in future     #
#     any changes to configuration, text file can take care of it      #
########################################################################
class InitEverything:
    
    # Function to read configuration file and get details of port number 
    # and number of sockets.
    def ReadConfigFile():
        global RadarConfig
        global PortToBindTo
        global NumberOfSockets

        try:
            with open(RadarConfig,encoding="Latin-1") as RadarConfig:
                for line in RadarConfig:
                    if line[0:11] == "PORT_NUMBER":
                        PortToBindTo = int(line[14:18])
                    elif line[0:17] == "NUMBER_OF_SOCKETS":
                        NumberOfSockets = int(line[20:24])

        except Exception as e:
            RadarErrorString = '{c} - {m}'.format(c = type(e).__name__, m = str(e)) + "\n"
            print(RadarErrorString)

########################################################################
#           Mainline code starts from here                             #
########################################################################

# Program started, lets open the configuration file and initialize the 
# variables.

InitEverything.ReadConfigFile()
AcceptStores.JoinStore()

