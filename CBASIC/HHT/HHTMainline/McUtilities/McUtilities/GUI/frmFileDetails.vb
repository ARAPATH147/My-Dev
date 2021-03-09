Public Class frmFileDetails

   
    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        FileStatusMgr.GetInstance.EndSession()
    End Sub

   
    Private Sub frmFileDetails_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        FileStatusMgr.GetInstance.DisplayReferenceInfo()
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click
        FileStatusMgr.GetInstance.ActiveDataInfo()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        FileStatusMgr.GetInstance.ExportDataInfo()
    End Sub
End Class