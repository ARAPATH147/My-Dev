Public Class frmPSWSummary

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        Try
            FreezeControls()
            PSWSessionMgr.GetInstance().EndSession()
            WorkflowMgr.GetInstance().ExecQuit()
        Catch ex As Exception

            'Handle Exit Exception here.
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