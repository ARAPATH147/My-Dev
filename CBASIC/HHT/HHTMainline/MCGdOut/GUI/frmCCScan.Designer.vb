<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCCScan
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
        Me.lblTitle = New System.Windows.Forms.Label
        Me.btnFinish = New CustomButtons.btn_Finish
        Me.lblInstruction = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.objProdSEL = New MCGdOut.ProdSEL
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.Black
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Menu Title"
        '
        'btnFinish
        '
        Me.btnFinish.BackColor = System.Drawing.Color.Transparent
        Me.btnFinish.Location = New System.Drawing.Point(85, 235)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(65, 24)
        Me.btnFinish.TabIndex = 18
        '
        'lblInstruction
        '
        Me.lblInstruction.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblInstruction.Location = New System.Drawing.Point(15, 195)
        Me.lblInstruction.Name = "lblInstruction"
        Me.lblInstruction.Size = New System.Drawing.Size(212, 20)
        Me.lblInstruction.Text = "Scan SEL or Scan / Enter Product "
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 16
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 21
        '
        'objProdSEL
        '
        Me.objProdSEL.Location = New System.Drawing.Point(25, 63)
        Me.objProdSEL.Name = "objProdSEL"
        Me.objProdSEL.Size = New System.Drawing.Size(187, 117)
        Me.objProdSEL.TabIndex = 3
        '
        'frmCCScan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnFinish)
        Me.Controls.Add(Me.lblInstruction)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.objProdSEL)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmCCScan"
        Me.Text = "Credit Claim"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents objProdSEL As MCGdOut.ProdSEL
    Friend WithEvents btnFinish As CustomButtons.btn_Finish
    Friend WithEvents lblInstruction As System.Windows.Forms.Label
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
