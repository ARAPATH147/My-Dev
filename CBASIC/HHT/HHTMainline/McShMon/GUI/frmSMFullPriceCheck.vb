Public Class frmSMFullPriceCheck

    Private Sub frmSMFullPriceCheck_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        'Stop the barcode Reader here
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub frmSMFullPriceCheck_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'Start the barcode Reader here
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub Btn_CalcPad_small1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            FreezeControls()
            Dim strKeyedBarcode As String = Nothing
            Dim objSftKeyPad As New frmCalcPad(lblEAN, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            strKeyedBarcode = lblEAN.Text
            lblEAN.Text = ""
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                If Not ((lblEAN.Text = "") Or (lblEAN.Text = Nothing)) Then
                    SMSessionMgr.GetInstance.HandleScanData(lblEAN.Text, BCType.ManualEntry)
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

    Private Sub Btn_GAP1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_GAP1.Click
        Try
            'Check if the item is already scanned and retreive the details.
            FreezeControls()
            SMSessionMgr.GetInstance.GetProductFromList(Me.lblBtsCode.Text.Replace("-", ""))

            If Not SMSessionMgr.GetInstance.IsMultisited Then
                SMSessionMgr.GetInstance.ProcessSalesFlrQnty(True)
            Else
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M76"), _
                                "Info", MessageBoxButtons.OK, _
                                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                'Fix for Pilot support.
                SMSessionMgr.GetInstance.GetProductFromList(Me.lblBtsCode.Text.Replace("-", ""))
                'Display the item details screen.
                SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemDetails)
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

    Private Sub btnQuit_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            'Check if the item is already scanned and retreive the details.
            FreezeControls()
            SMSessionMgr.GetInstance.GetProductFromList(Me.lblBtsCode.Text.Replace("-", ""))
            SMSessionMgr.GetInstance.DisplaySMScreen(SMSessionMgr.SMSCREENS.ItemDetails)
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
            Me.Btn_CalcPad_small1.Enabled = False
            Me.Btn_GAP1.Enabled = False
            Me.btnQuit.Enabled = False
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
            Me.Btn_CalcPad_small1.Enabled = True
            Me.Btn_GAP1.Enabled = True
            Me.btnQuit.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class