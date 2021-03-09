Public Class frmViewSuppliers
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwSupplerList.Activation = ItemActivation.OneClick
        Me.lvwSupplerList.Font = New System.Drawing.Font("Courier New", 9.0!, FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lblMessage1 As System.Windows.Forms.Label
    Friend WithEvents lblMessage2 As System.Windows.Forms.Label
    Friend WithEvents lvwSupplerList As System.Windows.Forms.ListView
    Friend WithEvents Btn_Info1 As System.Windows.Forms.PictureBox
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents Btn_CalcPad_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewSuppliers))
        Me.lblMessage1 = New System.Windows.Forms.Label
        Me.lblMessage2 = New System.Windows.Forms.Label
        Me.Btn_Info1 = New System.Windows.Forms.PictureBox
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.lvwSupplerList = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.Btn_CalcPad_small1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblMessage1
        '
        Me.lblMessage1.Location = New System.Drawing.Point(24, 24)
        Me.lblMessage1.Name = "lblMessage1"
        Me.lblMessage1.Size = New System.Drawing.Size(100, 16)
        Me.lblMessage1.Text = "View Carton"
        '
        'lblMessage2
        '
        Me.lblMessage2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMessage2.Location = New System.Drawing.Point(16, 152)
        Me.lblMessage2.Name = "lblMessage2"
        Me.lblMessage2.Size = New System.Drawing.Size(136, 32)
        Me.lblMessage2.Text = "Select Supplier or Scan/ Enter Carton"
        '
        'Btn_Info1
        '
        Me.Btn_Info1.Image = CType(resources.GetObject("Btn_Info1.Image"), System.Drawing.Image)
        Me.Btn_Info1.Location = New System.Drawing.Point(184, 8)
        Me.Btn_Info1.Name = "Btn_Info1"
        Me.Btn_Info1.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Info1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(168, 232)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'lvwSupplerList
        '
        Me.lvwSupplerList.Columns.Add(Me.ColumnHeader1)
        Me.lvwSupplerList.Columns.Add(Me.ColumnHeader2)
        Me.lvwSupplerList.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwSupplerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.lvwSupplerList.Location = New System.Drawing.Point(16, 48)
        Me.lvwSupplerList.Name = "lvwSupplerList"
        Me.lvwSupplerList.Size = New System.Drawing.Size(208, 80)
        Me.lvwSupplerList.TabIndex = 2
        Me.lvwSupplerList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "SupplierName"
        Me.ColumnHeader1.Width = 138
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Order Count"
        Me.ColumnHeader2.Width = 60
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(16, 192)
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.Size = New System.Drawing.Size(160, 21)
        Me.txtProductCode.TabIndex = 0
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.Image = CType(resources.GetObject("Btn_CalcPad_small1.Image"), System.Drawing.Image)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(196, 192)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(20, 23)
        Me.Btn_CalcPad_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmViewSuppliers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.lvwSupplerList)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Btn_Info1)
        Me.Controls.Add(Me.lblMessage2)
        Me.Controls.Add(Me.lblMessage1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmViewSuppliers"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Public bOverrideSelected As Boolean = False
    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        FreezeControls()
        VCSessionManager.GetInstance.QuitSession()
    End Sub
    Private Sub Btn_CalcPad_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CalcPad_small1.Click
        FreezeControls()
#If RF Then
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.PLEASE_WAIT)
        CalcPadSessionMgr.GetInstance().StartSession(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        objAppContainer.objStatusBar.SetMessage(StatusBar.MSGTYPE.EMPTY)
#ElseIf NRF Then
        Dim objSftKeyPad As New frmCalcPad(txtProductCode, CalcPadSessionMgr.EntryTypeEnum.Barcode)
        If objSftKeyPad.ShowDialog() = Windows.Forms.DialogResult.OK Then
            VCSessionManager.GetInstance().HandleScanData(txtProductCode.Text, BCType.ManualEntry)
        End If
#End If
        UnFreezeControls()
    End Sub

    Private Sub lvwSupplerList_ItemActivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwSupplerList.SelectedIndexChanged
        If bOverrideSelected Then
            VCSessionManager.GetInstance.GetSupplierListSelectedIndex()
            VCSessionManager.GetInstance.DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewCarton)
        End If
    End Sub
    Private Sub frmViewSuppliers_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance().StartRead()
    End Sub
    Private Sub frmViewSuppliers_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
        BCReader.GetInstance().StopRead()
    End Sub

    Private Sub Btn_Info1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Info1.Click
        FreezeControls()
        MessageBox.Show(MessageManager.GetInstance().GetMessage("M73"), "Help")
        UnFreezeControls()
    End Sub

    Private Sub frmViewSuppliers_GotFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.GotFocus
        If Not objAppContainer.objActiveScreen = AppContainer.ACTIVESCREEN.GINVIEWSUPPLIERS Then
            Me.Hide()
        End If
    End Sub
    Public Sub FreezeControls()
        Me.lvwSupplerList.Enabled = False
        Me.txtProductCode.Enabled = False
        Me.Btn_Info1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwSupplerList.Enabled = True
        Me.txtProductCode.Enabled = True
        Me.Btn_Info1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
