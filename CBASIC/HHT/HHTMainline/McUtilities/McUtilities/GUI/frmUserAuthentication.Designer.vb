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
        Me.pbExit = New System.Windows.Forms.PictureBox
        Me.pbEnter = New System.Windows.Forms.PictureBox
        Me.pbClr = New System.Windows.Forms.PictureBox
        Me.pbDel = New System.Windows.Forms.PictureBox
        Me.pbHelp = New System.Windows.Forms.PictureBox
        Me.tbPassword = New System.Windows.Forms.TextBox
        Me.tbUserID = New System.Windows.Forms.TextBox
        Me.lblUserID = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.pbNum9 = New System.Windows.Forms.Button
        Me.pbNum8 = New System.Windows.Forms.Button
        Me.pbNum7 = New System.Windows.Forms.Button
        Me.pbNum6 = New System.Windows.Forms.Button
        Me.pbNum5 = New System.Windows.Forms.Button
        Me.pbNum4 = New System.Windows.Forms.Button
        Me.pbNum3 = New System.Windows.Forms.Button
        Me.pbNum2 = New System.Windows.Forms.Button
        Me.pbNum1 = New System.Windows.Forms.Button
        Me.pbNum0 = New System.Windows.Forms.Button
        Me.btnTab = New System.Windows.Forms.PictureBox
        Me.objStatusBar = New McUtilities.CustomStatusBar
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pbExit
        '
        Me.pbExit.Image = CType(resources.GetObject("pbExit.Image"), System.Drawing.Image)
        Me.pbExit.Location = New System.Drawing.Point(157, 3)
        Me.pbExit.Name = "pbExit"
        Me.pbExit.Size = New System.Drawing.Size(40, 40)
        '
        'pbEnter
        '
        Me.pbEnter.Image = CType(resources.GetObject("pbEnter.Image"), System.Drawing.Image)
        Me.pbEnter.Location = New System.Drawing.Point(57, 145)
        Me.pbEnter.Name = "pbEnter"
        Me.pbEnter.Size = New System.Drawing.Size(91, 40)
        '
        'pbClr
        '
        Me.pbClr.Image = CType(resources.GetObject("pbClr.Image"), System.Drawing.Image)
        Me.pbClr.Location = New System.Drawing.Point(155, 143)
        Me.pbClr.Name = "pbClr"
        Me.pbClr.Size = New System.Drawing.Size(43, 42)
        '
        'pbDel
        '
        Me.pbDel.Image = CType(resources.GetObject("pbDel.Image"), System.Drawing.Image)
        Me.pbDel.Location = New System.Drawing.Point(158, 95)
        Me.pbDel.Name = "pbDel"
        Me.pbDel.Size = New System.Drawing.Size(40, 40)
        '
        'pbHelp
        '
        Me.pbHelp.Image = CType(resources.GetObject("pbHelp.Image"), System.Drawing.Image)
        Me.pbHelp.Location = New System.Drawing.Point(213, 7)
        Me.pbHelp.Name = "pbHelp"
        Me.pbHelp.Size = New System.Drawing.Size(27, 29)
        Me.pbHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tbPassword
        '
        Me.tbPassword.BackColor = System.Drawing.Color.White
        Me.tbPassword.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold)
        Me.tbPassword.ForeColor = System.Drawing.SystemColors.InfoText
        Me.tbPassword.Location = New System.Drawing.Point(83, 42)
        Me.tbPassword.MaxLength = 8
        Me.tbPassword.Name = "tbPassword"
        Me.tbPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbPassword.ReadOnly = True
        Me.tbPassword.Size = New System.Drawing.Size(128, 26)
        Me.tbPassword.TabIndex = 37
        '
        'tbUserID
        '
        Me.tbUserID.BackColor = System.Drawing.Color.White
        Me.tbUserID.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Bold)
        Me.tbUserID.ForeColor = System.Drawing.SystemColors.WindowFrame
        Me.tbUserID.Location = New System.Drawing.Point(83, 7)
        Me.tbUserID.MaxLength = 8
        Me.tbUserID.Name = "tbUserID"
        Me.tbUserID.ReadOnly = True
        Me.tbUserID.Size = New System.Drawing.Size(128, 26)
        Me.tbUserID.TabIndex = 55
        Me.tbUserID.Text = "99999999"
        Me.tbUserID.WordWrap = False
        '
        'lblUserID
        '
        Me.lblUserID.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblUserID.ForeColor = System.Drawing.Color.Navy
        Me.lblUserID.Location = New System.Drawing.Point(29, 13)
        Me.lblUserID.Name = "lblUserID"
        Me.lblUserID.Size = New System.Drawing.Size(57, 23)
        Me.lblUserID.Text = "User ID"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.Color.Navy
        Me.Label3.Location = New System.Drawing.Point(14, 47)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(74, 17)
        Me.Label3.Text = "Password"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Panel1.Controls.Add(Me.pbNum9)
        Me.Panel1.Controls.Add(Me.pbNum8)
        Me.Panel1.Controls.Add(Me.pbNum7)
        Me.Panel1.Controls.Add(Me.pbNum6)
        Me.Panel1.Controls.Add(Me.pbNum5)
        Me.Panel1.Controls.Add(Me.pbNum4)
        Me.Panel1.Controls.Add(Me.pbNum3)
        Me.Panel1.Controls.Add(Me.pbNum2)
        Me.Panel1.Controls.Add(Me.pbNum1)
        Me.Panel1.Controls.Add(Me.pbNum0)
        Me.Panel1.Controls.Add(Me.btnTab)
        Me.Panel1.Controls.Add(Me.pbEnter)
        Me.Panel1.Controls.Add(Me.pbExit)
        Me.Panel1.Controls.Add(Me.pbClr)
        Me.Panel1.Controls.Add(Me.pbDel)
        Me.Panel1.Location = New System.Drawing.Point(14, 79)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 190)
        '
        'pbNum9
        '
        Me.pbNum9.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum9.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum9.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum9.Location = New System.Drawing.Point(109, 6)
        Me.pbNum9.Name = "pbNum9"
        Me.pbNum9.Size = New System.Drawing.Size(40, 40)
        Me.pbNum9.TabIndex = 75
        Me.pbNum9.Text = "9"
        '
        'pbNum8
        '
        Me.pbNum8.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum8.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum8.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum8.Location = New System.Drawing.Point(58, 5)
        Me.pbNum8.Name = "pbNum8"
        Me.pbNum8.Size = New System.Drawing.Size(40, 40)
        Me.pbNum8.TabIndex = 74
        Me.pbNum8.Text = "8"
        '
        'pbNum7
        '
        Me.pbNum7.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum7.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum7.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum7.Location = New System.Drawing.Point(5, 4)
        Me.pbNum7.Name = "pbNum7"
        Me.pbNum7.Size = New System.Drawing.Size(40, 40)
        Me.pbNum7.TabIndex = 73
        Me.pbNum7.Text = "7"
        '
        'pbNum6
        '
        Me.pbNum6.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum6.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum6.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum6.Location = New System.Drawing.Point(109, 52)
        Me.pbNum6.Name = "pbNum6"
        Me.pbNum6.Size = New System.Drawing.Size(40, 40)
        Me.pbNum6.TabIndex = 72
        Me.pbNum6.Text = "6"
        '
        'pbNum5
        '
        Me.pbNum5.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum5.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum5.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum5.Location = New System.Drawing.Point(58, 51)
        Me.pbNum5.Name = "pbNum5"
        Me.pbNum5.Size = New System.Drawing.Size(40, 40)
        Me.pbNum5.TabIndex = 71
        Me.pbNum5.Text = "5"
        '
        'pbNum4
        '
        Me.pbNum4.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum4.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum4.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum4.Location = New System.Drawing.Point(5, 52)
        Me.pbNum4.Name = "pbNum4"
        Me.pbNum4.Size = New System.Drawing.Size(40, 40)
        Me.pbNum4.TabIndex = 70
        Me.pbNum4.Text = "4"
        '
        'pbNum3
        '
        Me.pbNum3.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum3.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum3.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum3.Location = New System.Drawing.Point(109, 100)
        Me.pbNum3.Name = "pbNum3"
        Me.pbNum3.Size = New System.Drawing.Size(40, 40)
        Me.pbNum3.TabIndex = 69
        Me.pbNum3.Text = "3"
        '
        'pbNum2
        '
        Me.pbNum2.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum2.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum2.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum2.Location = New System.Drawing.Point(58, 100)
        Me.pbNum2.Name = "pbNum2"
        Me.pbNum2.Size = New System.Drawing.Size(40, 40)
        Me.pbNum2.TabIndex = 68
        Me.pbNum2.Text = "2"
        '
        'pbNum1
        '
        Me.pbNum1.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum1.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum1.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum1.Location = New System.Drawing.Point(5, 100)
        Me.pbNum1.Name = "pbNum1"
        Me.pbNum1.Size = New System.Drawing.Size(40, 40)
        Me.pbNum1.TabIndex = 67
        Me.pbNum1.Text = "1"
        '
        'pbNum0
        '
        Me.pbNum0.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNum0.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNum0.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNum0.Location = New System.Drawing.Point(5, 146)
        Me.pbNum0.Name = "pbNum0"
        Me.pbNum0.Size = New System.Drawing.Size(40, 40)
        Me.pbNum0.TabIndex = 66
        Me.pbNum0.Text = "0"
        '
        'btnTab
        '
        Me.btnTab.Image = CType(resources.GetObject("btnTab.Image"), System.Drawing.Image)
        Me.btnTab.Location = New System.Drawing.Point(158, 49)
        Me.btnTab.Name = "btnTab"
        Me.btnTab.Size = New System.Drawing.Size(40, 40)
        '
        'objStatusBar
        '
        Me.objStatusBar.Location = New System.Drawing.Point(-1, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(241, 19)
        Me.objStatusBar.TabIndex = 38
        '
        'frmUserAuthentication
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.tbUserID)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.lblUserID)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.tbPassword)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.pbHelp)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmUserAuthentication"
        Me.Text = "User Sign On"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbExit As System.Windows.Forms.PictureBox
    Friend WithEvents pbEnter As System.Windows.Forms.PictureBox
    Friend WithEvents pbClr As System.Windows.Forms.PictureBox
    Friend WithEvents pbDel As System.Windows.Forms.PictureBox
    Friend WithEvents pbHelp As System.Windows.Forms.PictureBox
    Friend WithEvents tbPassword As System.Windows.Forms.TextBox
    Public WithEvents objStatusBar As McUtilities.CustomStatusBar
    Friend WithEvents tbUserID As System.Windows.Forms.TextBox
    Friend WithEvents lblUserID As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnTab As System.Windows.Forms.PictureBox
    Friend WithEvents pbNum9 As System.Windows.Forms.Button
    Friend WithEvents pbNum8 As System.Windows.Forms.Button
    Friend WithEvents pbNum7 As System.Windows.Forms.Button
    Friend WithEvents pbNum6 As System.Windows.Forms.Button
    Friend WithEvents pbNum5 As System.Windows.Forms.Button
    Friend WithEvents pbNum4 As System.Windows.Forms.Button
    Friend WithEvents pbNum3 As System.Windows.Forms.Button
    Friend WithEvents pbNum2 As System.Windows.Forms.Button
    Friend WithEvents pbNum1 As System.Windows.Forms.Button
    Friend WithEvents pbNum0 As System.Windows.Forms.Button

End Class
