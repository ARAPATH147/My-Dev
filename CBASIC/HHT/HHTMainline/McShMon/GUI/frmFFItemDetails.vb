Public Class frmFFItemDetails

    Private Sub btnBarCodeCalcPad_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Dim strPrdCode As String = lblBootsCode.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblBootsCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblBootsCode.Text
                lblBootsCode.Text = strPrdCode
                If strData.Equals("") Or strPrdCode.Equals(strData) Then
                Else
                    'IT Bug Fix: Application crashes if boots code is entered on FF Item details screen
                    FFSessionMgr.GetInstance().HandleScanData(strData, BCType.ManualEntry)
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

    Private Sub Info_button_i1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.FASTFILL)
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

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            FreezeControls()
            Dim objSftKeyPad As New frmCalcPad(lblFillQtyText, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                'Updates the entered Fast Fill quantity
                'If Me.lblFillQtyText.Text.Trim() = "" Then
                '    Me.lblFillQtyText.Text = "0"
                'End If
#If NRF Then
            FFSessionMgr.GetInstance().UpdateProductInfo()
#ElseIf RF Then
                If FFSessionMgr.GetInstance().UpdateProductInfo() Then
#End If

                    'System Testing - To automatically come back to the home screen
                    'If Not (Me.lblFillQtyText.Text.Trim().Equals("")) And Not (Me.lblFillQtyText.Text.Trim().Equals("0")) Then
                    If Me.lblFillQtyText.Text.Trim() <> "" And Me.lblFillQtyText.Text.Trim() <> "0" Then
                        FFSessionMgr.GetInstance.DisplayFFScreen(FFSessionMgr.FFSCREENS.Home)
                    Else
                        FFSessionMgr.GetInstance.DisplayFFScreen(FFSessionMgr.FFSCREENS.ItemDetails)
                    End If
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If RF Then
                End If
#End If
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

    Private Sub Btn_Next_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Next_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            objAppContainer.objLogger.WriteAppLog("Next button click", Logger.LogLevel.INFO)
            'System testing
            If Me.lblFillQtyText.Text.ToString() = "0" Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"), "Info", _
                                MessageBoxButtons.OK, _
                                MessageBoxIcon.Asterisk, _
                                MessageBoxDefaultButton.Button1)
                FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.ItemDetails)
            Else
#If NRF Then
                FFSessionMgr.GetInstance().UpdateProductInfo()
#ElseIf RF Then
                If FFSessionMgr.GetInstance().UpdateProductInfo() Then
#End If

                    'System Testing - To automatically come back to the home screen
                    If Me.lblFillQtyText.Text.Trim() <> "" And Me.lblFillQtyText.Text.Trim() <> "0" Then
                        FFSessionMgr.GetInstance.DisplayFFScreen(FFSessionMgr.FFSCREENS.Home)
                    Else
                        FFSessionMgr.GetInstance.DisplayFFScreen(FFSessionMgr.FFSCREENS.ItemDetails)
                    End If
                    objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If RF Then
                End If
#End If
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit Next button Session", Logger.LogLevel.INFO)
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

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)

            FFSessionMgr.GetInstance().ProcessItemDetailsQuit()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)
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
            objAppContainer.objLogger.WriteAppLog("View button click", Logger.LogLevel.INFO)
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            FFSessionMgr.GetInstance().DisplayFFScreen(FFSessionMgr.FFSCREENS.ItemView)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            objAppContainer.objLogger.WriteAppLog("Exit View button Session", Logger.LogLevel.INFO)
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
            Me.Btn_CalcPad_small1.Enabled = False
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
            Me.btnQuit.Enabled = True
            Me.btnView.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.Btn_CalcPad_small1.Enabled = True
            Me.Btn_Next_small1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#If RF Then
    Private Sub btn_OSSRItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OSSRItem.Click
        Try
            FFSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                  objAppContainer.objHelper.UnFormatBarcode(lblBootsCode.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
#End If
End Class