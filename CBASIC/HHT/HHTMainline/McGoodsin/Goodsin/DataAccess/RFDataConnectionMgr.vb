#If RF Then
Imports cPing
'-----------------------------------------------------------------------------
' Module  : ClientSocket.vb
' Desc    : Synchronous Sockets comms module
'  
'-----------------------------------------------------------------------------
'   This socket class acts synchronously. This allows the socket to run in the
'   same thread as the main application, therefore making Invoke calls to the
'   GUI unnecessary.
'
'   When the device goes out of range and loses the connection, the app will 
'   wait for the device to go back in range before completing a send or 
'   receive transaction.
'
'   When a transaction is sent to the server, a response must be received 
'   back from the server.  The Send() function will enable a ReceiveTimer 
'   which will notify the application that we are waiting to receive a 
'   response back.  Once the response is received, the timer will be disabled.
'-----------------------------------------------------------------------------
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Connects to alternate controller while primary is down
''' </Summary>
'''****************************************************************************
Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Diagnostics

' Socket notification commands.
Public Enum NotifyCommand
    Connected
    SentData
    ReceivedData
    SocketError
End Enum

Public Class RFDataConnectionMgr

    Dim _socket As Socket
    Private bConnected As Boolean = False
    'Private bDoAutoReconnect As Boolean = System.Convert.ToInt32(objAppContainer.Settings.DoAutoReconnect)          'Set this to false to prevent auto reconnect 'jp 10-may-2005
    Private Shared exitFlag As Boolean = False
    ''END
    'Temp varialbes used for time being
    Private Shared bDuplicateTimerEvent As Boolean = False
    Private bDoAutoReconnect As Boolean = False          'Set this to false to prevent auto reconnect 'jp 10-may-2005
    Private iTimeOut As Integer = 2000  '2000 'Set this to to configure the connection polling test timeout in milliseconds
    Private iTotRetries As Integer = 3 'RF RECONNECT 5   '3    'Total Number of Retries
    Private bPoweredOFF As Boolean = False  'Powered Off Status
    Private iRetryDelay As Integer = 10000 '10000      'Timer Retry in milliseconds
    Private iAttemptCnt As Integer = 0 'Connect Attemot Retry Counter
    Private WithEvents tmrRetry As New System.Windows.Forms.Timer
    ' Request buffer collection and class - new one is created for each Receive() issued
    Private id As Long = 0 ' A unique id is created for each request
    Private m_WaitTimeBeforeReconnect As Integer
    Private m_TimeOutDuration As Integer
    'RF RECONNECT
    Private bCancelReconnect As Boolean = False
    Private m_connector As frmConnector
    Private m_ReconnectTime As Integer = 1
    Private CurrentStamp As Date
    Private bConnectionComplete As Boolean = False
    Private Delegate Sub UpdateStatusCallback(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)
    'Private _req As New Collection
    Private Class Request
        Public id As Integer
        'Public buffer(1056) As Byte ' set GIB can be 1055 bytes
        Public buffer(1064) As Byte
        Public timestamp As Date
        Public Sub New(ByVal id As Integer)
            Me.id = id
            timestamp = Now()
        End Sub
    End Class
    Public Enum MODULESCREEN
        BATCHONE
        BATCHNEXT
        PREFINISH
        POSTFINISH
        FIRSTITEM
        NEXTITEM
        NONE
    End Enum
    Public Sub EndSession()

        m_connector = Nothing


    End Sub
    Public Sub New()
        m_WaitTimeBeforeReconnect = Convert.ToInt16(ConfigDataMgr.GetInstance().GetParam(ConfigKey.WAIT_TIME_BEFORE_RECONNECT).ToString)
        m_TimeOutDuration = Convert.ToInt32(ConfigDataMgr.GetInstance.GetParam(ConfigKey.TIME_OUT_DURATION).ToString()) * 1000000
        m_connector = New frmConnector()
        'objAppContainer.objSplashScreen.ShowSplashMessage("Connecting to server...")
        'If Connect(ConfigDataMgr.GetInstance().GetParam(ConfigKey.SERVER_IPADDRESS), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT)) Then
        If Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT)) Then
            objAppContainer.bConnect = Connected()
            'v1.1 MCF Change
        End If
        If objAppContainer.bMCFEnabled And Not objAppContainer.bConnect Then
            If fConnectAlternateInRF() Then
                objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                If Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT)) Then
                    objAppContainer.bConnect = Connected()
                    If objAppContainer.bConnect Then
                        ConfigDataMgr.GetInstance.SetActiveIP()
                    End If
                End If
            End If
        End If
    End Sub
    Public Function Connect(ByVal address As String, ByVal port As Integer) As Boolean

        ' Ensure the socket is disconnected
        Disconnect()



        ' Try and establish a connection with the server
        Try

            Dim ip As IPAddress = IPAddress.Parse(address)
            Dim endPoint As IPEndPoint = New IPEndPoint(ip, port)

            _socket = New Socket( _
                AddressFamily.InterNetwork, _
                SocketType.Stream, _
                ProtocolType.Tcp)

            '  objAppContainer.objStatusBar.SetMessage("Attempting to connect to server...")

            _socket.Connect(endPoint) 'Sync

            ' objAppContainer.objStatusBar.SetMessage("")

            If (_socket.Connected) Then
                ' Notify whether the socket connection was successful

                'objAppContainer.objSplashScreen.ShowSplashMessage("Server connection established.")
                '  objAppContainer.objSplashScreen.ShowSplashMessage("Loading, please wait...")
                objAppContainer.bConnect = True
                Connect = True
            Else
                objAppContainer.objLogger.WriteAppLog("Could not connect to controller", _
                                                             Logger.LogLevel.RELEASE)
                ' objAppContainer.objSplashScreen.ShowSplashMessage("Connection to server failed.")
                objAppContainer.bConnect = False
                Connect = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in RFConnectionMgr:Connect()" + ex.ToString(), _
                                             Logger.LogLevel.RELEASE)
            '  objAppContainer.objSplashScreen.ShowSplashMessage("Connection to server failed.")
            ' objAppContainer.objSplashScreen.ShowSplashMessage("Could not connect to controller")
            objAppContainer.bConnect = False
            Connect = False
        End Try

    End Function
    Public Function CheckTimeout() As Boolean
        Cursor.Current = Cursors.WaitCursor
        If _socket.Poll(m_TimeOutDuration, SelectMode.SelectRead) Then
            Cursor.Current = Cursors.Default
            Return True
        Else
            Cursor.Current = Cursors.Default
            Return False
        End If
    End Function
    Public Function Disconnect() As Boolean
        Try
            ' Return if a socket does not exist
            If _socket Is Nothing Then
                Return True
            End If
            ' Return if socket has already been disconnected
            If _socket.Connected = True Then
                '  objAppContainer.objStatusBar.SetMessage("Shutting down Server connection.")
                _socket.Shutdown(SocketShutdown.Both)
            End If
            '  objAppContainer.objStatusBar.SetMessage("Closing Server connection.")
            _socket.Close()
            _socket = Nothing
            Return True
        Catch ex As Exception
            '   objAppContainer.objStatusBar.SetMessage("Server connection closed abnormally.")
            _socket = Nothing
            Return False
        End Try
    End Function
    Function Connect2(ByVal address As String, ByVal port As Integer) As Boolean
        ' re-connect routines

        Try


            '   objAppContainer.objStatusBar.SetMessage("Attempting to connect to server[" & iAttemptCnt.ToString & "]...")
            Dim bContinue As Boolean = True
  
            If bContinue Then
                ' return right away if have not created socket
                If _socket Is Nothing Then
                    objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
                    Return False
                End If
                ' the socket is not connected if the Connected property is false
                If _socket.Connected = False Then
                    objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
                    Return False
                End If

                'Dim iTimeOut As Integer = 1000
                If _socket.Poll(iTimeOut, SelectMode.SelectWrite) Then
                    If _socket.Poll(iTimeOut, SelectMode.SelectError) Then
                        objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
                        Return False
                    Else
                        objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] re-connect success! ")
                        Return True
                    End If
                Else
                    objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
                    Return False
                End If

            Else
                objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
                Return False

            End If
        Catch ex As Exception
            objAppContainer.objStatusBar.SetMessage("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
            Return False
        End Try

    End Function
    Function ConnectSocket(ByVal address As String, ByVal port As Integer) As Boolean
        'Re-Connect socket 
        Try
            ' Ensure the socket is disconnected
            If Disconnect() = True Then

                Dim ip As IPAddress = IPAddress.Parse(address)
                Dim endPoint As IPEndPoint = New IPEndPoint(ip, port)

                ' Try and establish a connection with the server
                _socket = New Socket( _
                    AddressFamily.InterNetwork, _
                    SocketType.Stream, _
                    ProtocolType.Tcp)
                objAppContainer.objStatusBar.SetMessage("Attempting to connect to server[" & iAttemptCnt.ToString & "]...")
                '  objAppContainer.SetStatusBarText("Attempting to connect to server[" & iAttemptCnt.ToString & "]...")

                _socket.Connect(endPoint) 'Sync

                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)

                If (_socket.Connected) Then
                    objAppContainer.objStatusBar.SetMessage("Server connection established.")
                    '  objAppContainer.SetStatusBarText("Server connection established.")
                    Return True
                End If
            Else
                Return False
            End If

        Catch ex As Exception

            '  objAppContainer.SetStatusBarText("Connection #[" & iAttemptCnt.ToString & "] to server failed. ")
            objAppContainer.objLogger.WriteAppLog("Connection #[" & iAttemptCnt.ToString & "] to server failed. ", _
                                                           Logger.LogLevel.RELEASE)
            Return False
        End Try

    End Function
    ' Send data to the server.
    'Reny - changed sub to function
    'Public Function Send(ByVal data() As Byte) As Boolean
    Public Function Send(ByVal sSend As String) As Boolean

        sSend = Chr(255) + (sSend.Length + 5).ToString().PadLeft(4, "0") + sSend
        objAppContainer.objLogger.WriteAppLog("Send Message: " & sSend, Logger.LogLevel.RELEASE)
        Dim data() As Byte = System.Text.ASCIIEncoding.ASCII.GetBytes(sSend)
        data(0) = &HFF
        Dim bytesSent As Integer
        Try
            objAppContainer.objStatusBar.SetMessage("Sending transaction...")
            Try
                bytesSent = _socket.Send(data, 0, data.Length, SocketFlags.None)
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("Comms Err1.1:SEND:ErrNo=" & Err.Number)
                '  HandleSocketCommunicationFailure()
                'EstablishConnection()
                Send = False
                Exit Function
            End Try
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
            If bytesSent > 0 Then
                Send = True
            Else
                'MessageBox.Show("Warning: Nothing was sent.")
                Send = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Comms Err1.2:SEND:ErrNo=" & Err.Number, _
                                                      Logger.LogLevel.RELEASE)
            Send = False
        End Try

    End Function
    'Reny - seems to be non referenced function
    ' Read data from server.
    ' Public Function Receive(ByRef buffer() As Byte) As Boolean
    Public Function Receive(ByRef receivedString As String) As Boolean
        If Not Connected() Then
            objAppContainer.bCommFailure = True
            Return False
        End If
        Try

            id = (id + 1) Mod &H4  ' Allocate unique session ID (start small to test wrap)
            Dim req As Request = New Request(id)

            objAppContainer.objStatusBar.SetMessage("Waiting for server to respond...")

            Try
                _socket.Receive(req.buffer, 0, req.buffer.Length, SocketFlags.None)
            Catch ex As Exception
                objAppContainer.objStatusBar.SetMessage("Comms Err2.1:RECEIVE:ErrNo=" & Err.Number.ToString)
                Receive = False
                Exit Function
            End Try

            'Dim receivedString As String
            receivedString = System.Text.Encoding.ASCII.GetString(req.buffer, 0, req.buffer.Length)
            'new message format
            Dim RLength As Integer = receivedString.Length
            receivedString = receivedString.Substring(5, receivedString.Length - 5)
            objAppContainer.objLogger.WriteAppLog("Response Message" + receivedString.ToString(), Logger.LogLevel.RELEASE)
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)

            If receivedString.Length > 0 Then
                Try
                    'need to check if this is correctly coped ----------------------------------------
                    'Buffer = req.buffer
                    Receive = True
                Catch ex As Exception
                    If Err.Number <> 91 Then
                        MessageBox.Show("RaiseEvent Notify Failure: " & ex.Message, _
                        "Comms Failure:RECEIVE", _
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                        MessageBoxDefaultButton.Button1)
                    End If
                End Try
            Else
                Receive = False
            End If

        Catch ex As Exception
            MessageBox.Show(Err.Description & Erl.ToString & Err.Source)
            objAppContainer.objStatusBar.SetMessage("Comms Err2.3:RECEIVE:ErrNo=" & Err.Number.ToString)

            Receive = False
        End Try
    End Function

    Public Function OpenSocket(ByVal server As String, ByVal port As Short) As Boolean

        'jp 11-May-2005 New function to ensure that a socket can be created and connectivity is alive and kicking
        Dim tcpclient As New System.Net.Sockets.TcpClient
        Dim _socket2 As Socket
        Dim reader As StreamReader
        Dim buffer As String
        Dim ip As IPAddress = IPAddress.Parse(server)
        Dim endPoint As IPEndPoint = New IPEndPoint(ip, port)
        Try

            _socket2 = New Socket( _
                          AddressFamily.InterNetwork, _
                          SocketType.Stream, _
                          ProtocolType.Tcp)

            _socket2.Connect(endPoint) 'Sync

            If Not (_socket2.Connected) Then
                ' Notify whether the socket connection was successful
                _socket2.Close()
                OpenSocket = False
            Else
                _socket2.Close()
                OpenSocket = True
            End If
        Catch ex As Exception
            _socket2.Close()
            OpenSocket = False
        End Try

    End Function


    ' Returns true if the socket is connected to the server. The property '
    ' Socket.Connected does not always indicate if the socket is currently 
    ' connected, this polls the socket to determine the latest connectiouen state.
    Public Function Connected() As Boolean
        '  Get

        Dim bDoPing As Boolean = True

        If bDoPing Then
            Try

                Dim objPing As New cPing.Ping   'Instantiate Ping Object from cPing.dll
                objPing.Timeout = System.Convert.ToInt32(ConfigDataMgr.GetInstance().GetParam(ConfigKey.PING_TIMEOUT))  '1000          'Set TimeOut (milliseconds) for Ping Echo Response | Optional :- If if not set then defaulted to 1000 milliseconds
                Dim bPingStatus As Boolean = objPing.PingIP(objAppContainer.strActiveIP)   'Now Ping The Server
                If Not bPingStatus Then
                    Return False
                End If

            Catch ex As Exception
                Return False
            End Try
        End If


        ' return right away if have not created socket
        If _socket Is Nothing Then
            Return False
        End If

        ' the socket is not connected if the Connected property is false
        If _socket.Connected = False Then
            Return False
        End If

        ' there is no guarantee that the socket is connected even if the
        ' Connected property is true
        Try
            Dim iTimeOut As Integer = 1000
            If _socket.Poll(iTimeOut, SelectMode.SelectWrite) Then
                If _socket.Poll(iTimeOut, SelectMode.SelectError) Then
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

    Private Sub ForceExit()
        ' Display message and quit application
        objAppContainer.SetStatusBarText("Err: Server comms failure.")
        MessageBox.Show("RF Server application on EPOS controller has failed. Press 'OK' to exit application.", _
                    "Server Connection Failed", _
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                    MessageBoxDefaultButton.Button1)
        objAppContainer.DoCleanUp()
    End Sub
    ' This is the method to run when the timer is raised.
    Sub TimerEventProcessor(ByVal myObject As Object, _
    ByVal myEventArgs As EventArgs) Handles tmrRetry.Tick

        tmrRetry.Enabled = False 'Stop Timer and let Connect2 do its stuff

        If bDuplicateTimerEvent = False Then
            If Connect2(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT)) = True Then
                bConnected = True
                exitFlag = True
                'tmrRetry.Enabled = False
            Else
                bConnected = False
                exitFlag = True
                'tmrRetry.Enabled = True  ' Restarts the timer
            End If
            bDuplicateTimerEvent = True
        End If

    End Sub
    Public Function GetRetryMessage() As String 'ByVal ConnectionLostScenario As SMTransactDataManager.CurrentOperation) As String
        Dim bReconnectMsge As String = ""
        Dim strRcnMessage As String = "Retry {0} of 3 to reconnect. " + _
                                  "Please wait until successful or select Cancel to quit. "
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "Cartons already scanned will have been saved."
                    Case AppContainer.ModScreen.PREFINISH
                        bReconnectMsge = "Cartons already scanned may not have been saved."
                    Case AppContainer.ModScreen.ITEMSCAN
                        bReconnectMsge = "The last item scanned may not be saved."
                    Case Else
                        bReconnectMsge = ""
                End Select
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER _
                 Or objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS _
                 Or objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARY Then
                    bReconnectMsge = "The last item scanned may not be saved."
                End If
            Case AppContainer.ACTIVEMODULE.AUDITCARTON
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.CARTONSCAN
                        bReconnectMsge = "Cartons already scanned will not have been saved. "

                    Case AppContainer.ModScreen.ITEMSCAN
                        bReconnectMsge = "The last item scanned may not be saved. "
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "Cartons already scanned will have been saved. "

                End Select
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONITEM Then
                    bReconnectMsge = "Items already scanned will have been saved. " + _
                                     "The last item scanned may not be saved. "
                Else
                    bReconnectMsge = ""
                End If
            Case AppContainer.ACTIVEMODULE.AUDITUOD
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.CARTONSCAN
                        bReconnectMsge = "UODs already scanned may not be saved. "
                    Case AppContainer.ModScreen.ITEMSCAN
                        bReconnectMsge = "The last item scanned may not be saved. "
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "UODs already scanned will have been saved. "
                    Case AppContainer.ModScreen.QUIT
                        bReconnectMsge = "Audited UOD's may not be saved. "

                End Select
            Case AppContainer.ACTIVEMODULE.VCARTON Or AppContainer.ACTIVEMODULE.VUOD
                bReconnectMsge = ""
            Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.FIRSTBATCH
                        bReconnectMsge = "UODs already scanned may not be saved."
                    Case AppContainer.ModScreen.DRVRBDGESCAN
                        bReconnectMsge = "Previous batches of UODs have been saved. " + _
                                         "The current batch of UODs already scanned " + _
                                         "may not be saved."
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "Your batches of UODs already scanned have been saved."
                    Case AppContainer.ModScreen.NONE
                        bReconnectMsge = ""
                    Case Else
                        bReconnectMsge = ""
                End Select

        End Select
        strRcnMessage = strRcnMessage + bReconnectMsge
        Return strRcnMessage
    End Function
    Public Function ModuleReconnect() As Boolean
        Try
            If EstablishConnection() Then
                m_connector.btnCancel.Visible = True
                m_connector.btnCancel.Enabled = True
                m_connector.btnOK.Visible = False
                m_connector.btnOK.Enabled = False
                m_connector.Hide()
                objAppContainer.bReconnectSuccess = True
                'Sleep to prevent quick disappearing of Connector 
                Threading.Thread.Sleep(2000)
                objAppContainer.bCommFailure = False
                'v1.1 MCF Change
                If objAppContainer.iConnectedToAlternate = 1 Then
                    ConfigDataMgr.GetInstance.SetActiveIP()
                End If
                Return True
            Else
                If objAppContainer.iConnectedToAlternate <> 0 Then
                    m_connector.Hide()
                    If objAppContainer.iConnectedToAlternate = 1 Then
                        sCloseSession()
                    End If
                    Return False
                End If
                'if retry timeout happens then do not show the last Screen for Retry connection fail.
                'it should go to the Time out message box
                If objAppContainer.bRetryAtTimeout Then
                    m_connector.btnCancel.Visible = True
                    m_connector.btnCancel.Enabled = True
                    m_connector.btnOK.Visible = False
                    m_connector.btnOK.Enabled = False
                    m_connector.Visible = False
                    'calling time out
                    HandleTimeOut()
                    Return False
                End If
                'Checking if USer selected Ok after 3 retry fail and not CANCEL during Retry
                If Not bCancelReconnect Then
                    objAppContainer.bCommFailure = True
                    'for cancel click
                    objAppContainer.objLogger.WriteAppLog("Unable to regain connectivity", Logger.LogLevel.RELEASE)
                    m_connector.lblMessage.Text = RetryFailMessage()
                    m_connector.btnCancel.Visible = False
                    m_connector.btnCancel.Enabled = False
                    m_connector.btnOK.Visible = True
                    m_connector.btnOK.Enabled = True
                    m_connector.Refresh()
                    Cursor.Current = Cursors.Default
                    m_connector.ShowDialog()
                    m_connector.Visible = False
                    'CHANGE
                    'saving items or cartons scanned before connection loss
                    objAppContainer.bSaveDetails = True
                    SaveGIFDetails()
                    'disposing the current module
                    objAppContainer.DoCleanUp()
                    objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINMAINMNU
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                    'Connection was not established
                    'CHANGE
                    objAppContainer.bSaveDetails = False
                    objAppContainer.objGoodsInMenu.Visible = True
                    Return False
                Else
                    'if CANCEL is selected during retry
                    'disconnect the socket first
                    Disconnect()
                    objAppContainer.bCommFailure = True
                    m_connector.btnCancel.Visible = True
                    m_connector.btnCancel.Enabled = True
                    m_connector.btnOK.Visible = False
                    m_connector.btnOK.Enabled = False
                    m_connector.Visible = False
                    'CHANGE
                    'saving items or cartons scanned before connection loss
                    objAppContainer.bSaveDetails = True
                    SaveGIFDetails()
                    'disposing the current module
                    objAppContainer.DoCleanUp()
                    objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINMAINMNU
                    objAppContainer.m_ModScreen = AppContainer.ModScreen.NONE
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                    'CHANGE
                    objAppContainer.bSaveDetails = False
                    objAppContainer.objGoodsInMenu.Visible = True
                    Return False
                End If
                objAppContainer.bReconnectSuccess = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("ModuleReconnect : Exception" + ex.Message + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Function
    'Function to save the GIF details of Items or Cartons during connection loss and save it in a Global array list
    Public Function SaveGIFDetails() As Boolean
        Try
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.AUDITCARTON
                    'if items are scanned before connection loss, save the Item details
                    'to be send when connection is regained
                    'If ACSessionManager.GetInstance().m_ItemList.Count > 0 And _
                    '   objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONSUM Then
                    '    objAppContainer.objLogger.WriteAppLog("Saving Item details for Audit Carton on connection loss", Logger.LogLevel.RELEASE)
                    '    objAppContainer.objDataEngine.SendItemDetails(ACSessionManager.GetInstance().m_ItemList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                    '    objAppContainer.objDataEngine.SendItemQuantity(ACSessionManager.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                    '    objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, AppContainer.IsAbort.No)
                    'Else
                    objAppContainer.objLogger.WriteAppLog("Saving Item details for Audit Carton on connection loss", Logger.LogLevel.RELEASE)
                    If ACSessionManager.GetInstance().m_ItemList.Count > 0 Then
                        If objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingENQ Then
                            objAppContainer.m_SavedDetails = ACSessionManager.GetInstance().m_ItemList
                            objAppContainer.eDeliveryType = AppContainer.DeliveryType.ASN
                            objAppContainer.eFunctionType = AppContainer.FunctionType.Audit
                        ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingItemCount Then
                            objAppContainer.objDataEngine.SendItemDetails(ACSessionManager.GetInstance().m_ItemList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                            objAppContainer.objDataEngine.SendItemQuantity(ACSessionManager.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                            objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, AppContainer.IsAbort.No)
                        End If
                    ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingItemQuantity Then
                        'objAppContainer.objDataEngine.SendItemQuantity(ACSessionManager.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                        objAppContainer.m_FinishedDetails.Add(objAppContainer.objSavedGIFFinish)
                        objAppContainer.objSavedGIFFinish = New RFDataStructure.GIFMessage()
                    ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingSessionExit Then
                        objAppContainer.objSaveGIXMessage = objAppContainer.objSavedGIXMessage
                        objAppContainer.objSavedGIXMessage = New RFDataStructure.GIXMessage
                    ElseIf objAppContainer.objSaveGIXMessage.eDeliveryType <> AppContainer.DeliveryType.ASN Then
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit, AppContainer.IsAbort.Yes)
                    End If
                Case AppContainer.ACTIVEMODULE.AUDITUOD
                    'if items are scanned before connection loss save the item details
                    'to be send when connection is regained
                    objAppContainer.objLogger.WriteAppLog("Saving Item details for Audit UOD on connection loss", Logger.LogLevel.RELEASE)
                    If AUODSessionManager.GetInstance().m_ItemList.Count > 0 Then
                        If objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingENQ Then
                            objAppContainer.m_SavedDetails = AUODSessionManager.GetInstance().m_ItemList
                            objAppContainer.eDeliveryType = AppContainer.DeliveryType.SSC
                            objAppContainer.eFunctionType = AppContainer.FunctionType.Audit
                        ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingItemCount Then
                            objAppContainer.objDataEngine.SendItemDetails(AUODSessionManager.GetInstance().m_ItemList, AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit)
                            objAppContainer.objDataEngine.SendItemQuantity(AUODSessionManager.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit)
                            objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, AppContainer.IsAbort.No)
                        End If
                    ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingItemQuantity Then
                        'objAppContainer.objDataEngine.SendItemQuantity(AUODSessionManager.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                        objAppContainer.m_FinishedDetails.Add(objAppContainer.objSavedGIFFinish)
                        objAppContainer.objSavedGIFFinish = New RFDataStructure.GIFMessage()
                    ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingSessionExit Then
                        objAppContainer.objSaveGIXMessage = objAppContainer.objSavedGIXMessage
                        objAppContainer.objSavedGIXMessage = New RFDataStructure.GIXMessage
                    ElseIf objAppContainer.objSaveGIXMessage.eDeliveryType <> AppContainer.DeliveryType.SSC Then
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.Audit, AppContainer.IsAbort.Yes)
                    End If
                Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                    objAppContainer.objLogger.WriteAppLog("Saving Carton details for Book in Carton on connection loss", Logger.LogLevel.RELEASE)
                    'if cartons are booked in before connection loss, save the carton details
                    'to be send when connection is regained
                    If BCSessionMgr.GetInstance().m_CartonList.Count > 0 Then
                        objAppContainer.objDataEngine.SendCartonDetails(BCSessionMgr.GetInstance().m_CartonList, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.BookIn)
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.BookIn, AppContainer.IsAbort.No)
                        'ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingSessionExit Then
                        '    objAppContainer.objSaveGIXMessage = objAppContainer.objSavedGIXMessage
                        '    objAppContainer.objSavedGIXMessage = New RFDataStructure.GIXMessage
                    ElseIf BCSessionMgr.GetInstance().m_ItemList.Count > 0 Then
                        objAppContainer.objLogger.WriteAppLog("Saving Item details for Book in Carton/Order on connection loss", Logger.LogLevel.RELEASE)
                        If objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingENQ Then
                            objAppContainer.m_SavedDetails = BCSessionMgr.GetInstance().m_ItemList
                            objAppContainer.eDeliveryType = AppContainer.DeliveryType.Directs
                            objAppContainer.eFunctionType = AppContainer.FunctionType.BookIn
                        ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingItemCount Then
                            objAppContainer.objDataEngine.SendItemDetails(BCSessionMgr.GetInstance().m_ItemList, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn)
                            objAppContainer.objDataEngine.SendItemQuantity(BCSessionMgr.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn)
                            objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn, AppContainer.IsAbort.No)
                        End If
                    ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingItemQuantity Then
                        'objAppContainer.objDataEngine.SendItemQuantity(AUODSessionManager.GetInstance().m_ItemList.Count, AppContainer.DeliveryType.ASN, AppContainer.FunctionType.Audit)
                        objAppContainer.m_FinishedDetails.Add(objAppContainer.objSavedGIFFinish)
                        objAppContainer.objSavedGIFFinish = New RFDataStructure.GIFMessage()
                    ElseIf objAppContainer.eCurrLocation = AppContainer.CurrentLocation.SendingSessionExit Then
                        objAppContainer.objSaveGIXMessage = objAppContainer.objSavedGIXMessage
                        objAppContainer.objSavedGIXMessage = New RFDataStructure.GIXMessage()
                    Else
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.Directs, AppContainer.FunctionType.BookIn, AppContainer.IsAbort.Yes)
                    End If
                Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.BookIn, AppContainer.IsAbort.No)
                Case AppContainer.ACTIVEMODULE.VCARTON
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.ASN, AppContainer.FunctionType.View, AppContainer.IsAbort.No)
                Case AppContainer.ACTIVEMODULE.VUOD
                        objAppContainer.objDataEngine.SendSessionExit(AppContainer.DeliveryType.SSC, AppContainer.FunctionType.View, AppContainer.IsAbort.No)
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SaveGIFDetails : Exception" + ex.Message + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Function
    'Retry Fail messages for each particular module and action
    Public Function RetryFailMessage() As String
        Dim ReconnectMessage As String = ""
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.VCARTON
                ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Select OK to continue"
            Case AppContainer.ACTIVEMODULE.VUOD
                ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Select OK to continue"
            Case AppContainer.ACTIVEMODULE.AUDITCARTON
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.CARTONSCAN
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                 "Any cartons scanned may not be " + _
                                  "saved.Select OK to continue."
                    Case AppContainer.ModScreen.POSTFINISH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                 "Any cartons scanned will have been " + _
                                  "saved.Select OK to continue."
                    Case AppContainer.ModScreen.ITEMSCAN
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Your list has been saved. " + _
                                  "The last item scanned may not be saved." + _
                                   "Select OK to continue."
                    Case Else
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Select OK to continue."
                End Select

            Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.POSTFINISH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                   "Any cartons scanned will have been saved. " + _
                                   "Select OK to continue."
                    Case AppContainer.ModScreen.BCITEMFINISH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                   "Your list has been saved. The last item scanned may not be saved. " + _
                                   "Select OK to continue."
                    Case AppContainer.ModScreen.PREFINISH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Any cartons scanned may not be saved. " + _
                                    "Select OK to continue."
                    Case AppContainer.ModScreen.QUIT
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Any cartons scanned may not be saved. " + _
                                    "Select OK to continue."
                    Case Else
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Any cartons scanned may not be saved. " + _
                                    "Select OK to continue."
                End Select
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER _
                Or objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                    ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Your list has been saved. " + _
                                    "The last item scanned may not be " + _
                                    "saved. Select OK to continue."
                End If
            Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.POSTFINISH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                   "Your batches of UODs already scanned have " + _
                                   "been saved. Select OK to continue."
                    Case AppContainer.ModScreen.FIRSTITEM
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Any UODs scanned may not have been saved. " + _
                                            "Select OK to continue."
                    Case AppContainer.ModScreen.DRVRBDGESCAN
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Previous batches of UODs have been " + _
                                            "saved. The current batch of UODs already " + _
                                            "scanned may not be saved. " + _
                                            "Select OK to continue."
                    Case AppContainer.ModScreen.FIRSTBATCH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Any UODs scanned may not be saved. " + _
                                            "Select OK to continue."
                    Case Else
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Select OK to continue."
                End Select

            Case AppContainer.ACTIVEMODULE.AUDITUOD
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.CARTONSCAN
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                 "Any UODs scanned may not be " + _
                                  "saved.Select OK to continue."
                    Case AppContainer.ModScreen.POSTFINISH
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                 "Any UODs scanned will have been " + _
                                  "saved.Select OK to continue."
                    Case AppContainer.ModScreen.ITEMSCAN
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                  "Your list has been saved. " + _
                                  "The last item scanned may not be saved." + _
                                   "Select OK to continue."
                    Case Else
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Select OK to continue."
                End Select

            Case Else
                ReconnectMessage = "Unable to regain connectivity. " + _
                                    "Select OK to continue."
        End Select
        Return ReconnectMessage
    End Function
    'tries to establish connection with controller 
    Public Function EstablishConnection() As Boolean
        'Reset the Cancel Status
        bCancelReconnect = False
        If m_connector Is Nothing Then
            m_connector = New frmConnector()
        End If
        m_connector.cancelled = False
        'Retry Message
        Dim ReconnectMessage As String = ""
        'Reset the Retry Time
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        Dim bTemp As Boolean = False
        'Display the form here
        Cursor.Current = Cursors.Default
        If objAppContainer.bRetryAtTimeout Then     '  Timeout changes start
            m_connector.btnCancel.Visible = False   '  To avoid displaying cancel
            m_connector.btnCancel.Enabled = False   '  button in the message box
            ReconnectMessage = "Retry {0} of 3 to reconnect. Please wait until successful."
        Else
            ReconnectMessage = GetRetryMessage()    '  displayed for reconnection
            m_connector.btnCancel.Visible = True    '  after a data time out has occurred
            m_connector.btnCancel.Enabled = True    '  
        End If
        m_connector.btnOK.Visible = False
        m_connector.btnOK.Enabled = False
        'updating the status message in the retry message box
        UpdateStatus(ReconnectMessage, 1)
        m_connector.Show()
        Application.DoEvents()
        bConnectionComplete = False
        CurrentStamp = DateAndTime.Now
        objAppContainer.objLogger.WriteAppLog("Starting a new Thread for connection")
        'starting a new thread for reconnection
        Dim ReconnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(ReconnectMessage, CurrentStamp), Threading.ThreadStart))
        bConnected = False
        ReconnectThread.Start()
        While Not (bConnectionComplete)
            Application.DoEvents()
            'checking if user has cancelled the Retry attempts
            If m_connector.cancelled Then
                If Not bCancelReconnect Then
                    objAppContainer.objLogger.WriteAppLog("Reconnect Cancelled")
                    bCancelReconnect = True
                    'if user Cancelled retry then come out 
                    Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.BelowNormal
                    'DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Cancelled)
                    'Return False
                End If
                Threading.Thread.Sleep(Macros.CANCEL_SLEEP_TIME)
            End If
        End While
        'v1.1 MCF Change
        If (objAppContainer.bMCFEnabled And Not bConnected And Not bCancelReconnect) Then
            CurrentStamp = DateAndTime.Now
            bConnected = False
            If fConnectAlternateInRF() Then
                bConnectionComplete = False
                CurrentStamp = DateAndTime.Now
                objAppContainer.objLogger.WriteAppLog("Starting a new Thread for alternate connection")
                'starting a new thread for reconnection
                Dim AlternateConnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(ReconnectMessage, CurrentStamp), Threading.ThreadStart))
                bConnected = False
                AlternateConnectThread.Start()
                While Not (bConnectionComplete)
                    Application.DoEvents()
                    'checking if user has cancelled the Retry attempts
                    If m_connector.cancelled Then
                        If Not bCancelReconnect Then
                            objAppContainer.objLogger.WriteAppLog("Reconnect to alternate Cancelled")
                            bCancelReconnect = True
                            'if user Cancelled retry then come out 
                            Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.BelowNormal
                        End If
                        Threading.Thread.Sleep(Macros.CANCEL_SLEEP_TIME)
                    End If
                End While
            Else
                sCloseSession()
            End If
        End If
        objAppContainer.objLogger.WriteAppLog("Establish Connection END")
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
        Return bConnected

    End Function
    'handles the retry attempts
    Public Function ReconnectionHandler(ByVal ReconnectMessage As String, ByVal CurrentTryStamp As Date) As Boolean
        Dim iReconnectTime As Integer = 1
        objAppContainer.objLogger.WriteAppLog("Entered Reconnection Handler", Logger.LogLevel.RELEASE)
        'checking the retry attempts .. max 3 attempts
        Do While ((iReconnectTime <= Macros.RECONNECT_ATTEMPTS) And _
                  (CurrentTryStamp = CurrentStamp))
            If Not bCancelReconnect Then
                'update the status of the retry message box with the current retry attempt no.
                UpdateStatus(ReconnectMessage, iReconnectTime)
                objAppContainer.objLogger.WriteAppLog("Reconnect Attempt - " + iReconnectTime.ToString(), Logger.LogLevel.RELEASE)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
                objAppContainer.objLogger.WriteAppLog("Initialising Socket", Logger.LogLevel.RELEASE)
                'checking if controller can be connected from device or not
                If Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT)) = True Then
                    bConnected = True
                Else
                    bConnected = False
                End If
                bConnected = Connected()
                objAppContainer.objLogger.WriteAppLog("Completed Waiting", Logger.LogLevel.RELEASE)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
                'if reconnected then come out of the retry attempts
                If bConnected Then Exit Do
                Threading.Thread.Sleep(m_WaitTimeBeforeReconnect)
                iReconnectTime = iReconnectTime + 1
            Else
                Exit Do
            End If
        Loop
        bConnectionComplete = True
    End Function


    ''' <summary>
    ''' Updates the status of the message in the connector
    ''' </summary>
    ''' <param name="Connectivity_Message">Takes the unformated messages and 
    ''' formats with reconnect attemp and displays it to the user</param>
    ''' <remarks>none</remarks>
    Public Sub UpdateStatus(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)
        Try
            'Fix for Default message being shown in the connector
            'This is when the reconnect   attempt is 1 then The attempt is set to 1 else the corresponding thing is set
            If m_connector.InvokeRequired Then
                m_connector.Invoke(New UpdateStatusCallback(AddressOf UpdateStatus), Connectivity_Message, RetryAttempt)
            Else
                If RetryAttempt <= 3 Then
                    m_connector.setStatus(String.Format(Connectivity_Message, RetryAttempt.ToString()))
                    'Else
                    '    m_connector.setStatus(String.Format(Connectivity_Message, m_ReconnectTime - 1))
                End If
                'm_connector.Refresh()
                Application.DoEvents()
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured while Updating the connector status message" + _
                                                  "Message : " + ex.Message, Logger.LogLevel.RELEASE)

        End Try
    End Sub
    ''' <summary>
    ''' Function to handle timeout when there is no response received from Transact 
    ''' for a predefined amount of time.
    ''' </summary>
    ''' <param name="TimedOutScenario"></param>
    ''' <remarks></remarks>
    Public Function HandleTimeOut() As Boolean
        objAppContainer.objLogger.WriteAppLog("Timeout occured", Logger.LogLevel.INFO)
        Dim strMessage As String = "Unable to communicate with the controller. " + _
                                 "This may be due to controller reload, network or power failure. Once ready " + _
                                 "select RETRY to reconnect or press CANCEL to quit."
        objAppContainer.bTimeOut = True
        objAppContainer.bRetryAtTimeout = False
        m_connector.lblMessage.Text = strMessage
        m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Regular)
        m_connector.Label1.Text = "Timeout Occurred"
        m_connector.btnCancel.Visible = False
        m_connector.btnOK.Visible = False
        m_connector.btnTimeoutRetry.Visible = True
        m_connector.btnTimeoutRetry.Enabled = True
        m_connector.btnTimeoutCancel.Visible = True
        m_connector.btnTimeoutCancel.Enabled = True
        m_connector.Refresh()
        Cursor.Current = Cursors.Default
        m_connector.ShowDialog()
        'At this point either of the option is selected.
        'Reset the buttons and label text to default so as not to disturb reconnect form display.
        m_connector.btnTimeoutRetry.Visible = False
        m_connector.btnTimeoutRetry.Enabled = False
        m_connector.btnTimeoutCancel.Visible = False
        m_connector.btnTimeoutCancel.Enabled = False
        m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        m_connector.Label1.Text = "Unable to Connect to Controller"
        'if cancel selected during timeout
        If m_connector.TimeoutCancel Then
            'dispose the current module
            objAppContainer.objLogger.WriteAppLog("Timeout occured: Session Ended", Logger.LogLevel.INFO)
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                    BDSessionMgr.GetInstance().DisposeBookInUOD()
                Case AppContainer.ACTIVEMODULE.AUDITUOD
                    AUODSessionManager.GetInstance().DisposeAuditUOD()
                Case AppContainer.ACTIVEMODULE.VUOD
                    VUODSessionMgr.GetInstance().DisposeVUOD()
                Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                    BCSessionMgr.GetInstance().DisposeBookIn()
                    If objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.AUDITCARTON Then
                        ACSessionManager.GetInstance().DisposeAuditCarton()
                    End If
                Case AppContainer.ACTIVEMODULE.AUDITCARTON
                    ACSessionManager.GetInstance().DisposeAuditCarton()
                Case AppContainer.ACTIVEMODULE.VCARTON
                    VCSessionManager.GetInstance().DisposeViewCarton()
                    If objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.BOOKINCARTON Then
                        BCSessionMgr.GetInstance().DisposeBookIn()
                    End If
                Case AppContainer.ACTIVEMODULE.USERAUTH
                    UserSessionManager.GetInstance().EndSession()
            End Select
            'start closing the application
            If objAppContainer.objActiveModule <> AppContainer.ACTIVEMODULE.USERAUTH Then
                objAppContainer.objGoodsInMenu.Close()
            End If
            objAppContainer.bRetryAtTimeout = False
            Return True
        Else
            objAppContainer.bRetryAtTimeout = True
            Return False
        End If
    End Function

    Private Function HandleDisconnect() As Boolean
        Dim bTemp As Boolean = False
        If m_connector Is Nothing Then
            m_connector = New frmConnector()
        End If
        'Display the form here
        Cursor.Current = Cursors.Default
        m_connector.btnCancel.Visible = True
        m_connector.btnCancel.Enabled = True
        m_connector.btnOK.Visible = False
        m_connector.btnOK.Enabled = False
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        UpdateStatus()
        m_connector.Show()
        Application.DoEvents()
        System.Threading.Thread.Sleep(1000)
        objAppContainer.objLogger.WriteAppLog("Disconnected...Trying to reconnect", Logger.LogLevel.RELEASE)
        'Reconnect Logic
        Do While m_ReconnectTime <= Macros.RECONNECT_ATTEMPTS
            If Not m_connector.cancelled Then

                Cursor.Current = Cursors.Default
                UpdateStatus()
                m_ReconnectTime = m_ReconnectTime + 1
                If Connect(objAppContainer.strActiveIP, ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT)) = True Then
                    bConnected = True
                Else
                    bConnected = False
                End If
                ' bConnected = IsConnected()
                bConnected = Connected()
                Threading.Thread.Sleep(m_WaitTimeBeforeReconnect)
                If bConnected Then Exit Do
            Else
                Cursor.Current = Cursors.Default
                Exit Do
            End If
        Loop
        m_ReconnectTime = 1
        Cursor.Current = Cursors.Default
        If Not bConnected Then
            objAppContainer.bCommFailure = True
        End If
        Return bConnected
    End Function
    Public Function IsSktConnected(ByVal address As String, ByVal port As Integer) As Boolean
        'Try
        Dim iTimeOut As Integer = 1000
        If _socket.Poll(iTimeOut, SelectMode.SelectWrite) Then
            If _socket.Poll(iTimeOut, SelectMode.SelectError) Then
                Return False
            Else
                Return True
            End If
        Else

            Return False
        End If
        Disconnect()

        '    Dim ip As IPAddress = IPAddress.Parse(address)
        '    Dim endPoint As IPEndPoint = New IPEndPoint(ip, port)

        '    _socket = New Socket( _
        '        AddressFamily.InterNetwork, _
        '        SocketType.Stream, _
        '        ProtocolType.Tcp)

        '    '  objAppContainer.objStatusBar.SetMessage("Attempting to connect to server...")

        '    _socket.Connect(endPoint) 'Sync

        '    ' objAppContainer.objStatusBar.SetMessage("")

        '    'if (checkSocket.GetSocketOption(SocketOptionLevel.Soc ket, SocketOptionName.KeepAlive, 1)[0].Equals(1))

        '    ' return checkSocket.Connected;

        '    If _socket.Connected = False Then
        '        Return False
        '    End If

        '    'checkSocket.BeginSend(new byte[0], 0, 0, SocketFlags.None, null, null);

        '    Dim bSelectRead As Boolean = _socket.Poll(1, SelectMode.SelectRead)

        '    Dim bSelectWrite As Boolean = _socket.Poll(1, SelectMode.SelectWrite)

        '    Dim available As Integer = _socket.Available

        '    'if (bSelectWrite && bSelectRead && available 0)

        '    If bSelectWrite AndAlso bSelectRead Then


        '        'return true;

        '        'checkSocket.BeginReceive(new byte[1], 0, 1, SocketFlags.Peek, null, null);

        '        _socket.Receive(New Byte(-1) {}, 0, 0, SocketFlags.Peek)

        '        _socket.Send(New Byte(-1) {}, 0, 0, SocketFlags.None)


        '        Return _socket.Connected
        '    Else


        '        Return False

        '    End If

        'Catch generatedExceptionName As SocketException



        '    Return False

        'Catch generatedExceptionName As ObjectDisposedException



        '    Return False
        'End Try

    End Function

    Public Function IsConnected() As Boolean
        Dim bConnected As Boolean = False

        ' Dim bState As Boolean = _socket.Poll(-1, SelectMode.SelectWrite)
        Dim bState As Boolean = _socket.Poll(100, SelectMode.SelectRead)
        Try
            '  If Not bState AndAlso (_socket.Available = 0) Then
            If (bState AndAlso (_socket.Available = 0)) Then
                bConnected = False
            Else
                bConnected = True
            End If
        Catch ex As Exception
            '_socket.Available can throw an exception 
            bConnected = False
        End Try
        Return bConnected
    End Function
    Public Sub DisplayConnector()
        m_connector.Visible = True
    End Sub

    Public Sub HideConnector()
        m_connector.Visible = False
    End Sub
    Public Sub UpdateStatus()
        Dim bReconnectMsge As String = ""
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "Cartons already scanned will have been saved."
                    Case AppContainer.ModScreen.PREFINISH
                        bReconnectMsge = "Cartons already scanned may not have been saved."
                    Case AppContainer.ModScreen.ITEMSCAN
                        bReconnectMsge = "The last item scanned may not be saved."
                    Case Else
                        bReconnectMsge = ""
                End Select
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINITEMFORNOORDERNUMBER _
                Or objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINORDERSUMMARYOFCONTENTS Then
                    bReconnectMsge = "The last item scanned may not be saved."
                End If
            Case AppContainer.ACTIVEMODULE.AUDITCARTON
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.CARTONSCAN
                        bReconnectMsge = "Cartons already scanned may not have been saved. "

                    Case AppContainer.ModScreen.ITEMSCAN
                        bReconnectMsge = "The last item scanned may not be saved. "
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "Cartons already scanned will have been saved. "

                End Select
                If objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.AUDITCARTONITEM Then
                    bReconnectMsge = "Items already scanned will have been saved. " + _
                                     "The last item scanned may not be saved. "
                Else
                    bReconnectMsge = ""
                End If
            Case AppContainer.ACTIVEMODULE.AUDITUOD
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.CARTONSCAN
                        bReconnectMsge = "UODs already scanned may not have been saved. "

                    Case AppContainer.ModScreen.ITEMSCAN
                        bReconnectMsge = "The last item scanned may not be saved. "
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "UODs already scanned will have been saved. "

                End Select
            Case AppContainer.ACTIVEMODULE.VCARTON Or AppContainer.ACTIVEMODULE.VUOD
                bReconnectMsge = ""
            Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                Select Case objAppContainer.m_ModScreen
                    Case AppContainer.ModScreen.FIRSTBATCH
                        bReconnectMsge = "UODs already scanned may not have been saved."
                    Case AppContainer.ModScreen.DRVRBDGESCAN
                        bReconnectMsge = "Previous batches of UODs have been saved. " + _
                                         "The current batch of UODs already scanned " + _
                                         "may not have been saved."
                    Case AppContainer.ModScreen.POSTFINISH
                        bReconnectMsge = "Your batches of UODs already scanned have been saved."
                    Case AppContainer.ModScreen.NONE
                        bReconnectMsge = ""
                    Case Else
                        bReconnectMsge = ""
                End Select

        End Select
        m_connector.setStatus(String.Format(Macros.CONNECTIVITY_MESSAGE + bReconnectMsge, m_ReconnectTime))
        Application.DoEvents()
    End Sub
    'v1.1 MCF Changes
    '' <summary>
    '' Prompts the User Whether he wishes to conenct to Alternate Controller
    '' </summary>
    '' <param>None </param>
    '' <return> Boolean True if the user wishes to connect to alternate; False otherwise </return>
    Public Function fConnectAlternateInRF() As Boolean
        Try
            Dim bConnectToAlternate As Boolean = False
            Dim strMessage As String = "Unable to regain connectivity to the current controller." + _
                           " All work in this session may be lost. Select OK to exit or ALTERNATE " + _
                           "to continue with a new session on the alternate controller."
            m_connector.lblMessage.Text = strMessage
            m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Regular)
            m_connector.Label1.Text = "Unable to Connect to Controller"
            m_connector.btnCancel.Visible = False
            m_connector.btnOK.Visible = False
            m_connector.btnTimeoutRetry.Visible = False
            m_connector.btnTimeoutCancel.Visible = False
            m_connector.btnCancelAlternate.Visible = True
            m_connector.btnCancelAlternate.Enabled = True
            m_connector.btnConnectAlternate.Visible = True
            m_connector.btnConnectAlternate.Enabled = True
            m_connector.Refresh()
            Cursor.Current = Cursors.Default
            m_connector.Location = New System.Drawing.Point(7, 65)
            m_connector.ShowDialog()
            'At this point either of the option is selected.
            'Reset the buttons and label text to default so as not to disturb reconnect form display.
            m_connector.btnCancelAlternate.Visible = False
            m_connector.btnCancelAlternate.Enabled = False
            m_connector.btnConnectAlternate.Visible = False
            m_connector.btnConnectAlternate.Enabled = False
            m_connector.btnCancel.Visible = True
            m_connector.btnCancel.Enabled = True
            m_connector.lblMessage.Text = ""
            m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
            If m_connector.ConnectToAlternate = 1 Then
                bConnectToAlternate = True
            Else
                m_connector.HideConnector()
            End If
            If bConnectToAlternate Then
                objAppContainer.iConnectedToAlternate = 1
                If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                Else
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                End If
            Else
                objAppContainer.iConnectedToAlternate = -1
            End If
            Return bConnectToAlternate
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Function
    '' <summary>
    '' Closes the current sessions
    '' </summary>
    '' <param>None </param>
    '' <return> Boolean True if successfully close the session; False otherwise </return>
    Public Sub sCloseSession()
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                BDSessionMgr.GetInstance().DisposeBookInUOD()
            Case AppContainer.ACTIVEMODULE.AUDITUOD
                AUODSessionManager.GetInstance().DisposeAuditUOD()
            Case AppContainer.ACTIVEMODULE.VUOD
                VUODSessionMgr.GetInstance().DisposeVUOD()
            Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                BCSessionMgr.GetInstance().DisposeBookIn()
                If objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.AUDITCARTON Then
                    ACSessionManager.GetInstance().DisposeAuditCarton()
                End If
            Case AppContainer.ACTIVEMODULE.AUDITCARTON
                ACSessionManager.GetInstance().DisposeAuditCarton()
            Case AppContainer.ACTIVEMODULE.VCARTON
                VCSessionManager.GetInstance().DisposeViewCarton()
                If objAppContainer.objPrevMod = AppContainer.ACTIVEMODULE.BOOKINCARTON Then
                    BCSessionMgr.GetInstance().DisposeBookIn()
                End If
            Case AppContainer.ACTIVEMODULE.USERAUTH
                UserSessionManager.GetInstance().EndSession()
        End Select
        'start closing the application
        objAppContainer.objGoodsInMenu.Close()
    End Sub
End Class
#End If

