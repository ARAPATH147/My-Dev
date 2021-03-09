Public Class frmIIItemDetails
    Private Sub frmIIItemDetails_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        'Me.CustomStatusBar1.BatteryTimer.Enabled = False
    End Sub

    Private Sub frmIIItemDetails_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'BTCDBDataSet.DealList' table. You can move, or remove it, as needed.
    End Sub
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
            BCReader.GetInstance().StartRead()
            '#If RF Then
            '            If Not ItemInfoSessionMgr.GetInstance().EndSession() Then
            '                UnFreezeControls()
            '            End If
            '#ElseIf NRF Then
            '            ItemInfoSessionMgr.GetInstance().EndSession()
            '#End If
            ItemInfoSessionMgr.GetInstance().DisplayIIScreen(ItemInfoSessionMgr.IISCREENS.Home)
            'Added for RF Mode -Lakshmi
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            Dim strPrdCode As String = lblBootsCode.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblBootsCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblBootsCode.Text
                lblBootsCode.Text = strPrdCode
                'Me.Refresh()
                Cursor.Current = Cursors.WaitCursor
                If Not strData.Equals("") Then
                    ItemInfoSessionMgr.GetInstance().HandleScanData(strData, BCType.ManualEntry)
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

    Private Sub cmbDeal_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbDeal.SelectedIndexChanged
        Try
            ItemInfoSessionMgr.GetInstance().DisplayDealInfo()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub Btn_Print1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Print1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
#If NRF Then
            'Queue print request after displaying the message.
            ItemInfoSessionMgr.GetInstance().PrintProcess()
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M43"), "Info", _
                              MessageBoxButtons.OK, _
                              MessageBoxIcon.Asterisk, _
                              MessageBoxDefaultButton.Button1)
#ElseIf RF Then
            MessageBox.Show(MessageManager.GetInstance().GetMessage("M92"), "Info", _
                              MessageBoxButtons.OK, _
                              MessageBoxIcon.Asterisk, _
                              MessageBoxDefaultButton.Button1)
            'Queue print request after displaying the message.
            ItemInfoSessionMgr.GetInstance().PrintProcess()
#End If
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
    'System Testing
    Private Sub frmIIItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'IT - Internal form not getting activated if returned from Planner
        objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
        BCReader.GetInstance().StartRead()
    End Sub
    Private Sub frmIIItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    Private Sub PlannerNew1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlannerNew1.Click
        Try
            FreezeControls()
            BCReader.GetInstance().StopRead()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'System Testing
            Dim strBootsCode As String = lblBootsCode.Text.Trim()
            strBootsCode = objAppContainer.objHelper.UnFormatBarcode(strBootsCode)
            If ItemInfoSessionMgr.GetInstance.IsInvokedFromPlanner() Then
                MessageBox.Show(MessageManager.GetInstance().GetMessage("M62"), "Info", _
                                     MessageBoxButtons.OK, _
                                     MessageBoxIcon.Asterisk, _
                                     MessageBoxDefaultButton.Button1)
            Else


#If RF Then
                ItemInfoSessionMgr.GetInstance.PostDetailsSent = True
#End If
            If (SPSessionMgr.GetInstance.StartSession(SPSessionMgr.PLANTYPE.SearchPlanner, True)) Then
                'SPSessionMgr.GetInstance.DisplaySPScreen(SPSessionMgr.SPSCREENS.SPHome)
                SPSessionMgr.GetInstance().DisplayPlanner(strBootsCode)
#If RF Then
                Else
                    objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
                End If
                ItemInfoSessionMgr.GetInstance.PostDetailsSent = False
#End If
#If NRF Then
                End If
#End If
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
#If NRF Then
            UnFreezeControls()
#End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
#If RF Then
            objAppContainer.objActiveModule = AppContainer.ACTIVEMODULE.ITEMINFO
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
            Me.Btn_CalcPad_small1.Enabled = False
            Me.Btn_Quit_small1.Enabled = False
            Me.PlannerNew1.Enabled = False
            Me.Btn_Print1.Enabled = False
            Me.Btn_CalcPad_small1.Enabled = False
#If RF Then
            Me.btn_OSSRItem.Enabled = False
             'Removed as part of SFA
            'Me.Btn_TSF1.Enabled = False
#End If
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
            Me.Btn_Quit_small1.Enabled = True
            Me.PlannerNew1.Enabled = True
            Me.Btn_Print1.Enabled = True
            Me.Btn_CalcPad_small1.Enabled = True
#If RF Then
            Me.btn_OSSRItem.Enabled = True
             'Removed as part of SFA
            'Me.Btn_TSF1.Enabled = True
#End If
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
            ItemInfoSessionMgr.GetInstance().DisplayOSSRToggle(lblOSSR, _
                   objAppContainer.objHelper.UnFormatBarcode(lblBootsCode.Text.ToString()))
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception :: " + ex.Message)
        End Try
    End Sub

     'Removed as part of SFA
'    Private Sub Btn_TSF1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_TSF1.Click
'        Try

'            Dim strTSF As String = lblStockText.Text
'#If RF Then
'            Dim objCalcPad As New frmCalcPad(lblStockText, CalcPadSessionMgr.EntryTypeEnum.TSF)
'#ElseIf NRF Then
'            Dim objCalcPad As New frmCalcPad(lblStockText, CalcPadSessionMgr.EntryTypeEnum.Quantity)
'#End If
'            If objCalcPad.ShowDialog() = DialogResult.OK Then
'                Dim strBootscode As String = objAppContainer.objHelper.UnFormatBarcode(lblBootsCode.Text.ToString())
'                If Not ItemInfoSessionMgr.GetInstance.AmendTSF(strBootscode, lblStockText.Text) Then
'                    lblStockText.Text = strTSF
'                End If
'            End If
'            objCalcPad = Nothing
'        Catch ex As Exception
'            objAppContainer.objLogger.WriteAppLog(ex.Message + " Occcured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
'        End Try
'    End Sub
#End If
End Class