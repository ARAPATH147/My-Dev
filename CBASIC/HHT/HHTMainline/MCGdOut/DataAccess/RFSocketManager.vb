#If RF Then
Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class RFSocketManager

#Region "Delegates"
    Public Delegate Sub ConnectionDelegate(ByVal soc As Socket)
    Public Delegate Sub ErrorDelegate(ByVal ErroMessage As String, ByVal soc As Socket, ByVal ErroCode As Integer)
#End Region

#Region "Events"
    Public Event OnConnect As ConnectionDelegate
    Public Event OnDisconnect As ConnectionDelegate
    Public Event OnReceiveTimeout As ConnectionDelegate
    Public Event OnRead As ConnectionDelegate
    Public Event OnWrite As ConnectionDelegate
    Public Event OnError As ErrorDelegate
    Public Event OnSendFile As ConnectionDelegate
#End Region

#Region "Variables"
    Private WorkerCallBack As AsyncCallback
    Private mainSocket As Socket
    Private serverEndPoint As IPEndPoint
    Private dataBuffer As Byte() = New Byte(1024) {}
    Private mPort As Integer = 0
    Private mBytesReceived As Byte()
    Private mTextReceived As String = ""
    Private mTextSent As String = ""
    Private mRemoteAddress As String = ""
    Private mRemoteHost As String = ""
    'Private mwaitTimer As System.Windows.Forms.Timer
    Friend WithEvents mwaitTimer As System.Windows.Forms.Timer
    'Private mReconnectInterval As Integer = Macros.TIME_OUT_DURATION

    Dim m_HostName As String = Nothing
    Dim m_Port As String = Nothing
    Dim m_RecBufferSize As Integer = Macros.RECEIVE_BUFFER
    Dim m_CurrentMessageTimeStamp As Date
    Private m_awaitingDataStamp As Boolean = False
    Dim m_TimeOutDuration As Integer
    Dim m_MessageID As String
    Public Property MESSAGEID() As String
        Get
            ' SyncLock m_MessageID
            Return m_MessageID
            'End SyncLock
        End Get
        Set(ByVal value As String)
            'SyncLock m_MessageID
            m_MessageID = value
            ' End SyncLock
        End Set
    End Property
#End Region

