'******************************************************************************
' <FileName>AppContainer.vb</FileName>
' <summary>
' The Main application container class which will intialise all the applciation 
' parameters.
' </summary>
' <Version>1.0</Version>
' <Author>Infosys Technologies Ltd.</Author>
' <DateModified>21-Nov-2008</DateModified>
' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform>
' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight> 
'******************************************************************************
'''
''' * Modification Log
''' 
'''****************************************************************************
'''* 1.1    Archana Chandramathi    Jan 2013    
''' <Summary>
''' MCF changes - Check added to verify whether MCF is enabled or not
''' </Summary>
'''****************************************************************************
#If NRF Then
Imports System.IO
Imports McUtilities.TFTPClient
Imports System.Threading
Imports System.Net.Sockets
Public Class BatchProcessor
    Public Const FILE_ACTIVE As String = "E"
    Private strfileName As String
    ''' <summary>
    ''' Object of TFTPSession class to download files from controller.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_objTFTPSession As New TFTPClient.TFTPSession()
    Private m_tyOptions As New TFTPClient.TransferOptions()
    ''' <summary>
    ''' Interger variable to hold data packaet count. Increments evertime 
    ''' a packet is received while downloading a file.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_iPreviousGetStatus As Integer = 0
    ''' <summary>
    ''' Array list to hold the list of reference files to be downloaded.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_lstFiles As ArrayList
    ''' <summary>
    ''' Object of BatchConfigConfigParser to work with configuration file.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_objBatchConfigParser As BatchConfigParser
    ''' <summary>
    ''' Lock object to be used while writing to files.
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared m_objLock As Object = Nothing
    ''' <summary>
    ''' Private class to track the file download status.
    ''' </summary>
    ''' <remarks></remarks>
    Private Class DownloadMonitor
        Private m_RefFileDownload As RefernceFileDownload = Nothing
        Public strFileName As String
        Public objDownloadStatus As Status
        Public Sub New(ByVal strFileName As String)
            m_RefFileDownload = New RefernceFileDownload(strFileName)
        End Sub
        Public Sub StartDownload()
            objDownloadStatus = m_RefFileDownload.DownloadFile()
        End Sub
        Public Function KillDownloadThread() As Boolean
            m_RefFileDownload.KillDownloadThread()
        End Function
    End Class
    ''' <summary>
    ''' Constructor for Batch Processor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        m_objBatchConfigParser = BatchConfigParser.GetInstance()
        m_lstFiles = New ArrayList()
        If m_objLock Is Nothing Then
            m_objLock = New Object
        End If
    End Sub
    ''' <summary>
    ''' Batch Process function to doanload all files.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub BatchProcess()
        objAppContainer.objLogger.WriteAppLog("Entered BatchProcess of BatchConfigProcess", Logger.LogLevel.INFO)
        Try
            GetControlFile()
            ParseControlFile(objAppContainer.objConfigFileParams.TFTP.LocalFilePath.TrimEnd("\") + "\" + objAppContainer.objConfigFileParams.ControlFile.Name)
            DownloadReferenceFiles()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting BatchProcess of BatchConfigProcess", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Pull the SYNCTRL.DAT file from the controller to check the reference file(s)'s status.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetControlFile()
        objAppContainer.objLogger.WriteAppLog("Entered GetControlFile of BatchConfigProcess", Logger.LogLevel.INFO)
        Try
            'm_tyOptions.Host = objAppContainer.objConfigFileParams.TFTP.Host
            m_tyOptions.Host = objAppContainer.strActiveIP
            m_tyOptions.Port = objAppContainer.objConfigFileParams.TFTP.Port
            m_tyOptions.Action = TFTPClient.TransferType.Get
            'v1.1 MCF Change: To check whether the IP is active or not and send request
            If objAppContainer.bMCFEnabled Then
                Dim m_Socket As TcpClient
                m_Socket = New TcpClient()
                Dim bConnectAlt As Boolean = False
                Try
                    m_Socket.Connect(m_tyOptions.Host, m_tyOptions.Port)
                Catch ex As Exception
                    bConnectAlt = True
                End Try

                If Not m_Socket.Client.Connected Then
                    bConnectAlt = True
                End If

                If bConnectAlt Then
                    If objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then
                        objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                    Else
                        objAppContainer.strActiveIP = ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                    End If
                    ConfigDataMgr.GetInstance.SetActiveIP()
                    m_tyOptions.Host = objAppContainer.strActiveIP
                    objAppContainer.objLogger.WriteAppLog("Getting SYNCTRL.DAT file from Alternate Controller", Logger.LogLevel.RELEASE)
                End If
            End If
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.START, LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Downloading_File)
            If objAppContainer.objConfigFileParams.TFTP.RemoteFilePath = "" Then
                m_tyOptions.RemoteFilename = objAppContainer.objConfigFileParams.ControlFile.Name
            Else
                m_tyOptions.RemoteFilename = objAppContainer.objConfigFileParams.TFTP.RemoteFilePath.TrimEnd("\") + "\" + objAppContainer.objConfigFileParams.ControlFile.Name
            End If
            'file params are set
            m_tyOptions.LocalFilename = objAppContainer.objConfigFileParams.TFTP.LocalFilePath.TrimEnd("\") + "\" + objAppContainer.objConfigFileParams.ControlFile.Name
            'Send a LOG MEssage Here 
            'file downloaded now
            m_objTFTPSession.Get(m_tyOptions)
            'gets the file names and the status on to an arraylist
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Download_Complete)
        Catch ex As Exception
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Download_Fail)
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Unable to get control file", "Error", MessageBoxButtons.OK, _
                            MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting GetControlFile of BatchConfigProcess", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Parse SYNCTRL.DAT file pulled from controller and get the list of files
    ''' ready to download from controller.
    ''' </summary>
    ''' <param name="strFileName"></param>
    ''' <remarks></remarks>
    Private Sub ParseControlFile(ByVal strFileName As String)
        objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.START, LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Parse_File)
        objAppContainer.objLogger.WriteAppLog("Entered ParseControlFile of BatchConfigProcess", Logger.LogLevel.INFO)
        Dim srCtrlFile As StreamReader = Nothing
        Try
            srCtrlFile = New StreamReader(strFileName)
            Dim bFirstLine As Boolean = True
            Dim strLine As String = Nothing

            'read line by line and process the file details
            Do While srCtrlFile.Peek > -1
                strLine = srCtrlFile.ReadLine()
                Dim strDate As String = ""
                If strLine.StartsWith(".") Then
                    Continue Do
                ElseIf strLine.Substring(35, 1).Equals("R") And _
                   strLine.Substring(objAppContainer.objConfigFileParams.ControlFile.BuildStatus.StartIndex, _
                                     objAppContainer.objConfigFileParams.ControlFile.BuildStatus.Length).Equals("E") Then
                    'arrList.Add(sr.ReadLine())
                    Dim objFile As New FileDetails()
                    ' get file name and status
                    objFile.strFileName = strLine.Substring(objAppContainer.objConfigFileParams.ControlFile.FileNameField.StartIndex, _
                                                            objAppContainer.objConfigFileParams.ControlFile.FileNameField.Length).TrimEnd(" ")
                    objFile.strBuildStatus = strLine.Substring(objAppContainer.objConfigFileParams.ControlFile.BuildStatus.StartIndex, _
                                                               objAppContainer.objConfigFileParams.ControlFile.BuildStatus.Length)
                    'Get file last build date from the line read.
                    strDate = strLine.Substring(objAppContainer.objConfigFileParams.ControlFile.LastBuild.StartIndex, _
                                                objAppContainer.objConfigFileParams.ControlFile.LastBuild.Length)
                    objFile.dtLastBuild = New DateTime(Convert.ToInt32(strDate.Substring(0, 4)), _
                                                        Convert.ToInt32(strDate.Substring(4, 2)), _
                                                        Convert.ToInt32(strDate.Substring(6, 2)), _
                                                        Convert.ToInt32(strDate.Substring(8, 2)), _
                                                        Convert.ToInt32(strDate.Substring(10, 2)), _
                                                        Convert.ToInt32(strDate.Substring(12, 2)))
                    m_lstFiles.Add(objFile)
                End If
            Loop
            srCtrlFile.Close()
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
            LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Parse_Complete)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            srCtrlFile.Close()
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, LOGTransmitter.Status.ABEND, _
                LOGTransmitter.FileName.SYNCTRL, LOGTransmitter.Reasons.Other_Errors)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting ParseControlFile of BatchConfigProcess", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Function to download the reference file that are ready.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DownloadReferenceFiles(Optional ByVal bPurgeExitingData As Boolean = True)
        Try
            Dim objDBPopulate As New DBPopulate()
            Dim bIsFileDownloaded As Boolean = False
            Dim iIndex As Integer = 0
            Dim bFlag As Boolean = False
            'Download files one by one and then populate the same to database.
            While iIndex < m_lstFiles.Count
                Dim obj As FileDetails
                obj = m_lstFiles(iIndex)
                'For Each obj As FileDetails In m_lstFiles
                If (Not (obj Is Nothing)) And (obj.strBuildStatus = FILE_ACTIVE) Then
                    'Sending a LOG Message 
                    objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                     LOGTransmitter.Status.START, _
                                                                     obj.strFileName, _
                                                                     LOGTransmitter.Reasons.Downloading_File)
                    Dim objMonitor As New DownloadMonitor(obj.strFileName)
                    bIsFileDownloaded = False
                    'Start populating the database if file download is completed successfully.
                    objAppContainer.objLogger.WriteAppLog("BatchProcessor:Downloading file " & obj.strFileName, _
                                                                    Logger.LogLevel.RELEASE)
                    objMonitor.strFileName = obj.strFileName
                    'set the running Status
                    objMonitor.objDownloadStatus = Status.Running
                    'start file download thread.
                    Dim thDownloadThread As New Thread(AddressOf objMonitor.StartDownload)
                    'Setting thread to run
                    Dim tmpNum As Integer
                    m_iPreviousGetStatus = 0
                    PacketMonitor.GetInstance.PacketCounter = 0

                    'Start Running the thread
                    thDownloadThread.Start()
                    'Set status to the status form displayed
                    ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
                       ("The File " + obj.strFileName.Trim("\").ToString + _
                        " is being uploaded...")
                    'Checking the status
                    Threading.Thread.Sleep(objAppContainer.objConfigFileParams.InitialisingDownloadTime)
                    While True
                        'If the download thread is running currently then status returned is running.
                        If objMonitor.objDownloadStatus = Status.Running Then
                            'Get the current packet number.
                            tmpNum = PacketMonitor.GetInstance().PacketCounter
                            'Check the current packet number against the previous.
                            If (tmpNum = m_iPreviousGetStatus) Then
                                'v1.1 MCF Change
                                If Not objAppContainer.bConnectedToAlternate And objAppContainer.bMCFEnabled Then
                                    If objAppContainer.strActiveIP = _
                                              ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString() Then

                                        objAppContainer.strActiveIP = _
                                                        ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.SECONDARY_IPADDRESS).ToString()
                                    Else
                                        objAppContainer.strActiveIP = _
                                                        ConfigDataMgr.GetInstance.GetIPParam(ConfigKey.PRIMARY_IPADDRESS).ToString()
                                    End If
                                    ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
                                    ("Error connecting with Active Controller Trying Alternate Controller ...")
                                    ConfigDataMgr.GetInstance.SetActiveIP()
                                    objAppContainer.bConnectedToAlternate = True
                                    bFlag = True
                                    objAppContainer.objLogger.WriteAppLog("Change the IP to " & _
                                                        objAppContainer.strActiveIP, _
                                                        Logger.LogLevel.RELEASE)

                                Else
                                    'If no packet count update happened then there seems to be some
                                    'trouble in downloading the file.
                                    'Set status to the status form displayed
                                    ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
                                       ("Error in uploading the file " + obj.strFileName.Trim("\").ToString & _
                                        ". Please try again.")
                                    'Disconnected. Sending a LOG message with Disconnected status
                                    objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                                     LOGTransmitter.Status.ABEND, _
                                                                                     obj.strFileName, _
                                                                                     LOGTransmitter.Reasons.Disconnected)
                                End If

                                Try
                                    objMonitor.KillDownloadThread()
                                    thDownloadThread.Abort()
                                Catch ex As ThreadAbortException
                                    'Do nothing.
                                End Try
                                thDownloadThread = Nothing
                                objAppContainer.objLogger.WriteAppLog("TimerEvent: Restartign the device", _
                                                                                Logger.LogLevel.ERROR)
                                Exit While
                            Else
                                m_iPreviousGetStatus = tmpNum
                            End If
                            'Wait before next check on the packet status.
                            Threading.Thread.Sleep(objAppContainer.objConfigFileParams.ConnectionLostCheckTime)
                        ElseIf objMonitor.objDownloadStatus = Status.Completed Then
                            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_LOAD, _
                                                                             LOGTransmitter.Status.END_OK, _
                                                                             obj.strFileName, _
                                                                             LOGTransmitter.Reasons.Download_Complete)
                            'Set status to the status form displayed
                            ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
                            ("Successfully uploaded the file " + obj.strFileName.Trim("\").ToString & ".")
                            'If download is completed thread status is returned as Completed.
                            Try
                                thDownloadThread.Abort()
                            Catch ex As ThreadAbortException
                                'Do nothing.
                            End Try
                            thDownloadThread = Nothing
                            objMonitor = Nothing
                            bIsFileDownloaded = True
                            'File download completed successfully.
                            Exit While
                        End If
                    End While
                    'Start populating the database if file download is completed successfully.
                    objAppContainer.objLogger.WriteAppLog("BatchProcessor:Downloading file completed:" & obj.strFileName, _
                                                          Logger.LogLevel.RELEASE)
                    If bIsFileDownloaded Then
                        'Update the doanload status and populate the Database.
                        m_objBatchConfigParser.UpdateParams()
                        objAppContainer.objLogger.WriteAppLog("BatchProcessor:Parsing started for file " & obj.strFileName, _
                                                              Logger.LogLevel.RELEASE)
                        'Check if all the files are downloaded so that the database file can be deleted.
                        If m_lstFiles.Count >= objAppContainer.objConfigFileParams.ReferenceFileCount Then
                            If obj.strFileName = objAppContainer.objConfigFileParams.BOOTCODE Then
                                DeleteDBandBathcProcess()
                                BatchConfigParser.GetInstance.AddToXml(obj.strFileName)
                            End If
                            bPurgeExitingData = False
                        End If

                        If objDBPopulate.Populate(obj, bPurgeExitingData) Then
                            'Update the file details to config file.
                            UpdateBuildTime(obj)
                            ConfigParser.GetInstance().UpdateConfig()
                            objAppContainer.objLogger.WriteAppLog("BatchProcessor:DB Population success for file " & obj.strFileName, _
                                                                  Logger.LogLevel.RELEASE)
                        End If
                    Else
                        objAppContainer.objLogger.WriteAppLog("BatchProcessor: File not available " & obj.strFileName, _
                                                              Logger.LogLevel.RELEASE)
                    End If
                End If
                'Next
                iIndex = iIndex + 1
                If bFlag Then
                    iIndex = iIndex - 1
                    bFlag = False
                End If
            End While
            objAppContainer.objLogger.WriteAppLog("BatchProcessor:Downloading reference file completed.", _
                                                            Logger.LogLevel.RELEASE)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("BatchProcessor:DownloadReferenceFiles" + ex.Message, Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Used to download selected files from the controller
    ''' The Filenames are passed in an array list
    ''' </summary>
    ''' <param name="arrFileList"></param>
    ''' <remarks></remarks>
    Public Sub DownloadSelectedFiles(ByVal arrFileList As ArrayList)
        objAppContainer.objLogger.WriteAppLog("Entered Download Selected Reference Files of BatchProcessor", Logger.LogLevel.INFO)
        ReferenceDataMgr.GetInstance.objDownloadReferenceData.StartUserControl()
        Dim arrFileToDownload As ArrayList = New ArrayList()
        Try
            'Get the control file.
            GetControlFile()
            'Parse control file to get the list of files that are ready.
            ParseControlFile(objAppContainer.objConfigFileParams.TFTP.LocalFilePath.TrimEnd("\") + "\" + objAppContainer.objConfigFileParams.ControlFile.Name)
            'Get the list of files to be downloaded.
            For Each strfileName As String In arrFileList
                For Each obFile As FileDetails In m_lstFiles
                    If obFile.strFileName = strfileName Then
                        arrFileToDownload.Add(obFile)
                        Exit For
                    End If
                Next
            Next
            'Clear the private array ans provide only the list of files to be downloaded.
            If arrFileToDownload.Count > 0 Then
                m_lstFiles.Clear()
                m_lstFiles = arrFileToDownload
                DownloadReferenceFiles()
            Else
                MessageBox.Show("The Selected file is not ready to upload", "Error", _
                          MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
            End If
            'Call function to download the reference file.

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.INFO)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting Download Selected Reference Files of BatchProcessor", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Function to update build time for a file after it is downloaded and 
    ''' updated in the database.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <remarks></remarks>
    Public Sub UpdateBuildTime(ByVal obj As FileDetails)
        ReferenceDataMgr.GetInstance.objDownloadReferenceData.SetCurrentStatus _
        ("Updating the build time for the reference files")
        Select Case obj.strFileName
            Case objAppContainer.objConfigFileParams.BOOTCODE
                objAppContainer.objConfigFileParams.LastBuildBOOTCODE = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.PGROUP
                objAppContainer.objConfigFileParams.LastBuildPGROUP = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.SUPPLIER
                objAppContainer.objConfigFileParams.LastBuildSUPPLIER = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.USERS
                objAppContainer.objConfigFileParams.LastBuildUSERS = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.RECALL
                objAppContainer.objConfigFileParams.LastBuildRECALL = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.LIVEPOG
                objAppContainer.objConfigFileParams.LastBuildLIVEPOG = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.BARCODE
                objAppContainer.objConfigFileParams.LastBuildBARCODE = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.DEAL
                objAppContainer.objConfigFileParams.LastBuildDEAL = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.CATEGORY
                objAppContainer.objConfigFileParams.LastBuildCATEGORY = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.POGMODULE
                objAppContainer.objConfigFileParams.LastBuildPOGMODULE = obj.dtLastBuild
                Exit Select
            Case objAppContainer.objConfigFileParams.SHELFDES
                objAppContainer.objConfigFileParams.LastBuildSHELFDESC = obj.dtLastBuild
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub
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
            If File.Exists(objAppContainer.objConfigFileParams.TemplatDB) Then
                If File.Exists(objAppContainer.objConfigFileParams.DB) Then
                    'delete the data base.
                    File.Delete(objAppContainer.objConfigFileParams.DB)
                    objAppContainer.objLogger.WriteAppLog("File adding to DB", _
                                                           Logger.LogLevel.RELEASE)

                End If
                'Create the new db file from template db.
                File.Copy(objAppContainer.objConfigFileParams.TemplatDB, objAppContainer.objConfigFileParams.DB)
                objAppContainer.objLogger.WriteAppLog("File adding to TemplateDB", _
                                                      Logger.LogLevel.RELEASE)
                'Clear thed details in batch config process file.
                m_objBatchConfigParser.PurgeFile()
            Else
                objAppContainer.objLogger.WriteAppLog("TemplatDB not found", Logger.LogLevel.ERROR)
                'if template db not found in the folder.
                bReturn = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("AppContainer:Replacing database file failed" _
                                                  + ex.Message, Logger.LogLevel.ERROR)
            bReturn = False
        End Try
        Return bReturn
    End Function
End Class
''' <summary>
''' Enum to hold the thread status for download thread.
''' </summary>
''' <remarks></remarks>
Public Enum Status
    Running
    Completed
    Terminated
End Enum
#End If