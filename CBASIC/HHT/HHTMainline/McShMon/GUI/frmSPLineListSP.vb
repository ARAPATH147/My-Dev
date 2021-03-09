Public Class frmSPLineListSP
    Dim iCount As Integer
    Dim iIndex As Integer
    Private Sub lstView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.ItemActivate
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If (SPSessionMgr.GetInstance().ProcessProductSelectionForLineItemSP()) Then
                Me.Visible = False
            Else
                Me.Visible = True
            End If
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If

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

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'System Testing - Case where only one module is present needs to be handled
            If (SPSessionMgr.GetInstance.IsSingleModule()) Then
                SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.PlannerListSP)
            Else
                SPSessionMgr.GetInstance.DisplaySPScreen(SPSessionMgr.SPSCREENS.ModuleListSP)
            End If

#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If

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

    Private Sub Btn_Print1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Print1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.PrintSEL)
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

    
    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small1.Click
        Try
            'System Testing - Highlighting should be at the next instance of the same item.
            SPSessionMgr.GetInstance().HighlightNextItem()
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try

            SPSessionMgr.GetInstance().HighlightPreviousItem()
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

   
    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.SPCEPLAN, True)
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#Else
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
#End If
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
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_Quit_small1.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.Btn_Print1.Enabled = False
            Me.Btn_Next_small1.Enabled = False
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
            Me.Btn_Next_small1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class