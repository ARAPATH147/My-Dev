Imports Microsoft.VisualBasic
Imports System.Net.Sockets
Imports System.Text
Imports System.Runtime.InteropServices
Imports Goodsin.FileIO
Imports System.IO
'Imports Goodsin.RFDataManager
''' <summary>
''' This class handles the User Session Manager for User Authentication 
''' </summary>
''' <remarks></remarks>
'''
''' * Modification Log
''' 
'''*********************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Variable to display if connectivity is there in MCF configuration
''' </Summary>
'''*************************************************************************************
''' No:       Author               Date            Description 
''' 1.2     Christopher Kitto      14/04/2015   Modified as part of DALLAS project.
'''           (CK)                              Amended function ParseControlFile()to
'''                                             add WHUOD.CSV file name in the list of 
'''                                             files to be downloaded. Amended the
'''                                             functions BatchProcess, UpdateDB &
'''                                             DownloadActiveFiles
'''         Kiran Krishnan(KK)     23/04/2015   Added the code logic for DAC request
'''                                             and response processing 
'''*************************************************************************************
Public Class UserSessionManager
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private m_UserAuthHome As frmUserAuthentication
    Private Shared m_UserSessionMgr As UserSessionManager = Nothing
    Private actBuildDownloadTime As DateTime
    Private strTransactMessage As New System.Text.StringBuilder
    'Instantiate the UserInfo class
    Private objUserInfo As New UserInfo()
#If NRF Then
    'Declare object for active file parser.
    Private objExportDataManager As NRFDataManager = Nothing
    Private m_ActBuildTime As String = Nothing

    Private m_ActiveFileParser As ActiveFileParser = Nothing
#End If
    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks></remarks>
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
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As UserSessionManager
        ' Get the user auth active module 
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.USERAUTH
        If m_UserSessionMgr Is Nothing Then
            m_UserSessionMgr = New UserSessionManager()
            Return m_UserSessionMgr
        Else
            Return m_UserSessionMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        ' Instantiate the form User Authentication 
        m_UserAuthHome = New frmUserAuthentication
#If NRF Then
        'Instantiate the active file parser object
        m_ActiveFileParser = New ActiveFileParser()
#End If
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the UserSessionManager
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
    ''' To convert the String to Byte Array
    ''' </summary>
    ''' <param name="str"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function StrToByteArray(ByRef str As String) As Byte()
        'Declare the local variable 
        Dim encoding As New System.Text.ASCIIEncoding
        Return encoding.GetBytes(str)
    End Function
    ''' <summary>
    ''' To convert the Byte array to string
    ''' </summary>
    ''' <param name="readbyte"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ByteToStrArray(ByVal readbyte As Byte()) As String
        'Declare local variable 
        Dim encoding As New System.Text.ASCIIEncoding
        Return encoding.GetString(readbyte, 0, readbyte.Length - 1)
    End Function

    ''' <summary>
    ''' To check for free memory in the device
    ''' </summary>
    ''' <param name="folder"></param>
    ''' <param name="iFreemem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckForFreeMemory(ByVal folder As String, ByRef iFreemem As Long) As String
        'Declare the local variables
        Dim folderName As String = folder
        Dim freeBytesAvailableToCaller As Int32 = 0
        Dim totalNumberOfBytes As Int32 = 0
        Dim totalNumberOfFreeBytes As Int32 = 0
        Try
            'Call GetDiskFreeSpaceEx for getting the free memory space available in the specified device
            GetDiskFreeSpaceEx(folderName, freeBytesAvailableToCaller, totalNumberOfBytes, totalNumberOfFreeBytes)
            Dim totalFreeMemoryinMB As Long = totalNumberOfFreeBytes / (1024 * 1024)
            iFreemem = totalFreeMemoryinMB
        Catch ex As Exception
            Return False
        End Try
        Return iFreemem
    End Function
    ''' <summary>
    ''' To get the Disk free space using system DLL
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="lpFreeBytesAvailableToCaller"></param>
    ''' <param name="lpTotalNumberOfBytes"></param>
    ''' <param name="lpTotalNumberOfFreeBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("coredll.dll")> _
    Public Shared Function GetDiskFreeSpaceEx(ByVal directory As String, ByRef lpFreeBytesAvailableToCaller As Integer, ByRef lpTotalNumberOfBytes As Integer, ByRef lpTotalNumberOfFreeBytes As Integer) As Boolean
    End Function
