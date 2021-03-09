#If RF Then
Public Class frmSSSummary
    Private Sub btn_Ok_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Ok.Click
        Try
            FreezeControls()
#If RF Then
            If Not SSSessionManager.GetInstance().EndSession() Then
                UnFreezeControls()
            End If
#ElseIf NRF Then
            SSSessionManager.GetInstance().EndSession() 
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at OK button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            'Handle Exit Exception here.
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit OK button Session", Logger.LogLevel.INFO)
    End Sub
    ''' <summary>
    ''' To Freeze all the controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btn_Ok.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub UnFreezeControls()
        Try
            Me.btn_Ok.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
#End If