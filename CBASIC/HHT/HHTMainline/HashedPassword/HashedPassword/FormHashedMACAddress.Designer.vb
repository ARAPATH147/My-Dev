<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class ClsHashedPassword
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
        Me.dbBind = New System.Windows.Forms.Panel
        Me.lblPass = New System.Windows.Forms.Label
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.lblHahedPassword = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnQuit = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.btnHashedPassword = New System.Windows.Forms.Button
        Me.txtMACAddress = New System.Windows.Forms.TextBox
        Me.dbBind.SuspendLayout()
        Me.SuspendLayout()
        '
        'dbBind
        '
        Me.dbBind.BackColor = System.Drawing.Color.White
        Me.dbBind.Controls.Add(Me.lblPass)
        Me.dbBind.Controls.Add(Me.txtPassword)
        Me.dbBind.Controls.Add(Me.Label5)
        Me.dbBind.Controls.Add(Me.lblHahedPassword)
        Me.dbBind.Controls.Add(Me.Label4)
        Me.dbBind.Controls.Add(Me.Label3)
        Me.dbBind.Controls.Add(Me.Label2)
        Me.dbBind.Controls.Add(Me.Label1)
        Me.dbBind.Controls.Add(Me.btnQuit)
        Me.dbBind.Controls.Add(Me.btnDelete)
        Me.dbBind.Controls.Add(Me.btnHashedPassword)
        Me.dbBind.Controls.Add(Me.txtMACAddress)
        Me.dbBind.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.dbBind.Location = New System.Drawing.Point(-2, 0)
        Me.dbBind.Name = "dbBind"
        Me.dbBind.Size = New System.Drawing.Size(388, 461)
        Me.dbBind.TabIndex = 0
        '
        'lblPass
        '
        Me.lblPass.AutoSize = True
        Me.lblPass.BackColor = System.Drawing.Color.DarkGreen
        Me.lblPass.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPass.Location = New System.Drawing.Point(30, 276)
        Me.lblPass.Name = "lblPass"
        Me.lblPass.Size = New System.Drawing.Size(129, 13)
        Me.lblPass.TabIndex = 14
        Me.lblPass.Text = "Hashed Password is :"
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(176, 273)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(141, 20)
        Me.txtPassword.TabIndex = 13
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(273, 302)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(13, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "ff"
        '
        'lblHahedPassword
        '
        Me.lblHahedPassword.AutoSize = True
        Me.lblHahedPassword.Location = New System.Drawing.Point(123, 276)
        Me.lblHahedPassword.Name = "lblHahedPassword"
        Me.lblHahedPassword.Size = New System.Drawing.Size(39, 13)
        Me.lblHahedPassword.TabIndex = 11
        Me.lblHahedPassword.Text = "Label5"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Label4.Location = New System.Drawing.Point(94, 43)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(223, 17)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Hashed Password Generation"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(55, 132)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(65, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Enter key:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(129, 68)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(39, 13)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = "Label2"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(214, 56)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "xsdsdsd"
        '
        'btnQuit
        '
        Me.btnQuit.BackColor = System.Drawing.Color.IndianRed
        Me.btnQuit.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnQuit.Location = New System.Drawing.Point(273, 196)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(75, 35)
        Me.btnQuit.TabIndex = 6
        Me.btnQuit.Text = "Quit"
        Me.btnQuit.UseVisualStyleBackColor = False
        '
        'btnDelete
        '
        Me.btnDelete.BackColor = System.Drawing.Color.DarkGreen
        Me.btnDelete.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDelete.Location = New System.Drawing.Point(191, 196)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(75, 35)
        Me.btnDelete.TabIndex = 5
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = False
        '
        'btnHashedPassword
        '
        Me.btnHashedPassword.BackColor = System.Drawing.Color.DarkGreen
        Me.btnHashedPassword.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnHashedPassword.Location = New System.Drawing.Point(49, 196)
        Me.btnHashedPassword.Name = "btnHashedPassword"
        Me.btnHashedPassword.Size = New System.Drawing.Size(135, 36)
        Me.btnHashedPassword.TabIndex = 4
        Me.btnHashedPassword.Text = "Generate Password"
        Me.btnHashedPassword.UseVisualStyleBackColor = False
        '
        'txtMACAddress
        '
        Me.txtMACAddress.Font = New System.Drawing.Font("Tahoma", 16.0!, System.Drawing.FontStyle.Bold)
        Me.txtMACAddress.Location = New System.Drawing.Point(132, 119)
        Me.txtMACAddress.Name = "txtMACAddress"
        Me.txtMACAddress.Size = New System.Drawing.Size(221, 33)
        Me.txtMACAddress.TabIndex = 1
        '
        'ClsHashedPassword
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(387, 462)
        Me.Controls.Add(Me.dbBind)
        Me.KeyPreview = True
        Me.Name = "ClsHashedPassword"
        Me.Text = "Hashed Password"
        Me.dbBind.ResumeLayout(False)
        Me.dbBind.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dbBind As System.Windows.Forms.Panel
    Friend WithEvents txtMACAddress As System.Windows.Forms.TextBox
    Private WithEvents btnHashedPassword As System.Windows.Forms.Button
    Friend WithEvents btnQuit As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblHahedPassword As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblPass As System.Windows.Forms.Label

End Class
