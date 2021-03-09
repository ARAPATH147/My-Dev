<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmReportDetails
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
        Me.btn_Info_button_i = New CustomButtons.info_button_i
        Me.btn_Quit_small = New CustomButtons.btn_Quit_small
        Me.tvReports = New System.Windows.Forms.TreeView
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'btn_Info_button_i
        '
        Me.btn_Info_button_i.BackColor = System.Drawing.Color.Transparent
        Me.btn_Info_button_i.Location = New System.Drawing.Point(15, 226)
        Me.btn_Info_button_i.Name = "btn_Info_button_i"
        Me.btn_Info_button_i.Size = New System.Drawing.Size(32, 32)
        Me.btn_Info_button_i.TabIndex = 1
        '
        'btn_Quit_small
        '
        Me.btn_Quit_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_Quit_small.Location = New System.Drawing.Point(168, 234)
        Me.btn_Quit_small.Name = "btn_Quit_small"
        Me.btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.btn_Quit_small.TabIndex = 2
        '
        'tvReports
        '
        Me.tvReports.Location = New System.Drawing.Point(3, 18)
        Me.tvReports.Name = "tvReports"
        Me.tvReports.Size = New System.Drawing.Size(234, 195)
        Me.tvReports.TabIndex = 3
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 16
        '
        'frmReportDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.tvReports)
        Me.Controls.Add(Me.btn_Quit_small)
        Me.Controls.Add(Me.btn_Info_button_i)
        Me.Name = "frmReportDetails"
        Me.Text = "Reports"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btn_Info_button_i As CustomButtons.info_button_i
    Friend WithEvents btn_Quit_small As CustomButtons.btn_Quit_small
    Friend WithEvents tvReports As System.Windows.Forms.TreeView
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
