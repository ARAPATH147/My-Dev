Imports System.Threading
'''****************************************************************************
''' <FileName>AppContainer.vb</FileName>
''' <summary>
''' The Main application container class which will 
''' intialise all the applciation parameters.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''****************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Check added to verify whether MCF is enabled or not
''' </Summary>
'''****************************************************************************
Public Class AppContainer

    'Declaring the Business Class objects
    Public objScreen As ScreenMgr = Nothing
    Public objHelper As Helper = Nothing
    Public objDataEngine As DataEngine = Nothing
    Public bIsAutoLogOffatSummary As Boolean = False
#If NRF Then
    Public objDBConnection As DBConnections = Nothing
    Public objActiveFileParser As ActiveFileParser = Nothing
    Public objUserSessionMgr As UserSessionManager = Nothing
    Public iStockMovementValidityDays As Integer = 0
#ElseIf RF Then
    Public objUserSessionMgr As RFUserSessionManager = Nothing
    Public strPassword As String = Nothing
    Public isConnected As Boolean = True
    Public bExcessSalesRecall As Boolean = False
    Public eLastUsedModule As String = "None"
    Public stSession As SessionVariables = New SessionVariables()
    Public objGlobalItemList As ArrayList = New ArrayList()
    Public bRecallConnection As Boolean = False
    Public bIsActiveRecallListSCreen As Boolean = False
#End If
    Public strMacAddress As String = Nothing
    'Recalls Cr
    Public bRecallStarted As Boolean = False
    'Create Recalls
    Public objRecallCount As RecallCount = Nothing
    Public bIsCreateRecalls As Boolean = False
    Public objExportDataManager As GOExportDataManager = Nothing
    'Declaring the Form objects
    Public objSplashScreen As frmSplashScreen = Nothing
    'For auto logoff
    Public objAutologOff As AutoLogOff = Nothing

    'Declaring the Utils Class objects
    Public objMessageMgr As MessageManager = Nothing
    Public objActiveModule As ACTIVEMODULE
    Public objCurrentModule As ACTIVEMODULE
    Public objRecallTable As Hashtable = Nothing
    Public objConfigDataMgr As ConfigDataMgr = Nothing
    'Declaring for User Auth
    'Decalre variables for storing the user information
    Public strCurrentUserID As String = Nothing
    Public strSupervisorFlag As String = Nothing
    Public strPreviousUserID As String = Nothing
    Public strCurrentUserName As String = Nothing
    Public objLogger As Logger = Nothing
    Public bActiveform As Boolean = True
    Public objUODCollection As ArrayList = Nothing
    Public bCreateRecall As Boolean = Nothing
    Public bIsAppRegisterRequired As Boolean = True
    Public bConfigFile As Boolean = True
    Public Delegate Sub StartTimerCallback()
    Public Delegate Sub StopTimerCallback()
    Public iOffSet As Integer = 1
    Private Shared m_tmrTimer As System.Windows.Forms.Timer = Nothing
    Private m_Control As Control = Nothing
    Private m_TimerInterval As Integer = 0
    'Private m_bIsAutoLogoff As Boolean = False

    'v1.1 MCF Change
    'Declare variables for storing the active IP address
    Public strActiveIP As String = Nothing
    'v1.1 Declare variable for whether mcf enabled.
    Public bMCFEnabled As Boolean = False
    'v1.1 Declare variable whether connected to alternate IP
    Public iConnectedToAlternate As Integer = 0
    ''' <summary>
    ''' Initialize the AppContainer Class and loads config data
    ''' Most of this will be performed when the Splash screen is displayed at application startup.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Initialise()
        Try
            If (System.IO.File.Exists(Macros.CONFIG_FILE_PATH)) Then
                objConfigDataMgr = ConfigDataMgr.GetInstance()
                If Not bConfigFile Then
                    Exit Sub
                End If
            Else
                System.Windows.Forms.MessageBox.Show("Config file does not exist.", "Error")
                Exit Sub
            End If
            'Get the timer interval and assign the interval to the timer.
            m_TimerInterval = CInt(ConfigDataMgr.GetInstance.GetParam(ConfigKey.AUTO_LOGOFF_TIMEOUT))
            'If m_TimerInterval doesnot return anything it means Config file 
            'is not present so terminate
            If m_TimerInterval = 0 Then
                Exit Sub
            End If
            'v1.1 MCF Change
            strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.ACTIVE_IPADDRESS).ToString()

            If Not ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() = _
                   ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString() Then
                bMCFEnabled = True
            End If
