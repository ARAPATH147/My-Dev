<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSpclInstructions
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
        Me.lblRecallRefrence = New System.Windows.Forms.Label
        Me.lblRecallNoText = New System.Windows.Forms.Label
        Me.lblSpcl = New System.Windows.Forms.Label
        Me.lblSpclText = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.btnNext = New CustomButtons.btn_Next_small
        Me.btnHelp = New CustomButtons.help
        Me.lblBatchNos = New System.Windows.Forms.Label
        Me.lblBatchTitle = New System.Windows.Forms.Label
        Me.lblSpclText1 = New System.Windows.Forms.Label
        Me.lblSpclText2 = New System.Windows.Forms.Label
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblRecallRefrence
        '
        Me.lblRecallRefrence.Location = New System.Drawing.Point(0, 7)
        Me.lblRecallRefrence.Name = "lblRecallRefrence"
        Me.lblRecallRefrence.Size = New System.Drawing.Size(107, 20)
        Me.lblRecallRefrence.Text = "Recall Reference :"
        '
        'lblRecallNoText
        '
        Me.lblRecallNoText.Location = New System.Drawing.Point(102, 7)
        Me.lblRecallNoText.Name = "lblRecallNoText"
        Me.lblRecallNoText.Size = New System.Drawing.Size(98, 20)
        '
        'lblSpcl
        '
        Me.lblSpcl.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSpcl.Location = New System.Drawing.Point(42, 29)
        Me.lblSpcl.Name = "lblSpcl"
        Me.lblSpcl.Size = New System.Drawing.Size(156, 20)
        Me.lblSpcl.Text = "Special Instructions"
        '
        'lblSpclText
        '
        Me.lblSpclText.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSpclText.Location = New System.Drawing.Point(7, 50)
        Me.lblSpclText.Name = "lblSpclText"
        Me.lblSpclText.Size = New System.Drawing.Size(228, 34)
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label2.Location = New System.Drawing.Point(3, 225)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(237, 18)
        Me.Label2.Text = "Action: Select Next Button to Continue"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 248)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 57
        '
        'btnNext
        '
        Me.btnNext.BackColor = System.Drawing.Color.Transparent
        Me.btnNext.Location = New System.Drawing.Point(10, 248)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(50, 24)
        Me.btnNext.TabIndex = 56
        '
        'btnHelp
        '
        Me.btnHelp.BackColor = System.Drawing.Color.Transparent
        Me.btnHelp.Location = New System.Drawing.Point(203, 3)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(32, 32)
        Me.btnHelp.TabIndex = 58
        Me.btnHelp.Visible = False
        '
        'lblBatchNos
        '
        Me.lblBatchNos.Location = New System.Drawing.Point(10, 174)
        Me.lblBatchNos.Name = "lblBatchNos"
        Me.lblBatchNos.Size = New System.Drawing.Size(220, 43)
        '
        'lblBatchTitle
        '
        Me.lblBatchTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblBatchTitle.Location = New System.Drawing.Point(42, 147)
        Me.lblBatchTitle.Name = "lblBatchTitle"
        Me.lblBatchTitle.Size = New System.Drawing.Size(164, 20)
        Me.lblBatchTitle.Text = "Batch/Licence Number(s)"
        '
        'lblSpclText1
        '
        Me.lblSpclText1.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSpclText1.Location = New System.Drawing.Point(7, 84)
        Me.lblSpclText1.Name = "lblSpclText1"
        Me.lblSpclText1.Size = New System.Drawing.Size(228, 34)
        '
        'lblSpclText2
        '
        Me.lblSpclText2.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular)
        Me.lblSpclText2.Location = New System.Drawing.Point(7, 118)
        Me.lblSpclText2.Name = "lblSpclText2"
        Me.lblSpclText2.Size = New System.Drawing.Size(228, 17)
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 7
        '
        'frmSpclInstructions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblSpclText2)
        Me.Controls.Add(Me.lblSpclText1)
        Me.Controls.Add(Me.lblBatchTitle)
        Me.Controls.Add(Me.lblBatchNos)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblSpclText)
        Me.Controls.Add(Me.lblSpcl)
        Me.Controls.Add(Me.lblRecallNoText)
        Me.Controls.Add(Me.lblRecallRefrence)
        Me.Controls.Add(Me.objStatusBar)
        Me.KeyPreview = True
        Me.Name = "frmSpclInstructions"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
    Friend WithEvents lblRecallRefrence As System.Windows.Forms.Label
    Friend WithEvents lblRecallNoText As System.Windows.Forms.Label
    Friend WithEvents lblSpcl As System.Windows.Forms.Label
    Friend WithEvents lblSpclText As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents btnNext As CustomButtons.btn_Next_small
    Friend WithEvents btnHelp As CustomButtons.help
    Friend WithEvents lblBatchNos As System.Windows.Forms.Label
    Friend WithEvents lblBatchTitle As System.Windows.Forms.Label
    Friend WithEvents lblSpclText1 As System.Windows.Forms.Label
    Friend WithEvents lblSpclText2 As System.Windows.Forms.Label
End Class
