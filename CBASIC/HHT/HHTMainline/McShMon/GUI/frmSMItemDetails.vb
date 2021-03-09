Public Class frmSMItemDetails

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
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
    Private Sub btnCalcPadSlsFlr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcPadSlsFlr.Click
        Try
            'Check if Location is selected for Multisited products
            FreezeControls()
            If Not (SMSessionMgr.GetInstance.CheckLocationSelection()) Then
                UnFreezeControls()
                Return
            End If
            Dim objSftKeyPad As New frmCalcPad(lblSlsflrVal, 0)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                If Not ((lblSlsflrVal.Text = "") Or (lblSlsflrVal.Text = Nothing)) Then
                    UnFreezeControls()
                    SMSessionMgr.GetInstance.ProcessSalesFlrQnty()
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
    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        Try
            'Check if Location is selected for Multisited products
            FreezeControls()
            If Not (SMSessionMgr.GetInstance.CheckLocationSelection()) Then
                UnFreezeControls()
                Return
            End If
            SMSessionMgr.GetInstance.ProcessSalesFlrQnty()
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

    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            SMSessionMgr.GetInstance().DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemView)
#If NRF Then
        objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#End If
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub cmbbxMltSites_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbbxMltSites.SelectedIndexChanged
        Try

            SMSessionMgr.GetInstance.m_SelectedPOGSeqNum = cmbbxMltSites.SelectedIndex.ToString().PadLeft(3, "0")
            SMSessionMgr.GetInstance.GetMultiSiteData()
            'Total item count for multisited items - SFA
            SMSessionMgr.GetInstance.TotalItemCount() = Convert.ToInt16(Me.lblTotalItemText.Text)
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
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btnQuit.Enabled = False
            Me.btnView.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.btnZero.Enabled = False
            Me.btnCalcPadSlsFlr.Enabled = False
            Me.btnNext.Enabled = False
            Me.cmbbxMltSites.Enabled = False
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
            Me.btnQuit.Enabled = True
            Me.btnView.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.btnZero.Enabled = True
            Me.btnCalcPadSlsFlr.Enabled = True
            Me.btnNext.Enabled = True
            Me.cmbbxMltSites.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Private Sub btn_OSSRItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSRItem.Click
        Try
            'ambli
            'For OSSR
            SMSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                   objAppContainer.objHelper.UnFormatBarcode(lblBtsCode.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Try
            FreezeControls()
            'Check if Location is selected for Multisited products
            If Not (SMSessionMgr.GetInstance.CheckLocationSelection()) Then
                UnFreezeControls()
                Return
            End If

            'If Not (Microsoft.VisualBasic.Val(lblSlsflrVal.Text) > 0) Then
            '    Me.lblSlsflrVal.Text = "0"
            UnFreezeControls()
            SMSessionMgr.GetInstance.ProcessSalesFlrQnty(True)
            'Else
            'MessageBox.Show("Item Count already entered. Cannot Enter Gap.", "Info", _
            '                MessageBoxButtons.OK, _
            '                MessageBoxIcon.Asterisk, _
            '                MessageBoxDefaultButton.Button1)
            'End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
        Finally
            UnFreezeControls()
#End If
        End Try
    End Sub
End Class