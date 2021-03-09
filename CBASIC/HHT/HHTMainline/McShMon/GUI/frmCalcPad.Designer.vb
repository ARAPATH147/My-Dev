<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCalcPad
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalcPad))
        Me.pbQMark = New System.Windows.Forms.PictureBox
        Me.pbDelete = New System.Windows.Forms.PictureBox
        Me.pbQuit = New System.Windows.Forms.PictureBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.tbValue = New System.Windows.Forms.TextBox
        Me.pbMinus = New System.Windows.Forms.PictureBox
        Me.pbMultiply = New System.Windows.Forms.PictureBox
        Me.pbPlus = New System.Windows.Forms.PictureBox
        Me.tmrChecker = New System.Windows.Forms.Timer
        Me.pbZero = New System.Windows.Forms.Button
        Me.pbOne = New System.Windows.Forms.Button
        Me.pbTwo = New System.Windows.Forms.Button
        Me.pbThree = New System.Windows.Forms.Button
        Me.pbFour = New System.Windows.Forms.Button
        Me.pbFive = New System.Windows.Forms.Button
        Me.pbSix = New System.Windows.Forms.Button
        Me.pbSeven = New System.Windows.Forms.Button
        Me.pbEight = New System.Windows.Forms.Button
        Me.pbNine = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'pbQMark
        '
        Me.pbQMark.Image = CType(resources.GetObject("pbQMark.Image"), System.Drawing.Image)
        Me.pbQMark.Location = New System.Drawing.Point(182, 7)
        Me.pbQMark.Name = "pbQMark"
        Me.pbQMark.Size = New System.Drawing.Size(32, 32)
        Me.pbQMark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbDelete
        '
        Me.pbDelete.Image = CType(resources.GetObject("pbDelete.Image"), System.Drawing.Image)
        Me.pbDelete.Location = New System.Drawing.Point(75, 214)
        Me.pbDelete.Name = "pbDelete"
        Me.pbDelete.Size = New System.Drawing.Size(40, 40)
        Me.pbDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbQuit
        '
        Me.pbQuit.Image = CType(resources.GetObject("pbQuit.Image"), System.Drawing.Image)
        Me.pbQuit.Location = New System.Drawing.Point(125, 214)
        Me.pbQuit.Name = "pbQuit"
        Me.pbQuit.Size = New System.Drawing.Size(40, 40)
        Me.pbQuit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(25, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(183, 20)
        Me.Label1.Text = "Please Enter a Value"
        '
        'tbValue
        '
        Me.tbValue.Font = New System.Drawing.Font("Arial", 11.0!, System.Drawing.FontStyle.Regular)
        Me.tbValue.Location = New System.Drawing.Point(25, 42)
        Me.tbValue.Name = "tbValue"
        Me.tbValue.Size = New System.Drawing.Size(189, 23)
        Me.tbValue.TabIndex = 33
        '
        'pbMinus
        '
        Me.pbMinus.Image = CType(resources.GetObject("pbMinus.Image"), System.Drawing.Image)
        Me.pbMinus.Location = New System.Drawing.Point(175, 119)
        Me.pbMinus.Name = "pbMinus"
        Me.pbMinus.Size = New System.Drawing.Size(40, 40)
        Me.pbMinus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbMultiply
        '
        Me.pbMultiply.Image = CType(resources.GetObject("pbMultiply.Image"), System.Drawing.Image)
        Me.pbMultiply.Location = New System.Drawing.Point(175, 166)
        Me.pbMultiply.Name = "pbMultiply"
        Me.pbMultiply.Size = New System.Drawing.Size(40, 40)
        Me.pbMultiply.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'pbPlus
        '
        Me.pbPlus.Image = CType(resources.GetObject("pbPlus.Image"), System.Drawing.Image)
        Me.pbPlus.Location = New System.Drawing.Point(175, 71)
        Me.pbPlus.Name = "pbPlus"
        Me.pbPlus.Size = New System.Drawing.Size(40, 40)
        Me.pbPlus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'tmrChecker
        '
        '
        'pbZero
        '
        Me.pbZero.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbZero.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbZero.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbZero.Location = New System.Drawing.Point(25, 214)
        Me.pbZero.Name = "pbZero"
        Me.pbZero.Size = New System.Drawing.Size(40, 40)
        Me.pbZero.TabIndex = 56
        Me.pbZero.Text = "0"
        '
        'pbOne
        '
        Me.pbOne.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbOne.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbOne.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbOne.Location = New System.Drawing.Point(25, 166)
        Me.pbOne.Name = "pbOne"
        Me.pbOne.Size = New System.Drawing.Size(40, 40)
        Me.pbOne.TabIndex = 58
        Me.pbOne.Text = "1"
        '
        'pbTwo
        '
        Me.pbTwo.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbTwo.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbTwo.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbTwo.Location = New System.Drawing.Point(75, 166)
        Me.pbTwo.Name = "pbTwo"
        Me.pbTwo.Size = New System.Drawing.Size(40, 40)
        Me.pbTwo.TabIndex = 59
        Me.pbTwo.Text = "2"
        '
        'pbThree
        '
        Me.pbThree.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbThree.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbThree.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbThree.Location = New System.Drawing.Point(125, 166)
        Me.pbThree.Name = "pbThree"
        Me.pbThree.Size = New System.Drawing.Size(40, 40)
        Me.pbThree.TabIndex = 60
        Me.pbThree.Text = "3"
        '
        'pbFour
        '
        Me.pbFour.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbFour.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbFour.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbFour.Location = New System.Drawing.Point(25, 119)
        Me.pbFour.Name = "pbFour"
        Me.pbFour.Size = New System.Drawing.Size(40, 40)
        Me.pbFour.TabIndex = 61
        Me.pbFour.Text = "4"
        '
        'pbFive
        '
        Me.pbFive.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbFive.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbFive.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbFive.Location = New System.Drawing.Point(75, 119)
        Me.pbFive.Name = "pbFive"
        Me.pbFive.Size = New System.Drawing.Size(40, 40)
        Me.pbFive.TabIndex = 62
        Me.pbFive.Text = "5"
        '
        'pbSix
        '
        Me.pbSix.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbSix.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbSix.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbSix.Location = New System.Drawing.Point(125, 119)
        Me.pbSix.Name = "pbSix"
        Me.pbSix.Size = New System.Drawing.Size(40, 40)
        Me.pbSix.TabIndex = 63
        Me.pbSix.Text = "6"
        '
        'pbSeven
        '
        Me.pbSeven.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbSeven.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbSeven.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbSeven.Location = New System.Drawing.Point(25, 71)
        Me.pbSeven.Name = "pbSeven"
        Me.pbSeven.Size = New System.Drawing.Size(40, 40)
        Me.pbSeven.TabIndex = 64
        Me.pbSeven.Text = "7"
        '
        'pbEight
        '
        Me.pbEight.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbEight.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbEight.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbEight.Location = New System.Drawing.Point(75, 71)
        Me.pbEight.Name = "pbEight"
        Me.pbEight.Size = New System.Drawing.Size(40, 40)
        Me.pbEight.TabIndex = 65
        Me.pbEight.Text = "8"
        '
        'pbNine
        '
        Me.pbNine.BackColor = System.Drawing.Color.MidnightBlue
        Me.pbNine.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.pbNine.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.pbNine.Location = New System.Drawing.Point(125, 71)
        Me.pbNine.Name = "pbNine"
        Me.pbNine.Size = New System.Drawing.Size(40, 40)
        Me.pbNine.TabIndex = 66
        Me.pbNine.Text = "9"
        '
        'btnOK
        '
        Me.btnOK.Image = CType(resources.GetObject("btnOK.Image"), System.Drawing.Image)
        Me.btnOK.Location = New System.Drawing.Point(175, 214)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(40, 40)
        Me.btnOK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmCalcPad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.pbNine)
        Me.Controls.Add(Me.pbEight)
        Me.Controls.Add(Me.pbSeven)
        Me.Controls.Add(Me.pbSix)
        Me.Controls.Add(Me.pbFive)
        Me.Controls.Add(Me.pbFour)
        Me.Controls.Add(Me.pbThree)
        Me.Controls.Add(Me.pbTwo)
        Me.Controls.Add(Me.pbOne)
        Me.Controls.Add(Me.pbZero)
        Me.Controls.Add(Me.pbPlus)
        Me.Controls.Add(Me.pbMultiply)
        Me.Controls.Add(Me.pbMinus)
        Me.Controls.Add(Me.pbQMark)
        Me.Controls.Add(Me.pbDelete)
        Me.Controls.Add(Me.pbQuit)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbValue)
        Me.Name = "frmCalcPad"
        Me.Text = "CalcPad"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pbQMark As System.Windows.Forms.PictureBox
    Friend WithEvents pbDelete As System.Windows.Forms.PictureBox
    Friend WithEvents pbQuit As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents pbMinus As System.Windows.Forms.PictureBox
    Friend WithEvents pbMultiply As System.Windows.Forms.PictureBox
    Friend WithEvents pbPlus As System.Windows.Forms.PictureBox
    Public WithEvents tbValue As System.Windows.Forms.TextBox
    Friend WithEvents tmrChecker As System.Windows.Forms.Timer
    Friend WithEvents pbZero As System.Windows.Forms.Button
    Friend WithEvents pbOne As System.Windows.Forms.Button
    Friend WithEvents pbTwo As System.Windows.Forms.Button
    Friend WithEvents pbThree As System.Windows.Forms.Button
    Friend WithEvents pbFour As System.Windows.Forms.Button
    Friend WithEvents pbFive As System.Windows.Forms.Button
    Friend WithEvents pbSix As System.Windows.Forms.Button
    Friend WithEvents pbSeven As System.Windows.Forms.Button
    Friend WithEvents pbEight As System.Windows.Forms.Button
    Friend WithEvents pbNine As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.PictureBox
End Class
