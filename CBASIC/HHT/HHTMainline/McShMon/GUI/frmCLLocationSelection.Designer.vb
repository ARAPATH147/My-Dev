<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLLocationSelection
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblProductName = New System.Windows.Forms.Label
        Me.lblStatus = New System.Windows.Forms.Label
        Me.btnSalesFloor = New CustomButtons.btn_SalesFloor
        Me.lblNumSalesFloorItems = New System.Windows.Forms.Label
        Me.lblNumBackShopItems = New System.Windows.Forms.Label
        Me.lblCountLocationText = New System.Windows.Forms.Label
        Me.custCtrlBtnQuit = New CustomButtons.btn_Quit
        Me.btn_OSSR = New CustomButtons.btn_OSSR
        Me.lblNumOSSRItems = New System.Windows.Forms.Label
        Me.btnBackShop = New CustomButtons.BackShop
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblProductName
        '
        Me.lblProductName.Location = New System.Drawing.Point(17, 18)
        Me.lblProductName.Name = "lblProductName"
        Me.lblProductName.Size = New System.Drawing.Size(154, 20)
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(18, 52)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(153, 20)
        '
        'btnSalesFloor
        '
        Me.btnSalesFloor.BackColor = System.Drawing.Color.Transparent
        Me.btnSalesFloor.Location = New System.Drawing.Point(18, 89)
        Me.btnSalesFloor.Name = "btnSalesFloor"
        Me.btnSalesFloor.Size = New System.Drawing.Size(75, 21)
        Me.btnSalesFloor.TabIndex = 3
        '
        'lblNumSalesFloorItems
        '
        Me.lblNumSalesFloorItems.Location = New System.Drawing.Point(143, 90)
        Me.lblNumSalesFloorItems.Name = "lblNumSalesFloorItems"
        Me.lblNumSalesFloorItems.Size = New System.Drawing.Size(73, 20)
        Me.lblNumSalesFloorItems.Text = "0 Item"
        '
        'lblNumBackShopItems
        '
        Me.lblNumBackShopItems.Location = New System.Drawing.Point(143, 124)
        Me.lblNumBackShopItems.Name = "lblNumBackShopItems"
        Me.lblNumBackShopItems.Size = New System.Drawing.Size(73, 20)
        Me.lblNumBackShopItems.Text = "0 Item"
        '
        'lblCountLocationText
        '
        Me.lblCountLocationText.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblCountLocationText.Location = New System.Drawing.Point(17, 190)
        Me.lblCountLocationText.Name = "lblCountLocationText"
        Me.lblCountLocationText.Size = New System.Drawing.Size(198, 20)
        Me.lblCountLocationText.Text = "Select Location or Quit"
        '
        'custCtrlBtnQuit
        '
        Me.custCtrlBtnQuit.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnQuit.Location = New System.Drawing.Point(176, 213)
        Me.custCtrlBtnQuit.Name = "custCtrlBtnQuit"
        Me.custCtrlBtnQuit.Size = New System.Drawing.Size(40, 40)
        Me.custCtrlBtnQuit.TabIndex = 11
        '
        'btn_OSSR
        '
        Me.btn_OSSR.BackColor = System.Drawing.Color.Transparent
        Me.btn_OSSR.Location = New System.Drawing.Point(18, 156)
        Me.btn_OSSR.Name = "btn_OSSR"
        Me.btn_OSSR.Size = New System.Drawing.Size(76, 21)
        Me.btn_OSSR.TabIndex = 24
        Me.btn_OSSR.Visible = False
        '
        'lblNumOSSRItems
        '
        Me.lblNumOSSRItems.Location = New System.Drawing.Point(143, 157)
        Me.lblNumOSSRItems.Name = "lblNumOSSRItems"
        Me.lblNumOSSRItems.Size = New System.Drawing.Size(73, 20)
        Me.lblNumOSSRItems.Text = "0 Item"
        '
        'btnBackShop
        '
        Me.btnBackShop.BackColor = System.Drawing.Color.Transparent
        Me.btnBackShop.Location = New System.Drawing.Point(18, 123)
        Me.btnBackShop.Name = "btnBackShop"
        Me.btnBackShop.Size = New System.Drawing.Size(75, 21)
        Me.btnBackShop.TabIndex = 3
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(184, 18)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 30
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 17
        '
        'frmCLLocationSelection
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblNumOSSRItems)
        Me.Controls.Add(Me.btn_OSSR)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnQuit)
        Me.Controls.Add(Me.lblCountLocationText)
        Me.Controls.Add(Me.lblNumBackShopItems)
        Me.Controls.Add(Me.lblNumSalesFloorItems)
        Me.Controls.Add(Me.btnBackShop)
        Me.Controls.Add(Me.btnSalesFloor)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblProductName)
        Me.Name = "frmCLLocationSelection"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblProductName As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents btnSalesFloor As CustomButtons.btn_SalesFloor
    Friend WithEvents lblNumSalesFloorItems As System.Windows.Forms.Label
    Friend WithEvents lblNumBackShopItems As System.Windows.Forms.Label
    Friend WithEvents lblCountLocationText As System.Windows.Forms.Label
    Friend WithEvents custCtrlBtnQuit As CustomButtons.btn_Quit
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btn_OSSR As CustomButtons.btn_OSSR
    Friend WithEvents lblNumOSSRItems As System.Windows.Forms.Label
    Friend WithEvents btnBackShop As CustomButtons.BackShop
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
End Class
