Public Class frmDownloadReferenceData
#If NRF Then
    Public Delegate Sub EnableOkCallback()
    Public Delegate Sub SetCurrentStatusCallback(ByVal strTextToSet As String)
    Public Delegate Sub StopUserControlCallback()
    Public Delegate Sub StartUserControlCallback()
    Public Delegate Sub SetTitleCallBack(ByVal FormTitle As FormTitle)
    Public Delegate Sub ErrorLabelDisplaycallback(ByVal Time As Integer)
    Public Delegate Sub DBUpdationStartCallBack()
    Public Delegate Sub DBUpdationStopCallBack()
    Public Delegate Sub SetDBUpdationstatusCallBack(ByVal Percentage As Integer)
    Public Delegate Sub ChangeTimeCallBack(ByVal Time As Integer)
    ''' <summary>
    ''' Set the status bar details along with db updation.
    ''' </summary>
    ''' <param name="Percentage"></param>
    ''' <remarks></remarks>
    Public Sub SetDBUpdationstatus(ByVal Percentage As Integer)
        If Not Me.InvokeRequired Then
            If Percentage >= 0 And Percentage <= 100 Then
                lblPercentage.Text = Str(Percentage) + "%"
                pgBar.Value = Percentage
            ElseIf Percentage > 100 Then
                pgBar.Value = pgBar.Maximum
                lblPercentage.Text = "100%"
            ElseIf Percentage < 0 Then
                pgBar.Value = pgBar.Minimum
                lblPercentage.Text = "0%"
            End If
            Me.Refresh()
        Else
            Me.Invoke(New SetDBUpdationstatusCallBack(AddressOf SetDBUpdationstatus), Percentage)
        End If
    End Sub
    ''' <summary>
    ''' Function to perform initial settings before updating the database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DBUpdationStart()
        If Not Me.InvokeRequired Then
            lblDBUpdation.Visible = True
            lblPercentage.Visible = True
            pgBar.Visible = True
            lblPercentage.Text = "0%"
            pgBar.Value = pgBar.Minimum
            Me.Refresh()
        Else
            Me.Invoke(New DBUpdationStartCallBack(AddressOf DBUpdationStart))
        End If
    End Sub
    Public Sub DBUpdationStop()
        If Not Me.InvokeRequired Then
            If pgBar.Value <> pgBar.Maximum Or lblPercentage.Text <> "100%" Then
                lblPercentage.Text = "100%"
                pgBar.Value = pgBar.Maximum
                Threading.Thread.Sleep(2000)
            End If
            lblDBUpdation.Visible = False
            lblPercentage.Visible = False
            pgBar.Visible = False
            Me.Refresh()
        Else
            Me.Invoke(New DBUpdationStopCallBack(AddressOf DBUpdationStop))
        End If
    End Sub
    Public Sub DisplayErrorLabel(ByVal Time As Integer)
        If Not Me.InvokeRequired Then
            pbFileTransfer.Visible = False
            lblProcessindicator.Visible = False
            lblConnectionLost1.Visible = True
            lblProcess.Text = "Connection Lost"
            lblProcess.BackColor = Color.Red
            lblProcess.ForeColor = Color.White
            lblConnectionLost1.Text = "The connection to the device is lost."
            Me.Refresh()
        Else
            Me.Invoke(New ErrorLabelDisplaycallback(AddressOf DisplayErrorLabel), Time)
        End If
    End Sub
    Public Sub EnableButton()
        If Not Me.InvokeRequired Then
            btnOk.Enabled = True
            btnOk.Visible = True
            Me.Refresh()
        Else
            Me.Invoke(New EnableOkCallback(AddressOf EnableButton))
        End If
    End Sub
    Public Sub SetCurrentStatus(ByVal strTextToSet As String)
        If Not Me.InvokeRequired Then
            lblProcessindicator.Text = strTextToSet
            Me.Refresh()
        Else
            Me.Invoke(New SetCurrentStatusCallback(AddressOf SetCurrentStatus), strTextToSet)
        End If
    End Sub
    Public Sub StopUserControl()
        If Not Me.InvokeRequired Then
            pbFileTransfer.Visible = False
            Me.Refresh()
        Else
            Me.Invoke(New StopUserControlCallback(AddressOf StopUserControl))
        End If
    End Sub
    Public Sub StartUserControl()
        If Not Me.InvokeRequired Then
            pbFileTransfer.Visible = True
            Me.Refresh()
        Else
            Me.Invoke(New StartUserControlCallback(AddressOf StartUserControl))
        End If
    End Sub
    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        ReferenceDataMgr.GetInstance.DisplaySummary()
        Me.Close()
        ReferenceDataMgr.GetInstance.EndSession()
    End Sub
    Private Sub frmDownloadReferenceData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'ReferenceDataMgr.GetInstance.CheckFileException()
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
    Private Sub cmdDownloadOptions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDownloadOptions.SelectedIndexChanged
        Dim strFile As String = ""
        If cmdDownloadOptions.SelectedIndex <> 0 Then
            If cmdDownloadOptions.SelectedIndex = cmdDownloadOptions.Items.Count - 1 Then
                strFile = "0"
            ElseIf cmdDownloadOptions.SelectedIndex = cmdDownloadOptions.Items.Count - 2 _
                AndAlso cmdDownloadOptions.SelectedIndex <> 1 Then
                strFile = "1"
            Else
                strFile = cmdDownloadOptions.SelectedItem.ToString()
            End If
            ReferenceDataMgr.GetInstance.RecieveData(strFile)
        End If
    End Sub
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        ReferenceDataMgr.GetInstance.DisplaySummary()
        Me.Close()
        ReferenceDataMgr.GetInstance.EndSession()
    End Sub
    Public Enum FormTitle
        frmReferenceDataDownload
        frmLogFileUpload
    End Enum
#End If
End Class