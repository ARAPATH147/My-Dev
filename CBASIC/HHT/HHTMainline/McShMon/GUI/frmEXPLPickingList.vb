Public Class frmEXPLPickingList

    Private Sub btnSalesFloorCalcpad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSalesFloorCalcpad.Click
        Try
            Dim objSftKeyPad As New frmCalcPad(lblBackShopQty, CalcPadSessionMgr.EntryTypeEnum.Quantity)
            If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
                PLSessionMgr.GetInstance().ProcessEXPLEntry()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnNext.Click
        Try
            FreezeControls()
            PLSessionMgr.GetInstance().ProcessItemDetailsNext(Macros.SENDER_FORM_ACTION)
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnBack_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnBack.Click
        Try
            FreezeControls()
            PLSessionMgr.GetInstance().ProcessItemDetailsBack(Macros.SENDER_FORM_ACTION)
            UnFreezeControls()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub custCtrlBtnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles custCtrlBtnQuit.Click
        Try
            FreezeControls()
            PLSessionMgr.GetInstance().DisplayPLScreen(PLSessionMgr.PLSCREENS.Finish)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    'IT Internal
    ''' <summary>
    ''' Freeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub FreezeControls()
        Me.custCtrlBtnQuit.Enabled = False
        Me.custCtrlBtnNext.Enabled = False
        Me.custCtrlBtnBack.Enabled = False
        Me.btnSalesFloorCalcpad.Enabled = False
    End Sub
    ''' <summary>
    ''' UnFreeze all controls in the form
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UnFreezeControls()
        Me.custCtrlBtnQuit.Enabled = True
        Me.custCtrlBtnNext.Enabled = True
        Me.custCtrlBtnBack.Enabled = True
        Me.btnSalesFloorCalcpad.Enabled = True
    End Sub
End Class