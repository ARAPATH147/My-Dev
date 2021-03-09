<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmFFHome
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
        Me.lblSELVal = New System.Windows.Forms.Label
        Me.lblSELs = New System.Windows.Forms.Label
        Me.lblItemVal = New System.Windows.Forms.Label
        Me.lblItems = New System.Windows.Forms.Label
        Me.btnView = New CustomButtons.btnView
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblUserPrompt = New System.Windows.Forms.Label
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.ProdSEL1 = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblSELVal
        '
        Me.lblSELVal.Location = New System.Drawing.Point(175, 170)
        Me.lblSELVal.Name = "lblSELVal"
        Me.lblSELVal.Size = New System.Drawing.Size(34, 20)
        '
        'lblSELs
        '
        Me.lblSELs.Location = New System.Drawing.Point(135, 170)
        Me.lblSELs.Name = "lblSELs"
        Me.lblSELs.Size = New System.Drawing.Size(43, 20)
        Me.lblSELs.Text = "SELs:"
        '
        'lblItemVal
        '
        Me.lblItemVal.Location = New System.Drawing.Point(78, 170)
        Me.lblItemVal.Name = "lblItemVal"
        Me.lblItemVal.Size = New System.Drawing.Size(34, 20)
        '
        'lblItems
        '
        Me.lblItems.Location = New System.Drawing.Point(20, 170)
        Me.lblItems.Name = "lblItems"
        Me.lblItems.Size = New System.Drawing.Size(52, 20)
        Me.lblItems.Text = "Items:"
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(90, 244)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 18
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(175, 244)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 16
        '
        'lblUserPrompt
        '
        Me.lblUserPrompt.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblUserPrompt.Location = New System.Drawing.Point(6, 203)
        Me.lblUserPrompt.Name = "lblUserPrompt"
        Me.lblUserPrompt.Size = New System.Drawing.Size(234, 32)
        Me.lblUserPrompt.Text = "Scan/Enter Product Code or Scan SEL"
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(20, 239)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 20
        '
        'ProdSEL1
        '
        Me.ProdSEL1.Location = New System.Drawing.Point(20, 25)
        Me.ProdSEL1.Name = "ProdSEL1"
        Me.ProdSEL1.Size = New System.Drawing.Size(196, 128)
        Me.ProdSEL1.TabIndex = 26
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 32
        '
        'frmFFHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.ProdSEL1)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.lblSELVal)
        Me.Controls.Add(Me.lblSELs)
        Me.Controls.Add(Me.lblItemVal)
        Me.Controls.Add(Me.lblItems)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblUserPrompt)
        Me.Name = "frmFFHome"
        Me.Text = "Fast Fill"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblSELVal As System.Windows.Forms.Label
    Friend WithEvents lblSELs As System.Windows.Forms.Label
    Friend WithEvents lblItemVal As System.Windows.Forms.Label
    Friend WithEvents lblItems As System.Windows.Forms.Label
    Friend WithEvents btnView As CustomButtons.btnView
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblUserPrompt As System.Windows.Forms.Label
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents ProdSEL1 As MCShMon.ProdSEL
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
