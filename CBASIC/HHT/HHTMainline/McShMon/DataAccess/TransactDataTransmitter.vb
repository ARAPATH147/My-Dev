Imports System.IO
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Net
Imports System.Net.Sockets
Imports MCShMon.Message

'''****************************************************************************
''' <FileName>ExDataTransmitter.vb</FileName>
''' <summary>
''' Used for transmitting the export data present in the MC70 device to the 
''' EPOS controller.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'''****************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes for connecting to alternate controller while primary is down.
''' </Summary>
'''****************************************************************************
Public Class TransactDataTransmitter
    Private m_ExDatFilePath As String = Nothing
    Private aExDataRecords() As String = Nothing
#If NRF Then
    Private m_SockectConnMgr As SocketConnectionMgr = Nothing
    Private iNakErrorFlag As Boolean = False
#End If

    Private m_ListID As String = Nothing
    Private m_BootsCode As String = Nothing
    Private m_CreateListID As String = Nothing
    Private m_Retry As Integer = 0
    Private m_ControllerDateTime As String = Nothing
#If RF Then
    Private m_connector As frmConnector
    Private m_TransactMessageParser As TransactMessageParser = Nothing
    Private m_rfSocketManager As RFSocketManager
    Private m_socketSentText As String
    Private m_socketReceivedText As String
    Private status As DATAPOOL.ConnectionStatus
    Private m_status As DATAPOOL.ConnectionStatus
    Private m_WaitTimeBeforeReconnect As Integer
    Private bCancelReconnect As Boolean = False
    Private bConnectionCompleted As Boolean = False
    Private CurrentStamp As Date
    Private Delegate Sub UpdateStatusCallback(ByVal Connectivity_Message As String, ByVal RetryAttempt As Integer)
    Private bAlternate As Boolean = False ' v1.1 MCF Change
#End If

    'System time structure used to set system time.
    Public Structure SYSTEMTIME
        Public wYear As Short
        Public wMonth As Short
        Public wDayOfWeek As Short
        Public wDay As Short
        Public wHour As Short
        Public wMinute As Short
        Public wSecond As Short
        Public wMilliseconds As Short
    End Structure
    'To get system time.
    <DllImport("coredll.dll")> _
    Public Shared Sub GetSystemTime(ByRef lpSystemTime As SYSTEMTIME)
    End Sub
    'P/Invoke dec for setting the system time
    <DllImport("coredll.dll")> _
    Private Shared Function SetLocalTime(ByRef time As SYSTEMTIME) As Boolean
    End Function
    Public Sub EndSession()
#If RF Then
        m_connector = Nothing
        m_TransactMessageParser = Nothing
        m_rfSocketManager = Nothing
#ElseIf NRF Then
        m_SockectConnMgr = Nothing
#End If
    End Sub
#If RF Then
    Public Function ConnectionStatus() As Boolean
        If Not m_rfSocketManager Is Nothing Then
            Return m_rfSocketManager.Connected
        Else
            Return False
        End If
    End Function
#End If


    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        'Get export data file name.
        m_ExDatFilePath = ConfigDataMgr.GetInstance(). _
                          GetParam(ConfigKey.EXPORT_FILE_PATH).ToString()

#If NRF Then
        'Initialise the socket connection.
        m_SockectConnMgr = New SocketConnectionMgr()
#End If

