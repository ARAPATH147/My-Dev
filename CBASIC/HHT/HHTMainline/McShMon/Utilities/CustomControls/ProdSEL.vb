Public Class ProdSEL
    'Private ProdSELReadNotifyHander As Global.System.EventHandler = Nothing
    'Public Event ProdSEL_Event_Barcode_Scanned(ByVal barcode As String, ByVal BarcodeType As BCType)
    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        'AddHandler _bcreader.evtBCScanned, AddressOf Me.ProdSEL_Event_Barcode_Scanned_Handler

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Btn_CalcPad_small1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        Dim objSftKeyPad As New frmCalcPad(txtProduct, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.Parent.Refresh()
            Cursor.Current = Cursors.WaitCursor
            If txtProduct.Text.Trim().Length > 0 Then
                BCReader.GetInstance().EventBCScannedHandler(txtProduct.Text.ToString().Trim(), BCType.ManualEntry)
            End If
        End If
    End Sub
    Private Sub txtProduct_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtProduct.KeyDown
        If e.KeyValue = Keys.Enter Then
            If txtProduct.Text.Trim().Length > 0 Then
                BCReader.GetInstance().EventBCScannedHandler(txtProduct.Text.ToString().Trim(), BCType.ManualEntry)
            End If
        End If
    End Sub
End Class
