Public Class frmCLFullPriceCheck
    Private Sub frmCLFullPriceCheck_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        'Stop the barcode Reader here
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub frmCLFullPriceCheck_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'Start the barcode Reader here
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Try
            FreezeControls()
            Dim strKeyedBarcode As String = Nothing
            Dim objSftKeyPad As New frmCalcPad(lblEAN, CalcPadSessionMgr.EntryTypeEnum.Barcode)
            strKeyedBarcode = lblEAN.Text
            lblEAN.Text = ""
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                If Not ((lblEAN.Text = "") Or (lblEAN.Text = Nothing)) Then
                    CLSessionMgr.GetInstance.HandleScanData(lblEAN.Text, BCType.ManualEntry)
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

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            If CLSessionMgr.GetInstance().DisplayCLScreen(CLSessionMgr.CLSCREENS.SalesFloorProductCount) Then
                Me.Visible = False
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

    Private Sub btnZero_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnZero.Click
        Try
            FreezeControls()
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PROCESSING)
            If CLSessionMgr.GetInstance().ProcessZeroSelection(lblEAN.Text, lblBtsCode.Text, Macros.COUNT_LIST_FULLPRICECHECK) Then
                Me.Visible = False
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
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Try
            Me.Btn_CalcPad_small1.Enabled = False
            Me.btnZero.Enabled = False
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
            Me.btnZero.Enabled = True
            Me.btnQuit.Enabled = True
            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog("Error while Freeze / Unfreezing Controls, " + ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
End Class