#If RF Then
        m_WaitTimeBeforeReconnect = Convert.ToInt16(ConfigDataMgr.GetInstance().GetParam(ConfigKey.WAIT_TIME_BEFORE_RECONNECT).ToString)
        m_connector = New frmConnector()
        m_connector.Location = New System.Drawing.Point(7, 65)
        bAlternate = False
        'm_rfSocketManager = New RFSocketManager(ConfigDataMgr.GetInstance(). _
        '              GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString(), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
        'Initialize socket with active controller
        m_rfSocketManager = New RFSocketManager(objAppContainer.strActiveIP.ToString(), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
        ConnectToSocket()
        If DATAPOOL.getInstance.WaitForConnection = DATAPOOL.ConnectionStatus.Disconnected Then
            'v1.1 MCF Change
            'If MCF enabled
            If objAppContainer.bMCFEnabled Then
                frmSplashScreen.Enabled = False
                'Prompt the user to connect to alternate or not
                If fConnectAlternateInRF() Then
                    'If the user wishes to connect then reset variable
                    frmSplashScreen.Enabled = True
                    m_rfSocketManager = Nothing
                    bAlternate = True
                    DATAPOOL.getInstance.EndSession()
                    'Change the screen label
                    objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                    'Initialize the socket with current active controller(Alternate)
                    m_rfSocketManager = New RFSocketManager(objAppContainer.strActiveIP.ToString(), + _
                                                            ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
                    ConnectToSocket()
                    m_status = DATAPOOL.getInstance.WaitForConnection
                    If m_status = DATAPOOL.ConnectionStatus.Disconnected Then
                        bAlternate = False
                    ElseIf m_status = DATAPOOL.ConnectionStatus.Connected Then
                        ConfigDataMgr.GetInstance.SetActiveIP()
                    End If
                Else
                    frmSplashScreen.Enabled = True
                End If
            End If
        Else
            bAlternate = True
        End If
        If Not bAlternate Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M60"), "Connection Error", _
                               MessageBoxButtons.OK, _
                               MessageBoxIcon.Exclamation, _
                               MessageBoxDefaultButton.Button2)
            Throw New Exception("Unable to Connect")
        End If
        objAppContainer.objLogger.WriteAppLog("Initialising Parser")
        'Initialize the TransactMessage parser
        m_TransactMessageParser = New TransactMessageParser()
#End If

        'Get retry attempt to send export data.
        m_Retry = Macros.WRITE_RETRY
    End Sub

#If RF Then
    Public Function ModuleReconnect(ByVal ConnectionLostScenario As SMTransactDataManager.CurrentOperation, ByRef LastActiveModule As AppContainer.ACTIVEMODULE, ByRef BufferData As Object, Optional ByVal bIsReconnectingAfterDataTimeout As Boolean = False) As Boolean
        'Fix: isStartRecord Introduced - to check whether to call for end session or not
        bCancelReconnect = False
        m_connector.cancelled = False
        If EstablishConnection(ConnectionLostScenario, bIsReconnectingAfterDataTimeout) Then
            m_connector.Hide()
            'Sleep to prevent quick disappearing of Connector 
            Threading.Thread.Sleep(2000)
            'v1.1 MCF Change
            If objAppContainer.iConnectedToAlternate = 1 Then
                'If the reconnect successful then set the Active IP to secondary controller IP
                ConfigDataMgr.GetInstance.SetActiveIP()
            End If
            Return True
        ElseIf objAppContainer.iConnectedToAlternate <> 0 Then
            'If the connection to alternate is attempted and not sucessful
            If bCancelReconnect Then
                'IF the Socket connection is established after the user clicked on Cancel
                'Explicidly close the socket
                If Not m_rfSocketManager Is Nothing Then
                    m_rfSocketManager.Disconnect()
                End If
                m_rfSocketManager = Nothing
                Cursor.Current = Cursors.Default
                'Close the session
                fCloseSession(ConnectionLostScenario)
                m_connector.Visible = False
                'Connection was not established
                'm_connector.lblMessage.Text = "Unable to regain connectivity"
                'm_connector.btnCancel.Visible = False
                'm_connector.btnCancel.Enabled = False
                'm_connector.btnOK.Visible = True
                'm_connector.btnOK.Enabled = True
                'm_connector.Refresh()
                'Cursor.Current = Cursors.Default
                'm_connector.Location = New System.Drawing.Point(7, 65)
                'm_connector.ShowDialog()
                'm_connector.Visible = False
                'If cancelled the connectivity roll back the change in IP
                m_connector.Hide()
                If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                Else
                    objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                End If
                'objAppContainer.iConnectedToAlternate = 0
            End If
            'Archana : Close the app menu
            If objAppContainer.iConnectedToAlternate = 1 Then
                fCloseSession(ConnectionLostScenario)
                'Archana
                'Close shelfmanagement main menu
                objAppContainer.objShlfMgmntMenu.Close()
            End If
            Return False
        Else
            '******************Time out handle*********************************
            'Incase of time out don't close any application if it fails to reconnect.
            'Just return the status as not connected so that the user can be
            'left in the same screen as they were and display the timeout message.
            If bIsReconnectingAfterDataTimeout Then
                Return False
            End If
            '******************Time out handle end*****************************
            ''******* This section closes all the open sessions in case of Connection loss*************
            'Module which currently active / Displayed on the screen
            Dim CurrentActiveModule As AppContainer.ACTIVEMODULE = objAppContainer.objActiveModule
            'Close all the modules on the stack in case connection is lost
            'Closing the Modules
            LastActiveModule = objAppContainer.objActiveModule
            If objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.SPCEPLAN Then
                'Check whether planner is invoked from Item Info
                'Since Planner Is Invoked from Item Info, the item info start message would have been already sent
                If (SPSessionMgr.GetInstance.isPlannerInvokedFromItemInfo) Then
                    'Check Whether Item Info is Invoked from some other module
                    'Since Item info is started from some other module, INS would have been send before
                    objAppContainer.objLogger.WriteAppLog("Planner Invoked from Item Info", Logger.LogLevel.RELEASE)
                    If (ItemInfoSessionMgr.GetInstance.getItemInfoInvokingModule <> AppContainer.ACTIVEMODULE.ITEMINFO) Then
                        objAppContainer.objLogger.WriteAppLog("Item Info has been invoked from some other module", Logger.LogLevel.RELEASE)
                        'Close the Item Info Invoking Module
                        LastActiveModule = ItemInfoSessionMgr.GetInstance.getItemInfoInvokingModule
                        CloseSession(LastActiveModule, BufferData, ConnectionLostScenario)
                    Else
                        LastActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
                    End If
                    objAppContainer.objLogger.WriteAppLog("Closing Item Info", Logger.LogLevel.RELEASE)
                    CloseSession(AppContainer.ACTIVEMODULE.ITEMINFO, BufferData, ConnectionLostScenario)
                End If
            ElseIf objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO Then
                If Not (ItemInfoSessionMgr.GetInstance.getItemInfoInvokingModule = AppContainer.ACTIVEMODULE.ITEMINFO) Then
                    'Close the Item Info Invoking Module
                    LastActiveModule = ItemInfoSessionMgr.GetInstance.getItemInfoInvokingModule
                    'Before calling any other getinstance closing Item Info
                    'Becoz getinstance will change the App Module
                    CloseSession(LastActiveModule, BufferData, ConnectionLostScenario)
                End If
            End If
            ''*******------------------------------- End of Section------------------------------------------*************

            If Not bCancelReconnect Then
                'Check whether this is a inclusive list
                Dim ReconnectMessage As String = ""
                Select Case LastActiveModule
                    Case AppContainer.ACTIVEMODULE.SHLFMNTR
                        If SMSessionMgr.GetInstance.ISPostNext Then
                            If SMSessionMgr.GetInstance.IsFirstItemActioned Then
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                                      "Your list has been saved. " + _
                                                                      "The last item scanned may not be included. " + _
                                                                      "Select OK to continue."
                            Else
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                                   "The last item scanned may not be included. " + _
                                                                     "Select OK to continue."
                            End If

                        Else
                            ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                        End If
                    Case AppContainer.ACTIVEMODULE.FASTFILL
                        If FFSessionMgr.GetInstance.IsPostNext Then
                            If FFSessionMgr.GetInstance.IsFirstItemActioned Then
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                                     "Your list has been saved. " + _
                                                                     "The last item scanned may not be included. " + _
                                                                     "Select OK to continue."
                            Else
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                                     "The last item scanned may not be included. " + _
                                                                       "Select OK to continue."
                            End If
                        Else
                            ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                        End If
                    Case AppContainer.ACTIVEMODULE.PICKGLST
                        Select Case ConnectionLostScenario
                            Case SMTransactDataManager.CurrentOperation.MODULE_START_RECORD
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Select OK to continue."
                            Case SMTransactDataManager.CurrentOperation.LIST_INITIALISE
                                ReconnectMessage = "Unable to regain connectivity. Select OK to continue."
                            Case SMTransactDataManager.CurrentOperation.LIST_EXIT_xLX
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Your picked items have been saved. " + _
                                          "Select OK to continue."
                            Case SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Select OK to continue."
                            Case Else
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Select OK to continue."
                        End Select
                    Case AppContainer.ACTIVEMODULE.PRICECHK
                        ReconnectMessage = "Unable to regain connectivity. " + _
                                            "Your price checks have been saved. " + _
                                            "The last item checked may not be included. " + _
                                            "Select OK to continue."
                    Case AppContainer.ACTIVEMODULE.CUNTLIST
                        Select Case ConnectionLostScenario
                            Case SMTransactDataManager.CurrentOperation.MODULE_START_RECORD
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                     "Select OK to continue."
                            Case SMTransactDataManager.CurrentOperation.LIST_INITIALISE
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                  "Select OK to continue."
                            Case SMTransactDataManager.CurrentOperation.LIST_EXIT_xLX
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                "Your count list has been saved. " + _
                                "The Stock File may not be updated. Select OK to continue."
                            Case SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Select OK to continue."
                            Case Else
                                If CLSessionMgr.GetInstance.IsItTheFirstItemToBeActioned Then
                                    ReconnectMessage = "Unable to regain connectivity. " + _
                                                     "The item counted may not be included. " + _
                                                     "Select OK to continue."
                                Else
                                    ReconnectMessage = "Unable to regain connectivity. " + _
                                 "Your count list has been saved. " + _
                                 "The last item may not be counted. Select OK to continue."
                                End If
                        End Select
                    Case AppContainer.ACTIVEMODULE.EXCSSTCK
                        If EXSessionMgr.GetInstance.IsPostNext Then
                            If EXSessionMgr.GetInstance.IsFirstItemActioned Then
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                                    "Your list has been saved. " + _
                                                                    "The last item scanned may not be included. " + _
                                                                    "Select OK to continue."
                            Else
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                                                     "The last item scanned may not be included. " + _
                                                                       "Select OK to continue."
                            End If
                        Else
                            ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                        End If
                    Case AppContainer.ACTIVEMODULE.SPCEPLAN
                        If ConnectionLostScenario = SMTransactDataManager.CurrentOperation.PRINT_SELECTION Then
                            ReconnectMessage = "Unable to regain connectivity. " + _
                                                                   "Any SEL requests have been saved. " + _
                                                                   "The last SEL request may not be included. " + _
                                                                   "Select OK to continue."
                        Else
                            ReconnectMessage = "Unable to regain connectivity. Any SEL requests have been saved. Select OK to continue."
                        End If

                    Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                        If ConnectionLostScenario = SMTransactDataManager.CurrentOperation.PRINT_SELECTION Then
                            ReconnectMessage = "Unable to regain connectivity. Any SEL requests have been saved. " + _
                                                "The last SEL request may not be included. Select OK to continue."
                        Else
                            ReconnectMessage = "Unable to regain connectivity. Any SEL requests have been saved. " + _
                                                "Select OK to continue."
                        End If

                    Case AppContainer.ACTIVEMODULE.STORESALES
                        ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                    Case AppContainer.ACTIVEMODULE.ITEMSALES
                        ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                    Case AppContainer.ACTIVEMODULE.REPORTS
                        ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                    Case AppContainer.ACTIVEMODULE.ITEMINFO
                        If ItemInfoSessionMgr.GetInstance.PostDetailsSent Then
                            If ConnectionLostScenario = SMTransactDataManager.CurrentOperation.PRINT_SELECTION Then
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Last SEL request may not be saved. " + _
                                         "Select OK to continue."
                            ElseIf ConnectionLostScenario = SMTransactDataManager.CurrentOperation.TSF_MODIFICATION Then
                                ReconnectMessage = "Unable to regain connectivity. " + _
                                         "Last TSF request may not be saved. " + _
                                        "Select OK to continue."
                            Else
                                ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                            End If
                        Else
                            ReconnectMessage = "Unable to regain connectivity.  Select OK to continue."
                        End If
                    Case AppContainer.ACTIVEMODULE.PRINTSEL
                        ReconnectMessage = "Unable to regain connectivity.  " + _
                                            "Your SEL requests have been saved. " + _
                                            "The last SEL request may not be included. " + _
                                            "Select OK to continue."
                    Case AppContainer.ACTIVEMODULE.PRTCLEARANCE

                        ReconnectMessage = "Unable to regain connectivity. Select OK to continue."
                    Case Else
                        ReconnectMessage = "Unable to regain connectivity. Select OK to continue."
                End Select

                If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                    CloseSession(CurrentActiveModule, BufferData, ConnectionLostScenario)
                End If

                objAppContainer.objLogger.WriteAppLog("Unable to regain connectivity", Logger.LogLevel.RELEASE)
                m_connector.lblMessage.Text = ReconnectMessage
                m_connector.btnCancel.Visible = False
                m_connector.btnCancel.Enabled = False
                m_connector.btnOK.Visible = True
                m_connector.btnOK.Enabled = True
                m_connector.Refresh()
                Cursor.Current = Cursors.Default
                m_connector.Location = New System.Drawing.Point(7, 65)
                m_connector.ShowDialog()
                m_connector.Visible = False
                'Connection was not established
                Return False
            Else
                'IF the Socket connection is established after the user clicked on Cancel
                'Explicidly close the socket
                If Not m_rfSocketManager Is Nothing Then
                    m_rfSocketManager.Disconnect()
                End If
                m_rfSocketManager = Nothing
                Cursor.Current = Cursors.Default
                CloseSession(CurrentActiveModule, BufferData, ConnectionLostScenario)
                m_connector.Visible = False
                'objAppContainer.objShlfMgmntMenu.Close()
                'Application.Exit()
            End If
        End If
    End Function

    Private Sub CloseSession(ByVal ActiveModule As AppContainer.ACTIVEMODULE, ByRef BUFFERDATA As Object, ByRef CurrentScenario As SMTransactDataManager.CurrentOperation)
        Select Case ActiveModule
            Case AppContainer.ACTIVEMODULE.SHLFMNTR
                'Donot Reset/ Write Buffer data in case of Start Session
                If CurrentScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD And BUFFERDATA Is Nothing Then
                    BUFFERDATA = SMSessionMgr.GetInstance.GenerateGAX()
                End If
                If (Not BUFFERDATA Is Nothing) And (TypeOf (BUFFERDATA) Is GARRecord) Then
                    objAppContainer.objLogger.WriteAppLog("Generated GAX", Logger.LogLevel.RELEASE)
                End If
                objAppContainer.objLogger.WriteAppLog("Closing SM", Logger.LogLevel.RELEASE)
                SMSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.FASTFILL
                'Donot Reset/ Write Buffer data in case of Start Session
                If CurrentScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD And BUFFERDATA Is Nothing Then
                    BUFFERDATA = FFSessionMgr.GetInstance.GenerateGAX()
                End If
                If (Not BUFFERDATA Is Nothing) And TypeOf (BUFFERDATA) Is GARRecord Then
                    objAppContainer.objLogger.WriteAppLog("Generated GAX", Logger.LogLevel.RELEASE)
                End If
                objAppContainer.objLogger.WriteAppLog("Closing FF")
                FFSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.PICKGLST
                If CurrentScenario <> SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF And BUFFERDATA Is Nothing Then
                    BUFFERDATA = PLSessionMgr.GetInstance.GeneratePLX()
                    If TypeOf (BUFFERDATA) Is PLXRecord Then
                        objAppContainer.objLogger.WriteAppLog("Generated PLX", Logger.LogLevel.RELEASE)
                    End If
                End If
                objAppContainer.objLogger.WriteAppLog("Closing PL")
                PLSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.PRICECHK
                objAppContainer.objLogger.WriteAppLog("Closing PC")
                PCSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.CUNTLIST
                If CurrentScenario <> SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF And BUFFERDATA Is Nothing Then
                    BUFFERDATA = CLSessionMgr.GetInstance.HandleConnectionLossGenerateCLX()
                    If TypeOf (BUFFERDATA) Is CLXRecord Then
                        objAppContainer.objLogger.WriteAppLog("Generated CLX", Logger.LogLevel.RELEASE)
                    End If
                End If
                objAppContainer.objLogger.WriteAppLog("Closing CL")
                CLSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.EXCSSTCK
                'Donot Reset/ Write Buffer data in case of Start Session
                If CurrentScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD And BUFFERDATA Is Nothing Then
                    Dim objExGaxRecord As New EX_GAX_Record
                    objExGaxRecord.exGAXRecord = EXSessionMgr.GetInstance.GenerateGAX()
                    objExGaxRecord.currLocation = EXSessionMgr.GetInstance.CurrentLocation()
                    BUFFERDATA = objExGaxRecord
                    objExGaxRecord = Nothing
                End If
                If (Not BUFFERDATA Is Nothing) And TypeOf (BUFFERDATA) Is EX_GAX_Record Then
                    objAppContainer.objLogger.WriteAppLog("Generated GAX", Logger.LogLevel.RELEASE)
                End If
                objAppContainer.objLogger.WriteAppLog("Closing EX")
                EXSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.SPCEPLAN
                objAppContainer.objLogger.WriteAppLog("Closing Space Plan")
                SPSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                objAppContainer.objLogger.WriteAppLog("Closing Pending Plan")
                SPSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.STORESALES
                objAppContainer.objLogger.WriteAppLog("Closing Store Sales")
                SSSessionManager.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.ITEMSALES
                objAppContainer.objLogger.WriteAppLog("Closing Item Sales")
                ISSessionManager.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.REPORTS
                objAppContainer.objLogger.WriteAppLog("Closing Reports")
                ReportsSessionManager.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.ITEMINFO
                objAppContainer.objLogger.WriteAppLog("Closing Item Info")
                ItemInfoSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.PRINTSEL
                objAppContainer.objLogger.WriteAppLog("Closing Print SEL")
                PSSessionMgr.GetInstance.EndSession(True)
            Case AppContainer.ACTIVEMODULE.PRTCLEARANCE
                objAppContainer.objLogger.WriteAppLog("Closing Print Clearence")
                CLRSessionMgr.GetInstance.EndSession(True)
        End Select
    End Sub
    ''' <summary>
    ''' Function to handle timeout when there is no response received from Transact 
    ''' for a predefined amount of time.
    ''' </summary>
    ''' <param name="TimedOutScenario"></param>
    ''' <remarks></remarks>
    Public Function HandleTimeOut(ByVal TimedOutScenario As SMTransactDataManager.CurrentOperation) As Boolean
        Dim strMessage As String = "Unable to communicate with the controller. " + _
                                 "This may be due to controller reload, network or power failure. Once ready " + _
                                 "select RETRY to reconnect or select CANCEL to quit."
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
        m_connector.Location = New System.Drawing.Point(7, 65)
        m_connector.ShowDialog()
        'At this point either of the option is selected.
        'Reset the buttons and label text to default so as not to disturb reconnect form display.
        m_connector.btnTimeoutRetry.Visible = False
        m_connector.btnTimeoutRetry.Enabled = False
        m_connector.btnTimeoutCancel.Visible = False
        m_connector.btnTimeoutCancel.Enabled = False
        m_connector.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular)
        m_connector.Label1.Text = "Unable to Connect to Controller"
        If m_connector.TimeoutCancel Then
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.SHLFMNTR
                    'If SMSessionMgr.GetInstance.ISPostNext Then
                    '    'strMessage = "The application has timed out. " + _
                    '    '        "Your lists have been saved in Picking Lists. " + _
                    '    '        "The last item scanned may not be included. Select OK to continue."
                    '    strMessage = "The application has timed out. " + _
                    '                 "Your list may not be saved in Picking Lists. Select OK to continue."
                    'Else
                    '    strMessage = "The application has timed out. " + _
                    '                 "Select OK to continue."
                    'End If
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SMSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.FASTFILL
                    'If FFSessionMgr.GetInstance.IsPostNext Then
                    '    'strMessage = "The application has timed out. " + _
                    '    '         "Your lists have been saved in Picking Lists. " + _
                    '    '         "The last item scanned may not be included. Select OK to continue."
                    '    strMessage = "The application has timed out. " + _
                    '                 "Your list may not be saved in Picking Lists. Select OK to continue."
                    'Else
                    '    strMessage = "The application has timed out. " + _
                    '            "Select OK to continue."

                    'End If
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        FFSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PICKGLST
                    'strMessage = "The application has timed out. " + _
                    '              "Your picked items have been saved and any " + _
                    '              "unpicked items are in Picking Lists. Select OK to continue."
                    'strMessage = "The application has timed out. " + _
                    '                 "Your list may not be saved in Picking Lists. Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        PLSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PRICECHK
                    'strMessage = "The application has timed out. " + _
                    '                "Your price checks have been saved. Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        PCSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.CUNTLIST
                    'Select Case TimedOutScenario
                    '    Case SMTransactDataManager.CurrentOperation.MODULE_START_RECORD
                    '        strMessage = "The application has timed out. " + _
                    '                             "Select OK to continue."
                    '    Case SMTransactDataManager.CurrentOperation.MODULE_START_RECORD
                    '        strMessage = "The application has timed out. " + _
                    '                       "Select OK to continue."
                    '    Case SMTransactDataManager.CurrentOperation.LIST_EXIT_xLX
                    '        strMessage = "The application has timed out. " + _
                    '        "Your count list has been saved. " + _
                    '        "The Stock File may not be updated. Select OK to continue."
                    '    Case SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF
                    '        strMessage = "The application has timed out. " + _
                    '                          "Select OK to continue."
                    '    Case Else
                    '        If CLSessionMgr.GetInstance.IsItTheFirstItemToBeActioned Then
                    '            strMessage = "The application has timed out. " + _
                    '                      "The item scanned may not be counted. " + _
                    '                        "Select OK to continue."
                    '        Else
                    '            strMessage = "The application has timed out. " + _
                    '                        "Your counts have been saved. The last item scanned may not be counted. " + _
                    '                        "Select OK to continue."
                    '        End If
                    'End Select
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        CLSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.EXCSSTCK
                    'If EXSessionMgr.GetInstance.IsPostNext Then
                    '    'strMessage = "The application has timed out. " + _
                    '    '             "Your lists have been saved in Picking Lists. Select OK to continue."
                    '    strMessage = "The application has timed out. " + _
                    '                 "Your list may not be saved in Picking Lists. Select OK to continue."
                    'Else
                    '    strMessage = "The application has timed out. Select OK to continue."
                    'End If
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        EXSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.SPCEPLAN
                    'strMessage = "The application has timed out. Any SEL requests have been saved. " + _
                    '              "The last SEL request may not be included. Select OK to continue."
                    'strMessage = "The application has timed out. SEL requests may not be saved. Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SPSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                    'strMessage = "The application has timed out. Any SEL requests have been saved. " + _
                    '              "The last SEL request may not be included. Select OK to continue."
                    'strMessage = "The application has timed out. SEL requests may not be saved. Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SPSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.STORESALES
                    'strMessage = "The application has timed out.  Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SSSessionManager.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.ITEMSALES
                    'strMessage = "The application has timed out.  Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        ISSessionManager.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.REPORTS
                    'strMessage = "The application has timed out.  Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        ReportsSessionManager.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.ITEMINFO
                    'strMessage = "The application has timed out.  Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        ItemInfoSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PRINTSEL
                    'If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                    '    PSSessionMgr.GetInstance.EndSession(True)
                    '    strMessage = "The application has timed out.  " + _
                    '             "Your SEL requests have been saved. " + _
                    '             "The last SEL request may not be included  Select OK to continue."
                    'Else
                    '    strMessage = "The application has timed out.  " + _
                    '             "Your SEL requests have been saved. " + _
                    '             "The last SEL request may not be included  Select OK to continue."
                    'End If
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        PSSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PRTCLEARANCE
                    'strMessage = "The application has timed out. Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        CLRSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES
                    'strMessage = "The application has timed out. Select OK to continue."
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        AutoSYSSessionManager.GetInstance.EndSession()
                    End If
                    'strMessage = "The application has timed out. Select OK to continue."
                Case AppContainer.ACTIVEMODULE.ASSIGNPRINTER
                    If TimedOutScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        APSessionMgr.GetInstance.EndSession()
                        'strMessage = "The application has timed out. Select OK to continue."
                    End If
                    'Case Else
                    '    'Other case - User Auth / User Log off
                    '    strMessage = "The application has timed out. Select OK to continue."
            End Select
            'Close shelfmanagement main menu
            objAppContainer.objShlfMgmntMenu.Close()
            'objAppContainer.AppTerminate()
            Return False
        Else
            'Code to reattempt to connect to transact.
            Return True
        End If
    End Function
    ''' <summary>
    ''' Handles Reconnection
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function EstablishConnection(ByVal ConnectionLostScenario As SMTransactDataManager.CurrentOperation, Optional ByVal bIsReconnectingAfterDataTimeout As Boolean = False) As Boolean
        'Reset the connection status every time We establish a connection
        DATAPOOL.getInstance.isConnected = False
        'Reset the Cancel Status
        bCancelReconnect = False
        m_connector.cancelled = False
        'Retry Message
        Dim ReconnectMessage As String = ""
        'Reset the Retry Time
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest

        Dim bTemp As Boolean = False
        'Display the form here
        'm_connector.Invoke(New EventHandler(AddressOf DisplayConnector))
        Cursor.Current = Cursors.Default
        If bIsReconnectingAfterDataTimeout Then     '  Timeout changes start
            m_connector.btnCancel.Visible = False   '  To avoid displaying cancel
            m_connector.btnCancel.Enabled = False   '  button in the message box
            ReconnectMessage = "Retry {0} of 3 to reconnect. Please wait until successful."
        Else
            ReconnectMessage = GetRetryMessage(ConnectionLostScenario) '  displayed for reconnection
            m_connector.btnCancel.Visible = True    '  after a data time out has occurred
            m_connector.btnCancel.Enabled = True    '  
        End If                                      'Changes End
        m_connector.btnOK.Visible = False
        m_connector.btnOK.Enabled = False
        Application.DoEvents()
        'Fix for Default message being shown. 
        'Updaate the status Before displaying it to the user
        'Setting the Retry Attempt as 1 before displaying the Connector
        UpdateStatus(ReconnectMessage, 1)
        m_connector.Show()
        m_rfSocketManager = Nothing
        'here reconnector logic has to come 
        'There should be three reconnect attempts.
        CurrentStamp = DateAndTime.Now
        objAppContainer.objLogger.WriteAppLog("Starting a new Thread for connection")
        Dim ReconnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(ReconnectMessage, CurrentStamp), Threading.ThreadStart))
        bConnectionCompleted = False
        ReconnectThread.Start()
        While Not (bConnectionCompleted)
            Application.DoEvents()
            If m_connector.cancelled Then
                If Not bCancelReconnect Then
                    objAppContainer.objLogger.WriteAppLog("Reconnect Cancelled")
                    bCancelReconnect = True
                    Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.BelowNormal
                    'DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Cancelled)
                    'Return False
                End If
                Threading.Thread.Sleep(Macros.CANCEL_SLEEP_TIME)
            End If
        End While
        'v1.1 MCF Changes
        If (objAppContainer.bMCFEnabled And Not DATAPOOL.getInstance.isConnected And Not bCancelReconnect) Then
            If fConnectAlternateInRF() Then
                'Reset the Retry Time
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
                CurrentStamp = DateAndTime.Now
                objAppContainer.objLogger.WriteAppLog("Starting a new Thread for alternate connection")
                Dim AlternateConnectThread As New Threading.Thread(DirectCast(Function() ReconnectionHandler(ReconnectMessage, CurrentStamp), Threading.ThreadStart))
                bConnectionCompleted = False
                AlternateConnectThread.Start()
                While Not (bConnectionCompleted)
                    Application.DoEvents()
                    If m_connector.cancelled Then
                        If Not bCancelReconnect Then
                            objAppContainer.objLogger.WriteAppLog("Alternate Reconnect Cancelled")
                            bCancelReconnect = True
                            Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.BelowNormal
                        End If
                        Threading.Thread.Sleep(Macros.CANCEL_SLEEP_TIME)
                    End If
                End While
            Else
                bIsReconnectingAfterDataTimeout = True
                fCloseSession(ConnectionLostScenario)
                'Archana
                'Close shelfmanagement main menu
                objAppContainer.objShlfMgmntMenu.Close()
            End If
        End If
        objAppContainer.objLogger.WriteAppLog("Establish Connection END")
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
        Return DATAPOOL.getInstance.isConnected
    End Function
    Public Function ReconnectionHandler(ByVal ReconnectMessage As String, ByVal CurrentTryStamp As Date) As Boolean
        bConnectionCompleted = False
        Dim iReconnectTime As Integer = 1
        objAppContainer.objLogger.WriteAppLog("Entered Reconnection Handler", Logger.LogLevel.RELEASE)
        Do While ((iReconnectTime <= Macros.RECONNECT_ATTEMPTS) And _
                  (CurrentTryStamp = CurrentStamp))
            If Not bCancelReconnect Then
                UpdateStatus(ReconnectMessage, iReconnectTime)
                objAppContainer.objLogger.WriteAppLog("Reconnect Attempt - " + iReconnectTime.ToString(), Logger.LogLevel.RELEASE)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
                objAppContainer.objLogger.WriteAppLog("Initialising Socket", Logger.LogLevel.RELEASE)
                'm_rfSocketManager = New RFSocketManager(ConfigDataMgr.GetInstance(). _
                '     GetParam(ConfigKey.PRIMARY_IPADDRESS).ToString(), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
                m_rfSocketManager = New RFSocketManager(objAppContainer.strActiveIP.ToString(), _
                                                                        ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
                objAppContainer.objLogger.WriteAppLog("SOCKET DEFINED... ESTABLISHING CONNECTION", Logger.LogLevel.RELEASE)
                ConnectToSocket()
                status = DATAPOOL.getInstance.WaitForConnection()
                objAppContainer.objLogger.WriteAppLog("Completed Waiting", Logger.LogLevel.RELEASE)
                Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Normal
                iReconnectTime = iReconnectTime + 1
                'Wat ever might be the status if the cancel is click Quit the current Module
                If status = DATAPOOL.ConnectionStatus.Connected Then
                    objAppContainer.objLogger.WriteAppLog("CONNECTION ESTABLISHED AND TERMINATING THE THREAD", Logger.LogLevel.RELEASE)
                    DATAPOOL.getInstance.isConnected = True
                    'objAppContainer.objLogger.WriteAppLog("Connection was Successfully established", Logger.LogLevel.RELEASE)
                    Exit Do
                Else
                    DATAPOOL.getInstance.isConnected = False
                    m_rfSocketManager = Nothing
                End If
                'If m_ReconnectTime > 3 Then
                'objAppContainer.objLogger.WriteAppLog("Connection was not established, sleep before the Next Attempt", Logger.LogLevel.RELEASE)
                'Check before the Sleep
                If bCancelReconnect Then
                    Exit Do
                End If
                System.Threading.Thread.Sleep(m_WaitTimeBeforeReconnect)
            Else
                Exit Do
            End If
        Loop
        bConnectionCompleted = True
    End Function
    ''' <summary>
    ''' Function to retreive the retry message for any module
    ''' </summary>
    ''' <param name="ConnectionLostScenario"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRetryMessage(ByVal ConnectionLostScenario As SMTransactDataManager.CurrentOperation) As String
        Select Case objAppContainer.objActiveModule
            Case AppContainer.ACTIVEMODULE.SHLFMNTR
                objAppContainer.objLogger.WriteAppLog("The active module is SM and setting message for SM in Connector", Logger.LogLevel.RELEASE)
                If SMSessionMgr.GetInstance.ISPostNext Then
                    Return "Retry {0} of 3 to reconnect. " + _
                                  "Please wait until successful or select Cancel to quit. " + _
                                  "Last item may not be saved. "
                Else
                    Return "Retry {0} of 3 to reconnect. " + _
                                        "Please wait until successful or select Cancel to quit"
                End If
            Case AppContainer.ACTIVEMODULE.FASTFILL
                objAppContainer.objLogger.WriteAppLog("The active module is FF and setting message for FF in Connector", Logger.LogLevel.RELEASE)
                If FFSessionMgr.GetInstance.IsPostNext Then
                    Return "Retry {0} of 3 to reconnect. " + _
                                  "Please wait until successful or select Cancel to quit. " + _
                                  "Last item entered may not be saved. "
                Else
                    Return "Retry {0} of 3 to reconnect. " + _
                                        "Please wait until successful or select Cancel to quit"
                End If
            Case AppContainer.ACTIVEMODULE.PICKGLST
                objAppContainer.objLogger.WriteAppLog("The active module is PL and setting message for PL in Connector", Logger.LogLevel.RELEASE)
                Select Case ConnectionLostScenario
                    Case SMTransactDataManager.CurrentOperation.MODULE_START_RECORD
                        Return "Retry {0} of 3 to reconnect. " + _
                                                  "Please wait until successful or select Cancel to quit. "
                    Case SMTransactDataManager.CurrentOperation.LIST_INITIALISE
                        Return "Retry {0} of 3 to reconnect. " + _
                                                 "Please wait until successful or select Cancel to quit. "
                    Case SMTransactDataManager.CurrentOperation.OTHERS
                        Return "Retry {0} of 3 to reconnect. " + _
                          "Please wait until successful or select Cancel to quit. " + _
                          "Last item scanned may not be saved. "
                    Case SMTransactDataManager.CurrentOperation.LIST_EXIT_xLX
                        Return "Retry {0} of 3 to reconnect. " + _
                          "Please wait until successful or select Cancel to quit. " + _
                        "The Picking List has been Saved."
                    Case SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF
                        Return "Retry {0} of 3 to reconnect. " + _
                              "Please wait until successful or select Cancel to quit. "
                End Select
            Case AppContainer.ACTIVEMODULE.PRICECHK
                objAppContainer.objLogger.WriteAppLog("The active module is PC and setting message for PC in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit. " + _
                                    "Last item scanned may not be checked. "
            Case AppContainer.ACTIVEMODULE.CUNTLIST
                objAppContainer.objLogger.WriteAppLog("The active module is CL and setting message for CL in Connector", Logger.LogLevel.RELEASE)
                Select Case ConnectionLostScenario
                    Case SMTransactDataManager.CurrentOperation.MODULE_START_RECORD
                        Return "Retry {0} of 3 to reconnect. " + _
                                "Please wait until successful or select Cancel to quit. "
                    Case SMTransactDataManager.CurrentOperation.LIST_INITIALISE
                        Return "Retry {0} of 3 to reconnect. " + _
                             "Please wait until successful or select Cancel to quit. "
                    Case SMTransactDataManager.CurrentOperation.LIST_EXIT_xLX
                        Return "Retry {0} of 3 to reconnect. " + _
                                "Please wait until successful or select Cancel to quit. " + _
                                   "Your Count List has been Saved."
                    Case SMTransactDataManager.CurrentOperation.LIST_FINISH_xLF
                        Return "Retry {0} of 3 to reconnect. " + _
                              "Please wait until successful or select Cancel to quit. "
                    Case Else
                        If CLSessionMgr.GetInstance.IsItTheFirstItemToBeActioned Then
                            Return "Retry {0} of 3 to reconnect. " + _
                                         "Please wait until successful or select Cancel to quit. " + _
                                         "The item scanned may not be counted."
                        Else
                            Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit. " + _
                                    "The last item scanned may not be counted."
                        End If
                End Select
            Case AppContainer.ACTIVEMODULE.EXCSSTCK
                objAppContainer.objLogger.WriteAppLog("The active module is ES and setting message for ES in Connector", Logger.LogLevel.RELEASE)
                If EXSessionMgr.GetInstance.IsPostNext Then
                    Return "Retry {0} of 3 to reconnect. " + _
                                      "Please wait until successful or select Cancel to quit. " + _
                                      "Last item may not be saved. "
                Else
                    Return "Retry {0} of 3 to reconnect. " + _
                                        "Please wait until successful or select Cancel to quit. "
                End If
            Case AppContainer.ACTIVEMODULE.SPCEPLAN
                objAppContainer.objLogger.WriteAppLog("The active module is Space Plan and setting message for Space Planner in Connector", Logger.LogLevel.RELEASE)
                If ConnectionLostScenario = SMTransactDataManager.CurrentOperation.PRINT_SELECTION Then
                    Return "Retry {0} of 3 to reconnect. " + _
                                   "Please wait until successful or select Cancel to quit. " + _
                                   "Last SEL request may not be saved"
                Else
                    Return "Retry {0} of 3 to reconnect. " + _
                                   "Please wait until successful or select Cancel to quit. "
                End If
            Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                objAppContainer.objLogger.WriteAppLog("The active module is Pending Planner and setting message for Pending Planners in Connector", Logger.LogLevel.RELEASE)
                If ConnectionLostScenario = SMTransactDataManager.CurrentOperation.PRINT_SELECTION Then
                    Return "Retry {0} of 3 to reconnect. " + _
                                   "Please wait until successful or select Cancel to quit. " + _
                                   "Last SEL request may not be saved"
                Else
                    Return "Retry {0} of 3 to reconnect. " + _
                                   "Please wait until successful or select Cancel to quit. "
                End If
            Case AppContainer.ACTIVEMODULE.STORESALES
                objAppContainer.objLogger.WriteAppLog("The active module is Store Sales and setting message for Store Sales in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit. "
            Case AppContainer.ACTIVEMODULE.ITEMSALES
                objAppContainer.objLogger.WriteAppLog("The active module is Item Sales and setting message for Item Sales in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit"
            Case AppContainer.ACTIVEMODULE.REPORTS
                objAppContainer.objLogger.WriteAppLog("The active module is Reports and setting message for Reports in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit. "
            Case AppContainer.ACTIVEMODULE.ITEMINFO
                objAppContainer.objLogger.WriteAppLog("The active module is Item Info and setting message for Item Info in Connector", Logger.LogLevel.RELEASE)
                If ItemInfoSessionMgr.GetInstance.PostDetailsSent Then
                    If ConnectionLostScenario = SMTransactDataManager.CurrentOperation.PRINT_SELECTION Then
                        Return "Retry {0} of 3 to reconnect.  " + _
                                         "Please wait until successful or select Cancel to quit. " + _
                                         "Last SEL request may not be saved. "
                    ElseIf ConnectionLostScenario = SMTransactDataManager.CurrentOperation.TSF_MODIFICATION Then
                        Return "Retry {0} of 3 to reconnect.  " + _
                                          "Please wait until successful or select Cancel to quit. " + _
                                          "Last TSF update may not be saved"

                    Else
                        Return "Retry {0} of 3 to reconnect. " + _
                                            "Please wait until successful or select Cancel to quit. "
                    End If
                Else
                    Return "Retry {0} of 3 to reconnect. " + _
                                        "Please wait until successful or select Cancel to quit. "
                End If
            Case AppContainer.ACTIVEMODULE.PRINTSEL
                objAppContainer.objLogger.WriteAppLog("The active module is Print SEL and setting message for Print SEL in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                   "Please wait until successful or select Cancel to quit. " + _
                                    "Last SEL request may not have been saved"
            Case AppContainer.ACTIVEMODULE.PRTCLEARANCE
                objAppContainer.objLogger.WriteAppLog("The active module is Print Clearence Label and setting message for Print Cleaarence Label in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit. "
            Case Else
                objAppContainer.objLogger.WriteAppLog("The active module is NOT-AVAILABLE and setting default Message in Connector", Logger.LogLevel.RELEASE)
                Return "Retry {0} of 3 to reconnect. " + _
                                    "Please wait until successful or select Cancel to quit. "
        End Select
    End Function
    ''' <summary>
    ''' Shows the connector
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayConnector()
        m_connector.Visible = True
    End Sub
    ''' <summary>
    ''' Hides the connector
    ''' </summary>
    ''' <remarks>none</remarks>
    Public Sub HideConnector()
        m_connector.Visible = False
    End Sub
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
            m_connector.AutoScaleMode = AutoScaleMode.Dpi
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
                objAppContainer.objLogger.WriteAppLog("Connected to Alternate IP " + objAppContainer.strActiveIP, Logger.LogLevel.RELEASE)
            Else
                objAppContainer.iConnectedToAlternate = -1
                objAppContainer.objLogger.WriteAppLog("Cancelled connect to Alternate ", Logger.LogLevel.RELEASE)
            End If
            Return bConnectToAlternate
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in ConnecttoAlternate " + _
                                                          ex.Message.ToString(), _
                                                          Logger.LogLevel.RELEASE)
        End Try
    End Function
    '' <summary>
    '' Closes the current sessions
    '' </summary>
    '' <param>None </param>
    '' <return> Boolean True if successfully close the session; False otherwise </return>
    Public Function fCloseSession(ByVal ConnectionLostScenario As SMTransactDataManager.CurrentOperation) As Boolean
        Try
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.SHLFMNTR
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SMSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.FASTFILL
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        FFSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PICKGLST
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        PLSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PRICECHK
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        PCSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.CUNTLIST
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        CLSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.EXCSSTCK
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        EXSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.SPCEPLAN
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SPSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SPSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.STORESALES
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        SSSessionManager.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.ITEMSALES
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        ISSessionManager.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.REPORTS
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        ReportsSessionManager.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.ITEMINFO
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        ItemInfoSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PRINTSEL
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        PSSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.PRTCLEARANCE
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        CLRSessionMgr.GetInstance.EndSession(True)
                    End If
                Case AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        AutoSYSSessionManager.GetInstance.EndSession()
                    End If
                Case AppContainer.ACTIVEMODULE.ASSIGNPRINTER
                    If ConnectionLostScenario <> SMTransactDataManager.CurrentOperation.MODULE_START_RECORD Then
                        APSessionMgr.GetInstance.EndSession()
                    End If
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in fCloseSession " + _
                                              ex.Message.ToString(), _
                                              Logger.LogLevel.RELEASE)
        End Try
        Return True
    End Function
#End If

#Region "NRF"


#If NRF Then
    ''' <summary>
    ''' Function to read the content of export data file to string array.
    ''' </summary>
    ''' <param name="m_FileName"></param>
    ''' <returns>Bool
    ''' True - If successfuly opened the file and read the contents.
    ''' False - If any error occurred and the file is not read.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ReadFileToArray(ByVal m_FileName As String) As Boolean
        Dim m_FileReader As StreamReader = Nothing
        Try
            m_FileReader = New StreamReader(m_FileName)
            'Read the export data file until SOR type record is read.
            aExDataRecords = Split(m_FileReader.ReadToEnd(), ControlChars.NewLine)
            'Close the stream reader.
            m_FileReader.Close()
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in reading export data file:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", ex.Message)
            'Return false
            Return False
        End Try
        'return the array.
        Return True
    End Function
    ''' <summary>
    ''' To parse the resonse received from the TRANSACT service and return status
    ''' accordingly.
    ''' </summary>
    ''' <param name="m_ResponseMessage">Response message received from the 
    ''' TRANSACT service</param>
    ''' <returns>Bool
    ''' True - ACK, SNR is received.
    ''' False - NAK is received or any error occurred.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function ParseResponse(ByVal m_ResponseMessage As String, ByVal strRecordSent As String) As Boolean
        m_ResponseMessage = NTrim(m_ResponseMessage)
        'Response received from the controller.
        'new Message Format
        m_ResponseMessage = m_ResponseMessage.SubString(5, m_ResponseMessage.Length - 5)
        'Response received from the controller.
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Response received: " _
                                              & m_ResponseMessage, Logger.LogLevel.RELEASE)
        'Based on the message type parse the response and return the value.
        If m_ResponseMessage.StartsWith("ACK") Then
            'If Response received is ACK
            Return True
            'Based on the message type parse the response and return the value.
        ElseIf m_ResponseMessage.StartsWith("GAR") Then
            'If Response received is GAR
            m_ResponseMessage = m_ResponseMessage.Trim()
            Try
                m_ListID = m_ResponseMessage.Substring(Macros.GAR_LISTID_START_INDEX, _
                                                       Macros.GAR_LISTID_LENGTH)
                'Put the list ID recevied in the configuration file.
                ConfigDataMgr.GetInstance().SetParam(ConfigKey.LIST_ID, _
                                                     Val(m_ListID) + 1)
                Return True

            Catch ex As Exception
                'Add the exception to the device log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                        "Error parsing response message GAR:" & m_ResponseMessage _
                        & ex.Message.ToString(), _
                        Logger.LogLevel.RELEASE)
                Return False
            End Try
        ElseIf m_ResponseMessage.StartsWith("CLB") Then
            'If Response received is GAR
            m_ResponseMessage = m_ResponseMessage.Trim()
            Try
                m_CreateListID = m_ResponseMessage.Substring(Macros.CREATECOUNTID, _
                                                       Macros.CREATECOUNTID_LENGTH)
                'Put the list ID recevied in the configuration file.
                ConfigDataMgr.GetInstance().SetParam(ConfigKey.CREATE_LIST_ID, _
                                                     Val(m_CreateListID) + 1)
                Return True

            Catch ex As Exception
                'Add the exception to the device log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                        "Error parsing response message GAR:" & m_ResponseMessage _
                        & ex.Message.ToString(), _
                        Logger.LogLevel.RELEASE)
                Return False
            End Try
        ElseIf m_ResponseMessage.StartsWith("SNR") Then
            'set the device date time according to the date time received 
            'in the response.
            Dim strPrtNos As String = ""
            Try
                If m_ResponseMessage.Length >= Macros.SNR_DATETIME_START_INDEX + Macros.SNR_DATETIME_LENGTH Then
                    m_ControllerDateTime = m_ResponseMessage.Substring(Macros.SNR_DATETIME_START_INDEX, _
                                                              Macros.SNR_DATETIME_LENGTH)
                    objAppContainer.aPrinterList = m_ResponseMessage.Substring(SNR.PRTDESC_OFFSET).Trim().Split(vbCrLf)
                    strPrtNos = m_ResponseMessage.Substring(SNR.PRTNUM_OFFSET, SNR.PRTNUM).ToString()
                    strPrtNos = strPrtNos.Replace("X", "")
                    objAppContainer.aPrintNos = strPrtNos.Trim().ToCharArray()
                End If
                Return SetDeviceDateTime(m_ControllerDateTime)
            Catch ex As Exception
                'Add the exception to the device log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                        "Error parsing response message SNR:" & m_ResponseMessage _
                        & ex.Message.ToString(), _
                        Logger.LogLevel.RELEASE)
                Return False
            End Try

        ElseIf m_ResponseMessage.StartsWith("EQR") Then
            'EQR is in response to ENQ to update history file.
            Return True
        ElseIf m_ResponseMessage.StartsWith("PCR") Then
            'PCR is in response to PCS (Price Check response Message).
            Return True
        ElseIf m_ResponseMessage.StartsWith("NAK") Then
            strRecordSent = strRecordSent.Substring(5)
            If Not strRecordSent.StartsWith("OFF") Then
                'supress nakerror /s in case of CLD
                'for handling controller unknowns
                If Not (strRecordSent.StartsWith("CLD") And m_ResponseMessage.StartsWith("NAKERROR")) Then
                    Dim strNakMessage As String = ""
                    strNakMessage = m_ResponseMessage.Replace("NAK", "")    'Supress NAK String
                    strNakMessage = strNakMessage.Replace("NAKERROR", "")   'Suppress NAKERROR string
                    'Display the recevied NAK message to the user.
                    MessageBox.Show("Received error from controller:" + strNakMessage, _
                                    "Error", MessageBoxButtons.OK, _
                                    MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                    'Update the file status
                    objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", strNakMessage)
                    'If response received is NAK
                    Return False
                Else
                    iNakErrorFlag = True
                    Return True
                End If
            Else
                Return True
            End If
        ElseIf m_ResponseMessage.StartsWith("CLI") Then
            strRecordSent = strRecordSent.Substring(5)
            If strRecordSent.StartsWith("CLD") Then
                m_BootsCode = m_ResponseMessage.Substring(Macros.CREATEBOOTSCODE, _
                                                           Macros.CREATEBOOTSCODE_LENGTH)
                m_BootsCode = m_BootsCode.TrimStart("0")
            End If
            Return True
        Else
            'If a message other than the above ones is received.
            Return False
        End If
    End Function
    ''' <summary>
    ''' Convert the record to bytes and send to the TRANSACT service.
    ''' Receive the response and parse it to get the details.
    ''' </summary>
    ''' <param name="strRecord">Record to be sent to the TRANSACT</param>
    ''' <returns> Bool
    ''' True if successfully sent and received the records.
    ''' False is any error occurred during send / receive operation.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function SendRecord(ByVal strRecord As String) As Boolean
        Dim m_SendBytes As [Byte]() = Nothing
        Dim m_ReadBytes As [Byte]() = Nothing
        Dim m_Status As Boolean = Nothing
        Dim m_RetryWrite As Integer = 0
        Try
            'Records sent ot the controller.
            objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Record sent: " _
                                                  & strRecord, Logger.LogLevel.RELEASE)

            'new message format
            'strRecord = strRecord.Substring(0, strRecord.Length - 2)
            strRecord = Chr(255) + (strRecord.Length + 5).ToString.PadLeft(4, "0") + strRecord
            m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())
            m_SendBytes(0) = &HFF
            m_RetryWrite = m_Retry
            'Send the record to the controller.

            'Read the rety attempt for writing data to the socket stream.
            m_RetryWrite = Macros.WRITE_RETRY
            'Send the record to the controller.
            Do
                If m_SockectConnMgr.TransmitData(m_SendBytes) Then
                    'Read the response stream from the client.
                    If m_SockectConnMgr.ReadData(m_ReadBytes) And _
                       m_ReadBytes.Length > 0 Then
                        'Return the response after parsing it.
                        Return ParseResponse( _
                                    Encoding.ASCII.GetString(m_ReadBytes, _
                                                             0, _
                                                             m_ReadBytes.Length), strRecord)
                    Else
                        'Add the exception to the application log.
                        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Cannot " _
                                                              & "read from socket.", _
                                                              Logger.LogLevel.RELEASE)
                        'If reading response from the controller is failed.
                        Return False
                    End If
                End If
                m_RetryWrite = m_RetryWrite - 1
            Loop Until m_RetryWrite = 0
            'If all the write attempt failed.
            If m_RetryWrite = 0 Then
                'write the error message to the app log
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                            "Unable to write record to the stream. Retry attempt" _
                            & "failed for" & m_RetryWrite & "times", _
                            Logger.LogLevel.RELEASE)
                Return False
            End If

        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data to controller:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'incase of exception return false.
            Return False
        Finally
            m_SendBytes = Nothing
            m_ReadBytes = Nothing
        End Try
    End Function

    ''' <summary>
    ''' Convert ALR record to bytes and send to the TRANSACT service.
    ''' Receive the response and send the details to the calling function.
    ''' </summary>
    ''' <returns> String
    ''' Message retured by controller in strin format.
    ''' </returns>
    ''' <remarks></remarks>
    Public Function SendALR(ByVal strUserID As String) As String
        Dim strRecord As StringBuilder = Nothing
        Dim strOFFRecord As StringBuilder = Nothing
        Dim m_SendBytes As [Byte]() = Nothing
        Dim m_ReadBytes As [Byte]() = Nothing
        Dim m_Status As Boolean = Nothing
        Dim strResponse As String = Nothing
        Dim m_RetryWrite As Integer = 0
        Dim m_ACK1 As String = Nothing
        Dim m_ACK2 As String = Nothing
        Dim strData As String = Nothing
        Try
            m_ACK1 = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.ACK_1).ToString()
            m_ACK2 = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.ACK_2).ToString()
            'Generate record for OFF
            strOFFRecord = New StringBuilder()
            strOFFRecord.Append("OFF")
            strOFFRecord.Append(strUserID)
            strOFFRecord.Append(Environment.NewLine)

            'GEnerate record for ALR
            strRecord = New StringBuilder()
            strRecord.Append("ALR")
            strRecord.Append(strUserID)
            strRecord.Append("ACTBUILD")
            strData = strRecord.ToString()
            ' strRecord.Append(Environment.NewLine)

            'new message format
            ' m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())


            strData = Chr(255) + (strData.Length + 5).ToString.PadLeft(4, "0") + strData
            m_SendBytes = Encoding.ASCII.GetBytes(strData.ToString())
            m_SendBytes(0) = &HFF
            m_RetryWrite = m_Retry

            ' m_RetryWrite = m_Retry
            'Send the record to the controller.
            'Records sent ot the controller.
            objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Record sent: " _
                                                  & strRecord.ToString(), Logger.LogLevel.INFO)
            Do
                If m_SockectConnMgr.TransmitData(m_SendBytes) Then
                    'Read the response stream from the client.
                    If m_SockectConnMgr.ReadData(m_ReadBytes) And _
                       m_ReadBytes.Length > 0 Then
                        'Return the response after parsing it.
                        strResponse = Encoding.ASCII.GetString(m_ReadBytes, _
                                                        0, _
                                                        m_ReadBytes.Length)
                        'remove the null characters present at the end.
                        strResponse = NTrim(strResponse)
                        'trim the spaces in the trailing end
                        strResponse = strResponse.Trim()
                        'new message format 
                        strResponse = strResponse.Substring(5, strResponse.Length - 5)
                        'Response received from the controller.
                        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Response received: " _
                                                              & strResponse, Logger.LogLevel.INFO)
                        If strResponse.StartsWith(m_ACK1) Then
                            Return "ACK1"
                        ElseIf strResponse.StartsWith(m_ACK2) Then
                            Return "ACK2"
                        ElseIf strResponse.StartsWith("NAK") Then
                            strResponse = strResponse.Replace("NAK", "")
                            'Display the recevied NAK message to the user.
                            MessageBox.Show("Received NAK :" + strResponse, _
                                            "NAK Received", MessageBoxButtons.OK, _
                                            MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                            Return "NAK"
                        End If

                    Else
                        'If the device does not read data successfully
                        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Data " _
                                                              & "receive failure", _
                                                              Logger.LogLevel.RELEASE)
                        'If reading response from the controller is failed.
                        Return "NAK"
                    End If
                End If
                m_RetryWrite = m_RetryWrite - 1
            Loop Until m_RetryWrite = 0

            If m_RetryWrite = 0 Then
                'Add the exception to the application log.
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                                "Error sending data to controller", _
                                                Logger.LogLevel.RELEASE)
                Return "NAK"
            End If
            'Send OFF record and close the transaction.
            SendRecord(strOFFRecord.ToString())
        Catch ex As Exception
            'Add the exception to the application log.
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            'incase of exception return false.
            Return "NAK"
        Finally
            'Clear the variable memories.
            m_SendBytes = Nothing
            m_ReadBytes = Nothing

            'Close the socket connection to the controller.
            m_SockectConnMgr.TerminateConnection()

        End Try
    End Function
    ''' <summary>
    ''' Gets and return the status of the socket connection established
    ''' with the controller.
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function GetSocketStatus() As Boolean
        Return m_SockectConnMgr.ConnectionStatus
    End Function
    ''' <summary>
    ''' To send SOR record before sending ALR record.
    ''' </summary>
    ''' <param name="strRecords"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSOR(ByVal strRecords As String, ByRef strDateTime As String) As Boolean
        strRecords = strRecords.Replace(",", "")
        If SendRecord(strRecords) Then
            'Set controller date time
            strDateTime = m_ControllerDateTime
            'insert parsing characters accordingly
            strDateTime = strDateTime.Insert(4, "-")
            strDateTime = strDateTime.Insert(7, "-")
            strDateTime = strDateTime.Insert(10, " ")
            strDateTime = strDateTime.Insert(13, ":")
            strDateTime = strDateTime.Insert(16, ":")
            strDateTime = strDateTime & "00"
            'return the status
            Return True
        Else
            Return False
        End If
    End Function
#End If
#End Region
    ''' <summary>
    ''' Truncates a string at the first occurrence of the null character.
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function NTrim(ByVal Text As String) As String
        Dim Pos As Long
        Try
            Pos = InStr(Text, vbNullChar)

            If Pos > 0 Then
                NTrim = Left$(Text, Pos - 1)
            Else
                NTrim = Text
            End If
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data to controller:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
        End Try
    End Function

    ''' <summary>
    ''' To set device time to same as controller time.
    ''' </summary>
    ''' <param name="strDateTime">Datetime string recevived from controller</param>
    ''' <returns>
    ''' True - If successfully set the device time.
    ''' False - If error in setting the device time.
    ''' </returns>
    ''' <remarks></remarks>
    Private Function SetDeviceDateTime(ByVal strDateTime As String) As Boolean
        Dim objSysTime As SYSTEMTIME
        'Get the device time.
        GetSystemTime(objSysTime)
        'Populate structure to update the table.
        With objSysTime
            .wYear = Convert.ToUInt16(strDateTime.Substring(0, 4))
            .wMonth = Convert.ToUInt16(strDateTime.Substring(4, 2))
            .wDay = Convert.ToUInt16(strDateTime.Substring(6, 2))
            .wHour = Convert.ToUInt16(strDateTime.Substring(8, 2))
            .wMinute = Convert.ToUInt16(strDateTime.Substring(10, 2))
            .wSecond = Convert.ToUInt16(0)
        End With

        'Set the new time`
        Return SetLocalTime(objSysTime)
    End Function

