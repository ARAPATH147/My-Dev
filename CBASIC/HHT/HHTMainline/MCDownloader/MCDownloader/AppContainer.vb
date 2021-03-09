Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Data
Imports System.Data.SqlServerCe
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
