Imports System.Threading
#If NRF Then
Imports System.Data.SqlServerCe
#End If
'''******************************************************************************
''' <FileName>AppContainer.vb</FileName>
''' <summary>
''' This is the Applicaiton Container Class. This class initialises the generic 
''' application paramaeters and brings up the business modules.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''******************************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' Added check whether MCF is enabled or not.
''' </Summary>
'''****************************************************************************
Public Class AppContainer

    Public objMsgManager As MessageManager = Nothing
    Public objHelper As Helper = Nothing
    Public objLogger As Logger = Nothing
    Public objDataEngine As DataEngine = Nothing
    Public objExportDataManager As SMTransactDataManager = Nothing
    Public objConfigDataMgr As ConfigDataMgr = Nothing
    Public objAutologOff As AutoLogOff = Nothing
#If NRF Then
    Public objDBConnection As DBConnections = Nothing
    Public objUserSessionMgr As NRFUserSessionManager = Nothing
#ElseIf RF Then
    Public objUserSessionMgr As RFUserSessionManager = Nothing
    'For OSSR
    Public OSSRStoreFlag As String = ""
#End If


    Public objActiveModule As ACTIVEMODULE
#If NRF Then
    'Declare variables for Price Check
    Public iCompletedCount As Integer = 0
#End If
    'Variable for storing the mobile printer status.Added for mobile printing.********** Govindh
    Public bMobilePrinterAttachedAtSignon As Boolean
    Public bIsAppRegisterRequired As Boolean = True
    Public strCurrentPrinter As String = ""

    'Decalre variables for storing the user information
    Public strCurrentUserID As String = Nothing

    'v1.1 MCF Change
    'Decalre variables for storing the active IP address
    Public strActiveIP As String = Nothing
    'v1.1 Declare variable to check whether mcf is enabled.
    Public bMCFEnabled As Boolean = False
    'v1.1 Declare variable whether connected to alternate IP
    Public iConnectedToAlternate As Integer = 0
#If RF Then
    ''' <summary>
    ''' Password Entered by user is stored
    ''' </summary>
    ''' <remarks>this password is used to reconnect when connection is lost</remarks>
    Public strPassword As String = Nothing
    'Decalre variable for storing the user information
    Public strCurrentName As String = Nothing
    Public strMACADDRESS As String = Nothing
    Public ConnectionStatus As Boolean = True
#End If

    Public strSupervisorFlag As String = Nothing
    Public strPreviousUserID As String = Nothing
    Public strCurrentUserName As String = Nothing
    Public objSplashScreen As frmSplashScreen
    Public objShlfMgmntMenu As frmShlfMgmntMenu
    'Stock File Accuracy - Added new variable to check stock specilist
    Public bIsStockSpecialist As Boolean = False
    'Public m_iCLIIndex As Integer = 0
    Public iListCount As Integer = 10
    'Variables to keep track of count list
    Public objGlobalCLProductGroupList As ArrayList = Nothing
    Public objCLSummary As CLSummary = Nothing
    'Stock File Accuracy - Additional variable to keep track of count list
    Public objGlobalCLSiteInfoTable As Hashtable = Nothing
    Public objGlobalCLInfoTable As Hashtable = Nothing
    Public objGlobalCLProductInfoTable As Hashtable = Nothing
    Public objGlobalCreateCountList As Hashtable = Nothing
    Public objGlobalCreateCountListArray As ArrayList = Nothing
    Public bIsCreateOwnList As Boolean = False
    Public m_CLOSSRCountedInfoList As ArrayList = Nothing
    Public m_CLSalesFloorCountedInfoList As ArrayList = Nothing
    Public m_CLBackShopCountedInfoList As ArrayList = Nothing

    'Variables to keep track of picking list
    Public objGlobalPickingList As ArrayList = Nothing
    Public objGlobalPLMappingTable As Hashtable = Nothing
    Public objEXMultiSiteList As ArrayList = Nothing

    'To write INS and INX for PRT records
    Public bIsPRTWritten As Boolean = False
#If NRF Then
    ' Private m_ConnectionString = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONN_STRING).ToString()
    Private m_ConnectionString As String = Nothing
    Private m_SqlEngine As SqlCeEngine = Nothing
#End If
    'Naveen
    ''printer modules is there in RF and NRF
    '#If RF Then
    Public aPrinterList() As String
    Public aPrintNos() As Char
    '#End If
    Public bConfigFile As Boolean = True
    Public iOffSet As Integer = 1
    Public strPrintFlag As String = Macros.PRINT_BATCH
    ''' <summary>
    ''' Application initialisation.
    ''' Most of this will be performed when the Splash screen is displayed at application startup.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppInitialise()
        Try
            If (System.IO.File.Exists(Macros.CONFIG_FILE_PATH)) Then
