Public Class frmProcessLogFile
#If NRF Then
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        LogFileMgr.GetInstance().DisplayScreen(LOG_SCREENS.Home)
    End Sub
    Private Sub Quit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Quit.Click
        LogFileMgr.GetInstance.EndProcessLogFile()
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        LogFileMgr.GetInstance().ProcessView()
    End Sub

    Private Sub PictureBox3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox3.Click
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
        Quit.Visible = False
        Quit.Enabled = False
        lblSend.Text = "Sending File to the controller" + vbCr + "Please Wait..."
        lblSend.Visible = True
        Refresh()
        If LogFileMgr.GetInstance.SendLogFile() Then
            MessageBox.Show("The Log File has been Sucessfully Send.", _
                            "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
            LogFileMgr.GetInstance.RefreshlogFileList()
            LogFileMgr.GetInstance.EndProcessLogFile()
        Else
            lblSend.Visible = False
            Quit.Visible = True
            Quit.Enabled = True
        End If
        Cursor.Current = Cursors.Default
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    End Sub

    Private Sub frmProcessLogFile_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
        lblSend.Visible = False
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click

        If MessageBox.Show("Are you sure you want to delete this file?", "Alert ", _
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Quit.Visible = False
            Quit.Enabled = False
            lblSend.Text = "Deleting the log File " + vbCr + "Please Wait..."
            lblSend.Visible = True
            Refresh()
            If LogFileMgr.GetInstance.DeleteFile() Then
                If MessageBox.Show("File Deleted Successfully", _
                                   "Alert", _
                                   MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.OK Then
                    LogFileMgr.GetInstance.RefreshlogFileList()
                End If
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            LogFileMgr.GetInstance.EndProcessLogFile()
        End If

    End Sub
#End If
End Class