<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ProdSEL
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
        Me.txtProduct = New System.Windows.Forms.TextBox
        Me.txtSEL = New System.Windows.Forms.TextBox
        Me.lblProduct = New System.Windows.Forms.Label
        Me.lblSEL = New System.Windows.Forms.Label
        Me.Btn_CalcPad_small1 = New CustomButtons.btn_CalcPad_small
        Me.SuspendLayout()
        '
        'txtProduct
        '
        Me.txtProduct.BackColor = System.Drawing.SystemColors.Control
        Me.txtProduct.Location = New System.Drawing.Point(1, 24)
        Me.txtProduct.Name = "txtProduct"
        Me.txtProduct.ReadOnly = True
        Me.txtProduct.Size = New System.Drawing.Size(145, 21)
        Me.txtProduct.TabIndex = 0
        '
        'txtSEL
        '
        Me.txtSEL.BackColor = System.Drawing.SystemColors.Control
        Me.txtSEL.Location = New System.Drawing.Point(1, 65)
        Me.txtSEL.Name = "txtSEL"
        Me.txtSEL.ReadOnly = True
        Me.txtSEL.Size = New System.Drawing.Size(145, 21)
        Me.txtSEL.TabIndex = 1
        '
        'lblProduct
        '
        Me.lblProduct.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblProduct.Location = New System.Drawing.Point(1, 1)
        Me.lblProduct.Name = "lblProduct"
        Me.lblProduct.Size = New System.Drawing.Size(100, 20)
        Me.lblProduct.Text = "Product"
        '
        'lblSEL
        '
        Me.lblSEL.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblSEL.Location = New System.Drawing.Point(1, 48)
        Me.lblSEL.Name = "lblSEL"
        Me.lblSEL.Size = New System.Drawing.Size(133, 20)
        Me.lblSEL.Text = "SEL"
        '
        'Btn_CalcPad_small1
        '
        Me.Btn_CalcPad_small1.BackColor = System.Drawing.Color.Transparent
        Me.Btn_CalcPad_small1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular)
        Me.Btn_CalcPad_small1.Location = New System.Drawing.Point(152, 24)
        Me.Btn_CalcPad_small1.Name = "Btn_CalcPad_small1"
        Me.Btn_CalcPad_small1.Size = New System.Drawing.Size(24, 28)
        Me.Btn_CalcPad_small1.TabIndex = 14
        '
        'ProdSEL
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.Btn_CalcPad_small1)
        Me.Controls.Add(Me.txtProduct)
        Me.Controls.Add(Me.txtSEL)
        Me.Controls.Add(Me.lblProduct)
        Me.Controls.Add(Me.lblSEL)
        Me.Name = "ProdSEL"
        Me.Size = New System.Drawing.Size(191, 94)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents txtProduct As System.Windows.Forms.TextBox
    Friend WithEvents txtSEL As System.Windows.Forms.TextBox
    Friend WithEvents lblProduct As System.Windows.Forms.Label
    Friend WithEvents lblSEL As System.Windows.Forms.Label
    Friend WithEvents Btn_CalcPad_small1 As CustomButtons.btn_CalcPad_small

End Class
