Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Collections.Generic
Imports System.Text
Namespace TFTPClient
    ''' <summary>
    ''' Class to set and collect the packet count.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PacketMonitor
        Private m_iPacketCount As Integer
        Private Shared m_instance As PacketMonitor = Nothing
        Public Sub New()
            m_iPacketCount = 0
        End Sub
        Public Shared Function GetInstance() As PacketMonitor
            If m_instance Is Nothing Then
                m_instance = New PacketMonitor()
            End If
            Return m_instance
        End Function
        Public Property PacketCounter() As Integer
            Get
                Return m_iPacketCount
            End Get
            Set(ByVal value As Integer)
                m_iPacketCount = value
            End Set
        End Property
    End Class
    Partial Public Class TFTPSession
        Private Delegate Function CloseSocketCallBack()
        Private m_Control As New Control
        Private m_PacketCounter As Integer = 0
        Private UDPSock As Socket = Nothing
        Private BWriter As BinaryWriter = Nothing
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
        Public Function [Get](ByVal LocalFile As String, ByVal RemoteFile As String, ByVal Host As String, ByVal Mode As Modes, ByVal BlockSize As Integer, ByVal Timeout As Integer, Optional ByVal Port As Integer = 69) As Boolean
            Dim recvLen As Integer, remoteFileSize As Integer = 0, buffer As Integer = BlockSize + 4
            Dim bytesReceived As Long = 0

            Dim opCode As New TFTPSession.OpCodes()
            Dim ipAddr As System.Net.IPAddress = IPAddress.Parse(Host)
            'Dim hInfo As IPHostEntry = Dns.GetHostEntry(Host)
            'Dim address As IPAddress = hInfo.AddressList(0)
            Dim remoteEP As New IPEndPoint(ipAddr, Port)
            Dim localEP As EndPoint = (remoteEP)

            BWriter = New BinaryWriter(File.Open(LocalFile, FileMode.Create))
            UDPSock = New Socket(remoteEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp)

            ' Create initial request and buffer for response
            Dim sendData As Byte() = _packetBuilder.Request(TFTPSession.OpCodes.RRQ, RemoteFile, Mode, BlockSize, 0, Timeout)
            'modified----------
            'Dim recvData As Byte() = New Byte(BlockSize + 4) {}
            Dim recvData As Byte() = New Byte(BlockSize + 3) {}

            'UDPSock.ReceiveTimeout = Timeout * 1000;

            ' Send request and wait for response
            UDPSock.SendTo(sendData, remoteEP)
            recvLen = UDPSock.ReceiveFrom(recvData, localEP)

            ' Get TID
            remoteEP.Port = (DirectCast(localEP, IPEndPoint)).Port

            ' Invoke connected event
            'Connected.Invoke();

            While True
                ' Read opcode
                opCode = _packetReader.ReadOpCode(recvData)

                ' DATA packet
                If opCode = TFTPSession.OpCodes.DATA Then
                    bytesReceived += recvLen - 4



                    ' Invoke Transferring Event
                    'Transferring.Invoke(bytesReceived, remoteFileSize);

                    Dim h As Integer = 4
                    While h < recvLen
                        BWriter.Write(recvData(h))
                        h += 1
                    End While

                    sendData = _packetBuilder.Ack(recvData(2), recvData(3))

                    ' Check if this packet is the last
                    If recvLen < buffer Then
                        ' Send final ACK
                        UDPSock.SendTo(sendData, remoteEP)

                        ' Invoked TransferFinished Event
                        'TransferFinished.Invoke();

                        Exit While
                    End If
                ElseIf opCode = TFTPSession.OpCodes.OACK Then

                    ' OACK packet
                    remoteFileSize = _packetReader.ReadTransferSize(recvData)
                    sendData = _packetBuilder.Ack(0, 0)
                ElseIf opCode = TFTPSession.OpCodes.[ERROR] Then

                    ' ERROR packet
                    Dim transferError As ErrorPacket = _packetReader.ReadError(recvData)
                    'TransferFailed.Invoke(transferError.Code, transferError.Message);

                    Exit While
                End If
                'increment the packet counter
                m_PacketCounter = m_PacketCounter + 1
                PacketMonitor.GetInstance().PacketCounter = m_PacketCounter
                ' Send next packet
                UDPSock.SendTo(sendData, remoteEP)
                recvLen = UDPSock.ReceiveFrom(recvData, localEP)
                remoteEP.Port = (DirectCast(localEP, IPEndPoint)).Port
            End While

            BWriter.Close()
            UDPSock.Close()

            ' Invoke Disconnected Event
            'Disconnected.Invoke();
            Return True
        End Function

        Public Function [Get](ByVal TransferOptions As Object) As Boolean
            Dim tOptions As TransferOptions = DirectCast(TransferOptions, TransferOptions)
            Return [Get](tOptions.LocalFilename, tOptions.RemoteFilename, tOptions.Host, Mode, BlockSize, Timeout, tOptions.Port)
        End Function

        Public Function [Get](ByVal File As String) As Boolean
            Return [Get](File, File, Host, Mode, BlockSize, Timeout)
        End Function

        Public Function [Get](ByVal File As String, ByVal Host As String) As Boolean
            Return [Get](File, File, Host, Mode, BlockSize, Timeout)
        End Function

        Public Function [Get](ByVal LocalFile As String, ByVal RemoteFile As String, ByVal Host As String) As Boolean
            Return [Get](LocalFile, RemoteFile, Host, Mode, BlockSize, Timeout)
        End Function
        ''' <summary>
        ''' Function to close UDP Socket and the Binary Write.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CloseSocket() As Boolean
            Try
                If Not m_Control.InvokeRequired Then
                    UDPSock.Close()
                    BWriter.Close()
                    Return True
                Else
                    m_Control.Invoke(New CloseSocketCallBack(AddressOf CloseSocket))
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("tFTP Get: " + ex.Message, _
                                                      Logger.LogLevel.RELEASE)
            End Try
        End Function
    End Class
End Namespace


