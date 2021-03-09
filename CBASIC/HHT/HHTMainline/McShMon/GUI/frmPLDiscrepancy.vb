Public Class frmPLDiscrepancy


    
    Private Sub Help1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Help1.Click
        Try
            'If objAppContainer.objActiveModule.Equals(AppContainer.ACTIVEMODULE.SHLFMNTR) Then
            '* - Not all Multisites are Counted
            MessageBox.Show("Main back Shop counts are compulsory if a Pending Sales Plan site has been counted" & ControlChars.CrLf & ControlChars.CrLf & _
                                  ControlChars.CrLf, "Picking List Help")
            'End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub lstView_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.SelectedIndexChanged
        'FreezeControls()

        If (Me.lstView.Focused) Then
            PLSessionMgr.GetInstance().ProcessDispcrepancyItemSelect(Me.lstView.FocusedItem.Index)
        End If

        Me.Visible = False
        'UnFreezeControls()
    End Sub

    Private Sub FreezeControls()
        Try
            Me.Help1.Enabled = False
            Me.lstView.Enabled = False

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.Help1.Enabled = True
            Me.lstView.Enabled = True

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class