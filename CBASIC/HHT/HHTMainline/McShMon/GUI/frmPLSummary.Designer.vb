<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPLSummary
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
        Me.custCtrlBtnOk = New CustomButtons.btn_Ok
        Me.lblMessage = New System.Windows.Forms.Label
        Me.MainMenu2 = New System.Windows.Forms.MainMenu
        Me.lblPLStatDisplay = New System.Windows.Forms.Label
        Me.lblGapData = New System.Windows.Forms.Label
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'custCtrlBtnOk
        '
        Me.custCtrlBtnOk.BackColor = System.Drawing.Color.Transparent
        Me.custCtrlBtnOk.Location = New System.Drawing.Point(100, 207)
        Me.custCtrlBtnOk.Name = "custCtrlBtnOk"
        Me.custCtrlBtnOk.Size = New System.Drawing.Size(40, 40)
        Me.custCtrlBtnOk.TabIndex = 34
        '
        'lblMessage
        '
        Me.lblMessage.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblMessage.Location = New System.Drawing.Point(12, 175)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(205, 20)
        Me.lblMessage.Text = " Dock && Transmit"
        '
        'lblPLStatDisplay
        '
        Me.lblPLStatDisplay.Location = New System.Drawing.Point(12, 29)
        Me.lblPLStatDisplay.Name = "lblPLStatDisplay"
        Me.lblPLStatDisplay.Size = New System.Drawing.Size(205, 38)
        Me.lblPLStatDisplay.Text = "Picking List Complete"
        '
        'lblGapData
        '
        Me.lblGapData.Location = New System.Drawing.Point(39, 113)
        Me.lblGapData.Name = "lblGapData"
        Me.lblGapData.Size = New System.Drawing.Size(188, 20)
        '
        'objStatusBar
        '
        Me.objStatusBar.BackColor = System.Drawing.SystemColors.Window
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 37
        '
        'frmPLSummary
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.lblGapData)
        Me.Controls.Add(Me.custCtrlBtnOk)
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lblPLStatDisplay)
        Me.Name = "frmPLSummary"
        Me.Text = "Picking List"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents custCtrlBtnOk As CustomButtons.btn_Ok
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Private WithEvents MainMenu2 As System.Windows.Forms.MainMenu
    Friend WithEvents lblPLStatDisplay As System.Windows.Forms.Label
    Friend WithEvents lblGapData As System.Windows.Forms.Label
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
End Class
