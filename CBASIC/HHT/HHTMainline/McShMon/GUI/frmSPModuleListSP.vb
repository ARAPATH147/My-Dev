﻿Public Class frmSPModuleListSP
    Private Sub lstView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstView.ItemActivate
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If (SPSessionMgr.GetInstance().ProcessProductSelectionForModuleSP()) Then
                Me.Visible = False
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
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Dim iRes As Integer
            'If (SPSessionMgr.GetInstance().PrintProcessPRPForModules()) Then
            iRes = MessageBox.Show(MessageManager.GetInstance().GetMessage("M74"), "Info", _
                                 MessageBoxButtons.YesNo, _
                                 MessageBoxIcon.Asterisk, _
                                 MessageBoxDefaultButton.Button1)
            If iRes = MsgBoxResult.Yes Then
                SPSessionMgr.GetInstance().PrintProcessPRPForModules()
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M41"), "Info", _
                                        MessageBoxButtons.OK, _
                                        MessageBoxIcon.Asterisk, _
                                        MessageBoxDefaultButton.Button1)
            End If
            'End If
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
    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            SPSessionMgr.GetInstance().DisplaySPScreen(SPSessionMgr.SPSCREENS.PlannerListSP)
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
            Me.Info_button_i1.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.Btn_Print1.Enabled = False
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
            Me.Info_button_i1.Enabled = True
            Me.custCtrlBtnBack.Enabled = True
            Me.Btn_Print1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class