Public Class frmPLSummary
    Private Sub custCtrlBtnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnOk.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
#If RF Then
            If Not PLSessionMgr.GetInstance().EndSession() Then
                UnFreezeControls()
            End If
#ElseIf NRF Then
  PLSessionMgr.GetInstance().EndSession()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at OK button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit OK button Session", Logger.LogLevel.INFO)
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnOk.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub UnFreezeControls()
        Try
            Me.custCtrlBtnOk.Enabled = False
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub


End Class