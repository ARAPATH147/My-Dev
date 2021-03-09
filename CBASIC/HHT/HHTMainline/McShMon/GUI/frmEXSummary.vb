Public Class frmEXSummary

    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Ok1.Click
        Try
            FreezeControls()
#If RF Then
            If Not EXSessionMgr.GetInstance().EndSession() Then
                objAppContainer.objLogger.WriteAppLog("Into Unfreeze Controls", Logger.LogLevel.INFO)
                UnFreezeControls()
            End If
#ElseIf NRF Then
            EXSessionMgr.GetInstance().EndSession()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at OK button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            'Handle Exit Exception here.
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit OK button Session", Logger.LogLevel.INFO)
    End Sub
    ''' IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_Ok1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Unfreeze the OK Button
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UnFreezeControls()
        Try
            Me.Btn_Ok1.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class