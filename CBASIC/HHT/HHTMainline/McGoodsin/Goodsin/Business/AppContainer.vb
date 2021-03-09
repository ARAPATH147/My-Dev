Imports System.Threading
Imports System.Windows.Forms
Imports System.IO

''' <summary>
''' This is the Applicaiton Container Class.
''' This class initialises the generic application paramaeters and brings up the business modules.
''' </summary>
''' <remarks></remarks>
''' 
''' * Modification Log
''' 
'''**********************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Check added to verify whether MCF is enabled or not
''' </Summary>
'''**********************************************************************************
''' No:      Author            Date            Description 
''' 1.2     Kiran Krishnan  28/04/2015     Modified as part of DALLAS Positive
'''            (KK)                        receiving project.Added logic to check
'''                                        if Dallas Positive receiving store or not.   
'''                                                                            
'''********************************************************************************** 

Public Class AppContainer
    'Create all genereic objects here. 
    'EG: Splash Screen, AppConfig, Transaction Manager, Status Bar, Barcode Scanner etc...
    'AppContainer.AppInitialise () should be called from main ()

    Public objMsgManager As MessageManager = Nothing
    Public objHelper As Helper = Nothing
    Public objLogger As Logger = Nothing
    Public objDataEngine As DataEngine = Nothing

    Public objConfigDataMgr As ConfigDataMgr = Nothing
    Public objUserSessionMgr As UserSessionManager = Nothing
    Public objValueHolder As GIValueHolder = Nothing
    Public bAutoLogOffProcess As Boolean = False
#If NRF Then

    Public objDBConnection As DBConnections = Nothing
    'Public objExDataConnectionMgr As ExDataConnectionMgr = Nothing
    'CR Forced Log off
    Public bForceLogOff As Boolean = False

    'testing
    Public objUserAuth As frmUserAuthentication
    Public objActiveFileParser As ActiveFileParser = Nothing
#End If
#If RF Then
    Public Enum ModScreen
        POSTFINISH
        BCITEMFINISH
        PREFINISH
        BCITEMPREFINISH
        FIRSTBATCH
        FIRSTITEM
        NEXTITEM
        DRVRBDGESCAN
        ITEMSCAN
        CARTONSCAN
        UODSCAN
        ITEMSELECT
        SELCT
        QUIT
        NONE
    End Enum
    Public m_ModScreen As ModScreen = ModScreen.NONE
    Public m_PreviousScreen As ModScreen = ModScreen.NONE
    Public bReconnectSuccess As Boolean = False
    'CHANGE
    Public bSaveDetails As Boolean = False
    Public objSaveGIXMessage As RFDataStructure.GIXMessage
    Public objSavedGIXMessage As RFDataStructure.GIXMessage
    Public objSavedGIFFinish As RFDataStructure.GIFMessage
    Public m_FinishedDetails As ArrayList
    Public eDeliveryType As DeliveryType = DeliveryType.None
    Public eFunctionType As FunctionType = FunctionType.None
    Public m_SavedDetails As ArrayList
    Public eCurrLocation As CurrentLocation = CurrentLocation.None
#End If
    Public objAutologOff As AutoLogOff = Nothing
    Public objActiveModule As ACTIVEMODULE
    Public objActiveScreen As ACTIVESCREEN
    Public objPrevMod As ACTIVEMODULE
    Public strShowMsg As String = Nothing
    Public objCalcpadSessionMgr As CalcPadSessionMgr = Nothing
    'Public objAppCpnfig As Appconfig = Nothing
    'Declare variables for Price Check
    Public iCompletedCount As Integer = 0
    Public bCalcpad As Boolean = False
    Public bCommFailure As Boolean = False
    Public bTimeOut As Boolean = False
    Public bRetryAtTimeout As Boolean = False
    'Declare variables for storing the user information
    Public strCurrentUserID As String = Nothing
    Public strUser As String = Nothing
    Public strSupervisorFlag As String = Nothing
    Public strPreviousUserID As String = Nothing
    Public strCurrentUserName As String = Nothing
    Public strAuditFinishCheck As String = Nothing
    Public strDeviceType As String = Nothing
    Public bIsAppRegisterRequired As Boolean = True
    Public strAuditCartonNotinFile As String = Nothing
    Public strNIFCartonCode As String = Nothing
    Public bConnect As Boolean = False
    Public objSplashScreen As frmSplashScreen
    Public objStatusBar As StatusBar
    Public objGoodsInMenu As frmGoodsInMenu
    Public objSSCReceivingMainMenu As frmSSCReceivingMainMenu
    Public objDirReceive As frmDirReceive
    Public objCalcpad As frmCalcPad
    Public objMsgBox As MsgBx
    Public bConfigFile As Boolean = True
    Public objConfigValues As ConfigValues = Nothing
    Public iOffSet As Integer = 1
    'Variable to store current active session
    Private objActiveSession As GoodsInSession
    Public bUserSession As Boolean = False
    Public Structure ConfigValues
        Public ASNActive As String
        Public UODActive As String
        Public DirectsActive As String
        Public ONightDelivery As String
        Public ONightScan As String
        Public BatchSize As String
    End Structure

    'v1.1 MCF Change
    'Declare variables for storing the active IP address
    Public strActiveIP As String = Nothing
    'v1.1 Declare variable for whether mcf enabled.
    Public bMCFEnabled As Boolean = False
    'v1.1 Declare variable whether connected to alternate IP
    Public iConnectedToAlternate As Integer = 0
    'V1.2 - KK
    'Declared variable to check if DPR store or not
    Public bDallasPosReceiptEnabled As Boolean = False
    ''' <summary>
    '''  ''' Application initialisation.
    ''' Most of this will be performed when the Splash screen is displayed at application startup.
    ''' </summary>
    ''' <remarks></remarks>
#If RF Then
     Public Sub AppInitialise()
        Try
            If Not (System.IO.File.Exists(Macros.CONFIG_FILE_PATH)) _
               Or Not (System.IO.File.Exists(Macros.IPCONFIG_FILE_PATH)) Then 'v1.1 MCF Change
                System.Windows.Forms.MessageBox.Show("Config File does not exist", "Error")
                Exit Sub
            End If
            objConfigDataMgr = ConfigDataMgr.GetInstance()
            If Not bConfigFile Then
                Exit Sub
            End If
            'v1.1 MCF Change
            strActiveIP = objConfigDataMgr.GetIPParam(ConfigKey.ACTIVE_IPADDRESS).ToString()
            '.GetInstance.GetParam(ConfigKey.ACTIVE_IPADDRESS).ToString()

            If Not objConfigDataMgr.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() = _
                                                            objConfigDataMgr.GetIPParam _
                                                            (ConfigKey.SECONDARY_IPADDRESS).ToString() Then
                bMCFEnabled = True
            End If

            'Creating Diectories if not exists
            CreateAppDirectories()

            'delete old log files
            DeleteLogFiles()
           
           

            ''Do all Initilisations here
            objLogger = New Logger
            objMsgManager = MessageManager.GetInstance()
           
            objHelper = New Helper
            'Start a Thread to display Splash screen and let the main thread do all the initialisations.
            Dim thrSplashScreen As New Thread(AddressOf DisplaySplash)
            thrSplashScreen.Start()
            'if application RF
            objDataEngine = New RFDataSource
            objStatusBar = New StatusBar
            objConfigValues = New ConfigValues
            objActiveModule = New ACTIVEMODULE
            objCalcpad = New frmCalcPad
            objMsgBox = New MsgBx
            strDeviceType = ConfigDataMgr.GetInstance().GetParam(ConfigKey.DEVICE_TYPE).ToString()

            AddHandler BCReader.GetInstance().evtBCScanned, AddressOf BCReader.GetInstance().EventBCScannedHandler
            'CHANGE - for saving the details when connection is lost
            m_SavedDetails = New ArrayList
            m_FinishedDetails = New ArrayList()
            objSaveGIXMessage = New RFDataStructure.GIXMessage
            objSavedGIXMessage = New RFDataStructure.GIXMessage
            objSavedGIFFinish = New RFDataStructure.GIFMessage
            'Only start Application if connection to server is true

            If bConnect Then
                'Check the OS version and set the offset.
                'Check if the OS version in WM6.5 for MC55 device.
                If Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
                    iOffSet = 2
                    'Set barcode reader to user the laser bar beam rather than using image.
                End If
                'Bypassing User Authentication for testing Purposes.

                If (ConfigDataMgr.GetInstance.GetParam(ConfigKey.IS_TESTING) = "False") Then
                    'Intialised ConfigDataManager for User Auth
                    strPreviousUserID = ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER)
                    Dim bLogIn As Boolean = False
                    While Not bLogIn

                        bLogIn = True
                        'Display the User Authenticatioin form for User Auth
                        UserSessionManager.GetInstance.LaunchUser()
                        UserSessionManager.GetInstance.EndSession()
                        'strCurrentUserID = "111"
                        'check for old log file and delete.
                        objHelper.ValidateLogFile(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOGFILE_PATH))
                        'Display the Goods In main menu
                        If strCurrentUserID <> Nothing Then
                            'Create a thread for auto logoff.
                            'objAutologOff = New AutoLogOff()
                            If objDataEngine.GetConfigValues(objConfigValues) Then
                                If Not (objConfigValues.DirectsActive = "N" AndAlso objConfigValues.UODActive = "N") Then
                                    'AndAlso objConfigValues.ASNActive = "N") Then
                                    'V1.2 - KK
                                    'Function to check if Dallas Positive receiving store or not
                                    If Not objDataEngine.CheckDallasStore() Then
                                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M133"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                                    End If

                                    objSSCReceivingMainMenu = New frmSSCReceivingMainMenu
                                    objDirReceive = New frmDirReceive
                                    'Create a thread for auto logoff.
                                    objAutologOff = New AutoLogOff()
                                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                                    objGoodsInMenu = New frmGoodsInMenu
                                    objStatusBar.ShowDialog()
                                    objStatusBar.Dispose()
                                Else
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M112"), "Alert", _
                                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                End If

                            Else
                                If Not bCommFailure And Not bReconnectSuccess Then
                                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M112"), "Alert", _
                                          MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                                Else
                                    bLogIn = False
                                End If

                            End If

                            End If
                    End While

                Else
                    strCurrentUserID = "111"
                    strSupervisorFlag = "Y"
                    strPreviousUserID = "999"
                    strCurrentUserName = "Boots"
                    objDataEngine.SignOn("111")
                    'Create a thread for auto logoff.
                    ' objAutologOff = New AutoLogOff()
                    If objDataEngine.GetConfigValues(objConfigValues) Then
                        If Not (objConfigValues.DirectsActive = "N" AndAlso objConfigValues.UODActive = "N") Then
                            '  AndAlso objConfigValues.ASNActive = "N") 
                            'V1.2 - KK
                            'Function to check if Dallas Positive receiving store or not
                            objDataEngine.CheckDallasStore()
                            objSSCReceivingMainMenu = New frmSSCReceivingMainMenu
                            objDirReceive = New frmDirReceive
                            'Create a thread for auto logoff.
                            objAutologOff = New AutoLogOff()
                            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                            objGoodsInMenu = New frmGoodsInMenu
                            objStatusBar.ShowDialog()
                            objStatusBar.Dispose()
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M112"), "Alert", _
                                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        End If

                    Else
                        If Not bCommFailure And Not bReconnectSuccess Then
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M112"), "Alert", _
                                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        End If
                    End If

                End If
                bConnect = False
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M125"), "Error", _
                                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If

            'Call the terminate function 
            AppTerminate()

        Catch ex As Exception
            'SFA SIT - DEF 401
            'MessageBox.Show(" Cannot Initialise Application | " & Err.Description & " : " & Err.Number, "Exception", _
            '                           MessageBoxButtons.OK, _
            '                           MessageBoxIcon.Exclamation, _
            '                           MessageBoxDefaultButton.Button1)
            'Exit Applicaiton if Initialisation fails.
            AppTerminate()
        End Try
    End Sub
#ElseIf NRF Then
    Public Sub AppInitialise()
        Try
            If Not (System.IO.File.Exists(Macros.CONFIG_FILE_PATH)) _
               Or Not (System.IO.File.Exists(Macros.IPCONFIG_FILE_PATH)) Then 'v1.1 MCF Change
                System.Windows.Forms.MessageBox.Show("Config File does not exist", "Error")
                Exit Sub
            End If

            objConfigDataMgr = ConfigDataMgr.GetInstance()
            If Not bConfigFile Then
                Exit Sub
            End If
            objMsgManager = MessageManager.GetInstance()
            If (File.Exists(Macros.CONFIG_FILE_PATH)) Then
                objConfigDataMgr = ConfigDataMgr.GetInstance()
            Else
                System.Windows.Forms.MessageBox.Show("Config File does not exist", "Error")
                Exit Sub
            End If
            'v1.1 MCF Change
            strActiveIP = objConfigDataMgr.GetIPParam(ConfigKey.ACTIVE_IPADDRESS).ToString()
            '.GetInstance.GetParam(ConfigKey.ACTIVE_IPADDRESS).ToString()

            If Not objConfigDataMgr.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() = _
                                                            objConfigDataMgr.GetIPParam _
                                                            (ConfigKey.SECONDARY_IPADDRESS).ToString() Then
                bMCFEnabled = True
            End If

            'Creating required Directories
            CreateAppDirectories()
            Thread.Sleep(1000)
            ''Do all Initilisations here

            objHelper = New Helper()
            objLogger = New Logger()

            'if application for Non-RF
            objDataEngine = New NRFDataSource()
            objDBConnection = New DBConnections()
            'objExportDataManager = New SMExportDataManager()
            objActiveModule = New ACTIVEMODULE()
            objConfigValues = New ConfigValues()

            'Start a Thread to display Splash screen and let the main thread do all the initialisations.
            Dim thrSplashScreen As New Thread(AddressOf DisplaySplash)
            thrSplashScreen.Start()
            '----------------------------------
            'testing
            objUserAuth = New frmUserAuthentication
            '---------------------------------
            objStatusBar = New StatusBar()

            strDeviceType = ConfigDataMgr.GetInstance().GetParam(ConfigKey.DEVICE_TYPE).ToString()
            'objActiveFileParser = New ActiveFileParser()

            AddHandler BCReader.GetInstance().evtBCScanned, AddressOf BCReader.GetInstance().EventBCScannedHandler

            'Check the OS version and set the offset.
            'Check if the OS version in WM6.5 for MC55 device.
            If Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
                iOffSet = 2
                'Set barcode reader to user the laser bar beam rather than using image.
            End If

            'Intialised ConfigDataManager for User Auth
            'strPreviousUserID = ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER)
            'Bypassing User Authentication for testing Purposes.
            If (ConfigDataMgr.GetInstance.GetParam(ConfigKey.IS_TESTING) = "False") Then
                'Intialised ConfigDataManager for User Auth
                strPreviousUserID = ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER)

                'Display the User Authenticatioin form for User Auth
                UserSessionManager.GetInstance.LaunchUser()
                'Display the Goods In main menu
                If strCurrentUserID <> Nothing Then
                    If objDataEngine.GetConfigValues(objConfigValues) Then
                        If Not (objConfigValues.DirectsActive = "N" AndAlso objConfigValues.UODActive = "N") Then
                            objSSCReceivingMainMenu = New frmSSCReceivingMainMenu
                            objDirReceive = New frmDirReceive
                            'Create a thread for auto logoff.
                            objAutologOff = New AutoLogOff()
                            'Goods In Menu
                            objGoodsInMenu = New frmGoodsInMenu()
                            'objGoodsInMenu.Show()
                            objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                            objStatusBar.Location = New Drawing.Point(0, 26 * iOffSet)
                            objStatusBar.ShowDialog()
                        Else
                            MessageBox.Show(MessageManager.GetInstance().GetMessage("M129"), "Alert", _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        End If
                    Else
                        MessageBox.Show(MessageManager.GetInstance().GetMessage("M129"), "Alert", _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    End If

                End If
            Else
                strCurrentUserID = "111"
                strSupervisorFlag = "Y"
                strPreviousUserID = "999"
                strCurrentUserName = "Boots"
                objLogger.WriteAppLog("Write SOR", Logger.LogLevel.RELEASE)
                objDataEngine.SignOn("111")
                If objDataEngine.GetConfigValues(objConfigValues) Then
                    'To parse the CSV files and insert to DB.
                    UserSessionManager.GetInstance().UpdateDB()
                    objSSCReceivingMainMenu = New frmSSCReceivingMainMenu
                    objDirReceive = New frmDirReceive
                    'Create a thread for auto logoff.
                    objAutologOff = New AutoLogOff()
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                    'To display main menu
                    objGoodsInMenu = New frmGoodsInMenu()
                    objStatusBar.Location = New Drawing.Point(0, 26 * iOffSet)
                    objStatusBar.ShowDialog()
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M129"), "Alert", _
                               MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
            End If
            'To make the registry setting for the downloader
            If bIsAppRegisterRequired Then
                objAppContainer.objHelper.RegisterDownloader()
            End If

            AppTerminate()
        Catch ex As Exception
            'SFA SIT - DEF 401
            'MessageBox.Show(" Cannot Initialise Application | " & Err.Description & " : " & Err.Number, "Exception", _
            '                           MessageBoxButtons.OK, _
            '                           MessageBoxIcon.Exclamation, _
            '                           MessageBoxDefaultButton.Button1)
            'Exit Applicaiton if Initialisation fails.
            Application.Exit()
        End Try
    End Sub
#End If
    Public Enum HSCREENS
        Calcpad
        CustmMsgBox
    End Enum
#If RF Then
    Public Function DisplayHelpScreens(ByVal ScreenName As HSCREENS) As Boolean
        Try
            Select Case ScreenName
                Case HSCREENS.Calcpad
                    objCalcpad.Invoke(New EventHandler(AddressOf DisplayCalcForQuantity))
                Case HSCREENS.CustmMsgBox
                    objMsgBox.Invoke(New EventHandler(AddressOf DisplayMesssageBox))

            End Select
        Catch ex As Exception

        End Try
    End Function
#End If

    Public Sub ShowMessageBox(ByVal strMessage As String, _
        ByVal strTitle As String, ByVal bthStyle As MessageBoxButtons)

    End Sub
    Public Function SetStatusBarText(ByVal strMessage As String)
        '  objActiveSession.SetStatusBarText(strMessage)
        '  Application.DoEvents()
    End Function
    ''' <summary>
    ''' Sub Routine to perform all operations while the application is terminated.
    ''' AppTerminate will release all objects created by the Container, dispose al forms
    ''' and gracefully kill the application.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppTerminate()
        'Perform all Terminate functions here
        Try
            bCommFailure = False
            'Close auto logoff thread.
            If Not objAutologOff Is Nothing Then
                objAutologOff.Stop()
            End If
            objAutologOff = Nothing

            objCalcpad = Nothing
            objConfigDataMgr = Nothing
            objConfigValues = Nothing
            objUserSessionMgr = Nothing
            objLogger = Nothing
            objMsgBox = Nothing
            objHelper = Nothing
            objDataEngine = Nothing
            objMsgManager = Nothing
            objConfigDataMgr = Nothing
            objValueHolder = Nothing
            objSSCReceivingMainMenu = Nothing
            objDirReceive = Nothing
            objGoodsInMenu = Nothing
            objStatusBar = Nothing
#If RF Then
            'CHANGE
            'm_SavedDetails = Nothing
            objSaveGIXMessage = Nothing
#End If
#If NRF Then
            objDBConnection.Terminate()
            objDBConnection = Nothing
#End If
            TerminateContainers()

            'Terminate the application
            'Application.Exit()

        Catch ex As Exception
            'Handle Application Terminate Exception here
            Application.Exit()
            Return
        End Try
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all Busineess Containers created by the application containers.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TerminateContainers()
        'Dispose all Containers here
        TerminateSplash()
        objSplashScreen = Nothing
    End Sub
    ''' <summary>
    ''' Display the splash screen while the container initialises other businees containers and 
    ''' application parameters.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplaySplash()
        objSplashScreen = New frmSplashScreen
        Application.Run(objSplashScreen)
    End Sub
    ''' <summary>
    ''' Terminate the Splash screen display after all initialisations.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub TerminateSplash()
        If (objSplashScreen Is Nothing) Then Return

        objSplashScreen.Invoke(New EventHandler(AddressOf objSplashScreen.CloseSplash))
        ' Close the Splash screen
        objSplashScreen.Dispose()
        objSplashScreen = Nothing
    End Sub