#If RF Then
            ''Assigning the Instance to the Reference
            objUserSessionMgr = RFUserSessionManager.GetInstance()
#ElseIf NRF Then
            objUserSessionMgr = UserSessionManager.GetInstance()
            iStockMovementValidityDays = CInt(ConfigDataMgr.GetInstance.GetParam(ConfigKey.VALID_STOCK_MOVEMENT_DAYS))
#End If
            objAutologOff = New AutoLogOff()
            m_Control = New Control()
            'Initialise timer and handlers for the timer
            m_tmrTimer = New System.Windows.Forms.Timer()
            AddHandler m_tmrTimer.Tick, AddressOf AutoLogOffSession
            'Add event handler for getting the auto logoff timer tick.
            AddHandler objAppContainer.objAutologOff.evtPowerOn, AddressOf StopTimer
            AddHandler objAppContainer.objAutologOff.evtUserIdle, AddressOf StartTimer

            'm_tmrTimer.Interval = m_TimerInterval * 60 * 1000
            'm_tmrTimer.Enabled = False

            'Initialize all the classes before closing the splash screen
            'Initialize Barcode Reader
            AddHandler BCReader.GetInstance().evtBCScanned, AddressOf BCReader.GetInstance().EventBCScannedHandler
            'Initialize Helper Class
            objHelper = New Helper()
            'Instantiate the logger class
            objLogger = New Logger()
            'Creating required Directories
            CreateAppDirectories()
            'Fix for log file deletion.
#If RF Then
            DeleteLogFiles()
#End If
            objLogger.WriteAppLog("AppContainer::Initialize: Created Directories", Logger.LogLevel.RELEASE)
            'Create Splash screen and start the splash screen form
            Dim m_splashThread As New Thread(AddressOf StartSplash)
            m_splashThread.Start()
            'Loading message manager XML
            objMessageMgr = MessageManager.GetInstance()
            objLogger.WriteAppLog("AppContainer::Initialize: loaded Message Manager", Logger.LogLevel.RELEASE)
            'Initialize Screen Manager
            objScreen = ScreenMgr.GetInstance()

            'Instantiate the UOD Array List to hold all the UODs in a session
            objUODCollection = New ArrayList
            'RECALLS CR
            objRecallCount = New RecallCount()
            objLogger.WriteAppLog("AppContainer::Initialize:loaded Helper Module", Logger.LogLevel.RELEASE)
            objDataEngine = New DataEngine()
#If NRF Then
   'Database connection initialize

            objDBConnection = New DBConnections()
#End If

            objLogger.WriteAppLog("AppContainer::Initialize:Database connection established", Logger.LogLevel.INFO)

#If NRF Then
 'For User Auth
            objActiveFileParser = New ActiveFileParser()
#End If

            objExportDataManager = New GOExportDataManager()

            'Instantiating a hashtable to keep a track of recall list
            objRecallTable = New Hashtable()

            'Populate Workflow Dataset
            WorkflowMgr.GetInstance().ReadXML()
            objLogger.WriteAppLog("AppContainer::Initialize:Workflow XML read successfully", Logger.LogLevel.RELEASE)
            WorkflowMgr.GetInstance().WFIndex = "-1"
            'A flag to identify a create recall transaction
            bCreateRecall = False
            'Check if the OS version in WM6.5 for MC55 device.
            If Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
                iOffSet = 2
                'Set barcode reader to user the laser bar beam rather than using image.
            End If
            If CBool(ConfigDataMgr.GetInstance.GetParam(ConfigKey.AUTHREQUIRED)) Then
                'Intialised ConfigDataManager for User Auth
                strPreviousUserID = ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER)

                'Display the User Authenticatioin form for User Auth
                objUserSessionMgr.LaunchUser()

                'For User auth 
                If strCurrentUserID <> Nothing Then
                    'Create a thread for auto logoff.
                    objAutologOff.Start()
                    WorkflowMgr.GetInstance().NextScreen(1)
                End If
            Else
