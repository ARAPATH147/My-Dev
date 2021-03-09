Imports Microsoft.VisualBasic
Imports System.Net.Sockets
Imports System.Text
Imports System.Runtime.InteropServices
Imports MCShMon.FileIO
Imports System.IO
Imports MCShMon.SMTransactDataManager

#If NRF Then
'''****************************************************************************
''' <FileName>UserSessionManager.vb</FileName>
''' <summary>
''' Responsible for authenticating the user while login and handling the data 
''' during autologoff and explicit logoff from the shelf management
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>27-Jan-2009</DateModified>
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
Public Class NRFUserSessionManager
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks>Declaration of variables to be used in this class</remarks>
    Private m_UserAuthHome As frmUserAuthentication
    Private Shared m_UserSessionMgr As NRFUserSessionManager = Nothing
    Private m_ActBuildTime As String = Nothing
    Private strTransactMessage As New System.Text.StringBuilder

    Private m_ActiveFileParser As ActiveFileParser = Nothing

    'Instantiate the UserInfo class
    Private objUserInfo As New UserInfo()
    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks>Intialize the session</remarks>
    Private Sub New()
        Try
            Me.StartSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Exception in UserSessionManager" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Me.EndSession()
        End Try
    End Sub
    ''' <summary>
    ''' To get the instance of the UserSessionManager class
    ''' </summary>
    ''' <remarks>Instantiate the class</remarks>
    Public Shared Function GetInstance() As NRFUserSessionManager
        ' Get the user auth active module 
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.USERAUTH
        If m_UserSessionMgr Is Nothing Then
            m_UserSessionMgr = New NRFUserSessionManager()
            Return m_UserSessionMgr
        Else
            Return m_UserSessionMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks>Instantiate the User Authentication form</remarks>
    Public Sub StartSession()
        ' Instantiate the form User Authentication 
        m_UserAuthHome = New frmUserAuthentication()
        'Instantiate the object for active file parser.

        m_ActiveFileParser = New ActiveFileParser()


    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the 
    ''' UserSessionManager
    ''' </summary>
    ''' <remarks>Close and dispose all the objects used</remarks>
    Public Function EndSession() As Boolean
        Try
            m_UserAuthHome.Close()
            m_UserAuthHome.Dispose()
            m_UserSessionMgr = Nothing
            objUserInfo = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Exception in EndSession" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' To check the export data availability in the device
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CheckExportDataAvailability() As Boolean
        ' Declare local variable
        Dim bTemp As Boolean = False
        Dim strExportFilePath As String = Nothing
        Dim strExportFileName As String = Nothing
        Dim strExportFile As String = Nothing

        Dim LastModifiedTime As Date
        Dim TimeNow As Date = Now()
        Dim DateYest As Date
        Dim CheckDate As Date

        Try
            DateYest = TimeNow.AddDays(-1)
            CheckDate = New DateTime(DateYest.Year, DateYest.Month, _
                                                DateYest.Day, _
                                                23, _
                                                0, 0)
            'check whether the file exists in the specified location 
            strExportFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.EXPORT_FILE_PATH)
            strExportFileName = Macros.EXPORT_FILE_NAME
            strExportFile = strExportFilePath + strExportFileName

            ' Check if file exists

            If IO.File.Exists(strExportFile) Then
                LastModifiedTime = IO.File.GetLastWriteTime(strExportFile)
                If LastModifiedTime < CheckDate Then
                    IO.File.Delete(strExportFile)
                    'MessageBox.Show("SuccesFully Purged data")
                Else
                    bTemp = True
                End If
                objAppContainer.objLogger.WriteAppLog("UserSessionManager: Export data available in device", _
                                                      Logger.LogLevel.INFO)
            End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(" UserSessionManager:Problem in getting " _
                                                  & "details of export data file" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To delete the active data in the device
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DeleteActiveData() As Boolean
        'Declare local variable 
        Dim bTemp As Boolean = False
        Try
            'Get the status of the active data.
            Dim strActDataStatus As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY)
            'If active is present then delete the active data.
            If strActDataStatus = "True" Then
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Purging active data...")

                'initialise db connection to in activefileparser
                m_ActiveFileParser.Initialise()
                ' Call to purge the active data available in the device
                bTemp = m_ActiveFileParser.PurgeActiveData()
                'Terminate the connection opened in active file parser.
                m_ActiveFileParser.Terminate()


            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UseraSessiojnManager: Database failure in " _
                                                  & "purging the data" + ex.Message + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To delete the export data file avaialble in the device
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DeleteExportData() As Boolean
        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strExFilePath As String = Nothing
        Dim strExFileName As String = Nothing
        Dim strExportFile As String = Nothing

        Try
            'Read the Export Data File path
            strExFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.EXPORT_FILE_PATH)
            strExFileName = Macros.EXPORT_FILE_NAME
            strExportFile = strExFilePath + strExFileName

            ' Check for the file and if exist delete it
            IO.File.Delete(strExportFile)
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Export data deleted successfully", _
                                                  Logger.LogLevel.INFO)
            If Not IO.File.Exists(strExportFile) Then
                bTemp = True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Export data delete " _
             & "failure" + ex.StackTrace, _
             Logger.LogLevel.ERROR)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To download the export data file available in the device to the controller
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DownloadExportData() As String
        ' Declare local variable
        Dim bTemp As Boolean = False
        Dim objExDataTransmitter As TransactDataTransmitter = Nothing
        Dim bErrorDisplay As Boolean = True
        Try
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Connecting to the controller...")
            'Instantiate the Export Data Transmitter object 
            objExDataTransmitter = New TransactDataTransmitter()
            If objExDataTransmitter.GetSocketStatus() Then
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Downloading export data...")
                ' Call the function to download the export data 
                'If objExDataTransmitter.DownloadExData() = "1" Then
                '    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Export data download success", _
                '                                          Logger.LogLevel.RELEASE)
                '    bTemp = True
                'ElseIf objExDataTransmitter.DownloadExData() = "-1" Then
                '    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Export data download failed", _
                '                                          Logger.LogLevel.RELEASE)
                '    bTemp = False
                'ElseIf objExDataTransmitter.DownloadExData() = "0" Then
                '    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: No Export data available to download", _
                '                                          Logger.LogLevel.RELEASE)
                '    bTemp = True
                'End If
                bErrorDisplay = False
                Return objExDataTransmitter.DownloadExData()
                'v1.1 MCF Change
            ElseIf objAppContainer.bMCFEnabled Then
                'Change the active ip to secondary IP
                objAppContainer.objExportDataManager.sConnectAlternateInBatch()
                'Change the label
                objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                'Initialize the transmitter with the new active IP
                objExDataTransmitter = New TransactDataTransmitter()
                If objExDataTransmitter.GetSocketStatus() Then
                    'Chnage the IP to alternate controller IP
                    ConfigDataMgr.GetInstance.SetActiveIP()
                    'If connected to alternate controller download export data
                    objAppContainer.objLogger.WriteAppLog("Connected to Alternate Controller ", _
                                                           Logger.LogLevel.RELEASE)
                    objAppContainer.objSplashScreen.ChangeLabelText("Downloading export data...")
                    bErrorDisplay = False
                    Return objExDataTransmitter.DownloadExData()
                Else
                    'Revert the IP changes made
                    objAppContainer.objExportDataManager.sConnectAlternateInBatch()
                End If
            End If
            If bErrorDisplay Then
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Unable to establish Socket " _
                                                      & "connection with TRANSACT service.", _
                                                      Logger.LogLevel.RELEASE)
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M60"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                Return "-1"
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Export data download " _
                                                  & "failure" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return "-1"
        End Try
        'Return bTemp
    End Function

    ''' <summary>
    ''' To get the active files from the controller and update it to local database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetActiveData() As Boolean
        Dim objExDataTransmitter As TransactDataTransmitter = Nothing
        Dim strResponse As String = Nothing
        Dim sTemp As String = ""
        Dim bTemp As Boolean = False
        Dim strSORRecord As String = Nothing
        Try
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Connecting to the controller...")
            'Instantiate the Socke Connection manager class
            objExDataTransmitter = New TransactDataTransmitter()
            'Get the SOR record to be sent.
            ' changed: substring
            strSORRecord = objAppContainer.objExportDataManager.GenerateSOR( _
                                                objUserInfo.Password, _
                                                objUserInfo.UserID)
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Sending Sign On request...")
            If objExDataTransmitter.GetSocketStatus() And _
                objExDataTransmitter.SendSOR(strSORRecord.Substring(0, strSORRecord.Length - 2), m_ActBuildTime) Then
                'Set the time as per parsing rule yyyy-MM-dd HH:mm:ss
                Do
                    'Call get active data
                    objAppContainer.objSplashScreen. _
                                ChangeLabelText("Requesting active data rebuild...")
                    'Get the response from ACT build request.
                    ' changed: substring
                    strResponse = objExDataTransmitter.SendALR(objUserInfo.UserID)
                    'Get the current time in the device to set for ACTBUILD d/l time
                    'm_ActBuildTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                    'Pass the response from ALR request to download active data
                    'and update active tables.
                    sTemp = ActiveFileDownloadAndUpdateDb(strResponse)
                    If sTemp = "1" Then
                        'Update the user details 
                        UpdateUserDetails()
                        'Insert SOR record into export data 
                        InsertSOR()
                        bTemp = True
                    ElseIf sTemp = "-1" Then
                        objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in getting active data", _
                                                              Logger.LogLevel.RELEASE)
                        bTemp = False
                    Else
                        'Re-initialise the socket.
                        objExDataTransmitter = New TransactDataTransmitter()
                    End If
                    'loop until the response received is ACK or NAK. If ACK2 is received try asain.
                Loop Until strResponse = "ACK1" Or strResponse = "NAK"
                'v1.1 MCF Change
            ElseIf objAppContainer.bMCFEnabled Then
                'Change the active IP to secondary IP
                objAppContainer.objExportDataManager.sConnectAlternateInBatch()
                'Change the label
                objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                'Initialize the transmitter with the new active IP
                objExDataTransmitter = New TransactDataTransmitter()
                If objExDataTransmitter.GetSocketStatus() Then
                    objExDataTransmitter.SendSOR(strSORRecord.Substring(0, strSORRecord.Length - 2), m_ActBuildTime)
                    'Set the time as per parsing rule yyyy-MM-dd HH:mm:ss

                    Do
                        'Call get active data
                        objAppContainer.objSplashScreen. _
                                ChangeLabelText("Requesting active data rebuild to Alternate...")
                        'Get the response from ACT build request.
                        strResponse = objExDataTransmitter.SendALR(objUserInfo.UserID)
                        'Get the current time in the device to set for ACTBUILD d/l time
                        'm_ActBuildTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                        'Pass the response from ALR request to download active data
                        'and update active tables.
                        sTemp = ActiveFileDownloadAndUpdateDb(strResponse)
                        If sTemp = "1" Then
                            'Update the user details 
                            UpdateUserDetails()
                            'Insert SOR record into export data 
                            InsertSOR()
                            bTemp = True
                        ElseIf sTemp = "-1" Then
                            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in getting active data from Alternate", _
                                                              Logger.LogLevel.RELEASE)
                            bTemp = False
                        Else
                            'Re-initialise the socket.
                            objExDataTransmitter = New TransactDataTransmitter()
                        End If
                        'loop until the response received is ACK or NAK. If ACK2 is received try asain.
                    Loop Until strResponse = "ACK1" Or strResponse = "NAK"
                    'If a positive acknowledgemnet received
                    If strResponse = "ACK1" Then
                        'Chnage the IP to alternate controller IP
                        ConfigDataMgr.GetInstance.SetActiveIP()
                    End If
                Else
                    'Revert the IP changes made
                    objAppContainer.objExportDataManager.sConnectAlternateInBatch()
                    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Unable to establish " _
                                                          & "connection with TRANSACT service.", _
                                                          Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M60"), _
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                End If

            Else
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Unable to establish " _
                                                      & "connection with TRANSACT service.", _
                                                      Logger.LogLevel.RELEASE)
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M60"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Connection intialization failed " _
                                                  & "with controller" & ex.StackTrace, _
                                                  Logger.LogLevel.ERROR)
            Return False
        End Try
        'Return true if GetActive data is successfull.
        Return bTemp
    End Function
    ''' <summary>
    ''' To Validate the active data available in the device against configurable time
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ValidateActiveData() As Boolean
        'Declare local variable
        Dim btemp As Boolean = False
        Dim dtTimeDifference As New TimeSpan
        Dim dtCurrentTime As New DateTime
        Dim dtLastActiveDataDownload As DateTime
        Dim dtLastExDataWriteTime As DateTime
        Dim iValidActiveDataTime As Integer = 0
        Dim iHour As Integer = 0
        Dim iMinutes As Integer = 0
        Dim strLastActBuildTime As String = Nothing
        Dim strExportFilePath As String = Nothing
        Dim strExportFileName As String = Nothing
        Dim strExportFile As String = Nothing
        Dim iInterval As Integer = 0
        Try
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Validating active data...")
            'Read the last Active data download time from the config file
            strLastActBuildTime = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LAST_ACTBUILD_TIME)
            dtLastActiveDataDownload = DateTime.ParseExact(strLastActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
            'Get the validity period for active data.
            iValidActiveDataTime = CInt(ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.ACTIVEDATA_VALID_TIME))
            'Get hour and minutes to get the time difference.
            iMinutes = CInt(dtLastActiveDataDownload.Minute)
            If dtLastActiveDataDownload.Day = Now.Day Then
                iHour = CInt(dtLastActiveDataDownload.Hour)
            Else
                iHour = 0
                iMinutes = 0
            End If

            'Take the current time of the user login
            dtCurrentTime = DateTime.Now

            'Calculate the difference between the lastactbuild time 
            'and current time of login time
            iInterval = dtCurrentTime.Hour * 60 + dtCurrentTime.Minute _
                        - (iHour * 60 + iMinutes)

            If iInterval < 0 Then
                iInterval = iInterval * (-1)
            End If

            'Check the validity of the Active Data present in the device 
            'from the config file
            If iInterval < iValidActiveDataTime Then
                btemp = True
                objAppContainer.objLogger.WriteAppLog("Active data available in device is latest at " _
                                                      & dtCurrentTime, Logger.LogLevel.INFO)
            ElseIf iInterval > iValidActiveDataTime Then
                'Check if export data file is present and check the last 
                'modified time for the file.
                'check whether the file exists in the specified location 
                strExportFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                        ConfigKey.EXPORT_FILE_PATH)
                strExportFileName = Macros.EXPORT_FILE_NAME
                strExportFile = strExportFilePath + strExportFileName
                ' Check if file exists
                If IO.File.Exists(strExportFile) Then
                    dtLastExDataWriteTime = File.GetLastWriteTime(strExportFile)
                    'Calculate the difference between the lastactbuild time 
                    'and current time of login time
                    iInterval = dtCurrentTime.Hour * 60 + dtCurrentTime.Minute _
                                - (dtLastExDataWriteTime.Hour * 60 + dtLastExDataWriteTime.Minute)
                    If iInterval < 0 Then
                        iInterval = iInterval * (-1)
                    End If

                    'Check the validity of the Active Data present in the device 
                    'from the config file
                    If iInterval < iValidActiveDataTime Then
                        btemp = True
                    Else
                        btemp = False
                    End If
                Else
                    'If export data file is not present.
                    btemp = False
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionManager: Exception in " _
                                                  & "validating active data at" & dtCurrentTime _
                                                  & ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return btemp
    End Function

    ''' <summary>
    ''' To validate the user credentials login to the device
    ''' </summary>
    ''' <param name="strUserEnteredText"></param>
    ''' <remarks>The user entered text in the form</remarks>
    Public Function ValidateUser(ByVal strUserEnteredText As String) As Boolean
        'Declare local variable for storing validation 
        Dim bTemp As Boolean = False
        Dim bExData As Boolean = False
        Dim strUserEnteredID As String = Nothing
        Dim strUserEnteredPwd As String = Nothing

        'Break the userid and password entered by the user
        ' Removed padleft 
        strUserEnteredID = Mid(m_UserAuthHome.tbSignOn.Text, 1, 3)
        strUserEnteredPwd = Mid(m_UserAuthHome.tbSignOn.Text, 4, 6)

        'If the user entered userid and password is less than  MAX_PASSWORD do not accept it
        If strUserEnteredText.Length < Macros.MAX_PASSWORD Then
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M26"), _
                            "Alert")
            m_UserAuthHome.tbSignOn.Text = ""
            Return False
        End If

        If CInt(strUserEnteredID) < 100 Then
            MessageBox.Show(MessageManager.GetInstance.GetMessage("M27"), _
                                         "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                         MessageBoxDefaultButton.Button1)
            Return False

        End If

        Try
            ' Call the Data Source class for validating the user id received
            If objAppContainer.objDataEngine.GetUserDetails(strUserEnteredID, objUserInfo) Then
                'Assign the retreived values from DB to the User Info 
                If Not objUserInfo Is Nothing Then
                    ' Validate the password entered from the user
                    If (strUserEnteredPwd = objUserInfo.Password) Then
                        bTemp = True
                        objAppContainer.objLogger.WriteAppLog("UserSessionMgr: User validated successfully" _
                                                              & strUserEnteredID, _
                                                              Logger.LogLevel.INFO)
                    Else
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M27"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                    End If
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M61"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: User validation failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Launches the User Authentication form to the user
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LaunchUser()
        ' Make your form  visible
        m_UserAuthHome.Visible = True
        ' Display the User Authenticatioh form 
        m_UserAuthHome.ShowDialog()
    End Sub

    ''' <summary>
    ''' To process the response from the controller
    ''' </summary>
    ''' <param name="strResponse"></param>
    ''' <remarks>The response received from the controller</remarks>
    Private Function ActiveFileDownloadAndUpdateDb(ByVal strResponse As String) As String
        'Declare local variable
        Dim sTemp As String = "-1"
        Dim bDBUpload As Boolean = False
        Dim strResponseFromController As String = Nothing
        Dim strTime As Double = Nothing
        Dim dtRetryTime As Timer = Nothing
        'Declare an object of the Batch Processor class
        Dim objBatchProcess As New BatchProcessor

        ' Read the time to retry when gets disconnected
        strTime = Convert.ToDouble(ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.TRANSACTION_TIMEOUT))
        'Convert to milliseconds
        Dim ts As TimeSpan = TimeSpan.FromMilliseconds(strTime)

        Try
            'Select case to verify the response from the cotroller
            If strResponse = "ACK1" Then
                'If ACK recevied is ACTBUILD started successfully
                'Call get active data
                objAppContainer.objSplashScreen. _
                                ChangeLabelText("Initialising active data download...")
                'Download the active files from the controller
                If objBatchProcess.BatchProcess(True, m_ActBuildTime) = "1" Then
                    'Call get active data
                    objAppContainer.objSplashScreen. _
                        ChangeLabelText("Updating active data...")
                    'Update the database with active files
                    If UpdateDB() Then
                        'Update the config file for the last act build time
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.LAST_ACTBUILD_TIME, _
                                                           m_ActBuildTime)
                        sTemp = "1"
                        objAppContainer.objLogger.WriteAppLog( _
                                        "UserSessionMgr: Database updataion success", _
                                        Logger.LogLevel.RELEASE)
                    Else
                        objAppContainer.objLogger.WriteAppLog( _
                                        "UserSessionMgr: Database updataion failure", _
                                        Logger.LogLevel.RELEASE)
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M59"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    objAppContainer.objLogger.WriteAppLog( _
                                        "UserSessionMgr: tFTPing Active Files failure", _
                                        Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M57"), _
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                End If
            ElseIf strResponse = "ACK2" Then
                'if ACK recevied is ACTBUILD ALREADY RUNNING
                'Log the response
                objAppContainer.objLogger.WriteAppLog( _
                                    "UserSessionMgr: Received ACK ACTBUILD already running", _
                                    Logger.LogLevel.RELEASE)
                'Download the active files from the controller
                sTemp = objBatchProcess.BatchProcess(False, m_ActBuildTime)
            ElseIf strResponse = "NAK" Then
                'If NAK is received from controller
                'Dispose the object of batch process
                objBatchProcess = Nothing
                'Log the response
                sTemp = "-1"
                objAppContainer.objLogger.WriteAppLog( _
                                    "UserSessionMgr: Received NAK from controller", _
                                    Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Failure in parsing response from " _
                                                  & "controller" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return "-1"
        End Try
        Return sTemp
    End Function

    ''' <summary>
    ''' To insert the SOR into the Export data file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InsertSOR()
        Try
            'Insert the first record into the Export data file
            If objAppContainer.objExportDataManager.CreateSOR(objUserInfo.Password) Then 'SFA System testing - Removed substring as zeros removed from password -Substring(5, 3)
                objAppContainer.objLogger.WriteAppLog( _
                                "UserSessionMgr: Export data file  with SOR record", _
                                Logger.LogLevel.INFO)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Export data file without SOR record" + _
                                                  ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Handle user authentication with various business conditions
    ''' </summary>
    ''' <remarks></remarks>
    Public Function HandleUserAuthentication() As Boolean
        'Set status bar
        m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)

        'Declare local variable 
        Dim bTemp As Boolean = False
        Dim bIsUserSame As Boolean = False
        Dim bIsExDataAvailable As Boolean = False
        Dim bWriteIntoExData As Boolean = False
        Dim bIsCrashRecovered As Boolean = False
        Dim bCheckForActiveFile As Boolean = False
        Dim strDeviceIP As String = Nothing

        'Once user authentication is successful hide the authentication scree.
        m_UserAuthHome.Visible = False
        m_UserAuthHome.Refresh()
        Try
            ' Validate whether the current user is the same as previous user
            If (Mid(m_UserAuthHome.tbSignOn.Text, 1, 3) = _
                                    objAppContainer.strPreviousUserID) Then
                ' Set the boolean flag to true 
                bIsUserSame = True
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Current user same as " _
                                                      & "previous user", Logger.LogLevel.INFO)
            End If

            ' Check the export data availability
            bIsExDataAvailable = CheckExportDataAvailability()

            ' Check if the OFF record is present in the export data file
            If bIsExDataAvailable = True Then
                'Check if crash recovery to be done 
                bIsCrashRecovered = CheckForCrashAndRecover()

                'Force download,if crash recovery success
                If bIsCrashRecovered Then
                    'Inform the user about the crash recovery and the force download.
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M56"), "Error", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                    'Start force download
                    If DownloadExDataAndGetActiveData() Then
                        bTemp = True
                        objAppContainer.objLogger.WriteAppLog( _
                                            "UserSessionMgr: Crash Recovery successfully downloaded", _
                                            Logger.LogLevel.RELEASE)
                    Else
                        objAppContainer.objLogger.WriteAppLog( _
                                            "UserSessionMgr: Error in downloading export data after " _
                                            & "Crash Recovery ", Logger.LogLevel.RELEASE)
                    End If
                ElseIf bIsUserSame Then
                    'Check condition if user is same
                    'Check whether validations are successful
                    bTemp = ExDataSameUser()
                ElseIf bIsUserSame = False Then
                    'Check whether validations are successful
                    bTemp = ExDataDifferentUser()
                End If
            Else
                'Check whether the conditions are checked successfully or not
                bTemp = ExDataUnavailable()
            End If
            'To change the splash screen text to default
            objAppContainer.objSplashScreen.ChangeLabelText()
            If bTemp = False Then
                m_UserAuthHome.tbSignOn.Text = ""
                m_UserAuthHome.objStatusBar.SetMessage( _
                                            CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                m_UserAuthHome.Visible = True
                m_UserAuthHome.Refresh()
            End If
            Return bTemp
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr - Exception in full HandleUserAuthentication", Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' To check condition if export data is available and the current user is 
    ''' same as previous one
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ExDataSameUser() As Boolean
        'Set status bar
        m_UserAuthHome.objStatusBar.SetMessage( _
                                CustomStatusBar.MSGTYPE.PROCESSING)
        'Declare local variable 
        Dim bTemp As Boolean = False
        Dim dialog1Input As DialogResult
        Dim dialog2Input As DialogResult

        Try
            dialog1Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M28"), _
                                           "Alert", MessageBoxButtons.YesNo, _
                                           MessageBoxIcon.Exclamation, _
                                           MessageBoxDefaultButton.Button1)
            'If user clicks on Yes
            If dialog1Input = DialogResult.Yes Then
                dialog2Input = MessageBox.Show( _
                             MessageManager.GetInstance.GetMessage("M29"), _
                             "Alert", MessageBoxButtons.OKCancel, _
                             MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                If dialog2Input = DialogResult.OK Then
                    'Download export data and get the active data after rebuild.
                    bTemp = DownloadExDataAndGetActiveData()
                Else
                    bTemp = False
                End If
                'If the user clicks on No.
            ElseIf dialog1Input = DialogResult.No Then
                'Check for validation of active data in the device
                If ValidateActiveData() Then
                    'Update the Appcontainer Variables to the current user credentials
                    UpdateUserDetails()
                    'Insert the SOR record in the file 
                    InsertSOR()
                    'Write applog entry
                    objAppContainer.objLogger.WriteAppLog("Active data available is valid", _
                                                          Logger.LogLevel.INFO)
                    bTemp = True
                Else
                    'Inform the use about the invalidity of active data.
                    ' MessageBox.Show(MessageManager.GetInstance.GetMessage("M32"), "Info")
                    'Download export data and get lastest active data.
                    bTemp = DownloadExDataAndGetActiveData()
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in ExDataSameUser" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To check condition if export data is available and is of different user
    ''' from the current user trying to login.
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ExDataDifferentUser() As Boolean
        'Set status bar
        m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)

        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strDeviceIP As String = Nothing
        Dim dialog1Input As New DialogResult
        Dim dialog2Input As New DialogResult

        'Message to ask to download or delte the export data available in the device
        dialog1Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M30"), _
                                     "Alert", MessageBoxButtons.YesNo, _
                                     MessageBoxIcon.Exclamation, _
                                     MessageBoxDefaultButton.Button3)
        Try
            'If user clicks Yes to download export data
            If (dialog1Input = DialogResult.Yes) Then
                'Message to dock the device
                dialog2Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M29"), _
                                             "Alert", MessageBoxButtons.OKCancel, _
                                             MessageBoxIcon.Exclamation, _
                                             MessageBoxDefaultButton.Button1)
                If dialog2Input = DialogResult.OK Then
                    'Download export data and get the latest active data.
                    bTemp = DownloadExDataAndGetActiveData()
                Else
                    'return false to the calling function.
                    bTemp = False
                End If
            ElseIf dialog1Input = DialogResult.No Then
                'If user clicks No to delete the file
                'Ask for user confirmation to delete the export data of other user.
                dialog2Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M31"), _
                                             "Alert", MessageBoxButtons.YesNo, _
                                             MessageBoxIcon.Exclamation, _
                                             MessageBoxDefaultButton.Button1)
                If dialog2Input = DialogResult.Yes Then
                    'User confirms to delete the data available.
                    If CheckDeviceDocked() Then
                        'Call get active data
                        objAppContainer.objSplashScreen. _
                            ChangeLabelText("Deleting export data...")
                        'Delete export data and active data present
                        If DeleteExportData() And DeleteActiveData() Then
                            'Get latest active data
                            objAppContainer.objSplashScreen. _
                                ChangeLabelText("Downloading active data...")
                            'Get latest active data from controller
                            bTemp = GetActiveData()
                        Else
                            'return false to the calling function.
                            bTemp = False
                            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in deleting active data", _
                                                              Logger.LogLevel.RELEASE)
                        End If
                    Else
                        objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Device not docked properly", _
                                                              Logger.LogLevel.RELEASE)
                    End If
                Else
                    'return false to the calling function.
                    bTemp = False
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in ExDataDifferentUser" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        'Set status bar
        m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Return bTemp
    End Function
    ''' <summary>
    ''' To check condition if export data is not available and current user is different from previous
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ExDataUnavailable() As Boolean
        'Set status bar
        m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strActDataStatus As String = "False"
        Try
            'Message to ask the user to dock the device and get latest active data
            'MessageBox.Show(MessageManager.GetInstance.GetMessage("M32"), _
            '                "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, _
            '                MessageBoxDefaultButton.Button1)
            'Check if device is docked 
            If CheckDeviceDocked() Then
                'Get the status of the active data.
                strActDataStatus = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY)
                'Check for validation of active data in the device
                'If strActDataStatus = "True" And ValidateActiveData() Then
                '    'Update the Appcontainer Variables to the current user credentials
                '    UpdateUserDetails()
                '    'Insert the SOR record in the file 
                '    InsertSOR()
                '    'Write applog entry
                '    objAppContainer.objLogger.WriteAppLog("Active data available is valid", _
                '                                          Logger.LogLevel.INFO)
                '    bTemp = True
                'Else
                'Inform the use about the invalidity of active data.
                'MessageBox.Show(MessageManager.GetInstance.GetMessage("M32"), "Info")
                'delete active data and get the latest
                'Set the splash screen message
                objAppContainer.objSplashScreen. _
                    ChangeLabelText("Deleting active data...")
                'call delete active data function
                DeleteActiveData()
                'Set the splash screen message
                objAppContainer.objSplashScreen. _
                    ChangeLabelText("Downloading active data...")
                'Get latest active data from controller
                bTemp = GetActiveData()
                'End If
            Else
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Device not docked properly", _
                                                      Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in ExDataUnavailable" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To update the App container user details flag and update the config file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateUserDetails()
        'Declare local variable 
        Dim bTemp As Boolean = False

        'Update the App container user details
        'REmoved substring
        objAppContainer.strCurrentUserID = objUserInfo.UserID
        objAppContainer.strSupervisorFlag = objUserInfo.SupervisorFlag
        objAppContainer.strCurrentUserName = objUserInfo.UserName
        'Stock File Accuracy - Set stock specialist to true if stock access flag is "Y"
        If objUserInfo.StockSpecialist = "Y" Then
            objAppContainer.bIsStockSpecialist = True
        End If
        Try
            'Update the BTC Config file 
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.PREVIOUS_USER, _
                                               objAppContainer.strCurrentUserID)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Updating config file failed" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To read the last line of the export data file
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ReadLastLine() As Boolean
        'Declare local variable
        Dim strExFilePath As String = Nothing
        Dim strExFileName As String = Nothing
        Dim strExportFile As String = Nothing
        Dim retval As String = ""
        Dim strData() As String
        Dim bRecordPresent As Boolean = False

        Try
            'Read the Export Data File path
            strExFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.EXPORT_FILE_PATH)
            strExFileName = Macros.EXPORT_FILE_NAME
            strExportFile = strExFilePath + strExFileName

            ' Open the file to read from and read till the last line
            Dim rdrFileReader As IO.StreamReader = IO.File.OpenText(strExportFile)
            Do While rdrFileReader.Peek() >= 0
                retval = rdrFileReader.ReadLine()
            Loop
            'Check whether it contains OFF record
            strData = retval.Split(",")

            'Check if the first record is OFF
            If strData(0) = "OFF" Then
                bRecordPresent = True
            End If
            'Close and dispose the streamreader object
            rdrFileReader.Close()
            rdrFileReader.Dispose()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in ReadLastLine" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bRecordPresent

    End Function
    ''' <summary>
    ''' To write the data from the temporary file into the export data file
    ''' </summary>
    ''' <param name="strTempFilePath"></param>
    ''' <remarks>The path from where the file has to be read</remarks>
    Private Function WriteFromTempFile(ByVal strTempFilePath As String) As Boolean
        'Dim declare local variable for use
        objAppContainer.objLogger.WriteAppLog("Enter UserSessionManager WriteFromTempFile", Logger.LogLevel.RELEASE)
        Dim bTemp As Boolean = False
        Dim strLine As String = Nothing
        Dim strExportFilePath As String = Nothing
        Dim strExportFileName As String = Nothing
        Dim strExportFile As String = Nothing
        Dim sInputLine As String = Nothing

        ' Get the file path for export data file
        strExportFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.EXPORT_FILE_PATH)
        strExportFileName = Macros.EXPORT_FILE_NAME
        strExportFile = strExportFilePath + strExportFileName

        Try
            'Instantiate the fileIO class and to read the file
            Dim rdrFileReader As New System.IO.StreamReader(strTempFilePath)
            'Read the whole file of the Count List temp file
            sInputLine = rdrFileReader.ReadLine()

            While rdrFileReader.Peek <> -1
                'Reading a line from the temp file
                strLine = rdrFileReader.ReadLine()
                strLine = strLine.TrimEnd(vbCrLf)
                strLine = String.Concat(strLine, Environment.NewLine)
                'Insert into the export data file
                bTemp = WriteDataIntoFile(strExportFile, strLine, True)
            End While
            'Close and dispose the stream reader objects
            rdrFileReader.Close()
            rdrFileReader.Dispose()

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in WrieFromTempFile" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit UserSessionManager WriteFromTempFile", Logger.LogLevel.RELEASE)
        Return bTemp
    End Function
    ''' <summary>
    ''' To check whether the temporary file for Picking List exist or not
    ''' </summary>
    ''' <remarks></remarks>
    Private Function IsTempPLFileExist() As Boolean
        'Declare local variable
        Dim strTempPL As String
        Dim strPathOfPL As String
        Try
            'Read the path of the temporary PL File
            strTempPL = Macros.PL_EX_FILE_NAME
            strPathOfPL = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.TEMP_EXPORT_FILE_PATH)

            'Check whether the Picking List temporary file exist 
            If IO.File.Exists(strPathOfPL + strTempPL) Then
                Return True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in IsTempPLFileExist" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' To check whether the temporary file for Count List exist or not
    ''' </summary>
    ''' <remarks></remarks>
    Private Function IsTempCLFileExist() As Boolean
        'Declare local variable 
        Dim strTempCL As String = Nothing
        Dim strPathOfCL As String = Nothing
        Try
            'Read the path of temporary Count List file
            strTempCL = Macros.CL_EX_FILE_NAME
            strPathOfCL = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.TEMP_EXPORT_FILE_PATH)
            'Check whether the Picking List temporary file exist 
            If IO.File.Exists(strPathOfCL + strTempCL) Then
                Return True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in IsTempCLFileExist" _
                                                  & ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' This function will update the database with the active files tFTP from controller
    ''' </summary>
    ''' <remarks></remarks>
    Public Function UpdateDB() As Boolean
        'Declare local variable
        Dim bTemp As Boolean = False
        Try
            'Initialise db connection
            m_ActiveFileParser.Initialise()
            'Call the Active File Parser to update the local database 
            If (m_ActiveFileParser.ParseFile(Macros.CONTROL)) _
            And (m_ActiveFileParser.ParseFile(Macros.COUNT)) _
            And (m_ActiveFileParser.ParseFile(Macros.PICKING)) Then
                'Set the flag to true 
                bTemp = True
                'update the active data availability in the config file.
                ConfigDataMgr.GetInstance.SetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY, "True")
                'write log
                objAppContainer.objLogger.WriteAppLog("Database updataion success", _
                                                      Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            'write log
            objAppContainer.objLogger.WriteAppLog("Database updataion failure" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        Finally
            'Terminate the db connection opened in active file parser.
            m_ActiveFileParser.Terminate()
        End Try
        Return bTemp
    End Function

    ''' <summary>
    ''' To check whether some crash has taken place or not
    ''' </summary>
    ''' <remarks></remarks>
    Private Function CheckForCrashAndRecover() As Boolean
        'Declare local variable
        Dim bReadLastLine As Boolean = False
        Dim bWriteIntoExData As Boolean = False
        Dim bRecoverStatus As Boolean = False
        ' Dim strPathOfPL As String = Nothing
        Dim strPathOfTempFile As String = Nothing
        Dim strTempPL As String = Nothing
        Dim strTempCL As String = Nothing
        Try
            'Read the last line from the export data file
            bReadLastLine = ReadLastLine()

            'If the file is written into the export dat file
            If bReadLastLine = False Then

                'Write the entry in the log files
                objAppContainer.objLogger.WriteAppLog("Application crash encountered", _
                                                      Logger.LogLevel.RELEASE)

                'Get the path of the temp files
                strPathOfTempFile = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.TEMP_EXPORT_FILE_PATH)
                'strPathOfPL = ConfigDataMgr.GetInstance.GetParam( _
                '                                ConfigKey.TEMP_EXPORT_FILE_PATH)
                'Get the file name of the temp file
                strTempPL = strPathOfTempFile & Macros.PL_EX_FILE_NAME
                strTempCL = strPathOfTempFile & Macros.CL_EX_FILE_NAME

                'If Count List Temp file exist write it into export data file
                If IO.File.Exists(strTempCL) Then
                    If (WriteFromTempFile(strTempCL)) Then
                        IO.File.Delete(strTempCL)
                    Else
                        objAppContainer.objLogger.WriteAppLog("Write from temp file failed. Temp file not deleted ", _
                                                          Logger.LogLevel.RELEASE)
                    End If
                End If
                'If Picking List Temp file exist write it into export data file
                If IO.File.Exists(strTempPL) Then
                    If (WriteFromTempFile(strTempPL)) Then
                        IO.File.Delete(strTempPL)
                    Else
                        objAppContainer.objLogger.WriteAppLog("Write from temp file failed. Temp file not deleted ", _
                                                          Logger.LogLevel.RELEASE)
                    End If
                End If
                'IT - Internal

                'Insert the OFF record into the file
                If objAppContainer.objExportDataManager.CreateOFF(True) Then
                    bRecoverStatus = True
                    'Crash recover success
                    objAppContainer.objLogger.WriteAppLog("Crash Recovery Success", _
                                                          Logger.LogLevel.RELEASE)
                End If
            End If
        Catch ex As Exception
            'Crash Recover failure
            objAppContainer.objLogger.WriteAppLog("Crash Recovery Failure" + _
            ex.StackTrace, Logger.LogLevel.INFO)
            Return bRecoverStatus
        End Try
        Return bRecoverStatus
    End Function
    ''' <summary>
    ''' This function will do condition handling for user log off session 
    ''' from shelf management and for autologoff
    ''' </summary>
    ''' <param name="bIsExportDataDownload"></param>
    ''' <remarks>Is the logout for autologoff or explicit logout</remarks>
    Public Function LogOutSession(ByVal bIsExportDataDownload As Boolean) As Boolean
        'Set the status bar
        objAppContainer.objLogger.WriteAppLog("Enter LogOutSession", Logger.LogLevel.RELEASE)
        m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
        'Decalre local variable
        Dim bTemp As Boolean = False
        Dim strTemp As String = "-1"
        Dim diaResult As New DialogResult

        'Check for auto logoff
        If bIsExportDataDownload = False Then
            'Insert CountList record into Export data file
            If objAppContainer.objGlobalCLProductGroupList.Count > 0 Or _
            objAppContainer.objGlobalCreateCountList.Count > 0 Then
                CLSessionMgr.GetInstance().WriteExportData(False)
            End If
            'Insert PickingList record into Export data file
            If objAppContainer.objGlobalPickingList.Count > 0 AndAlso _
            objAppContainer.objGlobalPLMappingTable.Count > 0 Then
                PLSessionMgr.GetInstance().WriteExportData()
            End If
            'Insert OFF record in the export data file
            If ReadLastLine() = False Then
                objAppContainer.objExportDataManager.CreateOFF(False)
            End If
            'Check for normal log off
            bTemp = True
        ElseIf bIsExportDataDownload = True Then
            Try
                'Insert CountList record into Export data file
                If objAppContainer.objGlobalCLProductGroupList.Count > 0 Or _
                objAppContainer.objGlobalCreateCountList.Count > 0 Then
                    CLSessionMgr.GetInstance().WriteExportData()
                End If
                'Insert PickingList record into Export data file
                If objAppContainer.objGlobalPickingList.Count > 0 AndAlso _
                objAppContainer.objGlobalPLMappingTable.Count > 0 Then
                    PLSessionMgr.GetInstance().WriteExportData()
                End If
                'Insert OFF record in the export data file
                If ReadLastLine() = False Then
                    objAppContainer.objExportDataManager.CreateOFF(False)
                End If
                'Set the status bar
                m_UserAuthHome.objStatusBar.SetMessage( _
                                            CustomStatusBar.MSGTYPE.PROCESSING)
                'If downloaded export data file done, Get active data file for 
                'next set of operation 
                strTemp = DownloadExportData()
                If strTemp = "1" Then
                    'Call function to delete export data file and delete active data
                    If DeleteActiveData() Then
                        bTemp = True
                    End If
                ElseIf strTemp = "0" Then
                    'If the export data is not present and the file is deleted.
                    bTemp = True
                Else
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M34"), _
                                    "Alert")
                End If

            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("UserSessionManager:Exception in LogOutSession in " _
                                                      & ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try
        End If
        m_UserAuthHome.objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        objAppContainer.objLogger.WriteAppLog("Exit LogOutSession", Logger.LogLevel.RELEASE)
        Return bTemp
    End Function
    ''' <summary>
    ''' To check whether the device has been docked into cradle or not
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CheckDeviceDocked() As Boolean
        'Declare local variable
        Dim strDeviceIP As String = Nothing
        Dim bTemp As Boolean = False
        Dim iCount As Integer = 3
        Do
            'Check for device IP if it has been docked 
            strDeviceIP = objAppContainer.objHelper.GetIPAddress()
            If strDeviceIP <> "127.000.000.001" And strDeviceIP <> "" And _
                                        strDeviceIP <> Nothing Then
                bTemp = True
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M33"), _
                                "Alert", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                bTemp = False
                iCount = iCount - 1
                Threading.Thread.Sleep(Macros.WAIT_BEFORE_GET_IP)
            End If
        Loop Until bTemp = True Or iCount = 0
        Return bTemp
    End Function
    ''' <summary>
    ''' This function is used to check whether any action has been performed after log in
    ''' It checks for any other record in export data other than SOR
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateExData() As Boolean
        Dim m_FileReader As StreamReader = Nothing
        Dim aExDataRecords As String()
        Dim strExportFilePath As String = ""
        Dim strExportFileName As String = ""
        Dim strExportFile As String = ""
        Dim bRecordPresent As Boolean = False

        Try
            If IsTempCLFileExist() Or IsTempPLFileExist() Then
                Return True
            End If
            'check whether the file exists in the specified location 
            strExportFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.EXPORT_FILE_PATH)
            strExportFileName = Macros.EXPORT_FILE_NAME
            strExportFile = strExportFilePath + strExportFileName

            ' Check if file exists
            If IO.File.Exists(strExportFile) Then
                m_FileReader = New StreamReader(strExportFile)
                'Read the export data file until SOR type record is read.
                aExDataRecords = Split(m_FileReader.ReadToEnd(), ControlChars.NewLine)
                'Close the stream reader.
                m_FileReader.Close()

                'Check whether the export file contains any record other than SOR
                'Count - 2 since the last value in array will be newline character
                For iCount As Integer = 0 To aExDataRecords.Count - 2
                    Dim strData As String = ""
                    Dim strRecords As String()
                    strData = aExDataRecords(iCount).ToString()
                    strRecords = strData.Split(",")
                    'Check if any line starts  with a record other than SOR
                    If strRecords(0) <> "SOR" Then
                        bRecordPresent = True
                        Exit For
                    End If
                Next
            End If
            'Delete the export data file if no other records are present
            If Not bRecordPresent Then
                IO.File.Delete(strExportFile)
            End If

            Return bRecordPresent
        Catch ex As Exception
            'Add the exception to the device log.
            AppMainModule.objAppContainer.objLogger.WriteAppLog( _
                                    "Error in reading export data file: ValidateExData in UserSessionMgr" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' Forces the user to download the export data after crash recovery is success
    ''' </summary>
    ''' <remarks></remarks>
    Private Function DownloadExDataAndGetActiveData() As Boolean
        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strTemp As String = Nothing
        Dim strActDataStatus As String = Nothing
        Try
            If CheckDeviceDocked() Then
                'call download export data
                strTemp = DownloadExportData()
                If strTemp = "1" Then
                    'Set the splash screen message
                    objAppContainer.objSplashScreen. _
                        ChangeLabelText("Deleting active data...")
                    'call delete active data
                    If DeleteActiveData() Then
                        'Set the splash screen message
                        objAppContainer.objSplashScreen. _
                            ChangeLabelText("Downloading active data...")
                        'Get latest active data from controller
                        bTemp = GetActiveData()
                    Else
                        'error because purging active data failed.
                        objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in purging active data", _
                                                                  Logger.LogLevel.RELEASE)
                        bTemp = False
                    End If
                ElseIf strTemp = "0" Then
                    'Get the status of the active data.
                    strActDataStatus = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY)
                    'Check for validation of active data in the device
                    'If ValidateActiveData() And strActDataStatus = "True" Then
                    '    'Update the Appcontainer Variables to the current user credentials
                    '    UpdateUserDetails()
                    '    'Insert the SOR record in the file 
                    '    InsertSOR()
                    '    'Write applog entry
                    '    objAppContainer.objLogger.WriteAppLog("Active data available is valid", _
                    '                                          Logger.LogLevel.INFO)
                    '    bTemp = True
                    'Else
                    'Inform the use about the invalidity of active data.
                    'MessageBox.Show(MessageManager.GetInstance.GetMessage("M32"), "INFO")
                    'delet active data and get the latest
                    'Set the splash screen message
                    objAppContainer.objSplashScreen. _
                        ChangeLabelText("Deleting export data...")
                    'call delete export data
                    If DeleteActiveData() Then
                        'Set the splash screen message
                        objAppContainer.objSplashScreen. _
                            ChangeLabelText("Downloading active data...")
                        'Get latest active data from controller
                        bTemp = GetActiveData()
                    Else
                        'error because purging active data failed.
                        objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in purging active data", _
                                                                  Logger.LogLevel.RELEASE)
                        bTemp = False
                        'End If
                    End If
                Else
                    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in downloading export data", _
                                                          Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M34"), _
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Device not docked properly", _
                                                      Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in ForceDownload in " _
                                                  & "UserSessionManager" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
