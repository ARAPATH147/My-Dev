<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRecallid
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRecallid))
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblExample = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.objNumeric = New MCGdOut.NumericTextbox
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.pbContextHelp = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 12
        '
        'lblExample
        '
        Me.lblExample.Location = New System.Drawing.Point(14, 146)
        Me.lblExample.Name = "lblExample"
        Me.lblExample.Size = New System.Drawing.Size(190, 20)
        Me.lblExample.Text = "For Example : 01234567"
        Me.lblExample.Visible = False
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(204, 25)
        Me.lblTitle.Text = "Title"
        '
        'objNumeric
        '
        Me.objNumeric.Location = New System.Drawing.Point(14, 79)
        Me.objNumeric.Name = "objNumeric"
        Me.objNumeric.Size = New System.Drawing.Size(212, 54)
        Me.objNumeric.TabIndex = 11
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 15
        '
        'pbContextHelp
        '
        Me.pbContextHelp.Image = CType(resources.GetObject("pbContextHelp.Image"), System.Drawing.Image)
        Me.pbContextHelp.Location = New System.Drawing.Point(213, 9)
        Me.pbContextHelp.Name = "pbContextHelp"
        Me.pbContextHelp.Size = New System.Drawing.Size(21, 21)
        Me.pbContextHelp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmRecallid
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.pbContextHelp)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblExample)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.objNumeric)
        Me.Name = "frmRecallid"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblExample As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents objNumeric As MCGdOut.NumericTextbox
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
    Friend WithEvents pbContextHelp As System.Windows.Forms.PictureBox
End Class
