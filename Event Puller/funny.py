import socket
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect(('localhost', 1887))
while True :    
    str = "**********************************************************1\r\n"
    s.send(str.encode())

#s.close()
 