End Class






''' <summary>
''' The class holds the values for the User Info who has been validated against the database data
''' </summary>
''' <remarks></remarks>
Public Class UserInfo
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Public user_ID As String
    Public userFlag As String
    Public userPassword As String
    Public user_Name As String
    Public stock_specialist As String
    ''' <summary>
    ''' To set or get User ID of a User.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property UserID() As String
        Get
            Return user_ID
        End Get
        Set(ByVal value As String)
            user_ID = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the password.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Password() As String
        Get
            Return userPassword
        End Get
        Set(ByVal value As String)
            userPassword = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the Supervisor flag.
    ''' </summary>
    ''' <remarks></remarks>
    Public Property SupervisorFlag() As String
        Get
            Return userFlag
        End Get
        Set(ByVal value As String)
            userFlag = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the User Name
    ''' </summary>
    ''' <remarks></remarks>
    Public Property UserName() As String
        Get
            Return user_Name
        End Get
        Set(ByVal value As String)
            user_Name = value
        End Set
    End Property
    ''' <summary>
    ''' To set or get the stock adjustment flag
    ''' </summary>
    ''' <remarks></remarks>
    Public Property StockSpecialist() As String
        Get
            Return stock_specialist
        End Get
        Set(ByVal value As String)
            stock_specialist = value
        End Set
    End Property
