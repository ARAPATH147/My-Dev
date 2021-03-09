Imports cPing
Imports System.Net
Imports System.Net.Sockets
Imports System.IO
Imports System.Diagnostics

'''****************************************************************************
''' <FileName> ConnectionManager.vb </FileName> 
''' <summary> Class to handle socket connections to TRANSACT on Controller.
'''           Also handles connection to Alternate .28 controller.
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

Public Class ConnectionManager

    Private Shared moConnectionManager As ConnectionManager
    Private bCancelReconnect As Boolean = False
    Private m_Socket As Socket
    Private bConnected As Boolean = False
    Private miWaitTimeBeforeReconnect As Integer
    Private miIPPort As Integer
    Private miTimeOutDuration As Integer
    Private mfrmconnector As frmConnection
    Private msReconnectMessage As String = ""
    Private id As Long = 0
    Private CurrentStamp As Date
    Private bConnectionComplete As Boolean = False

    Private Delegate Sub UpdateStatusCallback(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)

    ''' <summary>
    ''' The shared function GetInstance will implement a check for the instantiation
    ''' of class ConnectionManager to make sure that the class has only one instance
    ''' </summary>
    Public Shared Function GetInstance() As ConnectionManager
        If moConnectionManager Is Nothing Then
            moConnectionManager = New ConnectionManager
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
        miWaitTimeBeforeReconnect = Convert.ToInt16(ConfigFileManager.GetInstance().GetParam(ConfigKey.WAIT_TIME_BEFORE_RECONNECT).ToString)
        miTimeOutDuration = Convert.ToInt32(ConfigFileManager.GetInstance.GetParam(ConfigKey.TIME_OUT_DURATION).ToString()) * 1000000
        miIPPort = ConfigFileManager.GetInstance().GetParam(ConfigKey.IPPORT)
        If Connect(oAppMain.cActiveIP, miIPPort) Then
            oAppMain.bConnect = Connected()
        End If
        If oAppMain.bMCFEnabled And Not oAppMain.bConnect Then
            If fConnectAlternateInRF() Then
                oAppMain.frmLoadScreen.ChangeLabelText("Connecting to Alternate controller...")
                If Connect(oAppMain.cActiveIP, miIPPort) Then
                    oAppMain.bConnect = Connected()
                    If oAppMain.bConnect Then
                        ConfigFileManager.GetInstance.SetParam(ConfigKey.ACTIVE_IPADDRESS, oAppMain.cActiveIP)
                    End If
                End If
            Else
                oAppMain.bConnect = Connected()
            End If
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
        'Development note - Comment out PING request when running in debug
        Dim bDoPing As Boolean = True
        If bDoPing Then
            Try
                ' Instantiate Ping Object from cPing.dll
                Dim objPing As New cPing.Ping
                ' Set TimeOut (milliseconds) for Ping Echo Response | Optional :- If if not set then defaulted to 1000 milliseconds
                objPing.Timeout = System.Convert.ToInt32(ConfigFileManager.GetInstance().GetParam(ConfigKey.PING_TIMEOUT))  '1000          
                Dim bPingStatus As Boolean = objPing.PingIP(oAppMain.cActiveIP)   'Now Ping The Server
                If Not bPingStatus Then
                    Return False
                End If

            Catch ex As Exception
                Return False
            End Try
        End If

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
            Receive = False
        End Try
    End Function

    ''' <summary>
    ''' Function to determine whether spinning hour-glass should be displayed or not
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function CheckTimeout() As Boolean
        Cursor.Current = Cursors.WaitCursor
        If m_Socket.Poll(miTimeOutDuration, SelectMode.SelectRead) Then
            Cursor.Current = Cursors.Default
            Return True
        Else
            Cursor.Current = Cursors.Default
            Return False
        End If
    End Function

    ''' <summary>
    ''' Function to handle connection to alternate .28 server.
    ''' Nb. this will only be successful in situations where the Primary
    '''     .27 server is down and the .28 Secondary/Alternate file server 
    '''     has been manually configured as Acting Master.
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Function fConnectAlternateInRF() As Boolean
        Try
            Dim frmconnector = New frmConnection

            Dim bConnectToAlternate As Boolean = False
            Dim strMessage As String = "Unable to regain connectivity to the current controller." + _
                           " All work in this session may be lost. Select OK to exit or ALTERNATE " + _
                           "to continue with a new session on the alternate controller."
            frmconnector.lblMessage.Text = strMessage
            frmconnector.lblMessage.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Regular)
            frmconnector.Label1.Text = "Unable to Connect to Controller"
            frmconnector.btnCancel.Visible = False
            frmconnector.btnOK.Visible = False
            frmconnector.btnTimeoutRetry.Visible = False
            frmconnector.btnTimeoutCancel.Visible = False
            frmconnector.btnCancelAlternate.Visible = True
            frmconnector.btnCancelAlternate.Enabled = True
            frmconnector.btnConnectAlternate.Visible = True
            frmconnector.btnConnectAlternate.Enabled = True
            frmconnector.Refresh()
            Cursor.Current = Cursors.Default
            frmconnector.Location = New System.Drawing.Point(7, 65)
            frmconnector.ShowDialog()
            'At this point either of the option is selected.
            'Reset the buttons and label text to default so as not to disturb reconnect form display.
            frmconnector.btnCancelAlternate.Visible = False
            frmconnector.btnCancelAlternate.Enabled = False
            frmconnector.btnConnectAlternate.Visible = False
            frmconnector.btnConnectAlternate.Enabled = False
            frmconnector.btnCancel.Visible = True
            frmconnector.btnCancel.Enabled = True
            frmconnector.lblMessage.Text = ""
            frmconnector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
            If frmconnector.ConnectToAlternate = 1 Then
                bConnectToAlternate = True
            End If
            frmconnector.Close()
            frmconnector = Nothing

            If bConnectToAlternate Then
                oAppMain.iConnectedToAlternate = 1
                If oAppMain.cActiveIP = ConfigFileManager.GetInstance.GetParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                    oAppMain.cActiveIP = ConfigFileManager.GetInstance.GetParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                Else
                    oAppMain.cActiveIP = ConfigFileManager.GetInstance.GetParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                End If
            Else
                oAppMain.iConnectedToAlternate = -1
            End If
            Return bConnectToAlternate
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString())
        End Try
    End Function

    ''' <summary>
    ''' Function to handle atempts to reconnect to the controller.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns>Boolean</returns>
    Public Function ModuleReconnect() As Boolean
        Try
            If EstablishConnection() Then
                mfrmconnector.btnCancel.Visible = True
                mfrmconnector.btnCancel.Enabled = True
                mfrmconnector.btnOK.Visible = False
                mfrmconnector.btnOK.Enabled = False
                mfrmconnector.Hide()
                oAppMain.bReconnectSuccess = True
                'Sleep to prevent quick disappearing of Connector 
                Threading.Thread.Sleep(2000)
                oAppMain.bCommFailure = False
                If oAppMain.iConnectedToAlternate = 1 Then
                    ConfigFileManager.GetInstance.SetParam(ConfigKey.ACTIVE_IPADDRESS, oAppMain.cActiveIP)
                End If
                Return True
            Else
                'If oAppMain.iConnectedToAlternate <> 0 Then
                '    mfrmconnector.Hide()
                '    Return False
                'End If
                'if retry timeout happens then do not show the last Screen for Retry connection fail.
                'it should go to the Time out message box
                If oAppMain.bRetryAtTimeout Then
                    mfrmconnector.btnCancel.Visible = True
                    mfrmconnector.btnCancel.Enabled = True
                    mfrmconnector.btnOK.Visible = False
                    mfrmconnector.btnOK.Enabled = False
                    mfrmconnector.Visible = False
                    'calling time out
                    HandleTimeOut()
                End If
                'Checking if USer selected Ok after 3 retry fail and not CANCEL during Retry
                If Not bCancelReconnect Then
                    oAppMain.bCommFailure = True
                    'for cancel click
                    mfrmconnector.lblMessage.Text = RetryFailMessage()
                    mfrmconnector.btnCancel.Visible = False
                    mfrmconnector.btnCancel.Enabled = False
                    mfrmconnector.btnOK.Visible = True
                    mfrmconnector.btnOK.Enabled = True
                    mfrmconnector.Refresh()
                    Cursor.Current = Cursors.Default
                    mfrmconnector.ShowDialog()
                    mfrmconnector.Visible = False
                Else
                    'if CANCEL is selected during retry
                    'disconnect the socket first
                    Disconnect()
                    oAppMain.bCommFailure = True
                    mfrmconnector.btnCancel.Visible = True
                    mfrmconnector.btnCancel.Enabled = True
                    mfrmconnector.btnOK.Visible = False
                    mfrmconnector.btnOK.Enabled = False
                    mfrmconnector.Visible = False
                End If

                Return False

            End If
        Catch ex As Exception

        End Try
    End Function

    ''' <summary>
    ''' Function to handle timeout when there is no response received from Transact 
    ''' for a predefined amount of time.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns>Boolean</returns>
    Public Function HandleTimeOut() As Boolean
        Dim strMessage As String = "Unable to communicate with the controller. " + _
                                 "This may be due to controller reload, network or power failure. Once ready " + _
                                 "select RETRY to reconnect or press CANCEL to quit."
        oAppMain.bTimeOut = True
        oAppMain.bRetryAtTimeout = False
        frmConnection.lblMessage.Text = strMessage
        frmConnection.lblMessage.Font = New System.Drawing.Font("Tahoma", 8.5!, System.Drawing.FontStyle.Regular)
        frmConnection.Label1.Text = "Timeout Occurred"
        frmConnection.btnCancel.Visible = False
        frmConnection.btnOK.Visible = False
        frmConnection.btnTimeoutRetry.Visible = True
        frmConnection.btnTimeoutRetry.Enabled = True
        mfrmconnector.btnTimeoutCancel.Visible = True
        mfrmconnector.btnTimeoutCancel.Enabled = True
        mfrmconnector.Refresh()
        Cursor.Current = Cursors.Default
        mfrmconnector.ShowDialog()
        'At this point either of the option is selected.
        'Reset the buttons and label text to default so as not to disturb reconnect form display.
        mfrmconnector.btnTimeoutRetry.Visible = False
        mfrmconnector.btnTimeoutRetry.Enabled = False
        mfrmconnector.btnTimeoutCancel.Visible = False
        mfrmconnector.btnTimeoutCancel.Enabled = False
        mfrmconnector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        mfrmconnector.Label1.Text = "Unable to Connect to Controller"
        'if cancel selected during timeout
        If mfrmconnector.TimeoutCancel Then
            oAppMain.bRetryAtTimeout = False
            Return True
        Else
            oAppMain.bRetryAtTimeout = True
            Return False
        End If
    End Function

    ''' <summary>
    ''' tries to establish connection with controller 
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns>booleon</returns>
    Public Function EstablishConnection() As Boolean
        'Reset the Cancel Status
        bCancelReconnect = False
        openConnectionForm()
        bConnectionComplete = False
        CurrentStamp = DateAndTime.Now
        'starting a new thread for reconnection
        Dim ReconnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(msReconnectMessage, CurrentStamp), Threading.ThreadStart))
        bConnected = False

        ReconnectThread.Start()
        WaitForRetrys()

        If (oAppMain.bMCFEnabled And Not bConnected And Not bCancelReconnect) Then

            mfrmconnector.Hide()

            CurrentStamp = DateAndTime.Now
            bConnected = False
            If fConnectAlternateInRF() Then

                openConnectionForm()

                bConnectionComplete = False
                CurrentStamp = DateAndTime.Now
                'starting a new thread for reconnection
                Dim AlternateConnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(msReconnectMessage, CurrentStamp), Threading.ThreadStart))
                bConnected = False
                AlternateConnectThread.Start()
                WaitForRetrys()

            Else
                'Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
                'Return bConnected
            End If
        End If
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
        Return bConnected

    End Function

    Public Sub OpenConnectionForm()
        If mfrmconnector Is Nothing Then
            mfrmconnector = New frmConnection()
        End If
        mfrmconnector.cancelled = False
        'Retry Message
        msReconnectMessage = ""
        'Reset the Retry Time
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        Dim bTemp As Boolean = False
        'Display the form here
        Cursor.Current = Cursors.Default
        If oAppMain.bRetryAtTimeout Then     '  Timeout changes start
            mfrmconnector.btnCancel.Visible = False   '  To avoid displaying cancel
            mfrmconnector.btnCancel.Enabled = False   '  button in the message box
            msReconnectMessage = "Retry {0} of 3 to reconnect. Please wait until successful."
        Else
            msReconnectMessage = GetRetryMessage()    '  displayed for reconnection
            mfrmconnector.btnCancel.Visible = True    '  after a data time out has occurred
            mfrmconnector.btnCancel.Enabled = True    '  
        End If
        mfrmconnector.btnOK.Visible = False
        mfrmconnector.btnOK.Enabled = False
        'updating the status message in the retry message box
        UpdateStatus(msReconnectMessage, 1)
        mfrmconnector.Show()
        Application.DoEvents()
    End Sub

    Public Sub WaitForRetrys()
        While bConnectionComplete = False
            Application.DoEvents()
            'checking if user has cancelled the Retry attempts
            If mfrmconnector.cancelled Then
                If Not bCancelReconnect Then
                    bCancelReconnect = True
                    'if user Cancelled retry then come out 
                    Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.BelowNormal
                    'Return False
                End If
                Threading.Thread.Sleep(ConfigKey.CANCEL_SLEEP_TIME)
            End If
        End While
    End Sub


    ''' <summary>
    ''' handles the retry attempts to reconnect with the controller. 
    ''' </summary>
    ''' <param name="ReconnectMessage">message to be displayed</param>
    ''' <param name="CurrentTryStamp">Current date</param>
    ''' <remarks></remarks>
    ''' <returns>Reconnection status</returns>
    Public Function ReconnectionHandler(ByVal ReconnectMessage As String, ByVal CurrentTryStamp As Date) As Boolean
        Dim iReconnectTime As Integer = 1
        'checking the retry attempts .. max 3 attempts
        Do While ((iReconnectTime <= ConfigKey.RECONNECT_ATTEMPTS) And _
                  (CurrentTryStamp = CurrentStamp)) And bCancelReconnect = False
            If Not bCancelReconnect Then
                'update the status of the retry message box with the current retry attempt no.
                UpdateStatus(ReconnectMessage, iReconnectTime)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
                'checking if controller can be connected from device or not
                If Connect(oAppMain.cActiveIP, miIPPort) Then
                    bConnected = Connected()
                Else
                    bConnected = False
                End If
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
                'if reconnected then come out of the retry attempts
                If bConnected Then Exit Do
                Threading.Thread.Sleep(miWaitTimeBeforeReconnect)
                iReconnectTime = iReconnectTime + 1
            Else
                Exit Do
            End If
        Loop
        bConnectionComplete = True
    End Function

    Public Sub UpdateStatus(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)
        Try
            If mfrmconnector.InvokeRequired Then
                mfrmconnector.Invoke(New UpdateStatusCallback(AddressOf UpdateStatus), Connectivity_Message, RetryAttempt)
            Else
                If RetryAttempt <= 3 Then
                    mfrmconnector.setStatus(String.Format(Connectivity_Message, RetryAttempt.ToString()))
                End If
                Application.DoEvents()
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Function to return the retry message when trying to reconnect to the controller
    ''' when called from another thread.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns>Retry message - Retry {0} of 3 to reconnect.</returns>
    Public Function GetRetryMessage() As String
        Dim bReconnectMsge As String = ""
        Dim strRcnMessage As String = "Retry {0} of 3 to reconnect. " + _
                                  "Please wait until successful or select Cancel to quit. "

        strRcnMessage = strRcnMessage + bReconnectMsge
        Return strRcnMessage
    End Function

    ''' <summary>
    ''' Function to return the failure message when unable to reconnect to the controller
    ''' when called from another thread.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <returns>Reconnect message</returns>
    Public Function RetryFailMessage() As String
        Dim ReconnectMessage As String = "Unable to regain connectivity. "
        Select Case oAppMain.enActiveModule
            Case AppMain.ACTIVEMODULE.USERAUTH
                'ReconnectMessage = "Unable to sign on to controller. "
            Case AppMain.ACTIVEMODULE.MOVEPUTAWAY
                'ReconnectMessage += "The Location of the Last Parcel scanned will not be updated "
                ReconnectMessage += MessageManager.GetInstance.GetMessage("M26")
            Case Else
                'ReconnectMessage += "The Last Parcel scanned will not be booked in "
                ReconnectMessage += MessageManager.GetInstance.GetMessage("M27")
        End Select
        ReconnectMessage += " Select OK to continue."

        Return ReconnectMessage
    End Function

    Public Sub CloseSession()
        moConnectionManager = Nothing
    End Sub

End Class
