Imports System.Reflection
Public Class frmRLScanUOD
    Private SpaceOK As Boolean = False
    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            'RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ScanUOD
            'RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
            'objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            'Dim iResult As Integer = 0
            'iResult = MessageBox.Show(MessageManager.GetInstance().GetMessage("M38"), _
            '"Caution", MessageBoxButtons.OK, MessageBoxIcon.Hand, _
            'MessageBoxDefaultButton.Button1)
            'If iResult = MsgBoxResult.Ok Then
            'End If
            'Check if all items in COMPANY HO recall is actioned or else don't allow the user to exit.
            'If WorkflowMgr.GetInstance().objActiveFeature = WorkflowMgr.ACTIVEFEATURE.COMPANYHORECALL Then
            '    If RLSessionMgr.GetInstance().m_ActionedItemsInRecall > 0 Then
            '        MessageBox.Show(MessageManager.GetInstance().GetMessage("M71"), _
            '                        "Alert", MessageBoxButtons.OK, MessageBoxIcon.Question, _
            '                        MessageBoxDefaultButton.Button1)
            '        Return
            '    End If
            'End If
            'If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
            '"Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
            'MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then

            '    RLSessionMgr.GetInstance().EndSession()
            '    WorkflowMgr.GetInstance().ExecQuit()

            'End If
            'FIX FOR DEFCT 4990
            '  RLSessionMgr.GetInstance().RemoveActionsItemsInPrevUODs()

            objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.PLEASE_WAIT)
            RLSessionMgr.GetInstance().CallingScreen = RLSessionMgr.RECALLSCREENS.ItemList
            RLSessionMgr.GetInstance().DisplayRecallScreen(RLSessionMgr.RECALLSCREENS.Scan)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub txtBarcode_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtBarcode.KeyPress
        Try
            Dim numberFormatInfo As Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
            Dim decimalSeparator As String = numberFormatInfo.NumberDecimalSeparator
            Dim groupSeparator As String = numberFormatInfo.NumberGroupSeparator
            Dim negativeSign As String = numberFormatInfo.NegativeSign

            Dim keyInput As String = e.KeyChar.ToString()

            If [Char].IsDigit(e.KeyChar) Then
                ' Digits are OK
            ElseIf keyInput.Equals(decimalSeparator) OrElse keyInput.Equals(groupSeparator) OrElse keyInput.Equals(negativeSign) Then
                ' Decimal separator is OK
            ElseIf e.KeyChar = vbBack Then
                ' Backspace key is OK
                '    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                '    {
                '     // Let the edit control handle control and alt key combinations
                '    }
            ElseIf Me.SpaceOK AndAlso e.KeyChar = " "c Then

            Else
                ' Consume this invalid key and beep.
                e.Handled = True
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub txtBarcode_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtBarcode.KeyDown
        Try
            If e.KeyValue = Keys.Enter Then
                If txtBarcode.Text.Trim().Length > 0 Then
                    BCReader.GetInstance().RaiseScanEvent(txtBarcode.Text.ToString().Trim(), BCType.UODManualEntry)
                    txtBarcode.Text = ""
                End If
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Public ReadOnly Property IntValue() As Integer
        Get
            Return Int32.Parse(Me.Text)
        End Get
    End Property
    Public ReadOnly Property DecimalValue() As Decimal
        Get
            Return [Decimal].Parse(Me.Text)
        End Get
    End Property
    Public Property AllowSpace() As Boolean

        Get
            Return Me.SpaceOK
        End Get
        Set(ByVal value As Boolean)
            Me.SpaceOK = value
        End Set
    End Property
    Private Sub btnCalcpad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcpad.Click
        Try
            Dim objSftKeyPad As New frmCalcPad(txtBarcode, CalcPadSessionMgr.EntryTypeEnum.UOD)
            objSftKeyPad.ShowDialog()
            objSftKeyPad.Close()

            If txtBarcode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().RaiseScanEvent(txtBarcode.Text.ToString().Trim(), BCType.UODManualEntry)
                txtBarcode.Text = ""
                objStatusBar.SetMessage(CustomStatusBar.MSGTYPE.ACT_DATATTIME)
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub
    Private Sub frmRLScanUOD_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        'BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
        BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN13)
        BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
        BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN8)
        BCReader.GetInstance().StartRead()
    End Sub

    Private Sub frmRLScanUOD_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
        'BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN128)
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN13)
        BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN8)
    End Sub

End Class