Public Class frmAutoSYSItemDetails

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            'AutoSYSSessionManager.GetInstance().QuitSession()
            AutoSYSSessionManager.GetInstance().DisplayASYSScreen(AutoSYSSessionManager.ASYSSCREENS.Home)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.AUTOSTUFFYOURSHELVES)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
            Cursor.Current = Cursors.Default
        End Try
    End Sub

    Private Sub frmAutoSYSItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            'Stop the barcode Reader here
            BCReader.GetInstance().StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmAutoSYSItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            'Start the barcode Reader here
            BCReader.GetInstance().StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    'Private Sub Btn_Back_sm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    AutoSYSSessionManager.GetInstance().DisplayASYSScreen(AutoSYSSessionManager.ASYSSCREENS.Home)
    'End Sub

    'Private Sub Btn_Previous1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Previous1.Click
    '    AutoSYSSessionManager.GetInstance().ProcessItemDetailsBack()
    'End Sub

    'Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    AutoSYSSessionManager.GetInstance().ProcessItemDetailsNext()
    'End Sub

    'Private Sub Btn_Previous31_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    AutoSYSSessionManager.GetInstance().ProcessItemDetailsBack()
    'End Sub

    Private Sub BtnView1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnView1.Click
        Try
            AutoSYSSessionManager.GetInstance().DisplayASYSScreen(AutoSYSSessionManager.ASYSSCREENS.View)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class