#If RF Then
Public Class frmPLOSSRFinish
    Private Sub custCtrlBtnYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnYes.Click
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            PLSessionMgr.GetInstance().SendOSSRList(True)
            PLSessionMgr.GetInstance().ProcessFinish()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occurred @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNo.Click
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            PLSessionMgr.GetInstance().SendOSSRList(False)
            PLSessionMgr.GetInstance().ProcessFinish()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occurred @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub lblPLStatDisplay_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblPLStatDisplay.ParentChanged

    End Sub
End Class
#End If