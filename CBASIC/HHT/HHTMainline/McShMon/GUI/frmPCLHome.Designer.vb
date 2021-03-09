<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPCLHome
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
        Me.lblScanPCSEL = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.Info_button_i1 = New CustomButtons.info_button_i
        Me.ProdSEL1 = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblScanPCSEL
        '
        Me.lblScanPCSEL.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblScanPCSEL.Location = New System.Drawing.Point(23, 176)
        Me.lblScanPCSEL.Name = "lblScanPCSEL"
        Me.lblScanPCSEL.Size = New System.Drawing.Size(214, 33)
        Me.lblScanPCSEL.Text = "Scan/Enter Product Code or Scan SEL/Clearance Label"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(177, 226)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 24)
        Me.Btn_Quit_small1.TabIndex = 5
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(195, 7)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 6
        '
        'ProdSEL1
        '
        Me.ProdSEL1.Location = New System.Drawing.Point(23, 45)
        Me.ProdSEL1.Name = "ProdSEL1"
        Me.ProdSEL1.Size = New System.Drawing.Size(187, 117)
        Me.ProdSEL1.TabIndex = 8
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 10
        '
        'frmPCLHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.ProdSEL1)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.lblScanPCSEL)
        Me.Name = "frmPCLHome"
        Me.Text = "Print Clearance"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblScanPCSEL As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents ProdSEL1 As MCShMon.ProdSEL
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