#If NRF Then
                m_ConnectionString = ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONN_STRING).ToString()
#End If
                objConfigDataMgr = ConfigDataMgr.GetInstance()
                If Not bConfigFile Then
                    Exit Sub
                End If
            Else
                System.Windows.Forms.MessageBox.Show("Config file does not exist.", "Error")
                Exit Sub
            End If
            'v1.1 MCF Change
            strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.ACTIVE_IPADDRESS).ToString()

            If Not ConfigDataMgr.GetInstance.GetIPParam _
                   (ConfigKey.PRIMARY_IPADDRESS).ToString() = ConfigDataMgr.GetInstance.GetIPParam _
                                                            (ConfigKey.SECONDARY_IPADDRESS).ToString() Then
                bMCFEnabled = True
            End If
#If RF Then
            DeleteLogFiles()
#End If

            'Do all Initilisations here
            objHelper = New Helper()
            objLogger = New Logger()
            'Creating required Directories
            CreateAppDirectories()

            'Start a Thread to display Splash screen and let the main thread do all the initialisations.
            Dim thrSplashScreen As New Thread(AddressOf DisplaySplash)

            thrSplashScreen.Start()

            'Fix for log file deletion.

            Thread.Sleep(1000)

            objMsgManager = MessageManager.GetInstance()
            
            objDataEngine = New DataEngine()
#If NRF Then
            objDBConnection = New DBConnections()
#End If
            objExportDataManager = New SMTransactDataManager()
            objGlobalCLProductGroupList = New ArrayList()
            'Stock File Accuracy  
            objGlobalCLProductInfoTable = New Hashtable()
            objGlobalCLInfoTable = New Hashtable()
            objGlobalCLSiteInfoTable = New Hashtable()
            objCLSummary = New CLSummary()          'Store the details of count list summary.
            objGlobalPickingList = New ArrayList()
            objGlobalPLMappingTable = New Hashtable()
            objGlobalCreateCountList = New Hashtable()
            m_CLSalesFloorCountedInfoList = New ArrayList()
            m_CLBackShopCountedInfoList = New ArrayList()
            m_CLOSSRCountedInfoList = New ArrayList()

            'objGlobalPickingList = New ArrayList()
            'objGlobalPLMappingTable = New Hashtable()
            objEXMultiSiteList = New ArrayList()

            'objAppContainer.objDataEngine.CreateTableIndex()
            AddHandler BCReader.GetInstance().evtBCScanned, AddressOf BCReader.GetInstance().EventBCScannedHandler

#If NRF Then
            'Set completed Price check count.
            Dim objPCTargetDetails As PCTargetDetails = New PCTargetDetails

            objAppContainer.objDataEngine.GetPCTargetDetails(objPCTargetDetails)
            iCompletedCount = objPCTargetDetails.PriceCheckCompleted
            objPCTargetDetails = Nothing
#End If
            'set the printer attach status. Added for mobile printing.********** Govindh
            If MobilePrintSessionManager.GetInstance() IsNot Nothing Then
                bMobilePrinterAttachedAtSignon = MobilePrintSessionManager.GetInstance().MobilePrinterStatus
                If bMobilePrinterAttachedAtSignon Then
                    strCurrentPrinter = "1     Mobile Printer"
                    strPrintFlag = Macros.PRINT_LOCAL
                Else
                    strCurrentPrinter = "0     Controller"
                End If
            Else
                bMobilePrinterAttachedAtSignon = False
                strCurrentPrinter = "0     Controller"
            End If
            'Check if the OS version in WM6.5 for MC55 device.
            If Environment.OSVersion.Version.ToString().StartsWith("5.2.") Then
                iOffSet = 2
                'Enter the code here to set default barcode reader as beam light
                'rather than image scanner.
            Else
                iOffSet = 1
            End If
            'Bypassing User Authentication for testing Purposes.
            If (ConfigDataMgr.GetInstance.GetParam(ConfigKey.IS_TESTING) = "False") Then
                'Intialised ConfigDataManager for User Auth
                strPreviousUserID = ConfigDataMgr.GetInstance.GetParam(ConfigKey.PREVIOUS_USER)