End Class
#End If
''' <summary>
''' To read the Control File description
''' </summary>
''' <remarks></remarks>
Public Class ControllerFile
    Public Name As String
    Public FileNameField As FieldProperty
    Public BuildStatus As FieldProperty
    Public TimeLastBuild As FieldProperty
    Public LastBuildDuration As FieldProperty
    Public IsType As FieldProperty
    Public AlreadyDownloaded As String
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks>The member variables are intialized</remarks>
    Public Sub New()
        FileNameField = New FieldProperty
        BuildStatus = New FieldProperty
        TimeLastBuild = New FieldProperty
        IsType = New FieldProperty
        LastBuildDuration = New FieldProperty
    End Sub
End Class
''' <summary>
''' To read the Field Property to be read from the Control file
''' </summary>
''' <remarks></remarks>
Public Class FieldProperty
    Public StartIndex As Integer
    Public Length As Integer
End Class
''' <summary>
''' The class performs the batch processing of getting the active files 
''' from the controller
''' </summary>
''' <remarks></remarks>
Public Class BatchProcessor
    Public Const FILE_ACTIVE As String = "E"
    Private m_objTFTPSession As New TFTPClient.TFTPSession()
    Private m_tyOptions As New TFTPClient.TransferOptions()
    Private m_lstFilesToDownload As New ArrayList
    Private m_lstFilesDownloaded As New ArrayList
    Private m_lstTimeLastBuild As New ArrayList
    Private m_lstBuildtime As New ArrayList
    Private m_DownloadRetryCount As Integer = 0
    Private m_Retry As Integer = 0
    ''' <summary>
    ''' To initiate the Batch Process of uploading the active files 
    ''' from the controller
    ''' </summary>
    ''' <param name="strAction"></param>
    ''' <remarks>Download files immediately or after calculated time</remarks>
    Public Function BatchProcess(ByVal strAction As Boolean, ByVal strActBuildTime As String) As String
        'Declare local variable
        Dim sTemp As String = "0"
        Dim iSeconds As Integer = 0
        Dim strHostIP As String = Nothing
        Dim iPort As Integer = 69
        Try
            'Read the Server IP address from the configuration file
            'strHostIP = ConfigDataMgr.GetInstance.GetParam(ConfigKey.SERVER_IPADDRESS_TFTP)
            strHostIP = objAppContainer.strActiveIP
            iPort = CInt(ConfigDataMgr.GetInstance.GetParam(ConfigKey.IPPORT_TFTP))
            m_DownloadRetryCount = CInt(ConfigDataMgr.GetInstance.GetParam(ConfigKey.SYNCTRL_DOWNLOAD_RETRY))
            'Set the parameters for getting the control file from the controller
            m_tyOptions.Host = strHostIP
            m_tyOptions.Port = iPort
            m_tyOptions.Action = TFTPClient.TransferType.Get
            m_Retry = m_DownloadRetryCount
            If strAction Then
                'To continue the process of getting all the active files from 
                'controller
                While Not m_lstFilesDownloaded.Count = Macros.ACTIVE_FILE_COUNT And m_Retry > 0
                    GetControlFile()
                    ParseControlFile(strActBuildTime)
                    DownloadActiveFiles()
                    sTemp = "1"
                    'No. of download retry permitted
                    'Wait for 1 second before retry.
                    Threading.Thread.Sleep(Macros.WAIT_BEFORE_DOWNLOAD_RETRY)
                    m_Retry = m_Retry - 1
                End While
                If m_Retry = 0 And m_lstFilesDownloaded.Count <> Macros.ACTIVE_FILE_COUNT Then
                    objAppContainer.objLogger.WriteAppLog("BatchProcess: Download retry elapsed. Please try again.", _
                                                          Logger.LogLevel.RELEASE)
                    sTemp = "-1"
                End If
            Else
                'Calculate the time taken by the last active build and wait.
                'tFTP the control file from the controller
                GetControlFile()
                'Retreive the total time after which again tFTP to be done
                iSeconds = TotalLastTimeBuild()
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Waiting for ACTBUILD to complete...")
                'Wait for time taken by the last ACTBUILD to issue next ALR request.
                System.Threading.Thread.Sleep(iSeconds * 1000)
                sTemp = "0"
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in BatchProcess in " _
                                                  & "UserSessionManager" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return "-1"
        End Try
        Return sTemp
    End Function
    ''' <summary>
    ''' Get the control file from the Controller
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetControlFile()
        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strRemoteFileName As String = Nothing
        Dim strLocalFileName As String = Nothing
        Dim strLocalPath As String = Nothing
        Dim strRemotePath As String = Nothing
        Try
            'Tell the local controller file name path to the tFTPd
            strLocalPath = ConfigDataMgr.GetInstance.GetParam(ConfigKey.LOCAL_PATH)
            strLocalFileName = strLocalPath + ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONTROL_FILE_NAME)

            'Tell the remote controller file name to be tFTPd
            strRemotePath = ConfigDataMgr.GetInstance.GetParam(ConfigKey.REMOTE_PATH)
            'remote filename
            strRemoteFileName = strRemotePath + ConfigDataMgr.GetInstance.GetParam(ConfigKey.CONTROL_FILE_NAME)

            'Set the Transfer Options object for tFTP
            m_tyOptions.LocalFilename = strLocalFileName
            m_tyOptions.RemoteFilename = strRemoteFileName
            m_tyOptions.Host = objAppContainer.strActiveIP
            'Set splash screen message
            objAppContainer.objSplashScreen. _
                    ChangeLabelText("Downloading control file...")
            'Start : MCF Change to connect to ALternate
            'If objAppContainer.bMCFEnabled And objAppContainer.iConnectedToAlternate <> 1 Then
            '    Dim m_Socket As TcpClient
            '    m_Socket = New TcpClient()
            '    Dim bConnectAlt As Boolean = False
            '    Try
            '        m_Socket.Connect(m_tyOptions.Host, m_tyOptions.Port)
            '    Catch ex As Exception
            '        bConnectAlt = True
            '    End Try

            '    If Not m_Socket.Client.Connected Then
            '        bConnectAlt = True
            '    End If

            '    If bConnectAlt Then
            '        If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
            '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
            '        Else
            '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
            '        End If
            '        ConfigDataMgr.GetInstance.SetActiveIP()
            '        m_tyOptions.Host = objAppContainer.strActiveIP
            '        objAppContainer.iConnectedToAlternate = 1
            '        objAppContainer.objLogger.WriteAppLog("Getting SYNCTRL.DAT file from Alternate Controller", Logger.LogLevel.RELEASE)
            '    End If
            'End If
            'End : MCF Change to connect to ALternate
            'Download file now
            If m_objTFTPSession.Get(m_tyOptions) Then
                bTemp = True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in GetControlFile in " _
												& "UserSessionManager" + ex.StackTrace, _
												Logger.LogLevel.RELEASE)
            bTemp = False
        End Try
    End Sub
    ''' <summary>
    ''' To Parse the control file and store the details about the files
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ParseControlFile(ByVal strActBuildTime As String)
        'Declare local variables
        Dim strControlFilePath As String = Nothing
        Dim strControlFileName As String = Nothing
        Dim strControlFile As String = Nothing
        Dim bFirstLine As Boolean = True
        Dim cStatus As Char = Nothing
        Dim cIsType As Char = Nothing
        Dim strFileName As String = Nothing
        Dim dtTimeLastBuild As DateTime = Nothing
        Dim dtActBuildTime As DateTime = Nothing
        Dim strTimeLastBuild As String = Nothing
        Dim bAlreadyToDownload As Boolean = False
        Dim srCtrlFile As StreamReader = Nothing
        Dim isFileNotPresent As Boolean = True
        Dim iTemp As Integer = 0
        'Instantiate the File class
        Dim objFile As New ControllerFile

        Try
            dtActBuildTime = DateTime.ParseExact(strActBuildTime, "yyyy-MM-dd HH:mm:ss", Nothing)
            'Clear the array containing files to download.
            m_lstFilesToDownload.Clear()
            'Read the path of control file 
            strControlFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LOCAL_PATH)
            strControlFileName = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.CONTROL_FILE_NAME)
            strControlFile = strControlFilePath + strControlFileName

            Dim str As String = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILENAME_FIELD_START_INDEX)

            'Gather the details about the Sync Conrol File      
            objFile.Name = strControlFile
            objFile.FileNameField.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILENAME_FIELD_START_INDEX)
            objFile.FileNameField.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILENAME_FIELD_LENGTH)
            objFile.BuildStatus.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.BUILDSTATUS_FIELD_START_INDEX)
            objFile.BuildStatus.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.BUILDSTATUS_FIELD_LENGTH)
            objFile.TimeLastBuild.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.TIME_LAST_BUILD_STARTINDEX)
            objFile.TimeLastBuild.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.TIME_LAST_BUILD_LENGTH)
            objFile.IsType.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.IS_TYPE_STARTINDEX)
            objFile.IsType.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.IS_TYPE_LENGTH)

            'Declare a variable for reading a line fromm sync control file
            Dim strLine As String = Nothing
            'Instantiate the stream reader class
            srCtrlFile = New StreamReader(strControlFile)
            'Set splash screen message
            objAppContainer.objSplashScreen. _
                    ChangeLabelText("Parsing control file...")

            'Read line by line and process the file details
            Do While srCtrlFile.Peek() > -1
                'Read the file line by line
                strLine = srCtrlFile.ReadLine()
                'Read the type of file
                cIsType = strLine.Substring(objFile.IsType.StartIndex, _
                                            objFile.IsType.Length)
                'Read the name of the file in the first line
                cStatus = strLine.Substring(objFile.BuildStatus.StartIndex, _
                                            objFile.BuildStatus.Length)
                'Get the file name for whom the status is "E"
                strFileName = strLine.Substring( _
                                objFile.FileNameField.StartIndex, _
                                objFile.FileNameField.Length).Trim()
                'If the file is of type Active file and status of the file is End
                If cIsType = "A" And cStatus = FILE_ACTIVE And _
                   (strFileName = Macros.CONTROL Or strFileName = Macros.PICKING Or _
                   strFileName = Macros.COUNT) Then

                    'Get the time last build for whom the status is"E"
                    strTimeLastBuild = strLine.Substring( _
                                        objFile.TimeLastBuild.StartIndex, _
                                        objFile.TimeLastBuild.Length)
                    strTimeLastBuild = strTimeLastBuild.Insert(4, "-")
                    strTimeLastBuild = strTimeLastBuild.Insert(7, "-")
                    strTimeLastBuild = strTimeLastBuild.Insert(10, " ")
                    strTimeLastBuild = strTimeLastBuild.Insert(13, ":")
                    strTimeLastBuild = strTimeLastBuild.Insert(16, ":")
                    dtTimeLastBuild = DateTime.ParseExact(strTimeLastBuild, "yyyy-MM-dd HH:mm:ss", _
                                                          Nothing)
                    'Compare the date time of last build time with the active build time.
                    iTemp = DateTime.Compare(dtTimeLastBuild, dtActBuildTime)

                    'Assuming that the file is not present in the downloaded list.
                    isFileNotPresent = True
                    'if there is no files downloaded so far then add the file name 
                    'to download list
                    If m_lstFilesDownloaded.Count <= 0 And iTemp > 0 Then
                        'Add the file name and time last build of this file 
                        'to the list of to be downloaded
                        m_lstFilesToDownload.Add(strFileName)
                        m_lstBuildtime.Add(dtTimeLastBuild)
                        'reset download retry
                        m_Retry = m_DownloadRetryCount
                    ElseIf m_lstFilesDownloaded.Count > 0 And iTemp > 0 Then
                        For Each strDownloadedFile As String In m_lstFilesDownloaded
                            If strDownloadedFile = strFileName Then
                                isFileNotPresent = False
                                'If file already downloaded then check for next file.
                                Exit For
                            End If
                        Next
                        'If file is not in the downloaded list then add to the list.
                        If isFileNotPresent Then
                            'Add the file name and time last build of 
                            'this file to the list of to be downloaded
                            m_lstFilesToDownload.Add(strFileName)
                            m_lstBuildtime.Add(dtTimeLastBuild)
                            'reset download retry
                            m_Retry = m_DownloadRetryCount
                        End If
                    End If
                End If
            Loop
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr - Exception in full ParseControlFile", Logger.LogLevel.RELEASE)
        Finally
            'Close the streamreader object and dispose it 
            srCtrlFile.Close()
            srCtrlFile.Dispose()
        End Try
    End Sub
    ''' <summary>
    ''' Download the active files from the controller one by one
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DownloadActiveFiles()
        'Declare local variable 
        Dim bAddToDownloaded As Boolean = False
        Dim strLocalFileName As String = Nothing
        Try
            'Run for each loop to get all the files which are already build 
            For Each strDownload As String In m_lstFilesToDownload

                'Set the trasfer optiovn object for each of the 
                'file for subsequent tFTP
                m_tyOptions.LocalFilename = _
                            ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.LOCAL_PATH) + strDownload
                m_tyOptions.RemoteFilename = _
                            ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.REMOTE_PATH) + strDownload
                m_tyOptions.Host = objAppContainer.strActiveIP
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Downloading " _
                                                                & strDownload & " ...")
                'Start : MCF Change to connect to ALternate
                'If objAppContainer.bMCFEnabled And objAppContainer.iConnectedToAlternate <> 1 Then
                '    Dim m_Socket As TcpClient
                '    m_Socket = New TcpClient()
                '    Dim bConnectAlt As Boolean = False
                '    Try
                '        m_Socket.Connect(m_tyOptions.Host, m_tyOptions.Port)
                '    Catch ex As Exception
                '        bConnectAlt = True
                '    End Try

                '    If Not m_Socket.Client.Connected Then
                '        bConnectAlt = True
                '    End If

                '    If bConnectAlt Then
                '        If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                '        Else
                '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                '        End If
                '        ConfigDataMgr.GetInstance.SetActiveIP()
                '        m_tyOptions.Host = objAppContainer.strActiveIP
                '        objAppContainer.iConnectedToAlternate = 1
                '        objAppContainer.objLogger.WriteAppLog("Getting" + m_tyOptions.LocalFilename + "file from Alternate Controller", Logger.LogLevel.RELEASE)
                '    End If
                'End If
                'End : MCF Change to connect to ALternate
                'tFTP the file from the host server
                If m_objTFTPSession.Get(m_tyOptions) Then
                    'Add to the list of files already downloaded
                    m_lstFilesDownloaded.Add(strDownload)
                End If
            Next
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("BatchProcessor: Exception in DownloadActiveFiles in " _
                                                  & "UserSessionManager" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To get the total time of the last build 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function TotalLastTimeBuild() As Integer
        'Declare local variables
        Dim strControlFilePath As String = Nothing
        Dim strControlFileName As String = Nothing
        Dim strControlFile As String = Nothing
        Dim cIsType As Char = Nothing
        Dim srCtrlFile As StreamReader = Nothing
        Dim strLastActBuildTime As String = Nothing
        Dim iHour As Integer = 0
        Dim iMinutes As Integer = 0
        Dim iSeconds As Integer = 0
        'Instantiate the File class
        Dim objFile As New ControllerFile

        Try
            'Read the path of control file 
            strControlFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LOCAL_PATH)
            strControlFileName = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.CONTROL_FILE_NAME)
            strControlFile = strControlFilePath + strControlFileName

            'Gather the details about the Sync Conrol File      
            objFile.IsType.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.IS_TYPE_STARTINDEX)
            objFile.IsType.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.IS_TYPE_LENGTH)
            objFile.LastBuildDuration.StartIndex = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LAST_DURATION_START_INDEX)
            objFile.LastBuildDuration.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.LAST_DURATION_LENGTH)

            'Declare a variable for reading a line fromm sync control file
            Dim strLine As String = Nothing
            'Instantiate the stream reader class
            srCtrlFile = New StreamReader(strControlFile)
            'Set splash screen message
            objAppContainer.objSplashScreen. _
                    ChangeLabelText("Parsing control file...")
            'Read the file line by line
            strLine = srCtrlFile.ReadLine()
            'Read line by line and process the file details
            Do While srCtrlFile.Peek() > -1 And strLine.Trim <> Nothing
                'Read the type of file
                cIsType = strLine.Substring(objFile.IsType.StartIndex, _
                                            objFile.IsType.Length)

                'If the file is of type Active file and status of the file is End
                If cIsType = "A" Then
                    'Get the latest act build time 
                    strLastActBuildTime = strLine.Substring( _
                                        objFile.LastBuildDuration.StartIndex, _
                                        objFile.LastBuildDuration.Length)
                    iHour = iHour + CInt(strLastActBuildTime.Substring(0, 2))
                    iMinutes = iMinutes + CInt(strLastActBuildTime.Substring(2, 2))
                    iSeconds = iSeconds + CInt(strLastActBuildTime.Substring(4, 2))
                End If
                'Read the file line by line
                strLine = srCtrlFile.ReadLine()
            Loop
            'calculate the total time taken in seconds.
            TotalLastTimeBuild = (iHour * 60 * 60) + (iMinutes * 60) + iSeconds
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr - Exception in full ParseControlFile", Logger.LogLevel.RELEASE)
        Finally
            'Close the streamreader object and dispose it 
            srCtrlFile.Close()
            srCtrlFile.Dispose()
        End Try
    End Function
End Class
