Public Class frmIPData

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOk.Click
        IPInfoMgr.GetInstance().EndSession()
    End Sub


    Private Sub frmIPData_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
    End Sub

    Private Sub frmIPData_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyCode = System.Windows.Forms.Keys.Up) Then
            'Up
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Down) Then
            'Down
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Left) Then
            'Left
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Right) Then
            'Right
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Enter) Then
            'Enter
        End If

    End Sub
End Class