#If RF Then
   Public Sub DisplayMesssageBox(ByVal o As Object, ByVal e As EventArgs)
        'Optionally create
        If objMsgBox Is Nothing Then
            objMsgBox = New MsgBx
        End If
        objMsgBox.Visible = True
    End Sub
#End If


    Public Sub DisplayCalcForQuantity(ByVal o As Object, ByVal e As EventArgs)
        'Optionally create
        If objCalcpad Is Nothing Then
            objCalcpad = New frmCalcPad
        End If
        objCalcpad.Visible = True
    End Sub


    Public Sub DoCleanUp()

        Select Case objAppContainer.objActiveModule
            Case ACTIVEMODULE.BOOKINDELIVERY
#If NRF Then
                BDSessionMgr.GetInstance().EndSession(IsAbort.Yes)
#ElseIf RF Then
                BDSessionMgr.GetInstance().DisposeBookInUOD()
#End If

            Case ACTIVEMODULE.AUDITUOD
#If NRF Then
                AUODSessionManager.GetInstance().EndSession(IsAbort.Yes)
#ElseIf RF Then
                AUODSessionManager.GetInstance().DisposeAuditUOD()
#End If

            Case ACTIVEMODULE.VUOD
#If RF Then
                ' If Not m_ModScreen = ModScreen.ITEMSELECT AndAlso objActiveScreen = ACTIVESCREEN.GINMAINMNU Then
                VUODSessionMgr.GetInstance().DisposeVUOD()
                ' End If
