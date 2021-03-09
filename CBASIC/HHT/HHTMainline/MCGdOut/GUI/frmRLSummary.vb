Public Class frmRLSummary
    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Try

            FreezeControls()
            If RLSessionMgr.GetInstance().bCreateAnotherRecallUOD Then
                RLSessionMgr.GetInstance().IsActionedinPreviousUOD()
                'To rest the hash table containing actioned items for last session.
                RLSessionMgr.GetInstance.ClearActionedItems()
                'Display product scan screen to add rest of the items to another UOD.
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
            Else
                RLSessionMgr.GetInstance().ClearData()
                ' RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ActiveRecallList)
                RLSessionMgr.GetInstance().CallingScreen = Nothing
                ' RLSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            End If
            '  RLSessionMgr.GetInstance().bCreateAnotherRecallUOD = False
            RLSessionMgr.GetInstance().bIsRecallReturns = False
            ' RLSessionMgr.GetInstance().bRecallActionCompleted = False
            'RLSessionMgr.GetInstance().ClearData()
            'RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ActiveRecallList)
            objAppContainer.bIsAutoLogOffatSummary = False
#If NRF Then
            UnFreezeControls()
#End If

        Catch ex As Exception
            'Handle Exit Exception here.
            objAppContainer.objLogger.WriteAppLog("Exception in OK button click " _
                                                  & "in Summary screen" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return
#If RF Then
        Finally
            UnFreezeControls()
#End If
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
            Me.btnOk.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ freeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub UnFreezeControls()
        Try
            Me.btnOk.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Unfreeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class