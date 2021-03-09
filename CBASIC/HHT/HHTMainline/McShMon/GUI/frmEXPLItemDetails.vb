Public Class frmEXPLItemDetails

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            PLSessionMgr.GetInstance().ProcessItemDetailsQuit()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
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
            If CInt(Me.lblCurrentQty.Text) = 0 And _
               PLSessionMgr.GetInstance().IsScanned Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M99"), "Alert", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Hand, _
                                MessageBoxDefaultButton.Button1)
            Else
                If CInt(Me.lblCurrentQty.Text) = 0 Then
                    MessageBox.Show(MessageManager.GetInstance().GetMessage("M102"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
                    PLSessionMgr.GetInstance().ProcessEXPLItemDetailsNext()
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

    Private Sub btnSalesFloorCalcpad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalesFloorCalcpad.Click
        Try
            FreezeControls()
            'Check if Location is selected for Multisited products
            'nan this check not required as a site will be selected by default
            'If Not (PLSessionMgr.GetInstance.CheckLocationSelection()) Then
            '    Return
            'End If

            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Dim objSftKeyPad As New frmCalcPad(lblCurrentQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                PLSessionMgr.GetInstance().ProcessEXPLEntry()
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

    Private Sub cmbLocation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbLocation.SelectedIndexChanged
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
    Private Sub frmEXPLItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            'BCReader.GetInstance().StartRead()
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmEXPLItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        'BCReader.GetInstance().StopRead()
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.btnSalesFloorCalcpad.Enabled = False
            Me.cmbLocation.Enabled = False
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
            Me.custCtrlBtnNext.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.custCtrlBtnBack.Enabled = True
            Me.btnSalesFloorCalcpad.Enabled = True
            Me.cmbLocation.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' Process when GAP button is pressed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    '    Private Sub Btn_GAP1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_GAP1.Click
    '        Try
    '            FreezeControls()
    '            'Check if Location is selected for Multisited products
    '            If Not (PLSessionMgr.GetInstance.CheckLocationSelection()) Then
    '                UnFreezeControls()
    '                Return
    '            End If

    '            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
    '            PLSessionMgr.GetInstance().ProcessEXPLEntry()
    '            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
    '#If NRF Then
    '        UnFreezeControls()
    '#End If
    '        Catch ex As Exception
    '            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
    '#If RF Then
    '        Finally
    '            UnFreezeControls()
    '#End If
    '        End Try
    '    End Sub
    '#If RF Then
    '    Private Sub btn_OSSRItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSRItem.Click
    '        PLSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
    '               objAppContainer.objHelper.UnFormatBarcode(lblBootsCodeDisplay.Text.ToString()))
    '    End Sub
    '#End If
#If RF Then
    Private Sub Btn_OSSRItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_OSSRItem1.Click
        Try
            PLSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                      objAppContainer.objHelper.UnFormatBarcode(lblBootsCodeDisplay.Text.ToString()), _
                     objAppContainer.objHelper.UnFormatBarcode(lblProductCodeDisplay.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
#End If
    'Auto Fast Fill PL CR
    Private Sub btn_View_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
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

    Private Sub objStatusBar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles objStatusBar.Click

    End Sub
    Private Sub lblOSSR_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblOSSR.ParentChanged

    End Sub
    Private Sub lblTotalItemQty_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblTotalItemQty.ParentChanged

    End Sub
    Private Sub lblBackshopHeader_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBackshopHeader.ParentChanged

    End Sub
    Private Sub lblCurrentQty_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblCurrentQty.ParentChanged

    End Sub
    Private Sub lblLocationHeader_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLocationHeader.ParentChanged

    End Sub
    Private Sub lblStockFigureHeader_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStockFigureHeader.ParentChanged

    End Sub
    Private Sub lblBootsCodeDisplay_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblBootsCodeDisplay.ParentChanged

    End Sub
    Private Sub lblProductCodeDisplay_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblProductCodeDisplay.ParentChanged

    End Sub
    Private Sub lblItemPosition_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblItemPosition.ParentChanged

    End Sub
    Private Sub lblStatus_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStatus.ParentChanged

    End Sub
    Private Sub lblStatusDisplay_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStatusDisplay.ParentChanged

    End Sub
    Private Sub lblProductDesc1_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblProductDesc1.ParentChanged

    End Sub
    Private Sub lblProductDesc2_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblProductDesc2.ParentChanged

    End Sub
    Private Sub lblProductDesc3_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblProductDesc3.ParentChanged

    End Sub
    Private Sub lblStockFigure_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblStockFigure.ParentChanged

    End Sub
    Private Sub lblSalesFloorHeader_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblSalesFloorHeader.ParentChanged

    End Sub
    Private Sub lblCounted_ParentChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblCounted.ParentChanged

    End Sub
    ''' <summary>
    ''' Process when Zero button is pressed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Try
            FreezeControls()
            'Check if Location is selected for Multisited products
            'If Not (PLSessionMgr.GetInstance.CheckLocationSelection()) Then
            '    UnFreezeControls()
            '    Return
            'End If

            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            PLSessionMgr.GetInstance().ZeroPressed() = 3
            PLSessionMgr.GetInstance().ProcessEXPLEntry()
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