Imports System.Reflection
Public Class frmDownloadReferenceData
    Public Delegate Sub SetCurrentStatusCallback(ByVal strTextToSet As String)
    Public Delegate Sub DisposeFormCallBack()
    Public Delegate Sub DisplayFormCallBack()
    Public Delegate Sub SetTitleCallBack(ByVal enFormTitle As FormTitle)
    Public Delegate Sub ErrorLabelDisplaycallback(ByVal Time As Integer)
    Public Delegate Sub DBUpdationStartCallBack()
    Public Delegate Sub DBUpdationStopCallBack()
    Public Delegate Sub SetDBUpdationstatusCallBack(ByVal Percentage As Integer)
    Public Delegate Sub ChangeTimeCallBack(ByVal Time As Integer)
    Private strTimeDisplay As String
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
    ''' <summary>
    ''' Set the status bar details along with db updation.
    ''' </summary>
    ''' <param name="Percentage"></param>
    ''' <remarks></remarks>
    Public Sub SetDBUpdationstatus(ByVal Percentage As Integer)
        Try
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
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: SetDBUpdationstatus:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    ''' <summary>
    ''' Function to perform initial settings before updating the database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DBUpdationStart()
        Try
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
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: DBUpdationStart:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Sub DBUpdationStop()
        Try
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
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: DBUpdationStop:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Sub DisplayErrorLabel(ByVal Time As Integer)
        Try
            If Not Me.InvokeRequired Then
                'Downloader1.Visible = False
                'm_imgGifImage.Visible = False
                picImgBox.Visible = False
                lblProcessindicator.Visible = False
                lblConnectionLost1.Visible = True
                lblConnectionLost2.Visible = True
                lblProcess.Text = "Connection Lost"
                lblProcess.BackColor = Color.Red
                lblProcess.ForeColor = Color.White
                lblConnectionLost1.Text = "The connection to the device is lost."
                strTimeDisplay = "The System will restart in {0} Seconds"
                lblConnectionLost2.Text = String.Format(strTimeDisplay, Time)
                Me.Refresh()
            Else
                Me.Invoke(New ErrorLabelDisplaycallback(AddressOf DisplayErrorLabel), Time)
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: DisplayErrorLabel:: Exception Occured, Message:" & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Sub ChangeTime(ByVal Value As Integer)
        Try
            If Me.InvokeRequired = False Then
                lblConnectionLost2.Text = String.Format(strTimeDisplay, Value)
                If Me.BackColor = Color.White Then
                    Me.BackColor = Color.Gray
                Else
                    Me.BackColor = Color.White
                End If
                Me.Refresh()
            Else
                Me.Invoke(New ChangeTimeCallBack(AddressOf ChangeTime), Value)
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: ChangeTime:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Enum FormTitle
        frmReferenceDataDownload
        frmLogFileUpload
    End Enum
    Public Sub setTitle(ByVal enFormTitle As FormTitle)
        Try
            If Not Me.InvokeRequired Then
                If enFormTitle = FormTitle.frmReferenceDataDownload Then
                    Me.Text = "Reference Data Upload"
                    lblProcess.Text = "Uploading Reference Data"
                ElseIf enFormTitle = FormTitle.frmLogFileUpload Then
                    Me.Text = "Log File Download"
                    lblProcess.Text = "Dowloading Log Files"
                End If
                Me.Refresh()
            Else
                Me.Invoke(New SetTitleCallBack(AddressOf setTitle), enFormTitle)
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: setTitle::Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Sub DisplayForm()
        Try
            If Not Me.InvokeRequired Then
                Me.Show()
            Else
                Me.Invoke(New DisplayFormCallBack(AddressOf DisplayForm))
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: DisplayForm:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Sub DisposeForm()
        Try
            If Not Me.InvokeRequired Then
                Me.Close()
            Else
                Me.Invoke(New DisposeFormCallBack(AddressOf DisposeForm))
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: DisposeForm:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Public Sub SetCurrentStatus(ByVal strTextToSet As String)
        Try
            If Not Me.InvokeRequired Then
                lblProcessindicator.Text = strTextToSet
                Me.Refresh()
            Else
                Me.Invoke(New SetCurrentStatusCallback(AddressOf SetCurrentStatus), strTextToSet)
            End If
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: SetCurrentStatus:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
    Private Sub frmDownloadReferenceData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            lblConnectionLost1.Visible = False
            lblConnectionLost2.Visible = False
            lblDBUpdation.Visible = False
            lblPercentage.Visible = False
            pgBar.Visible = False
        Catch ex As Exception
            AppContainer.GetInstance().obLogger.WriteAppLog("frmDownloadReferenceData:: frmDownloadReferenceData_Load:: Exception Occured, Message: " & ex.Message & ex.StackTrace, _
                                                            Logger.LogLevel.ERROR)
        End Try
    End Sub
End Class