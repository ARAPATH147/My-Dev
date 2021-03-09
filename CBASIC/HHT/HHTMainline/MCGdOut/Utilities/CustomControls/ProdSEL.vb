Imports System.Reflection
Public Class ProdSEL
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Btn_CalcPad_small_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small.Click
        Dim objSftKeyPad As New frmCalcPad(txtProduct, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objSftKeyPad.ShowDialog()
        If txtProduct.Text.Trim().Length > 0 Then
            BCReader.GetInstance().EventBCScannedHandler(txtProduct.Text.ToString().Trim(), BCType.ManualEntry)
            txtProduct.Text = ""
        End If
    End Sub

    Private Sub txtProduct_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtProduct.KeyDown
        If e.KeyValue = Keys.Enter Then
            If txtProduct.Text.Trim().Length > 0 Then
                BCReader.GetInstance().RaiseScanEvent(txtProduct.Text.ToString().Trim(), BCType.ManualEntry)
                txtProduct.Text = ""
            End If
        End If
    End Sub

End Class