#If RF Then
                ''Assigning the Instance to the Reference
                objUserSessionMgr = RFUserSessionManager.GetInstance()
#ElseIf NRF Then
                objUserSessionMgr = NRFUserSessionManager.GetInstance()
#End If
                'Display the User Authenticatioin form for User Auth
                objUserSessionMgr.LaunchUser()
                'Display the Shelf Managment main menu
                If strCurrentUserID <> Nothing Then
                    'Create a thread for auto logoff.
                    objAutologOff = New AutoLogOff()
                    'Shelf Management Menu
                    objShlfMgmntMenu = New frmShlfMgmntMenu()
#If RF Then
                    ConnectionStatus = True
                    objShlfMgmntMenu.UpdateStatusBar()
#End If
                    objActiveModule = ACTIVEMODULE.NONE
                    'Display the Shelf Management home menu
                    objShlfMgmntMenu.ShowDialog()
                End If
            Else
#If NRF Then
                Dim strPrinters As String = "Controller"
                Dim strPrintNos As String = "0XXXXXXXXX"
                NRFUserSessionManager.GetInstance().UpdateDB()
                'Set to defaul User and bypass login routines.
                strCurrentUserID = "111"
                strSupervisorFlag = "Y"
                strPreviousUserID = "999"
                strCurrentUserName = "TILL OPERATOR 111"
                aPrinterList = strPrinters.Trim().Split(vbCrLf)
                strPrintNos = strPrintNos.Replace("X", "")
                strPrintNos = strPrintNos.Trim()
                aPrintNos = strPrintNos.ToCharArray()
                objLogger.WriteAppLog("Write SOR", Logger.LogLevel.RELEASE)
                'Specify default printers to use int esting mode.

                If objExportDataManager.CreateSOR("111") Then
#ElseIf RF Then
                Dim objResponse As Object = Nothing
                Dim objSNR As SNRRecord
                'Pass the complete string
                If objExportDataManager.CreateSOR("111") Then
                    strCurrentUserID = "111"
                    strPassword = "111"
                    strSupervisorFlag = "Y"
                    strPreviousUserID = "999"
                    strCurrentUserName = "TILL OPERATOR 111"
                    If DATAPOOL.getInstance.GetNextObject(objResponse) Then
                        If TypeOf (objResponse) Is SNRRecord Then
                            objSNR = CType(objResponse, SNRRecord)
                            With objAppContainer
                                objAppContainer.strCurrentUserID = objSNR.strOperatorID
                                objAppContainer.strCurrentUserName = objSNR.strUserName
                                If objSNR.cAuthorityFlag = Macros.SNR_SUPERVISOR_TAG Then
                                    .strSupervisorFlag = Macros.SUPERVISOR_YES
                                Else
                                    .strSupervisorFlag = Macros.SUPERVISOR_NO
                                End If
                                .OSSRStoreFlag = objSNR.cOSSRFlag
                                .aPrinterList = objSNR.strPrinterDescription.Trim().Split(vbCrLf)
                                .aPrintNos = objSNR.strPrinterNumber.ToCharArray()
                                .strCurrentUserName = objSNR.strUserName
                            End With
                        Else

                        End If
                        objResponse = Nothing
                        objSNR = Nothing
                    Else

                    End If
#End If
#If NRF Then
                    'Create a thread for auto logoff.
                    objAutologOff = New AutoLogOff()
#End If
                    'Shelf Management Menu
                    objShlfMgmntMenu = New frmShlfMgmntMenu()
                    'Set active module to none after quitting the module
                    objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.NONE
                    objShlfMgmntMenu.ShowDialog()

                End If
            End If
            'To make the registry setting for the downloader
            'Register MCDownloader only if the logoff is manual process.
            'If from auto logoff then donot reqister as threads will cause issue.
