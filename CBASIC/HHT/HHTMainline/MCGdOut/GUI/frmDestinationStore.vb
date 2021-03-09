Public Class frmDestinationStore

    Private Sub objNumeric_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles objNumeric.KeyDown
        Try
            If e.KeyValue = Keys.Enter Then
                If GOTransferMgr.GetInstance().ValidateStoreid(Me.objNumeric.txtNumeric.Text) Then
                    'Set the auth id
                    GOTransferMgr.GetInstance().SetStoreID(Me.objNumeric.txtNumeric.Text)

                    'Launch the Product scan screen screen
                    GOTransferMgr.GetInstance().DisplayGOTransferScreen(GOTransferMgr.GOTRANSFER.Scan)
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M53"), "Error", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Hand, _
                                    MessageBoxDefaultButton.Button1)
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY Then
                GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
            Else
                If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then

                    GOSessionMgr.GetInstance().EndSession()
                    WorkflowMgr.GetInstance().ExecQuit()

                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class