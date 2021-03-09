Public Class ExportDataViewer
#If NRF Then
    Private Shared m_exportdataviewer As ExportDataViewer = Nothing
    Private m_Functionality As frmFunctionality = Nothing
    Private m_FileViewer As frmFileDataViewer = Nothing
    Private m_LogFileStream As System.IO.FileStream
    Private m_reader As System.IO.StreamReader
    Private m_strFileName As String
    Private m_FileViewerlabel As String
    ''' <summary>
    ''' Initialising the Array
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()

    End Sub

    Public Shared Function getinstance() As ExportDataViewer
        If m_exportdataviewer Is Nothing Then
            m_exportdataviewer = New ExportDataViewer
            Return m_exportdataviewer
        Else
            Return m_exportdataviewer
        End If
    End Function

    Public Sub startsession()
        objAppContainer.objLogger.WriteAppLog("Entered startsession of ExportDataViewer", Logger.LogLevel.INFO)
        Try

            m_Functionality = New frmFunctionality
            Displaygetfunctionality()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Cannot Display the export file data", "Warning", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting startsession of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
    Public Sub Displaygetfunctionality()
        objAppContainer.objLogger.WriteAppLog("Entered Displaygetfunctionality of ExportDataViewer", Logger.LogLevel.INFO)
        Try
            m_Functionality.Text = "Select Export Data File"
            m_Functionality.Show()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting Displaygetfunctionality of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
    Public Sub processdata(ByVal exportdata As Integer)
        objAppContainer.objLogger.WriteAppLog("Entered processdata of ExportDataViewer", Logger.LogLevel.INFO)
        Dim path As String = Nothing
        m_FileViewerlabel = "Export Data "
        Try
            path = ConfigDataMgr.GetInstance.GetParam("ExportFilePath")
            If (exportdata = 1) Then
                m_strFileName = path + Macros.MCSHMON_EXPORT_FILENAME.ToString
                m_FileViewerlabel = m_FileViewerlabel + " - Shelf Monitor"
            ElseIf (exportdata = 2) Then
                m_strFileName = path + Macros.GOODSOUT_EXPORT_FILENAME.ToString
                m_FileViewerlabel = m_FileViewerlabel + " - Goods Out"
            ElseIf (exportdata = 3) Then
                m_strFileName = path + Macros.GOODSIN_EXPORT_FILENAME
                m_FileViewerlabel = m_FileViewerlabel + " - Goods In"
            End If
            ProcessView()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Cannot display the export file data", "Warning", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting processdata of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
    Public Sub ProcessView()
        objAppContainer.objLogger.WriteAppLog("Entered ProcessView of ExportDataViewer", Logger.LogLevel.INFO)
        m_FileViewer = New frmFileDataViewer
        Try
            If m_FileViewer IsNot Nothing Then
                m_LogFileStream = New System.IO.FileStream(m_strFileName, IO.FileMode.Open)
                m_reader = New System.IO.StreamReader(m_LogFileStream)
                DisplayFileData()
                m_LogFileStream.Close()
                m_LogFileStream = Nothing
                m_reader = Nothing
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Export data file does not exist", "Error", _
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting ProcessView of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
    Public Sub DisplayFileData()
        objAppContainer.objLogger.WriteAppLog("Entered DisplayFileData of ExportDataViewer", Logger.LogLevel.INFO)
        Dim FileData As String = ""
        Try
            With m_FileViewer
                'Reading the Data From the file 
                'Displaying the file data in the text box
                .m_Invokingform = "ExportDataViewer"
                .lblFileViewer.Text = m_FileViewerlabel
                .Text = "Export Data File - Data Viewer"
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
            MessageBox.Show("Cannot Display the export file data", "Warning", _
                            MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting DisplayFileData of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
    Public Sub EndDisplay()
        objAppContainer.objLogger.WriteAppLog("Entered EndDisplay of ExportDataViewer", Logger.LogLevel.INFO)
        m_FileViewer.Dispose()
        m_FileViewer = Nothing
        objAppContainer.objLogger.WriteAppLog("Exiting EndDisplay of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
    Public Sub EndSession()
        objAppContainer.objLogger.WriteAppLog("Entered EndSession of ExportDataViewer", Logger.LogLevel.INFO)
        Try
            m_strFileName = Nothing
            m_exportdataviewer = Nothing
            m_Functionality.Close()
            m_Functionality.Dispose()
            m_Functionality = Nothing
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message.ToString, Logger.LogLevel.RELEASE)
            MessageBox.Show("Error while closing the export file data viewer", "Error", _
                           MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exiting EndSession of ExportDataViewer", Logger.LogLevel.INFO)
    End Sub
#End If
End Class
