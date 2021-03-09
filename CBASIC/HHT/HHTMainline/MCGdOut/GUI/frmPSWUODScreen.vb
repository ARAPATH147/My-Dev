Imports System.Reflection
Public Class frmPSWUODScreen
    Private SpaceOK As Boolean = False
    Private Sub frmPSWUODScreen_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        Try
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN13)
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
            BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.EAN8)
            BCReader.GetInstance().StartRead()
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmPSWUODScreen_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        Try
            BCReader.GetInstance().StopRead()
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN13)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.UPCA)
            BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.EAN8)
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub btnCalcpad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCalcpad.Click
        Try
            Dim objSftKeyPad As New frmCalcPad(txtBarcode, CalcPadSessionMgr.EntryTypeEnum.UOD)
            objSftKeyPad.ShowDialog()
            objSftKeyPad.Close()

            If txtBarcode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().RaiseScanEvent(txtBarcode.Text.ToString().Trim(), BCType.UODManualEntry)
            End If
            txtBarcode.Text = ""
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub txtBarcode_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtBarcode.KeyDown
        Try
            If e.KeyValue = Keys.Enter Then
                If txtBarcode.Text.Trim().Length > 0 Then
                    BCReader.GetInstance().RaiseScanEvent(txtBarcode.Text.ToString().Trim(), BCType.UODManualEntry)
                End If
                txtBarcode.Text = ""
            End If
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

    Private Sub btnQuit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnQuit.Click
        Try
            If MessageBox.Show(MessageManager.GetInstance().GetMessage("M3"), _
                   "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                   MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then
                PSWSessionMgr.GetInstance().EndSession()
                WorkflowMgr.GetInstance().ExecQuit()
            End If
        Catch ex As Exception
            objAppContainer.objLogger.WriteAppLog(ex.Message + "Occured @: " + ex.StackTrace, Logger.LogLevel.RELEASE)
        End Try
    End Sub

    Private Sub frmPSWUODScreen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class