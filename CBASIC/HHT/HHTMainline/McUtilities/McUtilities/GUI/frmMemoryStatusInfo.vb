Public Class frmMemoryStatusInfo

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        MemStatusMgr.GetInstance().EndSession()
    End Sub

    Private Sub frmMemoryStatusInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub
End Class