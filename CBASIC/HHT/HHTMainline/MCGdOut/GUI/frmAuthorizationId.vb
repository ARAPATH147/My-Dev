Public Class frmAuthorizationId
    Private Sub objNumeric_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles objNumeric.KeyDown
        Try
            If e.KeyValue = Keys.Enter Then
                If objAppContainer.objHelper.ValidateAuthid(Me.objNumeric.txtNumeric.Text) Then
                    'Set the auth id
                    GOSessionMgr.GetInstance().SetAuthorizationID(Me.objNumeric.txtNumeric.Text)

                    'If the Item info array list is empty then the scenario is Fire/Flood damage
                    'hence launch scan screen else launch scan UOD list screen
                    If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.DESTROY _
                    And GOSessionMgr.GetInstance().GetUODItemCount() > 0 Then
                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.ItemView)
                    Else
                        GOSessionMgr.GetInstance().DisplayGOScreen(GOSessionMgr.GOSCREENS.Scan)
                    End If
                    Me.objNumeric.txtNumeric.Text = ""
                Else
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M45"), "Error", _
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
            'if the feature is Destroy the n display the scan screen else exit
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