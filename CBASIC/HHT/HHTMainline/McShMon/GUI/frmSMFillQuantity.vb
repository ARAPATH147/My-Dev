Public Class frmSMFillQuantity

    Private Sub btnCalcPadFillQnty_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcPadFillQnty.Click
        Try
            Dim objSftKeyPad As New frmCalcPad(lblFilQntyVal, 0)
            FreezeControls()
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                If Not ((lblFilQntyVal.Text = "") Or (lblFilQntyVal.Text = Nothing)) Then
                    SMSessionMgr.GetInstance.ProcessFillQnty()
                    UnFreezeControls()
                    'anoop
                    If (SMSessionMgr.GetInstance.IsMultisited()) Then
                        If (SMSessionMgr.GetInstance.IsAllMSCounted) Then
                            SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
                        Else
                            SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemDetails)
                        End If
                    Else
                        SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
                    End If

                End If
            End If
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

    Private Sub btnNext_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            FreezeControls()
            SMSessionMgr.GetInstance.ProcessFillQnty()
            'anoop
            If (SMSessionMgr.GetInstance.IsMultisited()) Then
                If (SMSessionMgr.GetInstance.IsAllMSCounted) Then
                    SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
                Else
                    SMSessionMgr.GetInstance.m_ItemActioned = True
                    SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemDetails)
                End If
            Else
                SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
            End If
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

    Private Sub btnView_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            SMSessionMgr.GetInstance().DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemView)
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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

    Private Sub btnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            FreezeControls()
            SMSessionMgr.GetInstance().m_SalesFloorQty = ""
            SMSessionMgr.GetInstance().DisplaySMScreen(SMSessionMgr.SMSCREENS.Home)
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

    Private Sub btnCalcPadPCEntry_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            Dim objSftKeyPad As New frmCalcPad(lblEAN, 2)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                SMSessionMgr.GetInstance.HandleScanData(lblEAN.Text, BCType.EAN)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.SHLFMNTR)
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

    Private Sub frmSMFillQuantity_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'BCReader.GetInstance.StopRead()
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnCalcPadFillQnty.Enabled = False
            Me.btnNext.Enabled = False
            Me.btnQuit.Enabled = False
            Me.btnView.Enabled = False
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
            Me.btnCalcPadFillQnty.Enabled = True
            Me.btnNext.Enabled = True
            Me.btnQuit.Enabled = True
            Me.btnView.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

End Class