#If RF Then
    Public Function LogOutSession(ByVal bIsExportDataDownload As Boolean) As Boolean
        If Not (objAppContainer.objDataEngine.LogOff(False)) Then
            objAppContainer.objLogger.WriteAppLog("Log Off failed", Logger.LogLevel.RELEASE)
            Return False
        End If
        Return True


    End Function
    ''' <summary>
    ''' Handle user authentication with various business conditions
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub HandleUserAuthentication()
        'Declare local variable 
        Dim bIsUserSame As Boolean = False
        Dim bExDataAvailable As Boolean = False
        Dim bWriteIntoExData As Boolean = False
        Dim bCrashRecoveryDone As Boolean = False
        Dim bCheckForActiveFile As Boolean = False
        Dim strDeviceIP As String = Nothing

        ' Validate whether the current user is the same as previous user
        If (Mid(m_UserAuthHome.tbSignOn.Text, 1, 3) = objAppContainer.strPreviousUserID) Then
            ' Set the boolean flag to true 
            bIsUserSame = True
        End If


        ''Check if crash recovery to be done 
        'bCrashRecoveryDone = UserSessionManager.GetInstance.CheckForCrash()
        'Apply the logger class
        If bCrashRecoveryDone = True Then
            objAppContainer.objLogger.WriteAppLog("Crash Recovery successful", Logger.LogLevel.INFO)
        Else
            objAppContainer.objLogger.WriteAppLog("Crash Recovery failure", Logger.LogLevel.INFO)
        End If

    End Sub


    ''' <summary>
    ''' To validate the user credentials login to the device
    ''' </summary>
    ''' <param name="strUserEnteredText"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ValidateUser(ByVal strUserEnteredText As String) As Boolean
        'Declare local variable for storing validation 
        Dim bTemp As Boolean = False


        Try
            If strUserEnteredText.Length < Macros.MAX_PASSWORD Then
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M26"), "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                MessageBoxDefaultButton.Button1)
                m_UserAuthHome.tbSignOn.Text = ""
                Return False
            End If
            ' Call the Data Source class for validating the user id received
            If objAppContainer.objDataEngine.GetUserDetails(strUserEnteredText, objUserInfo) Then
                UserSessionManager.GetInstance().UpdateUserDetails()
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: User validaed successfully" _
                                                              & strUserEnteredText.Substring(0, 3), _
                                                              Logger.LogLevel.INFO)
                Return True
            Else
                If Not objAppContainer.bCommFailure AndAlso Not objAppContainer.bReconnectSuccess AndAlso Not objAppContainer.bTimeOut Then
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M27"), _
                              "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                              MessageBoxDefaultButton.Button1)
                End If
                objAppContainer.bReconnectSuccess = False
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
#End If


    ''' <summary>
    ''' To update the App container user details flag and update the config file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function UpdateUserDetails() As Boolean
        'Declare local variable 
        Dim bTemp As Boolean = False

        'Update the App container user details
        objAppContainer.strCurrentUserID = objUserInfo.UserID
        objAppContainer.strSupervisorFlag = objUserInfo.SupervisorFlag
        objAppContainer.strCurrentUserName = objUserInfo.UserName
        objAppContainer.strUser = objUserInfo.Password
        Try
            'Update the BTC Config file 
            ConfigDataMgr.GetInstance.SetParam(ConfigKey.PREVIOUS_USER, objAppContainer.strCurrentUserID)  'Minu check
            bTemp = True
        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function

    ''' <summary>
    ''' To parse the response received from the controller
    ''' </summary>
    ''' <param name="strResponse"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParseResponseFromController(ByVal strResponse As String) As String
        'Declare local variable
        Dim strResponseData() As String = Nothing
        ' Split the response received 
        strResponseData = strResponse.Split(",")
        'Return the response data after putting it in array
        Return strResponseData(1)
    End Function

    ''' <summary>
    ''' To get the active files from the controller at the Start of the day
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartOfDay() As Boolean
        'Declare a local variable
        Dim bTemp As Boolean = False

        Try
            'Get the active data from the controller
            'If UserSessionManager.GetInstance.GetActiveData() Then
            '    'update the local database 
            '    If UserSessionManager.GetInstance.UpdateDB() Then
            '        bTemp = True
            '    Else
            '        objAppContainer.objLogger.WriteAppLog("Local database updation failed", Logger.LogLevel.INFO)
            '    End If
            'Else
            '    objAppContainer.objLogger.WriteAppLog("Connection Failed with Controller", Logger.LogLevel.INFO)
            'End If
        Catch ex As Exception
            Return False
        End Try
        Return bTemp
    End Function
