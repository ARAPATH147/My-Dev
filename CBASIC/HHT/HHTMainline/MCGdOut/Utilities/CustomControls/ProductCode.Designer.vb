<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ProductCode
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
        Me.txtProductCode = New System.Windows.Forms.TextBox
        Me.lblProductCode = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.SuspendLayout()
        '
        'txtProductCode
        '
        Me.txtProductCode.Location = New System.Drawing.Point(0, 34)
        Me.txtProductCode.MaxLength = 13
        Me.txtProductCode.Name = "txtProductCode"
        Me.txtProductCode.ReadOnly = True
        Me.txtProductCode.Size = New System.Drawing.Size(156, 21)
        Me.txtProductCode.TabIndex = 0
        '
        'lblProductCode
        '
        Me.lblProductCode.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblProductCode.Location = New System.Drawing.Point(0, 11)
        Me.lblProductCode.Name = "lblProductCode"
        Me.lblProductCode.Size = New System.Drawing.Size(150, 20)
        Me.lblProductCode.Text = "Product"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(162, 30)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 2
        '
        'ProductCode
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.txtProductCode)
        Me.Controls.Add(Me.lblProductCode)
        Me.Name = "ProductCode"
        Me.Size = New System.Drawing.Size(189, 76)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtProductCode As System.Windows.Forms.TextBox
    Friend WithEvents lblProductCode As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small

End Class
