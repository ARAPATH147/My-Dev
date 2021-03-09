Public Class frmAutoSYSSummary

    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Ok1.Click
        Try
            AutoSYSSessionManager.GetInstance().EndSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class