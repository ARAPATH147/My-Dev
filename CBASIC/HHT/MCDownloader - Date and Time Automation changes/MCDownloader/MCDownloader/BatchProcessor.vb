Imports System.IO
Imports System.Threading
Imports System.Runtime.InteropServices
Imports MCDownloader.TFTPClient
Imports Microsoft.Win32
'''***************************************************************
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
'''***************************************************************
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Checks whether MCF is enabled or not
''' </Summary>
'''****************************************************************************
Public Class BatchProcessor
    <DllImport("CoreDLL.dll")> _
    Private Shared Function CeRunAppAtTime(ByVal AppName As String, ByVal ExecTime As IntPtr) As Boolean
    End Function
   

    Public Const FILE_ACTIVE As String = "E"
    Private m_iPreviousPutStatus As Integer = 0
    Private m_iPreviousGetStatus As Integer = 0
    Private strfileName As String
    Private m_objTFTPSession As New TFTPClient.TFTPSession()
    Private m_tyOptions As New TFTPClient.TransferOptions()
    Private m_objConfigParams As ConfigParams
    Private m_lstFiles As ArrayList
    Private m_objBatchConfigParser As BatchConfigParser
    Private m_FileThreadDownloadStatus As Boolean = False
    Private m_dtBtCodeBuildTime As DateTime
    Private m_bStopEvents As Boolean = False
    Private Shared m_objLock As Object = Nothing
    'v1.1 MCF Change
    Private regActiveServerIP As RegistryKey
    ''' <summary>
    ''' Private class to maintaing the file download status.
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DownloadMonitor
        Public strFileName As String
        Public objDownloadStatus As Status
        Public Sub StartDownload()
            Dim objRefFileDownload As New RefernceFileDownload(strFileName)
            objDownloadStatus = objRefFileDownload.DownloadThread()
        End Sub
    End Class
    ''' <summary>
    ''' Construtor for BatchProcessor.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        m_objConfigParams = ConfigParser.GetInstance().GetConfigParams()
        m_objBatchConfigParser = BatchConfigParser.GetInstance()
        If m_objLock Is Nothing Then
            m_objLock = New Object
        End If
    End Sub
    ''' <summary>
    ''' Get SYNCTRL.DAT file.
    ''' Parse the file and extract the file names that are ready to upload.
    ''' Get the files ready for upload.
    ''' </summary>
    ''' <param name="dtAppExitToday"></param>
    ''' <remarks></remarks>
    Public Sub BatchProcess(ByVal dtAppExitToday As DateTime)
        Try
            While dtAppExitToday.CompareTo(Now) > 0
                AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor::BatchProcess: Get Control file ", Logger.LogLevel.RELEASE)
                GetControlFile()
                AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor::BatchProcess: Parse Control file ", Logger.LogLevel.RELEASE)
                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Parsing Control File")
                ParseControlFile(m_objConfigParams.TFTP.LocalFilePath.TrimEnd("\") + "\" + m_objConfigParams.ControlFile.Name)
                If m_lstFiles.Count = 0 Then
                    Exit While
                Else
                    AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor::BatchProcees: Download all successfully build reference files", Logger.LogLevel.RELEASE)
                    DownloadReferenceFiles()
                End If
                'if download complete for all files execute the sameple query.
                If m_objConfigParams.DownloadCount >= m_objConfigParams.ReferenceFileCount Then
                    '    Dim objRefFileParser As ReferenceFileParser = New ReferenceFileParser()
                    '    objRefFileParser.ExecuteSampleQuery()
                    '    objRefFileParser.Terminate()
                    '    AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor: Executing sample query completed.", Logger.LogLevel.RELEASE)
                    Exit While
                End If
            End While
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:: Batch Process:: Exception Occured, Message:" + ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Get SYNCTRL.DAT file from controller via TFTP.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetControlFile()
        'Getting Control File starts here 
        Try
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.START, _
                                                                      LOGTransmitter.FileName.SYNCTRL, _
                                                                      LOGTransmitter.Reasons.Downloading_File)
            AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Downloading Control File")
            AppContainer.GetInstance().obLogger.WriteAppLog("Downloading control file ", _
                                                            Logger.LogLevel.RELEASE)

            Dim objMonitor As New DownloadMonitor
            'Start populating the database if file download is completed successfully.
            AppContainer.GetInstance().obLogger.WriteAppLog("Downloading started for the file " & _
                                                            m_objConfigParams.ControlFile.Name, _
                                                            Logger.LogLevel.RELEASE)
            objMonitor.strFileName = m_objConfigParams.ControlFile.Name
            'set the running Status
            objMonitor.objDownloadStatus = Status.Running
            'start file download thread.
            Dim thDownloadThread As New System.Threading.Thread(AddressOf objMonitor.StartDownload)
            'Setting thread to run
            Dim tmpNum As Integer
            m_iPreviousGetStatus = 0
            PacketMonitor.GetInstance.PacketCounter = 0

            'Start Running the thread
            thDownloadThread.Start()
            'Set status to the status form displayed
            AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus _
               ("The File " + m_objConfigParams.ControlFile.Name.Trim("\").ToString + " is being uploaded...")
            'Checking the status
            Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.InitialisingDownloadTime)
            While True
                'If the download thread is running currently then status returned is running.
                If objMonitor.objDownloadStatus = Status.Running Then
                    'Get the current packet number.
                    tmpNum = PacketMonitor.GetInstance().PacketCounter
                    'Check the current packet number against the previous.
                    If (tmpNum = m_iPreviousGetStatus) Then
                        'Displaying Error COntent since the updation doesn't procees
                        ''display error in UI
                         'v1.1 Start - MCF Changes
                        If AppContainer.GetInstance.bMCFEnabled Then
                            AppContainer.GetInstance.sChangeActiveIP()
                            ConfigParser.GetInstance.SetActiveIP()
                            'Change the value in Airbeam registry in order to avoid the HHT getting 
                            'stuck during startup
                            regActiveServerIP = Registry.LocalMachine.OpenSubKey("SOFTWARE\AIRBEAM", True)
                            regActiveServerIP.SetValue("SERVERIP", AppContainer.GetInstance.strActiveIP)
                            AppContainer.GetInstance().obLogger.WriteAppLog("Change the IP to " & _
                                                AppContainer.GetInstance.strActiveIP, _
                                                Logger.LogLevel.RELEASE)
                        End If
                        'v1.1 End - MCF Changes
                        Dim iTime As Integer
                        iTime = AppContainer.GetInstance.m_objConfigParams.ConnectionLostRestartTime
                        iTime = iTime / 1000
                        AppContainer.GetInstance.objRefDownloadForm.DisplayErrorLabel(iTime)
                        iTime = iTime - 1
                        While iTime <> 0
                            Thread.Sleep(950)
                            AppContainer.GetInstance.objRefDownloadForm.ChangeTime(iTime)
                            iTime = iTime - 1
                        End While
                        'Leave unattended mode
                        Dim objPwrState As PowerState = New PowerState()
                        objPwrState.SetSleepTimeOut(m_objConfigParams.BattSuspendTimeout)
                        AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:: DownloadReferenceFiles:: TimerEvent - Restartign the device", _
                                                                        Logger.LogLevel.ERROR)

                        'Disconnected. Sending a LOG message with Disconnected status
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                  LOGTransmitter.Status.ABEND, _
                                                                                  LOGTransmitter.FileName.SYNCTRL, _
                                                                                  LOGTransmitter.Reasons.Disconnected)
                        'Restart the device in case of connection lost.
                        Restart.GetInstance.ResetPocketPC()
                    Else
                        m_iPreviousGetStatus = tmpNum
                    End If
                    'Wait before next check on the packet status.
                    Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.ConnectionLostCheckTime)
                ElseIf objMonitor.objDownloadStatus = Status.Completed Then
                    'If download is completed thread status is returned as Completed.
                    AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                              LOGTransmitter.Status.END_OK, _
                                                                              LOGTransmitter.FileName.SYNCTRL, _
                                                                              LOGTransmitter.Reasons.Download_Complete)
                    Try
                        thDownloadThread.Abort()
                    Catch ex As ThreadAbortException
                        'Do nothing.
                    End Try
                    thDownloadThread = Nothing
                    objMonitor = Nothing
                    'File download completed successfully.
                    Exit While
                End If
            End While
            'Start populating the database if file download is completed successfully.
            AppContainer.GetInstance().obLogger.WriteAppLog("Downloading of" & m_objConfigParams.ControlFile.Name & " file Completed", _
                                                            Logger.LogLevel.RELEASE)
        Catch ex As Exception
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Download_Fail)
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor::GetControlFile:: Exception Occured, Message: " + ex.Message, Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Function to donaload the reference files.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DownloadReferenceFiles()
        Try
            AppContainer.GetInstance().obLogger.WriteAppLog("Starting download of Reference files", Logger.LogLevel.RELEASE)
            Dim objDBPopulate As New DBPopulate()
            'Download files one by one and then populate the same to database.
            For Each obj As FileDetails In m_lstFiles
                If obj.strBuildStatus = FILE_ACTIVE And Not CheckDownloadStatus(obj) Then
                    'Sending a LOG Message 
                    AppContainer.GetInstance().objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                              LOGTransmitter.Status.START, _
                                                                              obj.strFileName, _
                                                                              LOGTransmitter.Reasons.Downloading_File)
                    Dim objMonitor As New DownloadMonitor
                    'Start populating the database if file download is completed successfully.
                    AppContainer.GetInstance().obLogger.WriteAppLog("Downloading started for the file " & obj.strFileName, _
                                                                    Logger.LogLevel.RELEASE)
                    objMonitor.strFileName = obj.strFileName
                    'set the running Status
                    objMonitor.objDownloadStatus = Status.Running
                    'start file download thread.
                    Dim thDownloadThread As New System.Threading.Thread(AddressOf objMonitor.StartDownload)
                    'Setting thread to run
                    Dim tmpNum As Integer
                    m_iPreviousGetStatus = 0
                    PacketMonitor.GetInstance.PacketCounter = 0

                    'Start Running the thread
                    thDownloadThread.Start()
                    'Set status to the status form displayed
                    AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus _
                       ("The File " + obj.strFileName.Trim("\").ToString + _
                        " is being uploaded...")
                    'Checking the status
                    Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.InitialisingDownloadTime)
                    While True
                        'If the download thread is running currently then status returned is running.
                        If objMonitor.objDownloadStatus = Status.Running Then
                            'Get the current packet number.
                            tmpNum = PacketMonitor.GetInstance().PacketCounter
                            'Check the current packet number against the previous.
                            If (tmpNum = m_iPreviousGetStatus) Then
                                'v1.1 Start - MCF Changes
                                If AppContainer.GetInstance.bMCFEnabled Then
                                    AppContainer.GetInstance.sChangeActiveIP()
                                    ConfigParser.GetInstance.SetActiveIP()
                                    'Change the value in Airbeam registry in order to avoid the HHT getting 
                                    'stuck during startup
                                    regActiveServerIP = Registry.LocalMachine.OpenSubKey("SOFTWARE\AIRBEAM", True)
                                    regActiveServerIP.SetValue("SERVERIP", AppContainer.GetInstance.strActiveIP)
                                    AppContainer.GetInstance().obLogger.WriteAppLog("Change the IP to " & _
                                                AppContainer.GetInstance.strActiveIP, _
                                                Logger.LogLevel.RELEASE)
                                End If
                                'v1.1 End - MCF Changes
                                'Displaying Error COntent since the updation doesn't procees
                                ''display error in UI
                                Dim iTime As Integer
                                iTime = AppContainer.GetInstance.m_objConfigParams.ConnectionLostRestartTime
                                iTime = iTime / 1000
                                AppContainer.GetInstance.objRefDownloadForm.DisplayErrorLabel(iTime)
                                iTime = iTime - 1
                                While iTime <> 0
                                    Thread.Sleep(950)
                                    AppContainer.GetInstance.objRefDownloadForm.ChangeTime(iTime)
                                    iTime = iTime - 1
                                End While
                                'Leave unattended mode
                                Dim objPwrState As PowerState = New PowerState()
                                objPwrState.SetSleepTimeOut(m_objConfigParams.BattSuspendTimeout)
                                AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:: DownloadReferenceFiles:: TimerEvent - Restartign the device", _
                                                                                Logger.LogLevel.ERROR)

                                'Disconnected. Sending a LOG message with Disconnected status
                                AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                          LOGTransmitter.Status.ABEND, _
                                                                                          obj.strFileName, _
                                                                                          LOGTransmitter.Reasons.Disconnected)
                                'Restart the device in case of connection lost.
                                Restart.GetInstance.ResetPocketPC()
                            Else
                                m_iPreviousGetStatus = tmpNum
                            End If
                            'Wait before next check on the packet status.
                            Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.ConnectionLostCheckTime)
                        ElseIf objMonitor.objDownloadStatus = Status.Completed Then
                            'If download is completed thread status is returned as Completed.
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                      LOGTransmitter.Status.END_OK, _
                                                                                      obj.strFileName, _
                                                                                      LOGTransmitter.Reasons.Download_Complete)
                            Try
                                thDownloadThread.Abort()
                            Catch ex As ThreadAbortException
                                'Do nothing.
                            End Try
                            thDownloadThread = Nothing
                            objMonitor = Nothing
                            'File download completed successfully.
                            Exit While
                        End If
                    End While
                    'Start populating the database if file download is completed successfully.
                    AppContainer.GetInstance().obLogger.WriteAppLog("Downloading of" & obj.strFileName & " file Completed", _
                                                                    Logger.LogLevel.RELEASE)
                    'Update the doanload status and populate the Database.
                    m_objBatchConfigParser.UpdateParams()
                    AppContainer.GetInstance().obLogger.WriteAppLog("Parsing started for the file " & obj.strFileName, _
                                                                Logger.LogLevel.RELEASE)
                    If objDBPopulate.Populate(obj.strFileName) Then
                        'Update download count to config data
                        m_objConfigParams.DownloadCount += 1
                        'Update the file details to config file.
                        UpdateBuildTime(obj)
                        ConfigParser.GetInstance().UpdateConfig()
                        AppContainer.GetInstance().obLogger.WriteAppLog("Parsing completed for the file " & obj.strFileName, _
                                                                        Logger.LogLevel.RELEASE)
                    End If
                End If
            Next
            AppContainer.GetInstance().obLogger.WriteAppLog("Downloading reference files completed.", _
                                                            Logger.LogLevel.RELEASE)
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor::DownloadReferenceFiles:: Exception Occured, Message:" + ex.Message, Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Check if the file is already downloaded to the device.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsFileAlreadyDownloaded(ByVal strFileName As String) As Boolean
        Try
            Return m_objBatchConfigParser.IsFileAlreadyDownloaded(strFileName)
            AppContainer.GetInstance().obLogger.WriteAppLog("Checking if file already downloaded", _
                                                            Logger.LogLevel.ERROR)
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("Checking if file already downloaded failed", _
                                                          Logger.LogLevel.ERROR)
        End Try
    End Function
    ''' <summary>
    ''' Parse SYNCTRL.DAT file and get the files that are build and ready.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParseControlFile(ByVal strFileName As String) As Boolean
        Dim srCtrlFile As StreamReader = Nothing
        Dim bFirstLine As Boolean = True
        Dim strLine As String = Nothing
        Dim bReturn As Boolean = False
        Dim bBOOTCODE As Boolean = True
        Try
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.START, _
                                                                      LOGTransmitter.FileName.SYNCTRL, _
                                                                      LOGTransmitter.Reasons.Parse_File)
            srCtrlFile = New StreamReader(strFileName)
            m_lstFiles = New ArrayList()
            'read line by line and process the file details
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile::Parsing Control File started", Logger.LogLevel.RELEASE)
            Do While srCtrlFile.Peek > -1
                strLine = srCtrlFile.ReadLine()
                If strLine.Trim().StartsWith(".") Then
                    Continue Do
                End If
                ' get file name and status
                Dim objFile As New FileDetails()
                Dim strDate As String = ""
                'Get file name from the line read.
                objFile.strFileName = strLine.Substring(m_objConfigParams.ControlFile.FileNameField.StartIndex, _
                                                        m_objConfigParams.ControlFile.FileNameField.Length).TrimEnd(" ")
                'Get file build status from the line read.
                objFile.strBuildStatus = strLine.Substring(m_objConfigParams.ControlFile.BuildStatus.StartIndex, _
                                                           m_objConfigParams.ControlFile.BuildStatus.Length)
                'Get file last build date from the line read.
                strDate = strLine.Substring(m_objConfigParams.ControlFile.LastBuild.StartIndex, _
                                                           m_objConfigParams.ControlFile.LastBuild.Length)
                If Val(strDate) = 0 Then
                    Continue Do
                End If
                'Check if the file is BOOTCODE.CSV
                If bBOOTCODE And objFile.strFileName = m_objConfigParams.BOOTCODE Then
                    bBOOTCODE = False
                    m_dtBtCodeBuildTime = m_objConfigParams.LastBuildBOOTCODE
                    Dim dtLastBuildTime As New DateTime(Convert.ToInt32(strDate.Substring(0, 4)), _
                                                        Convert.ToInt32(strDate.Substring(4, 2)), _
                                                        Convert.ToInt32(strDate.Substring(6, 2)), _
                                                        Convert.ToInt32(strDate.Substring(8, 2)), _
                                                        Convert.ToInt32(strDate.Substring(10, 2)), _
                                                        Convert.ToInt32(strDate.Substring(12, 2)))
                    objFile.dtLastBuild = dtLastBuildTime
                    'If last build date time is same as that in SYNCTRL.DAT file
                    'Updated the condition for handling the scenario where BOOTCODE.CSV file build status
                    'is still in 'A' i.e., the build is still active at 00:20AM.
                    If dtLastBuildTime.CompareTo(m_objConfigParams.LastBuildBOOTCODE) = 0 _
                        AndAlso objFile.strBuildStatus = "E" Then
                        If m_objConfigParams.BuildInProgress = "Y" Or CheckForPendingFileDownload(dtLastBuildTime) Then
                            'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, _
                            '                                      LOGTransmitter.FileName.BOOTCODE, LOGTransmitter.Reasons.File_Build_Incomplete)
                            Continue Do
                        Else
                            srCtrlFile.Close()
                            'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                            '                                                          LOGTransmitter.Status.ABEND, _
                            '                                                          LOGTransmitter.FileName.BOOTCODE, _
                            '                                                          LOGTransmitter.Reasons.File_Build_Incomplete)
                            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                      LOGTransmitter.Status.END_OK, _
                                                                                      LOGTransmitter.FileName.SYNCTRL, _
                                                                                      LOGTransmitter.Reasons.Parse_Complete)
                            Return False
                        End If
                    ElseIf objFile.strBuildStatus = "E" Then
                        'If last build date time is different from SYNCTRL.DAT file then, this is
                        'the first time BOOTCODE.CSV is downloaded for the day. So, delete the
                        'Database file and create a copy from the existing template DB.
                        AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Deleting the Database...")
                        DeleteDBandBathcProcess()
                        m_objConfigParams.BuildInProgress = "Y"
                        m_objConfigParams.DownloadCount = 0
                        ConfigParser.GetInstance().UpdateConfig()
                        m_dtBtCodeBuildTime = dtLastBuildTime
                        m_lstFiles.Add(objFile)
                        AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile::File " & _
                                                                        objFile.strFileName & " is added to download list.", _
                                                                        Logger.LogLevel.RELEASE)
                        'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                        '                                                          LOGTransmitter.Status.INFO, _
                        '                                                          objFile.strFileName, _
                        '                                                          LOGTransmitter.Reasons.Build_Ready)
                    Else
                        'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                        '                                                          LOGTransmitter.Status.INFO, _
                        '                                                          LOGTransmitter.FileName.BOOTCODE, _
                        '                                                          LOGTransmitter.Reasons.File_Build_Incomplete)
                        AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile::File " & objFile.strFileName & "file is not ready. " _
                                                                    & "Current status is " & objFile.strBuildStatus, Logger.LogLevel.RELEASE)
                        Exit Do
                    End If
                ElseIf strLine.Substring(35, 1).Equals("R") AndAlso objFile.strBuildStatus = "E" Then
                    Dim dtLastBuildTime As New DateTime(Convert.ToInt32(strDate.Substring(0, 4)), _
                                                        Convert.ToInt32(strDate.Substring(4, 2)), _
                                                        Convert.ToInt32(strDate.Substring(6, 2)), _
                                                        Convert.ToInt32(strDate.Substring(8, 2)), _
                                                        Convert.ToInt32(strDate.Substring(10, 2)), _
                                                        Convert.ToInt32(strDate.Substring(12, 2)))
                    objFile.dtLastBuild = dtLastBuildTime
                    'For all reference files check if the build time is greater than
                    'BOOTCODE.CSV build time, check if previously downloaded and add to 
                    'download list.
                    If dtLastBuildTime.CompareTo(m_dtBtCodeBuildTime) > 0 AndAlso _
                        Not CheckDownloadStatus(objFile) Then
                        m_lstFiles.Add(objFile)
                        'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                        '                                                          LOGTransmitter.Status.INFO, _
                        '                                                          objFile.strFileName, _
                        '                                                          LOGTransmitter.Reasons.Build_Ready)
                        AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile::File " & objFile.strFileName & " is added to download list.", _
                                                                        Logger.LogLevel.RELEASE)
                    ElseIf dtLastBuildTime.CompareTo(m_dtBtCodeBuildTime) < 0 Then
                        'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                        '                                                          LOGTransmitter.Status.INFO, _
                        '                                                          objFile.strFileName, _
                        '                                                          LOGTransmitter.Reasons.File_Build_Incomplete)

                        AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile::File " & objFile.strFileName & " is not ready. " _
                                                                                   & "Current status is " & objFile.strBuildStatus, Logger.LogLevel.RELEASE)
                    End If
                Else
                    Continue Do
                End If
            Loop
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile::End of parsing process", _
                                                            Logger.LogLevel.RELEASE)
            srCtrlFile.Close()
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.END_OK, _
                                                                      LOGTransmitter.FileName.SYNCTRL, _
                                                                      LOGTransmitter.Reasons.Parse_Complete)
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:ParseControlFile:: Exception Occured, Message:" _
                                                            + ex.Message, Logger.LogLevel.ERROR)
            srCtrlFile.Close()
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                      LOGTransmitter.Status.ABEND, _
                                                                      LOGTransmitter.FileName.SYNCTRL, _
                                                                      LOGTransmitter.Reasons.Other_Errors)
        End Try
    End Function
    ''' <summary>
    ''' Function to check for any pending file to be downloaded from controller.
    ''' </summary>
    ''' <param name="dtBuildTimeBootCode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckForPendingFileDownload(ByVal dtBuildTimeBootCode As DateTime) As Boolean
        If dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildBARCODE) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildBARCODE) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildSUPPLIER) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildUSERS) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildRECALL) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildLIVEPOG) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildDEAL) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildCATEGORY) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildSHELFDES) > 0 Then
            Return True
        ElseIf dtBuildTimeBootCode.CompareTo(m_objConfigParams.LastBuildPOGMODULE) > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Function to update build time for a file after it is downloaded and 
    ''' updated in the database.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <remarks></remarks>
    Public Sub UpdateBuildTime(ByVal obj As FileDetails)
        AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus _
        ("Updating the build time for the reference files")
        Select Case obj.strFileName
            Case m_objConfigParams.BOOTCODE
                m_objConfigParams.LastBuildBOOTCODE = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.PGROUP
                m_objConfigParams.LastBuildPGROUP = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.SUPPLIER
                m_objConfigParams.LastBuildSUPPLIER = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.USERS
                m_objConfigParams.LastBuildUSERS = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.RECALL
                m_objConfigParams.LastBuildRECALL = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.LIVEPOG
                m_objConfigParams.LastBuildLIVEPOG = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.BARCODE
                m_objConfigParams.LastBuildBARCODE = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.DEAL
                m_objConfigParams.LastBuildDEAL = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.CATEGORY
                m_objConfigParams.LastBuildCATEGORY = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.SHELFDES
                m_objConfigParams.LastBuildSHELFDES = obj.dtLastBuild
                Exit Select
            Case m_objConfigParams.POGMODULE
                m_objConfigParams.LastBuildPOGMODULE = obj.dtLastBuild
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub
    ''' <summary>
    ''' Function to check the download status of a file.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDownloadStatus(ByVal obj As FileDetails) As Boolean
        Select Case obj.strFileName
            Case m_objConfigParams.BOOTCODE
                If m_objConfigParams.LastBuildBOOTCODE.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.PGROUP
                If m_objConfigParams.LastBuildPGROUP.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.SUPPLIER

                If m_objConfigParams.LastBuildSUPPLIER.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.USERS
                If m_objConfigParams.LastBuildUSERS.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.RECALL
                If m_objConfigParams.LastBuildRECALL.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.LIVEPOG
                If m_objConfigParams.LastBuildLIVEPOG.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.BARCODE
                If m_objConfigParams.LastBuildBARCODE.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.DEAL
                If m_objConfigParams.LastBuildDEAL.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.CATEGORY
                If m_objConfigParams.LastBuildCATEGORY.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.SHELFDES
                If m_objConfigParams.LastBuildSHELFDES.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case m_objConfigParams.POGMODULE
                If m_objConfigParams.LastBuildPOGMODULE.CompareTo(obj.dtLastBuild) = 0 Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                Exit Select
        End Select
    End Function
    ''' <summary>
    ''' Function to delete the database if present and the BatchProcess.xml file when 
    ''' the application is invoked for the first time for that day.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteDBandBathcProcess() As Boolean
        Dim strStatusFilename As String = Nothing
        Dim bReturn As Boolean = True
        Try
            If File.Exists(m_objConfigParams.TemplatDB) Then
                If File.Exists(m_objConfigParams.DB) Then
                    'delete the data base.
                    File.Delete(m_objConfigParams.DB)
                    AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:DeleteDBandBathcProcess::Deleted the Device Database", _
                                                           Logger.LogLevel.RELEASE)

                End If
                File.Copy(m_objConfigParams.TemplatDB, m_objConfigParams.DB)
                AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:DeleteDBandBathcProcess::Created New Database", _
                                                         Logger.LogLevel.RELEASE)
            Else
                AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:DeleteDBandBathcProcess::Error - TemplateDB not found", Logger.LogLevel.ERROR)
                'if template db not found in the folder.
                bReturn = False
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("BatchProcessor:DeleteDBandBathcProcess::Replacing database file failed" _
                                                            + ex.Message, Logger.LogLevel.ERROR)
            bReturn = False
        End Try
        m_objBatchConfigParser.PurgeFile()
        Return bReturn
    End Function
End Class
''' <summary>
''' Enum to hold the thread status.
''' </summary>
''' <remarks></remarks>
Public Enum Status
    Running
    Completed
    Terminated
End Enum