Public Class frmViewLogFileListScreen
#If NRF Then
    'Quit event
    'Handling the click event of the list item
    Private Sub lstvwLogFiles_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstvwLogFiles.ItemActivate
        If LogFileMgr.GetInstance().ProcessFileSelection() Then
            Me.Visible = False
        End If

    End Sub
    Private Sub Quit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Quit.Click
        Cursor.Current = Cursors.WaitCursor
        LogFileMgr.GetInstance().EndSession()
        Me.Close()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        MessageBox.Show("Select a Log File from the list of log files. " + _
                        vbCr + "Files with exceptions are shown in Red &" + _
                        vbCr + "Files without exception are shown in Green ", _
                        "Help ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)

    End Sub
    Private Sub pb_SendAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pb_SendAll.Click
        Cursor.Current = Cursors.WaitCursor
        'Me.lblStatus.Visible = True
        'Me.lblStatus.Text = "Status: Sending log transaction START"
        LogFileMgr.GetInstance().SendAllLogFiles()
        'Me.lblStatus.Visible = False
        If LogFileMgr.GetInstance.GetFileInfo() Then
            LogFileMgr.GetInstance.DisplayScreen(LOG_SCREENS.Home)
        End If
        Me.Refresh()
        Cursor.Current = Cursors.Default
    End Sub

    Private Sub frmViewLogFileListScreen_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
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
#End If
End Class