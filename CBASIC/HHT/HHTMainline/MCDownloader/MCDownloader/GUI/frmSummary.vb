Public Class frmSummaryScreen
    Public Delegate Sub DisposeFormCallBack()
    Public Delegate Sub DisplayFormCallBack()
    Public Delegate Sub StartTimerCallBack()
    Public Delegate Sub FormDisplayTimeCallBack(ByVal DisplayTime As Integer)
    Private Sub btnOk_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        AppContainer.GetInstance.obLogger.WriteAppLog("frmDownloadReferenceData:: OK Clicked:: Exiting Summary Screen", Logger.LogLevel.RELEASE)
        Me.Close()
    End Sub
    Private Sub frmRefDataSummary_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AppContainer.GetInstance.obLogger.WriteAppLog("frmSummaryScreen:: frmRefDataSummary_Load ::Loading Summary Screen", Logger.LogLevel.RELEASE)
        ' Initialising Variables 
        'tmrCloseTimer.Enabled = False
        Dim strFileName As String = ""
        Dim iUpperFileLimit As Integer
        Dim m_XMLStatusFile As New Xml.XmlDocument
        Dim strBuildStatus As String
        Dim iCount As Integer = 0
        'setting the process bar status
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        'Identifying the invoking Form
        strFileName = Macros.REF_STATUS_FILE.ToString
        iUpperFileLimit = Macros.NO_REF_FILES
        Dim Status As Xml.XmlElement
        'checking the existence of the file
        If System.IO.File.Exists(strFileName.ToString) Then
            Try
                m_XMLStatusFile.Load(strFileName.ToString)
                Status = m_XMLStatusFile.DocumentElement
                'Processing until all node are parsed
                While iCount < iUpperFileLimit
                    'Adding File Name to list view
                    SummaryList.Items.Add(New ListViewItem(Status.GetElementsByTagName("strFileName").ItemOf(iCount).InnerText.ToString))
                    'Getting the exception status 
                    strBuildStatus = Status.GetElementsByTagName("strBuildStatus").ItemOf(iCount).InnerText.ToString()
                    If strBuildStatus = "Y" Then
                        'No Exception occured and hence assigning "Success" to the subitem
                        SummaryList.Items(iCount).SubItems.Add("Success")
                    Else
                        'Exception occured and hence assigning "Failed" to the subitem
                        SummaryList.Items(iCount).SubItems.Add("Failed")
                    End If
                    iCount = iCount + 1
                End While
                'Setting the processing status to null
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            Catch ex As Exception
                'In Case of error the message is displayed and writing to the logger
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
                'MessageBox.Show("Error while retrieving reference data download status", _
                '                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                Me.Close()
            End Try
            Status = Nothing
            m_XMLStatusFile = Nothing

            'Time to keep the summary screen visible.
            tmrCloseTimer.Interval = AppContainer.GetInstance().m_objConfigParams.SummaryScreenDisplayTime
            tmrCloseTimer.Enabled = True
        Else
            'If file not found then displaying the appropriate message
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            'MessageBox.Show("Reference data download status file - Not Found", _
            '                     "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Me.Close()
        End If
    End Sub
    Private Sub CloseTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrCloseTimer.Tick
        AppContainer.GetInstance.obLogger.WriteAppLog("Exiting Summary Screen after timer tick.", Logger.LogLevel.RELEASE)
        Me.Dispose()
    End Sub
End Class