#If NRF Then
    ''' <summary>
    ''' Download export data record from device to the controller.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DownloadExData() As String
        Dim iCount As Integer = 0
        Dim m_Line As String = Nothing
        Dim m_Status As Boolean = Nothing
        Dim m_TempLine1 As String = Nothing
        Dim m_TempLine2 As String = Nothing
        Dim m_TempLine3 As String = Nothing
        Dim m_TempLine4 As String = Nothing
        Dim m_Ip As String = Nothing
        Dim iCountListSeq As Integer
        Try
            'Read export data file contents to an array.
            m_Status = ReadFileToArray( _
                            m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
            If m_Status And aExDataRecords.Length > 3 Then
                'Read the export data content till the end of the array list.
                While iCount < aExDataRecords.Length And _
                        aExDataRecords(iCount) <> Nothing
                    m_Line = aExDataRecords(iCount)
                    'Replace all commas in a line with no space.
                    m_Line = m_Line.Replace(",", "")
                    If m_Line.StartsWith("GAS") Then
                        'Send export data for SM, Fast Fill and Excess Stock
                        'Get the List ID from the response and use it for the
                        'GAP messages to be sent.
                        If Not SendRecord(m_Line) Or m_ListID = Nothing Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "List ID not received for GAS message. ", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            'Failed to send the record or receive a list ID.
                            Return "-1"
                        End If

                        'Send all the records until GAX record is read.
                        Do
                            iCount = iCount + 1
                            m_Line = aExDataRecords(iCount).ToString()
                            'remove the , comma character from the each record line.
                            m_Line = m_Line.Replace(",", "")
                            If m_Line.StartsWith("GAP") Or m_Line.StartsWith("GAX") Then
                                'Substitute the List ID in GAP record to be sent.
                                'Replace the existing list ID with the new list ID from controller.
                                m_TempLine1 = m_Line.Substring(0, 6)
                                m_TempLine2 = m_Line.Substring(9)
                                m_Line = m_TempLine1 + m_ListID + m_TempLine2
                            End If
                            'Return false if error occurred while transmitting.
                            If Not SendRecord(m_Line) Then
                                'Add the exception to the device log.
                                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            "Error in sending export data to controller:" _
                                            & "Record" & m_Line & "not sent. ", _
                                            Logger.LogLevel.RELEASE)
                                'Update the file status
                                objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                                           "Failed to send record: " & _
                                                                           m_Line)
                                Return "-1"
                            End If
                        Loop Until m_Line.StartsWith("GAX")
                    ElseIf m_Line.StartsWith("CLA") And m_Line.EndsWith("S") Then
                        iCountListSeq = 0  'Initialse seq no to zero for each list
                        'Send export data for Create own List
                        'Get the List ID from the response and use it for the
                        'GLD ,CLC and CLX messages to be sent.
                        If Not SendRecord(m_Line) Or m_CreateListID = Nothing Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "List ID not received for CLA message. ", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            'Failed to send the record or receive a list ID.
                            Return "-1"
                        End If

                        'Send all the records until end record is read.
                        Do
                            iCount = iCount + 1
                            m_Line = aExDataRecords(iCount).ToString()
                            'remove the , comma character from the each record line.
                            m_Line = m_Line.Replace(",", "")
                            If m_Line.StartsWith("CLD") Then
                                If iNakErrorFlag Then
                                    iNakErrorFlag = False
                                Else
                                    iCountListSeq += 1
                                End If
                                'Substitute the List ID in CLD record to be sent.
                                'Replace the existing list ID with the new list ID from controller.
                                m_TempLine1 = m_Line.Substring(0, 3)
                                m_TempLine2 = m_Line.Substring(9)
                                m_Line = m_TempLine1 + m_CreateListID + iCountListSeq.ToString().PadLeft(3, "0") + _
                                                m_TempLine2
                            ElseIf m_Line.StartsWith("CLC") Then
                                'Substitute the List ID in CLC record to be sent.
                                'Replace the existing list ID with the new list ID from controller.
                                If Not iNakErrorFlag Then
                                    m_TempLine1 = m_Line.Substring(0, 6)
                                    m_TempLine2 = m_Line.Substring(12)
                                    m_Line = m_TempLine1 + m_CreateListID + iCountListSeq.ToString().PadLeft(3, "0") + _
                                                m_TempLine2

                                    m_TempLine3 = m_Line.Substring(0, 12)
                                    m_TempLine4 = m_Line.Substring(19)
                                    m_Line = m_TempLine3 + m_BootsCode + m_TempLine4
                                Else
                                    Continue Do
                                End If
                            ElseIf m_Line.StartsWith("CLX") Then
                                'Substitute the List ID in CLX record to be sent.
                                'Replace the existing list ID with the new list ID from controller.
                                m_TempLine1 = m_Line.Substring(0, 3)
                                m_TempLine2 = m_Line.Substring(6)
                                m_Line = m_TempLine1 + m_CreateListID + m_TempLine2
                            ElseIf m_Line.StartsWith("CLA") And m_Line.EndsWith("X") Then
                                'Substitute the List ID in CLX record to be sent.
                                'Replace the existing list ID with the new list ID from controller.
                                m_TempLine1 = m_Line.Substring(0, 6)
                                m_TempLine2 = m_Line.Substring(9)
                                m_Line = m_TempLine1 + m_CreateListID + m_TempLine2
                            End If
                            'Return false if error occurred while transmitting.
                            If Not SendRecord(m_Line) Then
                                'Add the exception to the device log.
                                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                            "Error in sending export data to controller:" _
                                            & "Record" & m_Line & "not sent. ", _
                                            Logger.LogLevel.RELEASE)
                                'Update the file status
                                objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                                           "Failed to send record: " & _
                                                                           m_Line)
                                Return "-1"
                            End If
                        Loop Until m_Line.StartsWith("CLX")
                    ElseIf m_Line.StartsWith("SOR") Then
                        'Check for the IP address in the record.
                        If Mid(m_Line, 28, 15) = "127.000.000.001" Then
                            'Fixed in integration testing
                            m_TempLine1 = Mid(m_Line, 1, 27)
                            m_TempLine2 = Mid(m_Line, 44)
                            m_Ip = objAppContainer.objHelper.GetIPAddress()
                            m_Line = m_TempLine1 + m_Ip + m_TempLine2
                        End If
                        'Now send the record.
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "Record" & m_Line & "not sent. ", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            Return "-1"
                        End If
                    Else
                        'Send export data record for all pickign lists,
                        'Price Check, Count lists, SEL print request.
                        If Not SendRecord(m_Line) Then
                            'Add the exception to the device log.
                            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "Error in sending export data to controller:" _
                                        & "Message" & m_Line & "not sent. ", _
                                        Logger.LogLevel.RELEASE)
                            'Update the file status
                            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                                       "Failed to send record: " & _
                                                                       m_Line)
                            Return "-1"
                        End If
                    End If
                    'Increment the counter variable.
                    iCount = iCount + 1
                    m_Line = Nothing
                    m_Status = Nothing
                    m_TempLine1 = ""
                    m_TempLine2 = ""
                    m_TempLine3 = ""
                    m_TempLine4 = ""
                End While 'End of first while
            ElseIf aExDataRecords.Length = 3 Then
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "ExDataTransmitter:" _
                                        & "No data available in the export data file. ", _
                                        Logger.LogLevel.RELEASE)
                'delete export data file as no export data available
                'except SOR and OFF
                File.Delete(m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
                'Update the file status
                objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                           "No export data available")
                'return false to the calling function
                Return "0"
            Else
                'log the error
                AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                        "ExDataTransmitter:" _
                                        & "No data available in the export data file. ", _
                                        Logger.LogLevel.RELEASE)
                'Update the file status
                objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", _
                                                           "No export data available")
                'return false to the calling function
                Return "-1"
            End If
            'Before returning the status, delete the export data file in the 
            'local device.
            File.Delete(m_ExDatFilePath & "/" & Macros.EXPORT_FILE_NAME)
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "F", ex.Message)
            Return "-1"
        Finally
            aExDataRecords = Nothing
            'close the socket connection established with the controller.
            m_SockectConnMgr.TerminateConnection()
        End Try
        Try
            'Update the time in config file.
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.LAST_EXDATA_DOWNLOAD_TIME, _
                                               DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            'Update the file status
            objAppContainer.objHelper.UpdateFileStatus("McShMon_ExData", "P", "NA")

        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog("Error in sending export data:" _
                                                                & ex.Message.ToString(), Logger.LogLevel.RELEASE)
        End Try
        'return true if no error occured during the export data download.
        Return "1"
    End Function
