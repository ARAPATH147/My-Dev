Public Class frmSpclInstructions

    Private Sub btnNext_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        FreezeControls()
        RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
        UnFreezeControls()
    End Sub

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        'RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ActiveRecallList)
        'RLSessionMgr.GetInstance().EndSession()
        ' WorkflowMgr.GetInstance().ExecQuit()
        Try
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                   "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                   MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                RLSessionMgr.GetInstance().CallingScreen = Nothing
                'RLSessionMgr.GetInstance().EndSession()
                'WorkflowMgr.GetInstance().ExecQuit()
#If RF Then
                RLSessionMgr.GetInstance().SendRecallExitForQuit()
#End If
                RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.ActiveRecallList)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Me.btnQuit.Enabled = False
        Me.btnNext.Enabled = False
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Me.btnQuit.Enabled = True
        Me.btnNext.Enabled = True
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        MessageBox.Show("", "Info")
    End Sub

    Private Sub frmSpclInstructions_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If (e.KeyCode = System.Windows.Forms.Keys.Up) Then
            'Rocker Up
            'Up
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Down) Then
            'Rocker Down
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