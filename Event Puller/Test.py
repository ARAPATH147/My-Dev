import socketserver
import sys
import time
import datetime
import os
import glob 
from   pathlib import Path 
import re
import decimal
from threading import Thread
import struct
import socket
from   functools import partial
from   multiprocessing import Pool
from   multiprocessing.dummy import Pool as ThreadPool 
import socketserver

class MyTCPHandler(socketserver.BaseRequestHandler):
    """
    The request handler class for our server.

    It is instantiated once per connection to the server, and must
    override the handle() method to implement communication to the
    client.
    """

    def handle(self):
        # self.request is the TCP socket connected to the client
        while True:
            self.data = self.request.recv(1024).decode('Latin-1')
            if len(self.data) > 0:
                print("{} wrote:".format(self.client_address[0]))
                print(self.data)
        # just send back the same data, but upper-cased
        #self.request.sendall(self.data.upper())

if __name__ == "__main__":
    HOST, PORT = "localhost", 1887

    # Create the server, binding to localhost on port 9999
    with socketserver.TCPServer((HOST, PORT), MyTCPHandler) as server:
        # Activate the server; this will keep running until you
        # interrupt the program with Ctrl-C
        server.serve_forever()