Public Class frmSummary
    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Try
            FreezeControls()
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.TRANSFERS Then
                GOTransferMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            ElseIf objAppContainer.bIsCreateRecalls Then
                WorkflowMgr.GetInstance().ExecPrev()
                GOSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
                'WorkflowMgr.GetInstance().ExecPrev()
                'RLSessionMgr.GetInstance().EndSession()
            Else
                GOSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            End If
            'WorkflowMgr.GetInstance().ExecQuit()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in OK button click " _
                                               & "in Summary screen" + ex.StackTrace, _
                                               Logger.LogLevel.RELEASE)
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
        Me.btnOk.Enabled = False
    End Sub
End Class