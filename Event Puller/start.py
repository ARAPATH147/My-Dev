import socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('localhost', 1887))
#while True :    
str = "SI,01,CONNCTID,CE,5159,6,14:11:54:985,RADAR/|,20190307,0002,CE,0000,0024,W620,05,08,EAL"
s.send(str.encode())

#s.close()
 
