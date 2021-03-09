#If RF Then

Public Class frmNoReports
    Private Sub Btn_Ok1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Ok1.Click
        Try
            ReportsSessionManager.GetInstance().EndSession()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception in ok button click of Reports module" + ex.StackTrace(), Logger.LogLevel.RELEASE)
        Finally
            objAppContainer.objLogger.WriteAppLog("Exits ok button click", Logger.LogLevel.INFO)
        End Try
    End Sub
End Class
#End If