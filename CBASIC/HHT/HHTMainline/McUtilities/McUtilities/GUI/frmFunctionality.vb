Public Class frmFunctionality
#If NRF Then
    Private Sub btnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Close()
    End Sub

    Private Sub LinkLabel1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LinkLabel1.Click
        ExportDataViewer.getinstance.processdata(1)
    End Sub

    Private Sub LinkLabel2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LinkLabel2.Click
        ExportDataViewer.getinstance.processdata(2)
    End Sub

    Private Sub LinkLabel3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LinkLabel3.Click
        ExportDataViewer.getinstance.processdata(3)
    End Sub

    Private Sub frmFunctionality_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub

    Private Sub Quit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Quit.Click
        Me.Close()
    End Sub
#End If
End Class