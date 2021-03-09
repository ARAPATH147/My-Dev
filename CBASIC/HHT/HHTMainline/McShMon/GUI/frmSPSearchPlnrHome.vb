Public Class frmSPSearchPlnrHome
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
            If Not SPSessionMgr.GetInstance.EndSession() Then
                UnFreezeControls()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub frmSPHome_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StartRead()
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
    Private Sub frmSPHome_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If SPSessionMgr.GetInstance.IsInvokedFromItemInfo() Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M63"), "Info", _
                                     MessageBoxButtons.OK, _
                                     MessageBoxIcon.Asterisk, _
                                     MessageBoxDefaultButton.Button1)
            Else
                ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.SPCEPLAN, True)
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
            Me.objProdSEL.Btn_CalcPad_small1.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.Btn_Quit_small1.Enabled = False
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
            Me.objProdSEL.Btn_CalcPad_small1.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.Btn_Quit_small1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

End Class