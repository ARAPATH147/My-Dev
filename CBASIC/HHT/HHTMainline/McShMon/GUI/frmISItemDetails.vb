#If RF Then
Public Class frmISItemDetails
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Quit_small.Click
        objAppContainer.objLogger.WriteAppLog("Quit button click", Logger.LogLevel.INFO)
        Try
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'If (MessageBox.Show("Are you sure you wish to quit?", _
            '               "Confirmation", _
            '               MessageBoxButtons.YesNo, _
            '               MessageBoxIcon.Question, _
            '               MessageBoxDefaultButton.Button1) = (MsgBoxResult.Yes)) Then
            ISSessionManager.GetInstance().DisplayISScreen(ISSCREENS.Home)
            'Else
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.EMPTY)
            'End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Exception occured at Quit button click: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
        objAppContainer.objLogger.WriteAppLog("Exit Quit button Session", Logger.LogLevel.INFO)

    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_CalcPad_small.Click
        Try
            FreezeControls()
            Dim strPrdCode As String = lblBootsCode.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblBootsCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblBootsCode.Text
                lblBootsCode.Text = strPrdCode
                If strData.Equals("") Or strPrdCode.Equals(strData) Then

                Else
                    ISSessionManager.GetInstance().HandleScanData(strData, BCType.ManualEntry)
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

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Info_button_i.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.ITEMSALES)
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
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.btn_Info_button_i.Enabled = False
            Me.btn_CalcPad_small.Enabled = False
            Me.btn_Quit_small.Enabled = False
            Me.btn_Info_button_i.Enabled = False
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
            Me.btn_Info_button_i.Enabled = True
            Me.btn_CalcPad_small.Enabled = True
            Me.btn_Quit_small.Enabled = True
            Me.btn_Info_button_i.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmISItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance().StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class
#End If