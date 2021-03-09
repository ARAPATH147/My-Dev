Public Class frmCCSummary

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Try
            FreezeControls()
            CCSessionMgr.GetInstance().EndSession()
            WorkflowMgr.GetInstance().ExecQuit()
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
        Try
            Me.btnOk.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception @ Freeze Controls: " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class