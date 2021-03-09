Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Data
Imports System.Data.SqlServerCe
Imports MCDownloader.FileIO
Imports System.Threading
Imports System.Windows.Forms
Imports System.Text
'''****************************************************************************
''' <FileName>AppContainer.vb</FileName>
''' <summary>
''' The Main application container class which will intialise all the 
''' applciation parameters.AppContainer Class to initialize and control the 
''' program exection
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
''' MCF changes - Checks whether MCF is enabled or not
''' </Summary>
'''****************************************************************************
'''* 1.2    Arun Karikunnath       Jan 2016
''' <Summary>
''' Automated Date and Time changes - Gets the latest date and time from 
''' controller by sending a SOR message before checking for refference data 
''' download.
''' </Summary> 
'''**************************************************************************** 
Public Class AppContainer
    Private m_tmrAlarm As System.Windows.Forms.Timer
    Private m_iAlarmCounter As Integer = 1
    Private m_bExitFlag As Boolean = False
    Private m_objPwrState As PowerState
    Public m_objConfigParams As ConfigParams
    Public strLogFilename As String = ""
    Private Shared m_objAppcontainer As AppContainer
    Public obLogger As Logger
    Public objRefDownloadForm As frmDownloadReferenceData
    Public objSummaryScreen As frmSummaryScreen
    Private m_bFirstInvoke As Boolean = False

    'v1.2 Start - Automated Date and Time changes
    Private m_ActBuildTime As String = Nothing
    Private m_Retry As Integer = 0
    Private m_SockectConnMgr As SocketConnectionMgr = Nothing
    Private m_ControllerDateTime As String = Nothing
    'To get system time.
    <DllImport("coredll.dll")> _
    Public Shared Sub GetSystemTime(ByRef lpSystemTime As SystemTime)
    End Sub
    'P/Invoke dec for setting the system time
    <DllImport("coredll.dll")> _
    Private Shared Function SetLocalTime(ByRef time As SYSTEMTIME) As Boolean
    End Function
    'v1.2 End - Automated Date and Time changes

    Dim m_dtFirstInvokeToday As DateTime
    Dim m_dtFirstDownloadToday As DateTime
    Dim m_dtAppExitToday As DateTime
    'v1.1 Start - MCF Changes
    Public strActiveIP As String = Nothing

    'v1.1 Declare variable for whether mcf enabled.
    Public bMCFEnabled As Boolean = False
    'v1.1 Declare variable whether connected to alternate IP
    Public iConnectedToAlternate As Integer = 0

    'Private m_SockectConnMgr As SocketConnectionMgr = Nothing
 	'v1.1 End - MCF Changes
    ''' <summary>
    ''' Used to create and send the log messages
    ''' </summary>
    ''' <remarks></remarks>
    Public objLogMessageTransmitter As LOGTransmitter
    ''' <summary>
    ''' Contructor, retrieves information from configuration xml
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        obLogger = New Logger()
        'get config params from xml
        strLogFilename = obLogger.GetLogFileName()
        m_objConfigParams = ConfigParser.GetInstance().GetConfigParams()
    End Sub
    ''' <summary>
    ''' GetInstance to get the instance of singleton class
    ''' </summary>
    ''' <returns>AppContainer</returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As AppContainer
        If m_objAppcontainer Is Nothing Then
            m_objAppcontainer = New AppContainer()
            Return m_objAppcontainer
        Else
            Return m_objAppcontainer
        End If
    End Function
    ''' <summary>
    ''' The Start Method to enter into the processing logic
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Start()
        'v1.2 Start - Automated Date and Time changes
        'If the device rebooted the function will invoked to get the current date and time from the
        'controller. The function will be passed with dummy user name and password.
#If NRF Then
        GetTime("XXX", "XXX")
