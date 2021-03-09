<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSPSearchPlnrHome
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.Btn_Quit_small1 = New CustomButtons.btn_Quit_small
        Me.objProdSEL = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'Info_button_i1
        '
        Me.Info_button_i1.BackColor = System.Drawing.Color.Transparent
        Me.Info_button_i1.Location = New System.Drawing.Point(199, 6)
        Me.Info_button_i1.Name = "Info_button_i1"
        Me.Info_button_i1.Size = New System.Drawing.Size(32, 32)
        Me.Info_button_i1.TabIndex = 7
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(3, 206)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(234, 30)
        Me.Label1.Text = "Scan/ Enter Product Code or Scan SEL"
        '
        'Btn_Quit_small1
        '
        Me.Btn_Quit_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Quit_small1.Location = New System.Drawing.Point(179, 240)
        Me.Btn_Quit_small1.Name = "Btn_Quit_small1"
        Me.Btn_Quit_small1.Size = New System.Drawing.Size(50, 26)
        Me.Btn_Quit_small1.TabIndex = 10
        '
        'objProdSEL
        '
        Me.objProdSEL.Location = New System.Drawing.Point(22, 49)
        Me.objProdSEL.Name = "objProdSEL"
        Me.objProdSEL.Size = New System.Drawing.Size(187, 117)
        Me.objProdSEL.TabIndex = 6
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 12
        '
        'frmSPSearchPlnrHome
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Btn_Quit_small1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Info_button_i1)
        Me.Controls.Add(Me.objProdSEL)
        Me.Name = "frmSPSearchPlnrHome"
        Me.Text = "Search Planner"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents objProdSEL As MCShMon.ProdSEL
    Friend WithEvents Info_button_i1 As CustomButtons.info_button_i
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Btn_Quit_small1 As CustomButtons.btn_Quit_small
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
