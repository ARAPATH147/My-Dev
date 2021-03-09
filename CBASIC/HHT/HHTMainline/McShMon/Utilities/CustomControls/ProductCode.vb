Public Class ProductCode
    'Private ProdSELReadNotifyHander As Global.System.EventHandler = Nothing
    'Public Event ProductCode_Event_Barcode_Scanned(ByVal barcode As String, ByVal BarcodeType As BCType)
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        'AddHandler _bcreader.evtBCScanned, AddressOf Me.ProductCode_Event_Barcode_Scanned

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
            If txtProductCode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().EventBCScannedHandler(txtProductCode.Text.ToString().Trim(), BCType.ManualEntry)
            End If
        End If
    End Sub

    Private Sub txtProductCode_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtProductCode.KeyDown
        If e.KeyValue = Keys.Enter Then
            If txtProductCode.Text.Trim().Length > 0 Then
                BCReader.GetInstance().EventBCScannedHandler(txtProductCode.Text.ToString().Trim(), BCType.ManualEntry)
            End If
        End If
    End Sub
End Class
