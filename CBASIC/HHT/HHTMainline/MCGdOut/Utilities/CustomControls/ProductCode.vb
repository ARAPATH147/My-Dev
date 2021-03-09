Imports System.Reflection
Public Class ProductCode
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click

        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        'BCReader.GetInstance.StopRead()
        'BCReader.GetInstance.EnableDecoder(Symbol.Barcode.DecoderTypes.CODE128)
        objSftKeyPad.ShowDialog()
        objSftKeyPad.Close()
        'BCReader.GetInstance.StartRead()
        'BCReader.GetInstance.DisableDecoder(Symbol.Barcode.DecoderTypes.I2OF5)
        If txtProductCode.Text.Trim().Length > 0 Then
            BCReader.GetInstance().RaiseScanEvent(txtProductCode.Text.ToString().Trim(), BCType.ManualEntry)
            txtProductCode.Text = ""
        End If

    End Sub

    Private Sub txtProductCode_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtProductCode.KeyDown
        If e.KeyValue = Keys.Enter Then
            If txtProductCode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().RaiseScanEvent(txtProductCode.Text.ToString().Trim(), BCType.ManualEntry)
                txtProductCode.Text = ""
            End If
        End If
    End Sub
End Class
