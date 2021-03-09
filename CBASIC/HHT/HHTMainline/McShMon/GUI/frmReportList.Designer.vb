<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmReportList
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
        Me.lslSelect = New System.Windows.Forms.Label
        Me.btn_Info_button_i = New CustomButtons.info_button_i
        Me.btn_Quit_small = New CustomButtons.btn_Quit_small
        Me.lstReports = New System.Windows.Forms.ListView
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lslSelect
        '
        Me.lslSelect.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lslSelect.Location = New System.Drawing.Point(4, 207)
        Me.lslSelect.Name = "lslSelect"
        Me.lslSelect.Size = New System.Drawing.Size(186, 20)
        Me.lslSelect.Text = "Select a Report to Continue"
        '
        'btn_Info_button_i
        '
        Me.btn_Info_button_i.BackColor = System.Drawing.Color.Transparent
        Me.btn_Info_button_i.Location = New System.Drawing.Point(20, 230)
        Me.btn_Info_button_i.Name = "btn_Info_button_i"
        Me.btn_Info_button_i.Size = New System.Drawing.Size(32, 32)
        Me.btn_Info_button_i.TabIndex = 2
        '
        'btn_Quit_small
        '
        Me.btn_Quit_small.BackColor = System.Drawing.Color.Transparent
        Me.btn_Quit_small.Location = New System.Drawing.Point(168, 238)
        Me.btn_Quit_small.Name = "btn_Quit_small"
        Me.btn_Quit_small.Size = New System.Drawing.Size(50, 24)
        Me.btn_Quit_small.TabIndex = 16
        '
        'lstReports
        '
        Me.lstReports.Location = New System.Drawing.Point(3, 24)
        Me.lstReports.Name = "lstReports"
        Me.lstReports.Size = New System.Drawing.Size(234, 179)
        Me.lstReports.TabIndex = 0
        Me.lstReports.View = System.Windows.Forms.View.List
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 15
        '
        'frmReportList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btn_Quit_small)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btn_Info_button_i)
        Me.Controls.Add(Me.lslSelect)
        Me.Controls.Add(Me.lstReports)
        Me.Name = "frmReportList"
        Me.Text = "Reports"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lslSelect As System.Windows.Forms.Label
    Friend WithEvents btn_Info_button_i As CustomButtons.info_button_i
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents btn_Quit_small As CustomButtons.btn_Quit_small
    Friend WithEvents lstReports As System.Windows.Forms.ListView
End Class
