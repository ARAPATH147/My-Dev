<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCOLItemScan
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnView = New CustomButtons.btnView
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblItems = New System.Windows.Forms.Label
        Me.lblBackshop = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblItemtxt = New System.Windows.Forms.Label
        Me.lblSELtxt = New System.Windows.Forms.Label
        Me.ProdSEL1 = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(3, 202)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(203, 32)
        Me.Label1.Text = "Scan/Enter Product Code or Scan SEL"
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(20, 240)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 42
        Me.btnView.Visible = False
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(175, 240)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 41
        '
        'lblItems
        '
        Me.lblItems.Location = New System.Drawing.Point(20, 168)
        Me.lblItems.Name = "lblItems"
        Me.lblItems.Size = New System.Drawing.Size(43, 20)
        Me.lblItems.Text = "Items:"
        '
        'lblBackshop
        '
        Me.lblBackshop.Location = New System.Drawing.Point(133, 168)
        Me.lblBackshop.Name = "lblBackshop"
        Me.lblBackshop.Size = New System.Drawing.Size(34, 20)
        Me.lblBackshop.Text = "SELs:"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(193, 0)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 47
        '
        'lblItemtxt
        '
        Me.lblItemtxt.Location = New System.Drawing.Point(59, 168)
        Me.lblItemtxt.Name = "lblItemtxt"
        Me.lblItemtxt.Size = New System.Drawing.Size(43, 20)
        '
        'lblSELtxt
        '
        Me.lblSELtxt.Location = New System.Drawing.Point(165, 168)
        Me.lblSELtxt.Name = "lblSELtxt"
        Me.lblSELtxt.Size = New System.Drawing.Size(43, 20)
        '
        'ProdSEL1
        '
        Me.ProdSEL1.Location = New System.Drawing.Point(22, 36)
        Me.ProdSEL1.Name = "ProdSEL1"
        Me.ProdSEL1.Size = New System.Drawing.Size(196, 101)
        Me.ProdSEL1.TabIndex = 43
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 30
        '
        'frmCOLItemScan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblSELtxt)
        Me.Controls.Add(Me.lblItemtxt)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ProdSEL1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblItems)
        Me.Controls.Add(Me.lblBackshop)
        Me.Name = "frmCOLItemScan"
        Me.Text = "Create a Count List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ProdSEL1 As MCShMon.ProdSEL
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblItems As System.Windows.Forms.Label
    Friend WithEvents lblBackshop As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblItemtxt As System.Windows.Forms.Label
    Friend WithEvents lblSELtxt As System.Windows.Forms.Label
End Class
