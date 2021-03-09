Public Class frmBookInCartonExpectedOrder
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblDirRecv As System.Windows.Forms.Label
    Friend WithEvents lblExpectedOrders As System.Windows.Forms.Label
    Friend WithEvents lvwSuppliers As System.Windows.Forms.ListView
    Friend WithEvents lblSelect As System.Windows.Forms.Label
    Friend WithEvents Btn_Info1 As System.Windows.Forms.PictureBox

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwSuppliers.Font = New System.Drawing.Font("Courier New", 9.0!, FontStyle.Regular)
        Me.lvwSuppliers.Activation = ItemActivation.OneClick
        ' Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents btnEnterSupNo As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Supplier As System.Windows.Forms.ColumnHeader
    Friend WithEvents Qty As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblCode As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBookInCartonExpectedOrder))
        Me.lblDirRecv = New System.Windows.Forms.Label
        Me.lblExpectedOrders = New System.Windows.Forms.Label
        Me.lvwSuppliers = New System.Windows.Forms.ListView
        Me.Supplier = New System.Windows.Forms.ColumnHeader
        Me.Qty = New System.Windows.Forms.ColumnHeader
        Me.lblSelect = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.btnEnterSupNo = New System.Windows.Forms.PictureBox
        Me.Btn_Info1 = New System.Windows.Forms.PictureBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.lblCode = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'lblDirRecv
        '
        Me.lblDirRecv.Location = New System.Drawing.Point(16, 16)
        Me.lblDirRecv.Name = "lblDirRecv"
        Me.lblDirRecv.Size = New System.Drawing.Size(112, 16)
        Me.lblDirRecv.Text = "Directs Receiving"
        '
        'lblExpectedOrders
        '
        Me.lblExpectedOrders.Location = New System.Drawing.Point(16, 40)
        Me.lblExpectedOrders.Name = "lblExpectedOrders"
        Me.lblExpectedOrders.Size = New System.Drawing.Size(112, 16)
        Me.lblExpectedOrders.Text = "Expected Orders"
        '
        'lvwSuppliers
        '
        Me.lvwSuppliers.Columns.Add(Me.Supplier)
        Me.lvwSuppliers.Columns.Add(Me.Qty)
        Me.lvwSuppliers.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwSuppliers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvwSuppliers.Location = New System.Drawing.Point(16, 64)
        Me.lvwSuppliers.Name = "lvwSuppliers"
        Me.lvwSuppliers.Size = New System.Drawing.Size(200, 90)
        Me.lvwSuppliers.TabIndex = 6
        Me.lvwSuppliers.View = System.Windows.Forms.View.Details
        '
        'Supplier
        '
        Me.Supplier.Text = "Supplier"
        Me.Supplier.Width = 140
        '
        'Qty
        '
        Me.Qty.Text = "Qty"
        Me.Qty.Width = 35
        '
        'lblSelect
        '
        Me.lblSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSelect.Location = New System.Drawing.Point(16, 161)
        Me.lblSelect.Name = "lblSelect"
        Me.lblSelect.Size = New System.Drawing.Size(136, 32)
        Me.lblSelect.Text = "Select supplier or scan/enter carton"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(166, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btnEnterSupNo
        '
        Me.btnEnterSupNo.Image = CType(resources.GetObject("btnEnterSupNo.Image"), System.Drawing.Image)
        Me.btnEnterSupNo.Location = New System.Drawing.Point(16, 232)
        Me.btnEnterSupNo.Name = "btnEnterSupNo"
        Me.btnEnterSupNo.Size = New System.Drawing.Size(130, 24)
        Me.btnEnterSupNo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Info1
        '
        Me.Btn_Info1.Image = CType(resources.GetObject("Btn_Info1.Image"), System.Drawing.Image)
        Me.Btn_Info1.Location = New System.Drawing.Point(184, 16)
        Me.Btn_Info1.Name = "Btn_Info1"
        Me.Btn_Info1.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(184, 200)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lblCode
        '
        Me.lblCode.Location = New System.Drawing.Point(16, 200)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(152, 21)
        Me.lblCode.TabIndex = 0
        '
        'frmBookInCartonExpectedOrder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.Btn_Info1)
        Me.Controls.Add(Me.btnEnterSupNo)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblSelect)
        Me.Controls.Add(Me.lvwSuppliers)
        Me.Controls.Add(Me.lblExpectedOrders)
        Me.Controls.Add(Me.lblDirRecv)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmBookInCartonExpectedOrder"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        BCSessionMgr.GetInstance().QuitSession()

    End Sub

    Private Sub lvwSuppliers_ItemActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwSuppliers.SelectedIndexChanged
        BCSessionMgr.GetInstance().SupplierSelection()
        If BCSessionMgr.GetInstance().m_strSupplierSelectionCheck = "Y" Then
#If RF Then
            If Not objAppContainer.bCommFailure And Not objAppContainer.bReconnectSuccess Then
                Me.Hide()
                Me.Dispose()
            End If
            objAppContainer.bReconnectSuccess = False
#ElseIf NRF Then
            Me.Hide()
            Me.Dispose()
#End If
            
            BCSessionMgr.GetInstance().m_strSupplierSelectionCheck = Nothing
        End If
    End Sub

    Private Sub btnEnterSupNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnterSupNo.Click

        FreezeControls()
#If RF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(lblCode, CalcPadSessionMgr.EntryTypeEnum.Supplier)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(lblCode, CalcPadSessionMgr.EntryTypeEnum.Supplier)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            BCSessionMgr.GetInstance().SupplierNoEntry(lblCode.Text)
        End If
#End If

        UnFreezeControls()


    End Sub

    Private Sub frmBookInCartonExpectedOrder_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
        objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.BOOKINCARTONEXPECTEDORDER
    End Sub

    Private Sub frmBookInCartonExpectedOrder_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click

        FreezeControls()
#If RF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(lblCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(lblCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            BCSessionMgr.GetInstance().HandleScanData(lblCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()

    End Sub

    Private Sub Btn_Info1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info1.Click
        FreezeControls()
        'Displays the help information

        MessageBox.Show(MessageManager.GetInstance().GetMessage("M71"))
        UnFreezeControls()
    End Sub

    Public Sub FreezeControls()
        Me.Btn_Quit_small1.Enabled = False
        Me.Btn_CalcPad_small1.Enabled = False
        Me.Btn_Info1.Enabled = False
        Me.btnEnterSupNo.Enabled = False
        Me.lvwSuppliers.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.Btn_Quit_small1.Enabled = True
        Me.Btn_CalcPad_small1.Enabled = True
        Me.Btn_Info1.Enabled = True
        Me.btnEnterSupNo.Enabled = True
        Me.lvwSuppliers.Enabled = True

    End Sub
End Class
