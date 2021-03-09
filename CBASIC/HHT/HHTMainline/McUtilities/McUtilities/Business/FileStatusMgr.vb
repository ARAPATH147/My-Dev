Public Class FileStatusMgr
    ''' <summary>
    ''' Member variables.
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Private Shared m_FileStatusMgr As FileStatusMgr = Nothing
    Private m_FileInfo As frmFileDetails = Nothing
    Private m_RefFileDownloadInfo As frmSummaryScreen = Nothing
    Private m_ExportDataStatus As String = Nothing
    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        m_FileInfo = New frmFileDetails()
    End Sub
    ''' <summary>
    ''' To get the instance of the UserSessionManager class
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As FileStatusMgr
        ' Get the user auth active module 
        If m_FileStatusMgr Is Nothing Then
            m_FileStatusMgr = New FileStatusMgr()
            Return m_FileStatusMgr
        Else
            Return m_FileStatusMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        objAppContainer.objLogger.WriteAppLog("Entered StartSession of FileStatusMgr", Logger.LogLevel.INFO)
        Try
            Cursor.Current = Cursors.WaitCursor
            DisplayFileInfo()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Cursor.Current = Cursors.Default
        End Try
        Cursor.Current = Cursors.Default
        objAppContainer.objLogger.WriteAppLog("Exiting StartSession of FileStatusMgr", Logger.LogLevel.INFO)
        ' DisplayScreen(FILE_INFO_SCREENS.Home)
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the UserSessionManager
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EndSession() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of FileStatusMgr", Logger.LogLevel.INFO)
        Try
            m_FileInfo.Close()
            m_FileInfo.Dispose()
            m_FileStatusMgr = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting EndSession of FileStatusMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    Public Sub DisplayFileInfo()
        With m_FileInfo
            objAppContainer.objLogger.WriteAppLog("Entered DisplayFileInfo of FileStatusMgr", Logger.LogLevel.INFO)
            Try
                .btnOk.Visible = False
                .btnOk.Enabled = False
                .Visible = True
                .Refresh()
                .lblRefDataDwTimeVal.Text = ConfigDataMgr.GetInstance.GetParam( _
                                            ConfigKey.REFERENCE_FILE_DOWNLOAD_TIME).Substring(10, 6)
                .lblProcessInfo.Text = "Calculating reference data upload time."
                .Refresh()
                GetDownloadTimes()
                UpdateDownloadStatus()
                CalcLogFileSize()
                CalcExportFileSize()
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Finally
                .btnOk.Visible = True
                .btnOk.Enabled = True
            End Try
            objAppContainer.objLogger.WriteAppLog("Exiting DisplayFileInfo of FileStatusMgr", Logger.LogLevel.INFO)
        End With
    End Sub
    Public Sub UpdateDownloadStatus()
        objAppContainer.objLogger.WriteAppLog("Entered UpdateDownloadStatus of FileStatusMgr", Logger.LogLevel.INFO)
        Try
            'Updating the Reference File Download Status
            UpdateRefFileStatusInfo()
            'Updating the Active and Export Data Download Status
            UpdateActDataStatus()
            UpdateExportDataStatus()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Error occured while retrieving the download/upload details.", _
                            "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting UpdateDownloadStatus of FileStatusMgr", Logger.LogLevel.INFO)
    End Sub
    Public Sub UpdateActDataStatus()
        objAppContainer.objLogger.WriteAppLog("Entered UpdateActDataStatus of FileStatusMgr", Logger.LogLevel.INFO)
        With m_FileInfo
            Dim actDownloadStatusFile As New Xml.XmlDocument
            Dim Status As Xml.XmlElement
            Dim iCount As Integer = 0
            Try
                actDownloadStatusFile.Load(Macros.ACT_EXP_STATUSFILE.ToString)
                Status = actDownloadStatusFile.DocumentElement
                .lblActdataStatus.Text = "Pass"
                .lblActdataStatus.ForeColor = Color.DarkGreen
                While iCount < 7
                    If (Status.GetElementsByTagName("status").ItemOf(iCount).InnerText.ToString() = "F") Then
                        .lblActdataStatus.Text = "Fail"
                        .lblActdataStatus.ForeColor = Color.DarkRed
                        Exit While
                    End If
                    iCount = iCount + 1
                End While
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
                .lblActdataStatus.Text = "Error"
                .lblActdataStatus.ForeColor = Color.DarkOrchid
            End Try
            actDownloadStatusFile = Nothing
            .Refresh()
        End With
        objAppContainer.objLogger.WriteAppLog("Exiting UpdateActDataStatus of FileStatusMgr", Logger.LogLevel.INFO)
    End Sub
    Private Sub UpdateExportDataStatus()
        objAppContainer.objLogger.WriteAppLog("Entered Update Export Data Status", Logger.LogLevel.INFO)
        With m_FileInfo
            Dim expDownloadStatusFile As New Xml.XmlDocument
            Dim arrFileName As New ArrayList
            Dim iCount As Integer = 0
            'Adding the export data file names to the array list
            arrFileName.Add(Macros.MCSHMON_EXPORT_FILENAME.ToString().Split(".")(0))
            arrFileName.Add(Macros.GOODSOUT_EXPORT_FILENAME.ToString().Split(".")(0))
            arrFileName.Add(Macros.GOODSIN_EXPORT_FILENAME.ToString().Split(".")(0))
            If System.IO.File.Exists(Macros.ACT_EXP_STATUSFILE.ToString) Then
                expDownloadStatusFile.Load(Macros.ACT_EXP_STATUSFILE.ToString)
                Try
                    For Each file In arrFileName
                        Dim Node As Xml.XmlNode = expDownloadStatusFile.DocumentElement.SelectSingleNode("/filestatus/File[@name=""" & file.ToString & """]")
                        If Not Node Is Nothing Then
                            If (Node.Attributes("status").InnerText.ToString = "F") Then
                                m_ExportDataStatus = m_ExportDataStatus + file.ToString + "Download Failed" + vbCr
                            End If
                        End If
                    Next

                    If m_ExportDataStatus = "" Then
                        m_ExportDataStatus = "Successfully downloaded the export data files."
                        .lblExpStatus.Text = "Pass"
                        .lblExpStatus.ForeColor = Color.DarkGreen
                    Else
                        .lblExpStatus.Text = "Fail"
                        .lblExpStatus.ForeColor = Color.DarkRed
                    End If

                Catch ex As Exception
                    .lblExpStatus.Text = "Error"
                    .lblExpStatus.ForeColor = Color.DarkOrchid
                    m_ExportDataStatus = "Error occured while retrieving the status."
                    objAppContainer.objLogger.WriteAppLog(ex.Message.ToString(), Logger.LogLevel.INFO)
                End Try
            Else
                .lblExpStatus.Text = "Error"
                .lblExpStatus.ForeColor = Color.DarkOrchid
                m_ExportDataStatus = "Configuration file not found."
                objAppContainer.objLogger.WriteAppLog("Config file not found.", Logger.LogLevel.INFO)
            End If
            .Refresh()
        End With
        objAppContainer.objLogger.WriteAppLog("Exiting Update Export Data Status", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processing "I" Button Click from File Details Screen For Active Data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ActiveDataInfo()
        Try
            m_RefFileDownloadInfo = New frmSummaryScreen("ActiveDataSummary")
            'm_RefFileDownloadInfo.strInvokingform = 
            m_RefFileDownloadInfo.Text = "Active data upload status:"
            m_RefFileDownloadInfo.ShowDialog()
            m_RefFileDownloadInfo.Dispose()
        Catch ex As Exception
            MessageBox.Show("Unable to retrieve active file data.", "Error")
        End Try
    End Sub
    ''' <summary>
    ''' Processing "I" Button Click from File Details Screen For Export Data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ExportDataInfo()
        MessageBox.Show(m_ExportDataStatus, "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
    End Sub
    ''' <summary>
    ''' Getting the File Upload / Download Details
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetDownloadTimes()
        objAppContainer.objLogger.WriteAppLog("Entered getDownloadtimes of FileStatusMgr", Logger.LogLevel.INFO)
        Dim xd As New Xml.XmlDocument
        Dim arrFilepath As New ArrayList
        Dim arrData As New ArrayList
        Dim arrTimeData As New ArrayList
        Dim ErrorText As String
        Dim pgValue As Integer = 16
        arrFilepath.Add(Macros.MCSHMON_CONFIG_FILE)
        arrFilepath.Add(Macros.GOODSOUT_CONFIG_FILE)
        arrFilepath.Add(Macros.GOODSIN_CONFIG_FILE)
        arrData.Add("LastActBuildTime")
        arrData.Add("LastExDataDownloadTime")
        Try
            With m_FileInfo
                .pgBar.Value = 0
                .lblProcessInfo.Text = "Getting the download time information."
                .Refresh()
                For Each strFileName In arrFilepath
                    If System.IO.File.Exists(strFileName) Then
                        'load the xml file
                        xd.Load(strFileName.ToString())
                        'query for a value
                        For Each strDownloadTime In arrData
                            Dim Node As Xml.XmlNode = xd.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key=""" & strDownloadTime.ToString() & """]")
                            If Not Node Is Nothing Then
                                arrTimeData.Add(Node.Attributes.GetNamedItem("value").Value.Substring(10, 6))
                            Else
                                arrTimeData.Add("XX:XX")
                            End If
                        Next
                    Else
                        If (arrTimeData.Count = 0) Then
                            ErrorText = "Shelf Management - "
                        ElseIf (arrTimeData.Count = 2) Then
                            ErrorText = "Goods Out - "
                        Else
                            ErrorText = "Goods In - "
                        End If
                        ErrorText = ErrorText + "Config file not found."
                        MessageBox.Show(ErrorText, "Alert", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                        arrTimeData.Add("XX:XX")
                        arrTimeData.Add("XX:XX")
                    End If
                    .pgBar.Value = .pgBar.Value + pgValue
                    .Refresh()
                Next

                .lblSMActDwTimeVal.Text = arrTimeData.Item(0).ToString
                .lblSMExpDwTimeVal.Text = arrTimeData.Item(1).ToString
                .lblGOActDwTimeVal.Text = arrTimeData.Item(2).ToString
                .lblGOExpDwTimeVal.Text = arrTimeData.Item(3).ToString
                .lblGIActDwTimeVal.Text = arrTimeData.Item(4).ToString
                .lblGIExpDwTimeVal.Text = arrTimeData.Item(5).ToString
                'return the value or nothing if it doesn't exist
                .pgBar.Value = 100
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting getDownloadtimes of FileStatusMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Calculating the Individual Export File Size
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CalcExportFileSize()
        objAppContainer.objLogger.WriteAppLog("Entered CalcExportFileSize of FileStatusMgr", Logger.LogLevel.INFO)
        With m_FileInfo
            Try
                .lblProcessInfo.Text = "Calculating export file size."
                .pgBar.Visible = True
                .pgBar.Value = 0
                .lblTotalFiles.Text = "3"
                .Refresh()
                Dim ExportFileDir As IO.DirectoryInfo
                Dim Exportfile() As IO.FileInfo
                Dim discretefile As IO.FileInfo
                Dim FileSize As Int64 = 0
                Dim counter As Integer = 1
                Dim path As String
                Dim ExportDataPath As String
                ExportDataPath = ConfigDataMgr.GetInstance.GetParam("ExportFilePath")
                While counter < 4
                    .lblCurrFile.Text = counter.ToString
                    path = ExportDataPath
                    If (counter = 1) Then
                        ExportFileDir = New IO.DirectoryInfo(path)
                        Exportfile = (ExportFileDir.GetFiles(Macros.MCSHMON_EXPORT_FILENAME.ToString))
                        If (Exportfile.Length > 0) Then
                            For Each discretefile In Exportfile
                                FileSize = FileSize + discretefile.Length
                                If FileSize > 1024 Then
                                    FileSize = Math.Round(FileSize / 1024)
                                    .lbl_SM_Ex_Size.Text = "KB"
                                End If
                            Next
                            .lblExpShmonDataSizeVal.Text = FileSize.ToString
                            .Refresh()
                            FileSize = 0
                        Else
                            MessageBox.Show("Shelf Management export data file not found!", "Error" & _
                                        " ", MessageBoxButtons.OK, _
                                        MessageBoxIcon.Exclamation, _
                                        MessageBoxDefaultButton.Button1)
                            .lblExpShmonDataSizeVal.Text = "XXXX"
                        End If

                    ElseIf counter = 2 Then

                        ExportFileDir = New IO.DirectoryInfo(path)
                        Exportfile = (ExportFileDir.GetFiles(Macros.GOODSOUT_EXPORT_FILENAME.ToString))
                        If (Exportfile.Length > 0) Then
                            For Each discretefile In Exportfile
                                FileSize = FileSize + discretefile.Length
                                If FileSize > 1024 Then
                                    FileSize = Math.Round(FileSize / 1024)
                                    .lbl_GO_Ex_Size.Text = "KB"
                                End If
                            Next
                            .lblExpGODataSizeVal.Text = FileSize.ToString
                            .Refresh()
                            FileSize = 0
                        Else
                            MessageBox.Show("Goods Out export data file not found!", "Error" & _
                                       " ", MessageBoxButtons.OK, _
                                       MessageBoxIcon.Exclamation, _
                                       MessageBoxDefaultButton.Button1)
                            .lblExpGODataSizeVal.Text = "XXXX"
                        End If

                    Else
                        ExportFileDir = New IO.DirectoryInfo(path)
                        Exportfile = (ExportFileDir.GetFiles(Macros.GOODSIN_EXPORT_FILENAME))
                        If (Exportfile.Length > 0) Then
                            For Each discretefile In Exportfile
                                FileSize = FileSize + discretefile.Length
                                If FileSize > 1024 Then
                                    FileSize = Math.Round(FileSize / 1024)
                                    .lbl_GI_Ex_Size.Text = "KB"
                                End If
                            Next
                            .lblExpGIDataSizeVal.Text = FileSize.ToString
                            .Refresh()
                            FileSize = 0
                        Else
                            MessageBox.Show("Goods In export data file not found!", "Error" & _
                                       " ", MessageBoxButtons.OK, _
                                       MessageBoxIcon.Exclamation, _
                                       MessageBoxDefaultButton.Button1)
                            .lblExpGIDataSizeVal.Text = "XXXX"
                        End If

                    End If
                    .pgBar.Value = .pgBar.Maximum / (4 - counter)
                    ExportFileDir = Nothing
                    counter = counter + 1
                End While

            Catch ex As Exception
                .lblExpShmonDataSizeVal.Text = "XXXX"
                .lblExpGIDataSizeVal.Text = "XXXX"
                .lblExpGODataSizeVal.Text = "XXXX"
                objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
                MessageBox.Show("Unable to calculate file size.", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Finally
                .lblProcessInfo.Text = "Completed calculating file size."
                .pgBar.Value = .pgBar.Maximum
                .lblProcessInfo.Visible = False
                .btnOk.Visible = True
                .pgBar.Visible = False
                .lblTotalFiles.Visible = False
                .lblOf.Visible = False
                .lblCurrFile.Visible = False
                .Refresh()
            End Try
        End With
        objAppContainer.objLogger.WriteAppLog("Exiting CalcExportFileSize of FileStatusMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Calculates the Size of the log files
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CalcLogFileSize()
        objAppContainer.objLogger.WriteAppLog("Entered CalcLogFileSize of FileStatusMgr", Logger.LogLevel.INFO)
        Try
            Dim totalfilesize As Double = 0
            Dim CurrDirectory As New IO.DirectoryInfo(ConfigDataMgr.GetInstance.GetParam("LogFilePath"))
            Dim FileArray As IO.FileInfo() = CurrDirectory.GetFiles("*.*")
            Dim DiscreteFile As IO.FileInfo
            Dim Progressor As Integer = 0
            Dim PGvalue As Integer
            Dim counter As Integer = 1
            If (FileArray.Length > 0) Then
                Progressor = Math.Round((m_FileInfo.pgBar.Maximum / FileArray.Length) - 0.5)
                PGvalue = Progressor
                With m_FileInfo
                    .lblTotalFiles.Text = FileArray.Length.ToString
                    .lblProcessInfo.Text = "Calculating log file size."
                    .Refresh()
                    If FileArray.Length > 0 Then
                        For Each DiscreteFile In FileArray
                            totalfilesize = totalfilesize + DiscreteFile.Length
                            .pgBar.Value = PGvalue
                            .lblCurrFile.Text = counter.ToString
                            counter = counter + 1
                            PGvalue = PGvalue + Progressor
                        Next
                        If totalfilesize > 1024 Then
                            totalfilesize = Math.Round(totalfilesize / 1024)
                            .lblLogSize.Text = "KB"
                        End If
                        .pgBar.Value = m_FileInfo.pgBar.Maximum
                        .Refresh()
                        .lblProcessInfo.Text = "Completed calculating file size."
                        .pgBar.Visible = False
                        .lblLogFileSizeVal.Text = CStr(totalfilesize)
                        .Refresh()
                    Else
                        .Visible = False
                        MessageBox.Show("No log files available.")
                        .lblLogFileSizeVal.Text = "0"
                    End If
                End With
            Else
                MessageBox.Show("Log file not found", "Error", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                m_FileInfo.lblLogFileSizeVal.Text = "XXXX"
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Error occured while calculating file size.", "Error")
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting CalcLogFileSize of FileStatusMgr", Logger.LogLevel.INFO)
    End Sub
    Public Sub DisplayReferenceInfo()
        objAppContainer.objLogger.WriteAppLog("Entered DisplayReferenceInfo of FileStatusMgr", Logger.LogLevel.INFO)
        Try
            m_RefFileDownloadInfo = New frmSummaryScreen("ReferenceFileSummary")
            'm_RefFileDownloadInfo.strInvokingform = "ReferenceFileSummary"
            m_RefFileDownloadInfo.Text = "Reference data upload status:"
            m_RefFileDownloadInfo.ShowDialog()
            m_RefFileDownloadInfo.Dispose()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting DisplayReferenceInfo of FileStatusMgr", Logger.LogLevel.INFO)
    End Sub

    ''' <summary>
    ''' Screen Display method 
    ''' All Count List sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>True if success</returns>
    ''' <remarks></remarks>
    Public Function DisplayScreen(ByVal ScreenName As FILE_INFO_SCREENS)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScreen of FileStatusMgr", Logger.LogLevel.INFO)
        Try

            Select Case ScreenName
                Case MEM_INFO_SCREENS.Home
                    'Invoke method to display the home screen
                    m_FileInfo.Invoke(New EventHandler(AddressOf DisplayFileInfo))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting DisplayScreen of FileStatusMgr", Logger.LogLevel.INFO)
        Return True

    End Function
    ''' <summary>
    ''' This Functions checks the Reference Downloader (Batch Processor) XML and Sets the Item Info For other Modules
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateRefFileStatusInfo()
        objAppContainer.objLogger.WriteAppLog("Entered UpdateRefFileStatusInfo of FileStatusMgr", Logger.LogLevel.INFO)
        Dim RefDownloadStatusFile As New Xml.XmlDocument
        Dim Status As Xml.XmlElement
        Dim iCount As Integer = 0
        Try
            RefDownloadStatusFile.Load(Macros.REF_STATUS_FILE.ToString)
            Status = RefDownloadStatusFile.DocumentElement
            While iCount < 10
                If (Status.GetElementsByTagName("exception").ItemOf(iCount).InnerText.ToString() <> "NA") Then
                    m_FileInfo.lblReferenceStatus.Text = "Fail"
                    m_FileInfo.lblReferenceStatus.ForeColor = Color.DarkRed
                    Exit While
                End If
                iCount = iCount + 1
                m_FileInfo.lblReferenceStatus.Text = "PASS"
                m_FileInfo.lblReferenceStatus.ForeColor = Color.DarkGreen
            End While
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            m_FileInfo.lblReferenceStatus.Text = "Error"
            m_FileInfo.lblReferenceStatus.ForeColor = Color.DarkOrchid
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting UpdateRefFileStatusInfo of FileStatusMgr", Logger.LogLevel.INFO)
        RefDownloadStatusFile = Nothing
    End Sub
End Class
Public Enum FILE_INFO_SCREENS
    Home
End Enum