#If NRF Then
                'Entering dummy data to logon without User Authentication
                strCurrentUserName = "DummyUser"
                strCurrentUserID = "111"
                strPreviousUserID = "123"
                'write SOR record to the export data file.
                objExportDataManager.CreateSOR("111")
#ElseIf RF Then
                Dim objResponse As Object = Nothing
                Dim objSNR As SNRRecord
                'Pass the complete string
                strCurrentUserName = "DummyUser"
                strCurrentUserID = "111"
                strPassword = "111"
                If objExportDataManager.CreateSOR("111") Then
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is SNRRecord Then
                            objSNR = CType(objResponse, SNRRecord)
                            With objAppContainer
                                objAppContainer.strCurrentUserID = objSNR.strOperatorID
                                If objSNR.cAuthorityFlag = "S" Then
                                    .strSupervisorFlag = "Y"
                                Else
                                    .strSupervisorFlag = "N"
                                End If
                                .strCurrentUserName = objSNR.strUserName
                            End With
                        Else

                        End If
                        objResponse = Nothing
                        objSNR = Nothing
                    Else

                    End If
                End If
#End If
                objAutologOff.Start()
                'MCF change : to avoid error after calling WorkflowMgr.GetInstance().EndSession
                If Not objAppContainer.iConnectedToAlternate = -1 Then
                    WorkflowMgr.GetInstance().NextScreen(1)
                End If
            End If

            While bActiveform

                Application.DoEvents()

            End While
            'Close Splash Screen
            CloseSplash()
            'Terminate the Splash Screen thread after all intialisation are done
            m_splashThread = Nothing

            'To make the registry setting for the downloader
            'Register MCDownloader only if the logoff is manual process.
            'If from auto logoff then donot reqister as threads will cause issue.
#If NRF Then
            If bIsAppRegisterRequired Then
                objAppContainer.objHelper.RegisterDownloader()
            End If
#End If
            'Call Terminate function 
            AppTerminate()
        Catch ex As Exception
            If ex.Message = Macros.CONNECTION_LOSS_EXCEPTION Then
                objLogger.WriteAppLog("Connection to controller Failed @ " + ex.StackTrace, Logger.LogLevel.RELEASE)
            ElseIf ex.Message = "ThreadAbortException" Then
                If Not objLogger Is Nothing Then
                    objLogger.WriteAppLog("Connection to controller Failed", Logger.LogLevel.RELEASE)
                End If
            Else
                MessageBox.Show(" Initialization failed due to : " & ex.Message, "Exception", _
           MessageBoxButtons.OK, _
           MessageBoxIcon.Hand, _
           MessageBoxDefaultButton.Button1)
            End If

            'Call Terminate function 
            AppTerminate()
        End Try
    End Sub
    ''' <summary>
    ''' Check whether other applications are running
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsOtherProgramRunning() As Boolean
        Try
            Dim currFile As String
            Dim arrProcess() As String = ConfigDataMgr.GetInstance.GetParam("Applications").Split(",")
            Dim objProcess As New PROCESS

            For Each eleProcess As PROCESS In objProcess.GetProcess()
                currFile = eleProcess.GetProcessName().Split(".")(0)
                For Each file As String In arrProcess
                    If (currFile = file) Then
                        Dim Message As String = "The Application {0} is already running." + vbCr
                        Message = Message + "Unable to start new instance of MCGdOut." + vbCr
                        Message = Message + "Try closing {0} and start MCGdOut again"
                        MessageBox.Show(String.Format(Message, file.ToString), "Warning :", _
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                        MessageBoxDefaultButton.Button1)
                        Return True
                    End If
                Next
            Next
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Start the splash screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartSplash()
        objSplashScreen = New frmSplashScreen
        Application.Run(objSplashScreen)
    End Sub
    ''' <summary>
    ''' Closing the splash screen
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CloseSplash()
        ' Close the Splash screen
        If Not (objSplashScreen Is Nothing) Then
            objSplashScreen.Invoke(New EventHandler(AddressOf objSplashScreen.KillSplash))
            objSplashScreen.Dispose()
            objSplashScreen = Nothing
        End If
    End Sub
    ''' <summary>
    ''' Perform all actions before we Terminate the application
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppTerminate()
        Try
            'objAutologOff.Stop()