#ElseIf NRF Then
                VUODSessionMgr.GetInstance().EndSession()
#End If

            Case ACTIVEMODULE.BOOKINCARTON
#If NRF Then
                BCSessionMgr.GetInstance().EndSession(IsAbort.Yes)
#ElseIf RF Then
                BCSessionMgr.GetInstance().DisposeBookIn()
#End If

            Case ACTIVEMODULE.AUDITCARTON
#If RF Then
                ACSessionManager.GetInstance().DisposeAuditCarton()
#ElseIf NRF Then
                ACSessionManager.GetInstance().EndSession(IsAbort.Yes)
#End If

            Case ACTIVEMODULE.VCARTON
#If RF Then
                If Not m_ModScreen = ModScreen.ITEMSELECT AndAlso objActiveScreen = ACTIVESCREEN.GINMAINMNU Then
                    VCSessionManager.GetInstance().EndSession()
                Else
                    VCSessionManager.GetInstance().EndSession()
                End If
#ElseIf NRF Then
                VCSessionManager.GetInstance().EndSession()
#End If

        End Select
        'If Not objAppContainer.bCommFailure Then

        '    ' exit from application
        '    AppTerminate()
        'End If
        'objAppContainer.bCommFailure = False
    End Sub
    Public Function AutoLogOffSession() As Boolean
        'Start splash screen for performing auto logoff
        'objSplashScreen.Label1.Text = "Auto Logoff in progress. Please wait..."
        Dim thrSplashScreen As New Thread(AddressOf DisplaySplash)
        Try
            'Display Splas Screen.
            objAppContainer.objSplashScreen.ChangeLabelText("Auto Logoff in progress...")
            'Kill the auto logoff thread.
            objAppContainer.objAutologOff.SetSleepTimeOut()
            bAutoLogOffProcess = True
            'According to the Active module write the export to the export data
            'file. For picking list and count list the export data writing is 
            'taken care by the logg of function.
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.AUDITCARTON
                    'End session for the module.
                    ACSessionManager.GetInstance().EndSession(IsAbort.Yes)
                    Exit Select
                Case AppContainer.ACTIVEMODULE.AUDITUOD
                    'End session for the module.
                    AUODSessionManager.GetInstance().EndSession(IsAbort.Yes)
                    Exit Select
                Case AppContainer.ACTIVEMODULE.BOOKINCARTON
                    'End session for the module.
                    BCSessionMgr.GetInstance().EndSession(IsAbort.Yes)
                    Exit Select
                Case AppContainer.ACTIVEMODULE.BOOKINDELIVERY
                    'End session for the module.
                    BDSessionMgr.GetInstance().EndSession(IsAbort.No)
                    Exit Select
                Case AppContainer.ACTIVEMODULE.VCARTON
                    'End session for the module.
                    VCSessionManager.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.VUOD
                    'End session for the module.
                    VUODSessionMgr.GetInstance().AutoQuitSession()
                    Exit Select
                Case Else
                    Exit Select
            End Select
            'Call Log off function to add OFF record to the end of export data file.
            UserSessionManager.GetInstance().LogOutSession(False)
            'Display a message so that user is aware of the auto logoff.
            MessageBox.Show("Auto log off is in progress. Any saved data can be downloaded during next log in.", _
                            "Auto Log off", MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            objAppContainer.objLogger.WriteAppLog("Auto Logg off success" _
                                                  , Logger.LogLevel.RELEASE)
            'terminate the splash screen.
            TerminateSplash()
            'AppTerminate()
        Catch ex As Exception
            objLogger.WriteAppLog("Error in application logoff" & _
                                  ex.Message.ToString(), _
                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
#If RF Then


    ''' <summary>
    ''' Clear all the global variable used for reconnect data recovery.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearReconnectGlobals()
        Try
            m_SavedDetails = New ArrayList()
            m_FinishedDetails = New ArrayList()
            eCurrLocation = AppContainer.CurrentLocation.None
            eDeliveryType = AppContainer.DeliveryType.None
            eFunctionType = AppContainer.FunctionType.None
            objSaveGIXMessage = New RFDataStructure.GIXMessage
            objSavedGIXMessage = New RFDataStructure.GIXMessage
            objSavedGIFFinish = New RFDataStructure.GIFMessage
        Catch ex As Exception

        End Try
    End Sub
#End If
#Region "NRF"
#If NRF Then

    
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateAppDirectories() As Boolean
        If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH))) Then
            objLogger.WriteAppLog("Cannot Create Log Directory", _
                                  Logger.LogLevel.RELEASE)

        End If
        'Fix for Database and exportdata file getting created in RF Mode
