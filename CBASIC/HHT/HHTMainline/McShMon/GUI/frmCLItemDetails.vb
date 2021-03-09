Public Class frmCLItemDetails
    'Private bStopScan As Boolean = False
    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If CLSessionMgr.GetInstance().ProcessItemDetailsQuit() Then
                'Me.Text = ""
                Me.Visible = False
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'bStopScan = True
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
            CLSessionMgr.GetInstance.ProcessItemDetailsNext()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            'bStopScan = True

        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)

        Finally
            UnFreezeControls()
        End Try
    End Sub

    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            CLSessionMgr.GetInstance().ProcessItemDetailsBack()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)

            ' bStopScan = True
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
    Private Sub frmCLItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            Me.Focus()
            BCReader.GetInstance().StartRead()
            CLSessionMgr.GetInstance().CurrentScreen = CLSessionMgr.CLSCREENS.ItemDetails
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)

        Finally
            UnFreezeControls()
        End Try
    End Sub

    Private Sub frmCLItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            'If (CLSessionMgr.GetInstance().CurrentScreen = CLSessionMgr.CLSCREENS.ItemDetails) Then
            '    Me.Focus()
            '    Return
            'End If
            BCReader.GetInstance().StopRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            FreezeControls()
            If Not (CLSessionMgr.GetInstance.CheckLocationSelection()) Then
                UnFreezeControls()
                Return
            End If
            Dim strPrdCode As String = lblProductCodeDisplay.Text.Trim()
            Dim bIsQuit As Boolean = True
            Dim objSftKeyPad As New frmCalcPad(lblProductCodeDisplay, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            ' objSftKeyPad.ShowDialog()
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblProductCodeDisplay.Text
                lblProductCodeDisplay.Text = strPrdCode

                If strData.Equals("") Or strPrdCode.Equals(strData) Then

                Else
                    CLSessionMgr.GetInstance().HandleScanData(strData, BCType.ManualEntry)
                End If
            End If
            'bStopScan = True
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
            'bStopScan = True
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
            If Not (CLSessionMgr.GetInstance.CheckLocationSelection()) Then
                UnFreezeControls()
                Return
            End If
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            CLSessionMgr.GetInstance().ProcessZeroSelection(lblProductCodeDisplay.Text, lblBootsCodeDisplay.Text, Macros.COUNT_LIST_ITEMDETAILS)
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'bStopScan = True
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
            Me.btnZero.Enabled = False
            Me.Info_button_i1.Enabled = False
            Me.custCtrlBtnNext.Enabled = False
            Me.custCtrlBtnBack.Enabled = False
            Me.Btn_CalcPad_small1.Enabled = False
            Me.cmbMultiSite.Enabled = False
            Me.btn_View.Enabled = False
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Freeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Try
            Me.btn_View.Enabled = True
            Me.custCtrlBtnQuit.Enabled = True
            Me.btnZero.Enabled = True
            Me.Info_button_i1.Enabled = True
            Me.custCtrlBtnNext.Enabled = True
            Me.custCtrlBtnBack.Enabled = True
            Me.Btn_CalcPad_small1.Enabled = True
            Me.cmbMultiSite.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception Occured @ Unfreeze Controls " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub cmbMultiSite_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbMultiSite.SelectedIndexChanged

        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            CLSessionMgr.GetInstance.SelectedPOGSeqNum = cmbMultiSite.SelectedIndex.ToString()

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

    Private Sub btn_View_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_View.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            If CLSessionMgr.GetInstance().ProcessViewList() Then
                Me.Visible = False
            End If
            ' bStopScan = True
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

    'Private Sub frmCLItemDetails_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
    '    If (e.KeyCode = System.Windows.Forms.Keys.Up) Then
    '        'Up
    '    End If
    '    If (e.KeyCode = System.Windows.Forms.Keys.Down) Then
    '        'Down
    '    End If
    '    If (e.KeyCode = System.Windows.Forms.Keys.Left) Then
    '        'Left
    '    End If
    '    If (e.KeyCode = System.Windows.Forms.Keys.Right) Then
    '        'Right
    '    End If
    '    If (e.KeyCode = System.Windows.Forms.Keys.Enter) Then
    '        'Enter
    '    End If

    'End Sub

    Private Sub frmCLItemDetails_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'BCReader.GetInstance().StartRead()
        'Application.DoEvents()
        'Me.Update()
        'Me.Refresh()

    End Sub
End Class