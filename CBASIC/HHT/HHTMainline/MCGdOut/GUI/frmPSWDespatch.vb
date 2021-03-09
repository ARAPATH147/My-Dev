Public Class frmPSWDespatch

    Private Sub btnDespatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDespatch.Click
        Try
            FreezeControls()
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing Export data...")

            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M5"), _
            "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, _
            MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing Export data...")
#If NRF Then
                PSWSessionMgr.GetInstance().GenerateExportData()
                PSWSessionMgr.GetInstance().DisplayPSWScreen(PSWSessionMgr.PSWSCREENS.Summary)
#ElseIf RF Then
                If PSWSessionMgr.GetInstance().GenerateExportData() Then
                    PSWSessionMgr.GetInstance().DisplayPSWScreen(PSWSessionMgr.PSWSCREENS.Summary)
                End If
#End If
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                 "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                 MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                PSWSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnDespatch.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Freeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.btnQuit.Enabled = True
            Me.btnDespatch.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class