#End If

        'v1.2 End - Automated Date and Time changes

        m_objPwrState = New PowerState
        'If the device is invoked for the first time for the day at 00:05 AM
        'jusr warm reboot the device to make sure that no unwanted process is running that time.
        'When ever the device is warm rebooted, AIRBEAM script will automatically start 
        'MCDownloader application in all the device.
        m_dtFirstInvokeToday = New DateTime(Today.Year, Today.Month, _
                                               Today.Day, _
                                               Hour(m_objConfigParams.FirstInvokeTime), _
                                               Minute(m_objConfigParams.FirstInvokeTime), 0)
        m_dtFirstDownloadToday = New DateTime(Today.Year, Today.Month, _
                                            Today.Day, _
                                            Hour(m_objConfigParams.FirstDownloadTime), _
                                            Minute(m_objConfigParams.FirstDownloadTime), 0)
        'v1.1 Start - MCF Changes
        strActiveIP = ConfigParser.GetInstance.GetIPParam(IPParams.ActiveIP)
        'v1.1 Set the MCF enabled variable if the IPs in the config files are different
        If Not ConfigParser.GetInstance.GetIPParam(IPParams.PrimaryIP) = _
               ConfigParser.GetInstance.GetIPParam(IPParams.SecondaryIP) Then
            bMCFEnabled = True
        End If
        'v1.1 End - MCF Changes
        If CBool(m_objConfigParams.FirstInvokeForToday) And _
         m_dtFirstInvokeToday.CompareTo(Now) <= 0 And _
         m_dtFirstDownloadToday.CompareTo(Now) > 0 Then
            'Set FirstInvokeForToday to False so that the next invoke will 
            'not restart the device.
            m_objConfigParams.FirstInvokeForToday = "False"
            m_objConfigParams.RestartRequired = "True"
            'Change to delet and reset the downloaded files count.
            m_objConfigParams.DownloadCount = 0
            'Delete the batch config file.
            File.Delete(m_objConfigParams.HomeDir & m_objConfigParams.StatusFileName)
            'Update the batch config XML file.
            ConfigParser.GetInstance().UpdateConfig()
            'Unset suspend timeout to avoid device power off during download.
            m_objPwrState.UnSetSleepTimeOut()
            'Register MCDownloader for next day's invoke.
            RegisterApp()
            obLogger.WriteAppLog("Device reboot initiated....", Logger.LogLevel.RELEASE)
            'Restart the device if all file download and db population is completed.
            Restart.GetInstance.ResetPocketPC()
            Application.Exit()
            Exit Sub
        Else
            'Set FirstInvokeForToday to True if it is not the first invoke.
            m_objConfigParams.FirstInvokeForToday = "True"
            ConfigParser.GetInstance().UpdateConfig()
        End If

        'Check if this is the first invoke for the day.
        If Not CheckFirstInvoke() Then
            RegisterApp()
            Application.Exit()
            obLogger.WriteAppLog("Invoke Time Invalid (Not Valid Run Time for Downloader). Exitting the application.", _
                                 Logger.LogLevel.RELEASE)
            'Return use exit function instead
            Exit Sub
        End If

        'Check if this is the first application instance or 
        'application might exit due to error conditon or crash
        If InstanceChecker.IsInstanceRunning() Then
            obLogger.WriteAppLog("Another instance of the Downloader is running. Quiting the application.", _
                                 Logger.LogLevel.RELEASE)
            Application.Exit()
            'Return
            Exit Sub
        End If

        'Set registry such that the device will not enter sleep state until all
        'reference files are updated to the database.
        Try
            '==============================================
            ' Set system power state to avoid system sleep 
            '==============================================
            'Set suspend time out values to "0" to disable device suspend while downloader is running.
            m_objPwrState.UnSetSleepTimeOut()
            objRefDownloadForm = New frmDownloadReferenceData()
            'v1.1 MCF Changes
            objLogMessageTransmitter = New LOGTransmitter(AppContainer.GetInstance.strActiveIP, m_objConfigParams.ControllerPort)
            Dim objLogFileUploader As New LogFileUploader()
            DisplayScreen(Macros.REF_DOWNLOAD_SCREEN)

            If objLogFileUploader.CheckDeviceDocked() Then
                'if first invoke spown a thread to upload log files to server.
                If m_bFirstInvoke Then
                    '====================================================================
                    ' DOWNLOAD LOG FILES AND DELETE EXDATA FILE IF ANY PRESENT IN DEVICE 
                    '====================================================================
                    obLogger.WriteAppLog("First invoke for the day. " _
                                         & "Initialising log file download.", Logger.LogLevel.RELEASE)
                    'Dim strStatusFilename As String = Nothing

                    Dim thLogger As New Threading.Thread(AddressOf objLogFileUploader.Start)
                    thLogger.Start()
                    obLogger.WriteAppLog("Log file download thread started.", _
                                         Logger.LogLevel.RELEASE)
                    'Delete the export data files if any present in the folder.
                    DeleteExDataFile()
                End If
                'sets timer to raise events at particular intervals
                StartTimer()
                obLogger.WriteAppLog("Set timer to start at file download defined intervals.", _
                                         Logger.LogLevel.RELEASE)
                'continue waiting till the other threads finish processing
                While m_bExitFlag = False
                    'Processes all the events in the queue.
                    Application.DoEvents()
                End While
                'If exitting after downloading all ref files, restart the device.
                If m_bExitFlag And m_objConfigParams.RestartRequired = "True" Then
                    obLogger.WriteAppLog("Reference file download completed.", _
                                         Logger.LogLevel.RELEASE)
                    'Set back the restart flag.
                    m_objConfigParams.RestartRequired = "False"
                    ConfigParser.GetInstance().UpdateConfig()
                    'Restart the device if all file download and db population is completed.
                    Restart.GetInstance.ResetPocketPC()
                End If
            Else
                obLogger.WriteAppLog("Start Device is not docked Properly or IP address does not exists for the device..", _
                                     Logger.LogLevel.RELEASE)
                'To diplay the reason in the UI.
                objRefDownloadForm.SetCurrentStatus("Device is not docked properly. Exiting application in 20 seconds.")
                'Sleep for 20s and then exit.
                Threading.Thread.Sleep(20000)
                'Exit the application
                objRefDownloadForm.DisposeForm()
                Application.Exit()
            End If
        Catch ex As Exception
            'log the exception here
            obLogger.WriteAppLog("AppContainer:: Start:: Exception at Appcontainer. Exception @ ::" + ex.StackTrace, _
                                 Logger.LogLevel.RELEASE)
            'Leave unattended mode
            m_objPwrState.SetSleepTimeOut(m_objConfigParams.BattSuspendTimeout)
            'Restart the device if all file download and db population is completed.
            Restart.GetInstance.ResetPocketPC()
        Finally
            If Not (objRefDownloadForm Is Nothing) Then
                objRefDownloadForm = Nothing
            End If
            'Leave unattended mode
            m_objPwrState.SetSleepTimeOut(m_objConfigParams.BattSuspendTimeout)
            obLogger.WriteAppLog("AppContainer:: Start:: Exiting MCDownloader application.", _
                                     Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' The timer paramers are set 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartTimer()
        ' Adds the event and the event handler for the method that will
        ' process the timer event to the timer.
        m_tmrAlarm = New System.Windows.Forms.Timer()
        AddHandler m_tmrAlarm.Tick, AddressOf TimerEventProcessor

        'The timer will tick only after the interval expires.
        If m_bFirstInvoke Then
            'If it is the first invoke and first invoke time is earlier that current time
            'then set interval to invoke at first download time.
            If m_dtFirstInvokeToday.CompareTo(Now) < 0 Then
                m_tmrAlarm.Interval = IntervalTill(m_dtFirstDownloadToday)
            Else
                'If current time is later than first invoke time wait for 10s and start.
                m_tmrAlarm.Interval = 10000
            End If
        Else
            'If it is not the first invoke for the day then, set the interval as 10s
            m_tmrAlarm.Interval = 10000
        End If

        'Runs the timer to raise the event.
        m_tmrAlarm.Enabled = True
        obLogger.WriteAppLog("AppContainer:: Start Timer:: Timer Interval is set and timer is started.", _
                                     Logger.LogLevel.RELEASE)
    End Sub
    ''' <summary>
    ''' Function to get the timedifference in milliseconds between time.now and the timepassed
    ''' </summary>
    ''' <param name="dtTime"></param>
    ''' <returns>tsDifference</returns>
    ''' <remarks></remarks>
    Private Function IntervalTill(ByVal dtTime As DateTime) As Integer
        Dim tsDifference As TimeSpan
        tsDifference = dtTime.Subtract(Now)
        Return tsDifference.TotalMilliseconds
    End Function
    ''' <summary>
    ''' v1.2 added as part of Automated Date and Time changes
    ''' Function to get the time and date from the controller using SOR message
    ''' </summary>
    ''' <returns>boolean</returns>
    ''' <remarks></remarks>
    Private Function GetTime(ByVal strUserName As String, ByVal strPassword As String) As Boolean
        'Hardcoding garbage values as the below fields are not relevant in current scenario
        Dim strFreeMem As String = Nothing
        Dim strMacID As String = "000000000000"
        Dim strDeviceType As String = "R"
        Dim strAppId As String = "006"
        Dim strIPAdd As String = "XXX.XXX.XXX.XXX"
        Dim strAppVersion As String = "0001"
        Dim strExportDataString As New System.Text.StringBuilder()
        Dim strSORRecords As String
        strFreeMem = "00000000"
        'Creating the dummy SOR message record
        strExportDataString.Append("SOR")
        strExportDataString.Append(strUserName)
        strExportDataString.Append(strPassword)
        strExportDataString.Append(strAppId)
        strExportDataString.Append(strAppVersion)
        strExportDataString.Append(strMacID)
        strExportDataString.Append(strDeviceType)
        strExportDataString.Append(strIPAdd)
        strExportDataString.Append(strFreeMem)

        strSORRecords = strExportDataString.ToString()

        SendSOR(strSORRecords, m_ActBuildTime)

    End Function
    ''' <summary>
    ''' v1.2 added as part of Automated Date and Time changes
    ''' To send SOR record.
    ''' </summary>
    ''' <param name="strRecords"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendSOR(ByVal strRecords As String, ByRef strDateTime As String) As Boolean
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
    ''' <summary>
    ''' v1.2 added as part of Automated Date and Time changes
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
        m_SockectConnMgr = New SocketConnectionMgr()
        Try
            'Records sent to the controller.
            obLogger.WriteAppLog("ExDataTransmitter: Record sent: ", Logger.LogLevel.RELEASE)
            'new message format
            strRecord = Chr(255) + (strRecord.Length + 5).ToString.PadLeft(4, "0") + strRecord
            m_SendBytes = Encoding.ASCII.GetBytes(strRecord.ToString())
            m_SendBytes(0) = &HFF
            m_RetryWrite = m_Retry
            '  m_SendBytes = Encoding.ASCII.GetBytes(strRecord)
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
                                                             m_ReadBytes.Length))
                    Else
                        'Add the exception to the application log.
                        obLogger.WriteAppLog("ExDataTransmitter: Cannot " _
                                                              & "read from socket.", _
                                                              Logger.LogLevel.RELEASE)
                        'If reading response from the controller is failed.
                        MessageBox.Show("Failed")
                        Return False
                    End If
                End If
                m_RetryWrite = m_RetryWrite - 1
            Loop Until m_RetryWrite = 0
            'If all the write attempt failed.
            If m_RetryWrite = 0 Then
                'write the error message to the app log
                obLogger.WriteAppLog( _
                            "Unable to write record to the stream. Retry attempt" _
                            & "failed for" & m_RetryWrite & "times", _
                            Logger.LogLevel.RELEASE)
                Return False
            End If

        Catch ex As Exception
            MessageBox.Show("Record not send")
            'Add the exception to the device log.
            obLogger.WriteAppLog( _
                                    "Error in sending export data to controller:" _
                                    & ex.Message.ToString(), _
                                    Logger.LogLevel.RELEASE)
            'incase of exception return false.
            Return False
        Finally
            m_SendBytes = Nothing
            m_ReadBytes = Nothing
            'Close the socket connection to the controller.
            m_SockectConnMgr.TerminateConnection()
        End Try
    End Function
    ''' <summary>
    ''' v1.2 added as part of Automated Date and Time changes
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
    Private Function ParseResponse(ByVal m_ResponseMessage As String) As Boolean
        'new message format
        m_ResponseMessage = m_ResponseMessage.Substring(5, m_ResponseMessage.Length - 5)
        'Response received from the controller.
        obLogger.WriteAppLog("ExDataTransmitter: Response received: ", Logger.LogLevel.RELEASE)
        'Based on the message type parse the response and return the value.
        Select Case m_ResponseMessage.Substring(0, 3)
            Case "ACK"
                Return True
            Case "GIB"
                Return True
            Case "GIR"
                Return True
            Case "SNR"
                'set the device date time according to the date time received 
                'in the response.
                m_ControllerDateTime = m_ResponseMessage.Substring(Macros.SNR_DATETIME_START_INDEX, _
                                                      Macros.SNR_DATETIME_LENGTH)
                Return SetDeviceDateTime(m_ControllerDateTime)
            Case "NAK"
                Dim strNakMessage As String = ""
                strNakMessage = m_ResponseMessage.Replace("NAK", "")    'Supress NAK String
                strNakMessage = strNakMessage.Replace("NAKERROR", "")   'Suppress NAKERROR string
                'Display the recevied NAK message to the user.
                MessageBox.Show("Received error from controller:" + strNakMessage, _
                                "Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
                'If response received is NAK
                Return False
        End Select
    End Function
    ''' <summary>
    ''' v1.2 added as part of Automated Date and Time changes
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
            .wYear = Convert.ToInt16(strDateTime.Substring(0, 4))
            .wMonth = Convert.ToInt16(strDateTime.Substring(4, 2))
            .wDay = Convert.ToInt16(strDateTime.Substring(6, 2))
            .wHour = Convert.ToInt16(strDateTime.Substring(8, 2))
            .wMinute = Convert.ToInt16(strDateTime.Substring(10, 2))
            .wSecond = Convert.ToInt16(0)
        End With

        'Set the new time`
        Return SetLocalTime(objSysTime)
    End Function
    ''' <summary>
    ''' Function to set the next auto Inovoke time
    ''' Returns true if the function is invoked inbetween startime and endtime for 
    ''' daily download
    ''' Returns false if invoked at any other time - assumes that the application is 
    ''' invoked to set the first autoinvoke
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function CheckFirstInvoke() As Boolean
        Dim bReturn As Boolean = False
        Try
            obLogger.WriteAppLog("AppContainer:: CheckFirstInvoke:: Setting next invoke Time.", Logger.LogLevel.RELEASE)
            m_dtFirstInvokeToday = New DateTime(Today.Year, Today.Month, _
                                                Today.Day, _
                                                Hour(m_objConfigParams.FirstInvokeTime), _
                                                Minute(m_objConfigParams.FirstInvokeTime), 0)
            obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: First invoke expected time : " & _
                                 m_dtFirstInvokeToday.ToString(), Logger.LogLevel.RELEASE)
            m_dtFirstDownloadToday = New DateTime(Today.Year, Today.Month, _
                                                Today.Day, _
                                                Hour(m_objConfigParams.FirstDownloadTime), _
                                                Minute(m_objConfigParams.FirstDownloadTime), 0)
            obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: First download expected time : " & _
                                 m_dtFirstDownloadToday.ToString(), Logger.LogLevel.RELEASE)
            m_dtAppExitToday = New DateTime(Today.Year, Today.Month, _
                                               Today.Day, _
                                               Hour(m_objConfigParams.AppExitTime), _
                                               Minute(m_objConfigParams.AppExitTime), 0)
            obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: App exit expected time : " & _
                                 m_dtAppExitToday.ToString(), Logger.LogLevel.RELEASE)
            '==========================================
            ' IF CURRENT TIME IS GREATER THAN 07:10AM  
            '==========================================
            If m_dtAppExitToday.CompareTo(Now) < 0 Then
                bReturn = False
            ElseIf m_dtFirstInvokeToday.CompareTo(Now) < 0 AndAlso m_dtFirstDownloadToday.CompareTo(Now) > 0 Then
                '========================================================
                ' IF MCDOWNLOADER IS INVOKED BETWEEN 12:01AM AND 12:20AM 
                '========================================================
                m_bFirstInvoke = True
                bReturn = True
                obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: Application invoked at time : " & Now.ToString(), _
                                     Logger.LogLevel.RELEASE)
            Else
                'For app invoked between 00:21 and 07:10
                obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: Application invoked at time : " & Now.ToString(), _
                                     Logger.LogLevel.RELEASE)
                m_bFirstInvoke = False
                bReturn = True
            End If
            'register application to get invoked at first invoke 00:01am time next day.
            'RegisterApp()

            obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: Setting next invoke time completed.", Logger.LogLevel.RELEASE)
        Catch ex As Exception
            obLogger.WriteAppLog("AppContainer::CheckFirstInvoke:: Exception occured during SetNextInvoke :: Exception Message: " & ex.Message, _
                                 Logger.LogLevel.RELEASE)
        End Try
        Return bReturn
    End Function
    ''' <summary>
    ''' This is the method to run when the timer is raised.
    ''' </summary>
    ''' <param name="myObject"></param>
    ''' <param name="myEventArgs"></param>
    ''' <remarks></remarks>
    Private Sub TimerEventProcessor(ByVal myObject As Object, ByVal myEventArgs As EventArgs)
        m_tmrAlarm.Enabled = False
        'process any messages that are in queue during the wait time.
        Application.DoEvents()
        Try
            obLogger.WriteAppLog("AppContainer:: TimeEventProcessor:: Application started at : " & Now.ToString(), _
                                     Logger.LogLevel.RELEASE)
            If m_dtAppExitToday.CompareTo(Now) < 0 Then
                '============================================================
                ' IF MCDOWNLOADER IS INVOKED BEFORE 12:01AM OR AFTER 07:10AM     
                '============================================================
                'Register the application to run at next day 12:01 am.
                RegisterApp()
                'Do not enable the timer.
                m_tmrAlarm.Enabled = False
                m_bExitFlag = True
                If m_objConfigParams.DownloadCount >= m_objConfigParams.ReferenceFileCount Then
                    m_objConfigParams.BuildInProgress = "N"
                    ConfigParser.GetInstance().UpdateConfig()
                    'Execute the sample query.
                    'ExecuteSampleQuery()
                    obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Reference file download , parsing and DB Population completed.", _
                                         Logger.LogLevel.RELEASE)
                    'Set information text in the download status screen.
                    objRefDownloadForm.SetCurrentStatus("Successfully registered the application for next invoke.")
                    'Wait for 5 secs.
                    Threading.Thread.Sleep(5000)
                    objRefDownloadForm.DisposeForm()
                    'Application.Exit()
                End If
            Else
                '========================================================
                ' IF MCDOWNLOADER IS INVOKED BETWEEN 12:03AM AND 05:00AM 
                '========================================================
                'spawn thread to handle index file download and the rest
                Dim objBatchProcessor As New BatchProcessor()
                'objRefDownloadForm.SetTitle(frmDownloadReferenceData.FormTitle.frmReferenceDataDownload)
                'Start processing the SYNCTRL.DAT download and parse the file
                'to check the download status of all the files.
                objBatchProcessor.BatchProcess(m_dtAppExitToday)
                AppContainer.GetInstance().obLogger.WriteAppLog("AppContainer::TimeEventProcessor::Checking whether all the files are downloaded successfully", Logger.LogLevel.RELEASE)
                'If all the reference files are downloaded.
                If m_objConfigParams.DownloadCount >= m_objConfigParams.ReferenceFileCount Then
                    '=====================================================
                    ' IF ALL FILES ARE DOWNLOADED AND UPDATED IN DATABASE 
                    '=====================================================
                    'Register the application to invoke at 12:01am next day.
                    RegisterApp()
                    'Disable the timer.
                    m_tmrAlarm.Enabled = False
                    m_objConfigParams.BuildInProgress = "N"
                    ConfigParser.GetInstance().UpdateConfig()
                    obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Reference file download and parsing completed.", _
                                         Logger.LogLevel.RELEASE)
                    'Set exit flag to exit from MCDownloader application
                    m_bExitFlag = True
                    'Execute the sample query.
                    'ExecuteSampleQuery()
                    obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Disposing the reference file download status form", Logger.LogLevel.RELEASE)
                    'Dispose the reference file download status form.
                    objRefDownloadForm.DisposeForm()
                    objRefDownloadForm = Nothing
                    '=======================================
                    ' INITIALISE AND DISPLAY SUMMARY SCREEN 
                    '=======================================
                    'obLogger.WriteAppLog("AppContainer: Initialising Summary Screen", Logger.LogLevel.RELEASE)
                    'objSummaryScreen = New frmSummaryScreen
                    'obLogger.WriteAppLog("AppContainer: Setting Display time and starting Timer", Logger.LogLevel.RELEASE)
                    'obLogger.WriteAppLog("Starting Timer", Logger.LogLevel.RELEASE)
                    'Display summary screen using showdialog() function.
                    'DisplayScreen(Macros.SUMMARY_SCREEN)
                    'Exit application.
                    'Application.Exit()
                ElseIf m_objConfigParams.DownloadCount < m_objConfigParams.ReferenceFileCount And _
                   m_dtAppExitToday.CompareTo(Now) > 0 Then
                    '=====================================================================
                    ' IF NOT ALL FILES ARE DOWNLOADED AND APPEXIT TIME > CURRENT TIME 
                    '=====================================================================
                    'If all the files are not downloaded.
                    AppContainer.GetInstance().obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Setting Next Invoke Time in case of error", Logger.LogLevel.RELEASE)
                    Dim dtNextInterval As DateTime = m_dtFirstDownloadToday.AddMinutes(m_objConfigParams.DownloadInterval)
                    'Increase the itnerval if the interval time is less than current time and 
                    'also less than the AppExitToday time.
                    While dtNextInterval.CompareTo(Now) < 0 And dtNextInterval < m_dtAppExitToday
                        dtNextInterval = dtNextInterval.AddMinutes(m_objConfigParams.DownloadInterval)
                    End While

                    If dtNextInterval > m_dtAppExitToday Then
                        AppContainer.GetInstance().obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Interval is greater that AppExitToday. Thence exitting application.", _
                                                                        Logger.LogLevel.RELEASE)
                        'Execute the sample query.
                        'ExecuteSampleQuery()
                        'Set exit flag to exit from MCDownloader application
                        m_bExitFlag = True
                    Else
                        'Set the interval and enable the timer.
                        m_tmrAlarm.Interval = IntervalTill(dtNextInterval)
                        m_tmrAlarm.Enabled = True
                        objRefDownloadForm.SetCurrentStatus("Waiting till next download time: " + dtNextInterval)
                    End If
                Else
                    '======================================================================
                    ' IF MCD COMPLETES AFTER APPEXIT TIME OR ALL FILE UPLOADED SUCCESSFULLY
                    '======================================================================
                    'Register the application to invoke at 12:01am next day.
                    RegisterApp()
                    'Disable the timer.
                    m_tmrAlarm.Enabled = False
                    m_objConfigParams.BuildInProgress = "N"
                    ConfigParser.GetInstance().UpdateConfig()
                    obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Reference file download and parsing incomplete.", _
                                         Logger.LogLevel.RELEASE)
                    'Set exit flag to exit from MCDownloader application
                    m_bExitFlag = True

                    obLogger.WriteAppLog("AppContainer::TimeEventProcessor:: Disposing the reference file download Status form", Logger.LogLevel.RELEASE)
                    'Dispose the reference file download status form.
                    objRefDownloadForm.DisposeForm()
                    objRefDownloadForm = Nothing
                    '=======================================
                    ' INITIALISE AND DISPLAY SUMMARY SCREEN 
                    '=======================================
                    'obLogger.WriteAppLog("AppContainer: Initialising Summary Screen", Logger.LogLevel.RELEASE)
                    'objSummaryScreen = New frmSummaryScreen
                    'obLogger.WriteAppLog("AppContainer: Setting Display time and starting Timer", Logger.LogLevel.RELEASE)
                    'obLogger.WriteAppLog("Starting Timer", Logger.LogLevel.RELEASE)
                    'Display summary screen using showdialog() function.
                    'DisplayScreen(Macros.SUMMARY_SCREEN)
                End If
            End If
        Catch ex As Exception
            If Not objRefDownloadForm Is Nothing Then
                objRefDownloadForm.DisposeForm()
            End If
            AppContainer.GetInstance().obLogger.WriteAppLog("TimeEventProcessor:: Exception occured, Message:" + ex.Message + ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
            'Leave unattended mode
            m_objPwrState.SetSleepTimeOut(m_objConfigParams.BattSuspendTimeout)
            AppContainer.GetInstance().obLogger.WriteAppLog("AppContainer::TimeEventProcessor::: Rebooting the device", _
                                                            Logger.LogLevel.ERROR)
            'Restart the device if all file download and db population is completed.
            Restart.GetInstance.ResetPocketPC()
        End Try
    End Sub
    ''' <summary>
    ''' Set invoke time for next day.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RegisterApp()
        Dim dtNextInterval As DateTime
        Dim tySystemTime As SystemTime
        Dim intptrTime As IntPtr
        Try
            'Add one day to firstinvoketoday to set for next day.
            dtNextInterval = m_dtFirstInvokeToday.AddDays(1)

            'convert the invoke time to Unmanages system time struct
            tySystemTime = ConvertDateTime.FromDateTime(dtNextInterval)
            intptrTime = Marshal.AllocHGlobal(Marshal.SizeOf(tySystemTime))
            Marshal.StructureToPtr(tySystemTime, intptrTime, False)

            'register the application to run
            CeRunAppAtTime(m_objConfigParams.AppName, intptrTime)
            obLogger.WriteAppLog("Registered App to run at" & dtNextInterval.ToString(), _
                                 Logger.LogLevel.RELEASE)
        Catch ex As Exception
            obLogger.WriteAppLog("Exception Occured while registering the Application @ " & ex.StackTrace, _
                                 Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' To display the screen.
    ''' </summary>
    ''' <param name="iScreenName"></param>
    ''' <remarks></remarks>
    Private Sub DisplayScreen(ByVal iScreenName As Integer)
        Select Case iScreenName
            Case Macros.SUMMARY_SCREEN
                obLogger.WriteAppLog("Displaying Summary Screen", Logger.LogLevel.RELEASE)
                'Displays Summary Screen
                objSummaryScreen.ShowDialog()
            Case Macros.REF_DOWNLOAD_SCREEN
                obLogger.WriteAppLog("Displaying Ref File Download Status Screen", Logger.LogLevel.RELEASE)
                objRefDownloadForm.Show()
                Threading.Thread.Sleep(20000)
                'objRefDownloadForm.Refresh()
                objRefDownloadForm.SetCurrentStatus("Initialising Upload")
        End Select
    End Sub
    ''' <summary>
    ''' Delete all export data files that are present in the device.
    ''' This is to make sure that the presvious day's export data is not
    ''' present in the device on next day.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteExDataFile()
        Try
            Dim strPath As String = m_objConfigParams.HomeDir.TrimEnd("\") & "\" & "ExportData"
            Dim arrFileEntries As String() = Directory.GetFiles(strPath)
            'Delete the export data files.
            If arrFileEntries.Length > 0 Then
                For Each strFilePath In arrFileEntries
                    File.Delete(strFilePath)
                    obLogger.WriteAppLog("Deleted Export Data files: " & strFilePath, _
                                         Logger.LogLevel.RELEASE)
                Next
            Else
                obLogger.WriteAppLog("Deleting stale export data files.", Logger.LogLevel.RELEASE)
            End If
        Catch ex As Exception
            obLogger.WriteAppLog("AppContainer::DeleteExDataFile::Exception Occured, Message:" + ex.StackTrace, _
                                 Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Change the active IP global variable.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub sChangeActiveIP()
        Try
            If AppContainer.GetInstance.strActiveIP = ConfigParser.GetInstance.GetIPParam _
                                                        (IPParams.PrimaryIP) Then

                AppContainer.GetInstance.strActiveIP = ConfigParser.GetInstance.GetIPParam _
                                                        (IPParams.SecondaryIP)
            Else
                AppContainer.GetInstance.strActiveIP = ConfigParser.GetInstance.GetIPParam _
                                                        (IPParams.PrimaryIP)
            End If
        Catch ex As Exception
            obLogger.WriteAppLog("Error in changing the IP" + ex.ToString(), _
                                 Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Imports CeRunappTime of Coredll to register an application to run at a specific time
    ''' </summary>
    ''' <param name="AppName"></param>
    ''' <param name="ExecTime"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport("CoreDLL.dll")> _
    Private Shared Function CeRunAppAtTime(ByVal AppName As String, ByVal ExecTime As IntPtr) As Boolean
    End Function

End Class