#If NRF Then

        If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                ConfigKey.EXPORT_FILE_PATH))) Then
            objLogger.WriteAppLog("Cannot Create Export Data Directory", _
                                  Logger.LogLevel.RELEASE)

        End If

        If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                ConfigKey.ACTIVE_FILE_PATH))) Then
            objLogger.WriteAppLog("Cannot Create DataBase Directory", _
                                  Logger.LogLevel.RELEASE)
        End If
#End If

    End Function
    Public Sub ForcedLogOff()
        If Not bAutoLogOffProcess Then
            objGoodsInMenu.pnlDirectsReceiving.Enabled = False
            objGoodsInMenu.pnlSSCReceiving.Enabled = False
            objGoodsInMenu.lblDirectsReceiving.ForeColor = Color.DarkGray
            objGoodsInMenu.lblSSCReceiving.ForeColor = Color.DarkGray
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M113"), "Alert", MessageBoxButtons.OK, _
                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
        bAutoLogOffProcess = False
    End Sub

#End If
#End Region
#Region "RF"
#If RF Then


      Private Function DeleteLogFiles() As Boolean
        If Not (FileIO.LogFileDelete()) Then
            objLogger.WriteAppLog("Cannot Delete old log files", _
                                              Logger.LogLevel.RELEASE)
        End If

    End Function
      ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateAppDirectories() As Boolean
        If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance().GetParam(ConfigKey.LOGFILE_PATH))) Then
            objLogger.WriteAppLog("Cannot Create Log Directory", _
                                  Logger.LogLevel.RELEASE)

        End If
    End Function

       Public Property GoodsInSession() As GoodsInSession
        Get
            Return objActiveSession
        End Get
        Set(ByVal Value As GoodsInSession)
            objActiveSession = Value
        End Set
    End Property
