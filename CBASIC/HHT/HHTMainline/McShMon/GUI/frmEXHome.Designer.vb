<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmEXHome
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
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.lblLocation = New System.Windows.Forms.Label
        Me.lblBackshop = New System.Windows.Forms.Label
        Me.lblItems = New System.Windows.Forms.Label
        Me.lblItemsText = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnView = New CustomButtons.btnView
        Me.Label1 = New System.Windows.Forms.Label
        Me.ProdSEL1 = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(189, 1)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 9
        '
        'lblLocation
        '
        Me.lblLocation.Location = New System.Drawing.Point(20, 142)
        Me.lblLocation.Name = "lblLocation"
        Me.lblLocation.Size = New System.Drawing.Size(81, 20)
        Me.lblLocation.Text = "Location:"
        '
        'lblBackshop
        '
        Me.lblBackshop.Location = New System.Drawing.Point(133, 142)
        Me.lblBackshop.Name = "lblBackshop"
        Me.lblBackshop.Size = New System.Drawing.Size(76, 20)
        Me.lblBackshop.Text = "Back Shop"
        '
        'lblItems
        '
        Me.lblItems.Location = New System.Drawing.Point(20, 173)
        Me.lblItems.Name = "lblItems"
        Me.lblItems.Size = New System.Drawing.Size(81, 20)
        Me.lblItems.Text = "Items:"
        '
        'lblItemsText
        '
        Me.lblItemsText.Location = New System.Drawing.Point(133, 173)
        Me.lblItemsText.Name = "lblItemsText"
        Me.lblItemsText.Size = New System.Drawing.Size(76, 20)
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(175, 240)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 12
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(20, 240)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 24
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(3, 202)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(218, 32)
        Me.Label1.Text = "Scan/Enter Product Code or Scan SEL"
        '
        'ProdSEL1
        '
        Me.ProdSEL1.Location = New System.Drawing.Point(22, 36)
        Me.ProdSEL1.Name = "ProdSEL1"
        Me.ProdSEL1.Size = New System.Drawing.Size(196, 101)
        Me.ProdSEL1.TabIndex = 36
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 30
        '
        'frmEXHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ProdSEL1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblItemsText)
        Me.Controls.Add(Me.lblItems)
        Me.Controls.Add(Me.lblBackshop)
        Me.Controls.Add(Me.lblLocation)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Name = "frmEXHome"
        Me.Text = "Excess Stock"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents lblLocation As System.Windows.Forms.Label
    Friend WithEvents lblBackshop As System.Windows.Forms.Label
    Friend WithEvents lblItems As System.Windows.Forms.Label
    Friend WithEvents lblItemsText As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnView As CustomButtons.btnView
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents ProdSEL1 As MCShMon.ProdSEL
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
