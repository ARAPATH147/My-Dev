Public Class frmAutoSYSHomeScreen
    Private Sub frmAutoSYSHomeScreen_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            'Stop the barcode Reader here
            BCReader.GetInstance().StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmAutoSYSHomeScreen_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            'Start the barcode Reader here
            BCReader.GetInstance().StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
#If RF Then
            FreezeControls()
#End If
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
#If RF Then
            FreezeControls()
#End If
            AutoSYSSessionManager.GetInstance().QuitSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub

    Private Sub FreezeControls()
        Try
            Btn_Quit_small1.Enabled = False
            Info_button_i1.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error While Freezing / Unfreezing Controls", Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub UnFreezeControls()
        Try
            Btn_Quit_small1.Enabled = True
            Info_button_i1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error While Freezing / Unfreezing Controls", Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class