#If NRF Then
            If bIsAppRegisterRequired Then
                objAppContainer.objHelper.RegisterDownloader()
            End If
#End If
            'Call the terminate function 
            AppTerminate()
        Catch ex As Exception
            'MessageBox.Show(" Cannot Initialise Application | " & Err.Description & " : " & Err.Number, "Exception", _
            '                           MessageBoxButtons.OK, _
            '                           MessageBoxIcon.Exclamation, _
            '                           MessageBoxDefaultButton.Button1)
            objLogger.WriteAppLog("Exception occured at AppInitialise: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            objLogger.WriteAppLog("Exception Message: " + ex.Message)
            'Exit Applicaiton if Initialisation fails.
            AppTerminate()
        End Try
        'objLogger.WriteAppLog("Succesfully initialised AppContainer.", _
        '                                          Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Sub Routine to perform all operations while the application is terminated.
    ''' AppTerminate will release all objects created by the Container, dispose al forms
    ''' and gracefully kill the application.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub AppTerminate()
        'Perform all Terminate functions here
        Try
            'Naveen
            'Disconnect active socket
            If Not objExportDataManager Is Nothing Then
                objExportDataManager.EndSession()
                objExportDataManager = Nothing
            End If
            objSplashScreen = Nothing
            If Not objAutologOff Is Nothing Then
                objAutologOff.Stop()
            End If
            objShlfMgmntMenu = Nothing
            objAutologOff = Nothing
            BCReader.GetInstance().TerminateBCReader()
            'Stock File Accuracy 
            objGlobalCLInfoTable = Nothing
            objGlobalCLSiteInfoTable = Nothing
            objGlobalCLProductGroupList = Nothing
            objGlobalCLProductInfoTable = Nothing
            m_CLBackShopCountedInfoList = Nothing
            m_CLSalesFloorCountedInfoList = Nothing
            m_CLOSSRCountedInfoList = Nothing

            'IT Internal
            objGlobalPickingList = Nothing
            objGlobalPLMappingTable = Nothing

            objEXMultiSiteList = Nothing
            TerminateContainers()

            'BCReader.GetInstance.TerminateBCReader()
            objHelper = Nothing
            objDataEngine = Nothing
            objGlobalCreateCountList = Nothing
            objGlobalCreateCountListArray = Nothing

#If NRF Then
            objDBConnection.Terminate()
            objDBConnection = Nothing
#ElseIf RF Then
            DATAPOOL.getInstance.EndSession()
#End If
            'Terminate the application
            Application.Exit()
        Catch ex As Exception
            'Handle Application Terminate Exception here
            objLogger.WriteAppLog("Exception occured at AppTerminate: " _
                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Application.Exit()
            Return
        End Try
        objLogger.WriteAppLog("Succesfully terminated AppContainer.", _
                              Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Gracefully Terminate all Busineess Containers created by the application
    ''' containers.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private Sub TerminateContainers()
        'Dispose all Containers here
        TerminateSplash()
        frmSplashScreen = Nothing

    End Sub
    ''' <summary>
    ''' Display the splash screen while the container initialises other businees 
    ''' containers and application parameters.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplaySplash()
        objSplashScreen = New frmSplashScreen()
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
    Public Function AutoLogOffSession() As Boolean
        Try
            'Start splash screen for performing auto logoff
            objAppContainer.objSplashScreen.ChangeLabelText("Autolog off in progress. Please wait...")
            'Kill the auto logoff thread.
            objAppContainer.objAutologOff.SetSleepTimeOut()
            'set the boolean variable to prevent registering mcdownloader application
            bIsAppRegisterRequired = False
            'Label to use when there are two modules in an auto logoff session.
STARTAUTOLOGOFF:
            'According to the Active module write the export to the export data
            'file. For picking list and count list the export data writing is 
            'taken care by the logg of function.
            Select Case objAppContainer.objActiveModule
                Case AppContainer.ACTIVEMODULE.EXCSSTCK
                    'End session for the module.
                    EXSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.FASTFILL
                    'End session for the module.
                    FFSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.ITEMINFO
                    'End session for the module.
                    ItemInfoSessionMgr.GetInstance().EndSession()
                    'In case if this module is selected from other module then
                    'active module will be changed.
                    If Not objActiveModule = ACTIVEMODULE.ITEMINFO Then
                        GoTo STARTAUTOLOGOFF
                    End If
                    Exit Select
                Case AppContainer.ACTIVEMODULE.PRICECHK
                    'End session for the module.
                    'commenting this because end session again calls the write export data in NRf case 
                    'PCSessionMgr.GetInstance().WriteExportData()
                    PCSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.PRINTSEL
                    'End session for the module.
                    PSSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.PRTCLEARANCE
                    'End session for the module.
                    PSSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.SHLFMNTR
                    'End session for the module.
                    SMSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.SPCEPLAN
                    'End session for the module.
                    SPSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case AppContainer.ACTIVEMODULE.SPCEPLAN_PENDING
                    'End session for the module.
                    SPSessionMgr.GetInstance().EndSession()
                    Exit Select
                Case Else
                    Exit Select
            End Select
            'Call Log off function to add OFF record to the end of export data file.
            'anoop
#If RF Then
            RFUserSessionManager.GetInstance().LogOutSession(False)
#ElseIf NRF Then
            NRFUserSessionManager.GetInstance().LogOutSession(False)
#End If
            'Display a message so that user is aware of the auto logoff.
            MessageBox.Show("Auto log off is in progress. Any saved data can be downloaded during next log in.", _
                            "Auto Log off", MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            'objUserSessionMgr.LogOutSession(False)
            objAppContainer.objLogger.WriteAppLog("Auto Logg off success", _
                                                  Logger.LogLevel.RELEASE)
        Catch ex As Exception
            objLogger.WriteAppLog("Error in application logoff" & _
                                  ex.Message.ToString(), Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Check and create all the required application directories.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CreateAppDirectories() As Boolean
        Try
            If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOG_FILE_PATH))) Then
                objLogger.WriteAppLog("Cannot Create Log Directory", _
                                      Logger.LogLevel.RELEASE)
            End If

#If NRF Then
            If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.EXPORT_FILE_PATH))) Then
                objLogger.WriteAppLog("Cannot Create Export Data Directory", _
                                      Logger.LogLevel.RELEASE)

            End If



            If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.DATABASE_PATH))) Then
                objLogger.WriteAppLog("Cannot Create DataBase Directory", _
                                      Logger.LogLevel.RELEASE)
            End If
            'System Testing -Bug Fixing.

             If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.DATABASE_PATH) + "TempActiveFiles\")) Then
                objLogger.WriteAppLog("Cannot Create TempActiveFiles Directory", _
                                      Logger.LogLevel.RELEASE)
            End If

            If Not (FileIO.CreateDirectory(ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.DATABASE_PATH) + "TempRefFiles\")) Then
                objLogger.WriteAppLog("Cannot Create TempReferenceFiles Directory", _
                                      Logger.LogLevel.RELEASE)
            End If
#End If

           
        Catch ex As Exception
#If STUB Then
            MessageBox.Show("Error @ Create Directories", "Error:")
#End If
            objLogger.WriteAppLog("Error in App Directory Creation" & _
                                  ex.Message.ToString(), Logger.LogLevel.RELEASE)
        End Try
        
    End Function
#If RF Then
    Private Function DeleteLogFiles() As Boolean
        Try
            If Not (FileIO.LogFileDelete()) Then
                objLogger.WriteAppLog("Cannot Delete old log files", _
                                                  Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            'ignore any exceptions created at this point as it only tries to delete the log file.
        End Try
    End Function
#End If
    ''' <summary>
    ''' Enumerator to specify the active module.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ACTIVEMODULE
        SHLFMNTR
        FASTFILL
        PICKGLST
        PRICECHK
        CUNTLIST
        ITEMINFO
        EXCSSTCK
        SPCEPLAN
        PRINTSEL
        USERAUTH
        AUTOSTUFFYOURSHELVES
        PRTCLEARANCE
        ASSIGNPRINTER
        SPCEPLAN_PENDING
        ITEMSALES
        STORESALES
        REPORTS
        NONE
    End Enum
End Class
