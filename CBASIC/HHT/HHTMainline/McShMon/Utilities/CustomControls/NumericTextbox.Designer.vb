<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class NumericTextbox
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.txtNumeric = New System.Windows.Forms.TextBox
        Me.lblNumeric = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.SuspendLayout()
        '
        'txtNumeric
        '
        Me.txtNumeric.Location = New System.Drawing.Point(0, 29)
        Me.txtNumeric.Name = "txtNumeric"
        Me.txtNumeric.Size = New System.Drawing.Size(145, 21)
        Me.txtNumeric.TabIndex = 0
        '
        'lblNumeric
        '
        Me.lblNumeric.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblNumeric.Location = New System.Drawing.Point(0, 3)
        Me.lblNumeric.Name = "lblNumeric"
        Me.lblNumeric.Size = New System.Drawing.Size(132, 20)
        Me.lblNumeric.Text = "Numeric Text Box"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(151, 26)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 4
        '
        'NumericTextbox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.txtNumeric)
        Me.Controls.Add(Me.lblNumeric)
        Me.Name = "NumericTextbox"
        Me.Size = New System.Drawing.Size(195, 54)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtNumeric As System.Windows.Forms.TextBox
    Friend WithEvents lblNumeric As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small

End Class