#Region "Propetiers"
    '' <summary>
    '' Interval to reconnect
    '' </summary>
    'Public WriteOnly Property ReconnectInterval() As Integer
    '    Set(ByVal value As Integer)
    '        mReconnectInterval = value
    '    End Set
    'End Property
    ''' <summary>
    ''' Port to connect to server
    ''' </summary>
    Public ReadOnly Property Port() As Integer
        Get
            Return (mPort)
        End Get
    End Property

    ''' <summary>
    ''' Bytes received by the Socket
    ''' </summary>
    Public ReadOnly Property ReceivedBytes() As Byte()
        Get
            Dim temp As Byte() = Nothing
            If mBytesReceived IsNot Nothing Then
                temp = mBytesReceived
                mBytesReceived = Nothing
            End If
            Return (temp)
        End Get
    End Property

    ''' <summary>
    ''' Message received by the Socket
    ''' </summary>
    Public ReadOnly Property ReceivedText() As String
        Get
            Dim temp As String = mTextReceived
            mTextReceived = ""
            Return (temp)
        End Get
    End Property

    ''' <summary>
    ''' Message send by the Socket
    ''' </summary>
    Public ReadOnly Property WriteText() As String
        Get
            Dim temp As String = mTextSent
            mTextSent = ""
            Return (temp)
        End Get
    End Property

    ''' <summary>
    ''' IP Server
    ''' </summary>
    Public ReadOnly Property RemoteAddress() As String
        Get
            If mainSocket.Connected Then
                Return (mRemoteAddress)
            Else
                Return ""
            End If
        End Get
    End Property

    ''' <summary>
    ''' Host Server
    ''' </summary>
    Public ReadOnly Property RemoteHost() As String
        Get
            If mainSocket.Connected Then
                Return (mRemoteHost)
            Else
                Return ""
            End If
        End Get
    End Property

    ''' <summary>
    ''' Return true if the ClientSocket is connected to the Server
    ''' </summary>
    Public ReadOnly Property Connected() As Boolean
        Get
            Return (mainSocket.Connected)
        End Get
    End Property
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Default Constructor
    ''' </summary>
    ''' <param name="port">Port to connection
    ''' </param>
    Public Sub New(ByVal IP As String, ByVal port As Integer)
        Try
            mPort = port
            Dim ipAddress As IPAddress = ipAddress.Parse(IP)
            mRemoteAddress = ipAddress.ToString()
            'Dim ipss As IPHostEntry = Dns.GetHostEntry(mRemoteAddress)
            'mRemoteHost = ipss.HostName
            serverEndPoint = New IPEndPoint(ipAddress, port)
            mainSocket = New Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            m_TimeOutDuration = Convert.ToInt16(ConfigDataMgr.GetInstance.GetParam(ConfigKey.TIME_OUT_DURATION).ToString())
        Catch ex As Exception
            'If OnError IsNot Nothing Then
            RaiseEvent OnError(ex.Message, Nothing, 0)
            ''End If
        End Try
    End Sub
    Public Sub New()
        Try
            mPort = Port
            Dim ipAddress As IPAddress = ipAddress.Parse(m_HostName)
            mRemoteAddress = ipAddress.ToString()
            'Dim ipss As IPHostEntry = Dns.GetHostEntry(mRemoteAddress)
            'mRemoteHost = ipss.HostName
            serverEndPoint = New IPEndPoint(ipAddress, m_Port)
            mainSocket = New Socket(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        Catch ex As Exception
            'If OnError IsNot Nothing Then
            RaiseEvent OnError(ex.Message, Nothing, 0)
            ''End If
        End Try
    End Sub
#End Region

#Region "Functions and Events"
    Private Function GetConnectionDetails() As Boolean
        'm_HostName = ConfigDataMgr.GetInstance().GetParam( _
        '                                        ConfigKey.SERVER_IPADDRESS).ToString()
        m_HostName = objAppContainer.strActiveIP
        m_Port = ConfigDataMgr.GetInstance().GetParam( _
                                                ConfigKey.IPPORT).ToString()
    End Function

    ''' <summary>
    ''' Establishes connection with the IP and Port Server
    ''' </summary>
    Public Function Connect() As Boolean
        Try
            'Connect to Server
            mwaitTimer = New Timer()
            mwaitTimer.Interval = m_TimeOutDuration
            mainSocket.BeginConnect(serverEndPoint, New AsyncCallback(AddressOf ConfirmConnect), Nothing)
            Return True
        Catch ex As ArgumentException
            'If OnError <> Nothing Then
            RaiseEvent OnError(ex.Message, Nothing, 0)
            'End If
            Return False
        Catch ex As InvalidOperationException
            'If OnError <> Nothing Then
            RaiseEvent OnError(ex.Message, Nothing, 0)
            'End If
            Return False
        Catch se As SocketException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, mainSocket, se.ErrorCode)
            'End If
            Return False
        End Try
    End Function

    Private Sub ConfirmConnect(ByVal asyn As IAsyncResult)
        Try
            mainSocket.EndConnect(asyn)
            WaitForData(mainSocket)
            'If OnConnect <> Nothing Then
            RaiseEvent OnConnect(mainSocket)
            'End If
        Catch se As ObjectDisposedException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, Nothing, 0)
            'End If
        Catch se As SocketException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, Nothing, 0)
            'End If
        End Try
    End Sub

    Private Sub WaitForData(ByVal soc As Socket)
        Try
            'setting the priority as lowest that timer may get the priority
            Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
            If WorkerCallBack = Nothing Then
                WorkerCallBack = New AsyncCallback(AddressOf OnDataReceived)
            End If
            soc.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, WorkerCallBack, Nothing)
        Catch se As SocketException
            'If OnError <> Nothing Then
            'Raising event error. So stop the timer
            mwaitTimer.Enabled = False
            RaiseEvent OnError(se.Message, soc, se.ErrorCode)
            'End If
        End Try
    End Sub

    Private Sub OnDataReceived(ByVal asyn As IAsyncResult)
        Try
            Threading.Thread.Sleep(200)
            If m_awaitingDataStamp Then
                m_awaitingDataStamp = False
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
                Dim iRx As Integer = mainSocket.EndReceive(asyn)
                If iRx < 1 Then
                    mainSocket.Close()
                    If Not mainSocket.Connected Then
                        'If OnDisconnect <> Nothing Then
                        RaiseEvent OnDisconnect(mainSocket)
                        'End If
                    End If
                Else
                    objAppContainer.objLogger.WriteAppLog("Data Recieved @ socket and Timerr disabled", Logger.LogLevel.RELEASE)
                    mBytesReceived = dataBuffer
                    Dim chars As Char() = New Char(iRx + 1) {}
                    Dim d As Decoder = Encoding.UTF8.GetDecoder()
                    d.GetChars(dataBuffer, 0, iRx, chars, 0)
                    mTextReceived = New [String](chars)
                    'If OnRead <> Nothing Then
                    mwaitTimer.Enabled = False
                    RaiseEvent OnRead(mainSocket)
                    'End If
                    WaitForData(mainSocket)
                End If
            End If
        Catch se As ArgumentException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, Nothing, 0)
            'End If
        Catch se As InvalidOperationException
            mainSocket.Close()
            If Not mainSocket.Connected Then
                'If OnDisconnect <> Nothing Then
                RaiseEvent OnDisconnect(mainSocket)
                'ElseIf OnError <> Nothing Then
            Else
                RaiseEvent OnError(se.Message, Nothing, 0)
                'End If
            End If
        Catch se As SocketException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, mainSocket, se.ErrorCode)
            'End If
            If Not mainSocket.Connected Then
                'If OnDisconnect <> Nothing Then
                RaiseEvent OnDisconnect(mainSocket)
                'End If
            Else
                RaiseEvent OnError(se.Message, Nothing, 0)
            End If
        End Try
    End Sub

    ''' <summary>
    ''' Send a text message
    ''' </summary>
    ''' <param name="mens">Message</param>
    Public Function SendText(ByVal mens As String) As Boolean
        Try
            'new message format
            'Get Message ID 
            MESSAGEID = mens
            mens = Chr(255) + (mens.Length + 5).ToString().PadLeft(4, "0") + mens
            Dim byData As Byte() = System.Text.Encoding.ASCII.GetBytes(mens)
            byData(0) = &HFF
            Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
            Dim NumBytes As Integer = mainSocket.Send(byData)
            m_CurrentMessageTimeStamp = DateAndTime.Now
            If NumBytes = byData.Length Then
                'If OnWrite <> Nothing Then
                mTextSent = mens
                RaiseEvent OnWrite(mainSocket)
                m_awaitingDataStamp = True
                ' mwaitTimer.Enabled = True
                Dim myThread As System.Threading.Thread
                myThread = New System.Threading.Thread(DirectCast(Function() CheckForState(MESSAGEID, m_CurrentMessageTimeStamp), Threading.ThreadStart))
                myThread.Priority = Threading.ThreadPriority.AboveNormal
                myThread.Start()
                'End If
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
                Return True
            Else
                Return False
            End If
        Catch se As ArgumentException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, Nothing, 0)
            'End If
            Return False
        Catch se As ObjectDisposedException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, Nothing, 0)
            'End If
            Return False
        Catch se As SocketException
            'If OnError <> Nothing Then
            RaiseEvent OnError(se.Message, mainSocket, se.ErrorCode)
            'End If
            Return False
        End Try
    End Function
    Public Function CheckForState(ByVal strMessageID As String, ByVal MessageSentTimeStamp As Date) As Boolean
        Dim currentTime As Date = DateAndTime.Now
        objAppContainer.objLogger.WriteAppLog("Started Time Out Monitoring, Start Time: " + _
                                                          MessageSentTimeStamp, _
                                                          Logger.LogLevel.RELEASE)
        'checking the time stamp of the message to avoid inconsistancy
        'if the current message time stamp and thread creation time stamp is same then keep the time out check live
        While (m_awaitingDataStamp) And (Date.Compare(MessageSentTimeStamp, m_CurrentMessageTimeStamp) = 0)
            If (Date.Compare(MessageSentTimeStamp.AddSeconds(m_TimeOutDuration), currentTime) < 0) Then

                'Check Whether Message ID is similar
                If (strMessageID.Equals(MESSAGEID)) Then
                    objAppContainer.objLogger.WriteAppLog("Time Out Reached while sending the Message " + _
                                                          MESSAGEID, _
                                                          Logger.LogLevel.RELEASE)
                    'If timeout is reached then don't read the data coming in 
                    'before sending the next transact request.
                    m_awaitingDataStamp = False
                    RaiseEvent OnReceiveTimeout(mainSocket)
                End If
                Exit While
            End If
            objAppContainer.objLogger.WriteAppLog("Time Out Not reached, Expected Time: " + _
                                                      MessageSentTimeStamp.AddSeconds(m_TimeOutDuration).ToString(), _
                                                      Logger.LogLevel.RELEASE)
            Threading.Thread.Sleep(1000)
            Application.DoEvents()
            currentTime = DateAndTime.Now
        End While
        Return True
    End Function
    ''' <summary>
    ''' Close connection to the server
    ''' </summary>
    Public Function Disconnect() As Boolean
        mainSocket.Close()
        mwaitTimer.Dispose()
        If Not mainSocket.Connected Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub waitTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mwaitTimer.Tick
        objAppContainer.objLogger.WriteAppLog("Recieve Time Out! Generating the event TimeOut", Logger.LogLevel.RELEASE)
        mwaitTimer.Enabled = False
        RaiseEvent OnReceiveTimeout(mainSocket)
    End Sub

    '''' <summary>
    '''' Send file
    '''' </summary>
    '''' <param name="FileName">Path File</param>
    'Public Function SendFile(ByVal FileName As String) As Boolean
    '    Try
    '        mainSocket.BeginSendFile(FileName, New AsyncCallback(FileSendCallback), mainSocket)
    '        Return True
    '    Catch se As FileNotFoundException
    '        If OnError <> Nothing Then
    '            RaiseEvent OnError(se.Message, Nothing, 0)
    '        End If
    '        Return False
    '    Catch se As ObjectDisposedException
    '        If OnError <> Nothing Then
    '            RaiseEvent OnError(se.Message, Nothing, 0)
    '        End If
    '        Return False
    '    Catch se As SocketException
    '        If OnError <> Nothing Then
    '            RaiseEvent OnError(se.Message, mainSocket, se.ErrorCode)
    '        End If
    '        Return False
    '    End Try
    'End Function

    '''' <summary>
    '''' Send file
    '''' </summary>
    '''' <param name="FileName">Path File</param>
    '''' <param name="PreString">Message sent before the file</param>
    '''' <param name="PosString">Message sent after the File</param>
    'Public Function SendFile(ByVal FileName As String, ByVal PreString As String, ByVal PosString As String) As Boolean
    '    Try
    '        Dim preBuf As Byte() = Encoding.UTF8.GetBytes(PreString)
    '        Dim postBuf As Byte() = Encoding.UTF8.GetBytes(PosString)
    '        mainSocket.BeginSendFile(FileName, preBuf, postBuf, 0, New AsyncCallback(FileSendCallback), mainSocket)
    '        Return True
    '    Catch se As ArgumentException
    '        If OnError <> Nothing Then
    '            RaiseEvent OnError(se.Message, Nothing, 0)
    '        End If
    '        Return False
    '    Catch se As ObjectDisposedException
    '        If OnError <> Nothing Then
    '            RaiseEvent OnError(se.Message, Nothing, 0)
    '        End If
    '        Return False
    '    Catch se As SocketException
    '        If OnError <> Nothing Then
    '            RaiseEvent OnError(se.Message, mainSocket, se.ErrorCode)
    '        End If
    '        Return False
    '    End Try
    'End Function

    'Private Sub FileSendCallback(ByVal ar As IAsyncResult)
    '    Dim workerSocket As Socket = DirectCast(ar.AsyncState, Socket)
    '    workerSocket.EndSendFile(ar)
    '    If OnSendFile <> Nothing Then
    '        RaiseEvent OnSendFile(workerSocket)
    '    End If
    'End Sub


#End Region
End Class
#End If



