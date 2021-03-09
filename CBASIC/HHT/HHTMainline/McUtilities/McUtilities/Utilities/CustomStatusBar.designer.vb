<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class CustomStatusBar
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub
    Public Sub New()
        MyBase.New()

        AddHandler MessageChanged, AddressOf EventMessageChanged
        'This call is required by the Windows Form Designer.
        InitializeComponent()

        SetStoreId()

    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblMessage = New System.Windows.Forms.Label
        Me.lblStoreId = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblMessage
        '
        Me.lblMessage.BackColor = System.Drawing.Color.LightGray
        Me.lblMessage.Location = New System.Drawing.Point(-1, 0)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(204, 21)
        '
        'lblStoreId
        '
        Me.lblStoreId.BackColor = System.Drawing.Color.Gray
        Me.lblStoreId.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblStoreId.ForeColor = System.Drawing.Color.Black
        Me.lblStoreId.Location = New System.Drawing.Point(202, 0)
        Me.lblStoreId.Name = "lblStoreId"
        Me.lblStoreId.Size = New System.Drawing.Size(37, 22)
        Me.lblStoreId.Text = "1190"
        '
        'CustomStatusBar
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.LightGray
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lblStoreId)
        Me.Name = "CustomStatusBar"
        Me.Size = New System.Drawing.Size(238, 19)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents lblStoreId As System.Windows.Forms.Label

End Class