#Region "NRF"
#If NRF Then
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
                                        "UserSessionMgr: Database updation success", _
                                        Logger.LogLevel.RELEASE)
                    Else
                        objAppContainer.objLogger.WriteAppLog( _
                                        "UserSessionMgr: Database updation failure", _
                                        Logger.LogLevel.RELEASE)
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M124"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                    End If
                Else
                    objAppContainer.objLogger.WriteAppLog( _
                                        "UserSessionMgr: tFTPing Active Files failure", _
                                        Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M123"), _
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
    ''' To check the export data availability in the device
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CheckExportDataAvailability() As Boolean
        ' Declare local variable
        Dim bTemp As Boolean = False
        Dim strExportFilePath As String = Nothing
        Dim strExportFileName As String = Nothing
        Dim strExportFile As String = Nothing

        Try
            ' check whether the file exists in the specified location 
            strExportFilePath = ConfigDataMgr.GetInstance.GetParam( _
                                    ConfigKey.EXPORT_FILE_PATH)
            strExportFileName = Macros.EXPORT_FILE_NAME
            strExportFile = strExportFilePath + strExportFileName

            ' Check if file exists
            If IO.File.Exists(strExportFile) Then
                bTemp = True
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
    ''' To check whether some crash has taken place or not
    ''' </summary>
    ''' <remarks></remarks>
    Private Function CheckForCrashAndRecover() As Boolean
        'Declare local variable
        Dim bReadLastLine As Boolean = False
        Dim bWriteIntoExData As Boolean = False
        Dim bWriteSuccess As Boolean = False
        Dim strPathOfPL As String = Nothing
        Dim strPathOfCL As String = Nothing
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

                'Insert the OFF record into the file
                If objAppContainer.objDataEngine.LogOff(True) Then
                    'If objExpDataManager.CreateOFF(True) Then
                    bWriteSuccess = True
                    'Crash recover success
                    objAppContainer.objLogger.WriteAppLog("Crash Recovery Success", _
                                                          Logger.LogLevel.RELEASE)
                End If
            End If
        Catch ex As Exception
            'Crash Recover failure
            objAppContainer.objLogger.WriteAppLog("Crash Recovery Failure" + _
            ex.StackTrace, Logger.LogLevel.INFO)
            Return bWriteSuccess
        End Try
        Return bWriteSuccess
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
            ' Dim strActDataStatus As String = ConfigDataMgr.GetInstance.GetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY)
            'If active is present then delete the active data.
            'If strActDataStatus = "True" Then
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Purging active data...")
            'initialise db connection to in activefileparser
            m_ActiveFileParser.Initialise()
            ' Call to purge the active data available in the device
            bTemp = m_ActiveFileParser.PurgeActiveData()
            'Terminate the connection opened in active file parser.
            m_ActiveFileParser.Terminate()
            'End If
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
    ''' To check condition if export data is available and is of different user
    ''' from the current user trying to login.
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ExDataDifferentUser() As Boolean
        'Set status bar
        ' m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.PROCESSING)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strDeviceIP As String = Nothing
        Dim dialog1Input As New DialogResult
        Dim dialog2Input As New DialogResult

        'Message to ask to download or delte the export data available in the device
        dialog1Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M117"), _
                                     "Alert", MessageBoxButtons.YesNo, _
                                     MessageBoxIcon.Exclamation, _
                                     MessageBoxDefaultButton.Button3)
        Try
            'If user clicks Yes to download export data
            If (dialog1Input = DialogResult.Yes) Then
                'Message to dock the device
                dialog2Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M116"), _
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
                'If user clicks No to delete the file
            ElseIf dialog1Input = DialogResult.No Then
                'Ask for user action to delete the export data of other user
                dialog2Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M118"), _
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
            '   m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Exception in ExDataDifferentUser" _
                                                  + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' To check condition if export data is available and the current user is 
    ''' same as previous one
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ExDataSameUser() As Boolean
        'Set status bar
        '  m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.PROCESSING)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
        'Declare local variable 
        Dim bTemp As Boolean = False
        Dim dialog1Input As DialogResult
        Dim dialog2Input As DialogResult

        Try
            dialog1Input = MessageBox.Show(MessageManager.GetInstance.GetMessage("M115"), _
                                           "Alert", MessageBoxButtons.YesNo, _
                                           MessageBoxIcon.Exclamation, _
                                           MessageBoxDefaultButton.Button1)
            'If user clicks on Yes
            If dialog1Input = DialogResult.Yes Then
                dialog2Input = MessageBox.Show( _
                             MessageManager.GetInstance.GetMessage("M116"), _
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
                    ' Insert the SOR record in the file 
                    InsertSOR()
                    'Write applog entry
                    objAppContainer.objLogger.WriteAppLog("Active data available is valid", _
                                                          Logger.LogLevel.INFO)
                    bTemp = True
                Else
                    'Inform the use about the invalidity of active data.
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M118"), "Info")
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
    ''' To check condition if export data is not available and current user is different from previous
    ''' </summary>
    ''' <remarks></remarks>
    Private Function ExDataUnavailable() As Boolean
        'Set status bar
        '  m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.PROCESSING)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PROCESSING)
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
    ''' To get the active files from the controller and update it to local database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetActiveData() As Boolean
        Dim objExDataTransmitter As ExDataTransmitter = Nothing
        Dim strResponse As String = Nothing
        Dim sTemp As String = ""
        Dim bTemp As Boolean = False
        Dim strSORRecord As String = Nothing
        'v1.1 Variable to display if teh controller connectivity is not there in MCF
        Dim bNoErrorDisplay As Boolean = True
        Try
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Connecting to the controller...")
            'Instantiate the Socke Connection manager class
            objExDataTransmitter = New ExDataTransmitter()
            'Initialise the onject for generating SOR record.
            objExportDataManager = New NRFDataManager()
            'Get the SOR record to be sent.
            'Minu REmove substring(5,3)
            strSORRecord = objExportDataManager.GenerateSOR( _
                                                objUserInfo.Password, _
                                                objUserInfo.UserID)
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Sending Sign On request...")
            If objExDataTransmitter.GetSocketStatus() And _
                objExDataTransmitter.SendSOR(strSORRecord.Substring(0, strSORRecord.Length - 2), m_ActBuildTime) Then
                'Set the time as per parsing rule yyyy-MM-dd HH:mm:ss
                Do
                    'V1.2 - KK
                    'Function sendDAC to check if Dallas Positive receiving store or not
                    objExDataTransmitter.SendDAC(objUserInfo.UserID)
                    'Call get active data
                    objAppContainer.objSplashScreen. _
                                ChangeLabelText("Requesting active data rebuild...")
                    'Get the response from ACT build request.
                    'Minu REmoved substring
                    strResponse = objExDataTransmitter.SendALR( _
                                          objUserInfo.UserID)
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
                        objExDataTransmitter = New ExDataTransmitter()
                    End If
                    'loop until the response received is ACK or NAK. If ACK2 is received try asain.
                Loop Until strResponse = "ACK1" Or strResponse = "NAK"
            ElseIf objAppContainer.bMCFEnabled Then
                'Reset the values
                objExDataTransmitter = Nothing
                objExportDataManager = Nothing
                'Initialise the onject for generating SOR record.
                objExportDataManager = New NRFDataManager()

                objExportDataManager.sConnectAlternateInBatch()
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                'Instantiate the Socke Connection manager class
                objExDataTransmitter = New ExDataTransmitter()
                'Get the SOR record to be sent.
                strSORRecord = objExportDataManager.GenerateSOR( _
                                                    objUserInfo.Password, _
                                                objUserInfo.UserID)
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Sending Sign On request...")
                If objExDataTransmitter.GetSocketStatus() And _
                    objExDataTransmitter.SendSOR(strSORRecord.Substring(0, strSORRecord.Length - 2), m_ActBuildTime) Then
                    'Set the time as per parsing rule yyyy-MM-dd HH:mm:ss
                    Do
                        'V1.2 - KK
                        'Function sendDAC to check if Dallas Positive receiving store or not
                        objExDataTransmitter.SendDAC(objUserInfo.UserID)
                        'Call get active data
                        objAppContainer.objSplashScreen. _
                                    ChangeLabelText("Requesting active data rebuild...")
                        'Get the response from ACT build request.
                        strResponse = objExDataTransmitter.SendALR( _
                                              objUserInfo.UserID)
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
                            objExDataTransmitter = New ExDataTransmitter()
                        End If
                        'loop until the response received is ACK or NAK. If ACK2 is received try asain.
                    Loop Until strResponse = "ACK1" Or strResponse = "NAK"
                Else
                    'Revert the IP changes made
                    objExportDataManager.sConnectAlternateInBatch()
                    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Unable to establish Socket " _
                                                          & "connection with TRANSACT service.", _
                                                          Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M125"), _
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                End If
                'If a positive acknowledgemnet received
                If strResponse = "ACK1" Then
                    ConfigDataMgr.GetInstance.SetActiveIP()
                End If
            Else
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Unable to establish Socket " _
                                                      & "connection with TRANSACT service.", _
                                                      Logger.LogLevel.RELEASE)
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M125"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
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
    ''' Handle user authentication with various business conditions
    ''' </summary>
    ''' <remarks></remarks>
    Public Function HandleUserAuthentication() As Boolean
        'Set status bar

        ' m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.PLEASE_WAIT)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        'Declare local variable 
        Dim bTemp As Boolean = False
        Dim bIsUserSame As Boolean = False
        Dim bIsExDataAvailable As Boolean = False
        Dim bWriteIntoExData As Boolean = False
        Dim bIsCrashRecovered As Boolean = False
        Dim bCheckForActiveFile As Boolean = False
        Dim strDeviceIP As String = Nothing
        Try