#End If

#End Region
#Region "Enumerated Values"

    Public Enum ACTIVEMODULE
        USERAUTH
        AUDITUOD
        AUDITCARTON
        BOOKINDELIVERY
        BOOKINCARTON
        VUOD
        VCARTON
        CALCPAD
    End Enum
    Public Enum ACTIVESCREEN
        GINAUDIT
        GINAUDITITEM
        GINAUDITITEMDETAILS
        GINAUDITSUMMARY
        AUDITCARTON
        AUDITCARTONITEM
        AUDITCARTONITEMDETAILS
        AUDITCARTONSUM
        BOOKINDELIVERYHOME
        EXPECTEDANDOUTSTANDINGSUMM
        BOOKINUODSCAN
        BATCHDRVRCONFRM
        FINALDRVRCNFRM
        BOOKINSUMMARY
        BOOKINCARTONEXPECTEDORDER
        BOOKINCARTONSCAN
        BOOKINCARTONSUMMARY
        BOOKINCARTONITEMSCAN
        BOOKINCARTONITEMINFO
        GINVIEWUODMENU
        GINVIEWUOD
        GINVIEWUODDOLLY
        GINVIEWUODNONDOLLY
        GINVIEWCARTON
        GINVIEWITEMS
        GINVIEWSUPPLIERS
        BOOKINORDERINITIAL
        BOOKINORDERSUMMARYOFCONTENTS
        BOOKINORDERSUMMARY
        BOOKINITEMFORNOORDERNUMBER
        FINALBOOKINITEMSUMMARY
        GINMAINMNU
        CALCPAD
    End Enum
    Public Enum UINAV
        None
        Splash               ' Splash screen
        SignOn
        CommunicationsSplash
    End Enum
    Public Enum ScanType
        BookInScan
        AuditScan
        RetMisdirect
        LateUODDetails
        BatchConfirm
        DeliveryConfirm
        None
    End Enum
    Public Enum ItemDetailType
        UOD
        Carton
        OrderNo
    End Enum
    Public Enum FunctionType
        BookIn = 1
        Audit = 2
        View = 3
        None = 0
    End Enum
    Public Enum DeliveryType
        SSC = 1
        Directs = 2
        ASN = 3
        None = 0
    End Enum
    Public Enum GITNoteMatch
        Yes
        No
    End Enum
    Public Enum BatchRescan
        Yes
        No
    End Enum
    Public Enum IsAbort
        Yes
        No
    End Enum
    Public Enum CurrentLocation
        None = 0
        SendingItemCount = 1
        SendingItemQuantity = 2
        SendingSessionExit = 3
        SendingENQ = 4
    End Enum
#End Region

End Class
Public MustInherit Class GoodsInSession
    Public MustOverride Sub SetStatusBarText(ByVal strMessage As String)
End Class

