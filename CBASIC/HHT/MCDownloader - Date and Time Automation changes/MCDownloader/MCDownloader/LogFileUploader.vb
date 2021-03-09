Imports System.IO
Imports System.Runtime.InteropServices
Imports MCDownloader.TFTPClient
Imports System.Threading
Imports Ionic.Zip
''' <summary>
''' Class to handle downloading log files to the controller.
''' </summary>
''' <remarks></remarks>
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Refers to active controller IP address while connecting
''' </Summary>
'''****************************************************************************
Public Class LogFileUploader
    <DllImport("CoreDLL.dll")> _
    Private Shared Function CeRunAppAtTime(ByVal AppName As String, ByVal ExecTime As IntPtr) As Boolean
    End Function
    Private m_objConfigParams As ConfigParams
    Private m_PreviousPutStatus As Integer = 0
    Private Shared m_objTFTPSession As TFTPClient.TFTPSession
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        m_objConfigParams = ConfigParser.GetInstance().GetConfigParams()
        'AppendIPInLogFileName()
    End Sub
    ''' <summary>
    ''' Downloade class to initiate and manage file download.
    ''' </summary>
    ''' <remarks></remarks>
    Private Class Download
        Public tyOptions As New TFTPClient.TransferOptions()
        Public downloadStatus As Status
        Public Sub DownloadLog()
            If m_objTFTPSession.Put(tyOptions) Then
                downloadStatus = Status.Completed
            Else
                downloadStatus = Status.Terminated
            End If
        End Sub
    End Class
    ''' <summary>
    ''' Functions that is responsible for downloading log files.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Start()
        'Stores Log File directory Path
        Dim strLocalPath As String = m_objConfigParams.LocalLogFilePath
        'Path where the log files has to be saved
        Dim strRemotePath As String = m_objConfigParams.RemoteLogFilePath
        Dim strCurrLogFile As String = Nothing
        Try

            'Send a Log File Uploding LOG message Here
            'Downloader LOG CR
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                                LOGTransmitter.Status.START, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Downloading_File)
            'Get IP address and store in a variable.
            ' Dim strIPOctet As String = GetIPAddress().Split(".")(3)
            'AppContainer.GetInstance.objRefDownloadForm.SetTitle(frmDownloadReferenceData.FormTitle.frmLogFileUpload)
            'Check if local path is not equal to nothing.
            If strLocalPath = "" Then
                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Log Directory not found on configuration file", _
                                                                Logger.LogLevel.ERROR)
                Exit Sub
            End If
            AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Initialising Log File Download")
            If Directory.Exists(strLocalPath) Then
                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Starting LogFile Upload", _
                                                                Logger.LogLevel.RELEASE)
                Dim straFileEntries As String() = Directory.GetFiles(strLocalPath)
                ' Process the list of files found in the directory.
                Dim strFileName As String
                Dim objDownloader As New Download
                Dim tmpNum As Integer
                'Get the current log file.
                strCurrLogFile = AppContainer.GetInstance.obLogger.GetLogFileName()

                'Choose end point details
                'v1.1 MCF Changes
                'objDownloader.tyOptions.Host = m_objConfigParams.TFTP.Host
                objDownloader.tyOptions.Host = AppContainer.GetInstance.strActiveIP
                objDownloader.tyOptions.Port = m_objConfigParams.TFTP.Port
                objDownloader.tyOptions.Action = TFTPClient.TransferType.Put
                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Deleting Stale Log Files")
                'Purge all older log files and upload only the log files from immediate previous day
                For Each strFileName In straFileEntries
                    If IsLogFileStale(strFileName) Then
                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Deleting stale file" + strFileName, _
                                                                        Logger.LogLevel.RELEASE)
                        File.Delete(strFileName)
                        Continue For
                    End If
                Next
                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Preparing the Log files for compression")
                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Zipping log files into a single file", _
                                                                       Logger.LogLevel.RELEASE)
                'Get the list of file for downloading to controller.
                'Dim straFileList As String() = Directory.GetFiles(strLocalPath)
                Dim dirinfoLogDirectory As New DirectoryInfo(strLocalPath)
                'Zip File Name 
                Dim strMacAddress As String = Utility.GetSerialNumber()
                Dim strZipFileName As String = strMacAddress.Substring(strMacAddress.Length - 5)
                strZipFileName = strZipFileName + Convert.ToString(Date.Today.Month, 16)
                strZipFileName = strZipFileName + Date.Today.Day.ToString() + ".ZIP"
                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Zip File Name: " + strZipFileName, _
                                                                      Logger.LogLevel.RELEASE)
                Dim strZipFile As String = strLocalPath.TrimEnd("\") + "\" + strZipFileName
                Try
                    Using zippedLogFile As ZipFile = New ZipFile(strZipFile)
                        Dim arrFiles As FileInfo() = dirinfoLogDirectory.GetFiles("*.txt")
                        For Each logFileInfo As FileInfo In arrFiles
                            If Not (logFileInfo.Name.Equals(strCurrLogFile)) Then
                                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Compressing File: " + logFileInfo.Name)
                                zippedLogFile.AddFile(logFileInfo.FullName)
                            End If
                        Next
                        AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Saving Compressed File")
                        zippedLogFile.Save()
                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Log Files Zipped Successfully", _
                                                                      Logger.LogLevel.RELEASE)

                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Initialising - Deleting the log files which are compressed", _
                                                                 Logger.LogLevel.RELEASE)
                        For Each logFileInfo As FileInfo In arrFiles
                            'Delete all the log file and not the zip file
                            If Not logFileInfo.Name.Equals(strCurrLogFile) Then
                                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Log File Named " + logFileInfo.Name + " Deleted", _
                                                                              Logger.LogLevel.RELEASE)
                                logFileInfo.Delete()
                            End If
                        Next
                    End Using
                Catch ex As Exception
                    AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Error while Zipping log files, Message:" + ex.Message, _
                                                                      Logger.LogLevel.RELEASE)
                End Try
                'After Successfully creating Zipped File delete all the logs
                Dim arrZipFiles As String() = Directory.GetFiles(strLocalPath, "*.ZIP")
                'Download all the files one by one to the controller.
                For Each strFile In arrZipFiles
                    Try
                        'upload files one after another
                        'strFilename itself is giving full file path
                        'If Path.GetFileName(strFileName).Equals(strCurrLogFile) Then
                        '    Continue For
                        'End If
                        'Set local file name including path.
                        objDownloader.tyOptions.LocalFilename = strFile
                        'Set remote path to download the file to.
                        If strRemotePath = "" Then
                            objDownloader.tyOptions.RemoteFilename = Path.GetFileName(strFile)
                            'AppendIPInLogFileName(Path.GetFileName(strFileName), strIPOctet)
                        Else
                            'Set the destination path for the file.
                            objDownloader.tyOptions.RemoteFilename = strRemotePath.TrimEnd("\") + "\" + Path.GetFileName(strFile)
                            'AppendIPInLogFileName(Path.GetFileName(strFileName), strIPOctet)
                        End If
                        'Set downloader form message.
                        AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Downloading Compressed File " + _
                            Path.GetFileName(strFile).ToString.Trim("\") + " to the controller.")
                        'Log this to log file.
                        'AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUpLoader:: Start:: Downloading log file " & Path.GetFileName(strZipFileName), _
                        '                                                Logger.LogLevel.RELEASE)
                        m_PreviousPutStatus = 0
                        PacketMonitor.GetInstance.PacketCounter = 0
                        m_objTFTPSession = New TFTPClient.TFTPSession
                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Uploading Log File - " + strFile, _
                                                                        Logger.LogLevel.RELEASE)
                        'Send a LOG message for Log File Uploading
                        'AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, LOGTransmitter.Status.START, LOGTransmitter.FileName.POD_Log_Files)
                        'Initialise and start a thread for downloading the log files.
                        Dim thDownloadThread As New Threading.Thread(AddressOf objDownloader.DownloadLog)
                        thDownloadThread.Start()
                        'Wait before starting the monitor.
                        Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.InitialisingDownloadTime)
                        While True
                            'Check if thread is running and enter the loop
                            If objDownloader.downloadStatus = Status.Running Then
                                tmpNum = PacketMonitor.GetInstance.PacketCounter
                                'Check the packet number against the last saved number.
                                If tmpNum = m_PreviousPutStatus Then
                                    'If download is completed thread status is returned as Completed.
                                    AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Disconnected)
                                    Try
                                        thDownloadThread.Abort()
                                    Catch ex As ThreadAbortException
                                        'Do nothing.
                                    End Try
                                    thDownloadThread = Nothing
                                    'Start downloading next file to controller.
                                    'Continue For
                                Else
                                    m_PreviousPutStatus = tmpNum
                                End If
                            ElseIf objDownloader.downloadStatus = Status.Completed Then
                                'If download is completed thread status is returned as Completed.
                                AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Download_Complete)
                                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Deleting Compressed Log Files")
                                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Deleting Log File: " + strFile, _
                                                                   Logger.LogLevel.RELEASE)
                                'delete the zip file after download
                                If (File.Exists(strFile)) Then
                                    File.Delete(strFile)
                                End If

                                'Setting the current status of the controller
                                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Deleting Compressed Log Files")
                                Try
                                    thDownloadThread.Abort()
                                Catch ex As ThreadAbortException
                                    'Do nothing.
                                End Try
                                thDownloadThread = Nothing
                                'File download completed successfully.
                                'Set downloader form message.
                                AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Downloading log file successfully completed.")
                                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Downloading log file successfully completed.", _
                                                                                Logger.LogLevel.RELEASE)
                                'Exit while loop if file download is completed.
                                Exit While
                            End If
                            'Wait before next check.
                            Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.ConnectionLostCheckTime)
                        End While
                        'Delete the  Compressed log file which was created
                        'If (File.Exists(strAbsolutePath)) Then
                        '    File.Delete(strAbsolutePath)
                        'End If

                    Catch ex As Exception
                        'if any exception occurred during the file transmission
                        'skip to next file name in the list.
                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUpLoader:: Start :: Exception occured, Message: " & ex.Message, _
                                                                        Logger.LogLevel.RELEASE)

                        'Downloader LOG CR
                        'Log File Upload failure
                        AppContainer.GetInstance.objLogMessageTransmitter.sendLog( _
                            LOGTransmitter.Action.TFTP_SEND, LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Download_Fail)
                        'Continue For
                    End Try
                Next strFile
                m_objTFTPSession = Nothing
                ''Set downloader form message.
                'AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Downloading log file successfully completed.")
                'AppContainer.GetInstance().obLogger.WriteAppLog("Downloading log file successfully completed.", _
                '                                                Logger.LogLevel.RELEASE)
            Else
                AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader::Start::Log File Directory does not exist", _
                                                                Logger.LogLevel.ERROR)
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUpLoader:: Start :: Exception Occured, Message: " + _
                                                            ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
            AppContainer.GetInstance.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Other_Errors)
        End Try
    End Sub
    ''' <summary>
    ''' To check for the dynamic IP generated when the device is docked
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetIPAddress() As String
        'Declare the local variable and the get the host name
        Dim sDnsName As String = Nothing
        Dim m_IpHostEntry As System.Net.IPHostEntry = Nothing
        Dim m_aIPAddressArray As System.Net.IPAddress() = Nothing
        Dim strIP As String = ""
        Dim iIndex As Integer = 0
        Dim aIPSubnet() As String = Nothing
        Try
            sDnsName = System.Net.Dns.GetHostName()
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: GetIPAddress:: Getting DNS name" + _
                                                  sDnsName, Logger.LogLevel.INFO)
            m_IpHostEntry = System.Net.Dns.GetHostEntry(sDnsName)
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: GetIPAddress:: Getting IP Host", _
                                                  Logger.LogLevel.INFO)
            m_aIPAddressArray = m_IpHostEntry.AddressList()
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: GetIPAddress:: Getting IP address" + _
                                                  m_IpHostEntry.AddressList(0).ToString(), _
                                                  Logger.LogLevel.INFO)
            ' Check if the address array has a default value
            If m_aIPAddressArray.Length > 0 Then
                ' Check within a loop whether the IP is else then 127.0.0.1
                For iIndex = 0 To m_aIPAddressArray.Length - 1
                    ' If the address IP is else then convert it into string 
                    If m_aIPAddressArray(iIndex).ToString() = "127.0.0.1" And _
                       m_aIPAddressArray.Length = 1 Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: GetIPAddress:: IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    ElseIf m_aIPAddressArray(iIndex).ToString() <> "127.0.0.1" Then
                        strIP = m_aIPAddressArray(iIndex).ToString()
                        AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: GetIPAddress:: IP of device is" _
                                                              & strIP, _
                                                              Logger.LogLevel.RELEASE)
                    End If
                Next
                ' Return the new IP generated when the device is docked into 
                'the(cradle)
                'format the IP address to have 3 digits in all the three subnets
                aIPSubnet = strIP.Split(".")
                aIPSubnet(0) = aIPSubnet(0).PadLeft(3, "0")
                aIPSubnet(1) = aIPSubnet(1).PadLeft(3, "0")
                aIPSubnet(2) = aIPSubnet(2).PadLeft(3, "0")
                aIPSubnet(3) = aIPSubnet(3).PadLeft(3, "0")
                strIP = aIPSubnet(0) & "." & aIPSubnet(1) & "." & _
                           aIPSubnet(2) & "." & aIPSubnet(3)

                'returnt he IP address to the calling function
                Return strIP
            Else
                'Return the default IP of the device when the device is 
                'not docked into the cradle
                Return "127.000.000.001"
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: GetIPAddress:: Device IP retreival failure @ " + _
                                      ex.StackTrace, Logger.LogLevel.RELEASE)
            Return "127.000.000.001"
        End Try
    End Function
    ''' <summary>
    ''' Function to check whether the device is docked or not.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CheckDeviceDocked() As Boolean
        Dim bTemp As Boolean = False
        Try
            Dim iCounter As Integer = AppContainer.GetInstance.m_objConfigParams.IPCheckRetry
            While iCounter > 0
                If GetIPAddress().ToString().Equals("127.000.000.001") Then
                    Threading.Thread.Sleep(AppContainer.GetInstance.m_objConfigParams.IPCheckRetryWaitTime)
                    iCounter = iCounter - 1
                Else
                    bTemp = True
                    Exit While
                End If
            End While
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: CheckDeviceDocked:: Check for device docked failed @ " + _
                                      ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        Return bTemp
    End Function
    ''' <summary>
    ''' Function to remove the file extention and append last subnet of 
    ''' IP address as file extension for all file in a specified directory.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AppendIPInLogFileName()
        Try
            Dim straFileEntries As String() = Directory.GetFiles(m_objConfigParams.LocalLogFilePath)
            Dim strDeviceIP As String = GetIPAddress().Split(".")(3)
            Dim strNewFilePath As String = ""
            For Each strFilePath In straFileEntries
                strNewFilePath = strFilePath.Split(".")(0) & "." & strDeviceIP
                File.Copy(strFilePath, strNewFilePath)
                File.Delete(strFilePath)
            Next
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("LogFileUploader:: AppendIPInLofFileName:: Log File Renaming Failed @ " + _
                                      ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Function to remove the file extention and append last subnet of 
    ''' IP address as file extension for a specified file.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AppendIPInLogFileName(ByVal strFileName As String, ByVal strDeviceIP As String) As String
        Try
            Return strFileName.Split(".")(0) & "." & strDeviceIP
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("LLogFileUploader:: AppendIPInLofFileName:: Log File Renaming Failed @" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return strFileName
        End Try
    End Function
    ''' <summary>
    ''' To identify stale logfiles.
    ''' </summary>
    ''' <param name="strFileName">Logfile name</param>
    ''' <returns>True if the Log file is stale, else false</returns>
    ''' <remarks></remarks>
    Private Function IsLogFileStale(ByVal strFileName As String) As Boolean
        Try
            Dim dtCreationDateTimeStamp As Date = File.GetCreationTime(strFileName)
            
            If strFileName.EndsWith(".ZIP") Then
                Dim dtYesterday As Date = Now.AddDays(-7)
                dtYesterday = New Date(dtYesterday.Year, dtYesterday.Month, _
                                       dtYesterday.Day, 0, 1, 0)
                If dtCreationDateTimeStamp < dtYesterday Then
                    Return True
                Else
                    Return False
                End If
            Else
                Dim dtYesterday As Date = Now.AddDays(-1)
                dtYesterday = New Date(dtYesterday.Year, dtYesterday.Month, _
                                       dtYesterday.Day, 0, 1, 0)
                If dtCreationDateTimeStamp < dtYesterday Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("LofFileUploader:: IsLogFileStale:: Identify Stale Log File Failed" + _
                                                  ex.StackTrace, Logger.LogLevel.RELEASE)
            Return True
        End Try
    End Function
End Class
