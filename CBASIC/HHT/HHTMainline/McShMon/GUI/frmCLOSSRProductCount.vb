#If RF Then
'ambli
'For OSSR
Public Class frmCLOSSRProductCount
    'Private Sub lblBackShopVal_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    'CLSessionMgr.GetInstance().ProcessBackShopItemCounted(lblProductCodeDisplay.Text, lblBackShopVal.Text)
    'End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.custCtrlBtnQuit.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
            Me.Btn_CalcPad_small2.Enabled = False
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
            Me.custCtrlBtnQuit.Enabled = True
            Me.custCtrlBtnBack.Enabled = True
            Me.custCtrlBtnNext.Enabled = True
            Me.Btn_CalcPad_small2.Enabled = True
            Me.Info_button_i1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    'Private Sub btn_OSSRItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    Try
    '        CLSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
    '                           objAppContainer.objHelper.UnFormatBarcode(lblBootsCodeDisplay.Text.ToString()))

    '    Catch ex As Exception
    '        objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
    '    End Try
    'End Sub
    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            CLSessionMgr.GetInstance().ProcessZeroSelection(lblProductCodeDisplay.Text, lblBootsCodeDisplay.Text)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)

        Finally
            UnFreezeControls()
        End Try
    End Sub

    Private Sub custCtrlBtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNext.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If Not CLSessionMgr.GetInstance().ProcessProductCountNext(Macros.SENDER_FORM_ACTION, lblProductCodeDisplay.Text, lblBootsCodeDisplay.Text, lblShelfQty.Text) Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub

    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            If Not CLSessionMgr.GetInstance().ProcessProductCountBack() Then
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If CLSessionMgr.GetInstance().ProcessItemDetailsQuit() Then
                Me.Visible = False
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub


    Private Sub Btn_CalcPad_small2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small2.Click
        Try
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            FreezeControls()
            Dim objSftKeyPad As New frmCalcPad(lblShelfQty, 0)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                If CLSessionMgr.GetInstance().m_bIsCreateOwnList Then
                    CLSessionMgr.GetInstance().ProcessCOLItemCounted(lblProductCodeDisplay.Text, lblShelfQty.Text, lblBootsCodeDisplay.Text)
                Else
                    CLSessionMgr.GetInstance().ProcessOSSRItemCounted(lblProductCodeDisplay.Text, lblShelfQty.Text, lblBootsCodeDisplay.Text)
                End If
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub

    Private Sub Info_button_i1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.CUNTLIST)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        Finally
            UnFreezeControls()
        End Try
    End Sub
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
End Class
#End If