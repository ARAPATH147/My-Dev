<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmPCLHelp
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
        Me.lblHelpHeader = New System.Windows.Forms.Label
        Me.txtHelpText = New System.Windows.Forms.TextBox
        Me.Btn_Ok1 = New CustomButtons.btn_Ok
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblHelpHeader
        '
        Me.lblHelpHeader.BackColor = System.Drawing.Color.White
        Me.lblHelpHeader.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblHelpHeader.Location = New System.Drawing.Point(46, 118)
        Me.lblHelpHeader.Name = "lblHelpHeader"
        Me.lblHelpHeader.Size = New System.Drawing.Size(116, 20)
        Me.lblHelpHeader.Text = "Clearance Labels"
        Me.lblHelpHeader.Visible = False
        '
        'txtHelpText
        '
        Me.txtHelpText.BackColor = System.Drawing.Color.White
        Me.txtHelpText.Location = New System.Drawing.Point(7, 19)
        Me.txtHelpText.Multiline = True
        Me.txtHelpText.Name = "txtHelpText"
        Me.txtHelpText.ReadOnly = True
        Me.txtHelpText.Size = New System.Drawing.Size(226, 196)
        Me.txtHelpText.TabIndex = 13
        Me.txtHelpText.TabStop = False
        Me.txtHelpText.Text = "" & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & " Check that the correct Clearance Label" & Global.Microsoft.VisualBasic.ChrW(10) & " stationary is installed in the mobil" & _
            "e" & Global.Microsoft.VisualBasic.ChrW(10) & " printer before attempting to print any" & Global.Microsoft.VisualBasic.ChrW(10) & " labels." & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(10) & " Clearance Labels can only " & _
            "be printed " & Global.Microsoft.VisualBasic.ChrW(10) & " on the mobile printer."
        '
        'Btn_Ok1
        '
        Me.Btn_Ok1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_Ok1.Location = New System.Drawing.Point(89, 221)
        Me.Btn_Ok1.Name = "Btn_Ok1"
        Me.Btn_Ok1.Size = New System.Drawing.Size(40, 40)
        Me.Btn_Ok1.TabIndex = 12
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 11
        '
        'frmPCLHelp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblHelpHeader)
        Me.Controls.Add(Me.Btn_Ok1)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.txtHelpText)
        Me.Name = "frmPCLHelp"
        Me.Text = "Print Labels Help"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents Btn_Ok1 As CustomButtons.btn_Ok
    Friend WithEvents lblHelpHeader As System.Windows.Forms.Label
    Friend WithEvents txtHelpText As System.Windows.Forms.TextBox
End Class
