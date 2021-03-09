''' ***************************************************************************
''' <fileName>frmPCHome.vb</fileName>
''' <summary>The Print SEL Item Details Screen. The Item details are displayed here.
''' </summary>
''' <Version>1.0</Version>
''' <Author>Infosys Technologies Ltd.</Author>
''' <DateModified>21-Nov-2008</DateModified>
''' <Platform>Visual Basic, MS .Net CF 3.5 for MC70</Platform> 
''' <CopyRight>Boots the Chemists Ltd, Boots UK 2008</CopyRight>
'''*****************************************************************************
Public Class frmPSItemDetails
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            FreezeControls()
            Dim strPrdCode As String = lblBootsCode.Text.Trim()
            Dim objSftKeyPad As New frmCalcPad(lblBootsCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                Dim strData As String = lblBootsCode.Text
                lblBootsCode.Text = strPrdCode

                BCReader.GetInstance.EventBCScannedHandler(strData, BCType.ManualEntry)
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

    Private Sub Btn_Print1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Print1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            PSSessionMgr.GetInstance().ProcessPrintSelection()
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

    Private Sub Btn_CalcPad_small2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small2.Click
        Try
            FreezeControls()
            Dim strTemp As String = ""
            strTemp = lblSELQty.Text.ToString()
            Dim objCalcPad = New frmCalcPad(lblSELQty, CalcPadSessionMgr.EntryTypeEnum.PrintQuantity)
            'If Quit is selected from calcpad, the previous count needs to be maintained
            If (lblSELQty.Text.ToString() = "") Then
                lblSELQty.Text = strTemp
            End If
            If objCalcPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                objCalcPad = Nothing
                If Not (CInt(lblSELQty.Text().ToString()) > 9) And (CInt(lblSELQty.Text().ToString()) <> 0) Then
                    PSSessionMgr.GetInstance.UpdateNumSELsQueued()
                    'IT Fixes
                    'ElseIf (CInt(lblSELQty.Text().ToString()) = 0) Then
                    '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M1"))
                    '    lblSELQty.Text = "1"
                    'Else
                    '    MessageBox.Show(MessageManager.GetInstance().GetMessage("M22"))
                    '    lblSELQty.Text = "1"
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

    Private Sub Info_button_i1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Info_button_i1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            ItemInfoSessionMgr.GetInstance().StartSession(AppContainer.ACTIVEMODULE.PRINTSEL)
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
    Private Sub frmPSItemDetails_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmPSItemDetails_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Info_button_i1.Enabled = False
            Me.Btn_CalcPad_small1.Enabled = False
            Me.Btn_CalcPad_small2.Enabled = False
            Me.Btn_Print1.Enabled = False
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
            Me.Info_button_i1.Enabled = True
            Me.Btn_CalcPad_small1.Enabled = True
            Me.Btn_CalcPad_small2.Enabled = True
            Me.Btn_Print1.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            PSSessionMgr.GetInstance().ProcessPrintSelection()
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