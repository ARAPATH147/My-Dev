Public Class frmCLBackShopProductCount

  
    Private Sub cmbMultiSite_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbMultiSite.SelectedIndexChanged
        If Not CLSessionMgr.GetInstance.SelectedPOGSeqNum.Equals(cmbMultiSite.SelectedIndex.ToString()) Then
            'Process multi site select on selecting a differnent site
            CLSessionMgr.GetInstance.SelectedPOGSeqNum = cmbMultiSite.SelectedIndex.ToString()
            If CLSessionMgr.GetInstance().ProcessMultiSiteSelect() Then
                Me.Visible = False
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        End If
    End Sub

    Private Sub Btn_CalcPad_small2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small2.Click
        Try
            FreezeControls()
            Dim objSftKeyPad As New frmCalcPad(lblShelfQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                If CLSessionMgr.GetInstance().m_bIsCreateOwnList Then
                    CLSessionMgr.GetInstance().ProcessCOLItemCounted(lblProductCodeDisplay.Text, lblShelfQty.Text, lblBootsCodeDisplay.Text)
                Else
                CLSessionMgr.GetInstance().ProcessBackShopItemCounted(lblProductCodeDisplay.Text, lblShelfQty.Text, lblBootsCodeDisplay.Text)
                End If
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

    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.Btn_CalcPad_small2.Enabled = False
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
            Me.custCtrlBtnQuit.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.custCtrlBtnNext.Enabled = True
            Me.custCtrlBtnBack.Enabled = True
            Me.Btn_CalcPad_small2.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If CLSessionMgr.GetInstance().ProcessItemDetailsQuit() Then
                Me.Visible = False
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

   
    Private Sub custCtrlBtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNext.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If Not CLSessionMgr.GetInstance().ProcessProductCountNext(Macros.SENDER_FORM_ACTION, lblProductCodeDisplay.Text, lblBootsCodeDisplay.Text, lblShelfQty.Text) Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            CLSessionMgr.GetInstance().ProcessZeroSelection(lblProductCodeDisplay.Text, lblBootsCodeDisplay.Text)
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

    


    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.CUNTLIST)
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

   
    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If Not CLSessionMgr.GetInstance().ProcessProductCountBack() Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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

End Class