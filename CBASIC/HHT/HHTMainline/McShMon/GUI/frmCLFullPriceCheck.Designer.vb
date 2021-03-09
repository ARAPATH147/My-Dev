<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmCLFullPriceCheck
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
        Me.lblPrompt2 = New System.Windows.Forms.Label
        Me.lblPrompt1 = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.btnQuit = New CustomButtons.btn_Quit_small
        Me.lblDesc3 = New System.Windows.Forms.Label
        Me.lblDesc2 = New System.Windows.Forms.Label
        Me.lblDesc1 = New System.Windows.Forms.Label
        Me.lblEAN = New System.Windows.Forms.Label
        Me.lblBtsCode = New System.Windows.Forms.Label
        Me.btnZero = New CustomButtons.btn_Zero
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'lblPrompt2
        '
        Me.lblPrompt2.Location = New System.Drawing.Point(19, 219)
        Me.lblPrompt2.Name = "lblPrompt2"
        Me.lblPrompt2.Size = New System.Drawing.Size(203, 20)
        Me.lblPrompt2.Text = "or if GAP, then press Zero Button"
        '
        'lblPrompt1
        '
        Me.lblPrompt1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblPrompt1.Location = New System.Drawing.Point(19, 201)
        Me.lblPrompt1.Name = "lblPrompt1"
        Me.lblPrompt1.Size = New System.Drawing.Size(203, 20)
        Me.lblPrompt1.Text = "Scan/Enter Product Code"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(198, 22)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 157
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.Transparent
        Me.btnQuit.Location = New System.Drawing.Point(172, 248)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(50, 24)
        Me.btnQuit.TabIndex = 155
        '
        'lblDesc3
        '
        Me.lblDesc3.Location = New System.Drawing.Point(28, 86)
        Me.lblDesc3.Name = "lblDesc3"
        Me.lblDesc3.Size = New System.Drawing.Size(97, 15)
        '
        'lblDesc2
        '
        Me.lblDesc2.Location = New System.Drawing.Point(28, 72)
        Me.lblDesc2.Name = "lblDesc2"
        Me.lblDesc2.Size = New System.Drawing.Size(97, 15)
        '
        'lblDesc1
        '
        Me.lblDesc1.Location = New System.Drawing.Point(28, 58)
        Me.lblDesc1.Name = "lblDesc1"
        Me.lblDesc1.Size = New System.Drawing.Size(97, 15)
        '
        'lblEAN
        '
        Me.lblEAN.Location = New System.Drawing.Point(28, 38)
        Me.lblEAN.Name = "lblEAN"
        Me.lblEAN.Size = New System.Drawing.Size(152, 15)
        '
        'lblBtsCode
        '
        Me.lblBtsCode.Location = New System.Drawing.Point(28, 22)
        Me.lblBtsCode.Name = "lblBtsCode"
        Me.lblBtsCode.Size = New System.Drawing.Size(126, 15)
        Me.lblBtsCode.Text = "99-99-9999"
        '
        'btnZero
        '
        Me.btnZero.BackColor = System.Drawing.Color.Transparent
        Me.btnZero.Location = New System.Drawing.Point(19, 248)
        Me.btnZero.Name = "btnZero"
        Me.btnZero.Size = New System.Drawing.Size(50, 24)
        Me.btnZero.TabIndex = 163
        '
        'objStatusBar
        '
        Me.objStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.objStatusBar.Location = New System.Drawing.Point(0, 275)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(240, 19)
        Me.objStatusBar.TabIndex = 20
        '
        'frmCLFullPriceCheck
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnZero)
        Me.Controls.Add(Me.lblPrompt2)
        Me.Controls.Add(Me.lblPrompt1)
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.lblDesc3)
        Me.Controls.Add(Me.lblDesc2)
        Me.Controls.Add(Me.lblDesc1)
        Me.Controls.Add(Me.lblEAN)
        Me.Controls.Add(Me.lblBtsCode)
        Me.Controls.Add(Me.objStatusBar)
        Me.Name = "frmCLFullPriceCheck"
        Me.Text = "Count List - SF"
        Me.ResumeLayout(False)

    End Sub
    Public WithEvents objStatusBar As MCShMon.CustomStatusBar
    Friend WithEvents lblPrompt2 As System.Windows.Forms.Label
    Friend WithEvents lblPrompt1 As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small
    Friend WithEvents btnQuit As CustomButtons.btn_Quit_small
    Friend WithEvents lblDesc3 As System.Windows.Forms.Label
    Friend WithEvents lblDesc2 As System.Windows.Forms.Label
    Friend WithEvents lblDesc1 As System.Windows.Forms.Label
    Friend WithEvents lblEAN As System.Windows.Forms.Label
    Friend WithEvents lblBtsCode As System.Windows.Forms.Label
    Friend WithEvents btnZero As CustomButtons.btn_Zero
End Class
