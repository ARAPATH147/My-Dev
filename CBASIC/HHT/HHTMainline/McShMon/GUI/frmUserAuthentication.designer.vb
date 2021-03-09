<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmUserAuthentication
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUserAuthentication))
        Me.Label1 = New System.Windows.Forms.Label
        Me.pbExit = New System.Windows.Forms.PictureBox
        Me.pbClr = New System.Windows.Forms.PictureBox
        Me.pbDel = New System.Windows.Forms.PictureBox
        Me.pbHelp = New System.Windows.Forms.PictureBox
        Me.tbSignOn = New System.Windows.Forms.TextBox
        Me.pbNum0 = New System.Windows.Forms.Button
        Me.pbEnter = New System.Windows.Forms.PictureBox
        Me.pbNum1 = New System.Windows.Forms.Button
        Me.pbNum2 = New System.Windows.Forms.Button
        Me.pbNum3 = New System.Windows.Forms.Button
        Me.pbNum4 = New System.Windows.Forms.Button
        Me.pbNum5 = New System.Windows.Forms.Button
        Me.pbNum6 = New System.Windows.Forms.Button
        Me.pbNum7 = New System.Windows.Forms.Button
        Me.pbNum8 = New System.Windows.Forms.Button
        Me.pbNum9 = New System.Windows.Forms.Button
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.lblTitle = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(9, -15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(128, 20)
        Me.Label1.Text = "Enter User ID"
        '
        'pbExit
        '
        Me.pbExit.Image = CType(resources.GetObject("pbExit.Image"), System.Drawing.Image)
        Me.pbExit.Location = New System.Drawing.Point(179, 75)
        Me.pbExit.Name = "pbExit"
        Me.pbExit.Size = New System.Drawing.Size(40, 40)
        Me.pbExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbClr
        '
        Me.pbClr.Image = CType(resources.GetObject("pbClr.Image"), System.Drawing.Image)
        Me.pbClr.Location = New System.Drawing.Point(179, 169)
        Me.pbClr.Name = "pbClr"
        Me.pbClr.Size = New System.Drawing.Size(40, 40)
        Me.pbClr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbDel
        '
        Me.pbDel.Image = CType(resources.GetObject("pbDel.Image"), System.Drawing.Image)
        Me.pbDel.Location = New System.Drawing.Point(179, 121)
        Me.pbDel.Name = "pbDel"
        Me.pbDel.Size = New System.Drawing.Size(40, 40)
        Me.pbDel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbHelp
        '
        Me.pbHelp.Image = CType(resources.GetObject("pbHelp.Image"), System.Drawing.Image)
        Me.pbHelp.Location = New System.Drawing.Point(179, 32)
        Me.pbHelp.Name = "pbHelp"
        Me.pbHelp.Size = New System.Drawing.Size(32, 32)
        Me.pbHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbSignOn
        '
        Me.tbSignOn.Font = New System.Drawing.Font("Tahoma", 18.0!, System.Drawing.FontStyle.Bold)
        Me.tbSignOn.Location = New System.Drawing.Point(21, 32)
        Me.tbSignOn.MaxLength = 10
        Me.tbSignOn.Name = "tbSignOn"
        Me.tbSignOn.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbSignOn.ReadOnly = True
        Me.tbSignOn.Size = New System.Drawing.Size(143, 35)
        Me.tbSignOn.TabIndex = 37
        '
        'pbNum0
        '
        Me.pbNum0.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum0.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum0.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum0.Location = New System.Drawing.Point(21, 215)
        Me.pbNum0.Name = "pbNum0"
        Me.pbNum0.Size = New System.Drawing.Size(40, 40)
        Me.pbNum0.TabIndex = 55
        Me.pbNum0.Text = "0"
        '
        'pbEnter
        '
        Me.pbEnter.Image = CType(resources.GetObject("pbEnter.Image"), System.Drawing.Image)
        Me.pbEnter.Location = New System.Drawing.Point(75, 215)
        Me.pbEnter.Name = "pbEnter"
        Me.pbEnter.Size = New System.Drawing.Size(90, 40)
        Me.pbEnter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbNum1
        '
        Me.pbNum1.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum1.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum1.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum1.Location = New System.Drawing.Point(21, 169)
        Me.pbNum1.Name = "pbNum1"
        Me.pbNum1.Size = New System.Drawing.Size(40, 40)
        Me.pbNum1.TabIndex = 57
        Me.pbNum1.Text = "1"
        '
        'pbNum2
        '
        Me.pbNum2.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum2.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum2.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum2.Location = New System.Drawing.Point(74, 169)
        Me.pbNum2.Name = "pbNum2"
        Me.pbNum2.Size = New System.Drawing.Size(40, 40)
        Me.pbNum2.TabIndex = 58
        Me.pbNum2.Text = "2"
        '
        'pbNum3
        '
        Me.pbNum3.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum3.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum3.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum3.Location = New System.Drawing.Point(125, 169)
        Me.pbNum3.Name = "pbNum3"
        Me.pbNum3.Size = New System.Drawing.Size(40, 40)
        Me.pbNum3.TabIndex = 59
        Me.pbNum3.Text = "3"
        '
        'pbNum4
        '
        Me.pbNum4.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum4.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum4.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum4.Location = New System.Drawing.Point(21, 119)
        Me.pbNum4.Name = "pbNum4"
        Me.pbNum4.Size = New System.Drawing.Size(40, 40)
        Me.pbNum4.TabIndex = 60
        Me.pbNum4.Text = "4"
        '
        'pbNum5
        '
        Me.pbNum5.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum5.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum5.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum5.Location = New System.Drawing.Point(74, 120)
        Me.pbNum5.Name = "pbNum5"
        Me.pbNum5.Size = New System.Drawing.Size(40, 40)
        Me.pbNum5.TabIndex = 61
        Me.pbNum5.Text = "5"
        '
        'pbNum6
        '
        Me.pbNum6.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum6.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum6.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum6.Location = New System.Drawing.Point(125, 121)
        Me.pbNum6.Name = "pbNum6"
        Me.pbNum6.Size = New System.Drawing.Size(40, 40)
        Me.pbNum6.TabIndex = 62
        Me.pbNum6.Text = "6"
        '
        'pbNum7
        '
        Me.pbNum7.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum7.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum7.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum7.Location = New System.Drawing.Point(21, 73)
        Me.pbNum7.Name = "pbNum7"
        Me.pbNum7.Size = New System.Drawing.Size(40, 40)
        Me.pbNum7.TabIndex = 63
        Me.pbNum7.Text = "7"
        '
        'pbNum8
        '
        Me.pbNum8.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum8.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum8.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum8.Location = New System.Drawing.Point(74, 74)
        Me.pbNum8.Name = "pbNum8"
        Me.pbNum8.Size = New System.Drawing.Size(40, 40)
        Me.pbNum8.TabIndex = 64
        Me.pbNum8.Text = "8"
        '
        'pbNum9
        '
        Me.pbNum9.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum9.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum9.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum9.Location = New System.Drawing.Point(125, 75)
        Me.pbNum9.Name = "pbNum9"
        Me.pbNum9.Size = New System.Drawing.Size(40, 40)
        Me.pbNum9.TabIndex = 65
        Me.pbNum9.Text = "9"
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 38
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(21, 10)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(135, 22)
        Me.lblTitle.Text = "Enter User ID"
        '
        'frmUserAuthentication
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pbNum9)
        Me.Controls.Add(Me.pbNum8)
        Me.Controls.Add(Me.pbNum7)
        Me.Controls.Add(Me.pbNum6)
        Me.Controls.Add(Me.pbNum5)
        Me.Controls.Add(Me.pbNum4)
        Me.Controls.Add(Me.pbNum3)
        Me.Controls.Add(Me.pbNum2)
        Me.Controls.Add(Me.pbNum1)
        Me.Controls.Add(Me.pbNum0)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.pbExit)
        Me.Controls.Add(Me.pbEnter)
        Me.Controls.Add(Me.pbClr)
        Me.Controls.Add(Me.pbDel)
        Me.Controls.Add(Me.pbHelp)
        Me.Controls.Add(Me.tbSignOn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmUserAuthentication"
        Me.Text = "Shelf Management"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pbExit As System.Windows.Forms.PictureBox
    Friend WithEvents pbClr As System.Windows.Forms.PictureBox
    Friend WithEvents pbDel As System.Windows.Forms.PictureBox
    Friend WithEvents pbHelp As System.Windows.Forms.PictureBox
    Friend WithEvents tbSignOn As System.Windows.Forms.TextBox
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents pbNum0 As System.Windows.Forms.Button
    Friend WithEvents pbEnter As System.Windows.Forms.PictureBox
    Friend WithEvents pbNum1 As System.Windows.Forms.Button
    Friend WithEvents pbNum2 As System.Windows.Forms.Button
    Friend WithEvents pbNum3 As System.Windows.Forms.Button
    Friend WithEvents pbNum4 As System.Windows.Forms.Button
    Friend WithEvents pbNum5 As System.Windows.Forms.Button
    Friend WithEvents pbNum6 As System.Windows.Forms.Button
    Friend WithEvents pbNum7 As System.Windows.Forms.Button
    Friend WithEvents pbNum8 As System.Windows.Forms.Button
    Friend WithEvents pbNum9 As System.Windows.Forms.Button
    Friend WithEvents lblTitle As System.Windows.Forms.Label
End Class
