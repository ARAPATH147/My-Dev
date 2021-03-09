Public Class frmGODespatch

    Private Sub btnDespatch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDespatch.Click
        Try
            FreezeControls()
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M5"), _
            "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, _
            MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.CUSTOM, "Writing Export data...")
                If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
#If NRF Then
                GOTransferMgr.GetInstance().GenerateExportData()
                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Summary)
#ElseIf RF Then
                    If GOTransferMgr.GetInstance().GenerateExportData() Then
                        GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Summary)
                    End If
#End If

                Else
#If NRF Then
                    GOSessionMgr.GetInstance().GenerateExportData()
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.GOSummary)
#ElseIf RF Then
                    If GOSessionMgr.GetInstance().GenerateExportData() Then
                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.GOSummary)
                    End If
#End If
                End If
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
            'If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
            '    "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
            '    MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
            '    GOSessionMgr.GetInstance().EndSession()
            '    WorkflowMgr.GetInstance().ExecQuit()
            'End If
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                GOTransferMgr.GetInstance().ClearData()
                GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Scan)
            Else
                GOSessionMgr.GetInstance().ClearData()
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
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