Public Class frmPLItemConfirm
    Private bStopScan As Boolean = False
    Private Sub Info_button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.PICKGLST)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            bStopScan = True
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
            PLSessionMgr.GetInstance().ProcessItemConfirmNext()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            bStopScan = True
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
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            FreezeControls()
            Dim strPrdCode As String = lblProductCodeDisplay.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblProductCodeDisplay, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblProductCodeDisplay.Text
                lblProductCodeDisplay.Text = strPrdCode
                ' If Not bIsQuit Then
                If strData.Equals("") Or strData.Equals(strPrdCode) Then

                Else
                    PLSessionMgr.GetInstance().HandleScanData(strData, BCType.ManualEntry)
                End If
            End If

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            bStopScan = True
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
            PLSessionMgr.GetInstance().ProcessItemConfirmBack()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            bStopScan = True
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
            PLSessionMgr.GetInstance().ProcessItemConfirmQuit()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            bStopScan = True
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
    Private Sub frmPLItemConfirm_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StartRead()
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

    Private Sub frmPLItemConfirm_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            If bStopScan Then
                BCReader.GetInstance().StopRead()
            End If
            bStopScan = True
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub Btn_GAP1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_GAP1.Click
        Try
            FreezeControls()
            'Fix for FF and Ex PL
            PLSessionMgr.GetInstance().bIsGap = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            PLSessionMgr.GetInstance().ZeroPressed() = 1
            PLSessionMgr.GetInstance().ProcessGap(lblProductCodeDisplay.Text)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            bStopScan = True
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
            Me.custCtrlBtnQuit.Enabled = False
            Me.Btn_GAP1.Enabled = False
            Me.Info_button.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.Btn_CalcPad_small1.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
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
            Me.Btn_GAP1.Enabled = True
            Me.Info_button.Enabled = True
            Me.custCtrlBtnBack.Enabled = True
            Me.Btn_CalcPad_small1.Enabled = True
            Me.custCtrlBtnNext.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    'AFF PL CR
    Private Sub btn_View_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_View.Click
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

    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub cmbLocation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbLocation.SelectedIndexChanged
        'set the global variable that tracks which site is currently selected
        PLSessionMgr.GetInstance().SelectedSite = cmbLocation.SelectedIndex()
        PLSessionMgr.GetInstance().CheckSelectedSite()

    End Sub
End Class