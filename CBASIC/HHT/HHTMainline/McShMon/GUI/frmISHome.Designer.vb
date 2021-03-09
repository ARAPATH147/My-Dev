<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmISHome
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
        Me.btn_Quit_small = New CustomButtons.btn_Quit_small
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.ProdSEL1 = New MCShMon.ProdSEL
        Me.lblScanSel = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btn_Quit_small
        '
        Me.btn_Quit_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_Quit_small.Location = New System.Drawing.Point(161, 231)
        Me.btn_Quit_small.Name = "btn_Quit_small"
        Me.btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.btn_Quit_small.TabIndex = 29
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 30
        '
        'ProdSEL1
        '
        Me.ProdSEL1.Location = New System.Drawing.Point(15, 17)
        Me.ProdSEL1.Name = "ProdSEL1"
        Me.ProdSEL1.Size = New System.Drawing.Size(196, 150)
        Me.ProdSEL1.TabIndex = 31
        '
        'lblScanSel
        '
        Me.lblScanSel.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanSel.Location = New System.Drawing.Point(3, 196)
        Me.lblScanSel.Name = "lblScanSel"
        Me.lblScanSel.Size = New System.Drawing.Size(234, 32)
        Me.lblScanSel.Text = "Scan/Enter Product Code or Scan SEL"
        '
        'frmISHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblScanSel)
        Me.Controls.Add(Me.ProdSEL1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btn_Quit_small)
        Me.Name = "frmISHome"
        Me.Text = "Item Sales"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btn_Quit_small As CustomButtons.btn_Quit_small
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents ProdSEL1 As MCShMon.ProdSEL
    Friend WithEvents lblScanSel As System.Windows.Forms.Label
End Class
