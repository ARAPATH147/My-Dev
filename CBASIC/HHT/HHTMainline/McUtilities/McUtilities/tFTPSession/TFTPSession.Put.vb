Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Collections.Generic
Imports System.Text

Namespace TFTPClient
    Partial Public Class TFTPSession
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="LocalFile"></param>
        ''' <param name="RemoteFile"></param>
        ''' <param name="Host"></param>
        ''' <param name="Mode"></param>
        ''' <param name="BlockSize"></param>
        ''' <param name="Timeout"></param>
        ''' <param name="Port"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Put(ByVal LocalFile As String, ByVal RemoteFile As String, ByVal Host As String, ByVal Mode As Modes, ByVal BlockSize As Integer, ByVal Timeout As Integer, Optional ByVal Port As Integer = 69) As Boolean
            Dim block As Integer() = New Integer(2) {}
            Dim bufferSize As Integer = BlockSize
            Dim fileSize As Long, bytesSent As Long = 0

            Dim BReader As New BinaryReader(File.Open(LocalFile, FileMode.Open))
            Dim sendFile As New FileInfo(LocalFile)
            Dim ipAddr As System.Net.IPAddress = IPAddress.Parse(Host)
            Dim opCode As New TFTPSession.OpCodes()

            'Dim hostInfo As IPHostEntry = Dns.GetHostEntry(Host)
            'Dim address As IPAddress = hostInfo.AddressList(0)
            Dim remoteEP As New IPEndPoint(ipAddr, Port)
            Dim localEP As EndPoint = (remoteEP)
            Dim UDPSock As New Socket(remoteEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp)

            ' Retrieve filesize for tsize option
            fileSize = sendFile.Length
            'Set packet counter to 0
            m_PacketCounter = 0

            ' Create initial request and buffer for response
            Dim sendData As Byte() = _packetBuilder.Request(TFTPSession.OpCodes.WRQ, RemoteFile, Mode, BlockSize, fileSize, Timeout)
            Dim recvData As Byte() = New Byte(bufferSize - 1) {}

            'UDPSock.ReceiveTimeout = Timeout * 1000;

            ' Send request and wait for response
            UDPSock.SendTo(sendData, remoteEP)
            UDPSock.ReceiveFrom(recvData, localEP)

            'Get TID
            remoteEP.Port = (CType(localEP, IPEndPoint)).Port

            ' Invoke Connected Event
            'Connected.Invoke();

            While True
                ' Read opcode
                opCode = _packetReader.ReadOpCode(recvData)

                ' ACK packet
                If opCode = TFTPSession.OpCodes.ACK Then
                    block = _packetBuilder.IncrementBock(recvData, block)

                    sendData = BReader.ReadBytes(bufferSize)
                    bytesSent += sendData.Length

                    ' Invoke Transferring Event
                    'Transferring.Invoke(bytesSent, fileSize);

                    sendData = _packetBuilder.Data(sendData, block(0), block(1))

                    ' Check if this packet is the last
                    If sendData.Length < bufferSize + 4 Then
                        ' Send final data packet and wait for ack
                        While True
                            UDPSock.SendTo(sendData, remoteEP)
                            UDPSock.ReceiveFrom(recvData, localEP)
                            remoteEP.Port = (DirectCast(localEP, IPEndPoint)).Port

                            ' Check the blocks and break free if equal
                            If _packetReader.CompareBlocks(sendData, recvData) Then
                                Exit While
                            End If
                        End While

                        ' Invoke TransferFinished Event
                        'TransferFinished.Invoke();
                        Exit While
                    End If
                ElseIf opCode = TFTPSession.OpCodes.OACK Then

                    ' OACK packet
                    sendData = BReader.ReadBytes(bufferSize)
                    sendData = _packetBuilder.Data(sendData, 0, 1)
                    bytesSent += sendData.Length - 4

                    ' Invoke Transferring Event
                    'Transferring.Invoke(bytesSent, fileSize);

                    If fileSize = 0 Then
                        ' Invoke TransferFinished Event
                        'TransferFinished.Invoke();
                        Exit While
                    Else

                        ' Check if this packet is the last
                        If sendData.Length < bufferSize + 4 Then
                            ' Send final data packet and wait for ack
                            While True
                                UDPSock.SendTo(sendData, remoteEP)
                                UDPSock.ReceiveFrom(recvData, localEP)
                                remoteEP.Port = (DirectCast(localEP, IPEndPoint)).Port

                                ' Check the blocks and break free if equal
                                If _packetReader.CompareBlocks(sendData, recvData) Then
                                    Exit While
                                End If
                            End While
                            ' Invoke TransferFinished Event
                            'TransferFinished.Invoke();
                            Exit While
                        End If
                    End If
                ElseIf opCode = TFTPSession.OpCodes.[ERROR] Then
                    Dim transferError As ErrorPacket = _packetReader.ReadError(recvData)
                    'TransferFailed.Invoke(transferError.Code, transferError.Message);
                    Exit While
                End If
                'increment the packet counter
                m_PacketCounter = m_PacketCounter + 1
                PacketMonitor.GetInstance().PacketCounter = m_PacketCounter

                ' Send next packet
                UDPSock.SendTo(sendData, remoteEP)
                UDPSock.ReceiveFrom(recvData, localEP)
                remoteEP.Port = (DirectCast(localEP, IPEndPoint)).Port
            End While
            BReader.Close()
            UDPSock.Close()

            ' Invoke Disconnected Event
            'Disconnected.Invoke();

            Return True
        End Function

#Region "Put Overloads"
        Public Function Put(ByVal TransferOptions As Object) As Boolean
            Dim tOptions As TransferOptions = DirectCast(TransferOptions, TransferOptions)
            Return Put(tOptions.LocalFilename, tOptions.RemoteFilename, tOptions.Host, Mode, BlockSize, Timeout, tOptions.Port)
        End Function

        Public Function Put(ByVal File As String) As Boolean
            Return Put(File, File, Host, Mode, BlockSize, Timeout)
        End Function

        Public Function Put(ByVal File As String, ByVal Host As String) As Boolean
            Return Put(File, File, Host, Mode, BlockSize, Timeout)
        End Function

        Public Function Put(ByVal LocalFile As String, ByVal RemoteFile As String, ByVal Host As String) As Boolean
            Return Put(LocalFile, RemoteFile, Host, Mode, BlockSize, Timeout)
        End Function
#End Region
    End Class
End Namespace


