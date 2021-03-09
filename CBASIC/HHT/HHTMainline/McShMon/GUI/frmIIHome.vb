Public Class frmIIHome
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            FreezeControl()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
#If RF Then
            If Not ItemInfoSessionMgr.GetInstance().EndSession() Then
                UnFreezeControl()
            End If
#ElseIf NRF Then
            ItemInfoSessionMgr.GetInstance().EndSession()
#End If

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
            Return
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)
    End Sub
    Private Sub frmIIHome_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StartRead()
            UnFreezeControl()
            'BCReader.GetInstance().EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub frmIIHome_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    Private Sub FreezeControl()
        Try
            Me.Btn_Quit_small1.Enabled = False
            Me.ProdSEL1.Btn_CalcPad_small1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub UnFreezeControl()
        Try
            Me.Btn_Quit_small1.Enabled = True
            Me.ProdSEL1.Btn_CalcPad_small1.Enabled = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class