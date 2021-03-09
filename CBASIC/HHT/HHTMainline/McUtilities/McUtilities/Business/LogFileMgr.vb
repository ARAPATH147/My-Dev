#If NRF Then
Imports System.IO
Imports Ionic.Zip
Public Class LogFileMgr
    ''' <summary>
    ''' Initialise all the forms required.
    ''' </summary>
    ''' <remarks></remarks>
    Private m_ViewLogList As frmViewLogFileListScreen = Nothing
    Private m_ProcessLog As frmProcessLogFile = Nothing
    Private m_Functionality As frmFunctionality = Nothing
    Private m_viewlogData As frmFileDataViewer = Nothing
    ''' <summary>
    ''' Private object of the class to implement singleton class.
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared m_LogFileMgr As LogFileMgr = Nothing
    ''' <summary>
    ''' Member variables
    ''' </summary>
    ''' <remarks></remarks>
    Private m_strFileName As String = Nothing
    Private m_LogFileDataList As ArrayList = Nothing
    Private m_LogFileStream As System.IO.FileStream = Nothing
    Private m_reader As System.IO.StreamReader = Nothing
    Private m_SelectedIndex As Integer
    Private m_Filenumber As Integer
    Private m_Path As String = Nothing
    ''' <summary>
    ''' To intialise the local variables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub
    ''' <summary>
    ''' To get the instance of the LogFileMgr class
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As LogFileMgr
        If m_LogFileMgr Is Nothing Then
            m_LogFileMgr = New LogFileMgr()
            Return m_LogFileMgr
        Else
            Return m_LogFileMgr
        End If
    End Function
    ''' <summary>
    ''' To start the session and intialise all the variables 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartSession()
        objAppContainer.objLogger.WriteAppLog("Entering LogFileMgr", Logger.LogLevel.INFO)
        Cursor.Current = Cursors.WaitCursor
        m_LogFileDataList = New ArrayList()
        m_Functionality = New frmFunctionality
        m_ViewLogList = New frmViewLogFileListScreen()
        m_ProcessLog = New frmProcessLogFile()
        If GetFileInfo() Then
            DisplayScreen(LOG_SCREENS.Home)
        End If
        Cursor.Current = Cursors.Default
    End Sub
    ''' <summary>
    ''' Ends the Display of File Content and Disposes the form
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndDisplay()
        objAppContainer.objLogger.WriteAppLog("Exiting LogFile Display", Logger.LogLevel.INFO)
        m_viewlogData.Close()
        m_viewlogData.Dispose()
    End Sub
    ''' <summary>
    ''' End the ProcessLog File Form and releases its memory
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndProcessLogFile()
        Try
            objAppContainer.objLogger.WriteAppLog("Exiting from log-file Functions screen", Logger.LogLevel.INFO)
            m_ProcessLog.Dispose()
            m_ProcessLog = Nothing
            m_ViewLogList.Visible = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error While exiting Process Log File Display", Logger.LogLevel.INFO)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting ProcessLogFile Display", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' To end the session and release all the objects held by the LogFileMgr
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub EndSession()
        Try
            objAppContainer.objLogger.WriteAppLog("Exiting LogFileMgr", Logger.LogLevel.INFO)
            'm_ViewLogList.Close()
            'm_ViewLogList.Dispose()
            m_LogFileMgr = Nothing
            m_viewlogData = Nothing
            m_LogFileDataList = Nothing

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while exiting LogFileMgr", Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Displays the loglist screen for View Option
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayLogFileList()
        objAppContainer.objLogger.WriteAppLog("Entered Display Log File List", Logger.LogLevel.INFO)
        Try
            Dim pgBarsize As Integer
            Dim incrementer As Integer
            Dim counter As Integer = 1
            Dim Index As Integer = 0
            Cursor.Current = Cursors.WaitCursor
            pgBarsize = m_ViewLogList.pgBar.Maximum
            pgBarsize = Math.Round(pgBarsize / m_Filenumber - 0.5)
            incrementer = pgBarsize
            m_ViewLogList.lstvwLogFiles.Items.Clear()
            With m_ViewLogList
                .lstvwLogFiles.Enabled = False
                .lblTotalFiles.Text = m_Filenumber.ToString
                .Show()
                'Adding file deails to the list view
                For Each objLogFileData As LogFileInfo In m_LogFileDataList
                    .lstvwLogFiles.Items.Add( _
                        (New ListViewItem(New String() {objLogFileData.m_LogFileName, _
                                                        objLogFileData.m_ModifiedDate})))
                    If (objLogFileData.m_ExceptionStatus = "Y") Then
                        .lstvwLogFiles.Items(Index).ForeColor = Color.DarkRed
                    Else
                        .lstvwLogFiles.Items(Index).ForeColor = Color.DarkGreen
                    End If
                    Index = Index + 1
                    'Handling Progress bar
                    .pgBar.Value = incrementer
                    .lblCurrFile.Text = counter.ToString
                    counter = counter + 1
                    incrementer = incrementer + pgBarsize
                    .Refresh()
                Next
                .pgBar.Value = .pgBar.Maximum
                .Refresh()
                .lblCurrFile.Visible = False
                .lblTotalFiles.Visible = False
                .Label2.Visible = False
                .pgBar.Visible = False
                .Refresh()
                .lstvwLogFiles.Enabled = True
                Cursor.Current = Cursors.Default
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting Display Log File List", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displays the log file data for View Option
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayProcessLog()
        objAppContainer.objLogger.WriteAppLog("Entered DisplayProcess log of LogFileMgr", Logger.LogLevel.INFO)
        Try
            Dim strFileName As String = ""
            If Not m_SelectedIndex < 0 Then
                With m_ProcessLog
                    strFileName = m_ViewLogList.lstvwLogFiles.Items(m_SelectedIndex).Text.ToString()
                    .lblFileName.Text = strFileName
                    If strFileName.EndsWith(".ZIP") Or strFileName.EndsWith(".zip") Then
                        .PictureBox1.Visible = False
                        .Label1.Visible = False
                        .PictureBox2.Location = New System.Drawing.Point(34 * objAppContainer.iOffset, 40 * objAppContainer.iOffset)
                        .Label2.Location = New System.Drawing.Point(13 * objAppContainer.iOffset, 95 * objAppContainer.iOffset)
                    Else
                        .PictureBox1.Visible = True
                        .Label1.Visible = True
                        .PictureBox2.Location = New System.Drawing.Point(34 * objAppContainer.iOffset, 135 * objAppContainer.iOffset)
                        .Label2.Location = New System.Drawing.Point(10 * objAppContainer.iOffset, 187 * objAppContainer.iOffset)
                    End If
                    .Visible = True
                    .Refresh()
                End With
            Else
                MessageBox.Show("Unable to process your request", "Error", _
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayProcessLog of LogFileMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Screen Display method for Count List. 
    ''' All Count List sub screens will be displayed using this method.
    ''' </summary>
    ''' <param name="ScreenName"></param>
    ''' <returns>True if success</returns>
    ''' <remarks></remarks>
    Public Function DisplayScreen(ByVal ScreenName As LOG_SCREENS)
        objAppContainer.objLogger.WriteAppLog("Entered DisplayScreen of LogFileMgr", Logger.LogLevel.INFO)
        Try
            Select Case ScreenName
                Case LOG_SCREENS.Home
                    'm_ViewLogList = New frmViewLogFileListScreen
                    'Invoke method to display the home screen
                    m_ViewLogList.Invoke(New EventHandler(AddressOf DisplayLogFileList))
                Case LOG_SCREENS.SelectProcess
                    m_ProcessLog = New frmProcessLogFile()
                    'Invoke method to display the home screen
                    m_ProcessLog.Invoke(New EventHandler(AddressOf DisplayProcessLog))
            End Select
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit DisplayScreen of LogFileMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Process the click and finds the appropriate file
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessFileSelection() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered ProcessFileSelection of LogFileMgr", Logger.LogLevel.INFO)
        With m_ViewLogList
            Dim iIndex As Integer = 0
            Dim iCounter As Integer = 0
            Dim strFileName As String = ""

            'Gets the selected list id from the listview
            Try
                Dim bIsDataAvailable As Boolean = False
                If .lstvwLogFiles.SelectedIndices.Count > 0 Then
                    For iCounter = 0 To .lstvwLogFiles.Items.Count - 1
                        If .lstvwLogFiles.Items(iCounter).Selected Then
                            bIsDataAvailable = True
                            strFileName = .lstvwLogFiles.Items(iCounter).Text
                            m_SelectedIndex = iCounter
                            Exit For
                        End If
                    Next
                End If

                If bIsDataAvailable Then
                    m_strFileName = m_Path + strFileName
                    DisplayScreen(LOG_SCREENS.SelectProcess)
                    'System.Diagnostics.Process.Start("notepad.exe", strFileName)
                Else
                    Return False
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
                Return False
            End Try

        End With
        objAppContainer.objLogger.WriteAppLog("Exiting ProcessFileSelection of LogFileMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Selecting the appropriate file Path
    ''' </summary>
    ''' <param name="exportdata"></param>
    ''' <remarks></remarks>
    Public Sub processdata(ByVal exportdata As Integer)
        objAppContainer.objLogger.WriteAppLog("Entered processdata of LogFileMgr", Logger.LogLevel.INFO)
        m_Functionality.Close()
        m_Functionality.Dispose()
        m_Functionality = Nothing
        Try
            'Based on the value returned the path of log file is selected
            If (exportdata = 1) Then
                m_Path = ConfigDataMgr.GetInstance.GetParam("ExportFile1")
            ElseIf (exportdata = 2) Then
                m_Path = ConfigDataMgr.GetInstance.GetParam("ExportFile2")
            ElseIf (exportdata = 3) Then
                m_Path = ConfigDataMgr.GetInstance.GetParam("ExportFile3")
            End If
            'Adding log file path to the path
            m_Path = m_Path + "/Log/"
            If GetFileInfo() Then
                DisplayScreen(LOG_SCREENS.Home)
            End If
        Catch ex As Exception
            'MessageBox.Show(ex.Message)
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting processdata of LogFileMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Processes the View Button click
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ProcessView()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessView of LogFileMgr", Logger.LogLevel.INFO)
        m_viewlogData = New frmFileDataViewer()
        Try
            If Not m_strFileName.EndsWith(".zip") Then
                If m_strFileName <> Nothing Then
                    m_LogFileStream = New FileStream(m_strFileName, FileMode.Open)
                    m_reader = New StreamReader(m_LogFileStream)
                    DisplayFileData()
                    m_LogFileStream.Close()
                    m_LogFileStream = Nothing
                    m_reader = Nothing
                End If
            Else
                MessageBox.Show("File with .zip extension cannotbe viewed. Please select a different file.", _
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Unable to Process your Request", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting ProcessView of LogFileMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Displaying the data in the file in a text box
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DisplayFileData()
        objAppContainer.objLogger.WriteAppLog("Entered DisplayFileData of LogFileMgr", Logger.LogLevel.INFO)
        Dim FileData As String = ""
        Try
            'Displaying the file data in the text box
            With m_viewlogData
                'setting the data of the invoking Form
                .m_Invokingform = "LogFileMgr"
                .lblFileViewer.Text = "Log File Data"
                .Text = "Log File Data Viewer"
                .Show()
                .Refresh()
                Cursor.Current = Cursors.WaitCursor
                Do Until m_reader.EndOfStream
                    'Reading the Data From the file 
                    FileData = m_reader.ReadLine
                    'Displaing in the text box
                    .TextBox1.Text = .TextBox1.Text + FileData.ToString
                Loop
                Cursor.Current = Cursors.Default
                .Refresh()
            End With
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Unable to display the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting DisplayFileData of LogFileMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Sends the Log File using TFtp
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendLogFile() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered SendLogFile of LogFileMgr", Logger.LogLevel.INFO)
        If ReferenceDataMgr.GetInstance.CheckDeviceDocked() Then
            Try
                Dim obj_LogFileUploader As New LogFileUploader
                Dim objConfigParams As New ConfigParams
                If m_strFileName <> "" And File.Exists(m_strFileName) Then
                    obj_LogFileUploader.Start(m_strFileName)
                    File.Delete(m_strFileName)
                    objAppContainer.objLogger.WriteAppLog("Exiting SendLogFile of LogFileMgr after successfully sending log file", Logger.LogLevel.INFO)
                    Return True
                Else
                    MessageBox.Show("Unable to send the log file", _
                                     "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                     MessageBoxDefaultButton.Button1)
                    objAppContainer.objLogger.WriteAppLog("Exception: File Name Null", Logger.LogLevel.INFO)
                    Return False
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
                MessageBox.Show("Unable to Process Your Request", _
                                     "Alert ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, _
                                     MessageBoxDefaultButton.Button1)
                Return False
            End Try
        Else
            MessageBox.Show("Please make sure that the device has been docked properly and Try again", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            objAppContainer.objLogger.WriteAppLog("Exiting SendLogFile of LogFileMgr", Logger.LogLevel.INFO)
            Return False
        End If

    End Function
    ''' <summary>
    ''' Function to send all log files present in the devices.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SendAllLogFiles() As Boolean
        objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                              LOGTransmitter.Status.START, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Downloading_File)
        Try
            Cursor.Current = Cursors.WaitCursor
            Dim strPath As String = ""
            strPath = ConfigDataMgr.GetInstance.GetParam("LogFilePath")
            Dim dirInfo As New IO.DirectoryInfo(strPath)
            Dim fileInfo As IO.FileInfo
            Dim objLogFileUploader As New LogFileUploader
            'm_ViewLogList.lblStatus.Text = "Status: Compressing log files"
            Dim strLogFile As String = ZipAllFiles()
            Dim FileArray As IO.FileInfo() = dirInfo.GetFiles("*.ZIP")
            If File.Exists(strLogFile) Or FileArray.Length > 0 Then
                For Each fileInfo In FileArray
                    'm_ViewLogList.lblStatus.Text = "Status: Sending file " + fileInfo.Name
                    m_strFileName = strPath.TrimEnd("\") + "\" + fileInfo.Name
                    If m_strFileName <> "" And File.Exists(m_strFileName) Then
                        objLogFileUploader.Start(m_strFileName, True)
                        'm_ViewLogList.lblStatus.Text = "Status: Deleting file " + fileInfo.Name
                        File.Delete(m_strFileName)
                        Threading.Thread.Sleep(3000)
                    End If
                Next
            Else
                MessageBox.Show("No log files found", "Error")
            End If
            'm_ViewLogList.lblStatus.Text = "Status: Sending log transaction END"
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                         LOGTransmitter.Status.END_OK, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Download_Complete)
            Cursor.Current = Cursors.Default
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("SendAllLogFiles:" & ex.Message.ToString, _
                                                  Logger.LogLevel.RELEASE)
            objAppContainer.objLogMessageTransmitter.sendLog(LOGTransmitter.Action.TFTP_SEND, _
                        LOGTransmitter.Status.ABEND, LOGTransmitter.FileName.POD_Log_Files, LOGTransmitter.Reasons.Other_Errors)

        Finally
            Cursor.Current = Cursors.Default
        End Try
    End Function
    ''' <summary>
    ''' Delete a selected log file.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteFile() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered DeleteFile of LogFileMgr", Logger.LogLevel.INFO)
        Try
            File.Delete(m_strFileName)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            Return False
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting DeleteFile of LogFileMgr", Logger.LogLevel.INFO)
        Return True
    End Function
    ''' <summary>
    ''' Function to zip the log files when send all option is selected.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ZipAllFiles() As String
        Dim strMacAddress As String = ""
        Dim objUtils As Utility = New Utility()
        Dim strCurrLogFile As String = ""
        Dim strZipFileName As String = ""
        Dim strZipFile As String = ""
        Dim strLocalPath As String = objAppContainer.objConfigFileParams.LocalLogFilePath
        Try
            strMacAddress = objUtils.GetSerialNumber()
            strCurrLogFile = objAppContainer.objLogger.GetLogFileName
            Dim dirinfoLogDirectory As New DirectoryInfo(strLocalPath)
            strZipFileName = strMacAddress.Substring(strMacAddress.Length - 5)
            strZipFileName = strZipFileName + GetMonth()
            strZipFileName = strZipFileName + Date.Today.Day.ToString().PadLeft(2, "0") + ".ZIP"
            'Create the zip file name with path
            strZipFile = strLocalPath.TrimEnd("\") + "\" + strZipFileName
            objAppContainer.objLogger.WriteAppLog("LogFileUploader::Start::Zip File Name: " + strZipFileName, _
                                                                  Logger.LogLevel.RELEASE)


            Using zippedLogFile As ZipFile = New ZipFile(strZipFile)
                Dim arrFiles As FileInfo() = dirinfoLogDirectory.GetFiles("*.txt")
                If arrFiles.Length > 1 Then
                    For Each logFileInfo As FileInfo In arrFiles
                        If Not (logFileInfo.Name.Equals(strCurrLogFile)) Then
                            'objAppContainer.objRefDownloadForm.SetCurrentStatus("Compressing File: " + logFileInfo.Name)
                            zippedLogFile.AddFile(logFileInfo.FullName)
                        End If
                    Next
                    'AppContainer.GetInstance.objRefDownloadForm.SetCurrentStatus("Saving Compressed File")
                    zippedLogFile.Save()
                    objAppContainer.objLogger.WriteAppLog("LogFileUploader::Start::Log Files Zipped Successfully", _
                                                                  Logger.LogLevel.RELEASE)

                    objAppContainer.objLogger.WriteAppLog("LogFileUploader::Start::Initialising - Deleting the log files which are compressed", _
                                                             Logger.LogLevel.RELEASE)
                    For Each logFileInfo As FileInfo In arrFiles
                        'Delete all the log file and not the zip file
                        If Not logFileInfo.Name.Equals(strCurrLogFile) Then
                            objAppContainer.objLogger.WriteAppLog("LogFileUploader::Start::Log File Named " + logFileInfo.Name + " Deleted", _
                                                                          Logger.LogLevel.RELEASE)
                            logFileInfo.Delete()
                        End If
                    Next
                Else
                    MessageBox.Show("No log files available to download", "Alert")
                    Return "ERROR.ZIP"
                End If
            End Using
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("LogFileUploader::Start::Error while Zipping log files, Message:" + ex.Message, _
                                                              Logger.LogLevel.RELEASE)
            Return ""
        End Try
        Return strZipFile
    End Function
    ''' <summary>
    ''' Get current month as single digit. for OCT, NOV and DEC return A,B,C
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetMonth() As String
        Try
            Dim strMonth As Integer = Date.Today.Month
            If strMonth < 10 Then
                Return strMonth.ToString()
            ElseIf strMonth = 10 Then
                Return "A"
            ElseIf strMonth = 11 Then
                Return "B"
            ElseIf strMonth = 12 Then
                Return "C"
            End If
        Catch ex As Exception
            Return "X"
        End Try
    End Function
    ''' <summary>
    ''' Refresh the log file list displyed to the user.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RefreshlogFileList()
        Try
            If Not (m_SelectedIndex < 0) Then
                With m_ViewLogList
                    .lstvwLogFiles.Items.Remove(.lstvwLogFiles.Items(m_SelectedIndex))
                    m_SelectedIndex = -1
                    .Refresh()
                End With
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting RefreshlogFileList of LogFileMgr", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' Get the file information
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetFileInfo() As Boolean
        objAppContainer.objLogger.WriteAppLog("Entered GetFileInfo of LogFileMgr", Logger.LogLevel.INFO)
        Dim strFileSize As String = ""
        Cursor.Current = Cursors.WaitCursor
        m_Path = ConfigDataMgr.GetInstance.GetParam("LogFilePath")
        Try
            Dim dirInfo As New IO.DirectoryInfo(m_Path)
            Dim fileArray As IO.FileInfo() = dirInfo.GetFiles("*.*")
            Dim fileInfo As IO.FileInfo
            Dim Flag As Boolean
            m_Filenumber = fileArray.Length
            m_LogFileDataList.Clear()
            Try
                For Each fileInfo In fileArray
                    strFileSize = (Math.Round(fileInfo.Length / 1024)).ToString()
                    Dim objLogFileData As LogFileInfo = New LogFileInfo()
                    Dim FileName As New System.IO.FileStream(m_Path.Trim("\") + "\" + fileInfo.Name, FileMode.Open)
                    Dim FileReader As New System.IO.StreamReader(FileName)
                    Dim FileData As String
                    Flag = False
                    While Not FileReader.EndOfStream
                        FileData = FileReader.ReadLine
                        If (FileData.Contains("exception") Or FileData.Contains("Exception")) Then
                            Flag = True
                            Exit While
                        End If
                    End While
                    FileData = FileReader.ReadToEnd
                    objLogFileData.m_LogFileName = fileInfo.Name
                    objLogFileData.m_ModifiedDate = fileInfo.LastAccessTime.ToString("MM/dd/yy")
                    If Flag Then
                        objLogFileData.m_ExceptionStatus = "Y"
                    Else
                        objLogFileData.m_ExceptionStatus = "N"
                    End If
                    FileName.Dispose()
                    FileReader.Dispose()
                    FileData = Nothing
                    m_LogFileDataList.Add(objLogFileData)
                    objLogFileData = Nothing
                Next
                If m_LogFileDataList.Count > 0 Then
                    m_LogFileDataList.Sort()
                Else
                    MessageBox.Show("File Not Found", "Alert  ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                    Cursor.Current = Cursors.Default

                    objAppContainer.objLogger.WriteAppLog("File Not Found", Logger.LogLevel.RELEASE)
                    Return False
                End If
            Catch ex As Exception
                objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
                MessageBox.Show("Unable to Read Log Files", "Error ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Cursor.Current = Cursors.Default
                Return False
            End Try
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("File Not Found", "Alert ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Cursor.Current = Cursors.Default
            Return False
        End Try
        Cursor.Current = Cursors.Default
        Return True
    End Function
End Class
''' <summary>
''' Enum Class that defines all screens for Log File handling
''' </summary>
''' <remarks></remarks>
Public Enum LOG_SCREENS
    Home
    SelectProcess
End Enum
''' <summary>
''' Custom sort class to sort the log files before it is displayed to the user.
''' </summary>
''' <remarks></remarks>
Public Class LogFileInfo
    Implements IComparable
    Public m_LogFileName As String
    Public m_ModifiedDate As String
    Public m_ExceptionStatus As String
    ''' <summary>
    ''' Function to sort the log files according to last modified date.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CompareTo(ByVal obj As Object) As Integer _
      Implements System.IComparable.CompareTo
        If TypeOf obj Is LogFileInfo Then
            Dim objLogFileCompare As LogFileInfo = CType(obj, LogFileInfo)
            Dim iResult As Integer = Me.m_ModifiedDate.CompareTo(objLogFileCompare.m_ModifiedDate)

            If iResult = 0 Then
                iResult = Me.m_LogFileName.CompareTo(objLogFileCompare.m_LogFileName)
            End If
            Return iResult
        End If
    End Function
End Class
#End If