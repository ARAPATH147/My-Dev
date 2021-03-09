Public Class frmCLSummary

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
#If RF Then
            'If Not CLSessionMgr.GetInstance().EndSession() Then
            If Not CLSessionMgr.GetInstance().ProcessCLSummaryOK() Then
                UnFreezeControls()
            End If
#ElseIf NRF Then
            CLSessionMgr.GetInstance().ProcessCLSummaryOK()
#End If

        Catch ex As Exception
            'Handle Exit Exception here.
            objAppContainer.objLogger.WriteAppLog("Count List: Exception in OK button click " _
                                                  & "in Summary screen" + ex.StackTrace, _
                                                  Logger.LogLevel.RELEASE)
            Return
        Finally
            UnFreezeControls()
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
            Me.btnOK.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze All Controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.btnOK.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class