<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmHelp
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
        Me.txtHelpText = New System.Windows.Forms.TextBox
        Me.lblTitle = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'txtHelpText
        '
        Me.txtHelpText.AcceptsReturn = True
        Me.txtHelpText.AcceptsTab = True
        Me.txtHelpText.Location = New System.Drawing.Point(8, 42)
        Me.txtHelpText.Multiline = True
        Me.txtHelpText.Name = "txtHelpText"
        Me.txtHelpText.ReadOnly = True
        Me.txtHelpText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtHelpText.Size = New System.Drawing.Size(224, 186)
        Me.txtHelpText.TabIndex = 8
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(8, 4)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(195, 35)
        Me.lblTitle.Text = "Menu Title"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 12
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 15
        '
        'frmHelp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.txtHelpText)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmHelp"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtHelpText As System.Windows.Forms.TextBox
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
