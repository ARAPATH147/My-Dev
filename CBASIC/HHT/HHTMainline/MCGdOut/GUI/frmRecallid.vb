Public Class frmRecallid

    Private Sub objNumeric_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles objNumeric.KeyDown
        Try
            If e.KeyValue = Keys.Enter Then
                If objAppContainer.objHelper.ValidateAuthid(Me.objNumeric.txtNumeric.Text) Then
                    'Set the Recall id
                    GOSessionMgr.GetInstance().SetRecallid(Me.objNumeric.txtNumeric.Text)
                    GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                Else
                    MessageBox.Show("Invalid Recall Number", "Error", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                    Me.objNumeric.txtNumeric.Text = ""
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                 "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                 MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then

                GOSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()


            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub pbContextHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbContextHelp.Click
        Dim strRecallMsge = " Only Customer, Emergency and Withdrawn recall items" + _
        " where the recall is closed can go through Create a Recall." + _
         "#### Healthcare items under BC 55 can go through Create a Recall. " + _
        "Please use this recall number for these only (00000055)." + _
         "#### Any old items returned by customers that no longer have a till bar " + _
        "cannot be processed here. Please process through credit claiming."
        objAppContainer.objScreen.DisplayHelp(False, strRecallMsge)
        'Display the pop up message.
        '  MessageBox.Show(strRecallMsge, "Help", MessageBoxButtons.OK, _
        '                MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1)
    End Sub
End Class