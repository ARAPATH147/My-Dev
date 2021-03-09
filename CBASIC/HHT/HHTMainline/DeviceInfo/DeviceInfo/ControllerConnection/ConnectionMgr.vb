Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Diagnostics
Imports System.Windows.Forms

'''****************************************************************************
''' <FileName> ConnectionManager.vb </FileName> 
''' <summary> Class to handle socket connections to TRANSACT on Controller.
''' </summary> 
''' <Version>1.0</Version> 
''' <Author>Andrew Paton</Author> 
''' <DateModified>11-05-2016</DateModified> 
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC55RF</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK </CopyRight>
'''****************************************************************************
'''* Modification Log 
'''**************************************************************************** 
'''  1.0    Andrew Paton                             11/05/2016        
'''         Inital Version.
''' 
'''**************************************************************************** 

Public Class ConnectionMgr
    Private Shared moConnectionManager As ConnectionMgr
    Private m_Socket As Socket
    Private miIPPort As Integer
    Private msReconnectMessage As String = ""
    Private id As Long = 0
    Private cActiveIP As String = Nothing

    Private Delegate Sub UpdateStatusCallback(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class ConnectionManager to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As ConnectionMgr
        If moConnectionManager Is Nothing Then
            moConnectionManager = New ConnectionMgr
        End If
        Return moConnectionManager

    End Function

    Private Class Request
        Public id As Integer
        Public buffer(1064) As Byte
        Public timestamp As Date

        Public Sub New(ByVal id As Integer)
            Me.id = id
            timestamp = Now()
        End Sub
    End Class

    Public Sub New()
        miIPPort = ConfigKey.IPPORT
        cActiveIP = ConfigFileManager.GetInstance.GetIPParam(ConfigKey.ACTIVE_IPADDRESS)

        If Connect(cActiveIP, miIPPort) Then
            oAppMain.bConnect = Connected()
        End If
    End Sub

    ''' <summary>
    ''' Function to try and establish a connection with the Controller/server
    ''' </summary>
    ''' <param name="address">String containing IP address xxx.xxx.xxx.27</param>
    ''' <param name="port">String containing Port No. eg. 800</param>
    ''' <returns>Boolean</returns>
    Public Function Connect(ByVal address As String, ByVal port As Integer) As Boolean
        ' Ensure the socket is disconnected
        Disconnect()

        ' Try and establish a connection with the server
        Try
            Dim ip As IPAddress = IPAddress.Parse(address)
            Dim endPoint As IPEndPoint = New IPEndPoint(ip, port)

            m_Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            m_Socket.Connect(endPoint) 'Sync

            If (m_Socket.Connected) Then
                oAppMain.bConnect = True
                Connect = True
            Else
                oAppMain.bConnect = False
                Connect = False
            End If
        Catch ex As Exception
            oAppMain.bConnect = False
            Connect = False
        End Try

    End Function

    ''' <summary>
    ''' Function to disconnect connection with the Controller/server
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function Disconnect() As Boolean
        Try
            ' Return if a socket does not exist
            If m_Socket Is Nothing Then
                Return True
            End If
            ' Return if socket has already been disconnected
            If m_Socket.Connected = True Then
                m_Socket.Shutdown(SocketShutdown.Both)
            End If
            m_Socket.Close()
            m_Socket = Nothing
            Return True
        Catch ex As Exception
            m_Socket = Nothing
            oAppMain.bConnect = False
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Returns true if the socket is connected to the server. The property
    ''' "Socket.Connected" does not always indicate if the socket is currently
    ''' connected, this polls the socket to determine the latest connection state.
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function Connected() As Boolean

        ' return right away if have not created socket
        If m_Socket Is Nothing Then
            Return False
        End If

        ' the socket is not connected if the Connected property is false
        If m_Socket.Connected = False Then
            Return False
        End If

        ' there is no guarantee that the socket is connected even if the
        ' Connected property is true
        Try
            Dim iTimeOut As Integer = 1000
            If m_Socket.Poll(iTimeOut, SelectMode.SelectWrite) Then
                If m_Socket.Poll(iTimeOut, SelectMode.SelectError) Then
                    Return False
                Else
                    Return True
                End If
            Else

                Return False
            End If

        Catch
            Return False
        End Try
        ' End Get
    End Function

    ''' <summary>
    ''' Function to send message to the Controller/server over the socket.
    ''' It prefixes the message with control character FFh followed by a
    ''' 4-digit total message length (includes FFh + 4-digits in total).
    ''' </summary>
    ''' <param name="sSend">String containing message to send to server</param>
    ''' <returns>Boolean</returns>
    Public Function Send(ByVal sSend As String) As Boolean

        sSend = Chr(255) + (sSend.Length + 5).ToString().PadLeft(4, "0") + sSend
        Dim data() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(sSend)
        data(0) = &HFF
        Dim bytesSent As Integer
        Try
            bytesSent = m_Socket.Send(data, 0, data.Length, SocketFlags.None)

            If bytesSent > 0 Then
                Send = True
            Else
                Send = False
            End If
        Catch ex As Exception

            Send = False
        End Try
    End Function

    ''' <summary>
    ''' Function to receive message from the Controller/server over the socket.
    ''' </summary>
    ''' <param name="receivedString">String containing response from server</param>
    ''' <returns>Boolean</returns>
    Public Function Receive(ByRef receivedString As String) As Boolean
        If Not Connected() Then
            Return False
        End If

        Try
            id = (id + 1) Mod &H4  ' Allocate unique session ID (start small to test wrap)
            Dim req As Request = New Request(id)

            Try
                m_Socket.Receive(req.buffer, 0, req.buffer.Length, SocketFlags.None)
            Catch ex As Exception
                Receive = False
                Exit Function
            End Try

            receivedString = System.Text.Encoding.ASCII.GetString(req.buffer, 0, req.buffer.Length)
            Dim RLength As Integer = receivedString.Length
            receivedString = receivedString.Substring(5, receivedString.Length - 5)

            If receivedString.Length > 0 Then
                Try
                    Receive = True
                Catch ex As Exception
                    If Err.Number <> 91 Then
                        'MessageBox.Show("RaiseEvent Notify Failure: " & ex.Message, _
                        '"Comms Failure:RECEIVE", _
                        ' MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                        'MessageBoxDefaultButton.Button1)
                    End If
                End Try
            Else
                Receive = False
            End If

        Catch ex As Exception
            'MessageBox.Show(Err.Description & Erl.ToString & Err.Source)
            Receive = False
        End Try
    End Function

    Public Sub CloseSession()
        moConnectionManager = Nothing
    End Sub
End Class