#If RF Then
            If Not objAutologOff Is Nothing Then
                Try
                    objAutologOff.Stop()
                Catch ex As Exception
                    objAppContainer.objLogger.WriteAppLog("Exception when auto log off was stopped ::" + _
                                                          ex.Message, Logger.LogLevel.RELEASE)
                End Try
            End If
#End If
            objAutologOff = Nothing

            objLogger.WriteAppLog("AppContainer::AppTerminate:AutoLogoff successful, object terminated", Logger.LogLevel.INFO)
            BCReader.GetInstance().TerminateBCReader()
            'Terminating items held in the object instance
            objMessageMgr.TerminateMsgMgr()
            objLogger.WriteAppLog("AppContainer::AppTerminate:Message Manager terminated", Logger.LogLevel.RELEASE)
            objDataEngine = Nothing
#If NRF Then
            objDBConnection = Nothing
            objActiveFileParser = Nothing
#End If
            If Not objExportDataManager Is Nothing Then
                objExportDataManager.EndSession()
            End If
            objRecallCount = Nothing
            objExportDataManager = Nothing
            strCurrentUserID = Nothing
            strSupervisorFlag = Nothing
            strPreviousUserID = Nothing
            strCurrentUserName = Nothing
            objRecallTable = Nothing
            objMessageMgr = Nothing
            objHelper = Nothing
            objUODCollection = Nothing
            objScreen.DisposeScreenMgr()
            objScreen = Nothing
            objLogger.WriteAppLog("AppContainer::AppTerminate:Terminated all objects initialized", Logger.LogLevel.RELEASE)
            objLogger = Nothing
            bCreateRecall = Nothing
            CloseSplash()
        Catch ex As Exception
            'Handle Application Terminate Exception here
            'MessageBox.Show(" Error in Terminating the Applicaton : " _
            '                & ex.Message, "Exception", _
            '                MessageBoxButtons.OK, _
            '                MessageBoxIcon.Hand, _
            '                MessageBoxDefaultButton.Button1)
            Return
        End Try
    End Sub
    ''' <summary>
    ''' Screen Display method for Goods out Main Menu. 
    ''' All Goods out Menu driven screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName">Name of the screen to be displayed as per the Workflow xml</param>
    ''' <returns>True if display is sucess else False</returns>
    ''' <remarks></remarks>
    Public Function DisplayWorkflowScreen(ByVal ScreenName As String)
        ' Write to Log File
        objAppContainer.objLogger.WriteAppLog("Entered DisplayWorkflowScreen of" _
                                              & "AppContainer", _
                                              Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case "SignOn"
                    objScreen.DisplaySignon()
                Case "MainMenu"
                    objScreen.DisplayMainMenu()
                Case "GOMenu"
                    objScreen.DisplayGOMenu()
                Case "SemiCentReturn"
                    objScreen.DisplayGOMenu()
                Case "RetDestMenu"
                    objScreen.DisplayGOMenu()
                Case "ReasonRetDestMenu"
                    objScreen.DisplayGOMenu()
                Case "PreScan"
                    objScreen.DisplayProductScan()
                Case "Recall"
                    objScreen.DisplayRecallid()
                    bCreateRecall = True
                Case "Authorisation"
                    objScreen.DisplayAuthorizationid()
                Case "PharmacySpecialWaste"
                    objScreen.DisplayUOD()
                Case "StoreNumber"
                    objScreen.DisplayDesinationStoreid()
                Case "STQPreScan"
                    objScreen.DisplayProductScan()
                Case "Recall_Start"
                    objScreen.DisplayRecallList()
            End Select
        Catch ex As Exception
            Return False
        End Try
        'Writing to Log INFO File while exit
        objAppContainer.objLogger.WriteAppLog("Exit DisplayWorkflowScreen of " _
                                              & "AppContainer", _
                                              Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Enable the timer
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartTimer()
        ' Runs the timer, and raises the event.
        If Not (m_Control.InvokeRequired) Then
            'm_tmrTimer.Enabled = True
            AutoLogOffSession()
        Else
            m_Control.Invoke(New StartTimerCallback(AddressOf StartTimer))
        End If
    End Sub
    ''' <summary>
    ''' Disable the timer.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StopTimer()
        ' Runs the timer, and raises the event.
        If Not (m_Control.InvokeRequired) Then
            m_tmrTimer.Enabled = False
        Else
            m_Control.Invoke(New StopTimerCallback(AddressOf StopTimer))
        End If
    End Sub
    Private Sub AutoLogOffSession()
        'Stop the timer
        StopTimer()
        'Start splash screen for performing auto logoff
        'objSplashScreen.Label1.Text = "Auto Logoff in progress. Please wait..."
        Dim thrSplashScreen As New Thread(AddressOf StartSplash)
        Try
            thrSplashScreen.Start()
            'Kill the auto logoff thread.
            objAppContainer.objAutologOff.SetSleepTimeOut()
            'Set the boolean variable to prevent from registering mcdownloader
            bIsAppRegisterRequired = False

            'According to the Active module write the export to the export data
            'file. For picking list and count list the export data writing is 
            'taken care by the logg of function.
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.CRDCLM
                    'Write export data for credit claim
                    CCSessionMgr.GetInstance().GenerateExportData()
                    CCSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.CRTRCL
                    'Write export data for create recall
                    GOSessionMgr.GetInstance().GenerateExportData()
                    GOSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.GDSOUT
                    'Write export data for Goods Out
                    GOSessionMgr.GetInstance().GenerateExportData()
                    GOSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.GDSTFR
                    'Write export data record for goods out transfer
                    GOTransferMgr.GetInstance().GenerateExportData()
                    GOTransferMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.PHSLWT
                    'Write export data record for pharmacy special waste.
                    PSWSessionMgr.GetInstance().GenerateExportData()
                    PSWSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.RECALL
                    'write the export data for create recall from all screens 
                    'other than RECALL Summary Screen
                    If Not bIsAutoLogOffatSummary Then
                        RLSessionMgr.GetInstance().GenerateExportData()
                    End If

                    RLSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case Else
                    Exit Select
            End Select
            'Call Log off function to add OFF record to the end of export data file.
#If RF Then
            RFUserSessionManager.GetInstance().LogOutSession(False)
#ElseIf NRF Then
            UserSessionManager.GetInstance().LogOutSession(False)
#End If
            Threading.Thread.Sleep(3000)
            'Display a message so that user is aware of the auto logoff.
            MessageBox.Show("Auto log off is in progress. Any saved data can be downloaded during next log in.", _
                            "Auto Log off", MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            'objUserSessionMgr.LogOutSession(False)
            objAppContainer.objLogger.WriteAppLog("Auto Logg off success", _
                                                  Logger.LogLevel.RELEASE)
            'terminate the splash screen.
            CloseSplash()

        Catch ex As Exception

        Finally
            '    m_bIsAutoLogoff = True
            '    AppTerminate()
            '    'Set the active form to false.
            bActiveform = False
        End Try

    End Sub
    Private Function CreateAppDirectories() As Boolean
        If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH))) Then
            MessageBox.Show("Cannot Create Log Directory", "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If

#If NRF Then
 If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                ConfigKey.EXPORT_FILE_PATH))) Then
            MessageBox.Show("Cannot Create Export Data Directory", "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If

        If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                ConfigKey.DATABASE_PATH))) Then
            MessageBox.Show("Cannot Create Export Data Directory", "Info", _
                            MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, _
                            MessageBoxDefaultButton.Button1)
        End If
#End If

    End Function
#If RF Then
    Private Function DeleteLogFiles() As Boolean
        If Not (FileIO.LogFileDelete()) Then
            objLogger.WriteAppLog("Cannot Delete old log files", _
                                              Logger.LogLevel.RELEASE)
        End If

    End Function
#End If
    Public Enum ACTIVEMODULE
        NONE
        GDSOUT
        CRDCLM
        RECALL
        CRTRCL
        GDSTFR
        PHSLWT
        USERAUTH
#If RF Then
        LOGOFF
#End If
    End Enum
    'Structure to hold variables for session disconnect.
#If RF Then
    Public Structure SessionVariables
        Dim m_StoreID As String
        Dim m_Authorizationid As String
        Dim m_Supplier As GOSessionMgr.Supplier
        Dim m_BCType As String
        Dim m_SupplierType As String
    End Structure
#End If
End Class

