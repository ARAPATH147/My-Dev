import socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('10.230.3.59', 1887))
     
str = "SI,01,CONNCTID,CE,9999,6,14:11:54:985,RADAR/|,20190527,0002,CE,0000,0024,W,620,05,08,EALDO22L,5,29,0001\r\n"
s.send(str.encode())

s.close()

