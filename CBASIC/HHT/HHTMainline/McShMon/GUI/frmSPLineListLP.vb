Public Class frmSPLineListLP
    ''' <summary>
    ''' Hanldes item selected in line list screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub lstView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.ItemActivate
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If (SPSessionMgr.GetInstance().ProcessProductSelectionForLineItemLP()) Then
                Me.Visible = False
            Else
                Me.Visible = True
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
    ''' Handles Back button click in line list screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'System Testing - Case where only one module is present needs to be handled
            If (SPSessionMgr.GetInstance.IsSingleModule()) Then
                SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.PlannerListLP)
            Else
                SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.ModuleListLP)
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
    ''' Handles Quit button click in line list screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
#If RF Then
            If Not SPSessionMgr.GetInstance().EndSession() Then
                UnFreezeControls()
            End If
#Else
            SPSessionMgr.GetInstance().EndSession()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Handles Print button click in line list screen. This option allows printing SEL
    ''' for the entire list of items in the planner.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Btn_Print1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Print1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
#If RF Then
            SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.PrintSEL)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#Else
            SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.PrintSEL)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
    ''' To invoke Item Info functionality from line list screen.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
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
                ItemInfoSessionMgr.GetInstance().StartSession(objAppContainer.objActiveModule, True)
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
            Me.Btn_Quit_small1.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.Btn_Print1.Enabled = False
            Me.Info_button_i1.Enabled = False
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
            Me.custCtrlBtnBack.Enabled = True
            Me.Btn_Print1.Enabled = True
            Me.Info_button_i1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class