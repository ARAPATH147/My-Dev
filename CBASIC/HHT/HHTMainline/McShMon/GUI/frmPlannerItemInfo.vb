Public Class frmPlannerItemInfo
    ''' <summary>
    ''' Handles the Print button click.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Print1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Print1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            'Print SEL for the selected item.
            SPSessionMgr.GetInstance().PrintProcessPRT()
#If RF Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M92"), "Info", _
                                  MessageBoxButtons.OK, _
                                  MessageBoxIcon.Asterisk, _
                                  MessageBoxDefaultButton.Button1)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#ElseIf NRF Then
             MessageBox.Show(MessageManager.GetInstance().GetMessage("M43"), "Info", _
                                   MessageBoxButtons.OK, _
                                   MessageBoxIcon.Asterisk, _
                                   MessageBoxDefaultButton.Button1)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#End If
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Handles deal selection from the drop down liat.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbDeal_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbDeal.SelectedIndexChanged
        Try
            'Invoke function to display the deal information for the selected deal.
            SPSessionMgr.GetInstance().DisplayDealInfo()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occurred @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Process Quit button selection.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            'Set the previous sub module list screen visible.
            SPSessionMgr.GetInstance().SetPrevScreenVisible()
            Me.Visible = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occurred @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Process Planner button selection.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub PlannerNew1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlannerNew1.Click
        Try
            FreezeControls()
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M62"), "Info", _
                                    MessageBoxButtons.OK, _
                                    MessageBoxIcon.Asterisk, _
                                    MessageBoxDefaultButton.Button1)
#If NRF Then
        UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_Quit_small1.Enabled = False
            Me.PlannerNew1.Enabled = False
            Me.Btn_Print1.Enabled = False
            Me.cmbDeal.Enabled = False
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
            Me.Btn_Quit_small1.Enabled = True
            Me.PlannerNew1.Enabled = True
            Me.Btn_Print1.Enabled = True
            Me.cmbDeal.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class