<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmNoReports
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
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lblErrorReport = New System.Windows.Forms.Label
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.lblExitReport = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 14
        '
        'lblErrorReport
        '
        Me.lblErrorReport.Location = New System.Drawing.Point(62, 67)
        Me.lblErrorReport.Name = "lblErrorReport"
        Me.lblErrorReport.Size = New System.Drawing.Size(116, 49)
        Me.lblErrorReport.Text = "There are currently  " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "no Reports available" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "      for viewing." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(180, 227)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Ok1.TabIndex = 16
        '
        'lblExitReport
        '
        Me.lblExitReport.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblExitReport.Location = New System.Drawing.Point(20, 237)
        Me.lblExitReport.Name = "lblExitReport"
        Me.lblExitReport.Size = New System.Drawing.Size(100, 20)
        Me.lblExitReport.Text = "Exit Report"
        '
        'frmNoReports
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblExitReport)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Controls.Add(Me.lblErrorReport)
        Me.Controls.Add(Me.objStatusBar)
        Me.Name = "frmNoReports"
        Me.Text = "Reports"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblErrorReport As System.Windows.Forms.Label
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Friend WithEvents lblExitReport As System.Windows.Forms.Label
End Class
