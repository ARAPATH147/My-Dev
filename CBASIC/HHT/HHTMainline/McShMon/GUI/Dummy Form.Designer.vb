<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class Dummy_Form
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
        Me.objProduct = New MCShMon.ProductCode
        Me.objNumeric = New MCShMon.NumericTextbox
        Me.objProdSEL = New MCShMon.ProdSEL
        Me.objStatusBar = New MCShMon.CustomStatusBar
        Me.SuspendLayout()
        '
        'objProduct
        '
        Me.objProduct.Location = New System.Drawing.Point(22, 183)
        Me.objProduct.Name = "objProduct"
        Me.objProduct.Size = New System.Drawing.Size(189, 57)
        Me.objProduct.TabIndex = 4
        '
        'objNumeric
        '
        Me.objNumeric.Location = New System.Drawing.Point(22, 126)
        Me.objNumeric.Name = "objNumeric"
        Me.objNumeric.Size = New System.Drawing.Size(190, 54)
        Me.objNumeric.TabIndex = 3
        '
        'objProdSEL
        '
        Me.objProdSEL.Location = New System.Drawing.Point(22, 3)
        Me.objProdSEL.Name = "objProdSEL"
        Me.objProdSEL.Size = New System.Drawing.Size(187, 117)
        Me.objProdSEL.TabIndex = 2
        '
        'objStatusBar
        '
        Me.objStatusBar.Location = New System.Drawing.Point(0, 246)
        Me.objStatusBar.Name = "objStatusBar"
        Me.objStatusBar.Size = New System.Drawing.Size(238, 19)
        Me.objStatusBar.TabIndex = 5
        '
        'Dummy_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.objProduct)
        Me.Controls.Add(Me.objNumeric)
        Me.Controls.Add(Me.objProdSEL)
        Me.Controls.Add(Me.objStatusBar)
        Me.Name = "Dummy_Form"
        Me.Text = "Dummy_Form"
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents objProdSEL As ProdSEL
    Private WithEvents objNumeric As NumericTextbox
    Public WithEvents objProduct As ProductCode
    Public WithEvents objStatusBar As CustomStatusBar
End Class
