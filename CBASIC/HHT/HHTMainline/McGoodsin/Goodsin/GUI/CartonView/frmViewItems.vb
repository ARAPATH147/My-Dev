Public Class frmViewItems
    Inherits System.Windows.Forms.Form
    Friend WithEvents lblExptDateText As System.Windows.Forms.Label
    Friend WithEvents lblBkdInStatusValue As System.Windows.Forms.Label
    Friend WithEvents lblBkdInStatusText As System.Windows.Forms.Label
    Friend WithEvents lblCartonIdValue As System.Windows.Forms.Label
    Friend WithEvents lblCartonIdText As System.Windows.Forms.Label
    Friend WithEvents lblSupplierName As System.Windows.Forms.Label
    Friend WithEvents lblExptDateValue As System.Windows.Forms.Label
    Friend WithEvents lvwItemDetails As System.Windows.Forms.ListView
    Friend WithEvents Btn_Back_sm1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblViewText As System.Windows.Forms.Label

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.lvwItemDetails.Activation = ItemActivation.OneClick
        Me.lvwItemDetails.Font = New System.Drawing.Font("Courier New", 9.0!, System.Drawing.FontStyle.Regular)
        'Add any initialization after the InitializeComponent() call

    End Sub

    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Btn_Quit_small1 As System.Windows.Forms.PictureBox
    Friend WithEvents Item As System.Windows.Forms.ColumnHeader
    Friend WithEvents Desc As System.Windows.Forms.ColumnHeader
    Friend WithEvents Qty As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewItems))
        Me.lblViewText = New System.Windows.Forms.Label
        Me.lblExptDateText = New System.Windows.Forms.Label
        Me.lblBkdInStatusValue = New System.Windows.Forms.Label
        Me.lblBkdInStatusText = New System.Windows.Forms.Label
        Me.lblCartonIdValue = New System.Windows.Forms.Label
        Me.lblCartonIdText = New System.Windows.Forms.Label
        Me.lblSupplierName = New System.Windows.Forms.Label
        Me.lblExptDateValue = New System.Windows.Forms.Label
        Me.lvwItemDetails = New System.Windows.Forms.ListView
        Me.Item = New System.Windows.Forms.ColumnHeader
        Me.Desc = New System.Windows.Forms.ColumnHeader
        Me.Qty = New System.Windows.Forms.ColumnHeader
        Me.Btn_Quit_small1 = New System.Windows.Forms.PictureBox
        Me.Btn_Back_sm1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblViewText
        '
        Me.lblViewText.Location = New System.Drawing.Point(16, 16)
        Me.lblViewText.Name = "lblViewText"
        Me.lblViewText.Size = New System.Drawing.Size(72, 16)
        Me.lblViewText.Text = "View Carton"
        '
        'lblExptDateText
        '
        Me.lblExptDateText.Location = New System.Drawing.Point(132, 80)
        Me.lblExptDateText.Name = "lblExptDateText"
        Me.lblExptDateText.Size = New System.Drawing.Size(44, 16)
        Me.lblExptDateText.Text = "Expt:"
        '
        'lblBkdInStatusValue
        '
        Me.lblBkdInStatusValue.Location = New System.Drawing.Point(63, 80)
        Me.lblBkdInStatusValue.Name = "lblBkdInStatusValue"
        Me.lblBkdInStatusValue.Size = New System.Drawing.Size(58, 16)
        Me.lblBkdInStatusValue.Text = "Label3"
        '
        'lblBkdInStatusText
        '
        Me.lblBkdInStatusText.Location = New System.Drawing.Point(16, 80)
        Me.lblBkdInStatusText.Name = "lblBkdInStatusText"
        Me.lblBkdInStatusText.Size = New System.Drawing.Size(48, 16)
        Me.lblBkdInStatusText.Text = "Status:"
        '
        'lblCartonIdValue
        '
        Me.lblCartonIdValue.Location = New System.Drawing.Point(64, 56)
        Me.lblCartonIdValue.Name = "lblCartonIdValue"
        Me.lblCartonIdValue.Size = New System.Drawing.Size(100, 16)
        Me.lblCartonIdValue.Text = "Label5"
        '
        'lblCartonIdText
        '
        Me.lblCartonIdText.Location = New System.Drawing.Point(16, 56)
        Me.lblCartonIdText.Name = "lblCartonIdText"
        Me.lblCartonIdText.Size = New System.Drawing.Size(48, 16)
        Me.lblCartonIdText.Text = "Carton:"
        '
        'lblSupplierName
        '
        Me.lblSupplierName.Location = New System.Drawing.Point(16, 34)
        Me.lblSupplierName.Name = "lblSupplierName"
        Me.lblSupplierName.Size = New System.Drawing.Size(132, 22)
        Me.lblSupplierName.Text = "Clarins"
        '
        'lblExptDateValue
        '
        Me.lblExptDateValue.Location = New System.Drawing.Point(166, 80)
        Me.lblExptDateValue.Name = "lblExptDateValue"
        Me.lblExptDateValue.Size = New System.Drawing.Size(56, 16)
        Me.lblExptDateValue.Text = "Label1"
        '
        'lvwItemDetails
        '
        Me.lvwItemDetails.Columns.Add(Me.Item)
        Me.lvwItemDetails.Columns.Add(Me.Desc)
        Me.lvwItemDetails.Columns.Add(Me.Qty)
        Me.lvwItemDetails.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lvwItemDetails.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvwItemDetails.Location = New System.Drawing.Point(12, 104)
        Me.lvwItemDetails.Name = "lvwItemDetails"
        Me.lvwItemDetails.Size = New System.Drawing.Size(216, 96)
        Me.lvwItemDetails.TabIndex = 2
        Me.lvwItemDetails.View = System.Windows.Forms.View.Details
        '
        'Item
        '
        Me.Item.Text = "Item Code"
        Me.Item.Width = 76
        '
        'Desc
        '
        Me.Desc.Text = "Desc"
        Me.Desc.Width = 180
        '
        'Qty
        '
        Me.Qty.Text = "Qty"
        Me.Qty.Width = 37
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.Image = CType(resources.GetObject("Btn_Quit_small1.Image"), System.Drawing.Image)
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(178, 216)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Btn_Back_sm1
        '
        Me.Btn_Back_sm1.Image = CType(resources.GetObject("Btn_Back_sm1.Image"), System.Drawing.Image)
        Me.Btn_Back_sm1.Location = New System.Drawing.Point(12, 216)
        Me.Btn_Back_sm1.Name = "Btn_Back_sm1"
        Me.Btn_Back_sm1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Back_sm1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmViewItems
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(240, 269)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Back_sm1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lvwItemDetails)
        Me.Controls.Add(Me.lblExptDateValue)
        Me.Controls.Add(Me.lblSupplierName)
        Me.Controls.Add(Me.lblCartonIdText)
        Me.Controls.Add(Me.lblCartonIdValue)
        Me.Controls.Add(Me.lblBkdInStatusText)
        Me.Controls.Add(Me.lblBkdInStatusValue)
        Me.Controls.Add(Me.lblExptDateText)
        Me.Controls.Add(Me.lblViewText)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmViewItems"
        Me.Text = "Goods In"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Public bItemlistPurge As Boolean = False

    Private Sub Btn_Quit_small1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Quit_small1.Click
        'VCSessionManager.GetInstance.QuitItemDetailsScreen()
        VCSessionManager.GetInstance().QuitSession()
    End Sub

    Private Sub Btn_Back_sm1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Back_sm1.Click
        FreezeControls()
        If VCSessionManager.GetInstance.bScanned = True Then
            VCSessionManager.GetInstance.bScanned = False
            VCSessionManager.GetInstance.DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewSuppliers)
        Else
            VCSessionManager.GetInstance.DisplayViewCarton(VCSessionManager.VCARTONSCREENS.ViewCarton)
        End If
        UnFreezeControls()
    End Sub

    Private Sub frmViewItems_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        BCReader.GetInstance.StopRead()
    End Sub
    Public Sub FreezeControls()
        Me.lvwItemDetails.Enabled = False
        Me.Btn_Back_sm1.Enabled = False
        Me.Btn_Quit_small1.Enabled = False
    End Sub
    Public Sub UnFreezeControls()
        Me.lvwItemDetails.Enabled = True
        Me.Btn_Back_sm1.Enabled = True
        Me.Btn_Quit_small1.Enabled = True
    End Sub
End Class