#End If
#Region "RF"
#If RF Then
    Private Function ParseResponseRF(ByVal m_ResponseMessage As String, ByVal strRecordSent As String) As Boolean

        m_ResponseMessage = NTrim(m_ResponseMessage)
        'Response received from the controller.
        'new Message Format
        m_ResponseMessage = m_ResponseMessage.Substring(5, m_ResponseMessage.Length - 5)
        '''''
        'objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter::ParseResponseRF:: " + _
        '  m_ResponseMessage, Logger.LogLevel.RELEASE)
        'Based on the message type parse the response and return the value.
        If Not m_ResponseMessage.Trim = "" Then
            Select Case (m_ResponseMessage.Substring(0, 3))
                Case "ACK"
                    Dim strACK As String = m_ResponseMessage
                    m_TransactMessageParser.ParseACK(strACK)
                Case "NAK"
                    m_TransactMessageParser.ParseNAK(m_ResponseMessage)
                Case "SNR"
                    m_TransactMessageParser.ParseSNR(m_ResponseMessage)
                Case "GAR"
                    m_TransactMessageParser.ParseGAR(m_ResponseMessage)
                Case "PLI"
                    m_TransactMessageParser.ParsePLI(m_ResponseMessage)
                Case "PLL"
                    m_TransactMessageParser.ParsePLL(m_ResponseMessage)
                Case "PLE"
                    m_TransactMessageParser.ParsePLE(m_ResponseMessage)
                Case "EQR"
                    m_TransactMessageParser.ParseEQR(m_ResponseMessage)
                Case "PCR"
                    m_TransactMessageParser.ParsePCR(m_ResponseMessage)
                Case "LPR"
                    Dim strLPR As String = m_ResponseMessage
                    Dim objLPR As LPRRecord = New LPRRecord
                    m_TransactMessageParser.ParseLPR(strLPR, objLPR)
                Case "CLI"
                    m_TransactMessageParser.ParseCLI(m_ResponseMessage)
                Case "CLL"
                    m_TransactMessageParser.ParseCLL(m_ResponseMessage)
                    'Case "CLL"
                    '    m_TransactMessageParser.parseCLL(m_ResponseMessage)
                Case "CLE"
                    m_TransactMessageParser.ParseCLE(m_ResponseMessage)
                Case "RUP"
                    Dim strRUP As String = m_ResponseMessage
                    Dim objRUP As RUPRecord = New RUPRecord
                    m_TransactMessageParser.ParseRUP(strRUP, objRUP)
                Case "RLD"
                    Dim strRLD As String = m_ResponseMessage
                    Dim objRLD As RLDRecord = New RLDRecord
                    m_TransactMessageParser.ParseRLD(strRLD, objRLD)
                Case "SSR"
                    m_TransactMessageParser.ParseSSR(m_ResponseMessage)
                Case "PGG"
                    Return m_TransactMessageParser.ParsePGG(m_ResponseMessage)
                Case "PGR"
                    Return m_TransactMessageParser.ParsePGR(m_ResponseMessage)
                Case "PGN"
                    m_TransactMessageParser.ParsePGN(m_ResponseMessage)
                Case "PPR"
                    Dim strPPR As String = m_ResponseMessage
                    'Dim objPPR As PPRRecord = New PPRRecord
                    'm_TransactMessageParser.ParsePPR(strPPR, objPPR)
                Case "PSR"
                    m_TransactMessageParser.ParsePSR(m_ResponseMessage, strRecordSent)
                Case "PGI"
                    Return m_TransactMessageParser.ParsePGI(m_ResponseMessage)
                Case "PGB"
                    Dim strPGB As String = m_ResponseMessage
                    Dim objPGB As PGBRecord = New PGBRecord
                    m_TransactMessageParser.ParsePGB(strPGB, objPGB)
                Case "ISR"
                    m_TransactMessageParser.ParseISR(m_ResponseMessage)
                Case "DQR"
                    m_TransactMessageParser.ParseDQR(m_ResponseMessage)
                Case "RLR"
                    Dim strRLR As String = m_ResponseMessage
                    Dim objRLR As RLRRecord = New RLRRecord
                    m_TransactMessageParser.ParseRLR(strRLR, objRLR)
                Case "RLF"
                    Dim strRLF As String = m_ResponseMessage
                    m_TransactMessageParser.ParseRLF(strRLF)
                Case "RLD"
                    Dim strRLD As String = m_ResponseMessage
                    Dim objRLD As RLDRecord = New RLDRecord
                    m_TransactMessageParser.ParseRLD(strRLD, objRLD)
                Case "RUP"
                    Dim strRUP As String = m_ResponseMessage
                    Dim objRUP As RUPRecord = New RUPRecord
                    m_TransactMessageParser.ParseRUP(strRUP, objRUP)
                'Added as part of SFA - New transactions for Create own List
                Case "CLB"
                    'Dim strCLB As String = m_ResponseMessage
                    'Dim objCLB As CLBRecord = New CLBRecord
                    m_TransactMessageParser.ParseCLB(m_ResponseMessage)
            End Select
        Else
            DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.MessageRecieved)
        End If
    End Function

    Public Function SendRecordRF(ByVal strRecord As String) As Boolean
        'Dim m_SendBytes As [Byte]() = Nothing
        'Dim m_ReadBytes As [Byte]() = Nothing
        'Dim m_Status As Boolean = Nothing
        'Dim m_RetryWrite As Integer = 0
        Dim bTemp As Boolean = False
        Try
            'Read the rety attempt for writing data to the socket stream.
            'm_RetryWrite = Macros.WRITE_RETRY
            'Send the record to the controller.
            'Do
            'objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Sending Message " + strRecord, Logger.LogLevel.RELEASE)

            If Not m_rfSocketManager.SendText(strRecord) Then
                objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Cannot " _
                                                          & "send to socket", _
                                                          Logger.LogLevel.RELEASE)
                Return bTemp
            End If
            '    If m_SockectConnMgr.TransmitData(m_SendBytes) Then
            '        'Read the response stream from the client.
            '        If m_SockectConnMgr.ReadData(m_ReadBytes) And _
            '           m_ReadBytes.Length > 0 Then
            '            'Return the response after parsing it.
            '            Return ParseResponseRF( _
            '                        Encoding.ASCII.GetString(m_ReadBytes, _
            '                                                 0, _
            '                                                 m_ReadBytes.Length), strRecord)
            '        Else
            '            'Add the exception to the application log.
            '            objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Cannot " _
            '                                                  & "read from socket.", _
            '                                                  Logger.LogLevel.RELEASE)
            '            'If reading response from the controller is failed.
            '            Return False
            '        End If
            '    End If
            '    m_RetryWrite = m_RetryWrite - 1
            '    Loop Until m_RetryWrite = 0
            ''If all the write attempt failed.
            'If m_RetryWrite = 0 Then
            '    'write the error message to the app log
            '    AppMainModule.objAppContainer.objLogger.WriteAppLog( _
            '                "Unable to write record to the stream. Retry attempt" _
            '                & "failed for" & m_RetryWrite & "times", _
            '                Logger.LogLevel.RELEASE)
            '    Return False
            'End If

        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in sending export data to controller:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'incase of exception return false.
            Return False
        Finally
            bTemp = True
            'm_SendBytes = Nothing
            'm_ReadBytes = Nothing
        End Try
        Return bTemp
    End Function
    Private Sub ConnectToSocket()
        If m_rfSocketManager IsNot Nothing Then
            'm_rfSocketManager.ReconnectInterval = 5000
            AddHandler m_rfSocketManager.OnConnect, New RFSocketManager.ConnectionDelegate(AddressOf Me.HandleOnConnect)
            AddHandler m_rfSocketManager.OnDisconnect, New RFSocketManager.ConnectionDelegate(AddressOf Me.HandleOnDisconnect)
            AddHandler m_rfSocketManager.OnError, New RFSocketManager.ErrorDelegate(AddressOf HandleOnError)
            AddHandler m_rfSocketManager.OnRead, New RFSocketManager.ConnectionDelegate(AddressOf HandleOnRead)
            AddHandler m_rfSocketManager.OnWrite, New RFSocketManager.ConnectionDelegate(AddressOf HandleOnWrite)
            AddHandler m_rfSocketManager.OnReceiveTimeout, New RFSocketManager.ConnectionDelegate(AddressOf HandleOnReceiveTimeout)
            objAppContainer.objLogger.WriteAppLog("Trying to establish a new connection", _
                                                                           Logger.LogLevel.RELEASE)
            m_rfSocketManager.Connect()
        Else
            MessageBox.Show("Error Port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Cannot Connect to Socket ", _
                                                          Logger.LogLevel.RELEASE)
        End If
    End Sub
    Private Sub DisConnectSocket()
        m_rfSocketManager.Disconnect()
    End Sub

    Private Sub SentData(ByVal data As String)
        m_rfSocketManager.SendText(data)
    End Sub

    Private Sub HandleOnConnect(ByVal soc As Socket)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: EvntOnConnect : A successfull conccetion Event recived", _
                                                        Logger.LogLevel.RELEASE)
        DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Connected)
    End Sub
    Private Sub HandleOnDisconnect(ByVal soc As Socket)
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: EvntOnDisconnect ", _
                                                          Logger.LogLevel.RELEASE)
        DATAPOOL.getInstance.isConnected = False
        If Not m_rfSocketManager Is Nothing And DATAPOOL.getInstance.WaitingForConnection Then
            DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Disconnected)
        End If
    End Sub

    Private Sub HandleOnError(ByVal ErrorMessage As String, ByVal soc As Socket, ByVal ErrorCode As Integer)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        Dim m_attempts As Integer = 1
        objAppContainer.objLogger.WriteAppLog("HandleOnError:: In Error :: Message :" + _
                                              ErrorMessage + ",Error Code : " + _
                                              ErrorCode.ToString(), Logger.LogLevel.RELEASE)
        DATAPOOL.getInstance.isConnected = False
        Do While (m_attempts <= 2)
            If DATAPOOL.getInstance.WaitingForConnection Then
                DATAPOOL.getInstance.NotifyConnectionStatus(DATAPOOL.ConnectionStatus.Disconnected)
                Exit Do
            ElseIf DATAPOOL.getInstance.WaitingForNotification Then
                DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.Disconnected)
                Exit Do
            Else
                'Check whether a socket is present before sending an error
                objAppContainer.objLogger.WriteAppLog("Nothing is waiting:: sleeping for 100ms :: attempt - " + _
                                                                       m_attempts.ToString(), Logger.LogLevel.RELEASE)
                m_attempts = m_attempts + 1
                Threading.Thread.Sleep(100)
                'checking whether a control comes and waits after certain time 
            End If
        Loop


        'objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: EvntOnError - " _
        '                                                    & ErroMessage, _
        '                                                  Logger.LogLevel.RELEASE)
        'm_Reconnect = m_Reconnect + 1
        'Dim ReconnectMessage As String = Macros.CONNECTIVITY_MESSAGE
        'ReconnectMessage = String.Format(ReconnectMessage, m_Reconnect.ToString())
        'm_rfSocketManager = Nothing
        'If Not m_Reconnect > Macros.RECONNECT_ATTEMPTS Then
        '    '   objAppContainer.objConnector.SetCurrentStatus(ReconnectMessage)
        '    '   objAppContainer.objConnector.ShowConnector()
        '    '  objAppContainer.objConnector.Refresh()
        '    m_rfSocketManager = New RFSocketManager(ConfigDataMgr.GetInstance(). _
        '          GetParam(ConfigKey.SERVER_IPADDRESS).ToString(), ConfigDataMgr.GetInstance().GetParam(ConfigKey.IPPORT))
        '    'Initialise Socket connection for RF
        '    ConnectToSocket()

        '    'if not previously waiting 
        '    If Not DATAPOOL.getInstance.IsWaiting Then
        '        DATAPOOL.getInstance.WaitForConnection()
        '    End If
        'Else
        '    ''Put a Select case here and display a message Appropriately

        '    MessageBox.Show(ReconnectMessage)
        '    DATAPOOL.getInstance.NotifyConnectionStatus(False, True)
        'End If
    End Sub
    Private Sub HandleOnRead(ByVal soc As Socket)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Lowest
        m_socketReceivedText = m_rfSocketManager.ReceivedText
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Handle on Received :: - " & m_socketReceivedText, _
                                        Logger.LogLevel.RELEASE)
        ParseResponseRF(m_socketReceivedText, m_socketSentText)
        m_socketReceivedText = ""
    End Sub

    Private Sub HandleOnWrite(ByVal soc As Socket)
        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.Highest
        m_socketSentText = m_rfSocketManager.WriteText
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter::HandleOnWrite:: Sent - " _
                                              & m_socketSentText, _
                                              Logger.LogLevel.RELEASE)
    End Sub
    Private Sub HandleOnReceiveTimeout(ByVal soc As Socket)
        objAppContainer.objLogger.WriteAppLog("TransactDataTransmitter: Receive Timed Out ", _
                                                          Logger.LogLevel.RELEASE)
        'Recieve Time Out
        DATAPOOL.getInstance.NotifyDataEngine(DATAPOOL.ConnectionStatus.Timeout)
    End Sub
    'Public Function ReadDatafromSocket(ByRef strData As String) As Boolean
    '    'Have to implement Timer logic Becoz now connection problem can occur
    '    Try
    '        Dim m_temp As Boolean = False
    '        Dim m_RecievedBytes As [Byte]() = Nothing
    '        m_SockectConnMgr.ReadData(m_RecievedBytes)
    '        If m_RecievedBytes.Length > 0 Then
    '            strData = m_RecievedBytes.ToString()
    '            m_temp = True
    '        End If
    '    Catch ex As Exception
    '        ''have to implement logger here
    '    End Try
    'End Function

#End If
#End Region

End Class