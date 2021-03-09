<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class btn_SignOn
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(btn_SignOn))
        Me.PicBox = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'PicBox
        '
        Me.PicBox.Image = CType(resources.GetObject("PicBox.Image"), System.Drawing.Image)
        Me.PicBox.Location = New System.Drawing.Point(0, 0)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(80, 40)
        Me.PicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'btn_SignOn
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.PicBox)
        Me.Name = "btn_SignOn"
        Me.Size = New System.Drawing.Size(80, 40)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PicBox As System.Windows.Forms.PictureBox

End Class
