<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLFinish
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
        Me.lblCountListDisplay = New System.Windows.Forms.Label
        Me.lblSalesFloor = New System.Windows.Forms.Label
        Me.lblBackShop = New System.Windows.Forms.Label
        Me.lblNumSalesFloor = New System.Windows.Forms.Label
        Me.lblNumBackShop = New System.Windows.Forms.Label
        Me.lblConfirmMessage = New System.Windows.Forms.Label
        Me.custCtrlBtnYes = New CustomButtons.btn_Yes_sm
        Me.custCtrlBtnNo = New CustomButtons.btn_No_sm_red
        Me.lblOSSRSite = New System.Windows.Forms.Label
        Me.lblNumOSSR = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.Btn_Help = New CustomButtons.btn_Info
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblCountListDisplay
        '
        Me.lblCountListDisplay.Location = New System.Drawing.Point(12, 20)
        Me.lblCountListDisplay.Name = "lblCountListDisplay"
        Me.lblCountListDisplay.Size = New System.Drawing.Size(187, 20)
        '
        'lblSalesFloor
        '
        Me.lblSalesFloor.Location = New System.Drawing.Point(37, 69)
        Me.lblSalesFloor.Name = "lblSalesFloor"
        Me.lblSalesFloor.Size = New System.Drawing.Size(78, 20)
        Me.lblSalesFloor.Text = "Sales Floor:"
        '
        'lblBackShop
        '
        Me.lblBackShop.Location = New System.Drawing.Point(37, 106)
        Me.lblBackShop.Name = "lblBackShop"
        Me.lblBackShop.Size = New System.Drawing.Size(78, 20)
        Me.lblBackShop.Text = "Back Shop:"
        '
        'lblNumSalesFloor
        '
        Me.lblNumSalesFloor.Location = New System.Drawing.Point(135, 69)
        Me.lblNumSalesFloor.Name = "lblNumSalesFloor"
        Me.lblNumSalesFloor.Size = New System.Drawing.Size(39, 20)
        Me.lblNumSalesFloor.Text = "000"
        '
        'lblNumBackShop
        '
        Me.lblNumBackShop.Location = New System.Drawing.Point(135, 106)
        Me.lblNumBackShop.Name = "lblNumBackShop"
        Me.lblNumBackShop.Size = New System.Drawing.Size(39, 20)
        Me.lblNumBackShop.Text = "000"
        '
        'lblConfirmMessage
        '
        Me.lblConfirmMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblConfirmMessage.Location = New System.Drawing.Point(3, 182)
        Me.lblConfirmMessage.Name = "lblConfirmMessage"
        Me.lblConfirmMessage.Size = New System.Drawing.Size(234, 27)
        '
        'custCtrlBtnYes
        '
        Me.custCtrlBtnYes.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnYes.Location = New System.Drawing.Point(12, 229)
        Me.custCtrlBtnYes.Name = "custCtrlBtnYes"
        Me.custCtrlBtnYes.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnYes.TabIndex = 11
        '
        'custCtrlBtnNo
        '
        Me.custCtrlBtnNo.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnNo.Location = New System.Drawing.Point(175, 229)
        Me.custCtrlBtnNo.Name = "custCtrlBtnNo"
        Me.custCtrlBtnNo.Size = New System.Drawing.Size(50, 24)
        Me.custCtrlBtnNo.TabIndex = 12
        '
        'lblOSSRSite
        '
        Me.lblOSSRSite.Location = New System.Drawing.Point(37, 146)
        Me.lblOSSRSite.Name = "lblOSSRSite"
        Me.lblOSSRSite.Size = New System.Drawing.Size(78, 20)
        Me.lblOSSRSite.Text = "OSSR Site:"
        Me.lblOSSRSite.Visible = False
        '
        'lblNumOSSR
        '
        Me.lblNumOSSR.Location = New System.Drawing.Point(135, 146)
        Me.lblNumOSSR.Name = "lblNumOSSR"
        Me.lblNumOSSR.Size = New System.Drawing.Size(39, 20)
        Me.lblNumOSSR.Text = "000"
        Me.lblNumOSSR.Visible = False
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(193, 8)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 86
        Me.Info_button_i1.Visible = False
        '
        'Btn_Help
        '
        Me.Btn_Help.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Help.Location = New System.Drawing.Point(193, 8)
        Me.Btn_Help.Name = "Btn_Help"
        Me.Btn_Help.Size = New System.Drawing.Size(32, 32)
        Me.Btn_Help.TabIndex = 96
        Me.Btn_Help.Visible = False
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 19
        '
        'frmCLFinish
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Help)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblCountListDisplay)
        Me.Controls.Add(Me.lblNumOSSR)
        Me.Controls.Add(Me.lblOSSRSite)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.custCtrlBtnNo)
        Me.Controls.Add(Me.custCtrlBtnYes)
        Me.Controls.Add(Me.lblConfirmMessage)
        Me.Controls.Add(Me.lblNumBackShop)
        Me.Controls.Add(Me.lblNumSalesFloor)
        Me.Controls.Add(Me.lblBackShop)
        Me.Controls.Add(Me.lblSalesFloor)
        Me.Name = "frmCLFinish"
        Me.Text = "Count List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblCountListDisplay As System.Windows.Forms.Label
    Friend WithEvents lblSalesFloor As System.Windows.Forms.Label
    Friend WithEvents lblBackShop As System.Windows.Forms.Label
    Friend WithEvents lblNumSalesFloor As System.Windows.Forms.Label
    Friend WithEvents lblNumBackShop As System.Windows.Forms.Label
    Friend WithEvents lblConfirmMessage As System.Windows.Forms.Label
    Friend WithEvents custCtrlBtnYes As CustomButtons.btn_Yes_sm
    Friend WithEvents custCtrlBtnNo As CustomButtons.btn_No_sm_red
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblOSSRSite As System.Windows.Forms.Label
    Friend WithEvents lblNumOSSR As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Btn_Help As CustomButtons.btn_Info
End Class
