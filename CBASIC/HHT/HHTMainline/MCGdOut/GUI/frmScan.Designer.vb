<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmScan
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
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblTitle = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnReturn = New CustomButtons.btn_ReturnNew
        Me.btnFinish = New CustomButtons.btn_Finish
        Me.btnDestroy = New CustomButtons.btn_Destroy
        Me.objStatusBar = New MCGdOut.CustomStatusBar
        Me.objProduct = New MCGdOut.ProductCode
        Me.SuspendLayout()
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(175, 230)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 9
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.Location = New System.Drawing.Point(6, 9)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(231, 25)
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(26, 144)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(175, 20)
        Me.Label1.Text = "Scan / Enter Product "
        '
        'btnReturn
        '
        Me.btnReturn.BackColor = System.Drawing.Color.Transparent
        Me.btnReturn.Location = New System.Drawing.Point(85, 230)
        Me.btnReturn.Name = "btnReturn"
        Me.btnReturn.Size = New System.Drawing.Size(65, 24)
        Me.btnReturn.TabIndex = 13
        '
        'btnFinish
        '
        Me.btnFinish.BackColor = System.Drawing.Color.Transparent
        Me.btnFinish.Location = New System.Drawing.Point(85, 230)
        Me.btnFinish.Name = "btnFinish"
        Me.btnFinish.Size = New System.Drawing.Size(65, 24)
        Me.btnFinish.TabIndex = 14
        '
        'btnDestroy
        '
        Me.btnDestroy.BackColor = System.Drawing.Color.Transparent
        Me.btnDestroy.Location = New System.Drawing.Point(85, 230)
        Me.btnDestroy.Name = "btnDestroy"
        Me.btnDestroy.Size = New System.Drawing.Size(65, 24)
        Me.btnDestroy.TabIndex = 17
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 20
        '
        'objProduct
        '
        Me.objProduct.Location = New System.Drawing.Point(26, 74)
        Me.objProduct.Name = "objProduct"
        Me.objProduct.Size = New System.Drawing.Size(189, 57)
        Me.objProduct.TabIndex = 11
        '
        'frmScan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.objStatusBar)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.objProduct)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.btnFinish)
        Me.Controls.Add(Me.btnReturn)
        Me.Controls.Add(Me.btnDestroy)
        Me.Name = "frmScan"
        Me.Text = "Goods Out"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Public WithEvents objProduct As MCGdOut.ProductCode
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnReturn As CustomButtons.btn_ReturnNew
    Friend WithEvents btnFinish As CustomButtons.btn_Finish
    Friend WithEvents btnDestroy As CustomButtons.btn_Destroy
    Public WithEvents objStatusBar As MCGdOut.CustomStatusBar
End Class
