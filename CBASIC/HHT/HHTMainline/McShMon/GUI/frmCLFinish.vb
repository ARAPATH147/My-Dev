Public Class frmCLFinish
    Private Sub custCtrlBtnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnYes.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If Not CLSessionMgr.GetInstance().ProcessFinishYes() Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#If NRF Then
        UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub custCtrlBtnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNo.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If Not CLSessionMgr.GetInstance().ProcessFinishNo() Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnYes.Enabled = False
            Me.custCtrlBtnNo.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Freeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.custCtrlBtnYes.Enabled = True
            Me.custCtrlBtnNo.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Expection occured @ Unfreeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

   
    Private Sub Btn_Help_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Help.Click
        Try
            FreezeControls()
            MessageBox.Show(ControlChars.Tab & _
                      "SO - Support Office" & ControlChars.CrLf & ControlChars.CrLf & _
                                  ControlChars.CrLf, "Count List Type Help")
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
End Class