<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmSplashScreen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplashScreen))
        Me.lblStoreNo = New System.Windows.Forms.Label
        Me.lblVersion = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.lblRelease = New System.Windows.Forms.Label
        Me.lblPort = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.lbIPAddress = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblMode = New System.Windows.Forms.Label
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.SuspendLayout()
        '
        'lblStoreNo
        '
        Me.lblStoreNo.BackColor = System.Drawing.Color.White
        Me.lblStoreNo.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblStoreNo.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblStoreNo.Location = New System.Drawing.Point(9, 14)
        Me.lblStoreNo.Name = "lblStoreNo"
        Me.lblStoreNo.Size = New System.Drawing.Size(40, 15)
        Me.lblStoreNo.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.Color.White
        Me.lblVersion.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblVersion.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblVersion.Location = New System.Drawing.Point(170, 14)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(57, 15)
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.Label4.ForeColor = System.Drawing.Color.MediumBlue
        Me.Label4.Location = New System.Drawing.Point(3, 280)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(34, 10)
        Me.Label4.Text = "MODE:"
        '
        'lblRelease
        '
        Me.lblRelease.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblRelease.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblRelease.Location = New System.Drawing.Point(134, 281)
        Me.lblRelease.Name = "lblRelease"
        Me.lblRelease.Size = New System.Drawing.Size(64, 10)
        Me.lblRelease.Text = "RCL v6.00"
        '
        'lblPort
        '
        Me.lblPort.BackColor = System.Drawing.Color.White
        Me.lblPort.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.lblPort.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblPort.Location = New System.Drawing.Point(171, 266)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(25, 13)
        Me.lblPort.Text = "800"
        Me.lblPort.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.Color.White
        Me.Label7.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label7.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label7.Location = New System.Drawing.Point(134, 266)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(35, 15)
        Me.Label7.Text = "Port :"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lbIPAddress
        '
        Me.lbIPAddress.BackColor = System.Drawing.Color.White
        Me.lbIPAddress.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.lbIPAddress.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lbIPAddress.Location = New System.Drawing.Point(28, 266)
        Me.lbIPAddress.Name = "lbIPAddress"
        Me.lbIPAddress.Size = New System.Drawing.Size(100, 11)
        Me.lbIPAddress.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.White
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label3.Location = New System.Drawing.Point(0, 266)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(28, 11)
        Me.Label3.Text = "IP :"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.White
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.Label1.Location = New System.Drawing.Point(-1, 243)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(240, 22)
        Me.Label1.Text = "Please wait while the application loads..."
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblMode
        '
        Me.lblMode.Font = New System.Drawing.Font("Tahoma", 7.0!, System.Drawing.FontStyle.Regular)
        Me.lblMode.ForeColor = System.Drawing.Color.MediumBlue
        Me.lblMode.Location = New System.Drawing.Point(43, 280)
        Me.lblMode.Name = "lblMode"
        Me.lblMode.Size = New System.Drawing.Size(30, 10)
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(5, 8)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(229, 229)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        '
        'frmSplashScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblVersion)
        Me.Controls.Add(Me.lblStoreNo)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lblRelease)
        Me.Controls.Add(Me.lblPort)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.lbIPAddress)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblMode)
        Me.Controls.Add(Me.PictureBox1)
        Me.KeyPreview = True
        Me.Name = "frmSplashScreen"
        Me.Text = "Boots Store Application"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblStoreNo As System.Windows.Forms.Label
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblRelease As System.Windows.Forms.Label
    Friend WithEvents lblPort As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lbIPAddress As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblMode As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
End Class