#If NRF Then
            'Once user authentication is successful hide the authentication screen.
            m_UserAuthHome.Visible = False
            m_UserAuthHome.Refresh()

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
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M127"), "Error", _
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
                'If export data is not available in the device.
                bTemp = ExDataUnavailable()
            End If
#End If
            'To change the splash screen text to default
            objAppContainer.objSplashScreen.ChangeLabelText()
            If bTemp = False Then
                '    m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.EMPTY)
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
                m_UserAuthHome.tbSignOn.Text = ""
                'TODO: Update the object name correctly acording to Goods In.
                'objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
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
    ''' To insert the SOR into the Export data file
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InsertSOR()
        Try
            'Minu removed substring
            'Insert the first record into the Export data file
            If objAppContainer.objDataEngine.SignOn(objUserInfo.Password) Then
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
    ''' This function will update the database with the active files tFTP from controller
    ''' </summary>
    ''' <remarks></remarks>
    Public Function UpdateDB() As Boolean
        'Declare local variable
        Dim bTemp As Boolean = False
        Try
            If m_ActiveFileParser.Initialise() Then
                'Call the Active File Parser to update the local database 
                ' V1.2 - CK
                ' Calling ParseFile() to parse the active file WHUOD.CSV
                ' If m_ActiveFileParser.ParseFile(Macros.ASN) And _
                '   m_ActiveFileParser.ParseFile(Macros.DIRECTS) And _
                '   m_ActiveFileParser.ParseFile(Macros.SSCUODOT) Then
                If objAppContainer.bDallasPosReceiptEnabled = True Then
                    If m_ActiveFileParser.ParseFile(Macros.ASN) And _
                       m_ActiveFileParser.ParseFile(Macros.DIRECTS) And _
                       m_ActiveFileParser.ParseFile(Macros.SSCUODOT) And _
                       m_ActiveFileParser.ParseFile(Macros.WHUOD) Then

                        'Set the flag to true 
                        bTemp = True
                        'update the active data availability in the config file.
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY, "True")
                        'write log
                        objAppContainer.objLogger.WriteAppLog("Database updataion success", _
                                                              Logger.LogLevel.RELEASE)
                    End If
                Else
                    If m_ActiveFileParser.ParseFile(Macros.ASN) And _
                       m_ActiveFileParser.ParseFile(Macros.DIRECTS) And _
                       m_ActiveFileParser.ParseFile(Macros.SSCUODOT) Then

                        'Set the flag to true 
                        bTemp = True
                        'update the active data availability in the config file.
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.ACTIVE_DATA_AVAILABILITY, "True")
                        'write log
                        objAppContainer.objLogger.WriteAppLog("Database updataion success", _
                                                              Logger.LogLevel.RELEASE)
                    End If
                End If
            End If
        Catch ex As Exception
            'write log
            objAppContainer.objLogger.WriteAppLog("Database updataion failure" + ex.Message + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return False
        Finally
            m_ActiveFileParser.Terminate()
        End Try
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

            ' Check the validity of the Active Data present in the device 
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
                For iCount As Integer = 0 To aExDataRecords.Length - 2
                    Dim strData As String = ""
                    Dim strRecords As String()
                    strData = aExDataRecords(iCount).ToString()
                    strRecords = strData.Split(",")
                    'Check if any line starts  with a record other than SOR
                    If strRecords(0) <> "SOR" And strRecords(0) <> "GIA" Then
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
                                    "Error in reading export data file: IsActionPerformed in UserSessionMgr" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            Return False
        End Try
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

        Try
            'Minu REmoved padleft from userid and password
            'Break the userid and password entered by the user
            strUserEnteredID = Mid(m_UserAuthHome.tbSignOn.Text, 1, 3)
            strUserEnteredPwd = Mid(m_UserAuthHome.tbSignOn.Text, 4, 6)

            'If the user entered userid and password is less than  MAX_PASSWORD do not accept it
            If strUserEnteredText.Length < Macros.MAX_PASSWORD Then
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M26"), _
                            "Alert")
                m_UserAuthHome.tbSignOn.Text = ""
                Return False
            End If
            'Call the Data Source class for validating the user id received
            If objAppContainer.objDataEngine.GetUserDetails(strUserEnteredID, objUserInfo) Then
                'Assign the retreived values from DB to the User Info 
                If Not objUserInfo Is Nothing Then
                    ' Validate the password entered from the user
                    If (strUserEnteredPwd = objUserInfo.Password) Then
                        bTemp = True
                        objAppContainer.objLogger.WriteAppLog("UserSessionMgr: User validaed successfully" _
                                                              & strUserEnteredID, _
                                                              Logger.LogLevel.INFO)
                    Else
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M114"), _
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                        MessageBoxDefaultButton.Button1)
                    End If
                End If
            Else
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M126"), _
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
                    If ValidateActiveData() And strActDataStatus = "True" Then
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
                        MessageBox.Show(MessageManager.GetInstance.GetMessage("M118"), "Info")
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
                        End If
                    End If
                Else
                    objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Error in downloading export data", _
                                                          Logger.LogLevel.RELEASE)
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M121"), _
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
    ''' <summary>
    ''' To download the export data file available in the device to the controller
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DownloadExportData() As String
        ' Declare local variable
        Dim bTemp As Boolean = False
        Dim objExDataTransmitter As ExDataTransmitter = Nothing
        Dim bErrorDisplay As Boolean = True
        'Initialise the object
        objExportDataManager = New NRFDataManager()
        Try
            'Set splash screen message
            objAppContainer.objSplashScreen.ChangeLabelText("Connecting to the controller...")
            'Instantiate the Export Data Transmitter object 
            objExDataTransmitter = New ExDataTransmitter()
            If objExDataTransmitter.GetSocketStatus() Then
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Downloading export data...")
                bErrorDisplay = False
                ' Call the function to download the export data 
                Return objExDataTransmitter.DownloadExData()
            ElseIf objAppContainer.bMCFEnabled Then
                'Change the active IP
                objExportDataManager.sConnectAlternateInBatch()
                objAppContainer.objSplashScreen.ChangeLabelText("Connecting to Alternate controller...")
                'Initialize the connection
                objExDataTransmitter = New ExDataTransmitter()
                If objExDataTransmitter.GetSocketStatus() Then
                    objAppContainer.objLogger.WriteAppLog("Connected to Alternate Controller ", _
                                                           Logger.LogLevel.RELEASE)
                    objAppContainer.objSplashScreen.ChangeLabelText("Downloading export data...")
                    bErrorDisplay = False
                    Return objExDataTransmitter.DownloadExData()
                Else
                    'Revert the IP changes made
                    objExportDataManager.sConnectAlternateInBatch()
                End If
            End If
            If bErrorDisplay Then
                objAppContainer.objLogger.WriteAppLog("UserSessionMgr: Unable to establish Socket " _
                                                      & "connection with TRANSACT service.", _
                                                      Logger.LogLevel.RELEASE)
                MessageBox.Show(MessageManager.GetInstance.GetMessage("M125"), _
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
                '  objAppContainer.objStatusBar.ShowDialog()
                objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
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
    ''' This function will do condition handling for user log off session from shelf 
    ''' management and for autologoff
    ''' </summary>
    ''' <param name="bIsExportDataDownload"></param>
    ''' <remarks>Is the logout for autologoff or explicit logout</remarks>
    Public Function LogOutSession(ByVal bIsExportDataDownload As Boolean) As Boolean
        'Set the status bar
        objAppContainer.objLogger.WriteAppLog("Enter LogOutSession", Logger.LogLevel.RELEASE)
        '    m_UserAuthHome.SetMessage(m_UserAuthHome.MSGTYPE.PLEASE_WAIT)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        'Declare local variable
        Dim bTemp As Boolean = False
        Dim strTemp As String = "-1"

        'Check for normal log off
        If bIsExportDataDownload Then
            Try
                'Insert OFF record in the export data file
                If ReadLastLine() = False Then
                    objAppContainer.objDataEngine.LogOff(False)
                End If
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
                    MessageBox.Show(MessageManager.GetInstance.GetMessage("M121"), "Alert")
                    objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.ACT_DATATTIME)
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog("UserSessionManager:Exception in LogOutSession in " _
                                                      & ex.StackTrace, Logger.LogLevel.RELEASE)
                Return False
            End Try
        ElseIf bIsExportDataDownload = False Then
            'Insert OFF record in the export data file
            If ReadLastLine() = False Then
                objAppContainer.objDataEngine.LogOff(False)
            End If
            'assign the status flag to true
            bTemp = True
        End If

        objAppContainer.objSplashScreen. _
                        ChangeLabelText("Application logging off. please wait...")
        Return bTemp
    End Function
#End If
#End Region
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
    ''' <summary>
    ''' To set or get User ID of a User.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
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
    ''' <value></value>
    ''' <returns></returns>
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
    ''' <value></value>
    ''' <returns></returns>
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
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UserName() As String
        Get
            Return user_Name
        End Get
        Set(ByVal value As String)
            user_Name = value
        End Set
    End Property
End Class
#If NRF Then
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
    Public m_lstFilesToDownload As New ArrayList
    Private m_lstFilesDownloaded As New ArrayList
    Private m_lstTimeLastBuild As New ArrayList
    Private m_lstBuildtime As New ArrayList
    Private m_TotalActiveFiles As Integer = Macros.ACTIVE_FILE_COUNT
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
            ' v1.2 - CK
            ' If the store is not enabled for Dallas Positive receiving then no. of
            ' active files is 4
            If Not objAppContainer.bDallasPosReceiptEnabled Then
                m_TotalActiveFiles -= 1
            End If
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
                While Not m_lstFilesDownloaded.Count = m_TotalActiveFiles And m_Retry > 0
                    GetControlFile()
                    ParseControlFile(strActBuildTime)
                    'sTemp = DownloadActiveFiles()
                    DownloadActiveFiles()
                    'No. of download retry permitted
                    'Wait for 1 second before retry.
                    Threading.Thread.Sleep(Macros.WAIT_BEFORE_DOWNLOAD_RETRY)
                    m_Retry = m_Retry - 1
                End While
                If m_Retry = 0 And m_lstFilesDownloaded.Count <> m_TotalActiveFiles Then 'Macros.ACTIVE_FILE_COUNT Then
                    objAppContainer.objLogger.WriteAppLog("BatchProcess: Download retry elapsed. Please try again.", _
                                                          Logger.LogLevel.RELEASE)
                    sTemp = "-1"
                Else
                    sTemp = "1"
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
            'Update the config file with the latest act build time
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
            'If objAppContainer.bMCFEnabled Then
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
            '        objAppContainer.iConnectedToAlternate = 1
            '        ConfigDataMgr.GetInstance.SetActiveIP()
            '        m_tyOptions.Host = objAppContainer.strActiveIP
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
                                                ConfigKey.FILE_TYPE_STARTINDEX)
            objFile.IsType.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILE_TYPE_LENGTH)

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
                ' V1.2 - CK
                ' Amended the If condition to add the check for WHUOD.CSV file name.
                ' If cIsType = "A" And cStatus = FILE_ACTIVE And _
                '    (strFileName = Macros.ASN Or strFileName = Macros.DIRECTS Or _
                '     strFileName = Macros.SSCUODOT Or strFileName = Macros.CONTROL) Then
                If cIsType = "A" And cStatus = FILE_ACTIVE And _
                   (strFileName = Macros.ASN Or strFileName = Macros.DIRECTS Or _
                   strFileName = Macros.SSCUODOT Or strFileName = Macros.CONTROL Or _
                   strFileName = Macros.WHUOD) Then

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
    Private Function DownloadActiveFiles() As String
        Dim rdrFileReader As System.IO.StreamReader = Nothing
        Dim staData() As String
        Dim strLine As String = Nothing
        Dim bAddToDownloaded As Boolean = False
        Dim strLocalFileName As String = Nothing
        Dim sTemp As String = "-1"
        Try
            'Start : MCF Change to connect to ALternate
            'If objAppContainer.bMCFEnabled Then
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

            '    If Not bConnectAlt Then
            '        If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
            '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
            '        Else
            '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
            '        End If
            '        objAppContainer.iConnectedToAlternate = 1
            '        ConfigDataMgr.GetInstance.SetActiveIP()
            '        m_tyOptions.Host = objAppContainer.strActiveIP
            '        objAppContainer.objLogger.WriteAppLog("Getting" + m_tyOptions.LocalFilename + "file from Alternate Controller", Logger.LogLevel.RELEASE)
            '    End If
            'End If
            'End : MCF Change to connect to ALternate
            'm_TotalActiveFiles = 0
            If m_lstFilesToDownload.Contains(Macros.CONTROL) Then
                'Download Control.csv file and parse it to get the status flags.
                'Set the trasfer option object for each of the 
                'file for subsequent tFTP
                m_tyOptions.LocalFilename = _
                            ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.LOCAL_PATH) + Macros.CONTROL
                m_tyOptions.RemoteFilename = _
                            ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.REMOTE_PATH) + Macros.CONTROL
                m_tyOptions.Host = objAppContainer.strActiveIP
                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Downloading " _
                                                                & Macros.CONTROL & " ...")
                'tFTP the file from the host server
                m_objTFTPSession.Get(m_tyOptions)
                m_lstFilesDownloaded.Add(Macros.CONTROL)

                'Set splash screen message
                objAppContainer.objSplashScreen.ChangeLabelText("Parsing " _
                                                                & Macros.CONTROL & " ...")
                'Parse control.csv
                'Load the file for reading.
                rdrFileReader = New System.IO.StreamReader(m_tyOptions.LocalFilename)
                'Reading a line from the CSV file
                strLine = rdrFileReader.ReadLine()

                While Not (strLine.StartsWith("T"))
                    strLine = strLine.Trim()
                    If strLine.StartsWith("D") Then
                        strLine = strLine.Substring(2)
                        'Parsing the line
                        staData = strLine.Split(Macros.DELIMITER)
                        'Update the config key values.
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.DIRECTS_ACTIVE, staData(18))
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.ASN_ACTIVE, staData(19))
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.UOD_ACTIVE, staData(20))
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.ONIGHT_DELIVERY, staData(21))
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.ONIGHT_SCAN, staData(22))
                        ConfigDataMgr.GetInstance.SetParam(ConfigKey.SCAN_BATCH_SIZE, staData(23))
                    End If
                    strLine = rdrFileReader.ReadLine()
                End While
                m_TotalActiveFiles = 1
                If ConfigDataMgr.GetInstance.GetParam( _
                  ConfigKey.DIRECTS_ACTIVE) = "Y" Then
                    m_TotalActiveFiles += 1
                End If

                If ConfigDataMgr.GetInstance.GetParam( _
                 ConfigKey.ASN_ACTIVE) = "Y" Then
                    m_TotalActiveFiles += 1
                End If

                If ConfigDataMgr.GetInstance.GetParam( _
                  ConfigKey.UOD_ACTIVE) = "Y" Then
                    m_TotalActiveFiles += 1
                End If

                ' V1.2 - CK
                ' If the store is enabled for Dallas positive receiving, no. of total
                ' active files is incremented by 1.
                If objAppContainer.bDallasPosReceiptEnabled = True Then
                    m_TotalActiveFiles += 1
                End If

            End If

            'Run for each loop to get all the files which are already build 
            For Each strDownload As String In m_lstFilesToDownload
                'Set the trasfer optiovn object for each of the 
                'file for subsequent 2w
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
                '    If Not bConnectAlt Then
                '        If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                '        Else
                '            objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                '        End If
                '        objAppContainer.iConnectedToAlternate = 1
                '        ConfigDataMgr.GetInstance.SetActiveIP()
                '        m_tyOptions.Host = objAppContainer.strActiveIP
                '        objAppContainer.objLogger.WriteAppLog("Getting" + m_tyOptions.LocalFilename + "file from Alternate Controller", Logger.LogLevel.RELEASE)
                '    End If
                'End If
                'End : MCF Change to connect to ALternate
                'tFTP the file from the host server
                If strDownload = Macros.DIRECTS And ConfigDataMgr.GetInstance.GetParam( _
                  ConfigKey.DIRECTS_ACTIVE) = "Y" Then
                    If m_objTFTPSession.Get(m_tyOptions) Then
                        'Add to the list of files already downloaded
                        m_lstFilesDownloaded.Add(strDownload)
                        'm_TotalActiveFiles = m_TotalActiveFiles + 1
                    End If
                ElseIf strDownload = Macros.ASN And ConfigDataMgr.GetInstance.GetParam( _
                 ConfigKey.ASN_ACTIVE) = "Y" AndAlso ConfigDataMgr.GetInstance.GetParam( _
                  ConfigKey.DIRECTS_ACTIVE) = "Y" Then
                    If m_objTFTPSession.Get(m_tyOptions) Then
                        'Add to the list of files already downloaded
                        m_lstFilesDownloaded.Add(strDownload)
                        'm_TotalActiveFiles = m_TotalActiveFiles + 1
                    End If
                ElseIf strDownload = Macros.SSCUODOT And ConfigDataMgr.GetInstance.GetParam( _
                  ConfigKey.UOD_ACTIVE) = "Y" Then
                    If m_objTFTPSession.Get(m_tyOptions) Then
                        'Add to the list of files already downloaded
                        m_lstFilesDownloaded.Add(strDownload)
                        'm_TotalActiveFiles = m_TotalActiveFiles + 1
                    End If

                    ' V1.2 - CK
                    ' If the store is enabled for Dallas positive receiving and the file to
                    ' download is WHUOD.CSV
                ElseIf strDownload = Macros.WHUOD And objAppContainer.bDallasPosReceiptEnabled = True Then
                    If m_objTFTPSession.Get(m_tyOptions) Then
                        'Add to the list of files already downloaded
                        m_lstFilesDownloaded.Add(strDownload)
                    End If
                End If
            Next
            'sTemp = "1"
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("BatchProcessor: Exception in DownloadActiveFiles in " _
                                                  & "UserSessionManager" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            sTemp = "-1"
        End Try
        Return sTemp
    End Function
    ''' <summary>
    ''' To get the total time of the last build 
    ''' </summary>
    ''' <returns></returns>
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
                                                ConfigKey.FILE_TYPE_STARTINDEX)
            objFile.IsType.Length = ConfigDataMgr.GetInstance.GetParam( _
                                                ConfigKey.FILE_TYPE_LENGTH)
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
#End If








