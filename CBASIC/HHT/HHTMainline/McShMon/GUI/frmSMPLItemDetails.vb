Public Class frmSMPLItemDetails

    Private Sub btnBackShopCalcpad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBackShopCalcpad.Click
        Try
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            FreezeControls()
            Dim objSftKeyPad As New frmCalcPad(lblBackShopQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                PLSessionMgr.GetInstance().ProcessSMPLEntry()
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

    Private Sub custCtrlBtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNext.Click
        Try
            FreezeControls()
            If CInt(Me.lblBackShopQty.Text) = 0 And _
            PLSessionMgr.GetInstance().IsScanned Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Alert", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)

            Else
                If CInt(Me.lblBackShopQty.Text) = 0 And Me.lblQtyReqHeader.Visible = False Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                    PLSessionMgr.GetInstance().ProcessSMPLItemDetailsNext()
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
                Else
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                    PLSessionMgr.GetInstance().ProcessItemDetailsNext(Macros.SENDER_FORM_ACTION)
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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

    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessItemDetailsBack(Macros.SENDER_FORM_ACTION)
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

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessItemDetailsQuit()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.PICKGLST)
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

    Private Sub frmSMPLItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'BCReader.GetInstance().StartRead()
        UnFreezeControls()
    End Sub

    Private Sub frmSMPLItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        'BCReader.GetInstance().StopRead()
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnBack.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.custCtrlBtnQuit.Enabled = False
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
            Me.custCtrlBtnBack.Enabled = True
            Me.custCtrlBtnNext.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.custCtrlBtnQuit.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Private Sub btn_OSSRItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSRItem.Click
        Try
            PLSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                      objAppContainer.objHelper.UnFormatBarcode(lblBootsCodeDisplay.Text.ToString()), _
                      objAppContainer.objHelper.UnFormatBarcode(lblProductCodeDisplay.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If

    Private Sub frmSMPLItemDetails_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
        If (e.KeyCode = System.Windows.Forms.Keys.Up) Then
            'Up
        End If
        If (e.KeyCode = System.Windows.Forms.Keys.Down) Then
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
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try

    End Sub
#If NRF Then
        'Auto Fast Fill PL CR
    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.ItemView)

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If

    Private Sub lblLocationHeader_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub


    Private Sub cmbLocation_SelectedIndexChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbLocation.SelectedIndexChanged
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            'checks whether site is changed
            If (cmbLocation.SelectedIndex()) <> PLSessionMgr.GetInstance().SelectedSite Then

                PLSessionMgr.GetInstance().m_SelectedPOG = UCase(cmbLocation.SelectedItem.ToString())
                'nan removed
                'set the global variable that tracks which site is currently selected
                'here there is no "Select" this is why +1
                PLSessionMgr.GetInstance().SelectedSite = cmbLocation.SelectedIndex()
                PLSessionMgr.GetInstance().GetSiteConfirmation()

            End If
            'nan  PLSessionMgr.GetInstance().ProcessLocationChange()
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

    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Try
            FreezeControls()
            If PLSessionMgr.GetInstance().IsScanned Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Alert", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
            Else
                PLSessionMgr.GetInstance().bIsGap = True
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
                PLSessionMgr.GetInstance().ZeroPressed() = 2
                PLSessionMgr.GetInstance().ProcessGap(lblProductCodeDisplay.Text)
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
End Class