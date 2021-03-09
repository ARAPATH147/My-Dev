<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmRLScan
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
        Me.btnReturn = New CustomButtons.btn_ReturnNew
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblTitle = New System.Windows.Forms.Label
        Me.btnView = New CustomButtons.btnView
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.objProduct = New MCGdOut.ProductCode
        Me.lblDesc = New System.Windows.Forms.Label
        Me.lblRclDesc = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.Transparent
        Me.btnReturn.Location = New System.Drawing.Point(6, 235)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(65, 24)
        Me.btnReturn.TabIndex = 27
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(25, 148)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(175, 20)
        Me.Label1.Text = "Scan / Enter Product "
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(180, 235)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 25
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        Me.lblTitle.Text = "Company HO Recall"
        '
        'btnView
        '
        Me.btnView.BackColor = System.Drawing.Color.Transparent
        Me.btnView.Location = New System.Drawing.Point(96, 235)
        Me.btnView.Name = "btnView"
        Me.btnView.Size = New System.Drawing.Size(58, 24)
        Me.btnView.TabIndex = 30
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 33
        '
        'objProduct
        '
        Me.objProduct.Location = New System.Drawing.Point(25, 84)
        Me.objProduct.Name = "objProduct"
        Me.objProduct.Size = New System.Drawing.Size(189, 57)
        Me.objProduct.TabIndex = 26
        '
        'lblDesc
        '
        Me.lblDesc.Location = New System.Drawing.Point(6, 38)
        Me.lblDesc.Name = "lblDesc"
        Me.lblDesc.Size = New System.Drawing.Size(224, 20)
        '
        'lblRclDesc
        '
        Me.lblRclDesc.Location = New System.Drawing.Point(6, 29)
        Me.lblRclDesc.Name = "lblRclDesc"
        Me.lblRclDesc.Size = New System.Drawing.Size(208, 18)
        '
        'frmRLScan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblRclDesc)
        Me.Controls.Add(Me.lblDesc)
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.btnView)
        Me.Controls.Add(Me.btnReturn)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.objProduct)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblTitle)
        Me.Name = "frmRLScan"
        Me.Text = "Recalls"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnReturn As CustomButtons.btn_ReturnNew
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Public WithEvents objProduct As MCGdOut.ProductCode
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents btnView As CustomButtons.btnView
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
    Friend WithEvents lblDesc As System.Windows.Forms.Label
    Friend WithEvents lblRclDesc As System.Windows.Forms.Label
End Class
