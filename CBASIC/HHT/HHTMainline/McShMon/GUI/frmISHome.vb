#If RF Then
Public Class frmISHome
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Quit_small.Click
        Try
            FreezeControl()
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'UAT DEFECT FIX - Item sales and reports - do not need the 'are you sure you wish to quit' message 
            'If (MessageBox.Show("Are you sure you wish to quit?", _
            '                   "Confirmation", _
            '                   MessageBoxButtons.YesNo, _
            '                   MessageBoxIcon.Question, _
            '                   MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
            If Not ISSessionManager.GetInstance().EndSession() Then

                UnFreezeControl()
            End If
            'Else
            'UnFreezeControl()
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            'End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControl()
#End If
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)
    End Sub
    Private Sub FreezeControl()
        Try
            Me.btn_Quit_small.Enabled = False
            Me.ProdSEL1.Btn_CalcPad_small1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub UnFreezeControl()
        Try
            Me.btn_Quit_small.Enabled = True
            Me.ProdSEL1.Btn_CalcPad_small1.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub frmISHome_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StartRead()
            UnFreezeControl()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try

    End Sub
    Private Sub frmISHome_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            BCReader.GetInstance().StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
#End If